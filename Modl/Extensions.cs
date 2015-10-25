using Modl.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modl.Structure.Instance;
using Modl.Structure.Metadata;

namespace Modl
{
    public static class Extensions
    {
        internal static Backer GetBacker<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.InitializeModl(m).Modl.Backer;
        }

        public static M Modl<M>(this M m) where M : IModl, new()
        {
            return Handler<M>.InitializeModl(m);
        }

        public static bool IsNew<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsNew;
        }

        public static bool IsDeleted<M>(this M m) where M : IModl, new()
        {
            return m.GetBacker().IsDeleted;
        }

        public static bool HasId<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.ReadFromInstanceId(m);

            return backer.HasId();
        }

        public static bool IsModified<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.ReadFromInstance(m);

            return backer.IsModified();
        }

        public static M SetId<M>(this M m, object value) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.SetId(value);
            backer.WriteToInstanceId(m);

            return m;
        }

        public static M GenerateId<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.GenerateId();
            backer.WriteToInstanceId(m);

            return m;
        }

        public static object GetId<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.ReadFromInstanceId(m);

            return backer.GetId();
        }

        public static Relation<M> Relation<M>(this M m, string name)
            where M : IModl, new()
        {
            return new Modl.Relation<M>
            {
                Name = name,
                Backer = m.GetBacker()
            };
        }

        public static T GetRelation<M, T>(this M m, string name) 
            where M: IModl, new()
            where T: IModl, new()
        {
            return m.GetBacker().GetRelationValue<T>(name);
        }

        public static void SetRelation<M, T>(this M m, string name, T value)
            where M : IModl, new()
            where T : IModl, new()
        {
            m.GetBacker().SetRelationValue(name, value);
        }

        public static M Save<M>(this M m, bool includeRelations = true) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.Save(m, includeRelations);

            return m;
        }

        public static M Delete<M>(this M m) where M : IModl, new()
        {
            var backer = m.GetBacker();
            backer.Delete();

            return m;
        }
    }

    public class Relation<M>
         where M : IModl, new()
    {
        public string Name { get; internal set; }
        public Backer Backer { get; internal set; }

        public T GetValue<T>()
            where T : IModl, new()
        {
            return Backer.GetRelationValue<T>(Name);
        }

        public void SetValue<T>(T value)
            where T : IModl, new()
        {
            Backer.SetRelationValue(Name, value);
        }
    }
}
