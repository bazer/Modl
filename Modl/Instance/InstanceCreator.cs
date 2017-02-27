using Castle.DynamicProxy;
using Modl.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Instance
{
    public class ImmutableInstanceCreator<M> where M : class, IModl
    {
        public static M NewInstance(IModlData modlData)// where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<M>(new ImmutableInterceptor(modlData));
            return proxy;
        }

        internal class ImmutableInterceptor : IInterceptor //<M> : IInterceptor where M : class, IModl
        {
            private IModlData ModlData;
            
            public ImmutableInterceptor(IModlData modlData)
            {
                this.ModlData = modlData;
            }

            public void Intercept(IInvocation invocation)
            {
                var name = invocation.Method.Name;

                if (name.StartsWith("set_", StringComparison.Ordinal))
                    throw new Exception("Call to setter not allowed on an immutable type");

                if (!name.StartsWith("get_", StringComparison.Ordinal))
                    throw new NotImplementedException();

                name = name.Substring(4);

                if (name == "Modl")
                    invocation.ReturnValue = ModlData;
                else if (name == "IsMutable")
                    invocation.ReturnValue = false;
                else
                {
                    var value = ModlData.Backer.SimpleValueBacker.GetValue(name).Get();
                    invocation.ReturnValue = value;
                }
            }
        }
    }

    internal class MutableInstanceCreator<M> where M : class, IMutable
    {
        internal static M NewInstance(M immutableInstance)// where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<M>(new MutableInterceptor(immutableInstance));
            return proxy;
        }

        //internal static void MutateProperty(M mutableInstance, string property, object value)
        //{

        //}

        internal class MutableInterceptor : IInterceptor //<M> : IInterceptor where M : class, IModl
        {
            private IMutable immutableInstance;
            private Dictionary<string, object> mutatedValues = new Dictionary<string, object>();

            public MutableInterceptor(IMutable immutableInstance)
            {
                this.immutableInstance = immutableInstance;
            }

            public void Intercept(IInvocation invocation)
            {
                var info = new InvocationInfo(invocation);

                if (info.CallType == CallType.Set)
                {
                    if (!immutableInstance.Modl.Backer.Definitions.Properties.Any(x => x.PropertyName == info.Property))
                        throw new InvalidPropertyNameException($"Property with name '{info.Property}' doesn't exist on type '{immutableInstance.GetType()}'");

                    mutatedValues[info.Property] = info.Value;
                }
                else
                {
                    if (info.Property == "Modl")
                        invocation.ReturnValue = immutableInstance.Modl;
                    else if (info.Property == "IsMutable")
                        invocation.ReturnValue = true;
                    else if (info.Property == "IsNew")
                        invocation.ReturnValue = true;
                    else if (info.Property == "IsModified")
                        invocation.ReturnValue = mutatedValues.Any();
                    else if (info.Property == "Modifications")
                        invocation.ReturnValue = GetModifications();
                    else
                    {
                        if (mutatedValues.ContainsKey(info.Property))
                            invocation.ReturnValue = mutatedValues[info.Property];
                        else
                            invocation.ReturnValue = immutableInstance.Modl.Backer.SimpleValueBacker.GetValue(info.Property).Get();
                    }
                }


                //var name = invocation.Method.Name;

                //if (name.StartsWith("set_", StringComparison.Ordinal))
                //{
                //    if (invocation.Arguments.Length == 2)
                //    {
                //        name = invocation.Arguments[0] as string;


                //        mutatedValues[name] = invocation.Arguments[1];
                //    }
                //    else
                //    {
                //        name = name.Substring(4);
                //        mutatedValues[name] = invocation.Arguments[0];
                //    }
                //}
                //else if (name.StartsWith("get_", StringComparison.Ordinal))
                //{
                //    name = name.Substring(4);

                //    if (name == "Modl")
                //        invocation.ReturnValue = immutableInstance.Modl;
                //    else if (name == "IsMutable")
                //        invocation.ReturnValue = true;
                //    else if (name == "IsNew")
                //        invocation.ReturnValue = true;
                //    else if (name == "IsModified")
                //        invocation.ReturnValue = mutatedValues.Any();
                //    else if (name == "Modifications")
                //        invocation.ReturnValue = GetModifications();
                //    else
                //    {
                //        if (mutatedValues.ContainsKey(name))
                //            invocation.ReturnValue = mutatedValues[name];
                //        else
                //            invocation.ReturnValue = immutableInstance.Modl.Backer.SimpleValueBacker.GetValue(name).Get();
                //    }
                //}
                //else
                //    throw new NotImplementedException();
            }

            

            private IEnumerable<Modification> GetModifications()
            {
                foreach (var mutatedValue in mutatedValues)
                {
                    var property = new SimpleProperty(mutatedValue.Key, mutatedValue.Value);
                    yield return new Modification(immutableInstance, property);
                }
            }

            

            

            
        }
    }

    internal enum CallType
    {
        Get,
        Set
    }

    internal enum MethodType
    {
        Property,
        Indexer
    }

    internal struct InvocationInfo
    {
        

        internal CallType CallType { get; }
        internal MethodType MethodType { get; }
        internal string Property { get; }
        internal object Value { get; }

        internal InvocationInfo(IInvocation invocation)
        {
            var name = invocation.Method.Name;

            if (name.StartsWith("set_", StringComparison.Ordinal))
                this.CallType = CallType.Set;
            else if (name.StartsWith("get_", StringComparison.Ordinal))
                this.CallType = CallType.Get;
            else
                throw new NotImplementedException();

            if (invocation.Arguments.Length == 2)
            {
                this.MethodType = MethodType.Indexer;
                this.Property = invocation.Arguments[0] as string;
            }
            else
            {
                this.MethodType = MethodType.Property;
                this.Property = name.Substring(4);
            }

            if (this.CallType == CallType.Set && this.MethodType == MethodType.Property)
                this.Value = invocation.Arguments[0];
            else if (this.CallType == CallType.Set && this.MethodType == MethodType.Indexer)
                this.Value = invocation.Arguments[1];
            else
                this.Value = null;
        }
    }

    //public class DelegateWrapper
    //{
    //    public static T WrapAs<T>(Delegate impl)// where T : class
    //    {
    //        var generator = new ProxyGenerator();
    //        var proxy = generator.CreateInterfaceProxyWithoutTarget((typeof(T), new PropertyInterceptor(impl));
    //        return (T)proxy;
    //    }
    //}
}
