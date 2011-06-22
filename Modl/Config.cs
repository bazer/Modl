using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Modl
{
    public class Config
    {
        public static string ConnectionString { get; set; }

        static Config()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[1].ConnectionString;
        }

        public static void SetConnectionStringFromName(string name)
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
