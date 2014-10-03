using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using jar2dll.JsonToCSharp;
using JSharp.CSharp;
using JSharp.Utils;

namespace JSharp.JsonToCSharp
{
    class JsonToCSharpConverter
    {

        public JavaAssemblyContext Context { get; set; }
        public void Run()
        {
            Do(LoadConvert);
            Do(RemoveMembersWithUnresolvedTypes);
            Do(FixAssembly);
            Do(FilterOutIfNeeded);
            Do(Export);
        }
        public void LoadConvert()
        {
            Do(LoadJJar);
            Do(DoJJarToAssembly);
        }


        public void Export()
        {
            if (OutputDir != null)
                Export(OutputDir);
        }

        public void FilterOutIfNeeded()
        {
            var throwableClass = MainAssembly.GetClass("java.lang.Throwable");
            if (throwableClass != null && throwableClass.Assembly == MainAssembly)
            {
                var list2 = new string[] 
                {
                    "java.lang.*",
                    "java.lang.reflect.*",
                    "java.lang.ref.*",
                    "java.lang.annotation.*",
                    "java.util.*",
                    "java.util.concurrent.*",
                    "java.util.jar.*",
                    "java.util.regex.*",
                    "java.math.*",
                    "java.io.*",
                    "java.nio.*",
                    "java.net.*",
                };
                var list = list2.Select(FindClasses).SelectMany(t => t).ToList();
                var deps = DetectDependencies(list);
                var deps2 = deps.OrderBy(t => t.FullName).ToList();
                MainAssembly.Classes.RemoveAll(t => !deps.Contains(t));
            }
        }

        public void FixAssembly()
        {
            var x = Context.Context.GetClass("java.util.ArrayList");
            var xxx = x.Constructors().Count();
            var xx = x.Members.Where(t => t.Name == "submit").ToList();
            RemoveMembersWithUnresolvedTypes();
            RemoveUnneededInternalClasses(false);
            FixNestedInterfaces();
            Do(RemoveSyntheticBaseClasses);
            Do(RemoveSameParamsMethods);
            Do(RemoveSameSignatureInterfaceMethods);


            var enumerableIface = Context.EnumerableIface;
            var iterableIface = Context.Context.GetClass("java.lang.Iterable");
            if (iterableIface != null && enumerableIface != null)
            {
                var ce = Context.Context.MakeGenericClass(enumerableIface, iterableIface.GenericArguments);
                iterableIface.Interfaces.Add(ce);
            }

            Do(VerifyAllInterfacesImplemented);
            Do(ConvertInterfaceFieldsToProperties);
            TransformIntoProperties();
            Context.Context.GetClass("boolean").IfNotNull(t => t.Name = "bool");
            AutoNameMethodParameters();
            RemoveCtorsWithLastPrmNamedOne();
            RemoveAllStaticCtors();

            ReplaceDollarToUnderscoreInAllElements();
            RemoveAllNestedClassesNamedOne();
            FixDigitNamedClasses();

            FixStaticMembersInInterfaces();
            //Do(ConvertInterfaceFieldsToProperties);
            //TransformIntoProperties();
            SetMethodVirtualOverrideAndNew();
            //ExplicitlyImplementInterfaces();
            AddPrivateCtors();

            RenameInvalidMemberNames();
            FixGenericRenamedClasses();
            Do(SortMembers);
        }

        private void RemoveSameSignatureInterfaceMethods()
        {
            Context.Context.ClassesAndNestedAndMadeGenerics(MainAssembly).Where(t => t.IsInterface && t.Interfaces.Count > 0).ForEach(RemoveSameSignatureInterfaceMethods);
        }

        private void RemoveSameSignatureInterfaceMethods(Class ce)
        {
            foreach (var me in ce.Methods().ToList())
            {
                if (me.Name == "headMap" && me.DeclaringClass.Name == "NavigableMap" && me.Parameters.Count==1)
                {
                }
                var me2 = TryFindBaseInterfaceMethod(me);
                if (me2 != null)
                    me.Remove();
            }
        }

        private void FixGenericRenamedClasses()
        {
            foreach (var ce in Context.Context.MadeGenericClasses)
            {
                if (ce.Name != ce.GenericClassDefinition.Name)
                    ce.Name = ce.GenericClassDefinition.Name;
            }
        }

        private void SortMembers()
        {
            MainAssembly.ClassesAndNested.ForEach(t => SortMembers(t));
        }

        private void SortMembers(Class ce)
        {
            ce.Members = ce.Members.OrderBy(t => t.GetType().Name).ThenBy(t => t.Name).ToList();
        }

        private void RemoveSameParamsMethods()
        {
            MainAssembly.ClassesAndNested.ForEach(RemoveSameParamsMethods);
        }

        private void RemoveSameParamsMethods(Class ce)
        {
            if (ce.Name == "Scanner")
            {
            }
            var methods = ce.Methods().ToList();
            foreach (var me in methods)
            {
                if (me.DeclaringClass == null)
                    continue;
                var same = FindMethodsWithSameParams(me, methods).ToList();
                if (same.Count == 0)
                    continue;
                same.Add(me);
                var me2 = same.Where(t => t.Type != null && t.Type != Context.JavaLangObjectClass).FirstOrDefault();
                if (me2 == null)
                    me2 = same.First();
                same.Remove(me2);
                same.ForEach(t => t.Remove());

            }
        }

        IEnumerable<Method> FindMethodsWithSameParams(Method sig, List<Method> methods)
        {
            foreach (var me in methods)
            {
                if (me == sig)
                    continue;
                bool matchWithoutReturnType;
                var match = MatchesSignatureAndName(sig, me, out matchWithoutReturnType);
                if (!match && !matchWithoutReturnType)
                    continue;
                yield return me;
            }
        }

        void RemoveMembersWithUnresolvedTypes()
        {
            var throwableClass = Context.Context.GetClass("java.lang.Throwable");
            if (throwableClass != null)
                throwableClass.BaseClass = Context.Context.GetClass("System.Exception");

            Context.Context.ClassesAndNestedAndMadeGenerics(MainAssembly).ToList().ForEach(RemoveMembersWithUnresolvedTypes);
        }


        JJarToAssembly JJarToAssembly;
        private void DoJJarToAssembly()
        {
            JJarToAssembly = new JJarToAssembly { Context = Context, MainJar = MainJar, MainAssembly = MainAssembly };
            JJarToAssembly.Run();
            MainAssembly = JJarToAssembly.MainAssembly;

        }

        private void LoadJJar()
        {
            Do(DeserializeJar);

            Info(MainJar.Classes.Count);

            MainJar.Classes = MainJar.Classes.OrderBy(t => t.Name).ToList();
            MainJar.Classes.RemoveAll(t => t.IsAnonymousClass || t.IsSynthetic);
            MainJar.Classes.ForEach(FixClass);

            Info(MainJar.Classes.Count);
        }
        public string OutputDir { get; set; }

        void VerifyAllInterfacesImplemented()
        {
            foreach (var ce in MainAssembly.ClassesAndNested)
            {
                if (ce.IsInterface)
                    continue;
                foreach (var iface in ce.Interfaces)
                {
                    if (iface.GenericArguments.IsNotNullOrEmpty())
                    {
                        if (iface == null || iface.GenericClassDefinition == null)
                        {
                            Warn(iface + " is not made from generic type definition");
                            continue;//implement generic interfaces is not supported
                        }
                    }
                    if (iface != null && iface.IsInterface)
                        VerifyImplementInterface(iface, ce);
                }
            }
        }

        private void TransformIntoProperties()
        {
            MainAssembly.ClassesAndNested.DoOnceForEach(TransformIntoProperties);
        }

        IEnumerable<Class> FindClasses(string q)
        {
            if (q.EndsWith(".*"))
            {
                return MainAssembly.Classes.Where(t => t.Namespace == q.Substring(0, q.Length - 2));
            }
            return new Class[] { MainAssembly.GetClass(q) };
        }

        string ExtractPropertyName(string name)
        {
            if (name.Length > 3 && name.StartsWith("get") && Char.IsUpper(name[3]))
                return name.Substring(3);
            if (name.Length > 2 && name.StartsWith("is") && Char.IsUpper(name[2]))
                return ToCSharpNaming(name);
            return null;

        }
        string ToJavaPropertySetterName(string propName)
        {
            if (propName.StartsWith("Is"))
                return "set" + propName.Substring(2);
            return "set" + propName;
        }
        static string ToCSharpNaming(string name)
        {
            if (name.Length == 1)
                return name.ToUpper();
            return Char.ToUpper(name[0]) + name.Substring(1);
        }
        private void TransformIntoProperties(Class ce, Action<Class> doNow)
        {
            foreach (var me in ce.Methods().ToList())
            {
                if (me.ExplicitImplementationOfInterfaceMethod != null)
                    doNow(me.ExplicitImplementationOfInterfaceMethod.DeclaringClass);

                var name = me.Name;
                if (me.Parameters.IsNotNullOrEmpty())
                    continue;
                if (me.GenericArguments.IsNotNullOrEmpty())
                    continue;
                if (me.Type.IsNullOrVoid())
                    continue;
                var propName = ExtractPropertyName(me.Name);
                if (propName == null)
                    continue;
                var setterName = ToJavaPropertySetterName(propName);
                var setter = ce.Methods().Where(t => t.Name == setterName && t.Parameters.Count == 1 && t.Parameters[0].Type == me.Type).FirstOrDefault();
                //if (ce.Members.Any(t => t.Name == propName))
                //    continue;
                var pe = new Property { Name = propName, Type = me.Type, DeclaringClass = ce };
                if (setter == null)
                    pe.IsReadOnly = true;
                if (me.ExplicitImplementationOfInterfaceMethod != null)
                {
                    var ipe = me.ExplicitImplementationOfInterfaceMethod.DeclaringClass.Properties().Where(t => t.Name == propName).Single();
                    pe.ExplicitImplementationOfInterfaceProperty = ipe;
                }
                ce.Members.Add(pe);
                ce.Members.Remove(me);
            }
        }
        private void RemoveSyntheticBaseClasses()
        {
            foreach (var ce in MainAssembly.ClassesAndNested)
            {
                if (ce.BaseClass != null && !ce.BaseClass.IsResolved())
                {
                    //ce.BaseClass.GenericArguments;
                    ce.BaseClass.IsArray = false;
                    ce.BaseClass = Context.JavaLangObjectClass;
                }
                ce.Interfaces.RemoveAll(t => !t.IsResolved());
            }
        }

        private void RemoveUnneededInternalClasses(bool remove)
        {
            Info("Indexing");
            Context.Context.IndexClasses();
            Info("Done");
            var list = MainAssembly.Classes.Where(t => t.IsInternal).ToList();
            foreach (var ce in list)
            {
                if (Context.Context.GetDerivedClasses(ce).Any(t => t.IsPublic))
                {
                    ce.IsPublic = true;
                    continue;
                }
                if (remove)
                    ce.Remove();
            }
            Context.Context.ClearIndex();
        }


        #region Fixes
        private void RenameInvalidMemberNames()
        {
            foreach (var ce in MainAssembly.ClassesAndNested)
            {
                ce.Members.Where(t => t.Name == ce.Name).ForEach(t => t.Name = t.Name + "___");
                foreach (var pe in ce.PropertiesAndFields())
                {
                    if (ce.Methods().Any(t => t.Name == pe.Name))
                    {
                        pe.Name = "___" + pe.Name;
                    }
                }
            }
        }

        ////TODO:?
        //private void AddMissingGenerics(Class tr)
        //{
        //    if (tr != null && tr.GenericArguments.IsNotNullOrEmpty())
        //    {
        //        if (tr.GenericArguments.IsNullOrEmpty())
        //        {
        //            foreach (var arg in tr.GenericArguments)
        //            {
        //                tr.GenericArguments.Add(new ClassRef { Name = "Q", ResolvedClass = Context.WildcardClass });
        //            }
        //            tr.GenericArguments.ForEach(Resolve);
        //        }
        //    }
        //    if (tr.ArrayItemType != null)
        //        AddMissingGenerics(tr.ArrayItemType);
        //    if (tr.GenericArguments != null)
        //        tr.GenericArguments.ForEach(AddMissingGenerics);
        //    if (tr.UpperBounds != null)
        //        tr.UpperBounds.ForEach(AddMissingGenerics);
        //    if (tr.LowerBounds != null)
        //        tr.LowerBounds.ForEach(AddMissingGenerics);
        //}


        private void AddPrivateCtors()
        {
            foreach (var ce in MainAssembly.ClassesAndNested.Where(t => !t.IsInterface && t.Constructors().FirstOrDefault() == null))
            {
                ce.Members.Add(new Method { IsConstructor = true, IsPrivate = true, DeclaringClass = ce });
            }
        }

        private void ExplicitlyImplementInterfaces()
        {
            foreach (var ce in MainAssembly.ClassesAndNested.Where(t => !t.IsInterface && t.Interfaces.Count > 0))
            {
                foreach (var iface in ce.Interfaces.Select(t => t).Where(t => t != null).ToList())
                {
                    foreach (var ime in iface.Methods())
                    {
                    }
                }
            }
        }


        private void SetMethodVirtualOverrideAndNew()
        {
            foreach (var ce in MainAssembly.ClassesAndNested.Where(t => t.BaseClass != null && t.BaseClass != null))
            {
                var baseCe = ce.BaseClass;
                foreach (var me in ce.Methods().Where(t => t.IsVirtual))
                {

                    var baseMe = TryFindBaseMethod(me);
                    if (baseMe != null)
                    {
                        if (baseMe.IsProtected != me.IsProtected)
                        {
                            me.IsNew = true;
                            me.IsVirtual = true;
                            me.IsOverride = false;
                        }
                        else
                        {
                            me.IsOverride = true;
                            me.IsVirtual = false;
                        }
                    }
                }
            }
        }

        Method TryFindBaseMethod(Method me)
        {
            var ce = me.DeclaringClass;
            if (ce.BaseClass == null)
                return null;
            var baseCe = ce.BaseClass;
            while (baseCe != null)
            {
                var baseMe = baseCe.Methods().Where(t => t.Name == me.Name && MatchesSignature(me, t)).FirstOrDefault();
                if (baseMe != null)
                    return baseMe;

                if (baseCe.BaseClass == null)
                    return null;
                baseCe = baseCe.BaseClass;
            }
            return null;
        }
        Method TryFindBaseInterfaceMethod(Method me)
        {
            var ce = me.DeclaringClass;
            foreach (var baseCe in ce.AllInterfaces())
            {
                var baseMe = baseCe.Methods().Where(t => t.Name == me.Name && MatchesSignature(me, t)).FirstOrDefault();
                if (baseMe != null)
                    return baseMe;

                if (baseCe.BaseClass == null)
                    return null;
            }
            return null;
        }
        void VerifyImplementInterface(Class iface, Class ce)
        {
            foreach (var me in iface.AllInterfacesAndSelfInterfaceMembers().OfType<Method>().ToList())
            {
                ImplementMethodIfNotExist(me, ce);
            }
        }
        void ImplementMethodIfNotExist(Method meSig, Class ce)
        {
            var isPartialMatch = false;
            foreach (var me2 in ce.AllMethods())
            {
                bool partialMatch;
                if (MatchesSignatureAndName(meSig, me2, out partialMatch))
                    return;
                if (partialMatch)
                    isPartialMatch = partialMatch;
            }
            var prm = meSig.Parameters.FirstOrDefault();
            if (prm != null && prm.Type == null)
            {
            }
            var me = new Method
            {
                Name = meSig.Name,
                Type = meSig.Type,
                Parameters = meSig.Parameters.Select(t => new Parameter { Name = t.Name, Type = t.Type }).ToList(),
                GenericArguments = meSig.GenericArguments.ToList(),
                DeclaringClass = ce,
            };
            if (isPartialMatch)
            {
                me.ExplicitImplementationOfInterfaceMethod = meSig;
            }
            else
            {
                me.IsPublic = true;
                me.IsVirtual = true;
            }
            ce.Members.Add(me);
        }
        private bool MatchesSignature(Method me, Method me2)
        {
            bool x;
            return MatchesSignature(me, me2, out x);
        }
        private bool MatchesSignatureAndName(Method me, Method me2, out bool matchExceptReturnType)
        {
            if (me.Name != me2.Name)
            {
                matchExceptReturnType = false;
                return false;
            }
            return MatchesSignature(me, me2, out matchExceptReturnType);
        }
        private bool MatchesSignature(Method me, Method me2, out bool matchExceptReturnType)
        {
            if (me.Name == "readFully" && me2.Name == "readFully")
            {
            }
            if (me.Name == me2.Name && me.Parameters.Count == me2.Parameters.Count)
            {
            }
            matchExceptReturnType = false;
            var list1 = me.Parameters.Select(t => t.Type).ToList();
            var list2 = me2.Parameters.Select(t => t.Type).ToList();
            if (!list1.EqualsIgnoreGenericMethodArguments(list2))
                return false;
            if (!me.Type.EqualsIgnoreGenericMethodArguments(me2.Type))
            {
                matchExceptReturnType = true;
                return false;
            }
            return true;
        }

        private void FixStaticMembersInInterfaces()
        {
            var list = MainAssembly.ClassesAndNested.Where(t => t.IsInterface).ToList();
            foreach (var ce in list)
            {
                var list2 = ce.MembersExceptClasses().Where(m => m.IsStatic).ToList();
                ce.Members.RemoveAll(t => list2.Contains(t));
            }
        }

        private HashSet<Class> DetectDependencies(List<Class> list)
        {
            var list2 = new HashSet<Class>();
            foreach (var ce in list)
                DetectDependencies(ce, list2);
            return list2;
        }

        private HashSet<Class> DetectDependencies(Class ce)
        {
            var list = new HashSet<Class>();
            DetectDependencies(ce, list);
            return list;
        }

        private void DetectDependencies(Class ce, HashSet<Class> list)
        {
            if (ce == null)
                return;
            if (ce.IsGenericTypeArgument || ce.IsGenericMethodArgument)
                return;
            if (ce.GenericClassDefinition != null)
            {
                DetectDependencies(ce.GenericClassDefinition, list);
                ce.GenericArguments.ForEach(t => DetectDependencies(t, list));
                return;
            }
            else if (ce.IsArray)
            {
                DetectDependencies(ce.ArrayElementType, list);
                return;
            }
            if (!list.Add(ce))
                return;
            DetectDependencies(ce.BaseClass, list);
            ce.Interfaces.ForEach(t => DetectDependencies(t, list));
            ce.Classes().ForEach(t => DetectDependencies(t, list));
            ce.Members.Select(t => t.Type).ForEach(t => DetectDependencies(t, list));
            foreach (var me in ce.MethodsAndConstructors())
            {
                me.ParameterTypes().ForEach(t => DetectDependencies(t, list));
            }
        }

        private void ConvertInterfaceFieldsToProperties()
        {
            var list = MainAssembly.ClassesAndNested.Where(t => t.IsInterface).ToList();
            foreach (var type in list)
            {
                if (type.Fields().FirstOrDefault() != null)
                {
                    foreach (var field in type.Fields().Where(t => !t.IsStatic).ToList())
                    {
                        type.Members.Remove(field);
                        type.Members.Add(new Property
                        {
                            Name = field.Name,
                            IsReadOnly = field.IsReadOnly,
                            Type = field.Type,
                            DeclaringClass = field.DeclaringClass,
                        });
                    }
                }
            }
        }

        private void RemoveAllNestedClassesNamedOne()
        {
            foreach (var ce in MainAssembly.Classes)
            {
                foreach (var ce2 in ce.Members.OfType<Class>().ToList())
                {
                    if (ce2.Name == "1")
                        ce.Members.Remove(ce2);
                }
            }
        }

        private void ReplaceDollarToUnderscoreInAllElements()
        {
            foreach (var me in MainAssembly.Descendants().Where(t => t.Name.Contains("$")))
            {
                me.Name = me.Name.Replace("$", "_");
            }
        }

        private void RemoveAllStaticCtors()
        {
            foreach (var ce in MainAssembly.ClassesAndNested)
            {
                var staticCtors = ce.Members.OfType<Method>().Where(t => t.IsStatic && t.IsConstructor).ToList();
                staticCtors.ForEach(t => ce.Members.Remove(t));
            }
        }

        private void RemoveCtorsWithLastPrmNamedOne()
        {
            foreach (var me in MainAssembly.Descendants().OfType<Method>().ToList())
            {
                if (me.Parameters.Where(t => !t.Type.IsNullOrVoid() && t.Type.Name != null && t.Type.Name.Contains(".1")).FirstOrDefault() != null)
                    me.Remove();
            }
        }

        private void AutoNameMethodParameters()
        {
            foreach (var me in MainAssembly.Descendants().OfType<Method>())
            {
                var index = 0;
                foreach (var prm in me.Parameters)
                {
                    if (prm.Name.IsNullOrEmpty())
                    {
                        index++;
                        prm.Name = "prm" + index;
                    }
                }
            }
        }

        //private void AddVoidTypeRefs()
        //{
        //    foreach (var me in MainAssembly.Descendants().OfType<TypedElement>().Where(t => t.TypeRef == null))
        //    {
        //        me.TypeRef = new ClassRef { Name = "void" };
        //    }
        //}

        private void FixDigitNamedClasses()
        {
            MainAssembly.ClassesAndNested.Where(t => Char.IsDigit(t.Name[0])).ForEach(t => t.Name = "_" + t.Name);
        }

        private void FixNestedInterfaces(Class ce)
        {
            if (!ce.IsInterface)
                return;
            if (ce.Classes().FirstOrDefault() == null)
                return;
            foreach (var nestedCe in ce.Classes().ToList())
            {
                ce.Members.Remove(nestedCe);
                nestedCe.Name = ce.Name + "_" + nestedCe.Name;
                if (ce.DeclaringClass != null)
                {
                    ce.DeclaringClass.Members.Add(nestedCe);
                    nestedCe.DeclaringClass = ce.DeclaringClass;
                }
                else
                {
                    nestedCe.Namespace = ce.Namespace;
                    MainAssembly.Classes.Add(nestedCe);
                    nestedCe.IsProtected = false;

                    nestedCe.DeclaringClass = null;
                }
                FixNestedInterfaces(nestedCe);
            }
        }
        private void FixNestedInterfaces()
        {
            MainAssembly.Classes.ToList().ForEach(FixNestedInterfaces);
        }

        #endregion

        private void Export(string dir)
        {
            var x = 0;
            var total = MainAssembly.ClassesAndNested.Count();
            new CodeModelExporter
            {
                Assembly = MainAssembly,
                Context = Context.Context,
                OutputDir = dir,
                ExportingClass = ce => Console.WriteLine("{0}/{1} {2}", x++, total, ce),
                CreateCsProj = true,
            }.Export();
        }



        void FixClass(JClass ce)
        {
            ce.Classes = ce.Classes.NotNull();
            ce.Methods = ce.Methods.NotNull();
            ce.Fields = ce.Fields.NotNull();
            ce.Ctors = ce.Ctors.NotNull();

            ce.Classes.RemoveAll(t => t.IsAnonymousClass || IsNonPublic(t) || t.IsSynthetic);
            ce.Fields.RemoveAll(t => IsNonPublic(t) || t.IsSynthetic);
            ce.Methods.RemoveAll(t => IsNonPublic(t) || t.IsSynthetic);
            ce.Ctors.RemoveAll(t => IsNonPublic(t) || t.IsSynthetic);

            ce.Methods.ForEach(t => t.Parameters = t.Parameters.NotNull());
            ce.Methods.ForEach(t => t.TypeParameters = t.TypeParameters.NotNull());
            ce.Ctors.ForEach(t => t.Parameters = t.Parameters.NotNull());
            ce.Ctors.ForEach(t => t.TypeParameters = t.TypeParameters.NotNull());

            ce.Classes.ForEach(t => t.DeclaringClass = ce);
            ce.Methods.ForEach(t => t.DeclaringClass = ce);
            ce.Fields.ForEach(t => t.DeclaringClass = ce);
            ce.Ctors.ForEach(t => t.DeclaringClass = ce);

            ce.Methods.RemoveAll(t => t.Name.Contains("$"));
            ce.Classes.ForEach(FixClass);

            ce.Ctors.RemoveAll(t => t.TypeParameters.IsNotNullOrEmpty());


        }


        public string JarJsonFilename { get; set; }
        private void DeserializeJar()
        {
            var ser = new DataContractJsonSerializer(typeof(JJar));
            //var reader = new MemoryStream(Encoding.Default.GetBytes("{ }"));
            var obj = ser.ReadObject(new FileStream(JarJsonFilename, FileMode.Open));
            MainJar = (JJar)obj;
            //var ser = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            //var MainJar = ser.Deserialize<JJar>(File.ReadAllText(@"out.json"));
            //return MainJar;
        }


        #region Resolve

        //private void ResolveTypeRefs(Assembly asm)
        //{
        //    Context.Context.IndexClasses();
        //    foreach (var ce in asm.Classes)
        //    {
        //        ResolveTypeRefs(ce);
        //    }
        //    Context.Context.ClearIndex();
        //}

        //Class CurrentClass;
        //Method CurrentMethod;
        //private void ResolveTypeRefs(Class ce)
        //{
        //    var prevCurrent = CurrentClass;
        //    CurrentClass = ce;
        //    if (ce.GenericArgumentRefs.IsNotNullOrEmpty())
        //    {
        //        foreach (var ceRef in ce.GenericArgumentRefs)
        //        {
        //            if (ceRef.IsTypeVariable)
        //                ceRef.ResolvedClass = new Class { IsGenericTypeArgument = true, Name = ceRef.Name, GenericClassDefinition = ce };
        //            else
        //                Resolve(ceRef);
        //        }
        //    }

        //    ResolveAndFix(ce.BaseClass);
        //    ce.Interfaces.ForEach(ResolveAndFix);

        //    foreach (var me in ce.MethodsAndConstructors())
        //    {
        //        CurrentMethod = me;
        //        foreach (var ceRef in me.GenericArguments)
        //        {
        //            if (ceRef.IsTypeVariable && ceRef.IsTypeVariableOwnedByMethod)
        //                ceRef.ResolvedClass = new Class { IsGenericMethodArgument = true, Name = ceRef.Name, GenericMethodDefinition = me };
        //            else
        //                Resolve(ceRef);
        //        }
        //        ResolveAndFix(me.TypeRef);
        //        me.Parameters.ForEach(t => ResolveAndFix(t.TypeRef));
        //        CurrentMethod = null;
        //        //me.DescendantsTypeRefs().ForEach(ResolveAndFix);
        //    }
        //    foreach (var me in ce.PropertiesAndFields())
        //    {
        //        ResolveAndFix(me.TypeRef);
        //    }
        //    foreach (var ce2 in ce.Classes())
        //    {
        //        ResolveTypeRefs(ce2);
        //    }
        //    RemoveMembersWithUnresolvedTypes(ce);
        //    CurrentClass = prevCurrent;
        //}



        private void RemoveMembersWithUnresolvedTypes(Class ce)
        {
            foreach (var me in ce.MethodsAndConstructors().ToList())
            {
                if (ce.Name == "BasicPermission" && me.IsConstructor)
                {
                }
                if (me.Parameters.Any(x => x.Type==null))
                    me.Remove();
                else if (!me.IsConstructor && me.Type==null)
                    me.Remove();
            }
            foreach (var me in ce.Fields().ToList())
            {
                if (!me.Type.IsResolved())
                    me.Remove();
            }
        }

        //private void ResolveAndFix(ClassRef tr)
        //{
        //    if (tr == null)
        //        return;
        //    Resolve(tr);
        //    AddMissingGenerics(tr);
        //}

        //void Resolve(ClassRef cr)
        //{
        //    if (cr.ResolvedClass != null)
        //        return;
        //    if (cr.IsArray)
        //    {
        //        Resolve(cr.ArrayItemType);
        //        cr.ResolvedClass = Context.Context.MakeArrayClass(cr.ArrayItemType.ResolvedClass);
        //    }
        //    else if (cr.IsWildcardType)
        //    {
        //        cr.UpperBounds.ForEach(Resolve);
        //        cr.LowerBounds.ForEach(Resolve);
        //        cr.ResolvedClass = Context.WildcardClass;
        //        if (cr.UpperBounds.IsNotNullOrEmpty())
        //        {
        //            var ce2 = cr.UpperBounds[0];
        //            if (ce2.ResolvedClass != Context.JavaLangObjectClass)
        //                cr.ResolvedClass = ce2.ResolvedClass;
        //        }
        //    }
        //    else if (cr.IsTypeVariable)
        //    {
        //        if (cr.IsTypeVariableOwnedByMethod)
        //        {
        //            var arg = CurrentMethod.GenericArguments.Where(t => t.ResolvedClass.Name == cr.Name).FirstOrDefault();
        //            if (arg == null)
        //                Warn("generic arg not found: " + CurrentMethod + " " + cr.Name);
        //            else
        //                cr.ResolvedClass = arg.ResolvedClass;
        //        }
        //        else
        //        {
        //            var arg = CurrentClass.GenericArgumentRefs.Where(t => t.ResolvedClass.Name == cr.Name).FirstOrDefault();
        //            if (arg == null)
        //                Warn("generic arg not found: " + CurrentClass + " " + cr.Name);
        //            else
        //                cr.ResolvedClass = arg.ResolvedClass;
        //        }
        //    }
        //    else
        //    {
        //        if (cr.IsParameterizedType)
        //        {
        //            cr.GenericArguments.ForEach(Resolve);
        //            var ce = Context.Context.GetClass(cr.Name);
        //            if (ce != null)
        //                cr.ResolvedClass = Context.Context.MakeGenericClass(ce, cr.GenericArguments.Select(t => t.ResolvedClass).ToList());
        //        }
        //        else
        //        {
        //            cr.ResolvedClass = Context.Context.GetClass(cr.Name);
        //        }
        //        if (cr.ResolvedClass == null)
        //            Warn("Can't find class: " + cr.Name);
        //    }

        //}

        #endregion



        public JJar MainJar { get; set; }
        public Assembly MainAssembly { get; set; }


        #region Utils
        private void Warn(string p)
        {
            Console.WriteLine("WARNING: {0}", p);
        }
        private static string Quote(string p)
        {
            return "\"" + p + "\"";
        }

        void Do(Action action)
        {
            var name = action.Method.Name;
            Console.WriteLine("{0} {1} start", DateTime.Now, name);
            action();
            Console.WriteLine("{0} {1} end", DateTime.Now, name);
        }
        static bool IsNonPublic(JMember me)
        {
            return !me.IsPublic && !me.IsProtected;
        }
        private void Info(object p)
        {
            Console.WriteLine(p);
        }
        #endregion







    }
}
