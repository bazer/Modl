using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Config
    {
        public static string TestOutput { get; } = ConfigurationManager.AppSettings[nameof(TestOutput)];
    }
}
