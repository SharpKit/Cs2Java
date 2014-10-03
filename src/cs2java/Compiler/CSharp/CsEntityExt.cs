using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections;
using System.Collections.Concurrent;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    class AssemblyExt
    {
        public AssemblyExt(IAssembly me)
        {
            Assembly = me;
        }

        public IAssembly Assembly { get; private set; }

        List<ResolvedAttribute> _ResolvedAttributes;
        public List<ResolvedAttribute> ResolvedAttributes
        {
            get
            {
                if (_ResolvedAttributes == null)
                    _ResolvedAttributes = Assembly.AssemblyAttributes.Select(ToResolvedAttribute).ToList();
                return _ResolvedAttributes;
            }
        }
        ResolvedAttribute ToResolvedAttribute(IAttribute att)
        {
            return new ResolvedAttribute { IAttribute = att };
        }

    }

    class EntityExt
    {
        public EntityExt(IEntity me)
        {
            Entity = me;
        }

        public IEntity Entity { get; private set; }
        public Dictionary<Type, object> SingleDeclaredAttributeCache { get; set; }
        List<ResolvedAttribute> _ResolvedAttributes;
        public List<ResolvedAttribute> ResolvedAttributes
        {
            get
            {
                if (_ResolvedAttributes == null)
                    _ResolvedAttributes = Entity.Attributes.Select(ToResolvedAttribute).ToList();
                return _ResolvedAttributes;
            }
        }
        ResolvedAttribute ToResolvedAttribute(IAttribute att)
        {
            return new ResolvedAttribute { IAttribute = att };
        }

        List<ResolvedAttribute> _ExternalResolvedAttributes;
        public List<ResolvedAttribute> ExternalResolvedAttributes
        {
            get
            {
                if (_ExternalResolvedAttributes == null)
                    _ExternalResolvedAttributes = new List<ResolvedAttribute>();
                return _ExternalResolvedAttributes;
            }
        }

        List<ResolvedAttribute> _ParentExternalResolvedAttributes;
        public List<ResolvedAttribute> ParentExternalResolvedAttributes
        {
            get
            {
                if (_ParentExternalResolvedAttributes == null)
                {
                    _ParentExternalResolvedAttributes = new List<ResolvedAttribute>();
                    var me2 = Entity as IMember;
                    if (me2 != null && me2.MemberDefinition != me2 && me2.MemberDefinition != null)
                    {
                        var me3 = me2.MemberDefinition;
                        var ext3 = me3.GetExtension(false);
                        if (ext3 != null)
                        {
                            if (ext3.ExternalResolvedAttributes != null)
                                _ParentExternalResolvedAttributes.AddRange(ext3.ExternalResolvedAttributes);
                        }
                    }
                }
                return _ParentExternalResolvedAttributes;
            }
        }

        public IEnumerable<ResolvedAttribute> AllResolvedAttributes
        {
            get
            {
                return ExternalResolvedAttributes.Concat(ParentExternalResolvedAttributes).Concat(ResolvedAttributes);
            }
        }

        public bool? IsJsExported { get; set; }
        public bool? IsRemotable { get; set; }
        public bool? IsGlobalType { get; set; }
        public bool? IsNativeType { get; set; }
    }

    class ResolvedAttribute
    {
        public IAttribute IAttribute { get; set; }
        public Attribute Attribute { get; set; }

        public ICSharpCode.NRefactory.CSharp.AstNode GetDeclaration()
        {
            if (IAttribute == null)
                return null;
            return IAttribute.GetDeclaration();
        }
        public bool MatchesType<T>()
        {
            if (Attribute != null && Attribute is T)
                return true;
            if (IAttribute != null && IAttribute.AttributeType.FullName == typeof(T).FullName)
                return true;
            return false;
        }
        public T ConvertToCustomAttribute<T>(NProject2 project=null) where T : Attribute
        {
            if (Attribute == null && IAttribute != null)
                Attribute = IAttribute.ConvertToCustomAttribute<T>(project);
            return Attribute as T;
        }
    }

    static class EntityExtProvider
    {
        public static IEnumerable<ResolvedAttribute> GetAllResolvedAttributes(this IEntity ent)
        {
            return ent.GetExtension(true).AllResolvedAttributes;
        }

        public static AssemblyExt GetExtension(this IAssembly ent, bool create)
        {
            var ext = (AssemblyExt)ent.Tag;
            if (ext == null && create)
            {
                ext = new AssemblyExt(ent);
                ent.Tag = ext;
            }
            return ext;
        }

        public static EntityExt GetExtension(this IEntity ent, bool create)
        {
            var ext = (EntityExt)ent.Tag;
            if (ext == null && create)
            {
                ext = new EntityExt(ent);
                ent.Tag = ext;
            }
            return ext;
        }
    }


}
