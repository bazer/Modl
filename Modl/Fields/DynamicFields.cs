///*
//Copyright 2011-2012 Sebastian Öberg (https://github.com/bazer)

//This file is part of Modl.

//Modl is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Modl is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with Modl.  If not, see <http://www.gnu.org/licenses/>.
//*/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Dynamic;
//using System.Data.Common;

//namespace Modl.Fields
//{
//    public class DynamicFields<M, IdType> : DynamicObject where M : Modl<M, IdType>, new()
//    {
//        IContent store;

//        internal DynamicFields(IContent store)
//        {
//            this.store = store;
//        }

//        public override bool TryGetMember(GetMemberBinder binder, out object result)
//        {
//            result = store.GetValue<object>(binder.Name);
//            return true;

//            //if (!store.Dictionary.ContainsKey(binder.Name))
//            //    store.Dictionary[binder.Name] = null;

//            //return store.Dictionary.TryGetValue(binder.Name, out result);
//        }

//        public override bool TrySetMember(SetMemberBinder binder, object value)
//        {
//            store.SetValue(binder.Name, value);

//            //store.Dictionary[binder.Name] = value;
//            //NameOfLastInsertedMember = binder.Name;

//            return true;
//        }
//    }
//}
