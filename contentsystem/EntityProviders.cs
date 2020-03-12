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
        public class EntityTransferData
        {
            public Action<T, string> Assign;
            public Func<T, string> Retrieve;
            public Func<S, string> FieldLike;
        }

        public BaseEntityProvider(ILogger<BaseEntityProvider<T,B,S>> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) :
            base(logger, provider, mapper, keys) {}
        
        protected Dictionary<Enum, EntityTransferData> TransferData {get;set;} = new Dictionary<Enum, EntityTransferData>();
        protected abstract SystemType EntityType {get;}
        
        public async Task<List<T>> ExpandAsync(IEnumerable<B> items)
        {
            var result = new List<T>();
            var values = await GetSortedValues(items);

            foreach(var item in items)
            {
                var converted = mapper.Map<T>(item); //All T should have a mapping from/to entity

                foreach(var transfer in TransferData)
                    transfer.Value.Assign(converted, GetValue(transfer.Key, values[item.id]).value);

                result.Add(converted);
            }

            return result;
        }

        public async Task<List<B>> GetBasicAsync(S search)
        {
            var entitySearch = mapper.Map<EntitySearch>(search);
            entitySearch.TypeLike = keys[EntityType];

            var fieldItems = new List<EntityValue>();
            bool fieldSearched = false;

            foreach(var transfer in TransferData)
            {
                var fieldSearch = transfer.Value.FieldLike(search);

                if(!string.IsNullOrEmpty(fieldSearch))
                {
                    var valueSearch = new EntityValueSearch() { KeyLike = keys[transfer.Key], ValueLike = fieldSearch };
                    fieldItems.AddRange(await provider.GetEntityValuesAsync(valueSearch));
                    fieldSearched = true;
                }
            }

            //No need to search farther: there were no matching results. You can't increase the results; searching is "and"
            if(fieldSearched && fieldItems.Count == 0)
                return new List<B>();

            entitySearch.Ids.AddRange(fieldItems.Select(x => x.entityId));

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

                foreach(var transfer in TransferData)
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
            TransferData[Identifier.Email] = new EntityTransferData()
            {
                Assign = (u, s) => u.email = s,
                Retrieve = (u) => u.email,
                FieldLike = (s) => s.EmailLike
            };
            TransferData[Identifier.Password] = new EntityTransferData()
            {
                Assign = (u, s) => u.passwordHash = s,
                Retrieve = (u) => u.passwordHash,
                FieldLike = (s) => null
            };
        }

        protected override SystemType EntityType { get => SystemType.User; }
    }
}