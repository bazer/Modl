using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Modl;
using Modl.Exceptions;
using Modl.Json;
using Modl.Plugins;
using Xunit;

namespace Tests.Threading
{
    public class CustomIsolationTest
    {
        private ConcurrentDictionary<int, ConcurrentDictionary<Type, object>> threadDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<Type, object>>();

        public CustomIsolationTest()
        {
            Settings.GlobalSettings.Serializer = new JsonModl();
            Settings.GlobalSettings.Endpoint = new FileModl(Config.TestOutput);
            
            //Settings.GlobalSettings.CustomInstanceSeparationDictionary = () =>
            //{
            //    if (HttpContext.Current.Items["Modl"] == null)
            //        HttpContext.Current.Items["Modl"] = new ConcurrentDictionary<Type, object>();

            //    return HttpContext.Current.Items["Modl"] as ConcurrentDictionary<Type, object>;
            //};
        }

        [Fact]
        public void TestConfiguration()
        {
            Settings.GlobalSettings.InstanceSeparation = InstanceSeparation.Custom;

            var testClass = new PerRequestThreadClass();
            Assert.Throws<InvalidConfigurationException>(() => testClass.IsModified());

            var dictionary = new ConcurrentDictionary<Type, object>();
            Settings.GlobalSettings.CustomInstanceSeparationDictionary = () => dictionary;

            Assert.False(testClass.IsModified());
        }

        [Fact]
        public void TestRaces()
        {
            Settings.GlobalSettings.InstanceSeparation = InstanceSeparation.Custom;
            Settings.GlobalSettings.CustomInstanceSeparationDictionary = () => GetDictionary(Thread.CurrentThread.ManagedThreadId);

            var testClass = new PerRequestThreadClass();
            testClass.Save();
            var id = testClass.Id();

            Parallel.For(0, 10, i =>
            {
                var test = Modl<PerRequestThreadClass>.Get(id);
                SetAndTest(test, 100);
                SetAndTest(test, 200);
                SetAndTest(test, 300);
                SetAndTest(test, 400);
                SetAndTest(test, 500);
            });
        }

        private ConcurrentDictionary<Type, object> GetDictionary(int threadId)
        {
            if (!threadDictionary.ContainsKey(threadId))
                threadDictionary[threadId] = new ConcurrentDictionary<Type, object>();

            return threadDictionary[threadId] as ConcurrentDictionary<Type, object>;
        }

        private void SetAndTest(PerRequestThreadClass testClass, int value)
        {
            testClass.Property1 = value;

            Assert.True(testClass.IsModified());
            Assert.Equal(value, testClass.Property1);
        }

        public class PerRequestThreadClass : IModl
        {
            public IModlData Modl { get; set; }
            public int Property1 { get; set; }
            public int Property2 { get; set; }

            public bool IsMutable => throw new NotImplementedException();
        }
    }
}
