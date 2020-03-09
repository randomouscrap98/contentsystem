using System;
using System.Collections.Generic;

namespace Randomous.ContentSystem
{
    //This should look familiar
    public class BaseSearch
    {
        public List<long> Ids = new List<long>();
        public DateTime CreateStart = new DateTime(0);
        public DateTime CreateEnd = new DateTime(0);
    }

    public class UserSearch : BaseSearch
    {
        public string UsernameLike = null;
    }

    public class PermissionSearch : BaseSearch
    {
        public List<long> SubjectIds = new List<long>();
        public List<long> TargetIds = new List<long>();
        public Action HasAny = Action.None; //Only include in search if there is an action
    }
}