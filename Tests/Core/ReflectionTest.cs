using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;

namespace Tests.Core
{
    
    public class ReflectionTest
    {
        public class EmptyClass : IModl
        {
            public IModlData Modl { get; set; }
        }

        
        public ReflectionTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
        }

        [Fact]
        public void CreateNew()
        {
            var modl = ModlReflect.New(typeof(EmptyClass));
            Assert.Equal(typeof(EmptyClass), modl.GetType());
            Assert.True((modl as EmptyClass).IsNew());

            var id = Guid.NewGuid();
            modl = ModlReflect.New(typeof(EmptyClass), id);
            Assert.Equal(typeof(EmptyClass), modl.GetType());
            Assert.True((modl as EmptyClass).IsNew());
            Assert.True(id == (modl as EmptyClass).Id());
        }

        [Fact]
        public void Get()
        {
            var id = Guid.NewGuid();
            var modl = ModlReflect.New(typeof(EmptyClass), id) as EmptyClass;
            modl.Save();

            modl = ModlReflect.Get(typeof(EmptyClass), id) as EmptyClass;
            Assert.Equal(typeof(EmptyClass), modl.GetType());
            Assert.False(modl.IsNew());
            Assert.Equal(id, modl.Id().Get<Guid>());
        }

        [Fact]
        public void GetAll()
        {
            foreach (var m in ModlReflect.GetAll(typeof(EmptyClass)).Select(x => x as EmptyClass))
                m.Delete();

            var modlList = ModlReflect.GetAll(typeof(EmptyClass)).ToList();
            Assert.Equal(0, modlList.Count);

            var modl = new EmptyClass().Save();
            var modl2 = new EmptyClass().Save();

            var modlList2 = ModlReflect.GetAll(typeof(EmptyClass)).Select(x => x as EmptyClass).ToList();
            Assert.Equal(2, modlList2.Count);
            Assert.True(modlList2.Any(x => x.Id() == modl.Id()));
            Assert.True(modlList2.Any(x => x.Id() == modl2.Id()));
        }

        [Fact]
        public void List()
        {
            var modl = ModlReflect.New(typeof(EmptyClass)) as EmptyClass;
            modl.Save();

            var modl2 = ModlReflect.New(typeof(EmptyClass)) as EmptyClass;
            modl2.Save();

            var modlList = ModlReflect.List(typeof(EmptyClass)).ToList();
            Assert.NotEqual(0, modlList.Count);
            Assert.True(modlList.Any(x => (Guid)x == modl.Id().Get<Guid>()));
            Assert.True(modlList.Any(x => (Guid)x == modl2.Id().Get<Guid>()));

            var modlList2 = ModlReflect.List<Guid>(typeof(EmptyClass)).ToList();
            Assert.NotEqual(0, modlList2.Count);
            Assert.True(modlList2.Any(x => x == modl.Id().Get<Guid>()));
            Assert.True(modlList2.Any(x => x == modl2.Id().Get<Guid>()));
        }
    }
}
