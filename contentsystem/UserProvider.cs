using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class UserProvider : BaseProvider, IUserProvider
    {
        public UserProvider(ILogger<UserProvider> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys) : 
            base(logger, provider, mapper, keys) { }

        public async Task<List<User>> ExpandAsync(IEnumerable<BasicUser> items)
        {
            var result = new List<User>();
            var values = await GetSortedValues(items);

            foreach(var item in items)
            {
                var user = mapper.Map<User>(item);
                user.email = GetValue(Identifier.Email, values[item.id]).value;
                user.passwordHash = GetValue(Identifier.Password, values[item.id]).value;
                result.Add(user);
            }

            return result;
        }

        public async Task<List<BasicUser>> GetBasicAsync(UserSearch search)
        {
            var entitySearch = mapper.Map<EntitySearch>(search);
            entitySearch.TypeLike = keys[SystemType.User];

            if(!string.IsNullOrEmpty(search.EmailLike))
            {
                var valueSearch = new EntityValueSearch() { KeyLike = keys[Identifier.Email], ValueLike = search.EmailLike };
                var emailUsers = await provider.GetEntityValuesAsync(valueSearch);
                entitySearch.Ids.AddRange(emailUsers.Select(x => x.entityId));
            }

            var results = await provider.GetEntitiesAsync(entitySearch);
            return results.Select(x => mapper.Map<BasicUser>(x)).ToList();
        }

        public async Task WriteAsync(IEnumerable<User> items, bool writeIds = true)
        {
            var values = await GetSortedValues(items); //Get the OLD values.
            var storeValues = new List<EntityValue>();
            var storeUsers = new List<Tuple<User, Entity>>();

            foreach(var user in items)
            {
                //Update values or add new ones as necessary
                var email = GetValue(Identifier.Email, values[user.id]);
                email.value = user.email;
                storeValues.Add(email);

                var password = GetValue(Identifier.Password, values[user.id]);
                password .value = user.passwordHash;
                storeValues.Add(password);

                var entity = mapper.Map<Entity>(user);
                entity.type = keys[SystemType.User];
                storeUsers.Add(Tuple.Create(user, entity));
            }

            await provider.WriteAsync(storeUsers.Select(x => x.Item2));

            if(writeIds)
                storeUsers.ForEach(x => x.Item1.id = x.Item2.id);

            await provider.WriteAsync(storeValues);
        }
    }
}