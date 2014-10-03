using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.CSharp
{
    static class CodeModelExtensions
    {

        public static bool IsNullOrVoid(this Class ce)
        {
            return ce == null || ce.Name == "void";
        }

        public static bool HasEmptyConstructor(this Class ce)
        {
            return ce.GetEmptyConstructor() != null;
        }
        public static Method GetEmptyConstructor(this Class ce)
        {
            return ce.Members.OfType<Method>().Where(t => t.IsConstructor && t.Parameters.Count == 0 && !t.IsStatic).FirstOrDefault();
        }
        public static bool HasAnyConstructor(this Class ce)
        {
            return ce.Members.OfType<Method>().Where(t => t.IsConstructor).FirstOrDefault() != null;
        }
        public static IEnumerable<Method> Methods(this Class ce)
        {
            return ce.Members.Methods();
        }
        public static IEnumerable<Method> Methods(this IEnumerable<Member> list)
        {
            return list.OfType<Method>().Where(t => !t.IsConstructor);
        }
        public static IEnumerable<Method> AllMethods(this Class ce)
        {
            return ce.AllMembers().Methods();
        }
        public static IEnumerable<Member> AllMembers(this Class ce)
        {
            while (ce != null)
            {
                foreach (var me in ce.Members)
                {
                    yield return me;
                }
                if (ce.BaseClass == null)
                    yield break;
                ce = ce.BaseClass;
            }
        }
        static Class ToClassRef(this Class ce)
        {
            return ce;
        }
        public static List<Class> AllInterfacesAndSelfInterface(this Class ce)
        {
            var list = new HashSet<Class>();
            list.Add(ce);
            AddAllInterfaces(ce, list);
            return list.ToList();

        }
        public static List<Class> AllInterfaces(this Class ce)
        {
            var list = new HashSet<Class>();
            AddAllInterfaces(ce, list);
            return list.ToList();

        }
        public static void AddAllInterfaces(Class ce, HashSet<Class> list)
        {
            foreach (var x in ce.Interfaces)
            {
                if (list.Add(x))
                {
                    AddAllInterfaces(x, list);
                }
            }
        }
        public static IEnumerable<Member> AllInterfacesAndSelfInterfaceMembers(this Class ce)
        {
            if (!ce.IsInterface)
                throw new Exception();
            var list = ce.AllInterfacesAndSelfInterface();
            foreach (var ce2 in list)
            {
                if (ce2 == null)
                    continue;
                foreach (var me in ce2.Members)
                {
                    yield return me;
                }
            }
        }
        public static IEnumerable<Method> MethodsAndConstructors(this Class ce)
        {
            return ce.Members.OfType<Method>();
        }
        public static IEnumerable<Field> Fields(this Class ce)
        {
            return ce.Members.OfType<Field>();
        }
        public static IEnumerable<Property> Properties(this Class ce)
        {
            return ce.Members.OfType<Property>();
        }
        public static IEnumerable<Member> PropertiesAndFields(this Class ce)
        {
            return ce.Members.Where(t => t is Property || t is Field);
        }
        public static IEnumerable<Method> Constructors(this Class ce)
        {
            return ce.Members.OfType<Method>().Where(t => t.IsConstructor);
        }
        public static IEnumerable<Member> MembersExceptClasses(this Class ce)
        {
            return ce.Members.Where(t => !(t is Class));
        }
        public static IEnumerable<Class> Classes(this Class ce)
        {
            return ce.Members.OfType<Class>();
        }

        public static IEnumerable<TypedElement> Descendants(this Member me)
        {
            if (me is Method)
            {
                var me2 = (Method)me;
                foreach (var prm in me2.Parameters)
                    yield return prm;

            }
            else if (me is Class)
            {
                Class ce2 = (Class)me;
                foreach (var el in ce2.Descendants())
                    yield return el;

            }
        }

        public static IEnumerable<TypedElement> Descendants(this Class ce)
        {
            foreach (var me in ce.Members)
            {
                yield return me;
                foreach (var me2 in me.Descendants())
                    yield return me2;
            }
        }

        public static IEnumerable<TypedElement> Descendants(this Assembly asm)
        {
            foreach (var ce in asm.Classes)
            {
                yield return ce;
                foreach (var el in ce.Descendants())
                    yield return el;
            }
        }
        public static IEnumerable<Class> DescendantsTypeRefs(this Assembly asm)
        {
            return asm.Descendants().Select(t => t.Type).Where(t => !t.IsNullOrVoid());
        }
        public static IEnumerable<Class> DescendantsAndSelfTypeRefs(this Class ce)
        {
            if (ce.BaseClass != null)
                yield return ce.BaseClass;
            foreach (var ce2 in ce.Interfaces)
                yield return ce2;
            foreach (var ce2 in ce.Descendants().Select(t => t.Type).Where(t => !t.IsNullOrVoid()))
                yield return ce2;
        }
        public static IEnumerable<Class> TypeRefs(this Class ce)
        {
            if (ce.BaseClass != null)
                yield return ce.BaseClass;
            foreach (var ce2 in ce.Interfaces)
                yield return ce2;
        }
        public static IEnumerable<Class> TypeRefs(this Method me)
        {
            if (me.Type != null)
                yield return me.Type;
            foreach (var prm in me.Parameters)
                yield return prm.Type;
        }
        public static IEnumerable<Class> TypeRefs(this Member me)
        {
            if (me is Class)
                return ((Class)me).TypeRefs();
            if (me is Method)
                return ((Method)me).TypeRefs();
            if (me.Type != null)
                return new[] { me.Type };
            return Enumerable.Empty<Class>();
        }
        public static IEnumerable<Class> DescendantsTypeRefs(this Member ce)
        {
            return ce.Descendants().Select(t => t.Type).Where(t => !t.IsNullOrVoid());
        }

        public static IEnumerable<Class> ParameterTypes(this Method me)
        {
            return me.Parameters.Select(t=>t.Type);
        }

        public static bool IsResolved(this Class cr)
        {
            return cr != null;
            //if (cr == null)
            //    return true;
            //if (cr != null)
            //    return true;
            //if (cr.IsArray)
            //    return cr.ArrayItemType != null && cr.ArrayItemType.IsResolved();
            //else if (cr.IsWildcardType)
            //    return true;
            //else if (cr.IsTypeVariable)
            //    return true;
            //return false;

        }



        //public static IEnumerable<ClassRef> Children(this ClassRef ce)
        //{
        //    if (ce.GenericArguments != null)
        //    {
        //        foreach (var ce2 in ce.GenericArguments)
        //            yield return ce2;
        //    }
        //    if (ce.ArrayItemType != null)
        //        yield return ce.ArrayItemType;
        //    if (ce.UpperBounds != null)
        //    {
        //        foreach (var tr in ce.UpperBounds)
        //            yield return tr;
        //    }
        //    if (ce.LowerBounds != null)
        //    {
        //        foreach (var tr in ce.LowerBounds)
        //            yield return tr;
        //    }
        //}
        //public static IEnumerable<ClassRef> Descendants(this ClassRef ce)
        //{
        //    foreach (var ce2 in ce.Children())
        //    {
        //        yield return ce2;
        //        foreach (var ce3 in ce2.Descendants())
        //            yield return ce3;
        //    }
        //}
        //public static IEnumerable<ClassRef> DescendantsAndSelf(this ClassRef ce)
        //{
        //    yield return ce;
        //    foreach (var ce2 in ce.Descendants())
        //        yield return ce2;
        //}

        public static bool EqualsIgnoreGenericMethodArguments(this Class ce, Class ce2)
        {
            if (ce == ce2)
                return true;
            if (ce.IsGenericMethodArgument && ce2.IsGenericMethodArgument)
                return true;
            if (ce.IsArray)
                return ce2.IsArray && ce.ArrayElementType.EqualsIgnoreGenericMethodArguments(ce2.ArrayElementType);
            if (ce.GenericArguments.Count > 0)
            {
                if (ce.GenericArguments.Count != ce2.GenericArguments.Count)
                    return false;
                if (ce.GenericClassDefinition != ce2.GenericClassDefinition)
                    return false;
                return ce.GenericArguments.SequenceEqual(ce2.GenericArguments, (x, y) => x.EqualsIgnoreGenericMethodArguments(y));
            }
            return false;
        }


        public static bool EqualsIgnoreGenericMethodArguments(this List<Class> list1, List<Class> list2)
        {
            return list1.SequenceEqual(list2, (x, y) => x.EqualsIgnoreGenericMethodArguments(y));
        }
        public static bool EqualsTo(this List<Class> list1, List<Class> list2)
        {
            return list1.SequenceEqual(list2);
        }

        public static bool EqualsTo(this Class ce, Class ce2)
        {
            return ce == ce2;
        }


        public static void DoOnceForEach<T>(this IEnumerable<T> list, Action<T, Action<T>> action)
        {
            var set = new HashSet<T>();
            Action<T> redoAction = null;
            redoAction = t =>
                 {
                     if (set.Add(t))
                         action(t, redoAction);
                 };
            foreach (var item in list)
            {
                redoAction(item);
            }
        }


    }
}
