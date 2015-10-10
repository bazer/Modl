using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel
{
    //[Name("CarType")]
    public class CarType// : IModl
    {
        //public IModlData Modl { get; set; }
        //[Key]
        //public int TypeID { get; set; }
        public string Description { get; set; }

        //public string GetId() => Id.ToString();
        //public void SetId(string id) => Id = int.Parse(id);
    }
}
