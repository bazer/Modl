using Modl.Cache;
using Modl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modl.Structure
{
    public class Settings
    {
        public CacheLevel CacheLevel;
        public int CacheTimeout;

        private IEndpoint endpoint;
        public IEndpoint Endpoint
        {
            get
            {
                if (endpoint != null)
                    return endpoint;

                if (Config.GlobalSettings.endpoint != null)
                    return Config.GlobalSettings.endpoint;

                throw new Exception("No endpoint configured.");
            }

            set
            {
                endpoint = value;
            }
        }

        private ISerializer serializer;
        public ISerializer Serializer
        {
            get
            {
                if (serializer != null)
                    return serializer;

                if (Config.GlobalSettings.serializer != null)
                    return Config.GlobalSettings.serializer;

                throw new Exception("No serializer configured.");
            }

            set
            {
                serializer = value;
            }
        }


        public Settings()
        {
            CacheLevel = CacheConfig.DefaultCacheLevel;
            CacheTimeout = CacheConfig.DefaultCacheTimeout;
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