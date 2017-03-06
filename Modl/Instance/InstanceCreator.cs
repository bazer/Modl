using Castle.DynamicProxy;
using Modl.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modl.Instance
{
    public static class ImmutableInstanceCreator<M> where M : class, IModl
    {
        private static ProxyGenerator generator = new ProxyGenerator();

        public static M NewInstance(IModlData modlData)// where T : class
        {
            //var generator = new ProxyGenerator();
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

    internal static class MutableInstanceCreator<M> where M : class, IMutable
    {
        private static ProxyGenerator generator = new ProxyGenerator();

        internal static M NewInstance(M immutableInstance)// where T : class
        {
            //var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget<M>(new MutableInterceptor(immutableInstance));
            return proxy;
        }

        //internal static void MutateProperty(M mutableInstance, string property, object value)
        //{

        //}

        internal class MutableInterceptor : IInterceptor //<M> : IInterceptor where M : class, IModl
        {
            private IMutable immutableInstance;
            private Dictionary<string, IProperty> mutatedValues = new Dictionary<string, IProperty>();

            public MutableInterceptor(IMutable immutableInstance)
            {
                this.immutableInstance = immutableInstance;
            }

            public void Intercept(IInvocation invocation)
            {
                var info = new InvocationInfo(invocation);

                if (info.CallType == CallType.Set)
                {
                    //if (!immutableInstance.Modl.Backer.Definitions.Properties.Any(x => x.PropertyName == info.Property))
                    //    throw new InvalidPropertyNameException($"Property with name '{info.Property}' doesn't exist on type '{immutableInstance.GetType()}'");

                    var definition = immutableInstance.Modl.Backer.Definitions.Properties.SingleOrDefault(x => x.PropertyName == info.Property);
                    if (definition == null)
                        throw new InvalidPropertyNameException($"Property with name '{info.Property}' doesn't exist on type '{immutableInstance.GetType()}'");

                    if (definition.IsLink)
                        mutatedValues[info.Property] = new RelationProperty(definition, info.Property, info.Value as IModl);
                    else
                        mutatedValues[info.Property] = new SimpleProperty(definition, info.Property, info.Value);
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
                        var definition = immutableInstance.Modl.Backer.Definitions.Properties.SingleOrDefault(x => x.PropertyName == info.Property);
                        if (definition == null)
                            throw new InvalidPropertyNameException($"Property with name '{info.Property}' doesn't exist on type '{immutableInstance.GetType()}'");

                        if (definition.IsLink)
                        {
                            if (mutatedValues.ContainsKey(info.Property))
                                invocation.ReturnValue = (mutatedValues[info.Property] as IRelationProperty).Value;
                            else
                                invocation.ReturnValue = immutableInstance.Modl.Backer.RelationValueBacker.GetValue(info.Property).Get();
                        }
                        else
                        {
                            if (mutatedValues.ContainsKey(info.Property))
                                invocation.ReturnValue = (mutatedValues[info.Property] as ISimpleProperty).Value;
                            else
                                invocation.ReturnValue = immutableInstance.Modl.Backer.SimpleValueBacker.GetValue(info.Property).Get();
                        }
                    }
                }
            }


            

            private IEnumerable<Modification> GetModifications()
            {
                foreach (var mutatedValue in mutatedValues)
                {
                    var newProperty = mutatedValue.Value;
                    var oldProperty = newProperty.Metadata.IsLink 
                        ? new RelationProperty(newProperty.Metadata, mutatedValue.Key, immutableInstance.Modl.Backer.RelationValueBacker.GetValue(mutatedValue.Key).Get() as IModl) as IProperty
                        : new SimpleProperty(newProperty.Metadata, mutatedValue.Key, immutableInstance.Modl.Backer.SimpleValueBacker.GetValue(mutatedValue.Key).Get());
                    
                    yield return new Modification(Guid.NewGuid(), immutableInstance, oldProperty, newProperty);
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

            if ((this.CallType == CallType.Get && invocation.Arguments.Length == 1) || (this.CallType == CallType.Set && invocation.Arguments.Length == 2))
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
