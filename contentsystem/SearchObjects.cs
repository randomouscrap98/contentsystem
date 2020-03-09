using System;
using System.Collections.Generic;

namespace Randomous.ContentSystem
{
    //This ABSOLUTELY maps to search base because ALL "new" content objects SHOULD map directly to some entity system object.
    public class BaseSearch : EntitySystem.EntitySearchBase
    {
        //public List<long> Ids = new List<long>();
        //public DateTime CreateStart = new DateTime(0);
        //public DateTime CreateEnd = new DateTime(0);
        //public int Skip = -1;
        //public int Limit = -1;
    }

    //The rest do NOT necessarily map and will need to be handled specially
    public class UserSearch : BaseSearch
    {
        public string UsernameLike = null;
        public string EmailLike = null; //This requires a value lookup first.
    }

    public class PermissionSearch : BaseSearch
    {
        public List<long> SubjectIds = new List<long>();
        public List<long> TargetIds = new List<long>();
        public Action HasAny = Action.None; //Only include in search if there is an action
    }
}