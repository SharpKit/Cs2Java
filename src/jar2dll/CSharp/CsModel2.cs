using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace JSharp.CSharp
{
    [DebuggerStepThrough]
    partial class Element
    {
        public object Tag {get;set;}
        public string Name {get;set;}
        public virtual IEnumerable<Element> Children()
        {
            yield break;
        }
        public virtual R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitElement(this); }
        public virtual void AcceptVisitor(IElementVisitor visitor) { visitor.VisitElement(this); }
    }
    [DebuggerStepThrough]
    partial class Class : Member
    {
        public string Namespace {get;set;}
        public List<Class> GenericArguments {get;set;}
        public Class BaseClass {get;set;}
        public List<Class> Interfaces {get;set;}
        public List<Member> Members {get;set;}
        public bool IsInterface {get;set;}
        public bool IsDelegate {get;set;}
        public bool IsAbstract {get;set;}
        public bool IsSealed {get;set;}
        public Class()
        {
            Init();
            GenericArguments =  new List<Class>();
            Interfaces =  new List<Class>();
            Members =  new List<Member>();
        }
        partial void Init();
        public override IEnumerable<Element> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(GenericArguments!=null) foreach(var x in GenericArguments) yield return x;
            if(BaseClass!=null) yield return BaseClass;
            if(Interfaces!=null) foreach(var x in Interfaces) yield return x;
            if(Members!=null) foreach(var x in Members) yield return x;
        }
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitClass(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitClass(this); }
    }
    [DebuggerStepThrough]
    partial class Method : Member
    {
        public List<Parameter> Parameters {get;set;}
        public List<Class> GenericArguments {get;set;}
        public bool IsConstructor {get;set;}
        public bool IsAbstract {get;set;}
        public Method ExplicitImplementationOfInterfaceMethod {get;set;}
        public Method()
        {
            Init();
            Parameters =  new List<Parameter>();
            GenericArguments =  new List<Class>();
        }
        partial void Init();
        public override IEnumerable<Element> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Parameters!=null) foreach(var x in Parameters) yield return x;
            if(GenericArguments!=null) foreach(var x in GenericArguments) yield return x;
            if(ExplicitImplementationOfInterfaceMethod!=null) yield return ExplicitImplementationOfInterfaceMethod;
        }
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitMethod(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitMethod(this); }
    }
    [DebuggerStepThrough]
    partial class Parameter : TypedElement
    {
        public bool IsOptional {get;set;}
        public Method DeclaringMethod {get;set;}
        public override IEnumerable<Element> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(DeclaringMethod!=null) yield return DeclaringMethod;
        }
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitParameter(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitParameter(this); }
    }
    [DebuggerStepThrough]
    partial class Field : Member
    {
        public string Initializer {get;set;}
        public bool IsReadOnly {get;set;}
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitField(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitField(this); }
    }
    [DebuggerStepThrough]
    partial class Property : Member
    {
        public bool IsReadOnly {get;set;}
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitProperty(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitProperty(this); }
    }
    [DebuggerStepThrough]
    partial class Event : Member
    {
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitEvent(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitEvent(this); }
    }
    [DebuggerStepThrough]
    partial class TypedElement : Element
    {
        public Class TypeRef {get;set;}
        public override IEnumerable<Element> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(TypeRef!=null) yield return TypeRef;
        }
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitTypedElement(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitTypedElement(this); }
    }
    [DebuggerStepThrough]
    partial class Member : TypedElement
    {
        public bool IsStatic {get;set;}
        public Class DeclaringClass {get;set;}
        public List<Attribute> Attributes {get;set;}
        public string Summary {get;set;}
        public string Remarks {get;set;}
        public bool IsVirtual {get;set;}
        public bool IsOverride {get;set;}
        public bool IsNew {get;set;}
        public bool IsProtected {get;set;}
        public bool IsPrivate {get;set;}
        public bool IsInternal {get;set;}
        public Member()
        {
            Init();
            Attributes =  new List<Attribute>();
        }
        partial void Init();
        public override IEnumerable<Element> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(DeclaringClass!=null) yield return DeclaringClass;
        }
        public override R AcceptVisitor<R>(IElementVisitor<R> visitor) { return visitor.VisitMember(this); }
        public override void AcceptVisitor(IElementVisitor visitor) { visitor.VisitMember(this); }
    }
    interface IElementVisitor<out R>
    {
        R VisitElement(Element node);
        R VisitClass(Class node);
        R VisitMethod(Method node);
        R VisitParameter(Parameter node);
        R VisitField(Field node);
        R VisitProperty(Property node);
        R VisitEvent(Event node);
        R VisitTypedElement(TypedElement node);
        R VisitMember(Member node);
    }
    interface IElementVisitor
    {
        void VisitElement(Element node);
        void VisitClass(Class node);
        void VisitMethod(Method node);
        void VisitParameter(Parameter node);
        void VisitField(Field node);
        void VisitProperty(Property node);
        void VisitEvent(Event node);
        void VisitTypedElement(TypedElement node);
        void VisitMember(Member node);
    }
}
