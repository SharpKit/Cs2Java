using SystemUtils.Collections;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace JSharp.Java.Ast
{
    [DebuggerStepThrough]
    partial class JNode
    {
        public JNode Parent {get;set;}
        public virtual IEnumerable<JNode> Children()
        {
            yield break;
        }
        public virtual R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNode(this); }
        public virtual void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNode(this); }
    }
    [DebuggerStepThrough]
    partial class JCompilationUnit : JUnit
    {
        IList<JImport> _Imports;
        public IList<JImport> Imports { get { return _Imports; } set { _Imports.SetItems(value);} }
        IList<JClassDeclaration> _Declarations;
        public IList<JClassDeclaration> Declarations { get { return _Declarations; } set { _Declarations.SetItems(value);} }
        public string PackageName {get;set;}
        public JCompilationUnit()
        {
            Init();
            _Imports =  new CompositionList<JImport>(t=>t.Parent=this, t=>t.Parent=null);
            _Declarations =  new CompositionList<JClassDeclaration>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Imports!=null) foreach(var x in Imports) yield return x;
            if(Declarations!=null) foreach(var x in Declarations) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitCompilationUnit(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitCompilationUnit(this); }
    }
    [DebuggerStepThrough]
    partial class JImport : JNode
    {
        JMemberExpression _Type;
        public JMemberExpression Type { get { return _Type; } set { if(_Type!=null) _Type.Parent=null; _Type= value;if(_Type!=null) _Type.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Type!=null) yield return Type;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitImport(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitImport(this); }
    }
    [DebuggerStepThrough]
    partial class JEntityDeclaration : JNode
    {
        public string Name {get;set;}
        JMemberExpression _Type;
        public JMemberExpression Type { get { return _Type; } set { if(_Type!=null) _Type.Parent=null; _Type= value;if(_Type!=null) _Type.Parent=this;} }
        IList<JAnnotationDeclaration> _Annotations;
        public IList<JAnnotationDeclaration> Annotations { get { return _Annotations; } set { _Annotations.SetItems(value);} }
        public JEntityDeclaration()
        {
            Init();
            _Annotations =  new CompositionList<JAnnotationDeclaration>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Type!=null) yield return Type;
            if(Annotations!=null) foreach(var x in Annotations) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitEntityDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitEntityDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JAnnotationDeclaration : JNode
    {
        public string Name {get;set;}
        IList<JExpression> _Parameters;
        public IList<JExpression> Parameters { get { return _Parameters; } set { _Parameters.SetItems(value);} }
        IList<JAnnotationNamedParameter> _NamedParameters;
        public IList<JAnnotationNamedParameter> NamedParameters { get { return _NamedParameters; } set { _NamedParameters.SetItems(value);} }
        public JAnnotationDeclaration()
        {
            Init();
            _Parameters =  new CompositionList<JExpression>(t=>t.Parent=this, t=>t.Parent=null);
            _NamedParameters =  new CompositionList<JAnnotationNamedParameter>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Parameters!=null) foreach(var x in Parameters) yield return x;
            if(NamedParameters!=null) foreach(var x in NamedParameters) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitAnnotationDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitAnnotationDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JAnnotationNamedParameter : JNode
    {
        public string Name {get;set;}
        JExpression _Value;
        public JExpression Value { get { return _Value; } set { if(_Value!=null) _Value.Parent=null; _Value= value;if(_Value!=null) _Value.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Value!=null) yield return Value;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitAnnotationNamedParameter(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitAnnotationNamedParameter(this); }
    }
    [DebuggerStepThrough]
    partial class JClassDeclaration : JEntityDeclaration
    {
        public ITypeDefinition TypeDefinition {get;set;}
        IList<JEntityDeclaration> _Declarations;
        public IList<JEntityDeclaration> Declarations { get { return _Declarations; } set { _Declarations.SetItems(value);} }
        JMemberExpression _BaseClass;
        public JMemberExpression BaseClass { get { return _BaseClass; } set { if(_BaseClass!=null) _BaseClass.Parent=null; _BaseClass= value;if(_BaseClass!=null) _BaseClass.Parent=this;} }
        IList<JMemberExpression> _Interfaces;
        public IList<JMemberExpression> Interfaces { get { return _Interfaces; } set { _Interfaces.SetItems(value);} }
        IList<JMemberExpression> _TypeParameters;
        public IList<JMemberExpression> TypeParameters { get { return _TypeParameters; } set { _TypeParameters.SetItems(value);} }
        IList<JMemberExpression> _Extends;
        public IList<JMemberExpression> Extends { get { return _Extends; } set { _Extends.SetItems(value);} }
        IList<JMemberExpression> _Implements;
        public IList<JMemberExpression> Implements { get { return _Implements; } set { _Implements.SetItems(value);} }
        public JClassDeclaration()
        {
            Init();
            _Declarations =  new CompositionList<JEntityDeclaration>(t=>t.Parent=this, t=>t.Parent=null);
            _Interfaces =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
            _TypeParameters =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
            _Extends =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
            _Implements =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Declarations!=null) foreach(var x in Declarations) yield return x;
            if(BaseClass!=null) yield return BaseClass;
            if(Interfaces!=null) foreach(var x in Interfaces) yield return x;
            if(TypeParameters!=null) foreach(var x in TypeParameters) yield return x;
            if(Extends!=null) foreach(var x in Extends) yield return x;
            if(Implements!=null) foreach(var x in Implements) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitClassDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitClassDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JMethodDeclaration : JEntityDeclaration
    {
        public IMethod MethodDefinition {get;set;}
        public string CustomHeaderCode {get;set;}
        JBlock _MethodBody;
        public JBlock MethodBody { get { return _MethodBody; } set { if(_MethodBody!=null) _MethodBody.Parent=null; _MethodBody= value;if(_MethodBody!=null) _MethodBody.Parent=this;} }
        IList<JMemberExpression> _TypeParameters;
        public IList<JMemberExpression> TypeParameters { get { return _TypeParameters; } set { _TypeParameters.SetItems(value);} }
        IList<JParameterDeclaration> _Parameters;
        public IList<JParameterDeclaration> Parameters { get { return _Parameters; } set { _Parameters.SetItems(value);} }
        public JMethodDeclaration()
        {
            Init();
            _TypeParameters =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
            _Parameters =  new CompositionList<JParameterDeclaration>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(MethodBody!=null) yield return MethodBody;
            if(TypeParameters!=null) foreach(var x in TypeParameters) yield return x;
            if(Parameters!=null) foreach(var x in Parameters) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitMethodDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitMethodDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JFieldDeclaration : JEntityDeclaration
    {
        public IField FieldDefinition {get;set;}
        JExpression _Initializer;
        public JExpression Initializer { get { return _Initializer; } set { if(_Initializer!=null) _Initializer.Parent=null; _Initializer= value;if(_Initializer!=null) _Initializer.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Initializer!=null) yield return Initializer;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitFieldDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitFieldDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JParameterDeclaration : JEntityDeclaration
    {
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitParameterDeclaration(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitParameterDeclaration(this); }
    }
    [DebuggerStepThrough]
    partial class JNewAnonymousClassExpression : JNewObjectExpression
    {
        IList<JEntityDeclaration> _Declarations;
        public IList<JEntityDeclaration> Declarations { get { return _Declarations; } set { _Declarations.SetItems(value);} }
        public JNewAnonymousClassExpression()
        {
            Init();
            _Declarations =  new CompositionList<JEntityDeclaration>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Declarations!=null) foreach(var x in Declarations) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNewAnonymousClassExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNewAnonymousClassExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JMultiStatementExpression : JExpression
    {
        IList<JStatement> _Statements;
        public IList<JStatement> Statements { get { return _Statements; } set { _Statements.SetItems(value);} }
        public JMultiStatementExpression()
        {
            Init();
            _Statements =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Statements!=null) foreach(var x in Statements) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitMultiStatementExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitMultiStatementExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JExpression : JNode
    {
        JMemberExpression _Type;
        public JMemberExpression Type { get { return _Type; } set { if(_Type!=null) _Type.Parent=null; _Type= value;if(_Type!=null) _Type.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Type!=null) yield return Type;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JStatement : JNode
    {
        public IList<string> Comments {get;set;}
        public JStatement()
        {
            Init();
            Comments =  new List<string>();
        }
        partial void Init();
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JSwitchStatement : JStatement
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        IList<JSwitchSection> _Sections;
        public IList<JSwitchSection> Sections { get { return _Sections; } set { _Sections.SetItems(value);} }
        public JSwitchStatement()
        {
            Init();
            _Sections =  new CompositionList<JSwitchSection>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
            if(Sections!=null) foreach(var x in Sections) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitSwitchStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitSwitchStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JSwitchSection : JNode
    {
        IList<JSwitchLabel> _Labels;
        public IList<JSwitchLabel> Labels { get { return _Labels; } set { _Labels.SetItems(value);} }
        IList<JStatement> _Statements;
        public IList<JStatement> Statements { get { return _Statements; } set { _Statements.SetItems(value);} }
        public JSwitchSection()
        {
            Init();
            _Labels =  new CompositionList<JSwitchLabel>(t=>t.Parent=this, t=>t.Parent=null);
            _Statements =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Labels!=null) foreach(var x in Labels) yield return x;
            if(Statements!=null) foreach(var x in Statements) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitSwitchSection(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitSwitchSection(this); }
    }
    [DebuggerStepThrough]
    partial class JSwitchLabel : JNode
    {
        public bool IsDefault {get;set;}
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitSwitchLabel(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitSwitchLabel(this); }
    }
    [DebuggerStepThrough]
    partial class JWhileStatement : JStatement
    {
        JExpression _Condition;
        public JExpression Condition { get { return _Condition; } set { if(_Condition!=null) _Condition.Parent=null; _Condition= value;if(_Condition!=null) _Condition.Parent=this;} }
        JStatement _Statement;
        public JStatement Statement { get { return _Statement; } set { if(_Statement!=null) _Statement.Parent=null; _Statement= value;if(_Statement!=null) _Statement.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Condition!=null) yield return Condition;
            if(Statement!=null) yield return Statement;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitWhileStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitWhileStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JDoWhileStatement : JStatement
    {
        JExpression _Condition;
        public JExpression Condition { get { return _Condition; } set { if(_Condition!=null) _Condition.Parent=null; _Condition= value;if(_Condition!=null) _Condition.Parent=this;} }
        JStatement _Statement;
        public JStatement Statement { get { return _Statement; } set { if(_Statement!=null) _Statement.Parent=null; _Statement= value;if(_Statement!=null) _Statement.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Condition!=null) yield return Condition;
            if(Statement!=null) yield return Statement;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitDoWhileStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitDoWhileStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JIfStatement : JStatement
    {
        JExpression _Condition;
        public JExpression Condition { get { return _Condition; } set { if(_Condition!=null) _Condition.Parent=null; _Condition= value;if(_Condition!=null) _Condition.Parent=this;} }
        JStatement _IfStatement;
        public JStatement IfStatement { get { return _IfStatement; } set { if(_IfStatement!=null) _IfStatement.Parent=null; _IfStatement= value;if(_IfStatement!=null) _IfStatement.Parent=this;} }
        JStatement _ElseStatement;
        public JStatement ElseStatement { get { return _ElseStatement; } set { if(_ElseStatement!=null) _ElseStatement.Parent=null; _ElseStatement= value;if(_ElseStatement!=null) _ElseStatement.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Condition!=null) yield return Condition;
            if(IfStatement!=null) yield return IfStatement;
            if(ElseStatement!=null) yield return ElseStatement;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitIfStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitIfStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JForStatement : JStatement
    {
        IList<JStatement> _Initializers;
        public IList<JStatement> Initializers { get { return _Initializers; } set { _Initializers.SetItems(value);} }
        JExpression _Condition;
        public JExpression Condition { get { return _Condition; } set { if(_Condition!=null) _Condition.Parent=null; _Condition= value;if(_Condition!=null) _Condition.Parent=this;} }
        IList<JStatement> _Iterators;
        public IList<JStatement> Iterators { get { return _Iterators; } set { _Iterators.SetItems(value);} }
        JStatement _Statement;
        public JStatement Statement { get { return _Statement; } set { if(_Statement!=null) _Statement.Parent=null; _Statement= value;if(_Statement!=null) _Statement.Parent=this;} }
        public JForStatement()
        {
            Init();
            _Initializers =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
            _Iterators =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Initializers!=null) foreach(var x in Initializers) yield return x;
            if(Condition!=null) yield return Condition;
            if(Iterators!=null) foreach(var x in Iterators) yield return x;
            if(Statement!=null) yield return Statement;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitForStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitForStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JForInStatement : JStatement
    {
        JVariableDeclarationExpression _Initializer;
        public JVariableDeclarationExpression Initializer { get { return _Initializer; } set { if(_Initializer!=null) _Initializer.Parent=null; _Initializer= value;if(_Initializer!=null) _Initializer.Parent=this;} }
        JExpression _Member;
        public JExpression Member { get { return _Member; } set { if(_Member!=null) _Member.Parent=null; _Member= value;if(_Member!=null) _Member.Parent=this;} }
        JStatement _Statement;
        public JStatement Statement { get { return _Statement; } set { if(_Statement!=null) _Statement.Parent=null; _Statement= value;if(_Statement!=null) _Statement.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Initializer!=null) yield return Initializer;
            if(Member!=null) yield return Member;
            if(Statement!=null) yield return Statement;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitForInStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitForInStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JContinueStatement : JStatement
    {
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitContinueStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitContinueStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JBlock : JStatement
    {
        IList<JStatement> _Statements;
        public IList<JStatement> Statements { get { return _Statements; } set { _Statements.SetItems(value);} }
        public JBlock()
        {
            Init();
            _Statements =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Statements!=null) foreach(var x in Statements) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitBlock(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitBlock(this); }
    }
    [DebuggerStepThrough]
    partial class JThrowStatement : JStatement
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitThrowStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitThrowStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JTryStatement : JStatement
    {
        JBlock _TryBlock;
        public JBlock TryBlock { get { return _TryBlock; } set { if(_TryBlock!=null) _TryBlock.Parent=null; _TryBlock= value;if(_TryBlock!=null) _TryBlock.Parent=this;} }
        JCatchClause _CatchClause;
        public JCatchClause CatchClause { get { return _CatchClause; } set { if(_CatchClause!=null) _CatchClause.Parent=null; _CatchClause= value;if(_CatchClause!=null) _CatchClause.Parent=this;} }
        JBlock _FinallyBlock;
        public JBlock FinallyBlock { get { return _FinallyBlock; } set { if(_FinallyBlock!=null) _FinallyBlock.Parent=null; _FinallyBlock= value;if(_FinallyBlock!=null) _FinallyBlock.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(TryBlock!=null) yield return TryBlock;
            if(CatchClause!=null) yield return CatchClause;
            if(FinallyBlock!=null) yield return FinallyBlock;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitTryStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitTryStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JBreakStatement : JStatement
    {
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitBreakStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitBreakStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JExpressionStatement : JStatement
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitExpressionStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitExpressionStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JReturnStatement : JStatement
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitReturnStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitReturnStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JVariableDeclarationStatement : JStatement
    {
        JVariableDeclarationExpression _Declaration;
        public JVariableDeclarationExpression Declaration { get { return _Declaration; } set { if(_Declaration!=null) _Declaration.Parent=null; _Declaration= value;if(_Declaration!=null) _Declaration.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Declaration!=null) yield return Declaration;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitVariableDeclarationStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitVariableDeclarationStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JCommentStatement : JStatement
    {
        public string Text {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitCommentStatement(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitCommentStatement(this); }
    }
    [DebuggerStepThrough]
    partial class JConditionalExpression : JExpression
    {
        JExpression _Condition;
        public JExpression Condition { get { return _Condition; } set { if(_Condition!=null) _Condition.Parent=null; _Condition= value;if(_Condition!=null) _Condition.Parent=this;} }
        JExpression _TrueExpression;
        public JExpression TrueExpression { get { return _TrueExpression; } set { if(_TrueExpression!=null) _TrueExpression.Parent=null; _TrueExpression= value;if(_TrueExpression!=null) _TrueExpression.Parent=this;} }
        JExpression _FalseExpression;
        public JExpression FalseExpression { get { return _FalseExpression; } set { if(_FalseExpression!=null) _FalseExpression.Parent=null; _FalseExpression= value;if(_FalseExpression!=null) _FalseExpression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Condition!=null) yield return Condition;
            if(TrueExpression!=null) yield return TrueExpression;
            if(FalseExpression!=null) yield return FalseExpression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitConditionalExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitConditionalExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JAssignmentExpression : JExpression
    {
        public string Operator {get;set;}
        JExpression _Left;
        public JExpression Left { get { return _Left; } set { if(_Left!=null) _Left.Parent=null; _Left= value;if(_Left!=null) _Left.Parent=this;} }
        JExpression _Right;
        public JExpression Right { get { return _Right; } set { if(_Right!=null) _Right.Parent=null; _Right= value;if(_Right!=null) _Right.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Left!=null) yield return Left;
            if(Right!=null) yield return Right;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitAssignmentExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitAssignmentExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JParenthesizedExpression : JExpression
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitParenthesizedExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitParenthesizedExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JBinaryExpression : JExpression
    {
        public string Operator {get;set;}
        JExpression _Left;
        public JExpression Left { get { return _Left; } set { if(_Left!=null) _Left.Parent=null; _Left= value;if(_Left!=null) _Left.Parent=this;} }
        JExpression _Right;
        public JExpression Right { get { return _Right; } set { if(_Right!=null) _Right.Parent=null; _Right= value;if(_Right!=null) _Right.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Left!=null) yield return Left;
            if(Right!=null) yield return Right;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitBinaryExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitBinaryExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JPostUnaryExpression : JExpression
    {
        public string Operator {get;set;}
        JExpression _Left;
        public JExpression Left { get { return _Left; } set { if(_Left!=null) _Left.Parent=null; _Left= value;if(_Left!=null) _Left.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Left!=null) yield return Left;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitPostUnaryExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitPostUnaryExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JPreUnaryExpression : JExpression
    {
        public string Operator {get;set;}
        JExpression _Right;
        public JExpression Right { get { return _Right; } set { if(_Right!=null) _Right.Parent=null; _Right= value;if(_Right!=null) _Right.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Right!=null) yield return Right;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitPreUnaryExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitPreUnaryExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JStringExpression : JExpression
    {
        public string Value {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitStringExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitStringExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JNullExpression : JExpression
    {
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNullExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNullExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JVariableDeclarationExpression : JExpression
    {
        IList<JVariableDeclarator> _Declarators;
        public IList<JVariableDeclarator> Declarators { get { return _Declarators; } set { _Declarators.SetItems(value);} }
        public JVariableDeclarationExpression()
        {
            Init();
            _Declarators =  new CompositionList<JVariableDeclarator>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Declarators!=null) foreach(var x in Declarators) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitVariableDeclarationExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitVariableDeclarationExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JVariableDeclarator : JNode
    {
        public string Name {get;set;}
        JExpression _Initializer;
        public JExpression Initializer { get { return _Initializer; } set { if(_Initializer!=null) _Initializer.Parent=null; _Initializer= value;if(_Initializer!=null) _Initializer.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Initializer!=null) yield return Initializer;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitVariableDeclarator(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitVariableDeclarator(this); }
    }
    [DebuggerStepThrough]
    partial class JNewObjectExpression : JExpression
    {
        JInvocationExpression _Invocation;
        public JInvocationExpression Invocation { get { return _Invocation; } set { if(_Invocation!=null) _Invocation.Parent=null; _Invocation= value;if(_Invocation!=null) _Invocation.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Invocation!=null) yield return Invocation;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNewObjectExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNewObjectExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JNewArrayExpression : JExpression
    {
        JExpression _Size;
        public JExpression Size { get { return _Size; } set { if(_Size!=null) _Size.Parent=null; _Size= value;if(_Size!=null) _Size.Parent=this;} }
        IList<JExpression> _Items;
        public IList<JExpression> Items { get { return _Items; } set { _Items.SetItems(value);} }
        public JNewArrayExpression()
        {
            Init();
            _Items =  new CompositionList<JExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Size!=null) yield return Size;
            if(Items!=null) foreach(var x in Items) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNewArrayExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNewArrayExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JFunction : JExpression
    {
        public string Name {get;set;}
        public IList<string> Parameters {get;set;}
        JBlock _Block;
        public JBlock Block { get { return _Block; } set { if(_Block!=null) _Block.Parent=null; _Block= value;if(_Block!=null) _Block.Parent=this;} }
        public JFunction()
        {
            Init();
            Parameters =  new List<string>();
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Block!=null) yield return Block;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitFunction(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitFunction(this); }
    }
    [DebuggerStepThrough]
    partial class JInvocationExpression : JExpression
    {
        JExpression _Member;
        public JExpression Member { get { return _Member; } set { if(_Member!=null) _Member.Parent=null; _Member= value;if(_Member!=null) _Member.Parent=this;} }
        IList<JExpression> _Arguments;
        public IList<JExpression> Arguments { get { return _Arguments; } set { _Arguments.SetItems(value);} }
        public string ArgumentsPrefix {get;set;}
        public string ArgumentsSuffix {get;set;}
        public bool OmitParanthesis {get;set;}
        public bool OmitCommas {get;set;}
        public IMethod Method {get;set;}
        public JInvocationExpression()
        {
            Init();
            _Arguments =  new CompositionList<JExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Member!=null) yield return Member;
            if(Arguments!=null) foreach(var x in Arguments) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitInvocationExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitInvocationExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JIndexerAccessExpression : JExpression
    {
        JExpression _Member;
        public JExpression Member { get { return _Member; } set { if(_Member!=null) _Member.Parent=null; _Member= value;if(_Member!=null) _Member.Parent=this;} }
        IList<JExpression> _Arguments;
        public IList<JExpression> Arguments { get { return _Arguments; } set { _Arguments.SetItems(value);} }
        public JIndexerAccessExpression()
        {
            Init();
            _Arguments =  new CompositionList<JExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Member!=null) yield return Member;
            if(Arguments!=null) foreach(var x in Arguments) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitIndexerAccessExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitIndexerAccessExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JMemberExpression : JExpression
    {
        public string Name {get;set;}
        IList<JMemberExpression> _GenericArguments;
        public IList<JMemberExpression> GenericArguments { get { return _GenericArguments; } set { _GenericArguments.SetItems(value);} }
        JExpression _PreviousMember;
        public JExpression PreviousMember { get { return _PreviousMember; } set { if(_PreviousMember!=null) _PreviousMember.Parent=null; _PreviousMember= value;if(_PreviousMember!=null) _PreviousMember.Parent=this;} }
        public IMember Member {get;set;}
        public JMemberExpression()
        {
            Init();
            _GenericArguments =  new CompositionList<JMemberExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(GenericArguments!=null) foreach(var x in GenericArguments) yield return x;
            if(PreviousMember!=null) yield return PreviousMember;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitMemberExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitMemberExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JThis : JMemberExpression
    {
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitThis(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitThis(this); }
    }
    [DebuggerStepThrough]
    partial class JStatementExpressionList : JExpression
    {
        IList<JExpression> _Expressions;
        public IList<JExpression> Expressions { get { return _Expressions; } set { _Expressions.SetItems(value);} }
        public JStatementExpressionList()
        {
            Init();
            _Expressions =  new CompositionList<JExpression>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expressions!=null) foreach(var x in Expressions) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitStatementExpressionList(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitStatementExpressionList(this); }
    }
    [DebuggerStepThrough]
    partial class JNumberExpression : JExpression
    {
        public double Value {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNumberExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNumberExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JRegexExpression : JExpression
    {
        public string Code {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitRegexExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitRegexExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JCatchClause : JNode
    {
        public string IdentifierName {get;set;}
        JBlock _Block;
        public JBlock Block { get { return _Block; } set { if(_Block!=null) _Block.Parent=null; _Block= value;if(_Block!=null) _Block.Parent=this;} }
        JMemberExpression _Type;
        public JMemberExpression Type { get { return _Type; } set { if(_Type!=null) _Type.Parent=null; _Type= value;if(_Type!=null) _Type.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Block!=null) yield return Block;
            if(Type!=null) yield return Type;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitCatchClause(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitCatchClause(this); }
    }
    [DebuggerStepThrough]
    partial class JCodeExpression : JExpression
    {
        public string Code {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitCodeExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitCodeExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JCastExpression : JExpression
    {
        JExpression _Expression;
        public JExpression Expression { get { return _Expression; } set { if(_Expression!=null) _Expression.Parent=null; _Expression= value;if(_Expression!=null) _Expression.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Expression!=null) yield return Expression;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitCastExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitCastExpression(this); }
    }
    [DebuggerStepThrough]
    partial class JNodeList : JNode
    {
        IList<JNode> _Nodes;
        public IList<JNode> Nodes { get { return _Nodes; } set { _Nodes.SetItems(value);} }
        public JNodeList()
        {
            Init();
            _Nodes =  new CompositionList<JNode>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Nodes!=null) foreach(var x in Nodes) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitNodeList(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitNodeList(this); }
    }
    [DebuggerStepThrough]
    partial class JUnit : JNode
    {
        IList<JStatement> _Statements;
        public IList<JStatement> Statements { get { return _Statements; } set { _Statements.SetItems(value);} }
        public JUnit()
        {
            Init();
            _Statements =  new CompositionList<JStatement>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Statements!=null) foreach(var x in Statements) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitUnit(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitUnit(this); }
    }
    [DebuggerStepThrough]
    partial class JJsonMember : JNode
    {
        public bool IsStringLiteral {get;set;}
        public string Name {get;set;}
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitJsonMember(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitJsonMember(this); }
    }
    [DebuggerStepThrough]
    partial class JJsonNameValue : JNode
    {
        JJsonMember _Name;
        public JJsonMember Name { get { return _Name; } set { if(_Name!=null) _Name.Parent=null; _Name= value;if(_Name!=null) _Name.Parent=this;} }
        JExpression _Value;
        public JExpression Value { get { return _Value; } set { if(_Value!=null) _Value.Parent=null; _Value= value;if(_Value!=null) _Value.Parent=this;} }
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(Name!=null) yield return Name;
            if(Value!=null) yield return Value;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitJsonNameValue(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitJsonNameValue(this); }
    }
    [DebuggerStepThrough]
    partial class JJsonObjectExpression : JExpression
    {
        IList<JJsonNameValue> _NamesValues;
        public IList<JJsonNameValue> NamesValues { get { return _NamesValues; } set { _NamesValues.SetItems(value);} }
        public JJsonObjectExpression()
        {
            Init();
            _NamesValues =  new CompositionList<JJsonNameValue>(t=>t.Parent=this, t=>t.Parent=null);
        }
        partial void Init();
        public override IEnumerable<JNode> Children()
        {
            foreach(var x in base.Children()) yield return x;
            if(NamesValues!=null) foreach(var x in NamesValues) yield return x;
        }
        public override R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { return visitor.VisitJsonObjectExpression(this); }
        public override void AcceptVisitor(IJNodeVisitor visitor) { visitor.VisitJsonObjectExpression(this); }
    }
    interface IJNodeVisitor<out R>
    {
        R VisitNode(JNode node);
        R VisitCompilationUnit(JCompilationUnit node);
        R VisitImport(JImport node);
        R VisitEntityDeclaration(JEntityDeclaration node);
        R VisitAnnotationDeclaration(JAnnotationDeclaration node);
        R VisitAnnotationNamedParameter(JAnnotationNamedParameter node);
        R VisitClassDeclaration(JClassDeclaration node);
        R VisitMethodDeclaration(JMethodDeclaration node);
        R VisitFieldDeclaration(JFieldDeclaration node);
        R VisitParameterDeclaration(JParameterDeclaration node);
        R VisitNewAnonymousClassExpression(JNewAnonymousClassExpression node);
        R VisitMultiStatementExpression(JMultiStatementExpression node);
        R VisitExpression(JExpression node);
        R VisitStatement(JStatement node);
        R VisitSwitchStatement(JSwitchStatement node);
        R VisitSwitchSection(JSwitchSection node);
        R VisitSwitchLabel(JSwitchLabel node);
        R VisitWhileStatement(JWhileStatement node);
        R VisitDoWhileStatement(JDoWhileStatement node);
        R VisitIfStatement(JIfStatement node);
        R VisitForStatement(JForStatement node);
        R VisitForInStatement(JForInStatement node);
        R VisitContinueStatement(JContinueStatement node);
        R VisitBlock(JBlock node);
        R VisitThrowStatement(JThrowStatement node);
        R VisitTryStatement(JTryStatement node);
        R VisitBreakStatement(JBreakStatement node);
        R VisitExpressionStatement(JExpressionStatement node);
        R VisitReturnStatement(JReturnStatement node);
        R VisitVariableDeclarationStatement(JVariableDeclarationStatement node);
        R VisitCommentStatement(JCommentStatement node);
        R VisitConditionalExpression(JConditionalExpression node);
        R VisitAssignmentExpression(JAssignmentExpression node);
        R VisitParenthesizedExpression(JParenthesizedExpression node);
        R VisitBinaryExpression(JBinaryExpression node);
        R VisitPostUnaryExpression(JPostUnaryExpression node);
        R VisitPreUnaryExpression(JPreUnaryExpression node);
        R VisitStringExpression(JStringExpression node);
        R VisitNullExpression(JNullExpression node);
        R VisitVariableDeclarationExpression(JVariableDeclarationExpression node);
        R VisitVariableDeclarator(JVariableDeclarator node);
        R VisitNewObjectExpression(JNewObjectExpression node);
        R VisitNewArrayExpression(JNewArrayExpression node);
        R VisitFunction(JFunction node);
        R VisitInvocationExpression(JInvocationExpression node);
        R VisitIndexerAccessExpression(JIndexerAccessExpression node);
        R VisitMemberExpression(JMemberExpression node);
        R VisitThis(JThis node);
        R VisitStatementExpressionList(JStatementExpressionList node);
        R VisitNumberExpression(JNumberExpression node);
        R VisitRegexExpression(JRegexExpression node);
        R VisitCatchClause(JCatchClause node);
        R VisitCodeExpression(JCodeExpression node);
        R VisitCastExpression(JCastExpression node);
        R VisitNodeList(JNodeList node);
        R VisitUnit(JUnit node);
        R VisitJsonMember(JJsonMember node);
        R VisitJsonNameValue(JJsonNameValue node);
        R VisitJsonObjectExpression(JJsonObjectExpression node);
    }
    interface IJNodeVisitor
    {
        void VisitNode(JNode node);
        void VisitCompilationUnit(JCompilationUnit node);
        void VisitImport(JImport node);
        void VisitEntityDeclaration(JEntityDeclaration node);
        void VisitAnnotationDeclaration(JAnnotationDeclaration node);
        void VisitAnnotationNamedParameter(JAnnotationNamedParameter node);
        void VisitClassDeclaration(JClassDeclaration node);
        void VisitMethodDeclaration(JMethodDeclaration node);
        void VisitFieldDeclaration(JFieldDeclaration node);
        void VisitParameterDeclaration(JParameterDeclaration node);
        void VisitNewAnonymousClassExpression(JNewAnonymousClassExpression node);
        void VisitMultiStatementExpression(JMultiStatementExpression node);
        void VisitExpression(JExpression node);
        void VisitStatement(JStatement node);
        void VisitSwitchStatement(JSwitchStatement node);
        void VisitSwitchSection(JSwitchSection node);
        void VisitSwitchLabel(JSwitchLabel node);
        void VisitWhileStatement(JWhileStatement node);
        void VisitDoWhileStatement(JDoWhileStatement node);
        void VisitIfStatement(JIfStatement node);
        void VisitForStatement(JForStatement node);
        void VisitForInStatement(JForInStatement node);
        void VisitContinueStatement(JContinueStatement node);
        void VisitBlock(JBlock node);
        void VisitThrowStatement(JThrowStatement node);
        void VisitTryStatement(JTryStatement node);
        void VisitBreakStatement(JBreakStatement node);
        void VisitExpressionStatement(JExpressionStatement node);
        void VisitReturnStatement(JReturnStatement node);
        void VisitVariableDeclarationStatement(JVariableDeclarationStatement node);
        void VisitCommentStatement(JCommentStatement node);
        void VisitConditionalExpression(JConditionalExpression node);
        void VisitAssignmentExpression(JAssignmentExpression node);
        void VisitParenthesizedExpression(JParenthesizedExpression node);
        void VisitBinaryExpression(JBinaryExpression node);
        void VisitPostUnaryExpression(JPostUnaryExpression node);
        void VisitPreUnaryExpression(JPreUnaryExpression node);
        void VisitStringExpression(JStringExpression node);
        void VisitNullExpression(JNullExpression node);
        void VisitVariableDeclarationExpression(JVariableDeclarationExpression node);
        void VisitVariableDeclarator(JVariableDeclarator node);
        void VisitNewObjectExpression(JNewObjectExpression node);
        void VisitNewArrayExpression(JNewArrayExpression node);
        void VisitFunction(JFunction node);
        void VisitInvocationExpression(JInvocationExpression node);
        void VisitIndexerAccessExpression(JIndexerAccessExpression node);
        void VisitMemberExpression(JMemberExpression node);
        void VisitThis(JThis node);
        void VisitStatementExpressionList(JStatementExpressionList node);
        void VisitNumberExpression(JNumberExpression node);
        void VisitRegexExpression(JRegexExpression node);
        void VisitCatchClause(JCatchClause node);
        void VisitCodeExpression(JCodeExpression node);
        void VisitCastExpression(JCastExpression node);
        void VisitNodeList(JNodeList node);
        void VisitUnit(JUnit node);
        void VisitJsonMember(JJsonMember node);
        void VisitJsonNameValue(JJsonNameValue node);
        void VisitJsonObjectExpression(JJsonObjectExpression node);
    }
}
