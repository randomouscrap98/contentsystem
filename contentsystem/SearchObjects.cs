using System;
using System.Collections.Generic;

namespace Randomous.ContentSystem
{
    //This ABSOLUTELY maps to search base because ALL "new" content objects SHOULD map directly to some entity system object.
    public class BaseSearch
    {
        public List<long> Ids = new List<long>();
        public DateTime CreateStart = new DateTime(0);
        public DateTime CreateEnd = new DateTime(0);
        public int Skip = -1;
        public int Limit = -1;
    }

    //The rest do NOT necessarily map and will need to be handled specially
    public class UserSearch : BaseSearch
    {
        public string UsernameLike = null;
        public string EmailLike = null; //This requires a value lookup first.
    }

    public class ContentSearch : BaseSearch
    {
        public string NameLike = null;
        public long Parent = 0;
        public long Owner = 0;
    }

    //These can be permissions, upvotes (!!), logging... but why give all this leeway?
    public class RelationSearch : BaseSearch
    {
        public string TypeLike = null;
        public string ValueLike = null;
    }

    //These can be keywords, extra fields in content (for like a page), etc.
    public class ValueSearch : BaseSearch
    {
        //What will ID search look like? Is this literally a direct mapping to... the entity system?
        //Just make sure you prepend something to the keys so they don't collide.
        public string KeyLike = null;
        public string ValueLike = null;
    }

    //public class CommentSearch : ContentSearch {}
    //public class CategorySearch : ContentSearch {}

    //public class PermissionSearch : BaseSearch
    //{
    //    public List<long> SubjectIds = new List<long>();
    //    public List<long> TargetIds = new List<long>();
    //    public Action HasAny = Action.None; //Only include in search if there is an action
    //}
}