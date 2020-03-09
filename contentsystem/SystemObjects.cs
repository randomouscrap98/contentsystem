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
}
