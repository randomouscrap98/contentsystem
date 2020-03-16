using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    /// <summary>
    /// Entity provider baseline. Any object that maps directly to an entity (plus values) should use this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class BaseEntityProvider<T, B, S> : BaseProvider where T : BaseSystemObject where B : BaseSystemObject where S : BaseSearch
    {
        /// <summary>
        /// Each entity provider only differs by the values and relations mapping. This class describes those mappings
        /// </summary>
        /// <typeparam name="V"></typeparam>
        public class SingleTransferDescriptor<V>
        {
            public Action<T, V> Assign;
            public Func<T, V> Retrieve;
            public Func<S, V> SearchRetrieve;
        };

        public BaseEntityProvider(ILogger<BaseEntityProvider<T,B,S>> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) :
            base(logger, provider, mapper, keys) {}
        
        //Each entity has associated values and relations. They are SINGULAR, meaning just ONE type of every relation and value.
        //For instance, an entity might be "owned" by someone, but only ONE someone. That someone could own many entities, but
        //since this entity is only owned by one person, we have the ONE relationship on our (entity) side.
        protected Dictionary<Enum, SingleTransferDescriptor<string>> TransferValues {get;set;} = new Dictionary<Enum, SingleTransferDescriptor<string>>();
        protected Dictionary<Enum, SingleTransferDescriptor<long>> TransferRelations {get;set;} = new Dictionary<Enum, SingleTransferDescriptor<long>>();
        protected abstract SystemType EntityType {get;}
        
        /// <summary>
        /// Expand a basic entity using the value and relation mapping descriptors
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<List<T>> ExpandAsync(IEnumerable<B> items)
        {
            var result = new List<T>();
            var values = await GetSortedValues(items);
            var relations = await GetSortedRelations(items);

            foreach(var item in items)
            {
                var converted = mapper.Map<T>(item); //All T should have a mapping from/to entity

                foreach(var transfer in TransferValues)
                    transfer.Value.Assign(converted, GetValue(transfer.Key, values[item.id]).value);

                foreach(var transfer in TransferRelations)
                    transfer.Value.Assign(converted, GetRelation(transfer.Key, relations[item.id]).entityId1);

                result.Add(converted);
            }

            return result;
        }

        /// <summary>
        /// A helper function: when searching for values/relations, you must and the results every time.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newThings"></param>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public List<V> AndedSearch<V>(IEnumerable<V> original, IEnumerable<V> newThings)
        {
            if(original.Count() == 0)
                return newThings.ToList();
            else
                return original.Intersect(newThings).ToList();
        }

        public async Task<List<B>> GetBasicAsync(S search)
        {
            var entitySearch = mapper.Map<EntitySearch>(search);
            entitySearch.TypeLike = keys[EntityType];

            List<long> searchIds = new List<long>();

            foreach(var transfer in TransferValues)
            {
                var fieldSearch = transfer.Value.SearchRetrieve(search);

                if(!string.IsNullOrEmpty(fieldSearch))
                {
                    var valueSearch = new EntityValueSearch() { KeyLike = keys[transfer.Key], ValueLike = fieldSearch };
                    searchIds = AndedSearch(searchIds, (await provider.GetEntityValuesAsync(valueSearch)).Select(x => x.entityId));
                    if(searchIds.Count == 0)
                        return new List<B>();
                }
            }

            foreach(var transfer in TransferRelations)
            {
                var fieldSearch = transfer.Value.SearchRetrieve(search);

                if(fieldSearch > 0)
                {
                    var relationSearch = new EntityRelationSearch() { TypeLike = keys[transfer.Key] };
                    relationSearch.EntityIds1.Add(fieldSearch);
                    searchIds = AndedSearch(searchIds, (await provider.GetEntityRelationsAsync(relationSearch)).Select(x => x.entityId2));
                    if(searchIds.Count == 0)
                        return new List<B>();
                }
            }

            entitySearch.Ids.AddRange(searchIds);

            var results = await provider.GetEntitiesAsync(entitySearch);
            return results.Select(x => mapper.Map<B>(x)).ToList();
        }

        public async Task WriteAsync(IEnumerable<T> items, bool writeIds = true)
        {
            var relations = await GetSortedRelations(items); //Get the OLD relations
            var values = await GetSortedValues(items);       //Get the OLD values.
            var storeItems = new List<Tuple<T, Entity, List<EntityValue>, List<EntityRelation>>>();

            foreach(var item in items)
            {
                var entity = mapper.Map<Entity>(item);
                entity.type = keys[EntityType];
                var storeItem = Tuple.Create(item, entity, new List<EntityValue>(), new List<EntityRelation>());

                foreach(var transfer in TransferValues)
                {
                    //Update values or add new ones as necessary
                    var value = GetValue(transfer.Key, values[item.id]);
                    value.value = transfer.Value.Retrieve(item);
                    storeItem.Item3.Add(value);
                }

                foreach(var transfer in TransferRelations)
                {
                    //Update relations or add new ones as necessary
                    var relation = GetRelation(transfer.Key, relations[item.id]);
                    relation.entityId1 = transfer.Value.Retrieve(item);
                    storeItem.Item4.Add(relation);
                }

                storeItems.Add(storeItem);
            }

            await provider.WriteAsync(storeItems.Select(x => x.Item2));

            foreach(var item in storeItems)
            {
                if(writeIds)
                    item.Item1.id = item.Item2.id;
                
                //All relations are OWNERSHIP relations... or... will that not really work?
                //Yes, relationships like this will work because 1-1 relations are VALUES. ONE value to ONE entity. The relations
                //table is for 1-many or many-many. 1-1 entities should PROBABLY just be values... unless something special comes up.
                item.Item3.ForEach(x => x.entityId = item.Item2.id);
                item.Item4.ForEach(x => x.entityId2 = item.Item2.id);
            }
            //Update the ids and stuff. This is getting ridiculous

            await provider.WriteAsync(storeItems.SelectMany(x => x.Item3));
            await provider.WriteAsync(storeItems.SelectMany(x => x.Item4));
        }
    }

    /// <summary>
    /// The entire user provider interface. Users are entities
    /// </summary>
    public class UserProvider : BaseEntityProvider<User, BasicUser, UserSearch> , IUserProvider
    {
        public UserProvider(ILogger<UserProvider> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) : 
            base(logger, provider, mapper, keys) 
        { 
            TransferValues[Identifier.Email] = new SingleTransferDescriptor<string>()
            {
                Assign = (u, s) => u.email = s,
                Retrieve = (u) => u.email,
                SearchRetrieve = (s) => s.EmailLike
            };
            TransferValues[Identifier.Password] = new SingleTransferDescriptor<string>()
            {
                Assign = (u, s) => u.passwordHash = s,
                Retrieve = (u) => u.passwordHash,
                SearchRetrieve = (s) => null
            };
        }

        protected override SystemType EntityType { get => SystemType.User; }
    }

    /// <summary>
    /// The entire user provider interface. Users are entities
    /// </summary>
    public class ContentProvider: BaseEntityProvider<Content, BasicContent, ContentSearch> , IContentProvider
    {
        public ContentProvider(ILogger<ContentProvider> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) : 
            base(logger, provider, mapper, keys) 
        { 
            TransferRelations[Identifier.Parent] = new SingleTransferDescriptor<long>()
            {
                Assign = (c, l) => c.parent = l,
                Retrieve = (c) => c.parent,
                SearchRetrieve = (s) => s.Parent
            };
            TransferRelations[Identifier.Owner] = new SingleTransferDescriptor<long>()
            {
                Assign = (c, l) => c.owner = l,
                Retrieve = (c) => c.owner,
                SearchRetrieve = (s) => s.Owner
            };
        }

        protected override SystemType EntityType { get => SystemType.Content; }
    }
}