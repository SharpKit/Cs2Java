using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using JSharp.Java;
using System.Collections.Concurrent;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    static class Extensions_Metadata
    {

        public static T GetMetadataWithCache<T>(this IEnumerable<ResolvedAttribute> list, IEntity me) where T : Attribute
        {
            var ext = me.GetExtension(true);
            object att;
            if (ext.SingleDeclaredAttributeCache == null)
                ext.SingleDeclaredAttributeCache = new Dictionary<Type, object>();
            else if (ext.SingleDeclaredAttributeCache.TryGetValue(typeof(T), out att))
                return (T)att;
            var att2 = list.GetMetadatas<T>().FirstOrDefault();
            ext.SingleDeclaredAttributeCache[typeof(T)] = att2;
            return att2;
        }

        public static T GetMetadata<T>(this IEnumerable<ResolvedAttribute> list) where T : Attribute
        {
            return list.GetMetadatas<T>().FirstOrDefault();
        }

        public static T GetMetadata<T>(this IEntity me) where T : Attribute
        {
            if (me == null)
                return default(T);
            return me.GetAllResolvedAttributes().GetMetadataWithCache<T>(me);
        }
        public static T GetMetadata<T>(this IEntity me, bool inherit) where T : Attribute
        {
            return me.GetMetadatas<T>(inherit).FirstOrDefault();
        }
        public static T GetMetadata<T>(this IAssembly me) where T : Attribute
        {
            return me.GetExtension(true).ResolvedAttributes.GetMetadata<T>();
        }
        public static IEnumerable<T> GetMetadatas<T>(this IEnumerable<ResolvedAttribute> list) where T:Attribute
        {
            return list.FindByType<T>().Select(t => t.ConvertToCustomAttribute<T>()).Where(t => t != null);
        }
        
        
        public static IEnumerable<T> GetMetadatas<T>(this IEntity me) where T : Attribute
        {
            return me.GetExtension(true).AllResolvedAttributes.GetMetadatas<T>();
        }
        public static IEnumerable<T> GetMetadatas<T>(this IEntity me, bool inherit) where T : Attribute
        {
            if (inherit)
            {
                var me2 = me;
                while (me2 != null)
                {
                    var list = me2.GetMetadatas<T>();
                    foreach (var item in list)
                        yield return item;
                    me2 = me2.GetBaseEntityForMetadataInheritance();
                }
            }
            else
            {
                var list = me.GetMetadatas<T>();
                foreach (var item in list)
                    yield return item;
            }
        }

        public static IEnumerable<T> GetMetadatas<T>(this IAssembly asm) where T : Attribute
        {
            return asm.GetExtension(true).ResolvedAttributes.GetMetadatas<T>();
        }



        #region Direct, low-level, slow
        //public static T GetDirectMetadata<T>(this IEnumerable<IAttribute> list) where T : Attribute
        //{
        //    return list.GetDirectMetadatas<T>().FirstOrDefault();
        //}
    
        //public static IEnumerable<T> GetDirectMetadatas<T>(this IEnumerable<IAttribute> list) where T : Attribute
        //{
        //    return list.FindByType<T>().Select(t => t.ConvertToCustomAttribute<T>()).Where(t => t != null);
        //}
        //public static IEnumerable<T> GetDirectMetadatas<T>(this IAssembly me) where T : Attribute
        //{
        //    return me.AssemblyAttributes.GetDirectMetadatas<T>();
        //}
        //public static IEnumerable<IAttribute> GetAttriubtesAndExternal(this IEntity me)
        //{
        //    return me.GetExtension(true).AllResolvedAttributes.Select(t => t.IAttribute);
        //}
        //public static IEnumerable<IAttribute> GetExternalAndParentAttributes(this IEntity me)
        //{
        //    var ext = me.GetExtension(true);
        //    return ext.ExternalResolvedAttributes.Concat(ext.ParentExternalResolvedAttributes).Select(t => t.IAttribute);
        //}

        static ClrResolveVisitor ClrResolveVisitor = new ClrResolveVisitor();
        static object ToClrValue(ResolveResult res)
        {
            var value = ClrResolveVisitor.Visit(res);
            return value;
        }

        static T ConvertToCustomAttribute<T>(this IEnumerable<IAttribute> attributes)
        {
            if (attributes != null)
            {
                foreach (var att in attributes)
                {
                    if (att.AttributeType.FullName == typeof(T).FullName)
                    {
                        var x = ConvertToCustomAttribute<T>(att);
                        if (x != null)
                            return x;
                    }
                }
            }
            return default(T);
        }

        static T[] ConvertToCustomAttributes<T>(this IEnumerable<IAttribute> attributes)
        {
            var list = new List<T>();
            if (attributes != null)
            {
                foreach (var att in attributes)
                {
                    if (att.AttributeType.FullName == typeof(T).FullName)
                    {
                        var att2 = ConvertToCustomAttribute<T>(att);
                        if (att2 != null)
                            list.Add(att2);
                    }
                }
            }
            return list.ToArray();
        }

        public static Type GetMirroredClrType(this IType ce)
        {
            var type = Type.GetType(ce.ReflectionName, true);
            return type;
        }

        static ConcurrentDictionary<IAttribute, object> AttributeCache = new ConcurrentDictionary<IAttribute, object>();
        static int AttributeCacheHits;
        public static T ConvertToCustomAttribute<T>(this IAttribute att, NProject2 globalNamespace = null)
        {
            var att2 = AttributeCache.TryGetValue(att);
            if (att2 != null)
            {
                AttributeCacheHits++;
                return (T)att2;
            }
            //object att2;
            object[] prms = null;
            if (att.PositionalArguments != null)
            {
                var prms2 = new List<object>();
                foreach (var prm in att.PositionalArguments)
                {
                    var value = ToClrValue(prm);
                    prms2.Add(value);
                }
                prms = prms2.ToArray();
            }
            var type = typeof(T);
            //var ctor = type.GetConstructor(prms.Select(t => t.GetType()).ToArray());
            att2 = Activator.CreateInstance(type, prms);
            if (att.NamedArguments != null)
            {
                foreach (var arg in att.NamedArguments)
                {
                    var value = ToClrValue(arg.Value);
                    var pe = att2.GetType().GetProperty(arg.Key.Name);
                    if (pe == null)
                    {
                        throw new CompilerException(att, String.Format("Cannot find property {0} for attribute {1}", arg.Key.Name, type.Name));
                        //continue;
                    }
                    pe.SetValue(att2, value, null);
                }
            }
            if (!IsAttributeSupportedForThisVersion(att2))
                return default(T);
            var x = att2 as ISupportSourceAttribute;
            if (x != null)
                x.SourceAttribute = att;
            AttributeCache[att] = att2;
            return (T)att2;
        }

        public static bool IsAttributeSupportedForThisVersion(object att)
        {
            var att2 = att as ISupportJSharpVersion;
            if (att2 == null || att2.JSharpVersion.IsNullOrEmpty())
                return true;
            var version = att2.JSharpVersion;
            if (!version.Contains("5") && !version.Contains("4+"))
                return false;
            return true;
        }


        public static IEnumerable<IAttribute> FindByType<T>(this IEnumerable<IAttribute> list)
        {
            return list.Where(t => t.AttributeType.FullName == typeof(T).FullName);
        }

        public static IEnumerable<ResolvedAttribute> FindByType<T>(this IEnumerable<ResolvedAttribute> list)
        {
            return list.Where(t => t.MatchesType<T>());
        }

        #endregion

        #region Utils
        public static ITypeDefinition GetDefinitionOrArrayType(this IType ce, CompilerTool compiler)
        {
            var def = ce.GetDefinition();
            if (def == null)
            {
                if (ce.Kind == TypeKind.Array)
                    return Cs.ArrayType(compiler.Project);
            }
            return def;
        }
        static IEntity GetBaseEntityForMetadataInheritance(this IEntity me)
        {
            if (me.SymbolKind == SymbolKind.Method)
            {
                var me2 = (IMethod)me;
                return me2.GetBaseMethod();
            }
            else if (me.SymbolKind == SymbolKind.TypeDefinition)
            {
                var me2 = (ITypeDefinition)me;
                return me2.GetBaseTypeDefinition();
            }
            else
            {
                return null;
            }
        }
 
        #endregion

    }
}
