﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Data.Common;

namespace Modl.Fields
{
    public class DynamicFields<C> : DynamicObject where C : Modl<C>, new()
    {
        Store<C> store;

        internal DynamicFields(Store<C> store)
        {
            this.store = store;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = store.GetValue<object>(binder.Name);
            return true;

            //if (!store.Dictionary.ContainsKey(binder.Name))
            //    store.Dictionary[binder.Name] = null;

            //return store.Dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            store.SetValue(binder.Name, value);

            //store.Dictionary[binder.Name] = value;
            //NameOfLastInsertedMember = binder.Name;

            return true;
        }
    }
}
