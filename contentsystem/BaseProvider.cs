using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    public class BaseProvider
    {
        protected ILogger logger;
        protected IEntityProvider provider;
        protected IMapper mapper;
        protected Dictionary<Enum, string> keys;

        public BaseProvider(ILogger<BaseProvider> logger, IEntityProvider provider, IMapper mapper, Dictionary<Enum, string> keys)
        {
            this.logger = logger;
            this.provider = provider;
            this.mapper = mapper;
            this.keys = keys;
        }

        public async Task<Dictionary<long, List<EntityValue>>> GetSortedValues<T>(IEnumerable<T> items) where T : BaseSystemObject
        {
            var values = await provider.GetEntityValuesAsync(new EntityValueSearch()
            {
                EntityIds = items.Select(x => x.id).ToList()
            });

            var result = items.Where(x => x.id != 0).ToDictionary(x => x.id, y => new List<EntityValue>()); //new Dictionary<long, List<EntityValue>>();
            result.Add(0, new List<EntityValue>());

            foreach(var value in values)
                result[value.entityId].Add(value);

            return result;
        }

        public EntityValue GetValue(Enum id, IEnumerable<EntityValue> values)
        {
            var value = values.FirstOrDefault(x => x.key == keys[id]);

            if(value == null)
                value = new EntityValue() { key = keys[id], value = null };

            return value;
        }
    }
}