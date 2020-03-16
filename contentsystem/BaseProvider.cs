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
    /// The very most baseline provider. Only provides what every single provider might want.
    /// </summary>
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

        /// <summary>
        /// Retrieve relations that "own" the given entities (one to many, us being one of the many). For instance, if
        /// a user owns content, the content id will be entity2, and that is what we get.
        /// </summary>
        /// <param name="items"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<Dictionary<long, List<EntityRelation>>> GetSortedRelations<T>(IEnumerable<T> items) where T : BaseSystemObject
        {
            var relations = await provider.GetEntityRelationsAsync(new EntityRelationSearch()
            {
                EntityIds2 = items.Select(x => x.id).ToList()
            });

            var result = items.Where(x => x.id != 0).ToDictionary(x => x.id, y => new List<EntityRelation>());
            result.Add(0, new List<EntityRelation>());

            foreach(var relation in relations)
                result[relation.entityId2].Add(relation);

            return result;
        }

        public async Task<Dictionary<long, List<EntityValue>>> GetSortedValues<T>(IEnumerable<T> items) where T : BaseSystemObject
        {
            var values = await provider.GetEntityValuesAsync(new EntityValueSearch()
            {
                EntityIds = items.Select(x => x.id).ToList()
            });

            var result = items.Where(x => x.id != 0).ToDictionary(x => x.id, y => new List<EntityValue>());
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

        public EntityRelation GetRelation(Enum id, IEnumerable<EntityRelation> relations)
        {
            var relation = relations.FirstOrDefault(x => x.type == keys[id]);

            if(relation == null)
                relation = new EntityRelation() { type = keys[id], value = null };

            return relation;
        }
    }
}