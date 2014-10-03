//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.CodeDom.Compiler;
//using System.Diagnostics;

//namespace SharpKit.Java.Ast
//{
//    public enum JNodeType
//    {
//        AssignmentExpression,
//        BinaryExpression,
//        Block,
//        BreakStatement,
//        CatchClause,
//        CodeExpression,
//        CommentStatement,
//        ConditionalExpression,
//        ContinueStatement,
//        DoWhileStatement,
//        Expression,
//        ExpressionStatement,
//        ExternalFileUnit,
//        ForInStatement,
//        ForStatement,
//        Function,
//        IfStatement,
//        IndexerAccessExpression,
//        InvocationExpression,
//        JsonArrayExpression,
//        JsonMember,
//        JsonNameValue,
//        JsonObjectExpression,
//        MemberExpression,
//        NewObjectExpression,
//        NodeList,
//        NullExpression,
//        NumberExpression,
//        ParenthesizedExpression,
//        PostUnaryExpression,
//        PreUnaryExpression,
//        RegexExpression,
//        ReturnStatement,
//        Statement,
//        StatementExpressionList,
//        StringExpression,
//        SwitchLabel,
//        SwitchSection,
//        SwitchStatement,
//        This,
//        ThrowStatement,
//        TryStatement,
//        Unit,
//        VariableDeclarationExpression,
//        VariableDeclarationStatement,
//        VariableDeclarator,
//        WhileStatement,
//        UseStrictStatement,
//        CompilationUnit,
//        Import,
//        ClassDeclaration,
//        MethodDeclaration,
//        FieldDeclaration,
//        NewAnonymousClassExpression,
//    }
//    partial class JAssignmentExpression { [DebuggerStepThrough] public JAssignmentExpression() { NodeType = JNodeType.AssignmentExpression; } }
//    partial class JBinaryExpression { [DebuggerStepThrough] public JBinaryExpression() { NodeType = JNodeType.BinaryExpression; } }
//    partial class JBlock { [DebuggerStepThrough] public JBlock() { NodeType = JNodeType.Block; } }
//    partial class JsBreakStatement { [DebuggerStepThrough] public JsBreakStatement() { NodeType = JNodeType.BreakStatement; } }
//    partial class JCatchClause { [DebuggerStepThrough] public JCatchClause() { NodeType = JNodeType.CatchClause; } }
//    partial class JCodeExpression { [DebuggerStepThrough] public JCodeExpression() { NodeType = JNodeType.CodeExpression; } }
//    partial class JCommentStatement { [DebuggerStepThrough] public JCommentStatement() { NodeType = JNodeType.CommentStatement; } }
//    partial class JConditionalExpression { [DebuggerStepThrough] public JConditionalExpression() { NodeType = JNodeType.ConditionalExpression; } }
//    partial class JContinueStatement { [DebuggerStepThrough] public JContinueStatement() { NodeType = JNodeType.ContinueStatement; } }
//    partial class JDoWhileStatement { [DebuggerStepThrough] public JDoWhileStatement() { NodeType = JNodeType.DoWhileStatement; } }
//    partial class JExpression { [DebuggerStepThrough] public JExpression() { NodeType = JNodeType.Expression; } }
//    partial class JExpressionStatement { [DebuggerStepThrough] public JExpressionStatement() { NodeType = JNodeType.ExpressionStatement; } }
//    partial class JsExternalFileUnit { [DebuggerStepThrough] public JsExternalFileUnit() { NodeType = JNodeType.ExternalFileUnit; } }
//    partial class JForInStatement { [DebuggerStepThrough] public JForInStatement() { NodeType = JNodeType.ForInStatement; } }
//    partial class JsForStatement { [DebuggerStepThrough] public JsForStatement() { NodeType = JNodeType.ForStatement; } }
//    partial class JFunction { [DebuggerStepThrough] public JFunction() { NodeType = JNodeType.Function; } }
//    partial class JIfStatement { [DebuggerStepThrough] public JIfStatement() { NodeType = JNodeType.IfStatement; } }
//    partial class JIndexerAccessExpression { [DebuggerStepThrough] public JIndexerAccessExpression() { NodeType = JNodeType.IndexerAccessExpression; } }
//    partial class JInvocationExpression { [DebuggerStepThrough] public JInvocationExpression() { NodeType = JNodeType.InvocationExpression; } }
//    partial class JNewArrayExpression { [DebuggerStepThrough] public JNewArrayExpression() { NodeType = JNodeType.JsonArrayExpression; } }
//    partial class JsJsonMember { [DebuggerStepThrough] public JsJsonMember() { NodeType = JNodeType.JsonMember; } }
//    partial class JsJsonNameValue { [DebuggerStepThrough] public JsJsonNameValue() { NodeType = JNodeType.JsonNameValue; } }
//    partial class JsJsonObjectExpression { [DebuggerStepThrough] public JsJsonObjectExpression() { NodeType = JNodeType.JsonObjectExpression; } }
//    partial class JMemberExpression { [DebuggerStepThrough] public JMemberExpression() { NodeType = JNodeType.MemberExpression; } }
//    partial class JNewObjectExpression { [DebuggerStepThrough] public JNewObjectExpression() { NodeType = JNodeType.NewObjectExpression; } }
//    partial class JsNodeList { [DebuggerStepThrough] public JsNodeList() { NodeType = JNodeType.NodeList; } }
//    partial class JNullExpression { [DebuggerStepThrough] public JNullExpression() { NodeType = JNodeType.NullExpression; } }
//    partial class JsNumberExpression { [DebuggerStepThrough] public JsNumberExpression() { NodeType = JNodeType.NumberExpression; } }
//    partial class JParenthesizedExpression { [DebuggerStepThrough] public JParenthesizedExpression() { NodeType = JNodeType.ParenthesizedExpression; } }
//    partial class JPostUnaryExpression { [DebuggerStepThrough] public JPostUnaryExpression() { NodeType = JNodeType.PostUnaryExpression; } }
//    partial class JPreUnaryExpression { [DebuggerStepThrough] public JPreUnaryExpression() { NodeType = JNodeType.PreUnaryExpression; } }
//    partial class JsRegexExpression { [DebuggerStepThrough] public JsRegexExpression() { NodeType = JNodeType.RegexExpression; } }
//    partial class JReturnStatement { [DebuggerStepThrough] public JReturnStatement() { NodeType = JNodeType.ReturnStatement; } }
//    partial class JStatement { [DebuggerStepThrough] public JStatement() { NodeType = JNodeType.Statement; } }
//    partial class JsStatementExpressionList { [DebuggerStepThrough] public JsStatementExpressionList() { NodeType = JNodeType.StatementExpressionList; } }
//    partial class JStringExpression { [DebuggerStepThrough] public JStringExpression() { NodeType = JNodeType.StringExpression; } }
//    partial class JSwitchLabel { [DebuggerStepThrough] public JSwitchLabel() { NodeType = JNodeType.SwitchLabel; } }
//    partial class JSwitchSection { [DebuggerStepThrough] public JSwitchSection() { NodeType = JNodeType.SwitchSection; } }
//    partial class JSwitchStatement { [DebuggerStepThrough] public JSwitchStatement() { NodeType = JNodeType.SwitchStatement; } }
//    partial class JThis { [DebuggerStepThrough] public JThis() { NodeType = JNodeType.This; } }
//    partial class JThrowStatement { [DebuggerStepThrough] public JThrowStatement() { NodeType = JNodeType.ThrowStatement; } }
//    partial class JTryStatement { [DebuggerStepThrough] public JTryStatement() { NodeType = JNodeType.TryStatement; } }
//    partial class JsUnit { [DebuggerStepThrough] public JsUnit() { NodeType = JNodeType.Unit; } }
//    partial class JVariableDeclarationExpression { [DebuggerStepThrough] public JVariableDeclarationExpression() { NodeType = JNodeType.VariableDeclarationExpression; } }
//    partial class JVariableDeclarationStatement { [DebuggerStepThrough] public JVariableDeclarationStatement() { NodeType = JNodeType.VariableDeclarationStatement; } }
//    partial class JVariableDeclarator { [DebuggerStepThrough] public JVariableDeclarator() { NodeType = JNodeType.VariableDeclarator; } }
//    partial class JWhileStatement { [DebuggerStepThrough] public JWhileStatement() { NodeType = JNodeType.WhileStatement; } }
//    partial class JsUseStrictStatement { [DebuggerStepThrough] public JsUseStrictStatement() { NodeType = JNodeType.UseStrictStatement; } }
//}