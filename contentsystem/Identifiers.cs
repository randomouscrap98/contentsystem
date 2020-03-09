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
    }

    public class IdentifierService 
    {
        public Dictionary<Action, string> ActionKeys = new Dictionary<Action, string>()
        {
            //"sa" for system action
            { Action.Create,    "sa:c" },      //c for action create
            { Action.Read,      "sa:r" },      //r for action read
            { Action.Update,    "sa:u" },      //u for action update 
            { Action.Delete,    "sa:d" },      //d for action delete
        };

        public Dictionary<Identifier, string> IDKeys = new Dictionary<Identifier, string>()
        {
            { Identifier.Permission,    "si:p" },   //p for permission
            { Identifier.Parent,        "si:o" },   //o for owner
        };

        public string GetKeyGeneric<T>(Dictionary<T,string> dic, T id)
        {
            if(!dic.ContainsKey(id))
                throw new InvalidOperationException($"No key for identifier {id}");

            return dic[id];
        }

        public string GetKey(Identifier id) { return GetKeyGeneric(IDKeys, id); }
        public string GetKey(Action id) { return GetKeyGeneric(ActionKeys, id); }
    }
}