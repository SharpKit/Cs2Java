using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using JSharp.Compiler;
using JSharp.Utils;

namespace JSharp.Java.Ast
{
    static class J
    {
        public static JMemberExpression RemoveGenericArgs(this JMemberExpression exp)
        {
            exp.GenericArguments.Clear();
            return exp;
        }


        public static JExpression JAccess(this IEntity me)
        {
            return JNaming.JAccess(me);
        }

        //public static JInvocationExpression Invoke(this JExpression me, IMethod method, params JExpression[] prms)
        //{
        //    return new JInvocationExpression { Member = me, Method = method, Arguments = prms == null ? null : prms.ToList() };
        //}


        public static JNewObjectExpression New(IMethod ctor, params JExpression[] prms)
        {
            return New(ctor.JAccess(), prms);
        }
        public static JNewObjectExpression New(JExpression ctor, params JExpression[] prms)
        {
            return new JNewObjectExpression { Invocation = ctor.Invoke(prms) };
        }
        public static JNewObjectExpression New(IType type)
        {
            var ctor = type.GetConstructors().FirstOrDefault();
            return New(ctor);
        }



        public static JNewAnonymousClassExpression NewAnonymousType(IType type)
        {
            var x = New(type);
            return new JNewAnonymousClassExpression { Type = x.Type, Invocation = x.Invocation };
        }
        public static JNewAnonymousClassExpression NewAnonymousType(JExpression type)
        {
            var x = New(type);
            return new JNewAnonymousClassExpression { Type = x.Type, Invocation = x.Invocation };
        }

        public static JMemberExpression AddGenericArg(this JMemberExpression exp, JMemberExpression arg)
        {
            exp.GenericArguments.Add(arg);
            return exp;
        }
        public static JMemberExpression CreateTypeRef(IType type)
        {
            return JNaming.JAccess(type);
        }
        public static JMemberExpression CreateTypeRef(string typeName, params string[] genericArgs)
        {
            var node = J.Members(typeName);
            if (genericArgs != null)
                node.GenericArguments = genericArgs.Select(t => J.Member(t)).ToList();
            return node;
        }
        public static JMemberExpression CreateTypeRef(string typeName, JMemberExpression[] genericArgs)
        {
            var node = J.Members(typeName);
            if (genericArgs != null)
                node.GenericArguments = genericArgs.ToList();
            return node;
        }

        public static JNewAnonymousClassExpression CreateDelegate(JMemberExpression delType, JParameterDeclaration[] prms2, JMemberExpression returnType, JBlock body)
        {
            if (prms2 == null)
                prms2 = new JParameterDeclaration[0];
            if (returnType == null)
                returnType = J.Member("void");
            var ce = NewAnonymousType(delType);
            var me = new JMethodDeclaration
            {
                MethodBody = body,
                Parameters = prms2.ToList(),
                Annotations = { new JAnnotationDeclaration { Name = "Override" } },
                Type = returnType,
                Name = "invoke",
                Modifiers = { IsPublic = true },
            };
            ce.Declarations.Add(me);
            return ce;
        }

        public static JNewAnonymousClassExpression CreateActionOrFunc(JBlock body, JParameterDeclaration[] prms, JMemberExpression retType, NProject2 project)
        {
            if (prms == null)
                prms = new JParameterDeclaration[0];
            var argTypes = prms.Select(t => t.Type).ToList();
            var delTypeName = "system.Action";
            var netTypeName = "System.Action";
            if (retType != null)
            {
                delTypeName = "system.Func";
                netTypeName = "System.Func";
                argTypes.Add(retType);
            }
            if (prms.Length > 0)
                delTypeName += prms.Length;
            var delType = CreateTypeRef(delTypeName, argTypes.ToArray());
            if (argTypes.Count > 0)
                netTypeName += "`" + argTypes.Count;
            delType.TypeRef = project.FindType(netTypeName);
            return CreateDelegate(delType, prms, retType, body);
        }
        public static JNewAnonymousClassExpression CreateAction(JBlock body, NProject2 project)
        {
            return CreateActionOrFunc(body, null, null, project);
        }
        public static JNewAnonymousClassExpression CreateFunc(JMemberExpression retType, JBlock body, NProject2 project)
        {
            return CreateActionOrFunc(body, null, retType, project);
        }

        #region Unsorted




        public static JInvocationExpression AddArgument(this JInvocationExpression exp, JExpression arg)
        {
            if (exp.Arguments == null)
                exp.Arguments = new List<JExpression> { arg };
            else
                exp.Arguments.Add(arg);
            return exp;
        }
        public static JInvocationExpression InsertArgument(this JInvocationExpression exp, int index, JExpression arg)
        {
            if (exp.Arguments == null)
                exp.Arguments = new List<JExpression> { arg };
            else
                exp.Arguments.Insert(index, arg);
            return exp;
        }

        public static JVariableDeclarationExpression AndVar(this JVariableDeclarationExpression decl, string variableName, JExpression initializer = null)
        {
            if (decl.Declarators == null)
                decl.Declarators = new List<JVariableDeclarator>();
            decl.Declarators.Add(new JVariableDeclarator { Name = variableName, Initializer = initializer });
            return decl;
        }
        public static JVariableDeclarationExpression Var(string variableName, JMemberExpression type, JExpression initializer = null)
        {
            if (type == null)
                throw new Exception();
            return new JVariableDeclarationExpression { Type = type, Declarators = new List<JVariableDeclarator> { new JVariableDeclarator { Name = variableName, Initializer = initializer } } };
        }

        public static JJsonObjectExpression Json()
        {
            return new JJsonObjectExpression();
        }
        public static JJsonObjectExpression Add(this JJsonObjectExpression exp, string name, JExpression value)
        {
            if (exp.NamesValues == null)
                exp.NamesValues = new List<JJsonNameValue>();
            exp.NamesValues.Add(JsonNameValue(name, value));
            return exp;
        }
        public static JJsonNameValue JsonNameValue(string name, JExpression value)
        {
            return new JJsonNameValue { Name = new JJsonMember { Name = name }, Value = value };
        }
        public static JExpression Value(object value)
        {
            var s = JavaScriptHelper.ToJavaScriptValue(value);
            //if (value is char)
            //    return Code(JavaScriptHelper.ToJavaScriptChar((char)value));
            //else if(value is string)
            //    return Code(JavaScriptHelper.ToJavaScriptString((string)value));
            //var s = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(value);
            return Code(s);//JavaScriptHelper.ToJavaScriptValue(value));
        }

        public static JSwitchStatement Switch(JExpression exp)
        {
            return new JSwitchStatement { Expression = exp, Sections = new List<JSwitchSection>() };
        }

        public static JSwitchStatement Case(this JSwitchStatement node, JExpression value, List<JStatement> statements)
        {
            if (node.Sections == null)
                node.Sections = new List<JSwitchSection>();
            node.Sections.Add(new JSwitchSection { Labels = new List<JSwitchLabel> { new JSwitchLabel { Expression = value } }, Statements = statements });
            return node;
        }

        public static JUnit Unit()
        {
            return new JUnit { Statements = new List<JStatement>() };
        }
        public static JThis This()
        {
            return new JThis();
        }
        public static JFunction Function(params string[] prms)
        {
            return new JFunction { Parameters = prms == null ? null : prms.ToList() };
        }
        public static JBlock Block(this JFunction func)
        {
            if (func.Block == null)
                func.Block = new JBlock { Statements = new List<JStatement>() };
            return func.Block;
        }
        /// <summary>
        /// If statement is a block, cast and return it, otherwise, create a new block with statement inside.
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static JBlock ToBlock(this JStatement st)
        {
            if (st is JBlock)
                return (JBlock)st;
            return new JBlock { Statements = new List<JStatement> { st } };
        }
        public static JBlock Block()
        {
            return new JBlock();
        }
        public static JFunction Add(this JFunction func, JStatement st)
        {
            if (func.Block == null)
                func.Block = new JBlock();
            if (func.Block.Statements == null)
                func.Block.Statements = new List<JStatement>();
            func.Block.Statements.Add(st);
            return func;
        }
        public static JFunction AddStatements(this JFunction func, params JStatement[] sts)
        {
            if (sts.IsNullOrEmpty())
                return func;
            if (func.Block == null)
                func.Block = new JBlock();
            if (func.Block.Statements == null)
                func.Block.Statements = new List<JStatement>();
            func.Block.Statements.AddRange(sts);
            return func;
        }
        public static JBlock Add(this JBlock block, JStatement st)
        {
            if (block.Statements == null)
                block.Statements = new List<JStatement>();
            block.Statements.Add(st);
            return block;
        }
        public static JNullExpression Null()
        {
            return new JNullExpression();
        }
        public static JCodeExpression Code(string code)
        {
            return new JCodeExpression { Code = code };
        }
        public static JReturnStatement Return(JExpression exp = null)
        {
            return new JReturnStatement { Expression = exp };
        }
        public static JStatement Statement(this JExpression exp)
        {
            if (exp != null && exp is JVariableDeclarationExpression)
                return ((JVariableDeclarationExpression)exp).Statement();//new JsVariableDeclarationStatement { Declaration =  };
            return new JExpressionStatement { Expression = exp };
        }
        public static JVariableDeclarationStatement Statement(this JVariableDeclarationExpression exp)
        {
            return new JVariableDeclarationStatement { Declaration = exp };
        }
        public static JIfStatement If(JExpression condition, JStatement ifStatement = null, JStatement elseStatement = null)
        {
            return new JIfStatement { Condition = condition, IfStatement = ifStatement, ElseStatement = elseStatement };
        }
        public static JIfStatement Then(this JIfStatement ifStatement, JStatement thenStatement)
        {
            ifStatement.IfStatement = thenStatement;
            return ifStatement;
        }
        public static JIfStatement Else(this JIfStatement ifStatement, JStatement elseStatement)
        {
            ifStatement.ElseStatement = elseStatement;
            return ifStatement;
        }
        public static JInvocationExpression Typeof(JMemberExpression me)
        {
            return new JInvocationExpression { Member = new JMemberExpression { Name = "typeof" }, Arguments = new List<JExpression> { me } };
        }
        public static JInvocationExpression Invoke(this JExpression me, params JExpression[] prms)
        {
            var x = new JInvocationExpression { Member = me, Arguments = prms == null ? null : prms.ToList() };
            var me2 = me as JMemberExpression;
            if(me2!=null)
                x.Method = me2.Member as IMethod;
            return x;
        }
        public static JInvocationExpression InvokeWithContextIfNeeded(this JExpression me, JExpression context, params JExpression[] prms)
        {
            if (context == null)
                return me.Invoke(prms);
            var prms2 = prms.ToList();
            prms2.Insert(0, context);
            return me.Member("call").Invoke(prms2.ToArray());
        }
        public static JIndexerAccessExpression IndexerAccess(this JExpression me, params JExpression[] prms)
        {
            return new JIndexerAccessExpression { Member = me, Arguments = prms.ToList() };
        }
        public static JNewArrayExpression NewJsonArray(params JExpression[] items)
        {
            return new JNewArrayExpression { Items = items == null ? null : items.ToList() };
        }
        public static JExpression NewArray(IType type, JExpression size, JExpression[] items)
        {
            if (items == null)
                items = new JExpression[0];
            return new JNewArrayExpression { Type = type.JAccess(), Size = size, Items = items.ToList() };
        }
        public static JMemberExpression Member()
        {
            return null;
        }
        public static JMemberExpression Member(string name)
        {
            return new JMemberExpression { Name = name };
        }
        public static JMemberExpression MemberOrSelf(this JMemberExpression member, string name)
        {
            if (name.IsNullOrEmpty())
                return member;
            if (member == null)
                return Member(name);
            return member.Member(name);
        }
        public static JMemberExpression Member(this JExpression member, string name)
        {
            return new JMemberExpression { Name = name, PreviousMember = member };
        }
        public static JMemberExpression Member(this JExpression member, JMemberExpression exp)
        {
            exp.PreviousMember = member;
            return exp;
        }
        public static JBinaryExpression InstanceOf(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "instanceof", Right = exp2 };
        }
        public static JBinaryExpression Equal(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "==", Right = exp2 };
        }
        public static JBinaryExpression NotEqual(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "!=", Right = exp2 };
        }
        public static JBinaryExpression Or(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "||", Right = exp2 };
        }
        public static JBinaryExpression BitwiseOr(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "|", Right = exp2 };
        }
        public static JBinaryExpression LessThan(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "<", Right = exp2 };
        }
        public static JBinaryExpression GreaterThan(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = ">", Right = exp2 };
        }
        public static JBinaryExpression GreaterThanOrEqualTo(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = ">=", Right = exp2 };
        }
        public static JBinaryExpression LessThanOrEqualTo(this JExpression exp, JExpression exp2)
        {
            return new JBinaryExpression { Left = exp, Operator = "<=", Right = exp2 };
        }
        public static JPostUnaryExpression PlusPlus(this JExpression exp)
        {
            return new JPostUnaryExpression { Left = exp, Operator = "++" };
        }
        public static JPostUnaryExpression MinusMinus(this JExpression exp)
        {
            return new JPostUnaryExpression { Left = exp, Operator = "--" };
        }
        public static JParenthesizedExpression Parentheses(this JExpression exp)
        {
            return new JParenthesizedExpression { Expression = exp };
        }
        public static JAssignmentExpression Assign(this JExpression me, JExpression exp2)
        {
            var node = new JAssignmentExpression { Left = me, Right = exp2 };
            return node;
        }
        public static JWhileStatement While(JExpression condition, JStatement statement = null)
        {
            return new JWhileStatement { Condition = condition, Statement = statement };
        }
        public static JStringExpression String(string s)
        {
            return new JStringExpression { Value = s };
        }
        public static JMemberExpression Members(string members)
        {
            JMemberExpression node = null;
            foreach (var token in members.Split('.'))
            {
                node = new JMemberExpression { PreviousMember = node };
                node.Name = token;
            }
            return node;
        }

        public static JNode True()
        {
            return J.Value(true);
        }
        public static JExpression Conditional(this JExpression condition, JExpression trueExp, JExpression falseExp)
        {
            return new JConditionalExpression { Condition = condition, TrueExpression = trueExp, FalseExpression = falseExp };
        }
        public static JThrowStatement ThrowNewError(string msg)
        {
            return new JThrowStatement { Expression = J.New(J.Member("RuntimeException"), J.String(msg)) };
        }

        #endregion


        public static JCastExpression Cast(JExpression exp, JMemberExpression type)
        {
            return new JCastExpression { Expression = exp, Type = type };
        }
    }

    static class JsRefactorer
    {
        /// <summary>
        /// MyFunction(prm1, prm2) -> MyFunction.call(context, prm1, prm2)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void ToCallWithContext(JInvocationExpression node, JExpression context)
        {
            node.Member = node.Member.Member("call");
            if (node.Arguments == null)
                node.Arguments = new List<JExpression>();
            node.Arguments.Insert(0, J.This());
        }
    }


}
