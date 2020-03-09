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
            var results = await provider.GetEntitiesAsync(entitySearch);
            return results.Select(x => mapper.Map<BasicUser>(x)).ToList();
        }

        public async Task WriteAsync(IEnumerable<User> items)
        {
            var values = await GetSortedValues(items); //Get the OLD values.
            var storeValues = new List<EntityValue>();
            var storeUsers = new List<Entity>();

            foreach(var user in items)
            {
                //Update values or add new ones as necessary
                var email = GetValue(Identifier.Email, values[user.id]);
                email.value = user.email;
                storeValues.Add(email);

                var password = GetValue(Identifier.Password, values[user.id]);
                password .value = user.passwordHash;
                storeValues.Add(password);

                storeUsers.Add(mapper.Map<Entity>(user));
            }

            await provider.WriteAsync(storeUsers);
            await provider.WriteAsync(storeValues);
        }
    }
}