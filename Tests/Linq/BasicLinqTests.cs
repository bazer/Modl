﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Modl;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Linq
{
    
    public class BasicLinqTests
    {
        public class EmptyClass : IModl
        {
            public IModlData Modl { get; set; }
        }

        
        public void Initialize()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        

        //[Fact]
        //public void QueryAll()
        //{
        //    foreach (var m in Modl<EmptyClass>.Query())
        //        m.Delete();

        //    var modls = Modl<EmptyClass>.Query().ToList();
        //    Assert.Equal(0, modls.Count);

        //    var modl = new EmptyClass();
        //    modl.Save();

        //    modls = Modl<EmptyClass>.Query().ToList();
        //    Assert.Equal(1, modls.Count);
        //}
    }
}
