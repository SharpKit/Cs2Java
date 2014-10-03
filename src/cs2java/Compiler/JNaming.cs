using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSharp.Java;
using ICSharpCode.NRefactory.TypeSystem;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    /// <summary>
    /// Provides utilities for proper cs to js naming
    /// </summary>
    static class JNaming
    {
        public static bool IsWildcardArg(this JMemberExpression exp)
        {
            return exp != null && exp.Name == "?";
        }

        public static JMemberExpression JAccessStatic(this IType typeRef)
        {
            return typeRef.JAccess(true);
        }
        public static JMemberExpression JAccess(this IType typeRef)
        {
            return typeRef.JAccess(false);
        }
        static JMemberExpression JAccess(this IType typeRef, bool isStatic)
        {
            if (typeRef == null)
                return null;
            if (typeRef.IsKnownType(KnownTypeCode.Void))
                return J.Member("void");
            if (typeRef.IsKnownType(KnownTypeCode.Boolean))
                return J.Member("boolean");
            if (typeRef.IsKnownType(KnownTypeCode.Int32))
                return J.Member("int");
            if (typeRef.IsKnownType(KnownTypeCode.String))
                return J.Member("String");
            if (typeRef.Kind == TypeKind.Anonymous)
                return J.Member("Object");
            return JName2(typeRef, isStatic);
        }

        public static JMemberExpression JAccessNonPrimitive(this IType typeRef)
        {
            if (typeRef == null)
                return null;
            var x = typeRef.JAccess();
            if (x.Name == "boolean")
                return J.Member("Boolean");
            if (x.Name == "int")
                return J.Member("Integer");
            return x;
        }


        public static JMemberExpression JAccess(this ITypeDefinition me)
        {
            return JName2(me);
        }

        public static JMemberExpression JAccess(this IEntity me)
        {
            if (me == null)
                return null;
            if (me is ITypeDefinition)
                return JAccess((ITypeDefinition)me);
            if (me is IType)
                return JAccess((IType)me);

            var name = JName2(me);
            if (me.IsStatic())
            {
                var member = name;
                if (JMeta.IsGlobalMember(me))
                    return member;
                if (me is IMethod && JMeta.ExtensionImplementedInInstance((IMethod)me))
                    return member;
                member.PreviousMember = JAccessStatic(me.DeclaringType);
                return member;
            }
            else if (me.SymbolKind == SymbolKind.Constructor)
            {
                var att = JMeta.GetJMethodAttribute((IMethod)me);
                if (att != null && att.Name != null) //TODO: hack
                    return J.Member(att.Name);
                var ce = me.DeclaringType;
                var member = (JMemberExpression)JAccess(ce);
                return member;
            }
            return name;
        }

        static JMemberExpression JName2(this IEntity me)
        {
            if (me is IMethod)
                return JName2((IMethod)me);
            else if (me is ITypeDefinition)
                return JName2((ITypeDefinition)me);
            else if (me is IProperty)
                return JName2((IProperty)me);
            else if (me.SymbolKind == SymbolKind.Field)
                return JName2((IField)me);
            return J.Member(me.Name);
        }
        public static JMemberExpression JName2(this ITypeDefinition ce)
        {
            var name = JName2((IType)ce);
            return name;
        }
        static JMemberExpression JName2(this IType ceref, bool omitGenericTypeParameters = false)
        {
            if (ceref.Name == "Feature")
            {
            }
            if (ceref.Kind == TypeKind.TypeParameter)
            {
                return JName2((ITypeParameter)ceref);
            }
            else if (ceref.Kind == TypeKind.Array)
            {
                return JName2((ArrayType)ceref);
            }
            else if (ceref.Kind == TypeKind.ByReference)
            {
                return JName2(((ByReferenceType)ceref).ElementType);
            }
            else if (ceref.Kind == TypeKind.Dynamic) //TODO: Bug in NRefactory?
            {
                //var objType = CompilerTool.Current.Project.Compilation.FindType(KnownTypeCode.Object);
                return J.Member("Object");// JName2(objType);
            }

            var ce = ceref;
            var ceDef = ceref.GetDefinition();

            JMemberExpression name;
            var name2 = ce.Name;
            var att = ceDef.GetJsTypeAttribute();
            if (att != null && att.Name != null)
            {
                name2 = att.Name;
                name = J.Members(name2);
            }
            else
            {

                name = J.Member(name2);
                if (ce.DeclaringType != null)
                    name = JName2(ce.DeclaringType).Member(name);
                else
                {
                    var ns = ce.GetPackageName();
                    if (ns.IsNotNullOrEmpty())
                    {
                        name = J.Member(ns).Member(name);
                    }
                }
            }
            if (!omitGenericTypeParameters && ce.TypeArguments.IsNotNullOrEmpty())
            {
                var list = ce.TypeArguments.Select(t => JAccessNonPrimitive(t)).ToList();
                name.GenericArguments.AddRange(list);
            }
            name.TypeRef = ce;
            return name;
        }

        public static string JName(IEntity me)
        {
            return JName2(me).ToJs();
        }

        public static string JName(ITypeDefinition ce)
        {
            return JName2(ce).ToJs();
        }

        public static string ToJavaNaming(this string s)
        {
            return Char.ToLower(s[0]) + s.Substring(1);
        }

        public static string GetPackageName(this IType ce)
        {
            var ns = ce.Namespace;
            if (ns.IsNotNullOrEmpty())
            {
                ns = ns.Split('.').Select(ToJavaNaming).StringConcat(".");
            }
            return ns;
        }



        public static JMemberExpression EntityMethodToJsFunctionRef(IMethod me)
        {
            var ownerType = me.GetDeclaringTypeDefinition();
            if (JMeta.IsGlobalMethod(me))
            {
                var member = J.Member(JNaming.JName(me));
                return member;
            }
            else
            {
                var member = JNaming.JAccess(ownerType);
                if (!me.IsStatic)
                {
                    if (JMeta.IsNativeType(ownerType))
                        member = member.Member("prototype");
                    else
                        member = member.Member("commonPrototype");
                }
                member = member.Member(JNaming.JName(me));
                return member;
            }
        }



        public static JMemberExpression JName2(IField fe)
        {
            var name = fe.Name.ToJavaNaming();
            var att = fe.GetMetadata<JFieldAttribute>();
            if (att != null && att.Name != null)
                name = att.Name;
            return J.Member(name);

        }

        static JMemberExpression JName2(IMethod me2)
        {
            IMethod me = me2;
            if (me.IsConstructor)
                return J.Member("");
            var att = me.GetMetadata<JMethodAttribute>(true);
            if (att != null && att.Name != null)
            {
                var name = att.Name;
                return J.Member(name);
            }
            var owner = me.GetOwner();
            if (owner != null && owner is IProperty)
            {
                var pe = (IProperty)owner;
                if (pe.SymbolKind == SymbolKind.Indexer && JMeta.UseNativeIndexer(pe))
                    return null;
                var name2 = JName2(pe);
                if (me.IsGetter())
                {
                    if (pe.Name.StartsWith("Is"))
                        name2.Name = name2.Name.ToJavaNaming();
                    else
                        name2.Name = "get" + name2.Name;
                }
                else
                {
                    name2.Name = "set" + name2.Name;
                }
                return name2;
            }
            return J.Member(me.Name.ToJavaNaming());

        }
        static JMemberExpression JName2(IProperty pe)
        {
            var name = pe.Name;
            var att = pe.GetMetadata<JPropertyAttribute>();
            if (att != null && att.Name != null)
            {
                name = att.Name;
            }
            else if (JMeta.IsNativeField(pe))
            {
                name = name.ToJavaNaming();
            }

            return J.Member(JCodeImporter.JsIdentifier(name));
        }

        static JMemberExpression JName2(ArrayType tr)
        {
            var me = JName2(tr.ElementType);
            me.IsArray = true;
            me.TypeRef = tr;
            return me;
        }
        static JMemberExpression JName2(ITypeParameter tr)
        {
            var tr2 = J.Member(tr.Name);
            tr2.TypeRef = tr;
            return tr2;
        }

    }

}
