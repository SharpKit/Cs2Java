using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using jar2dll.JsonToCSharp;
using JSharp.CSharp;
using JSharp.JsonToCSharp;
using JSharp.Utils;
using JSharp;

namespace jar2dll.JsonToCSharp
{
    class JJarToAssembly
    {
        public JJar MainJar { get; set; }
        public Assembly MainAssembly { get; set; }

        public void Run()
        {
            MainAssembly = new Assembly();
            MainAssembly.Classes = MainJar.Classes.Select(Visit).ToList();
            Context.Assemblies.Add(MainAssembly);
            Context.Context.IndexClasses();
            OnProcessBases.Run();
            OnProcessMembers.Run();
            OnProcessMemberTypes.Run();
            Console.WriteLine("CompletePartialGenericClasses start");
            Context.Context.CompletePartialGenericClasses();
            Console.WriteLine("CompletePartialGenericClasses end");
            Context.Context.ClearIndex();

            //MainAssembly.Classes.ForEach(FixClass);
            MainAssembly.Classes.ForEach(t => t.Assembly = MainAssembly);
            if (Context.JavaLangObjectClass == null)
                Context.JavaLangObjectClass = Context.Context.GetClass("java.lang.Object");
        }

        public JavaAssemblyContext Context { get; set; }

        #region Import


        Phase OnProcessBases = new Phase { Name = "ProcessBases" };
        Phase OnProcessMembers = new Phase { Name = "ProcessMembers" };
        Phase OnProcessMemberTypes = new Phase { Name = "ProcessMemberTypes" };

        Class Visit(JClass ce)
        {
            var name = ce.Name;
            if (ce.DeclaringClass != null)
            {
                name = name.Substring(name.LastIndexOf("$") + 1);
            }
            var ce2 = new Class
            {
                FullName = name,
                IsInternal = !ce.IsProtected && !ce.IsPrivate && !ce.IsPublic,
                IsInterface = ce.IsInterface,
                IsProtected = ce.IsProtected,
                IsStatic = ce.IsStatic,
                IsSealed = ce.IsFinal,
                IsVirtual = !ce.IsFinal && ce.IsClass,
                IsAbstract = ce.IsAbstract && !ce.IsInterface, //TODO:
            };

            foreach (var ceRef in ce.GenericArguments.NotNull())
            {
                if (ceRef.IsTypeVariable)
                    ce2.GenericArguments.Add(new Class { IsGenericTypeArgument = true, Name = ceRef.Name, GenericClassDefinition = ce2 });
                else
                    throw new Exception();
                //Resolve(ceRef);
            }

            ce2.Members.AddRange(ce.Classes.Where(t => !t.IsSynthetic).Select(Visit));
            ce2.Members.ForEach(t => t.DeclaringClass = ce2);
            OnProcessBases.Do(() =>
            {
                ce2.BaseClass = ResolveAndFix(ce.BaseClass, ce2);
                ce2.Interfaces = ce.Interfaces.NotNull().Select(t => ResolveAndFix(t, ce2)).ToList();
            });
            OnProcessMembers.Do(() =>
                {
                    //ce2.GenericArgumentRefs = ce.GenericArguments.NotNull().Select(Visit).ToList();
                    ce2.Members.AddRange(ce.Fields.Where(t => !t.IsSynthetic).Select(Visit));
                    ce2.Members.AddRange(ce.Ctors.Where(t => !t.IsSynthetic).Select(Visit));
                    ce2.Members.AddRange(ce.Methods.Where(t => !t.IsSynthetic).Select(Visit));
                    ce2.Members.ForEach(t => t.DeclaringClass = ce2);
                });

            OnVisit(ce, ce2);
            return ce2;
        }
        HashSet<Class> Completed = new HashSet<Class>();
        Method Visit(JMethod me)
        {
            var me2 = new Method
            {
                Name = me.Name,
                IsInternal = !me.IsProtected && !me.IsPrivate && !me.IsPublic,
                IsProtected = me.IsProtected,
                IsStatic = me.IsStatic,
                IsVirtual = !me.IsFinal && !me.IsStatic,
                IsAbstract = me.IsAbstract, //TODO:
                //TypeRef = ResolveAndFix(me.Type),
                Parameters = me.Parameters.Select(Visit).ToList(),
                //GenericArguments = me.TypeParameters.Select(Visit).ToList(),
            };

            foreach (var ceRef in me.TypeParameters)
            {
                if (ceRef.IsTypeVariable && ceRef.IsTypeVariableOwnedByMethod)
                    me2.GenericArguments.Add(new Class { IsGenericMethodArgument = true, Name = ceRef.Name, GenericMethodDefinition = me2 });
                else
                    throw new Exception();
                //Resolve(ceRef);
            }
            me2.Parameters.ForEach(t => t.DeclaringMethod = me2);

            OnProcessMemberTypes.Do(() =>
            {
                me2.Type = ResolveAndFix(me.Type, me2);
            });

            OnVisit(me, me2);
            return me2;
        }
        Method Visit(JConstructor me)
        {
            var me2 = new Method
            {
                Name = me.Name,
                IsInternal = !me.IsProtected && !me.IsPrivate && !me.IsPublic,
                IsProtected = me.IsProtected,
                IsStatic = me.IsStatic,
                IsVirtual = !me.IsFinal,
                IsAbstract = me.IsAbstract, //TODO:
                //TypeRef = Visit(me.Type),
                Parameters = me.Parameters.NotNull().Select(Visit).ToList(),
                IsConstructor = true,
            };
            me2.Parameters.ForEach(t => t.DeclaringMethod = me2);
            OnProcessMemberTypes.Do(() =>
            {
                me2.Type = ResolveAndFix(me.Type, me2);
            });

            OnVisit(me, me2);
            return me2;
        }
        Field Visit(JField fe)
        {
            var me2 = new Field
            {
                Name = fe.Name,
                IsInternal = !fe.IsProtected && !fe.IsPrivate && !fe.IsPublic,
                IsProtected = fe.IsProtected,
                IsStatic = fe.IsStatic,
                IsReadOnly = fe.IsFinal,
                //IsVirtual = !fe.IsFinal,
                //TypeRef = ResolveAndFix(fe.Type),
            };
            OnProcessMemberTypes.Do(() =>
            {
                me2.Type = ResolveAndFix(fe.Type, me2.DeclaringClass);
            });
            OnVisit(fe, me2);
            return me2;
        }
        //ClassRef Visit(JClassRef tr)
        //{
        //    if (tr == null)
        //        return null;// new ClassRef { Name = "void" };
        //    var tr2 = new ClassRef
        //    {
        //        Name = tr.Name,
        //        ArrayItemType = Visit(tr.ArrayItemType),
        //        GenericArguments = tr.GenericArguments.NotNull().Select(Visit).ToList(),
        //        IsWildcardType = tr.IsWildcardType,
        //        LowerBounds = tr.LowerBounds.NotNull().Select(Visit).ToList(),
        //        UpperBounds = tr.UpperBounds.NotNull().Select(Visit).ToList(),
        //        IsParameterizedType = tr.IsParameterizedType,
        //        IsTypeVariable = tr.IsTypeVariable,
        //        IsTypeVariableOwnedByMethod = tr.IsTypeVariableOwnedByMethod,
        //        IsArray = tr.IsArray,

        //    };
        //    if (tr2.Name != null && tr2.Name.Contains("$"))
        //    {
        //        tr2.Name = tr2.Name.Replace("$", "+");
        //    }
        //    return tr2;

        //}
        Parameter Visit(JParameter prm)
        {
            var prm2 = new Parameter
            {
                Name = prm.Name,
                //TypeRef = ResolveAndFix(prm.Type),
            };
            OnProcessMemberTypes.Do(() =>
            {
                var me = prm2.DeclaringMethod;
                if (me.Name == "getContent" && me.DeclaringClass.Name == "URLConnection")
                {
                }

                prm2.Type = ResolveAndFix(prm.Type, prm2.DeclaringMethod);
            });

            OnVisit(prm, prm2);
            return prm2;
        }



        public Dictionary<object, object> JToCs = new Dictionary<object, object>();
        void OnVisit(object me, object el)
        {
            JToCs[me] = el;
        }
        #endregion

        #region Resolve


        private Class ResolveAndFix(JClassRef tr)
        {
            return ResolveAndFix(tr, null, null);
        }
        private Class ResolveAndFix(JClassRef tr, Class parentClass)
        {
            return ResolveAndFix(tr, parentClass, null);
        }
        private Class ResolveAndFix(JClassRef tr, Method parentMethod)
        {
            return ResolveAndFix(tr, parentMethod.DeclaringClass, parentMethod);
        }
        private Class ResolveAndFix(JClassRef tr, Class parentClass, Method parentMethod)
        {
            if (tr == null)
                return null;
            var x = Resolve(tr, parentClass, parentMethod);
            if (x == null)
                return null;
            return x;
            //TODO: AddMissingGenerics(tr);
        }

        List<Class> ResolveEach(List<JClassRef> list, Class parentClass, Method parentMethod)
        {
            return list.Select(t => Resolve(t, parentClass, parentMethod)).ToList();
        }
        Class Resolve(JClassRef cr, Class parentClass, Method parentMethod)
        {
            if (cr.IsArray)
            {
                var ce = Resolve(cr.ArrayItemType, parentClass, parentMethod);
                var ce2 = Context.Context.MakeArrayClass(ce);
                return ce2;
            }
            else if (cr.IsWildcardType)
            {
                var upperBounds = ResolveEach(cr.UpperBounds, parentClass, parentMethod);
                var lowerBounds = ResolveEach(cr.LowerBounds, parentClass, parentMethod);

                var ce = Context.WildcardClass;
                if (upperBounds.IsNotNullOrEmpty())
                {
                    var ce2 = upperBounds[0];
                    if (ce2 != Context.JavaLangObjectClass)
                        ce = ce2;
                }
                return ce;
            }
            else if (cr.IsTypeVariable)
            {
                return ResolveTypeVariable(cr, parentClass, parentMethod);
            }
            else
            {
                if (cr.IsParameterizedType)
                {
                    var genArgs = ResolveEach(cr.GenericArguments, parentClass, parentMethod);
                    var ce = GetClass(cr.Name);
                    if (ce != null)
                    {
                        if (ce.GenericArguments.Count == 0 && genArgs.Count == 0)
                            return ce;
                        var ce2 = MakeGenericClass(ce, genArgs);
                        return ce2;
                    }
                    return null;
                }
                else
                {
                    var ce = GetClass(cr.Name);
                    if (ce != null && ce.GenericArguments.Count > 0 && ce.GenericArguments.Any(t => t.IsGenericTypeArgument))
                    {
                        //var genArgs = ce.GenericArguments.Select(t => Context.WildcardClass).ToList();
                        var genArgs = ce.GenericArguments.Select(t => Context.ObjectClass).ToList();
                        var ce2 = MakeGenericClass(ce, genArgs);
                        return ce2;
                    }
                    return ce;
                }
            }

        }

        private Class MakeGenericClass(Class ce, List<Class> genArgs)
        {
            var ce2 = Context.Context.MakeGenericClass(ce, genArgs, true);
            return ce2;
        }

        private Class GetClass(string name)
        {
            name = name.Replace("$", "+");
            var ce = Context.Context.GetClass(name);
            if (ce == null)
                Warn("Can't find class: " + name);
            return ce;
        }

        private Class ResolveTypeVariable(JClassRef cr, Class parentClass, Method parentMethod)
        {
            if (!cr.IsTypeVariable)
                throw new Exception();
            if (cr.IsTypeVariableOwnedByMethod)
            {
                var arg = parentMethod.GenericArguments.Where(t => t.Name == cr.Name).FirstOrDefault();
                if (arg == null)
                {
                    Warn("generic arg not found: " + parentMethod + " " + cr.Name);
                    return null;
                }
                return arg;
            }
            else
            {
                var arg = parentClass.GenericArguments.Where(t => t.Name == cr.Name).FirstOrDefault();
                if (arg == null)
                {
                    Warn("generic arg not found: " + parentClass + " " + cr.Name);
                    return null;
                }
                return arg;
            }
        }

        private void Warn(string p)
        {
            Console.WriteLine("WARNING: {0}", p);
        }

        #endregion

    }


    class Phase
    {
        public string Name { get; set; }
        Action Action;
        public void Do(Action action)
        {
            if (Running)
                throw new Exception();
            Action += action;
        }
        bool Running;
        public void Run()
        {
            if (Action == null)
                return;
            Running = true;
            var x = Action;
            Action = null;
            Console.WriteLine("{0} start", Name);
            x();
            Console.WriteLine("{0} end", Name);
            if (Action != null)
                throw new Exception();
        }
    }
}
