//using System.Collections.Generic;

//namespace SharpKit.Java.Ast
//{
//    partial class JNode
//    {
//        public virtual IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JsNodeList
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Nodes != null) foreach (var x in Nodes) yield return x;
//        }
//    }
//    partial class JsUnit
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Statements != null) foreach (var x in Statements) yield return x;
//        }
//    }
//    partial class JStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JSwitchStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//            if (Sections != null) foreach (var x in Sections) yield return x;
//        }
//    }
//    partial class JSwitchSection
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Labels != null) foreach (var x in Labels) yield return x;
//            if (Statements != null) foreach (var x in Statements) yield return x;
//        }
//    }
//    partial class JSwitchLabel
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//        }
//    }
//    partial class JWhileStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Condition != null) yield return Condition;
//            if (Statement != null) yield return Statement;
//        }
//    }
//    partial class JDoWhileStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Condition != null) yield return Condition;
//            if (Statement != null) yield return Statement;
//        }
//    }
//    partial class JIfStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Condition != null) yield return Condition;
//            if (IfStatement != null) yield return IfStatement;
//            if (ElseStatement != null) yield return ElseStatement;
//        }
//    }
//    partial class JsForStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Initializers != null) 
//                foreach(var x in Initializers)
//                    yield return x;
//            if (Condition != null) yield return Condition;
//            if (Iterators != null) 
//                foreach(var x in Iterators)
//                    yield return x;
//            if (Statement != null) yield return Statement;
//        }
//    }
//    partial class JForInStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Initializer != null) yield return Initializer;
//            if (Member != null) yield return Member;
//            if (Statement != null) yield return Statement;
//        }
//    }
//    partial class JContinueStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JBlock
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Statements != null) foreach (var x in Statements) yield return x;
//        }
//    }
//    partial class JThrowStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//        }
//    }
//    partial class JTryStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (TryBlock != null) yield return TryBlock;
//            if (CatchClause != null) yield return CatchClause;
//            if (FinallyBlock != null) yield return FinallyBlock;
//        }
//    }
//    partial class JsBreakStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JExpressionStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//        }
//    }
//    partial class JReturnStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//        }
//    }
//    partial class JVariableDeclarationStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Declaration != null) yield return Declaration;
//        }
//    }
//    partial class JCommentStatement
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JConditionalExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Condition != null) yield return Condition;
//            if (TrueExpression != null) yield return TrueExpression;
//            if (FalseExpression != null) yield return FalseExpression;
//        }
//    }
//    partial class JAssignmentExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Left != null) yield return Left;
//            if (Right != null) yield return Right;
//        }
//    }
//    partial class JParenthesizedExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expression != null) yield return Expression;
//        }
//    }
//    partial class JBinaryExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Left != null) yield return Left;
//            if (Right != null) yield return Right;
//        }
//    }
//    partial class JPostUnaryExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Left != null) yield return Left;
//        }
//    }
//    partial class JPreUnaryExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Right != null) yield return Right;
//        }
//    }
//    partial class JsJsonObjectExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (NamesValues != null) foreach (var x in NamesValues) yield return x;
//        }
//    }
//    partial class JStringExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JsNumberExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JsRegexExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JNullExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JVariableDeclarationExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Declarators != null) foreach (var x in Declarators) yield return x;
//        }
//    }
//    partial class JVariableDeclarator
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Initializer != null) yield return Initializer;
//        }
//    }
//    partial class JNewObjectExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Invocation != null) yield return Invocation;
//        }
//    }
//    partial class JFunction
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Block != null) yield return Block;
//        }
//    }
//    partial class JInvocationExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Member != null) yield return Member;
//            if (Arguments != null) foreach (var x in Arguments) yield return x;
//        }
//    }
//    partial class JIndexerAccessExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Member != null) yield return Member;
//            if (Arguments != null) foreach (var x in Arguments) yield return x;
//        }
//    }
//    partial class JMemberExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (PreviousMember != null) yield return PreviousMember;
//        }
//    }
//    partial class JThis
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (PreviousMember != null) yield return PreviousMember;
//        }
//    }
//    partial class JNewArrayExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Items != null) foreach (var x in Items) yield return x;
//        }
//    }
//    partial class JsStatementExpressionList
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Expressions != null) foreach (var x in Expressions) yield return x;
//        }
//    }
//    partial class JCatchClause
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Block != null) yield return Block;
//        }
//    }
//    partial class JsJsonMember
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JCodeExpression
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            yield break;
//        }
//    }
//    partial class JsJsonNameValue
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Name != null) yield return Name;
//            if (Value != null) yield return Value;
//        }
//    }
//    partial class JsExternalFileUnit
//    {
//        public override IEnumerable<JNode> Children()
//        {
//            if (Statements != null) foreach (var x in Statements) yield return x;
//        }
//    }
//}
