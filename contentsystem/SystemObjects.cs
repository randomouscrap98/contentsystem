using System;
using System.Collections.Generic;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    //These are all PODOs, don't worry about complexity for conversion. You are micro-optimizing.
    public class BaseSystemObject
    {
        public long id = 0;
        public DateTime createDate = DateTime.Now;
    }

    public class BasicUser : BaseSystemObject
    {
        public string username;
    }

    public class User : BasicUser
    {
        public string email;
        public string passwordHash;
    }

    public class BasicPermission : BaseSystemObject
    {
        long subject;
        long target;
        Action permissions;
    }

    //public class BaseWrapper
    //{
    //    public List<Entity> Entities = new List<Entity>();
    //    public List<EntityValue> EntityValues = new List<EntityValue>();
    //    public List<EntityRelation> EntityRelations = new List<EntityRelation>();


    //}

    ///// <summary>
    ///// A light wrapper around an entity and associated values.
    ///// </summary>
    //public class User : BaseWrapper
    //{
    //    public Entity BaseEntity = null;
    //    public Dictionary<string, EntityValue> BaseValues = new Dictionary<string, EntityValue>();

    //    public long id { get => BaseEntity.id; }
    //    public DateTime createDate { get => BaseEntity.createDate; }
    //    public string username 
    //    { 
    //        get => BaseEntity.name;
    //        set => BaseEntity.name = value;
    //    }

    //    //public string email
    //    //{
    //    //    get => BaseValues[]
    //    //}

    //}
}
