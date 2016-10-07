using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Exceptions;

namespace Modl.Instance
{
    public class ValueBacker<T> where T: IValue
    {
        private Dictionary<string, T> Values { get; } = new Dictionary<string, T>();

        public ValueBacker()
        {
        }

        public bool IsModified => Values.Any(x => x.Value.IsModified);

        public bool HasValue(string name) => Values.ContainsKey(name);


        public void AddValue(string name, T value)
        {
            if (HasValue(name))
                throw new InvalidPropertyNameException($"Name '{name}' has already been added to the backer");

            Values[name] = value;
        }

        public T GetValue(string name)
        {
            if (!HasValue(name))
                throw new InvalidPropertyNameException($"Name '{name}' hasn't been added to the backer");

            return Values[name];
        }

        //public void SetValue(string name, T value)
        //{
        //    if (!HasValue(name))
        //        throw new InvalidPropertyNameException($"Name '{name}' hasn't been added to the backer");

        //    Values[name].Set(value);
        //}


        internal void ResetValuesToUnmodified()
        {
            foreach (var value in Values.Values)
                value.Reset();
        }
    }
}
