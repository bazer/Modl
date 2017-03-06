using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl
{
    public interface IUser
    {
        string Name { get; }
    }

    public class User: IUser
    {
        public string Name { get; }

        public User(string name)
        {
            this.Name = name;
        }
    }
}
