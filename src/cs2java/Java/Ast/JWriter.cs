using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;
using JSharp.Compiler;
using SystemTools.Text;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Java.Ast
{
    class JWriter : CodeWriter, IJNodeVisitor, IDisposable
    {
        #region Visitor

        public void VisitImport(JImport node)
        {
            InnerWriter.Write("import ");
            Visit(node.Type);
            WriteSemicolon();
            WriteLine();
        }
        public void VisitCompilationUnit(JCompilationUnit node)
        {
            WriteLine("package " + node.PackageName + ";");
            WriteLine();
            VisitEach(node.Imports);
            WriteLine();
            VisitEach(node.Declarations);
        }
        public void VisitFieldDeclaration(JFieldDeclaration node)
        {
            var fe = node.FieldDefinition;
            WriteModifiers(node);
            Visit(node.Type);
            Write(" ");
            Write(node.Name);
            if (node.Initializer != null)
            {
                Write("=");
                Visit(node.Initializer);
            }
            WriteSemicolon();
            WriteLine();
        }
        bool IsInReturnTypeParametersDeclaration;
        public void VisitClassDeclaration(JClassDeclaration node)
        {
            var ce = node.TypeDefinition;
            WriteModifiers(node);
            if (node.IsInterface)
                Write("interface");
            else
                Write("class");
            Write(" " + node.Name);
            if (node.TypeParameters.IsNotNullOrEmpty())
            {
                if (IsInReturnTypeParametersDeclaration)
                    throw new Exception("IsInClassDeclarationTypeParameters");
                IsInReturnTypeParametersDeclaration = true;
                Write("<");
                VisitEachJoin(node.TypeParameters, WriteComma);
                Write(">");
                IsInReturnTypeParametersDeclaration = false;
            }
            if (node.Extends.IsNotNullOrEmpty())
            {
                Write(" extends ");
                node.Extends.ForEachJoin(Visit, WriteComma);
            }
            if (node.Implements.IsNotNullOrEmpty())
            {
                Write(" implements ");
                node.Implements.ForEachJoin(Visit, WriteComma);
            }
            WriteLine();
            BeginBlock();
            VisitEach(node.Declarations);
            EndBlock();
        }
        public void Visit(IType type)
        {
            var node = JNaming.JAccess(type);
            Visit(node);
        }
        public void Visit(IMember member)
        {
            Visit(JNaming.JAccess(member));
        }
        public void VisitMethodDeclaration(JMethodDeclaration node)
        {
            VisitEach(node.Annotations);

            var me = node.MethodDefinition;
            if (node.CustomHeaderCode != null)
            {
                Write(node.CustomHeaderCode);
            }
            else
            {
                WriteModifiers(node);
                if (node.TypeParameters.IsNotNullOrEmpty())
                {
                    if (IsInReturnTypeParametersDeclaration)
                        throw new Exception("IsInReturnTypeParametersDeclaration");
                    IsInReturnTypeParametersDeclaration = true;
                    Write("<");
                    VisitEachJoin(node.TypeParameters, WriteComma);
                    Write(">");
                    Write(" ");
                    IsInReturnTypeParametersDeclaration = false;
                }
                if (node.Type != null)
                {
                    Visit(node.Type);
                    Write(" ");
                }
                var name = node.Name;
                if (name == null)
                {
                    if (me.IsConstructor)
                        name = me.DeclaringTypeDefinition.Name;
                    else
                        name = me.Name;
                }
                Write(name);
                Write("(");
                VisitEachJoin(node.Parameters, WriteComma);
                Write(")");
                if (node.MethodBody == null)
                    Write(";");
                WriteLine();
            }
            Visit(node.MethodBody);
        }


        public void VisitAnnotationDeclaration(JAnnotationDeclaration node)
        {
            Write("@" + node.Name);
            if (node.Parameters.IsNotNullOrEmpty() || node.NamedParameters.IsNotNullOrEmpty())
            {
                Write("(");
                VisitEach(node.Parameters);
                VisitEach(node.NamedParameters);
                Write(")");
            }
            WriteLine();
        }

        public void VisitAnnotationNamedParameter(JAnnotationNamedParameter node)
        {
            throw new NotImplementedException();
        }


        private void WriteModifiers(JEntityDeclaration node2)
        {
            var node = node2.Modifiers;
            if (node.IsPublic)
                Write("public ");
            if (node.IsPrivate)
                Write("private ");
            if (node.IsProtected)
                Write("protected ");
            if (node.IsStatic)
                Write("static ");
            if (node.IsAbstract)
            {
                var ce = node2 as JClassDeclaration;
                var me = node2 as JMethodDeclaration;
                if (ce != null && ce.IsInterface)
                {
                }
                else if (me != null && me.MethodDefinition != null && me.MethodDefinition.DeclaringTypeDefinition.IsInterface())
                {
                }
                else
                {
                    Write("abstract ");
                }
            }
        }

        private void WriteModifiers(IMember me)
        {
            if (me.IsPublic)
                Write("public ");
            if (me.IsPrivate)
                Write("private ");
            if (me.IsProtected)
                Write("protected ");
            if (me.IsStatic)
                Write("static ");
        }
        public void VisitNewAnonymousClassExpression(JNewAnonymousClassExpression node)
        {
            VisitNewObjectExpression(node);
            BeginBlock();
            VisitEach(node.Declarations);
            EndBlock();
        }
        public void VisitAssignmentExpression(JAssignmentExpression node)
        {
            Visit(node.Left);
            Write(node.Operator ?? "=");
            Visit(node.Right);
        }
        public void VisitBinaryExpression(JBinaryExpression node)
        {
            Visit(node.Left);
            Write(" ");
            Write(node.Operator);
            Write(" ");
            Visit(node.Right);
        }
        public void VisitBlock(JBlock node)
        {
            BeginBlock();
            VisitEach(node.Statements);
            if (node.Comments.IsNotNullOrEmpty())
                WriteComments(node.Comments);
            var parent = node.Parent;
            EndBlock(parent != null && parent is JFunction);
        }
        public void VisitsBreakStatement(JBreakStatement node)
        {
            Write("break");
            WriteSemicolon();
            WriteLine();
        }
        public void VisitCatchClause(JCatchClause node)
        {
            Write("catch");
            Write("(");
            Visit(node.Type);
            Write(" ");
            Write(node.IdentifierName);
            Write(")");
            if (OpenBraceInNewLine)
                WriteLine();
            Visit(node.Block);
        }
        public void VisitCodeExpression(JCodeExpression node)
        {
            if (node.WriteOverride != null)
                node.WriteOverride(this);
            else
                Write(node.Code);
        }
        public void VisitCommentStatement(JCommentStatement node)
        {
            Write(string.Format("/*{0}*/", node.Text));
            WriteLine();
        }
        public void VisitConditionalExpression(JConditionalExpression node)
        {
            Visit(node.Condition);
            Write("?");
            Visit(node.TrueExpression);
            Write(":");
            Visit(node.FalseExpression);
        }
        public void VisitContinueStatement(JContinueStatement node)
        {
            Write("continue");
            WriteSemicolon();
            WriteLine();
        }
        public void VisitDoWhileStatement(JDoWhileStatement node)
        {
            Write("do");
            Visit(node.Statement);
            Write("while");
            Write("(");
            Visit(node.Condition);
            Write(")");
            WriteLine();
        }
        public void VisitExpression(JExpression node)
        {
        }
        public void VisitExpressionStatement(JExpressionStatement node)
        {
            Visit(node.Expression);
            if (SkipSemicolon(node))
                WriteLine();
            else
            {
                WriteSemicolon();
                WriteLine();
            }
        }
        public void VisitsExternalFileUnit(JsExternalFileUnit node)
        {
            VisitUnit(node);
            using (var reader = new StreamReader(node.Filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    WriteLine(line);
                }

            }
        }
        public void VisitForInStatement(JForInStatement node)
        {
            Write("for");
            Write("(");
            Visit(node.Initializer);
            Write(" : ");
            Visit(node.Member);
            Write(")");
            if (OpenBraceInNewLine)
                WriteLine();
            IndentVisitIfNeeded(node.Statement);
        }
        public void VisitsForStatement(JForStatement node)
        {
            Write("for");
            Write("(");
            if (node.Initializers != null)
            {
                VisitEachJoin(ExtractExpressions(node.Initializers), WriteComma);
            }
            Write(";");
            if (node.Condition != null)
                Visit(node.Condition);
            Write(";");
            if (node.Iterators != null)
            {
                var exps = ExtractExpressions(node.Iterators);
                VisitEachJoin(exps, WriteComma);
            }
            Write(")");
            if (OpenBraceInNewLine) InnerWriter.WriteLine();
            IndentVisitIfNeeded(node.Statement);
        }
        public void VisitFunction(JFunction node)
        {
            Write("function");
            if (node.Name.IsNotNullOrEmpty())
                Write(node.Name);
            Write("(");
            //WriteValue(node.Parameters.NotNull().StringConcat(", ")); //TODO:SL

            bool first = true;
            foreach (var param in node.Parameters)
            {
                if (first) first = false;
                else WriteComma();

                Write(param);
            }

            Write(")");
            if (OpenBraceInNewLine)
                WriteLine();
            if (node.Block != null)
            {
                Visit(node.Block);
            }
        }
        public void VisitIfStatement(JIfStatement node)
        {
            Write("if");
            Write("(");
            Visit(node.Condition);
            Write(")");
            if (OpenBraceInNewLine) WriteLine();
            IndentVisitIfNeeded(node.IfStatement);
            if (node.ElseStatement != null)
            {
                Write("else ");
                if (node.ElseStatement is JIfStatement)
                {
                    Visit(node.ElseStatement);
                }
                else
                {
                    if (OpenBraceInNewLine)
                        WriteLine();
                    IndentVisitIfNeeded(node.ElseStatement);
                }
            }
        }
        public void VisitIndexerAccessExpression(JIndexerAccessExpression node)
        {
            Visit(node.Member);
            Write("[");
            VisitEachJoin(node.Arguments, WriteComma);
            Write("]");
        }
        public void VisitInvocationExpression(JInvocationExpression node)
        {
            Visit(node.Member);
            if (!node.OmitParanthesis)
                Write("(");
            else if (node.Arguments.IsNotNullOrEmpty() && node.ArgumentsPrefix == null)
                Write(" ");
            Write(node.ArgumentsPrefix);
            if (node.Arguments != null)
            {
                if (node.OmitCommas)
                    VisitEach(node.Arguments);
                else
                    VisitEachJoin(node.Arguments, WriteComma);
            }
            Write(node.ArgumentsSuffix);
            if (!node.OmitParanthesis)
                Write(")");
        }
        public void VisitNewArrayExpression(JNewArrayExpression node)
        {
            Write("new ");
            Visit(node.Type);
            //Write("[");
            if (node.Size != null && node.Items == null)
                throw new Exception();
            //Write("]");
            if (node.Items != null)
            {
                Write("{");
                VisitEachJoin(node.Items, WriteComma);
                Write("}");
            }
        }
        public void VisitsJsonMember(JJsonMember node)
        {
            if (node.IsStringLiteral)
                Write(string.Format("\"{0}\"", node.Name));
            else
                Write(node.Name);
        }
        public void VisitsJsonNameValue(JJsonNameValue node)
        {
            Visit(node.Name);
            Write(":");
            Visit(node.Value);
        }
        public void VisitsJsonObjectExpression(JJsonObjectExpression node)
        {
            if (node.NamesValues.IsNullOrEmpty())
            {
                Write("{}");
            }
            else
            {
                var singleLine = node.NamesValues.Where(t => t.Value.IsAny(typeof(JFunction), typeof(JJsonObjectExpression))).FirstOrDefault() == null;
                if (!singleLine && OpenBraceInNewLine)
                    WriteLine();
                BeginBlock(singleLine);
                VisitEachJoin(node.NamesValues, (first) =>
                {
                    if (EmptyLineBetweenJsonMethods && !singleLine && first.Value != null && first.Value is JFunction)
                        WriteLine();
                }, (prev, next) =>
                {
                    if (singleLine)
                        WriteComma();
                    else
                    {
                        WriteComma();
                        WriteLine();
                        if (EmptyLineBetweenJsonMethods && ((prev.Value != null && prev.Value is JFunction) || (next.Value != null && next.Value is JFunction)))
                            WriteLine();
                    }
                }, (last) =>
                {
                    if (EmptyLineBetweenJsonMethods && !singleLine && last.Value != null && last.Value is JFunction)
                        WriteLine();
                });
                if (!singleLine)
                    WriteLine();
                EndBlock(true);
            }
        }

        bool IsInTypeParameter;
        public void VisitMemberExpression(JMemberExpression node)
        {
            if (node.PreviousMember != null)
            {
                Visit(node.PreviousMember);
                Write(".");
            }
            if (node.Name != null)
            {
                Write(node.Name);
                if (node.GenericArguments.IsNotNullOrEmpty())
                {
                    Write("<");
                    VisitEachJoin(node.GenericArguments, () => Write(","));
                    Write(">");
                }
            }
            else if (node.Member != null)
                Visit(node.Member);

            if (node.TypeRef is ITypeParameter && !IsInTypeParameter && IsInReturnTypeParametersDeclaration)
            {
                IsInTypeParameter = true;
                var tr = (ITypeParameter)node.TypeRef;
                if (tr.EffectiveInterfaceSet.IsNotNullOrEmpty())
                {
                    Write(" extends ");
                    VisitEachJoin(tr.EffectiveInterfaceSet.Select(t => t.JAccess()).ToList(), WriteComma);
                }
                IsInTypeParameter = false;
            }

            if (node.IsArray)
                Write("[]");
        }

        public void VisitNewObjectExpression(JNewObjectExpression node)
        {
            Write("new ");
            Visit(node.Invocation);
        }
        public void VisitsNodeList(JNodeList node)
        {
            VisitEachJoin(node.Nodes, WriteComma);
        }
        public void VisitNullExpression(JNullExpression node)
        {
            Write("null");
        }
        public void VisitsNumberExpression(JNumberExpression node)
        {
            Write(node.Value.ToString());
        }
        public void VisitParenthesizedExpression(JParenthesizedExpression node)
        {
            Write("(");
            Visit(node.Expression);
            Write(")");
        }
        public void VisitPostUnaryExpression(JPostUnaryExpression node)
        {
            Visit(node.Left);
            Write(node.Operator);
        }
        public void VisitPreUnaryExpression(JPreUnaryExpression node)
        {
            Write(node.Operator);
            Visit(node.Right);
        }
        public void VisitsRegexExpression(JRegexExpression node)
        {
            Write(node.Code);
        }
        public void VisitReturnStatement(JReturnStatement node)
        {
            Write("return ");
            if (node.Expression != null)
            {
                Visit(node.Expression);
            }
            WriteSemicolon();
            WriteLine();
        }
        public void VisitStatement(JStatement node)
        {
            var parent = node.Parent;
            if (parent != null && parent is JBlock)
                return;
            WriteSemicolon();
            WriteLine();
        }
        public void VisitsStatementExpressionList(JStatementExpressionList node)
        {
            VisitEachJoin(node.Expressions, WriteComma);
        }
        public void VisitStringExpression(JStringExpression node)
        {
            //Write("\"");
            //Write(node.Value);
            //Write("\"");
            Write("\"" + node.Value.ToString() + "\"");
        }
        public void VisitSwitchLabel(JSwitchLabel node)
        {
            if (node.IsDefault)
            {
                Write("default");
            }
            else
            {
                Write("case");
                Visit(node.Expression);
            }
            Write(":");
            WriteLine();
        }
        public void VisitSwitchSection(JSwitchSection node)
        {
            VisitEach(node.Labels);
            InnerWriter.Indent++;
            VisitEach(node.Statements);
            Indent--;
        }
        public void VisitSwitchStatement(JSwitchStatement node)
        {
            Write("switch");
            Write("(");
            Visit(node.Expression);
            Write(")");
            if (OpenBraceInNewLine) WriteLine();
            BeginBlock();
            VisitEach(node.Sections);
            EndBlock();
        }
        public void VisitThis(JThis node)
        {
            Write("this");
        }
        public void VisitThrowStatement(JThrowStatement node)
        {
            SupressLineBreak(() =>
            {
                Write("throw ");
                Visit(node.Expression);
                WriteSemicolon();
            });
            WriteLine();
        }
        public void VisitTryStatement(JTryStatement node)
        {
            Write("try");
            if (OpenBraceInNewLine) WriteLine();

            Visit(node.TryBlock);
            if (node.CatchClause != null)
                Visit(node.CatchClause);
            if (node.FinallyBlock != null)
            {
                Write("finally");
                if (OpenBraceInNewLine) WriteLine();

                Visit(node.FinallyBlock);
            }
        }
        public void VisitsUnit(JUnit node)
        {
            if (node.Statements == null)
                return;
            VisitEach(node.Statements);
        }
        public void VisitVariableDeclarationExpression(JVariableDeclarationExpression node)
        {
            //Write("var");
            Visit(node.Type);
            Write(" ");
            VisitEachJoin(node.Declarators, WriteComma);
        }
        public void VisitVariableDeclarationStatement(JVariableDeclarationStatement node)
        {
            Visit(node.Declaration);
            WriteSemicolon();
            WriteLine();
        }

        public void VisitVariableDeclarator(JVariableDeclarator node)
        {
            Write(node.Name);
            if (node.Initializer != null)
            {
                Write("=");
                Visit(node.Initializer);
            }
        }
        public void VisitWhileStatement(JWhileStatement node)
        {
            Write("while");
            Write("(");
            Visit(node.Condition);
            Write(")");
            if (OpenBraceInNewLine) WriteLine();
            Visit(node.Statement);
        }
        #endregion

        #region IJNodeVisitor Members

        public void VisitNode(JNode node)
        {
            throw new NotImplementedException();
        }

        public void VisitEntityDeclaration(JEntityDeclaration node)
        {
            throw new NotImplementedException();
        }

        public void VisitParameterDeclaration(JParameterDeclaration node)
        {
            var prm = node.Parameter;
            if (prm != null && prm.Attributes.FirstOrDefault(t => t.AttributeType.Name == "FinalAttribute") != null)
                Write("final ");
            Visit(node.Type);
            Write(" ");
            Write(node.Name);
        }

        public void VisitForStatement(JForStatement node)
        {
            throw new NotImplementedException();
        }

        public void VisitBreakStatement(JBreakStatement node)
        {
            WriteLine("break;");
        }

        public void VisitStatementExpressionList(JStatementExpressionList node)
        {
            throw new NotImplementedException();
        }


        public void VisitNumberExpression(JNumberExpression node)
        {
            throw new NotImplementedException();
        }

        public void VisitRegexExpression(JRegexExpression node)
        {
            throw new NotImplementedException();
        }

        public void VisitNodeList(JNodeList node)
        {
            throw new NotImplementedException();
        }

        public void VisitUnit(JUnit node)
        {
            throw new NotImplementedException();
        }

        public void VisitJsonMember(JJsonMember node)
        {
            throw new NotImplementedException();
        }

        public void VisitJsonNameValue(JJsonNameValue node)
        {
            throw new NotImplementedException();
        }

        public void VisitJsonObjectExpression(JJsonObjectExpression node)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Visit
        public Action<JNode> Visiting { get; set; }
        [DebuggerStepThrough]
        public void Visit(JNode node)
        {
            if (node == null)
                return;
            if (Visiting != null)
                Visiting(node);
            if (node.StartLocation.IsEmpty)
                node.StartLocation = new TextLocation(CurrentLine, CurrentColumn);
            var st = node as JStatement;
            if (st != null && st.Comments.IsNotNullOrEmpty() && !(st is JBlock))
                WriteComments(st.Comments);

            node.AcceptVisitor(this);
            if (node.EndLocation.IsEmpty)
                node.EndLocation = new TextLocation(CurrentLine, InnerWriter.CurrentColumn);

        }
        [DebuggerStepThrough]
        public void VisitEach<T>(IList<T> list) where T : JNode
        {
            if (list.IsNullOrEmpty())
                return;
            list.ForEach(Visit);
        }

        [DebuggerStepThrough]
        public void VisitEachJoin<T>(IList<T> list, Action actionBetweenItems) where T : JNode
        {
            if (list.IsNullOrEmpty())
                return;
            list.ForEachJoin(Visit, actionBetweenItems);
        }

        [DebuggerStepThrough]
        public void VisitEachJoin<T>(IList<T> list, Action<T> first, Action<T, T> actionBetweenItems, Action<T> last) where T : JNode
        {
            if (list.IsNullOrEmpty())
                return;
            list.ForEachJoin(Visit, first, actionBetweenItems, last);
        }

        #endregion


        #region Utils
        private static List<JExpression> ExtractExpressions(IList<JStatement> statements)
        {
            var list = new List<JExpression>();
            foreach (var st in statements)
            {
                if (st is JExpressionStatement)
                    list.Add(((JExpressionStatement)st).Expression);
                else if (st is JVariableDeclarationStatement)
                    list.Add(((JVariableDeclarationStatement)st).Declaration);
                else
                    throw new Exception("Error extracting expressions from statements");
            }
            return list;
        }
        public bool OpenBraceInNewLine
        {
            get
            {
                return true; //false is JS very general standard! BUT: Make configurable with skc5.config or JsExportAttribute!
            }
        }


        private bool EmptyLineBetweenJsonMethods
        {
            get
            {
                return false;
            }
        }


        public static JWriter CreateInMemory()
        {
            var innerWriter = new StringWriter();
            var writer = new JWriter { InnerWriter = new LineWriter(innerWriter) };
            return writer;
        }
        public static JWriter Create(string filename, bool append)
        {
            var innerWriter = new StreamWriter(filename, append);
            var writer = new JWriter { InnerWriter = new LineWriter(innerWriter) };
            return writer;
        }


        private void WriteComments(IList<string> list)
        {
            foreach (var cmt in list)
            {
                if (cmt == null)
                    continue;
                if (cmt.Contains('\n'))
                {
                    var reader = new StringReader(cmt);
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (line == null)
                            break;
                        line = line.TrimStart();
                        Write(line);
                        WriteLine();
                    }
                }
                else
                {
                    Write(cmt);
                    WriteLine();
                }
            }
        }

        bool SkipSemicolon(JExpressionStatement node)
        {
            //if (node.Expression.NodeType == JsNodeType.Function)
            //    return true;
            //if (node.Expression.NodeType == JsNodeType.AssignmentExpression && ((JsAssignmentExpression)node.Expression).Right.NodeType == JsNodeType.Function)//TODO: check parent is unit
            //    return true;
            if (node.Expression != null && (node.Expression.IsAny(typeof(JCodeExpression))))
                return true;
            return false;
        }
        public void Dispose()
        {
            if (InnerWriter != null)
                InnerWriter.Dispose();
        }


        #endregion

        #region Writer


        public StringBuilder GetStringBuilder()
        {
            var sw = InnerWriter.InnerWriter as StringWriter;
            if (sw != null)
            {
                InnerWriter.Flush();
                return sw.GetStringBuilder();
            }
            return null;
        }


        [DebuggerStepThrough]
        public void IndentVisitIfNeeded(JNode node)
        {
            if (!(node is JBlock))
                IndentVisit(node);
            else
                Visit(node);
        }

        [DebuggerStepThrough]
        public void IndentVisit(JNode node)
        {
            Indented(() => Visit(node));
        }

        private StringBuilder sbSupressLineBreak;
        public void SupressLineBreak(Action action)
        {
            //Debugger.Break();
            if (sbSupressLineBreak != null)
            {
                action();
                return;
            }

            sbSupressLineBreak = new StringBuilder();
            action();
            InnerWriter.Write(sbSupressLineBreak.ToString());
            sbSupressLineBreak = null;
        }



        #endregion





        #region IJNodeVisitor Members


        public void VisitMultiStatementExpression(JMultiStatementExpression node)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IJNodeVisitor Members


        public void VisitCastExpression(JCastExpression node)
        {
            Write("(");
            Visit(node.Type);
            Write(")");
            Visit(node.Expression);
        }

        #endregion
    }



    static class JsNodeExtensions
    {
        //public static void VerifyParent(this JsNode node, JsNode parent)
        //{
        //    if (node.Parent == null)
        //        node.Parent = parent;
        //}
        public static bool IsAny(this JNode node, params Type[] list)
        {
            return list.Any(t => t.IsInstanceOfType(t));
        }
    }
}
