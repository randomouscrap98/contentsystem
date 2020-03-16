using System;
using System.Collections.Generic;
using Randomous.EntitySystem;

namespace Randomous.ContentSystem
{
    //These are all PODOs, don't worry about complexity for conversion. You are micro-optimizing.
    public class BaseSystemObject
    {
        public long id {get;set;}
        public DateTime createDate {get;set;}

        protected virtual bool EqualsSelf(object obj)
        {
            var other = (BaseSystemObject)obj;
            return other.id == id && other.createDate == createDate;
        }

        public override bool Equals(object obj)
        {
            if(obj != null && this.GetType().Equals(obj.GetType()))
                return EqualsSelf(obj);
            else
                return false;
        }

        public override int GetHashCode() 
        { 
            return id.GetHashCode(); 
        }
    }

    public class BasicUser : BaseSystemObject {
        public string username {get;set;}

        protected override bool EqualsSelf(object obj)
        {
            var other = (BasicUser)obj;
            return base.EqualsSelf(obj) && username == other.username;
        }
    }

    public class User : BasicUser {
        public string email {get;set;}
        public string passwordHash {get;set;}

        protected override bool EqualsSelf(object obj)
        {
            var other = (User)obj;
            return base.EqualsSelf(obj) && email == other.email && passwordHash == other.passwordHash;
        }
    }

    public class BasicContent : BaseSystemObject {
        public string name {get;set;}
        public string content {get;set;}

        protected override bool EqualsSelf(object obj)
        {
            var other = (BasicContent)obj;
            return base.EqualsSelf(obj) && name == other.name && content == other.content;
        }
    }

    public class Content : BasicContent {
        public long parent {get;set;}
        public long owner {get;set;}

        protected override bool EqualsSelf(object obj)
        {
            var other = (Content)obj;
            return base.EqualsSelf(obj) && parent == other.parent && owner == other.owner;
        }
    }

    //These are all separate in case they need extra fields. They may not but... idk.
    public class BasicCategory : BasicContent {}
    public class Category : Content {}
    public class BasicComment : BasicContent {}
    public class Comment : Content {}

    public class BasicPermission : BaseSystemObject
    {
        long subject {get;set;}
        long target {get;set;}
        Action permissions {get;set;}

        protected override bool EqualsSelf(object obj)
        {
            var other = (BasicPermission)obj;
            return base.EqualsSelf(obj) && subject == other.subject && target == other.target && permissions == other.permissions;
        }
    }
}
