using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using JSharp.Compiler;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;
using JSharp.Java;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    partial class JCodeImporter
    {
        public CompilerTool Compiler { get; set; }
        public NProject2 Project { get; set; }
        #region Visit
        int VisitDepth;
        const int MaxVisitDepth = 100;
        [DebuggerStepThrough]
        public JNode Visit(AstNode node)
        {
            if (node == null)
                return null;
            VisitDepth++;
            if (VisitDepth > MaxVisitDepth)
                throw new Exception("StackOverflow imminent, depth>" + MaxVisitDepth);
            try
            {
                var node2 = _Visit(node);
                return node2;
            }
            catch (CompilerException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new CompilerException(node, e);
            }
            finally
            {
                VisitDepth--;
            }
        }
        [DebuggerStepThrough]
        JNode _Visit(AstNode node)
        {
            if (CompilerConfig.Current.EnableLogging)
            {
                var region = node.GetRegion();
                if (region != null && !region.IsEmpty)
                    Log.Debug(String.Format("JsCodeImporter: Visit AstNode: {0}", ToDebug(node)));
            }
            if (BeforeConvertCsToJsAstNode != null)
                BeforeConvertCsToJsAstNode(node);
            var node2 = node.AcceptVisitor(this);
            if (node2 != null)
            {
                var ex = node2.Ex(true);
                if (ex.AstNode == null)
                    ex.AstNode = node;
            }
            if (AfterConvertCsToJsAstNode != null)
                AfterConvertCsToJsAstNode(node, node2);
            return node2;

        }
        string ToDebug(AstNode node)
        {
            if (node == null)
                return null;
            var region = node.GetRegion();
            if (region != null && !region.IsEmpty)
                return String.Format("{1}: [{2}, {3}] - {0}", node.GetType().Name, region.FileName, region.BeginLine, region.BeginColumn);
            return node.GetType().Name;
        }
        [DebuggerStepThrough]
        private List<JExpression> VisitExpressions(IEnumerable<Expression> nodes)
        {
            return nodes.Select(VisitExpression).ToList();
        }
        [DebuggerStepThrough]
        public JExpression VisitExpression(Expression node)
        {
            return (JExpression)Visit(node);
        }
        [DebuggerStepThrough]
        private JStatement VisitStatement(Statement node)
        {
            return (JStatement)Visit(node);
        }

        public string _Visit(BinaryOperatorType op)
        {
            return BinaryOperatorExpression.GetOperatorRole(op).Token;
        }
        public string _Visit(UnaryOperatorType op)
        {
            return UnaryOperatorExpression.GetOperatorRole(op).Token;
        }
        private List<JStatement> VisitStatements(AstNodeCollection<Statement> list)
        {
            return list.Select(VisitStatement).ToList();
        }
        private string Visit(System.Linq.Expressions.ExpressionType op)
        {
            switch (op)
            {
                case System.Linq.Expressions.ExpressionType.Add: return "+";
                case System.Linq.Expressions.ExpressionType.AddAssign: return "+=";
                case System.Linq.Expressions.ExpressionType.AddAssignChecked: break;
                case System.Linq.Expressions.ExpressionType.AddChecked: break;
                case System.Linq.Expressions.ExpressionType.And: return "&";
                case System.Linq.Expressions.ExpressionType.AndAlso: return "&&";
                case System.Linq.Expressions.ExpressionType.AndAssign: return "&=";
                case System.Linq.Expressions.ExpressionType.ArrayIndex: break;
                case System.Linq.Expressions.ExpressionType.ArrayLength: break;
                case System.Linq.Expressions.ExpressionType.Assign: return "=";
                case System.Linq.Expressions.ExpressionType.Block: break;
                case System.Linq.Expressions.ExpressionType.Call: break;
                case System.Linq.Expressions.ExpressionType.Coalesce: return "??";
                case System.Linq.Expressions.ExpressionType.Conditional: break;
                case System.Linq.Expressions.ExpressionType.Constant: break;
                case System.Linq.Expressions.ExpressionType.Convert: break;
                case System.Linq.Expressions.ExpressionType.ConvertChecked: break;
                case System.Linq.Expressions.ExpressionType.DebugInfo: break;
                case System.Linq.Expressions.ExpressionType.Decrement: break;
                case System.Linq.Expressions.ExpressionType.Default: break;
                case System.Linq.Expressions.ExpressionType.Divide: return "/";
                case System.Linq.Expressions.ExpressionType.DivideAssign: return "/=";
                case System.Linq.Expressions.ExpressionType.Dynamic: break;
                case System.Linq.Expressions.ExpressionType.Equal: return "==";
                case System.Linq.Expressions.ExpressionType.ExclusiveOr: return "^";
                case System.Linq.Expressions.ExpressionType.ExclusiveOrAssign: return "^=";
                case System.Linq.Expressions.ExpressionType.Extension: break;
                case System.Linq.Expressions.ExpressionType.Goto: break;
                case System.Linq.Expressions.ExpressionType.GreaterThan: return ">";
                case System.Linq.Expressions.ExpressionType.GreaterThanOrEqual: return ">=";
                case System.Linq.Expressions.ExpressionType.Increment: break;
                case System.Linq.Expressions.ExpressionType.Index: break;
                case System.Linq.Expressions.ExpressionType.Invoke: break;
                case System.Linq.Expressions.ExpressionType.IsFalse: break;
                case System.Linq.Expressions.ExpressionType.IsTrue: break;
                case System.Linq.Expressions.ExpressionType.Label: break;
                case System.Linq.Expressions.ExpressionType.Lambda: break;
                case System.Linq.Expressions.ExpressionType.LeftShift: return "<<";
                case System.Linq.Expressions.ExpressionType.LeftShiftAssign: return "<<=";
                case System.Linq.Expressions.ExpressionType.LessThan: return "<";
                case System.Linq.Expressions.ExpressionType.LessThanOrEqual: return "<=";
                case System.Linq.Expressions.ExpressionType.ListInit: break;
                case System.Linq.Expressions.ExpressionType.Loop: break;
                case System.Linq.Expressions.ExpressionType.MemberAccess: break;
                case System.Linq.Expressions.ExpressionType.MemberInit: break;
                case System.Linq.Expressions.ExpressionType.Modulo: return "%";
                case System.Linq.Expressions.ExpressionType.ModuloAssign: return "%=";
                case System.Linq.Expressions.ExpressionType.Multiply: return "*";
                case System.Linq.Expressions.ExpressionType.MultiplyAssign: return "*=";
                case System.Linq.Expressions.ExpressionType.MultiplyAssignChecked: break;
                case System.Linq.Expressions.ExpressionType.MultiplyChecked: break;
                case System.Linq.Expressions.ExpressionType.Negate: return "-";
                case System.Linq.Expressions.ExpressionType.NegateChecked: break;
                case System.Linq.Expressions.ExpressionType.New: break;
                case System.Linq.Expressions.ExpressionType.NewArrayBounds: break;
                case System.Linq.Expressions.ExpressionType.NewArrayInit: break;
                case System.Linq.Expressions.ExpressionType.Not: return "!";
                case System.Linq.Expressions.ExpressionType.NotEqual: return "!=";
                case System.Linq.Expressions.ExpressionType.OnesComplement: return "~";
                case System.Linq.Expressions.ExpressionType.Or: return "|";
                case System.Linq.Expressions.ExpressionType.OrAssign: return "|=";
                case System.Linq.Expressions.ExpressionType.OrElse: return "||";
                case System.Linq.Expressions.ExpressionType.Parameter: break;
                case System.Linq.Expressions.ExpressionType.PostDecrementAssign: return "--";
                case System.Linq.Expressions.ExpressionType.PostIncrementAssign: return "++";
                case System.Linq.Expressions.ExpressionType.Power: break;
                case System.Linq.Expressions.ExpressionType.PowerAssign: break;
                case System.Linq.Expressions.ExpressionType.PreDecrementAssign: return "--";
                case System.Linq.Expressions.ExpressionType.PreIncrementAssign: return "++";
                case System.Linq.Expressions.ExpressionType.Quote: break;
                case System.Linq.Expressions.ExpressionType.RightShift: return ">>";
                case System.Linq.Expressions.ExpressionType.RightShiftAssign: return ">>=";
                case System.Linq.Expressions.ExpressionType.RuntimeVariables: break;
                case System.Linq.Expressions.ExpressionType.Subtract: return "-";
                case System.Linq.Expressions.ExpressionType.SubtractAssign: return "-=";
                case System.Linq.Expressions.ExpressionType.SubtractAssignChecked: break;
                case System.Linq.Expressions.ExpressionType.SubtractChecked: break;
                case System.Linq.Expressions.ExpressionType.Switch: break;
                case System.Linq.Expressions.ExpressionType.Throw: break;
                case System.Linq.Expressions.ExpressionType.Try: break;
                case System.Linq.Expressions.ExpressionType.TypeAs: break;
                case System.Linq.Expressions.ExpressionType.TypeEqual: break;
                case System.Linq.Expressions.ExpressionType.TypeIs: break;
                case System.Linq.Expressions.ExpressionType.UnaryPlus: break;
                case System.Linq.Expressions.ExpressionType.Unbox: break;
                default: break;
            }
            throw new NotImplementedException(op.ToString());
        }

        #endregion

        #region Utils

        void TransformIntoBaseMethodCallIfNeeded(CSharpInvocationResolveResult res, JInvocationExpression node2)
        {
            var target = res.TargetResult as ThisResolveResult;
            if (target != null && target.CausesNonVirtualInvocation) //base.
            {
                //var info = res.GetInfo();
                //var node = info.Nodes.FirstOrDefault();
                var ce = target.Type;// node.FindThisEntity();
                if (ce != null && JMeta.IsExtJsType(ce.GetDefinitionOrArrayType(Compiler)))
                {
                    node2.Member = J.This().Member("callParent");
                    if (node2.Arguments.IsNotNullOrEmpty())
                        node2.Arguments = new List<JExpression> { J.NewJsonArray(node2.Arguments.ToArray()) };
                    //var me2 = (node2.Member as JsMemberExpression);
                    //me2.Name = "callParent";
                    return;

                }
                IMethod me2;
                var me = res.Member;
                if (me is IProperty)
                    me2 = ((IProperty)me).Getter;
                else if (me is IMethod)
                    me2 = (IMethod)res.Member;
                else
                    throw new Exception("Can't resolve method from member: " + res.Member);
                ((JMemberExpression)node2.Member).PreviousMember = J.Member("super");

            }
        }

        public CompilerLogger Log { get; set; }
        public bool ExportComments { get; set; }


        int VariableExceptionCounter = 1;
        int VariableIteratorCounter = 1;
        int VariableResourceCounter = 1;
        int VariableInitializerCounter = 1;
        int ParameterNameCounter = 1;



        JExpression CreateJsDelegate(JExpression instanceContext, JExpression func)
        {
            if (instanceContext == null)
                return func;
            return new JInvocationExpression { Member = new JMemberExpression { Name = "$CreateDelegate" }, Arguments = new List<JExpression> { instanceContext, func } };
        }
        JExpression CreateAnonymousJsDelegate(JExpression instanceContext, JExpression func)
        {
            if (instanceContext == null)
                return func;
            return new JInvocationExpression { Member = new JMemberExpression { Name = "$CreateAnonymousDelegate" }, Arguments = new List<JExpression> { instanceContext, func } };
        }
        JExpression CreateJsExtensionDelegate(JExpression prm1, JExpression func)
        {
            if (prm1 == null)
                return func;
            return new JInvocationExpression { Member = new JMemberExpression { Name = "$CreateExtensionDelegate" }, Arguments = new List<JExpression> { prm1, func } };
        }
        JNode CreateJsDelegateIfNeeded(JFunction func, IMember currentMember, IType delType, bool isAnonymous)
        {
            if (currentMember != null && !currentMember.IsStatic && !UseNativeFunctions(delType) && !JMeta.ForceDelegatesAsNativeFunctions(currentMember))
            {
                var instanceContext = new JThis();
                JExpression wrapper;
                if (isAnonymous)
                    wrapper = CreateAnonymousJsDelegate(instanceContext, func);
                else
                    wrapper = CreateJsDelegate(instanceContext, func);
                return wrapper;
            }
            else
            {
                return func;
            }
        }

        /// <summary>
        /// Indicates that object is IProperty that uses getter setter functions, and not native fields
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsEntityFunctionProperty(IEntity entity, ResolveResult scope)
        {
            var pe = entity as IProperty;
            if (pe != null)
            {
                var ce = pe.DeclaringType;
                if (ce != null && ce.Kind == TypeKind.Anonymous)
                {
                    var ce2 = scope.GetParentType();
                    if (ce2 != null && JMeta.UseNativeJsons(ce2))
                        return false;
                }
                return !JMeta.IsNativeField(pe) && !JMeta.UseNativeIndexer(pe); // && !Sk.IsNativeProperty(pe);
            }
            return false;
        }


        bool IsNonStatic(IEntity me)
        {
            if (!me.IsStatic())
                return true;
            if (me is IMethod && JMeta.ExtensionImplementedInInstance((IMethod)me))
                return true;
            return false;
        }
        bool UseNativeFunctions(IType delegateType)
        {
            return UseNativeFunctions(delegateType.GetEntityType());
        }
        bool UseNativeFunctions(ITypeDefinition delegateType)
        {
            if (delegateType != null)
            {
                var att2 = delegateType.GetMetadata<JsDelegateAttribute>();
                if (att2 != null && att2.NativeFunction)
                    return true;
            }
            return false;
        }



        private JExpression ParenthesizeIfNeeded(ResolveResult res, JExpression exp)
        {
            if (exp is JParenthesizedExpression)
                return exp;
            var nodes = res.GetNodes();
            if (nodes == null)
                return exp;
            var cspe = nodes.OfType<ParenthesizedExpression>().FirstOrDefault();
            if (cspe == null)
                return exp;
            return new JParenthesizedExpression { Expression = exp };
        }


        static InitializedObjectResolveResult FindInitializedObjectResolveResult(CSharpInvocationResolveResult res)
        {
            var init1 = res.InitializerStatements[0];
            while (init1 != null && !(init1 is InitializedObjectResolveResult))
            {
                if (init1 is OperatorResolveResult)
                {
                    init1 = ((OperatorResolveResult)init1).Operands[0];
                }
                else if (init1 is CSharpInvocationResolveResult)
                {
                    init1 = ((CSharpInvocationResolveResult)init1).TargetResult;
                }
                else if (init1 is MemberResolveResult)
                {
                    init1 = ((MemberResolveResult)init1).TargetResult;
                }
                else
                {
                    throw new NotImplementedException("FindInitializedObjectResolveResult");
                }
            }
            return ((InitializedObjectResolveResult)init1);

        }


        #endregion
        #region Utils

        [System.Diagnostics.DebuggerStepThrough]
        public JExpression VisitExpression(ResolveResult res)
        {
            return (JExpression)Visit(res);
        }
        [System.Diagnostics.DebuggerStepThrough]
        public List<JExpression> VisitExpressions(IList<ResolveResult> nodes)
        {
            return nodes.Select(VisitExpression).ToList();
        }
        [System.Diagnostics.DebuggerStepThrough]
        public JNode Visit(ResolveResult res)
        {
            try
            {
                if (CompilerConfig.Current.EnableLogging)
                {
                    var node3 = res.GetFirstNode();
                    Log.WriteLine("JsCodeImporter: Visit ResolveResult: {0}, AstNode: {1}", res, ToDebug(node3));
                }
                if (BeforeConvertCsToJsResolveResult != null)
                    BeforeConvertCsToJsResolveResult(res);
                var node2 = res.AcceptVisitor(this);
                if (node2 is JExpression)
                    node2 = ParenthesizeIfNeeded(res, (JExpression)node2);

                var node = res.GetFirstNode();
                if (node != null && node2 != null)
                {
                    var ex = node2.Ex(true);
                    if (ex.AstNode == null)
                        ex.AstNode = node;
                }
                if (AfterConvertCsToJsResolveResult != null)
                    AfterConvertCsToJsResolveResult(res, node2);
                return node2;
            }
            catch (CompilerException e)
            {
                if (e.AstNode == null)
                    e.AstNode = res.GetFirstNode();
                throw e;
            }
            catch (Exception e)
            {
                throw new CompilerException(res.GetFirstNode(), e);
            }
        }

        #endregion


        public event Action<AstNode> BeforeConvertCsToJsAstNode;

        public event Action<AstNode, JNode> AfterConvertCsToJsAstNode;

        public event Action<ResolveResult> BeforeConvertCsToJsResolveResult;

        public event Action<ResolveResult, JNode> AfterConvertCsToJsResolveResult;


    }
}
