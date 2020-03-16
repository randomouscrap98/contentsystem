using System;
using System.Collections.Generic;

namespace Randomous.ContentSystem
{
    //[Flags]
    //public enum Action
    //{
    //    None = 0,
    //    Create = 1,
    //    Read = 2,
    //    Update = 4,
    //    Delete = 8
    //}

    public enum Identifier
    {
        Permission,
        Parent,
        Email,
        Password,
        Owner
    }

    public enum SystemType
    {
        User,
        Content,
        Category,
        Comment
    }

    public class IdentifierKeys
    {
        public Dictionary<Enum, string> GetKeys()
        {
            return new Dictionary<Enum, string>()
            {
                //{ Action.Create,    "ac" },         //c for create
                //{ Action.Read,      "ar" },         //r for read
                //{ Action.Update,    "au" },         //u for update 
                //{ Action.Delete,    "ad" },         //d for delete
                { Identifier.Permission,    "ip" },     //p for permission
                { Identifier.Parent,        "id" },     //d for daddy
                { Identifier.Email,         "ie" },     //e for email
                { Identifier.Password,      "is" },     //s for secret
                { Identifier.Owner,         "io" },     //o for oWner
                { SystemType.User,      "tu" },     //u for user
                { SystemType.Comment,   "tm" },     //m for message
                { SystemType.Category,  "tc" },     //c for category
                { SystemType.Content,   "tp" },     //p for post
            };
        }
    }
}