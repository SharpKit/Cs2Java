using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace JSharp.CSharp
{
    partial class TypeName
    {

        public TypeName(string ns, string name)
        {
            this.Namespace = ns;
            this.Name = name;
        }
        public TypeName(string fullName)
        {
            FullName = fullName;
        }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public TypeName DeclaringTypeName { get; set; }

        public string FullName
        {
            get
            {
                if (DeclaringTypeName != null)
                    return DeclaringTypeName.FullName + "+" + Name;
                if (Namespace.IsNotNullOrEmpty())
                    return Namespace + "." + Name;
                return Name;
            }
            set
            {
                if (value.Contains("+"))
                {
                    var tokens2 = value.Split('+');
                    Name = tokens2[1];
                    DeclaringTypeName = new TypeName(tokens2[0]);
                    return;
                }
                var tokens = value.Split('.');
                Name = tokens.Skip(tokens.Length - 1).First();
                Namespace = tokens.Take(tokens.Length - 1).StringConcat(".");
            }
        }

    }
    partial class Class : Member
    {
        public string FullName
        {
            get
            {
                if (DeclaringClass != null)
                {
                    return DeclaringClass.FullName + "+" + Name;
                }
                return new TypeName(Namespace, Name).FullName;
            }
            set
            {
                var tn = new TypeName(value);
                Name = tn.Name;
                Namespace = tn.Namespace;
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(FullName);
            if (GenericArguments.IsNotNullOrEmpty())
            {
                sb.Append("<");
                GenericArguments.ForEachJoin(t => sb.Append(t), () => sb.Append(","));
                sb.Append(">");
            }
            return sb.ToString();
        }
        public override void Remove()
        {
            if (DeclaringClass == null)
            {
                Assembly.Classes.Remove(this);
                return;
            }
            base.Remove();
        }



        public Class GenericClassDefinition { get; set; }

        public bool IsArray { get; set; }

        public Class ArrayElementType { get; set; }

        public bool IsGenericTypeArgument { get; set; }
        public bool IsGenericMethodArgument { get; set; }

        public Method GenericMethodDefinition { get; set; }
    }
    partial class Method : Member
    {

        public override string UniqueName
        {
            get
            {
                var name = base.UniqueName;
                var prms = Parameters.Select(t => t.Type.Name/*.UniqueName*/).StringConcat(",");
                return name + "(" + prms + ")";
            }
        }

    }
    partial class Property : Member
    {
        public Property ExplicitImplementationOfInterfaceProperty { get; set; }
    }
    partial class Event : Member
    {
    }
    partial class Element
    {
        public override string ToString()
        {
            return Name;
        }
    }
    partial class TypedElement : Element
    {
        public Class Type { get; set; }
    }

    partial class Member : TypedElement
    {

        public virtual string UniqueName
        {
            get
            {
                var name = Name;
                if (DeclaringClass != null)
                    name = DeclaringClass.UniqueName + "." + name;
                return name;
            }
        }
        public bool IsPublic
        {
            get
            {
                return !IsPrivate && !IsInternal && !IsProtected;
            }
            set
            {
                IsPrivate = false;
                IsInternal = false;
                IsProtected = false;
            }
        }

        public virtual void Remove()
        {
            if (DeclaringClass == null)
                return;
            DeclaringClass.Members.Remove(this);
            DeclaringClass = null;
        }

        Assembly _Assembly;
        public Assembly Assembly
        {
            get
            {
                if (_Assembly == null && DeclaringClass != null)
                {
                    _Assembly = DeclaringClass.Assembly;
                }
                return _Assembly;
            }
            set
            {
                _Assembly = value;
            }
        }

    }


    partial class Assembly
    {
        public Assembly()
        {
            Classes = new List<Class>();
            Usings = new List<string>();
            Attributes = new List<Attribute>();
        }
        public List<Attribute> Attributes { get; set; }

        public List<Class> Classes { get; set; }
        public IEnumerable<Class> ClassesAndNested
        {
            get
            {
                foreach (var ce in Classes)
                {
                    yield return ce;
                    foreach (var ce2 in ce.Members.OfType<Class>())
                    {
                        yield return ce2;
                        foreach (var ce3 in ce2.Members.OfType<Class>())
                        {
                            yield return ce3;
                        }
                    }
                }
            }
        }
        public Class GetClass(string fullName)
        {
            return ClassesAndNested.Where(t => t.FullName == fullName).FirstOrDefault();
        }
        public List<string> Usings { get; set; }
    }


    partial class Attribute
    {
        public Attribute()
        {
            Parameters = new List<string>();
            NamedParamters = new Dictionary<string, string>();
        }
        public Attribute Clone()
        {
            return new Attribute { Name = Name, Parameters = new List<string>(Parameters), NamedParamters = new Dictionary<string, string>(NamedParamters) };
        }
        public string Name { get; set; }
        public List<string> Parameters { get; set; }
        public Dictionary<string, string> NamedParamters { get; set; }
    }

}
