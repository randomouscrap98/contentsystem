using System;
using System.Collections.Generic;

namespace Randomous.ContentSystem
{
    [Flags]
    public enum Action
    {
        None = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8
    }

    public enum Identifier
    {
        Permission,
        Parent,
        Email,
        Password
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
                //"sa" for system action
                { Action.Create,    "sa:c" },      //c for action create
                { Action.Read,      "sa:r" },      //r for action read
                { Action.Update,    "sa:u" },      //u for action update 
                { Action.Delete,    "sa:d" },      //d for action delete
                { Identifier.Permission,    "si:p" },   //p for permission
                { Identifier.Parent,        "si:o" },   //o for owner
                { Identifier.Email,         "si:e" },   //e for email
                { Identifier.Password,      "si:s" },   //s for secret
                { SystemType.User,      "st:u" },       //u for user
                { SystemType.Comment,   "st:m" },       //m for message
                { SystemType.Category,  "st:c" },       //c for category
                { SystemType.Content,   "st:p" },       //p for post
            };
        }

        //public Dictionary<Identifier, string> IDKeys = new Dictionary<Identifier, string>()
        //{
        //};
        
        //public Dictionary<SystemType, string> SystemKeys = new Dictionary<SystemType, string>()
        //{
        //};

        //public string GetKeyGeneric<T>(Dictionary<T,string> dic, T id)
        //{
        //    if(!dic.ContainsKey(id))
        //        throw new InvalidOperationException($"No key for identifier {id}");

        //    return dic[id];
        //}

        //public string GetKey(Enum id)
        //{
        //    if(id is Identifier)
        //        return GetKeyGeneric(IDKeys, (Identifier)id);
        //    else if (id is Action)
        //        return GetKeyGeneric(ActionKeys, (Action)id);
        //    else if (id is SystemType)
        //        return GetKeyGeneric(SystemKeys, (SystemType)id);
        //    
        //    throw new InvalidOperationException($"Unsupported enumerator {id.GetType()}");
        //}
    }
}