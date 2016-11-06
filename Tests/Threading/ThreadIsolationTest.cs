using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modl;
using Modl.Json;
using Modl.Plugins;
using Xunit;

namespace Tests.Threading
{
    public class ThreadIsolationTest
    {
        public ThreadIsolationTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
            Settings.GlobalSettings.InstanceSeparation = InstanceSeparation.Thread;
        }

        public class ThreadClass1 : IModl
        {
            public IModlData Modl { get; set; }
            public int Property1 { get; set; }
            public int Property2 { get; set; }
        }

        [Fact]
        public void TestRaces()
        {
            var testClass = new ThreadClass1();
            testClass.Save();
            var id = testClass.Id();

            Parallel.For(0, 10, i =>
            {
                var test = Modl<ThreadClass1>.Get(id);
                SetAndTest(test, 100);
                SetAndTest(test, 200);
                SetAndTest(test, 300);
                SetAndTest(test, 400);
                SetAndTest(test, 500);
            });
        }

        private void SetAndTest(ThreadClass1 testClass, int value)
        {
            testClass.Property1 = value;
            
            Assert.True(testClass.IsModified());
            Assert.Equal(value, testClass.Property1);
        }
    }
}
