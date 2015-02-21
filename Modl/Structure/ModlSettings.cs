using Modl.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modl.Structure
{
    public class ModlSettings
    {
        private IModlEndpoint endpoint;
        public IModlEndpoint Endpoint
        {
            get
            {
                if (endpoint != null)
                    return endpoint;

                if (ModlConfig.GlobalSettings.endpoint != null)
                    return ModlConfig.GlobalSettings.endpoint;

                throw new Exception("No endpoint configured.");
            }

            set
            {
                endpoint = value;
            }
        }

        private IModlSerializer serializer;
        public IModlSerializer Serializer
        {
            get
            {
                if (serializer != null)
                    return serializer;

                if (ModlConfig.GlobalSettings.serializer != null)
                    return ModlConfig.GlobalSettings.serializer;

                throw new Exception("No serializer configured.");
            }

            set
            {
                serializer = value;
            }
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