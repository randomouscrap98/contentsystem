using System;
using System.Collections.Generic;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    /// <summary>
    /// A light wrapper around an entity and associated values.
    /// </summary>
    public class User
    {
        public Entity BaseEntity = null;
        public Dictionary<string, EntityValue> BaseValues = new Dictionary<string, EntityValue>();

        public long id { get => BaseEntity.id; }
        public DateTime createDate { get => BaseEntity.createDate; }
        public string username 
        { 
            get => BaseEntity.name;
            set => BaseEntity.name = value;
        }

        //public string email
        //{
        //    get => BaseValues[]
        //}

    }
}
