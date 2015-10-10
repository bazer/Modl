using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl
{
    //public enum IdType
    //{
    //    Int,
    //    Guid
    //}

    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
        public Type Entity { get; private set; }
        public string FieldName { get; private set; }



        public ForeignKeyAttribute(Type entity)
        {
            this.Entity = entity;
        }

        public ForeignKeyAttribute(Type entity, string fieldName)
        {
            this.Entity = entity;
            this.FieldName = fieldName;
        }

        //public KeyAttribute(string name)
        //{
        //    Name = name;
        //    //Type = type;
        //}
    }
}
