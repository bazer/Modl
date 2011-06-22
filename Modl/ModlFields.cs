using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Modl
{
    public class ModlFields : DynamicObject
    {
        public Dictionary<string, object> Dictionary = new Dictionary<string, object>();
        public Dictionary<string, Type> Types = new Dictionary<string, Type>();
        string NameOfLastInsertedMember;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!Dictionary.ContainsKey(binder.Name))
                Dictionary[binder.Name] = null;

            return Dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Dictionary[binder.Name] = value;
            NameOfLastInsertedMember = binder.Name;

            return true;
        }

        public string LastInsertedMemberName
        {
            get
            {
                return NameOfLastInsertedMember;
            }
        }
    }
}
