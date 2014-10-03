using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSharp.Compiler;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using JSharp.Java;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    partial class JCodeImporter : IAstVisitor<JNode>
    {
        public bool SupportClrYield = false;

        #region IAstVisitor<JsNode> Members

        public JNode VisitAnonymousMethodExpression(AnonymousMethodExpression node)
        {
            return Visit(node.Resolve());
            //var func = new JsFunction();
            //func.Parameters = node.Parameters.Select(t => t.Name).ToList();
            //var body = Visit(node.Body);
            //func.Block = (JsBlock)body;
            //return CreateJsDelegateIfNeeded(func, node, true);
        }


        public JNode VisitArrayCreateExpression(ArrayCreateExpression node)
        {
            return Visit(node.Resolve());
        }


        public JNode VisitAsExpression(AsExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitAssignmentExpression(AssignmentExpression node)
        {
            var res = node.Resolve();
            if (res.Type.Kind == TypeKind.Dynamic)
            {
                return VisitExpression(node.Left).Assign(VisitExpression(node.Right));
            }
            return Visit(res);
        }


        public JNode VisitBinaryOperatorExpression(BinaryOperatorExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitCastExpression(CastExpression node)
        {
            var res = node.Resolve();
            return Visit(res);
        }


        public JNode VisitConditionalExpression(ConditionalExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitDefaultValueExpression(DefaultValueExpression node)
        {
            return Visit(node.Resolve());
            //return Js.Member("Default").Invoke(SkJs.EntityTypeRefToMember(node.type.entity_typeref));
        }


        public JNode VisitIdentifierExpression(IdentifierExpression node)
        {
            var res = node.Resolve();
            return Visit(res);
        }

        public JNode VisitIndexerExpression(IndexerExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitInvocationExpression(InvocationExpression node)
        {
            var res = node.Resolve();
            //TEMP: danel
            //if (res.Type.Kind == TypeKind.Dynamic)
            //{
            //    var node2 = Js.Invoke(VisitExpression(node.Target), VisitExpressions(node.Arguments).ToArray());
            //    return node2;
            //}
            var node3 = Visit(res);
            return node3;
        }

        public JNode VisitIsExpression(IsExpression node)
        {
            var res2 = node.Resolve();
            return Visit(res2);
        }

        public JNode VisitLambdaExpression(LambdaExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitMemberReferenceExpression(MemberReferenceExpression node)
        {
            var res = node.Resolve();
            if (res.Type.Kind == TypeKind.Dynamic)
            {
                return VisitExpression(node.Target).Member(node.MemberName);
            }
            return Visit(res);
        }


        public JNode VisitNamedExpression(NamedExpression node)
        {
            var name = new JJsonMember { Name = node.Name };
            if (name.Name.IsNullOrEmpty())
            {
                throw new NotImplementedException();
                //if (d.expression.e == cs_node.n_simple_name)
                //    name.Name = ((CsSimpleName)d.expression).identifier.identifier;
                //else if (d.expression.e == cs_node.n_primary_expression_member_access)
                //    name.Name = ((CsPrimaryExpressionMemberAccess)d.expression).identifier.identifier;
            }
            var value = VisitExpression(node.Expression);
            var ce = node.GetParentType();
            var nativeJson = JMeta.UseNativeJsons(ce.GetDefinitionOrArrayType(Compiler));

            if (!nativeJson)
            {
                name.Name = "get_" + name.Name;
                value = new JFunction { Block = new JBlock { Statements = new List<JStatement> { new JReturnStatement { Expression = value } } } };
            }
            return new JJsonNameValue { Name = name, Value = value };
        }

        public JNode VisitNullReferenceExpression(NullReferenceExpression node)
        {
            return J.Null();
        }

        public JNode VisitObjectCreateExpression(ObjectCreateExpression node)
        {
            var res = node.Resolve();
            return Visit(res);
        }

        public JNode VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitParenthesizedExpression(ParenthesizedExpression node)
        {
            var res = node.Resolve();
            var node2 = Visit(res);
            return node2;
        }


        public JNode VisitPrimitiveExpression(PrimitiveExpression node)
        {
            return Visit(node.Resolve());
        }


        public JNode VisitThisReferenceExpression(ThisReferenceExpression node)
        {
            return Visit(node.Resolve());
        }

        public JNode VisitTypeOfExpression(TypeOfExpression node)
        {
            return Visit(node.Resolve());
        }


        public JNode VisitUnaryOperatorExpression(UnaryOperatorExpression node)
        {
            return Visit(node.Resolve());
            //return new JsPreUnaryExpression { Operator = _Visit(node.Operator), Right = VisitExpression(node.Expression) };
        }

        public JNode VisitQueryExpression(QueryExpression node)
        {
            var res = node.Resolve();
            return Visit(res);
        }


        public JNode VisitBlockStatement(BlockStatement node)
        {
            CommentsExporter cmt = null;
            if (ExportComments)
                cmt = new CommentsExporter { Nodes = node.Children.ToList() };
            var statements = new List<JStatement>();
            foreach (var st in node.Statements)
            {
                var st2 = VisitStatement(st);
                if (cmt != null)
                    st2.Comments = cmt.ExportCommentsUptoNode(st);
                statements.Add(st2);
            }
            var block = new JBlock { Statements = new List<JStatement>() };
            if (cmt != null)
                block.Comments = cmt.ExportAllLeftoverComments();
            foreach (var st in statements)
            {
                if (st is JBlock)
                {
                    var block2 = (JBlock)st;
                    if (block2.Statements != null)
                        block.Statements.AddRange(block2.Statements);
                }
                else
                {
                    block.Statements.Add(st);
                }
            }
            return block;
        }


        public JNode VisitBreakStatement(BreakStatement node)
        {
            return new JBreakStatement();
        }


        public JNode VisitContinueStatement(ContinueStatement node)
        {
            return new JContinueStatement();
        }

        public JNode VisitDoWhileStatement(DoWhileStatement node)
        {
            var node2 = new JDoWhileStatement { Statement = VisitStatement(node.EmbeddedStatement), Condition = VisitExpression(node.Condition) };
            return node2;
        }

        public JNode VisitEmptyStatement(EmptyStatement node)
        {
            return new JStatement();
        }

        public JNode VisitExpressionStatement(ExpressionStatement node)
        {
            var exp2 = VisitExpression(node.Expression);
            if (exp2 == null)
                return new JStatement();
            if (exp2 is JMemberExpression)
            {
                var me = (JMemberExpression)exp2;
                if (me.Name.IsNullOrEmpty() && me.PreviousMember == null)
                    return new JStatement();
            }
            return new JExpressionStatement { Expression = exp2 };
        }


        public JNode VisitForeachStatement(ForeachStatement node)
        {
            var node2 = new JForInStatement
            {
                Initializer = J.Var(node.VariableName, node.VariableType.Resolve().Type.JAccess()),
                Member = VisitExpression(node.InExpression),
                Statement = VisitStatement(node.EmbeddedStatement)
            };
            return node2;
        }
        public JNode VisitForeachStatement_old(ForeachStatement node)
        {
            if (node.InExpression != null)
            {
                var expRes = node.InExpression.Resolve();
                var et = expRes.Type.GetDefinitionOrArrayType(Compiler);
                var iteratorType = expRes.Type;
                //var et = node.expression.entity_typeref.GetEntityType();
                if (et != null)
                {
                    var jta = JMeta.GetJsTypeAttribute(et);
                    if (jta != null && jta.NativeEnumerator)
                    {
                        var node2 = new JForInStatement
                        {
                            Initializer = J.Var(node.VariableName, node.Resolve().Type.JAccess()),
                            Member = VisitExpression(node.InExpression),
                            Statement = VisitStatement(node.EmbeddedStatement)
                        };
                        return node2;
                    }
                    else if (jta != null && jta.NativeArrayEnumerator)
                    {
                        VariableIteratorCounter++;
                        var iteratorName = "$i" + VariableIteratorCounter;
                        var lengthCacheName = "$l" + VariableIteratorCounter;
                        var exp2 = VisitExpression(node.InExpression);
                        var target = exp2;
                        var targetCacheName = "$t" + VariableIteratorCounter;
                        if (exp2 is JMemberExpression || ((JMemberExpression)exp2).PreviousMember != null)//is not simple name
                        {
                            target = J.Member(targetCacheName);

                        }
                        var itemAccess = target.IndexerAccess(J.Member(iteratorName));
                        var node2 = new JForStatement();

                        node2.Condition = J.Member(iteratorName).LessThan(J.Member(lengthCacheName));
                        node2.Iterators = new List<JStatement> { J.Member(iteratorName).PlusPlus().Statement(), J.Member(node.VariableName).Assign(itemAccess).Statement() };
                        if (target != exp2)//use target caching
                        {
                            node2.Initializers = new List<JStatement> { J.Var(iteratorName, iteratorType.JAccess(), J.Value(0)).AndVar(targetCacheName, exp2).AndVar(lengthCacheName, target.Member("length")).AndVar(node.VariableName, itemAccess).Statement() };
                        }
                        else
                        {
                            node2.Initializers = new List<JStatement> { J.Var(iteratorName, iteratorType.JAccess(), J.Value(0)).AndVar(lengthCacheName, exp2.Member("length")).AndVar(node.VariableName, itemAccess).Statement() };
                        }
                        node2.Statement = VisitStatement(node.EmbeddedStatement);
                        return node2;
                    }
                }
            }

            var iteratorName2 = "$it" + VariableIteratorCounter;
            VariableIteratorCounter++;
            var node3 = J.Var(iteratorName2, node.Resolve().Type.JAccess(), VisitExpression(node.InExpression).Member("GetEnumerator").Invoke()).Statement();
            var whileNode = J.While(J.Member(iteratorName2).Member("MoveNext").Invoke());
            var getCurrentStatement = J.Var(node.VariableName, node.Resolve().Type.JAccess(), J.Member(iteratorName2).Member("get_Current").Invoke()).Statement();
            var jsStatement = VisitStatement(node.EmbeddedStatement);
            JBlock block;
            if (jsStatement is JBlock)
                block = (JBlock)jsStatement;
            else
                block = J.Block().Add(jsStatement);
            block.Statements.Insert(0, getCurrentStatement);
            whileNode.Statement = block;

            var block2 = J.Block().Add(node3).Add(whileNode);
            return block2;
        }

        public JNode VisitForStatement(ForStatement node)
        {
            var node2 = new JForStatement
            {
                Condition = VisitExpression(node.Condition),
                Statement = VisitStatement(node.EmbeddedStatement),
            };
            
            if (node.Iterators != null)
                node2.Iterators = VisitStatements(node.Iterators);
            if (node.Initializers != null)
                node2.Initializers = VisitStatements(node.Initializers);
            return node2;
        }


        public JNode VisitIfElseStatement(IfElseStatement node)
        {
            return new JIfStatement { Condition = VisitExpression(node.Condition), IfStatement = VisitStatement(node.TrueStatement), ElseStatement = VisitStatement(node.FalseStatement) };
        }


        public JNode VisitReturnStatement(ReturnStatement node)
        {
            return J.Return(VisitExpression(node.Expression));
        }

        public JNode VisitSwitchStatement(SwitchStatement node)
        {
            return new JSwitchStatement
            {
                Expression = VisitExpression(node.Expression),
                Sections = node.SwitchSections.Select(t => (JSwitchSection)Visit(t)).ToList(),
            };
        }

        public JNode VisitSwitchSection(SwitchSection node)
        {
            return new JSwitchSection
            {
                Labels = node.CaseLabels.Select(t => (JSwitchLabel)Visit(t)).ToList(),
                Statements = node.Statements.Select(VisitStatement).ToList(),
            };
        }

        public JNode VisitCaseLabel(CaseLabel node)
        {
            var node2 = new JSwitchLabel
            {
                IsDefault = node.Expression.IsNull, //the alternative doesn't work: node.Role == CaseLabel.DefaultKeywordRole,
                Expression = VisitExpression(node.Expression),
            };
            return node2;
        }

        public JNode VisitThrowStatement(ThrowStatement node)
        {
            JExpression node2;
            IType exceptionType;
            if (node.Expression == null || node.Expression.IsNull) //happens when performing "throw;"
            {
                var cc = node.GetParent<CatchClause>();
                if (cc != null)
                {
                    node2 = J.Member(cc.VariableName);
                    var type = cc.Type;
                    if (type == null || type.IsNull)
                        exceptionType = Project.Compilation.FindType(KnownTypeCode.Exception);
                    else
                        exceptionType = cc.Type.Resolve().Type;
                }
                else
                    throw new Exception("Rethrow not supported, catch clause not found");
            }
            else
            {
                node2 = VisitExpression(node.Expression);
                exceptionType = node.Expression.Resolve().Type;
            }
            //if (!Sk.IsNativeError(exceptionType.GetDefinitionOrArrayType()))
            //{
            //    throw new NotSupportedException();
            //    //node2 = J.Member("$CreateException").Invoke(node2, J.New(J.Member("Error")));
            //}
            return new JThrowStatement { Expression = node2 };
        }

        public JNode VisitTryCatchStatement(TryCatchStatement node)
        {
            var node2 = new JTryStatement { TryBlock = (JBlock)Visit(node.TryBlock) };
            if (node.CatchClauses != null && node.CatchClauses.Count > 0)
            {
                if (node.CatchClauses.Count > 1)
                    throw new CompilerException(node, "Client code may not have more than one catch clause, due to JavaScript limitation");
                node2.CatchClause = (JCatchClause)Visit(node.CatchClauses.First());
            }
            if (node.FinallyBlock != null)
                node2.FinallyBlock = (JBlock)Visit(node.FinallyBlock);
            return node2;
        }

        public JNode VisitCatchClause(CatchClause node)
        {
            var node2 = new JCatchClause();

            if (node.VariableName.IsNullOrEmpty())
                node.VariableName = "$$e" + (VariableExceptionCounter++); //Generate a psuedo-unique variable name
            node2.IdentifierName = node.VariableName;
            node2.Type = node.Type.Resolve().Type.JAccess();
            node2.Block = (JBlock)Visit(node.Body);
            if (node2.Block != null)
            {
                node2.Descendants<JThrowStatement>().Where(t => t.Expression == null).ForEach(t => t.Expression = J.Member(node2.IdentifierName));
            }
            return node2;
        }


        public JNode VisitUsingStatement(UsingStatement node)
        {
            var st3 = Visit(node.ResourceAcquisition);
            JVariableDeclarationStatement stVar;
            if (st3 is JExpression)
            {
                stVar = J.Var("$r" + VariableResourceCounter++, node.Resolve().Type.JAccess(), (JExpression)st3).Statement();
            }
            else
            {
                stVar = (JVariableDeclarationStatement)st3;
            }
            var trySt = VisitStatement(node.EmbeddedStatement);
            var st2 = new JTryStatement { TryBlock = trySt.ToBlock(), FinallyBlock = J.Block() };

            //var resource = node.ResourceAcquisition;
            //var decl = resource as VariableDeclarationStatement;
            //if (decl == null || decl.Variables.Count == 0)
            //    throw new Exception("using statement is supported only with the var keyword in javascript. Example: using(var g = new MyDisposable()){}");
            foreach (var dr in stVar.Declaration.Declarators)
            {
                st2.FinallyBlock.Add(J.Member(dr.Name).Member("Dispose").Invoke().Statement());
            }
            return J.Block().Add(stVar).Add(st2); //TODO: get rid of block
        }

        public JNode VisitVariableDeclarationStatement(VariableDeclarationStatement node)
        {
            var vars = node.Variables.Select(Visit).Cast<JVariableDeclarator>().ToList();
            var y = node.Resolve();
            var x = node.Type.Resolve();
            return new JVariableDeclarationStatement { Declaration = new JVariableDeclarationExpression { Declarators = vars, Type = x.Type.JAccess() } };
        }

        public JNode VisitWhileStatement(WhileStatement node)
        {
            return new JWhileStatement { Condition = VisitExpression(node.Condition), Statement = (JStatement)Visit(node.EmbeddedStatement) };
        }

        public JNode VisitYieldBreakStatement(YieldBreakStatement node)
        {
            if (SupportClrYield)
            {
                return new JsYieldBreakStatement();
            }

            var me = node.GetCurrentMethod();
            var node2 = GenerateYieldReturnStatement(me);

            return node2;
        }

        public static JReturnStatement GenerateYieldReturnStatement(IMethod me)
        {
            JReturnStatement node2;
            if (me != null && me.ReturnType != null && me.ReturnType.FullName.StartsWith("System.Collections.Generic.IEnumerator"))
                node2 = J.Return(J.Member("$yield").Member("GetEnumerator").Invoke());
            else
                node2 = J.Return(J.Member("$yield"));
            return node2;
        }


        public JNode VisitYieldReturnStatement(YieldReturnStatement node)
        {
            var exp2 = VisitExpression(node.Expression);
            if (SupportClrYield)
            {
                return new JsYieldReturnStatement { Expression = exp2 };
            }
            var node2 = J.Member("$yield.add").Invoke(exp2).Statement();
            return node2;
        }


        public JNode VisitVariableInitializer(VariableInitializer node)
        {
            return new JVariableDeclarator { Name = node.Name, Initializer = VisitExpression(node.Initializer) };
        }

        #endregion

        #region NotImplemented
        public JNode VisitUncheckedStatement(UncheckedStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitUnsafeStatement(UnsafeStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitFixedFieldDeclaration(FixedFieldDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitFixedVariableInitializer(FixedVariableInitializer node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitSyntaxTree(SyntaxTree node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitSimpleType(SimpleType node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitMemberType(MemberType node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitComposedType(ComposedType node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitArraySpecifier(ArraySpecifier node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitPrimitiveType(PrimitiveType node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitComment(Comment node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitPreProcessorDirective(PreProcessorDirective node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitDocumentationReference(DocumentationReference node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitTypeParameterDeclaration(TypeParameterDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitConstraint(Constraint node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitCSharpTokenNode(CSharpTokenNode node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitIdentifier(Identifier node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitPatternPlaceholder(AstNode placeholder, ICSharpCode.NRefactory.PatternMatching.Pattern node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitWhitespace(WhitespaceNode node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitText(TextNode node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitNewLine(NewLineNode node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitUndocumentedExpression(UndocumentedExpression node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitAccessor(Accessor node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitConstructorDeclaration(ConstructorDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitConstructorInitializer(ConstructorInitializer node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitDestructorDeclaration(DestructorDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitEnumMemberDeclaration(EnumMemberDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitEventDeclaration(EventDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitCustomEventDeclaration(CustomEventDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitFieldDeclaration(FieldDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitIndexerDeclaration(IndexerDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitMethodDeclaration(MethodDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitOperatorDeclaration(OperatorDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitParameterDeclaration(ParameterDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitPropertyDeclaration(PropertyDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitBaseReferenceExpression(BaseReferenceExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitCheckedExpression(CheckedExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitDirectionExpression(DirectionExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitNamedArgumentExpression(NamedArgumentExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitUncheckedExpression(UncheckedExpression node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitArrayInitializerExpression(ArrayInitializerExpression node)
        {
            return Visit(node.Resolve());
        }
        public JNode VisitPointerReferenceExpression(PointerReferenceExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitSizeOfExpression(SizeOfExpression node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitStackAllocExpression(StackAllocExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitTypeReferenceExpression(TypeReferenceExpression node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitQueryContinuationClause(QueryContinuationClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryFromClause(QueryFromClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryLetClause(QueryLetClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryWhereClause(QueryWhereClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryJoinClause(QueryJoinClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryOrderClause(QueryOrderClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryOrdering(QueryOrdering node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQuerySelectClause(QuerySelectClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitQueryGroupClause(QueryGroupClause node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitAttribute(ICSharpCode.NRefactory.CSharp.Attribute node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitAttributeSection(AttributeSection node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitDelegateDeclaration(DelegateDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitNamespaceDeclaration(NamespaceDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitTypeDeclaration(TypeDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitUsingAliasDeclaration(UsingAliasDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitUsingDeclaration(UsingDeclaration node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitExternAliasDeclaration(ExternAliasDeclaration node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitCheckedStatement(CheckedStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitFixedStatement(FixedStatement node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitGotoCaseStatement(GotoCaseStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitGotoDefaultStatement(GotoDefaultStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitGotoStatement(GotoStatement node)
        {
            throw new NotImplementedException();
        }
        public JNode VisitLabelStatement(LabelStatement node)
        {
            throw new NotImplementedException();
        }

        public JNode VisitLockStatement(LockStatement node)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
