using System.Runtime.Serialization;
using java.lang;
using java.util;

namespace JSharpKit
{
    static class JJarExtensions
    {
        public static System.Collections.Generic.IEnumerable<JClassRef> Children(this JClassRef ce)
        {
            if (ce.GenericArguments != null)
            {
                foreach (var ce2 in ce.GenericArguments)
                    yield return ce2;
            }
            if (ce.ArrayItemType != null)
                yield return ce.ArrayItemType;
        }
        public static System.Collections.Generic.IEnumerable<JClassRef> Descendants(this JClassRef ce)
        {
            foreach (var ce2 in ce.Children())
            {
                yield return ce2;
                foreach (var ce3 in ce2.Descendants())
                    yield return ce3;
            }
        }
        public static System.Collections.Generic.IEnumerable<JClassRef> DescendantsAndSelf(this JClassRef ce)
        {
            yield return ce;
            foreach (var ce2 in ce.Descendants())
                yield return ce2;
        }
    }
    [DataContract]
    public class JJar
    {
        public JJar()
        {
            Classes = new ArrayList<JClass>();
        }
        [DataMember(IsRequired = false)]
        public List<JClass> Classes { get; set; }
    }

    [DataContract]
    public class JClassRef
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Name { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsParameterizedType { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> GenericArguments { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsArray { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public JClassRef ArrayItemType { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsWildcardType { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> LowerBounds { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> UpperBounds { get; set; }
        //public override string ToString()
        //{
        //    return String.Format("{0}", Name);
        //}

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsTypeVariable { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsTypeVariableOwnedByMethod { get; set; }

    }
    [DataContract]
    public class JElement
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }

    }
    [DataContract]
    public class JClass : JMember
    {
        public JClass()
        {
            Fields = new ArrayList<JField>();
            Methods = new ArrayList<JMethod>();
            Ctors = new ArrayList<JConstructor>();
            Classes = new ArrayList<JClass>();
            Interfaces = new ArrayList<JClassRef>();
            GenericArguments = new ArrayList<JClassRef>();

        }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public JClassRef BaseClass { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> Interfaces { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JField> Fields { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JMethod> Methods { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JConstructor> Ctors { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClass> Classes { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> GenericArguments { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsInterface { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClass { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnum { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public JClassRef ArrayItemType { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsArray { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAnonymousClass { get; set; }
    }
    [DataContract]
    public class JTypedElement : JElement
    {

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public JClassRef Type { get; set; }
        public override string ToString()
        {
            return Name;// String.Format("{0} {1}", Type, Name);
        }

    }

    [DataContract]
    public class JMember : JTypedElement
    {
        public override string ToString()
        {
            return Name;// String.Format("{0} {1}", Type, Name);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsPublic { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsPrivate { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsProtected { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStatic { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFinal { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsSynchronized { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsVolatile { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsTransient { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAbstract { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public JClass DeclaringClass { get; set; }
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsSynthetic { get; set; }

    }

    [DataContract]
    public class JField : JMember
    {

    }

    [DataContract]
    public class JMethod : JMember
    {
        public JMethod()
        {
            Parameters = new ArrayList<JParameter>();
            TypeParameters = new ArrayList<JClassRef>();
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JParameter> Parameters { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<JClassRef> TypeParameters { get; set; }
    }

    [DataContract]
    public class JConstructor : JMethod
    {
    }

    [DataContract]
    public class JParameter : JTypedElement
    {

    }


}
