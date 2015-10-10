using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modl.Structure.Instance
{
    public class RelationValue : SimpleValue
    {
        public object Id { get; set; }
        public bool HasId { get; set; }
        public bool IsLoaded { get; set; }

        public RelationValue(object value): base(value)
        {
        }

        //public override bool IsModified
        //{
        //    get
        //    {
        //        return base.IsModified;
        //    }
        //}
    }
}
