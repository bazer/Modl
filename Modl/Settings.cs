using Modl.Cache;
using Modl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Modl.Exceptions;

namespace Modl
{
    public enum CacheLevel
    {
        On,
        Off,
        All
    }

    public enum CacheTimeout
    {
        Never = 0,
        TenMinutes = 10,
        TwentyMinutes = 20,
        ThirtyMinutes = 30,
        OneHour = 60,
        OneDay = 1440
    }

    public enum InstanceSeparation
    {
        None,
        Thread,
        Custom
    }

    public class Settings
    {
        public CacheLevel CacheLevel;

        public int CacheTimeout;
        static Dictionary<Type, Settings> AllSettings = new Dictionary<Type, Settings>();
        private IEndpoint endpoint;
        private ISerializer serializer;
        public Settings()
        {
            CacheLevel = CacheConfig.DefaultCacheLevel;
            CacheTimeout = CacheConfig.DefaultCacheTimeout;
        }

        public static Settings GlobalSettings { get; private set; } = new Settings();
        public IEndpoint Endpoint
        {
            get
            {
                if (endpoint != null)
                    return endpoint;

                if (GlobalSettings.endpoint != null)
                    return GlobalSettings.endpoint;

                throw new Exception("No endpoint configured.");
            }

            set
            {
                endpoint = value;
            }
        }

        public InstanceSeparation InstanceSeparation { get; set; } = InstanceSeparation.None;
        private Func<ConcurrentDictionary<Type, object>> customInstanceSeparationDictionary;
        public Func<ConcurrentDictionary<Type, object>> CustomInstanceSeparationDictionary
        {
            get
            {
                if (customInstanceSeparationDictionary == null)
                    throw new InvalidConfigurationException("Custom instance separation mode is being used but CustomInstanceSeparationDictionary is not set");

                return customInstanceSeparationDictionary;
            }
            set
            {
                customInstanceSeparationDictionary = value;
            }
        }

        public ISerializer Serializer
        {
            get
            {
                if (serializer != null)
                    return serializer;

                if (GlobalSettings.serializer != null)
                    return GlobalSettings.serializer;

                throw new Exception("No serializer configured.");
            }

            set
            {
                serializer = value;
            }
        }
        public static Settings Get(Type type)
        {
            if (!AllSettings.ContainsKey(type))
                AllSettings.Add(type, new Settings());

            return AllSettings[type];
        }

        //public IEnumerable<IModlPipeline<M>> GetPipeline { get; private set; }
        //public IEnumerable<IModlPipeline<M>> SavePipeline { get; private set; }

        //public void ConfigurePipeline(params IModlPipeline<M>[] pipeline)
        //{
        //    this.SavePipeline = pipeline.ToList();
        //    this.GetPipeline = pipeline.Reverse().ToList();
        //}
    }
}