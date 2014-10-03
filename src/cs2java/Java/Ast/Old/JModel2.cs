using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Globalization;
using ICSharpCode.NRefactory.TypeSystem;
using System.Diagnostics;

namespace SharpKit.Java.Ast
{

    //partial class JNode
    //{
    //    public JNodeType NodeType { get; protected set; }
    //    public string ToJs()
    //    {
    //        using (var writer = JWriter.CreateInMemory())
    //        {
    //            writer.Visit(this);
    //            return writer.GetStringBuilder().ToString();
    //        }
    //    }
    //    //public JsNode Parent { get; set; }
    //    public object Metadata { get; set; }
    //    public TextLocation StartLocation { get; set; }
    //    public TextLocation EndLocation { get; set; }
    //    [DebuggerStepThrough]
    //    public virtual IEnumerable<JNode> Children()
    //    {
    //        yield break;
    //    }

    //    [DebuggerStepThrough]
    //    public virtual R AcceptVisitor<R>(IJNodeVisitor<R> visitor) { throw new Exception(); }
    //    [DebuggerStepThrough]
    //    public virtual void AcceptVisitor(IJNodeVisitor visitor) { throw new Exception(); }


    //}
    //public partial class JExpression : JNode
    //{
    //    public IType Type { get; set; }
    //}


    //public partial class JStatement : JNode
    //{
    //    public List<string> Comments { get; set; }
    //}

    //#region Statements
    //public partial class JSwitchStatement : JStatement
    //{
    //    public JExpression Expression { get; set; }
    //    public List<JSwitchSection> Sections { get; set; }
    //}
    //public partial class JSwitchSection : JNode
    //{
    //    public List<JSwitchLabel> Labels { get; set; }
    //    public List<JStatement> Statements { get; set; }
    //}
    //public partial class JSwitchLabel : JNode
    //{
    //    public bool IsDefault { get; set; }
    //    public JExpression Expression { get; set; }
    //}

    //public partial class JWhileStatement : JStatement
    //{
    //    public JExpression Condition { get; set; }
    //    public JStatement Statement { get; set; }
    //}
    //public partial class JDoWhileStatement : JStatement
    //{
    //    public JExpression Condition { get; set; }
    //    public JStatement Statement { get; set; }
    //}
    //public partial class JIfStatement : JStatement
    //{
    //    public JExpression Condition { get; set; }
    //    public JStatement IfStatement { get; set; }
    //    public JStatement ElseStatement { get; set; }
    //}


    //public partial class JsForStatement : JStatement
    //{
    //    public List<JStatement> Initializers { get; set; }
    //    public JExpression Condition { get; set; }
    //    public List<JStatement> Iterators { get; set; }
    //    public JStatement Statement { get; set; }
    //}
    //public partial class JForInStatement : JStatement
    //{
    //    public JVariableDeclarationExpression Initializer { get; set; }
    //    public JExpression Member { get; set; }
    //    public JStatement Statement { get; set; }
    //}
    //public partial class JContinueStatement : JStatement
    //{
    //}
    //public partial class JBlock : JStatement
    //{
    //    public List<JStatement> Statements { get; set; }
    //}

    //public partial class JThrowStatement : JStatement
    //{
    //    public JExpression Expression { get; set; }
    //}
    //public partial class JTryStatement : JStatement
    //{
    //    public JBlock TryBlock { get; set; }
    //    public JCatchClause CatchClause { get; set; }
    //    public JBlock FinallyBlock { get; set; }
    //}

    //public partial class JsBreakStatement : JStatement
    //{
    //}

    //public partial class JExpressionStatement : JStatement
    //{
    //    public JExpression Expression { get; set; }
    //    internal bool SkipSemicolon()
    //    {
    //        if (Expression is JFunction)
    //            return true;
    //        if (Expression is JAssignmentExpression && ((JAssignmentExpression)Expression).Right is JFunction)//TODO: check parent is unit
    //            return true;
    //        if (Expression is JCodeExpression)
    //            return true;
    //        return false;
    //    }
    //}
    //public partial class JReturnStatement : JStatement
    //{
    //    public JExpression Expression { get; set; }
    //}
    //public partial class JVariableDeclarationStatement : JStatement
    //{
    //    public JVariableDeclarationExpression Declaration { get; set; }
    //}
    //public partial class JCommentStatement : JStatement
    //{
    //    public string Text { get; set; }
    //}

    //#endregion



    //#region Expressions
    //public partial class JConditionalExpression : JExpression
    //{
    //    public JExpression Condition { get; set; }
    //    public JExpression TrueExpression { get; set; }
    //    public JExpression FalseExpression { get; set; }
    //}
    //public partial class JAssignmentExpression : JExpression
    //{
    //    public string Operator { get; set; }
    //    public JExpression Left { get; set; }
    //    public JExpression Right { get; set; }
    //}
    //public partial class JParenthesizedExpression : JExpression
    //{
    //    public JExpression Expression { get; set; }
    //}
    //public partial class JBinaryExpression : JExpression
    //{
    //    public string Operator { get; set; }
    //    public JExpression Left { get; set; }
    //    public JExpression Right { get; set; }
    //}
    //public partial class JPostUnaryExpression : JExpression
    //{
    //    public string Operator { get; set; }
    //    public JExpression Left { get; set; }
    //}
    //public partial class JPreUnaryExpression : JExpression
    //{
    //    public string Operator { get; set; }
    //    public JExpression Right { get; set; }
    //}
    //public partial class JStringExpression : JExpression
    //{
    //    public string Value { get; set; }
    //}

    //public partial class JNullExpression : JExpression
    //{
    //}

    //public partial class JVariableDeclarationExpression : JExpression
    //{
    //    public List<JVariableDeclarator> Declarators { get; set; }
    //}
    //public partial class JVariableDeclarator : JNode
    //{
    //    public string Name { get; set; }
    //    public JExpression Initializer { get; set; }
    //}
    //public partial class JNewObjectExpression : JExpression
    //{
    //    public JInvocationExpression Invocation { get; set; }

    //}

    //public partial class JNewArrayExpression : JExpression
    //{
    //    public JExpression Size { get; set; }
    //    public List<JExpression> Items { get; set; }
    //}
    //public partial class JFunction : JExpression
    //{
    //    public string Name { get; set; }
    //    public List<string> Parameters { get; set; }
    //    public JBlock Block { get; set; }
    //}
    //public partial class JInvocationExpression : JExpression
    //{
    //    public JExpression Member { get; set; }
    //    public List<JExpression> Arguments { get; set; }
    //    public string ArgumentsPrefix { get; set; }
    //    public string ArgumentsSuffix { get; set; }
    //    public bool OmitParanthesis { get; set; }

    //    public bool OmitCommas { get; set; }

    //    public IMethod Method { get; set; }
    //}
    //public partial class JIndexerAccessExpression : JExpression
    //{
    //    public JExpression Member { get; set; }
    //    public List<JExpression> Arguments { get; set; }
    //}
    //public partial class JMemberExpression : JExpression
    //{
    //    public string Name { get; set; }
    //    public List<JMemberExpression> GenericArguments { get; set; }
    //    public JExpression PreviousMember { get; set; }

    //    public IMember Member { get; set; }
    //}
    //public partial class JThis : JMemberExpression
    //{
    //}
    //public partial class JsStatementExpressionList : JExpression
    //{
    //    public List<JExpression> Expressions { get; set; }
    //}
    //#endregion


    //#region Depracate
    //public partial class JsUseStrictStatement : JStatement
    //{
    //}

    //public partial class JsNumberExpression : JExpression
    //{
    //    public double Value { get; set; }
    //}
    //public partial class JsRegexExpression : JExpression
    //{
    //    public string Code { get; set; }
    //}

    //public partial class JCatchClause : JNode
    //{
    //    public string IdentifierName { get; set; }
    //    public JBlock Block { get; set; }
    //}

    //public partial class JCodeExpression : JExpression
    //{
    //    internal Action<JWriter> WriteOverride { get; set; }
    //    public string Code { get; set; }
    //}
    //public partial class JsNodeList : JNode
    //{
    //    public List<JNode> Nodes { get; set; }
    //}
    //public partial class JsUnit : JNode
    //{
    //    public List<JStatement> Statements { get; set; }
    //}

    //public partial class JsJsonMember : JNode
    //{
    //    public bool IsStringLiteral { get; set; }
    //    public string Name { get; set; }
    //}

    //public partial class JsJsonNameValue : JNode
    //{
    //    public JsJsonMember Name { get; set; }
    //    public JExpression Value { get; set; }
    //}
    //public partial class JsJsonObjectExpression : JExpression
    //{
    //    public List<JsJsonNameValue> NamesValues { get; set; }
    //}

        
    //#endregion


}
