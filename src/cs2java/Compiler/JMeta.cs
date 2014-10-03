using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSharp.Java;
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    static class JMeta
    {

        public static string DirectorySeparator = Path.DirectorySeparatorChar.ToString();

        //static JExportAttribute _JsExportAttribute;
        //public static JExportAttribute GetJsExportAttribute()
        //{
        //    if (_JsExportAttribute == null)
        //    {
        //        _JsExportAttribute = GetJsExportAttribute(CompilerTool.Current.Project.Compilation.MainAssembly);
        //    }
        //    return _JsExportAttribute;
        //}
        public static JExportAttribute GetJsExportAttribute(IAssembly asm)
        {
            return asm.GetMetadata<JExportAttribute>();
        }

        public static string GetExportPath(ITypeDefinition ce)
        {
            var att = ce.GetJsTypeAttribute();
            string path;
            if (att != null && att.Filename.IsNotNullOrEmpty())
            {
                path = att.Filename.Replace("/", JMeta.DirectorySeparator);
                if (path.StartsWith(@"~\") || path.StartsWith(@"~/"))
                    path = path.Substring(2);
                else
                    path = Path.Combine(Path.GetDirectoryName(ce.GetFileOrigin()), path);
                var asm = ce.ParentAssembly;
                var att2 = asm.GetMetadata<JExportAttribute>();
                if (att2 != null && att2.FilenameFormat.IsNotNullOrEmpty())
                    path = String.Format(att2.FilenameFormat, path);
            }
            else
            {
                path = GetDefaultJsFilename(ce);
            }
            return path;
        }
        private static string GetDefaultJsFilename(ITypeDefinition ce)
        {
            var asm = ce.ParentAssembly;
            var s = "res" + JMeta.DirectorySeparator + asm.AssemblyName + ".js";
            var att = asm.GetMetadata<JExportAttribute>();
            if (att != null)
            {
                if (att.DefaultFilename.IsNotNullOrEmpty())
                {
                    s = att.DefaultFilename;
                }
                else if (att.DefaultFilenameAsCsFilename)
                {
                    var filename = ce.GetFileOrigin();
                    filename = Path.ChangeExtension(filename, ".js");
                    if (att.FilenameFormat.IsNotNullOrEmpty())
                        filename = String.Format(att.FilenameFormat, filename);
                    s = filename;
                }
            }
            return s.Replace("/", JMeta.DirectorySeparator);
        }


        #region JsMethodAttribute
        public static JMethodAttribute GetJMethodAttribute(IMethod me)
        {
            if (me == null)
                return null;
            return me.GetMetadata<JMethodAttribute>(true);
        }
        public static bool UseNativeOverloads(IMethod me)
        {
            if (me.IsPropertyAccessor())
                return true;
            if (me.IsEventAccessor())
                return true;
            JMethodAttribute jma = me.GetMetadata<JMethodAttribute>(true);
            if (jma != null && jma._NativeOverloads != null)
                return jma._NativeOverloads.GetValueOrDefault();

            var t = me.GetDeclaringTypeDefinition();
            if (t != null)
            {
                return UseNativeOverloads(t);
            }
            else
            {
                return false; //Not declared on method, not declared on type
            }

        }
        public static string GetNativeCode(IMethod me)
        {
            JMethodAttribute jma = me.GetMetadata<JMethodAttribute>();
            return (jma == null) ? null : jma.Code;
        }
        public static bool ExtensionImplementedInInstance(IMethod me)
        {
            JMethodAttribute jma = me.GetMetadata<JMethodAttribute>();
            return (jma == null) ? false : jma.ExtensionImplementedInInstance;
        }
        public static bool IgnoreGenericMethodArguments(IMethod me)
        {
            if (me == null)
                return false;
            return MD_JMethodOrJType(me, t => t._IgnoreGenericArguments, t => t._IgnoreGenericMethodArguments).GetValueOrDefault();
        }
        public static bool IsGlobalMethod(IMethod me)
        {
            var att = me.GetMetadata<JMethodAttribute>(true);
            if (att != null && att._Global != null)
                return att._Global.Value;
            var owner = me.GetOwner();
            if (owner != null && owner is IProperty)
            {
                return IsGlobalProperty((IProperty)owner);
            }
            return IsGlobalType(me.GetDeclaringTypeDefinition());

        }

        #endregion

        #region JsEventAttribute
        public static JsEventAttribute GetJsEventAttribute(IEntity me) //TODO: implement
        {
            return me.GetMetadata<JsEventAttribute>();
        }
        #endregion

        #region JsPropertyAttribute
        public static JPropertyAttribute GetJsPropertyAttribute(IProperty pe)
        {
            return pe.GetMetadata<JPropertyAttribute>();
        }
        public static bool IsNativeField(IProperty pe)
        {
            var jpa = pe.GetMetadata<JPropertyAttribute>();
            if (jpa != null)
                return jpa.NativeField;
            var att = GetJsTypeAttribute(pe.GetDeclaringTypeDefinition());//.GetMetadata<JsTypeAttribute>(true);
            if (att != null)
            {
                if (att.PropertiesAsFields)
                    return true;
                else if (att.AutomaticPropertiesAsFields && pe.IsAutomaticProperty(null))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static bool UseNativeIndexer(IProperty pe)
        {
            return pe.MD<JPropertyAttribute, bool>(t => t.NativeIndexer);
        }

        public static bool IsNativeProperty(IProperty pe)
        {
            if (IsNativeField(pe))
                return false;
            var x = MD_JPropertyOrJType(pe, t => t._NativeProperty, t => t._NativeProperties);
            return x.GetValueOrDefault();
            //var attr = GetJsPropertyAttribute(pe);
            //return attr != null && attr.NativeProperty;
        }

        public static bool IsNativePropertyEnumerable(IProperty pe)
        {
            var x = MD_JPropertyOrJType(pe, t => t._NativePropertyEnumerable, t => t._NativePropertiesEnumerable);
            return x.GetValueOrDefault(); ;
        }

        #endregion

        #region JsExport

        public static bool IsJsExported(IEntity me)
        {
            var ext = me.GetExtension(true);
            if (ext.IsJsExported == null)
            {
                ext.IsJsExported = IsJsExported_Internal(me).GetValueOrDefault();
                //if (ext.IsJsExported == null)
                //{
                //    var decType = me.GetDeclaringTypeDefinition();
                //    if(decType!=null)
                //        ext.IsJsExported = IsJsExported(decType);
                //}
            }
            return ext.IsJsExported.Value;
        }

        private static bool? IsJsExported_Internal(IEntity me)
        {
            if (me is ITypeDefinition)
            {
                var ce = (ITypeDefinition)me;
                return ce.MD_JType(t => t._Export).GetValueOrDefault(true);
            }
            if (me.SymbolKind == SymbolKind.Method || me.SymbolKind == SymbolKind.Accessor)
            {
                var me2 = (IMethod)me;
                return me2.MD_JMethodOrJType(t => t._Export, t => t._Export).GetValueOrDefault(true);
            }
            if (me.SymbolKind == SymbolKind.Property)
            {
                var pe = (IProperty)me;
                return pe.MD_JPropertyOrJType(t => t._Export, t => t._Export).GetValueOrDefault(true);
            }
            if (me.SymbolKind == SymbolKind.Field)//danel: || const
            {
                var pe = (IField)me;
                return pe.MD_JFieldOrJType(t => t._Export, t => t._Export).GetValueOrDefault(true);
            }
            //other entity types
            var decType = me.GetDeclaringTypeDefinition();
            if (decType != null)
                return IsJsExported(decType);
            return null;
        }


        #endregion

        #region JsType
        public static bool UseNativeJsons(ITypeDefinition type)
        {
            var att = type.GetJsTypeAttribute();
            if (att != null && att.NativeJsons)
                return true;
            return false;
        }

        public static JTypeAttribute GetJsTypeAttribute(this ITypeDefinition ce)
        {
            if (ce == null)
                return null;
            var att = ce.GetMetadata<JTypeAttribute>();
            if (att == null && ce.ParentAssembly != null)
                att = GetDefaultJsTypeAttribute(ce);
            return att;
        }

        private static JTypeAttribute GetDefaultJsTypeAttribute(ITypeDefinition ce)
        {
            if (ce == null) 
                return null;
            return ce.ParentAssembly.GetMetadatas<JTypeAttribute>().Where(t => t.TargetType == null).FirstOrDefault();
        }
        public static bool UseNativeOperatorOverloads(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._NativeOperatorOverloads).GetValueOrDefault();
        }
        public static bool UseNativeOverloads(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._NativeOverloads).GetValueOrDefault();
        }
        public static bool IgnoreTypeArguments(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._IgnoreGenericTypeArguments).GetValueOrDefault();
        }
        public static bool IsGlobalType(ITypeDefinition ce)
        {
            if (ce == null)
                return false;
            var ext = ce.GetExtension(true);
            if (ext.IsGlobalType == null)
                ext.IsGlobalType = IsGlobalType_Internal(ce);
            return ext.IsGlobalType.Value;
        }

        public static bool IsGlobalType_Internal(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._GlobalObject).GetValueOrDefault();
        }

        public static bool IsClrType(ITypeDefinition ce)
        {
            if (ce == null)
                return false;
            return !IsNativeType(ce) && !IsGlobalType(ce);
        }

        public static bool IsNativeType(ITypeDefinition ce)
        {
            if (ce == null)
                return false;
            var ext = ce.GetExtension(true);
            if (ext.IsNativeType == null)
                ext.IsNativeType = IsNativeType_Internal(ce);
            return ext.IsNativeType.Value;
        }
        public static bool IsExtJsType(ITypeDefinition ce)
        {
            var mode = ce.MD_JType(t => t._Mode);
            return mode != null && mode.Value == JsMode.ExtJs;
        }

        public static bool IsNativeType_Internal(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._Native).GetValueOrDefault();
        }

        public static bool OmitDefaultConstructor(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._OmitDefaultConstructor).GetValueOrDefault();
        }

        
        #endregion

        #region JsDelegate

        public static JsDelegateAttribute GetJsDelegateAttribute(ITypeDefinition et)
        {
            if (et == null || !et.IsDelegate())
                return null;

            var data = et.GetMetadata<JsDelegateAttribute>();
            return data;
        }

        #endregion

        #region Entity

        public static bool IsGlobalMember(IEntity me)
        {
            if (me is IMethod)
                return IsGlobalMethod((IMethod)me);
            if (me is ITypeDefinition)
                return IsGlobalType((ITypeDefinition)me);
            if (me is IProperty)
                return IsGlobalProperty((IProperty)me);
            return IsGlobalType(me.GetDeclaringTypeDefinition());

        }

        private static bool IsGlobalProperty(IProperty me)
        {
            var att = me.GetMetadata<JPropertyAttribute>(true);
            if (att != null && att._Global != null)
                return att._Global.Value;
            return IsGlobalType(me.GetDeclaringTypeDefinition());
        }

        #endregion

        public static ITypeDefinition GetBaseJClrType(ITypeDefinition ce)
        {
            var baseCe = ce.GetBaseTypeDefinition();
            while (baseCe != null && !IsClrType(baseCe))
                baseCe = baseCe.GetBaseTypeDefinition();
            return baseCe;
        }

        public static bool IsJsonMode(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._Mode) == JsMode.Json;
        }

        //public static bool ForceDelegatesAsNativeFunctions(IEntity me)
        //{
        //    if(me is IMethod)
        //        return ForceDelegatesAsNativeFunctions((IMethod)me);
        //    else if (me is ITypeDefinition)
        //        return ForceDelegatesAsNativeFunctions(((ITypeDefinition)me));
        //    else if (me is IType)
        //        return ForceDelegatesAsNativeFunctions(((IType)me).GetDefinitionOrArrayType());
        //    return ForceDelegatesAsNativeFunctions(me.DeclaringTypeDefinition);
        //}
        public static bool ForceDelegatesAsNativeFunctions(IMethod me)
        {
            return me.MD_JMethodOrJType(t => t._ForceDelegatesAsNativeFunctions, t => t._ForceDelegatesAsNativeFunctions).GetValueOrDefault();
        }
        public static bool ForceDelegatesAsNativeFunctions(IMember me)
        {
            if (me is IMethod)
                return ForceDelegatesAsNativeFunctions((IMethod)me);
            ITypeDefinition ce;
            if (me is ITypeDefinition)
                ce = (ITypeDefinition)me;
            else
                ce = me.DeclaringTypeDefinition;

            return ce.MD_JType(t => t._ForceDelegatesAsNativeFunctions).GetValueOrDefault();
        }
        //public static bool ForceDelegatesAsNativeFunctions(ITypeDefinition ce)
        //{
        //    return ce.MD_JsType(t => t._ForceDelegatesAsNativeFunctions).GetValueOrDefault();
        //}

        public static bool InlineFields(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._InlineFields).GetValueOrDefault();
        }
        public static bool OmitInheritance(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._OmitInheritance).GetValueOrDefault();
        }

        public static bool OmitCasts(ITypeDefinition ce, NProject2 project)
        {
            var att = GetJsExportAttribute(project.Compilation.MainAssembly);
            if (att != null && att.ForceOmitCasts)
                return true;
            var value = ce.MD_JType(t => t._OmitCasts);
            return value.GetValueOrDefault();
        }

        public static bool OmitOptionalParameters(IMethod me)
        {
            return me.MD_JMethodOrJType(t => t._OmitOptionalParameters, t => t._OmitOptionalParameters).GetValueOrDefault();
        }

        //public static bool IsStructAsClass(IStruct ce)
        //{
        //    var att = GetJsStructAttribute(ce);
        //    if (att != null)
        //        return att.IsClass;
        //    return false;
        //}

        //private static JsStructAttribute GetJsStructAttribute(IStruct ce)
        //{
        //    return ce.GetMetadata<JsStructAttribute>();
        //}

        #region Utils

        static R MD<T, R>(this IEntity me, Func<T, R> selector) where T : System.Attribute
        {
            var att = me.GetMetadata<T>(true);
            if (att != null)
                return selector(att);
            return default(R);
        }
        static R MD_JMethod<R>(this IMethod me, Func<JMethodAttribute, R> func)
        {
            return me.MD(func);
        }
        static R MD_JProperty<R>(this IProperty me, Func<JPropertyAttribute, R> func)
        {
            return me.MD(func);
        }
        static R MD_JField<R>(this IField me, Func<JFieldAttribute, R> func)
        {
            return me.MD(func);
        }
        static R MD_JType<R>(this ITypeDefinition ce, Func<JTypeAttribute, R> func2)
        {
            var att = ce.GetMetadata<JTypeAttribute>();
            if (att != null)
            {
                var x = func2(att);
                if (((object)x) != null)
                    return x;
            }
            att = GetDefaultJsTypeAttribute(ce);
            if (att != null)
                return func2(att);
            return default(R);
        }
        static R MD_JMethodOrJType<R>(this IMethod me, Func<JMethodAttribute, R> func, Func<JTypeAttribute, R> func2)
        {
            var x = me.MD_JMethod(func);
            if (((object)x) != null)
                return x;
            var ce = me.GetDeclaringTypeDefinition();
            if (ce != null)
                x = ce.MD_JType(func2);
            return x;
        }
        static R MD_JPropertyOrJType<R>(this IProperty me, Func<JPropertyAttribute, R> func, Func<JTypeAttribute, R> func2)
        {
            var x = me.MD_JProperty(func);
            if (((object)x) != null)
                return x;
            var ce = me.GetDeclaringTypeDefinition();
            if (ce != null)
                x = ce.MD_JType(func2);
            return x;
        }
        static R MD_JFieldOrJType<R>(this IField me, Func<JFieldAttribute, R> func, Func<JTypeAttribute, R> func2)
        {
            var x = me.MD_JField(func);
            if (((object)x) != null)
                return x;
            var ce = me.GetDeclaringTypeDefinition();
            if (ce != null)
                x = ce.MD_JType(func2);
            return x;
        }
        #endregion

        public static bool IsNativeParams(IMethod me)
        {
            var x = me.MD_JMethodOrJType(t => t._NativeParams, t => t._NativeParams);
            if (x == null)
                return true;
            return x.Value;
        }

        public static string GetPrototypeName(ITypeDefinition ce)
        {
            var att = GetJsTypeAttribute(ce);
            if (att != null && att.PrototypeName != null)
                return att.PrototypeName;
            return "prototype";
        }

        public static bool IsNativeError(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._NativeError).GetValueOrDefault();
        }

        public static bool NativeCasts(ITypeDefinition ce)
        {
            return ce.MD_JType(t => t._NativeCasts).GetValueOrDefault();
        }

        public static string GetGenericArugmentJsCode(ITypeDefinition ce)
        {
            return MD_JType(ce, t => t._GenericArgumentJsCode);
        }


        /// <summary>
        /// Gets a member or a class, identifies if it's an enum member or type, 
        /// and returns whether this enum has no JsType attribute or has JsType(JsMode.Json)
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static bool UseJsonEnums(IEntity me, out bool valuesAsNames)
        {
            if (me.IsEnumMember() || me.IsEnum())
            {
                var ce = me.IsEnum() ? (ITypeDefinition)me : me.GetDeclaringTypeDefinition();
                var use = true;
                var att = ce.GetJsTypeAttribute();
                if (att != null)
                    use = att.Mode == JsMode.Json;
                valuesAsNames = false;
                var att2 = ce.GetMetadata<JEnumAttribute>();
                if (att2 != null && att2._ValuesAsNames != null)
                {
                    valuesAsNames = att2 != null && att2.ValuesAsNames;
                }
                else if (ce.ParentAssembly != null)
                {
                    var att3 = ce.ParentAssembly.GetMetadata<JEnumAttribute>();
                    if (att3 != null)
                        valuesAsNames = att3.ValuesAsNames;
                }
                return use;
            }
            valuesAsNames = false;
            return false;
        }
    }

}
