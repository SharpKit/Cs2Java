using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.CSharp
{
    class GenericMaker : IElementVisitor<Element>
    {
        public AssemblyContext Context { get; set; }
        public void PreMake()
        {
            if (MadeClass == null)
                MadeClass = new Class();
            var ce = RootClass;
            var ce2 = MadeClass;
            ce2.GenericClassDefinition = RootClass;
            ce2.Namespace = ce.Namespace;
            ce2.Assembly = ce.Assembly;
            ce2.GenericArguments = RootGenericArgs.ToList();//VisitEach();
            ce2.IsInterface = ce.IsInterface;
            ce2.IsDelegate = ce.IsDelegate;
            VisitMember(ce, ce2);
        }
        public void Make()
        {
            var ce = RootClass;
            var ce2 = MadeClass;
            ce2.BaseClass = Visit(ce.BaseClass);
            ce2.Interfaces = VisitEach(ce.Interfaces);
            ce2.Members = VisitEach(ce.MembersExceptClasses().ToList()); //TODO:
            ce2.Members.ForEach(t => t.DeclaringClass = ce2);
        }
        [DebuggerStepThrough]
        T Visit<T>(T me) where T : Element
        {
            if (me == null)
                return null;
            return (T)me.AcceptVisitor(this);
        }
        [DebuggerStepThrough]
        List<T> VisitEach<T>(List<T> list) where T : Element
        {
            return list.Select(Visit).ToList();
        }
        public Class MadeClass { get; set; }
        public Class RootClass { get; set; }
        public List<Class> RootGenericArgs { get; set; }

        //public Dictionary<Class, Class> ReplaceArgs { get; set; }
        #region IElementVisitor<Element> Members

        public Element VisitClass(Class ce)
        {
            if (ce.IsGenericTypeArgument)
            {
                if (RootGenericArgs.Contains(ce))
                    return ce;
                var index = RootClass.GenericArguments.IndexOf(t => t == ce);
                if (index == -1)
                {
                    Console.WriteLine("Can't find generic arg: " + ce.Name);
                    return null;
                }
                return RootGenericArgs[index];
            }
            if (ce.GenericArguments.Count > 0)
            {
                var args = VisitEach(ce.GenericArguments);
                if (args.SequenceEqual(ce.GenericArguments))
                    return ce;
                var ce3 = ce;
                if (ce.GenericClassDefinition != null)
                    ce3 = ce.GenericClassDefinition;
                var ce2 = Context.MakeGenericClass(ce3, args);
                return ce2;
            }
            return ce;
        }

        private void VisitMember(Member el, Member el2)
        {
            el2.IsStatic = el.IsStatic;
            el2.Attributes = el.Attributes;
            el2.Summary = el.Summary;
            el2.Remarks = el.Remarks;
            el2.IsVirtual = el.IsVirtual;
            el2.IsNew = el.IsNew;
            el2.IsProtected = el.IsProtected;
            el2.IsPrivate = el.IsPrivate;
            el2.IsInternal = el.IsInternal;
            VisitTypedElement(el, el2);
        }
        private void VisitTypedElement(TypedElement el, TypedElement el2)
        {
            el2.Type = Visit(el.Type);
            VisitElement(el, el2);
        }
        private void VisitElement(Element el, Element el2)
        {
            el2.Name = el.Name;
        }

        public Element VisitMethod(Method me)
        {
            var me2 = new Method
            {
                Parameters = VisitEach(me.Parameters),
                GenericArguments = VisitEach(me.GenericArguments),
            };
            me2.Parameters.ForEach(t => t.DeclaringMethod = me2);

            VisitMember(me, me2);
            return me2;
        }

        public Element VisitParameter(Parameter me)
        {
            var me2 = new Parameter
            {
                Name = me.Name,
            };
            VisitTypedElement(me, me2);
            return me2;
        }

        public Element VisitField(Field me)
        {
            var me2 = new Field
            {
                Name = me.Name,
            };
            VisitMember(me, me2);
            return me2;
        }

        public Element VisitProperty(Property me)
        {
            var me2 = new Property
            {
                Name = me.Name,
            };
            VisitMember(me, me2);
            return me2;
        }

        public Element VisitEvent(Event node)
        {
            throw new NotImplementedException();
        }

        public Element VisitElement(Element node)
        {
            throw new NotImplementedException();
        }

        public Element VisitTypedElement(TypedElement node)
        {
            throw new NotImplementedException();
        }

        public Element VisitMember(Member node)
        {
            throw new NotImplementedException();
        }


        #endregion


        //ClassRef Visit(ClassRef cr)
        //{
        //    if (cr == null)
        //        return null;
        //    //if (cr.IsTypeVariable && !cr.IsTypeVariableOwnedByMethod)
        //    //{
        //    //    var x = RootClass.GenericArgumentRefs.IndexOf(t => t.Name == cr.Name);
        //    //    if (x == -1)
        //    //    {
        //    //        Console.WriteLine("Can't find generic arg: " + cr.Name);
        //    //        return null;
        //    //    }
        //    //    return RootGenericArgs[x];
        //    //}
        //    return new ClassRef
        //    {
        //        ArrayItemType = Visit(cr.ArrayItemType),
        //        GenericArguments = VisitEach(cr.GenericArguments),
        //        ResolvedClass = Visit(cr.ResolvedClass),
        //        UpperBounds = VisitEach(cr.UpperBounds),
        //        LowerBounds = VisitEach(cr.LowerBounds),
        //        Name = cr.Name,
        //        IsWildcardType = cr.IsWildcardType,
        //        IsArray = cr.IsArray,
        //        IsTypeVariable = cr.IsTypeVariable,
        //        IsTypeVariableOwnedByMethod = cr.IsTypeVariableOwnedByMethod,
        //    };
        //}
        //List<ClassRef> VisitEach(List<ClassRef> list)
        //{
        //    return list.Select(Visit).ToList();
        //}

    }
}
