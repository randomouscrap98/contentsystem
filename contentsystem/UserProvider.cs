using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class UserProvider : IUserProvider
    {
        protected ILogger logger;
        protected IEntityProvider provider;
        protected IMapper mapper;

        public UserProvider(ILogger<UserProvider> logger, IEntityProvider provider, IMapper mapper)
        {
            this.logger = logger;
            this.provider = provider;
            this.mapper = mapper;
        }

        public async Task<List<User>> ExpandAsync(IEnumerable<BasicUser> items)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<BasicUser>> GetBasicAsync(UserSearch search)
        {
            var entitySearch = mapper.Map<EntitySearch>(search);
            var results = await provider.GetEntitiesAsync(entitySearch);
            return results.Select(x => mapper.Map<BasicUser>(x)).ToList();
        }

        public async Task WriteAsync(IEnumerable<User> items)
        {
            throw new System.NotImplementedException();
        }
    }
}