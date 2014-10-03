using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.CSharp
{
    class AssemblyContext
    {
        public AssemblyContext()
        {
            Assemblies = new List<Assembly>();
            ClassMappings = new Dictionary<string, string>();
            MadeGenericClasses = new List<Class>();
            ArrayClasses = new List<Class>();
            PendingMakers = new List<GenericMaker>();
        }
        public List<Assembly> Assemblies { get; set; }
        public Dictionary<string, string> ClassMappings { get; set; }

        Dictionary<string, Class> AllClassesByFullName;
        Dictionary<Class, List<Class>> AllClassesByBaseClass;
        public void IndexClasses()
        {
            AllClassesByFullName = new Dictionary<string, Class>();
            foreach (var asm in Assemblies)
            {
                asm.ClassesAndNested.ForEach(t => AllClassesByFullName[t.FullName] = t);
            }
            AllClassesByBaseClass = new Dictionary<Class, List<Class>>();
            foreach (var asm in Assemblies)
            {
                foreach (var ce in asm.ClassesAndNested)
                {
                    if (ce.BaseClass == null)
                        continue;
                    AllClassesByBaseClass.GetCreate(ce.BaseClass).Add(ce);

                }
            }
        }
        public void ClearIndex()
        {
            AllClassesByFullName = null;
            AllClassesByBaseClass = null;
        }
        public Class GetClass(string name)
        {
            name = ClassMappings.TryGetValue(name) ?? name;
            if (AllClassesByFullName != null)
                return AllClassesByFullName.TryGetValue(name);
            foreach (var asm in Assemblies)
            {
                var type = asm.GetClass(name);
                if (type != null)
                    return type;
            }
            return null;
        }

        public IEnumerable<Class> GetDerivedClasses(Class ce)
        {
            if (AllClassesByBaseClass != null)
            {
                var list = AllClassesByBaseClass.TryGetValue(ce) ?? new List<Class>();
                foreach (var ce2 in list)
                    yield return ce2;
                yield break;
            }
            foreach (var ce2 in ce.Assembly.ClassesAndNested)
            {
                if (ce2.BaseClass != null && ce2.BaseClass == ce)
                    yield return ce2;
            }
        }

        public IEnumerable<Class> ClassesAndNestedAndMadeGenerics(Assembly asm)
        {
            foreach (var ce in asm.ClassesAndNested)
            {
                yield return ce;
            }
            foreach (var ce in MadeGenericClasses)
            {
                if (ce.Assembly == asm)
                    yield return ce;
            }
        }

        public List<Class> MadeGenericClasses { get; set; }
        public List<GenericMaker> PendingMakers { get; set; }
        public Class MakeGenericClass(Class ce, List<Class> genericArgs)
        {
            return MakeGenericClass(ce, genericArgs, false);
        }
        public Class MakeGenericClass(Class ce, List<Class> genericArgs, bool makePartial)
        {
            if (ce.GenericArguments.IsNullOrEmpty())
                throw new Exception();
            if (ce.GenericArguments.Any(t => !t.IsGenericTypeArgument))
                throw new Exception();
            if (genericArgs.IsNullOrEmpty())
                throw new Exception();
            if (ce.GenericArguments.Count != genericArgs.Count)
                throw new Exception();
            if (ce.GenericArguments.SequenceEqual(genericArgs))
                return ce;
            foreach (var ce3 in MadeGenericClasses.Where(t => t.GenericClassDefinition == ce))
            {
                if (ce3.GenericArguments.SequenceEqual(genericArgs))
                    return ce3;
            }
            var x = new GenericMaker { RootClass = ce, RootGenericArgs = genericArgs, Context = this };
            x.PreMake();
            MadeGenericClasses.Add(x.MadeClass);
            if (makePartial)
                PendingMakers.Add(x);
            else
                x.Make();
            return x.MadeClass;
        }

        public void CompletePartialGenericClasses()
        {
            var x = PendingMakers.ToList();
            PendingMakers.Clear();
            x.ForEach(t => t.Make());
        }

        public List<Class> ArrayClasses { get; set; }
        public Class MakeArrayClass(Class ce)
        {
            var ce2 = ArrayClasses.Where(t => t.IsArray && t.ArrayElementType == ce).FirstOrDefault();
            if (ce2 == null)
            {
                ce2 = new Class
                {
                    IsArray = true,
                    ArrayElementType = ce,
                };
                ArrayClasses.Add(ce2);
            }
            return ce2;
        }
    }

}
