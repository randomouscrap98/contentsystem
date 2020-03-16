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
        public class SingleTransferDescriptor<V>
        {
            public Action<T, V> Assign;
            public Func<T, V> Retrieve;
            public Func<S, V> SearchRetrieve;
        };

        public BaseEntityProvider(ILogger<BaseEntityProvider<T,B,S>> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) :
            base(logger, provider, mapper, keys) {}
        
        protected Dictionary<Enum, SingleTransferDescriptor<string>> TransferValues {get;set;} = new Dictionary<Enum, SingleTransferDescriptor<string>>();
        protected Dictionary<Enum, SingleTransferDescriptor<long>> TransferRelations {get;set;} = new Dictionary<Enum, SingleTransferDescriptor<long>>();
        protected abstract SystemType EntityType {get;}
        
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
            var values = await GetSortedValues(items); //Get the OLD values.
            var storeItems = new List<Tuple<T, Entity, List<EntityValue>>>();

            foreach(var item in items)
            {
                var entity = mapper.Map<Entity>(item);
                entity.type = keys[EntityType];
                var storeItem = Tuple.Create(item, entity, new List<EntityValue>());

                foreach(var transfer in TransferValues)
                {
                    //Update values or add new ones as necessary
                    var value = GetValue(transfer.Key, values[item.id]);
                    value.value = transfer.Value.Retrieve(item);
                    storeItem.Item3.Add(value);
                }

                storeItems.Add(storeItem);
            }

            await provider.WriteAsync(storeItems.Select(x => x.Item2));

            //Update the ids and stuff. This is getting ridiculous
            foreach(var item in storeItems)
            {
                if(writeIds)
                    item.Item1.id = item.Item2.id;
                item.Item3.ForEach(x => x.entityId = item.Item2.id);
            }

            await provider.WriteAsync(storeItems.SelectMany(x => x.Item3));
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
}