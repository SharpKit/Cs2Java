using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using JSharp.Compiler;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using JSharp.Java;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    partial class JCodeImporter : ICSharpResolveResultVisitor<JNode>
    {
        Dictionary<InitializedObjectResolveResult, string> Initializers = new Dictionary<InitializedObjectResolveResult, string>();

        #region Resolve Visitor

        public JNode VisitInitializedObjectResolveResult(InitializedObjectResolveResult res)
        {
            var varName = Initializers.TryGetValue(res);
            if (varName == null)
            {
                varName = "$v" + VariableInitializerCounter++;
                Initializers[res] = varName;
            }
            return J.Member(varName);
        }

        public JNode VisitTypeOfResolveResult(TypeOfResolveResult res)
        {
            return J.Member("Typeof").Invoke(JNaming.JAccess(res.ReferencedType));
        }

        public JNode VisitArrayAccessResolveResult(ArrayAccessResolveResult res)
        {
            var node2 = new JIndexerAccessExpression { Member = VisitExpression(res.Array), Arguments = VisitExpressions(res.Indexes) };
            return node2;
        }

        static string GetJsArrayType(IType arrayType)
        {
            var elementType = ((ArrayType)arrayType).ElementType;
            switch (elementType.Name)
            {
                case "Byte": return "Int8Array";
                //case "Uint8Array": return "Uint8Array";
                case "Int16": return "Int16Array";
                case "UInt16": return "Uint16Array";
                case "Int32": return "Int32Array";
                case "UInt32": return "Uint32Array";
                case "Single": return "Float32Array";
                case "Double": return "Float64Array";
            }
            return "Array";
        }
        public JNode VisitArrayCreateResolveResult(ArrayCreateResolveResult res)
        {
            JExpression[] items = null;
            JExpression size = null;
            if (res.InitializerElements.IsNotNullOrEmpty())
                items = VisitExpressions(res.InitializerElements).ToArray();
            else if (res.SizeArguments.IsNotNullOrEmpty())
                size = VisitExpression(res.SizeArguments.Single());

            return J.NewArray(res.Type, size, items);
        }

        public JNode VisitTypeIsResolveResult(TypeIsResolveResult res)
        {
            var typeDef = res.TargetType.GetDefinitionOrArrayType(Compiler);
            //if (Sk.OmitCasts(typeDef))
            //{
            //    return J.True();
            //    //var node2 = Visit(res.Input);
            //    //return node2;
            //}
            //else
            //{
            var node2 = VisitExpression(res.Input).InstanceOf(JNaming.JAccess(res.TargetType));
            return node2;
            //}
        }

        public JNode VisitMethodGroupResolveResult(MethodGroupResolveResult res)
        {
            var info = res.GetInfo();
            IMethod me;
            if (info != null && info.Conversion != null && info.Conversion.Method != null)
            {
                me = info.Conversion.Method;
            }
            else //happens when invoking a method with overloads, and a parameter is dynamic
            {
                var list = res.Methods.ToList();
                if (list.Count == 0)
                    throw new Exception("Method group not resolved to any method");
                else if (list.Count == 1)
                    me = list[0];
                else
                    me = list[0];
                //TODO: verify all methods has the same js name
            }
            var isExtensionMethodStyle = me.IsExtensionMethod && !(res.TargetResult is TypeResolveResult);//TODO: IsExtensionMethodStyle(new CsInvocationExpression { entity = me, expression = node });
            JExpression firstPrm = null;
            if (isExtensionMethodStyle)
            {
                firstPrm = (JExpression)Visit(res.TargetResult);
            }
            var node2 = JNaming.JAccess(me);
            JExpression node3;
            JExpression instanceContext = null;
            if (me.IsStatic || res.TargetResult == null) //getting ThisResolveResult even on static methods, getting TargetResult=null when MethodGroupResolveResult when using delegates
            {
                node3 = node2;
            }
            else
            {
                instanceContext = VisitExpression(res.TargetResult);
                node3 = instanceContext.Member(node2);
            }
            if (info != null && (instanceContext != null || firstPrm != null))
            {
                var conv = info.Conversion;
                if (info.ConversionTargetType != null && !UseNativeFunctions(info.ConversionTargetType))//delegate type
                {
                    var parentMethod = info.Nodes.FirstOrDefault().GetCurrentMethod();
                    if (parentMethod == null || !JMeta.ForceDelegatesAsNativeFunctions(parentMethod))
                    {
                        if (parentMethod == null)
                            Log.Warn(info.Nodes.FirstOrDefault(), "GetParentMethod() returned null");
                        var func = (JExpression)node2;
                        if (instanceContext != null)
                            node3 = CreateJsDelegate(instanceContext, func);
                        else if (firstPrm != null)
                            node3 = CreateJsExtensionDelegate(firstPrm, func);
                    }
                }
            }
            return node3;


        }

        public JNode VisitOperatorResolveResult(OperatorResolveResult res)
        {
            if (res.Operands.Count == 1)
            {
                if (res.UserDefinedOperatorMethod != null && !JMeta.UseNativeOperatorOverloads(res.UserDefinedOperatorMethod.DeclaringTypeDefinition))
                {
                    var fake = Cs.InvokeMethod(res.UserDefinedOperatorMethod, null, res.Operands[0]);
                    return Visit(fake);
                }

                var isProperty = false;
                var meRes = res.Operands[0] as MemberResolveResult;
                if (meRes != null && meRes.Member != null && IsEntityFunctionProperty(meRes.Member, res))
                    isProperty = true;

                JExpression node2;
                if (res.OperatorType == System.Linq.Expressions.ExpressionType.Negate ||
                    res.OperatorType == System.Linq.Expressions.ExpressionType.PreDecrementAssign ||
                    res.OperatorType == System.Linq.Expressions.ExpressionType.PreIncrementAssign ||
                    res.OperatorType == System.Linq.Expressions.ExpressionType.Not ||
                    res.OperatorType == System.Linq.Expressions.ExpressionType.OnesComplement)
                {
                    var simpler = res.OperatorType.ExtractCompoundAssignment();
                    if (isProperty && simpler != null)
                    {
                        var fakeCs = meRes.Member.AccessSelf().Binary(simpler.Value, Cs.Value(1, Project), meRes.Type);
                        node2 = VisitExpression(fakeCs);
                    }
                    else
                    {
                        node2 = new JPreUnaryExpression { Operator = Visit(res.OperatorType), Right = VisitExpression(res.Operands[0]) };
                    }
                }
                else if (res.OperatorType == System.Linq.Expressions.ExpressionType.PostIncrementAssign ||
                         res.OperatorType == System.Linq.Expressions.ExpressionType.PostDecrementAssign ||
                         res.OperatorType == System.Linq.Expressions.ExpressionType.PreIncrementAssign ||
                         res.OperatorType == System.Linq.Expressions.ExpressionType.PreDecrementAssign)
                {
                    if (isProperty)
                    {
                        var simpler = res.OperatorType.ExtractCompoundAssignment();
                        var fakeCs = meRes.Member.AccessSelf().Binary(simpler.Value, Cs.Value(1, Project), meRes.Type);
                        node2 = VisitExpression(fakeCs);

                    }
                    else
                    {
                        node2 = new JPostUnaryExpression { Operator = Visit(res.OperatorType), Left = VisitExpression(res.Operands[0]) };
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                return node2;
            }
            else if (res.Operands.Count == 2)
            {
                if (res.UserDefinedOperatorMethod != null && !JMeta.UseNativeOperatorOverloads(res.UserDefinedOperatorMethod.DeclaringTypeDefinition))
                {
                    var fake = Cs.InvokeMethod(res.UserDefinedOperatorMethod, null, res.Operands[0], res.Operands[1]);
                    return Visit(fake);
                }

                if (res.OperatorType == System.Linq.Expressions.ExpressionType.Coalesce)
                {
                    var fake = Cs.Conditional(res.Operands[0].NotEqual(Cs.Null(), Project), res.Operands[0], res.Operands[1], res.Type);
                    var fake2 = Visit(fake);
                    fake2 = new JParenthesizedExpression { Expression = (JExpression)fake2 };
                    return fake2;
                }
                var mrrOp0 = res.Operands[0] as MemberResolveResult;
                if (mrrOp0 != null && mrrOp0.Member.SymbolKind == SymbolKind.Event)
                {
                    var pe = (IEvent)mrrOp0.Member;
                    if (res.OperatorType == System.Linq.Expressions.ExpressionType.AddAssign || res.OperatorType == System.Linq.Expressions.ExpressionType.SubtractAssign)
                    {
                        var accessor = res.OperatorType == System.Linq.Expressions.ExpressionType.AddAssign ? pe.AddAccessor : pe.RemoveAccessor;
                        var fake = new CSharpInvocationResolveResult(mrrOp0.TargetResult, accessor, new List<ResolveResult> 
                            { 
                                res.Operands[1]
                            });
                        var node6 = Visit(fake);
                        return node6;
                    }
                }
                if (mrrOp0 != null && IsEntityFunctionProperty(mrrOp0.Member, res))
                {
                    var compOpType = res.OperatorType.ExtractCompoundAssignment();

                    var pe = (IProperty)mrrOp0.Member;
                    if (compOpType != null)
                    {
                        var fake = new CSharpInvocationResolveResult(mrrOp0.TargetResult, pe.Setter, new List<ResolveResult> 
                            { 
                                new OperatorResolveResult(res.Type, compOpType.Value, new CSharpInvocationResolveResult(mrrOp0.TargetResult, pe.Getter, null), res.Operands[1]) 
                            });
                        var node6 = Visit(fake);
                        return node6;
                    }
                    else if (res.OperatorType == System.Linq.Expressions.ExpressionType.Assign)
                    {
                        var args = new List<ResolveResult>();
                        if (pe.IsIndexer)
                        {
                            var irrOp0 = (CSharpInvocationResolveResult)res.Operands[0];
                            args.AddRange(irrOp0.Arguments);
                        }
                        args.Add(res.Operands[1]);
                        var fake = new CSharpInvocationResolveResult(mrrOp0.TargetResult, pe.Setter, args).AssociateWithOriginal(res);
                        var node6 = Visit(fake);
                        node6 = WrapSetterToReturnValueIfNeeded(res, node6);
                        return node6;
                    }
                }
                if (res.Operands[0] is ConversionResolveResult && res.Operands[1] is ConstantResolveResult)
                {
                    var leftConv = (ConversionResolveResult)res.Operands[0];
                    var rightConst = (ConstantResolveResult)res.Operands[1];
                    if (leftConv.Conversion.IsNumericConversion && leftConv.Input.Type == Cs.CharType(Project))
                    {
                        var value = ((char)(int)rightConst.ConstantValue).ToString();
                        var fake = Cs.Binary(leftConv.Input, res.OperatorType, Cs.Value(value, Project), leftConv.Input.Type);
                        return Visit(fake);
                    }
                }
                if (res.Operands[0].Type.Kind == TypeKind.Delegate && res.Operands[1].Type.Kind == TypeKind.Delegate)
                {
                    if (res.OperatorType == System.Linq.Expressions.ExpressionType.AddAssign || res.OperatorType == System.Linq.Expressions.ExpressionType.SubtractAssign)
                    {
                        var op = res.OperatorType == System.Linq.Expressions.ExpressionType.AddAssign ? System.Linq.Expressions.ExpressionType.Add : System.Linq.Expressions.ExpressionType.Subtract;
                        var fake = Cs.Assign(res.Operands[0], Cs.Binary(res.Operands[0], op, res.Operands[1], res.Type));
                        var node6 = Visit(fake);
                        return node6;
                    }
                    else if (res.OperatorType == System.Linq.Expressions.ExpressionType.Add || res.OperatorType == System.Linq.Expressions.ExpressionType.Subtract)
                    {
                        var combineMethod = Compiler.Project.Compilation.FindType(KnownTypeCode.Delegate).GetMethods(t => t.Name == "Combine").FirstOrDefault();
                        var removeMethod = Compiler.Project.Compilation.FindType(KnownTypeCode.Delegate).GetMethods(t => t.Name == "Remove").FirstOrDefault();

                        var meOp = res.OperatorType == System.Linq.Expressions.ExpressionType.Add ? combineMethod : removeMethod;

                        var fake = Cs.Member(null, meOp).Invoke(res.Operands[0], res.Operands[1]);
                        var node6 = Visit(fake);
                        return node6;

                    }
                }

                var node5 = new JBinaryExpression { Operator = Visit(res.OperatorType), Left = VisitExpression(res.Operands[0]), Right = VisitExpression(res.Operands[1]) };

                if (res.OperatorType == System.Linq.Expressions.ExpressionType.Equal && node5.Operator == "==")
                {
                    var att = JMeta.GetJsExportAttribute(Project.Compilation.MainAssembly);
                    if (att != null && att.UseExactEquals)
                        node5.Operator = "===";
                }
                if (res.OperatorType == System.Linq.Expressions.ExpressionType.NotEqual && node5.Operator == "!=")
                {
                    var att = JMeta.GetJsExportAttribute(Project.Compilation.MainAssembly);
                    if (att != null && att.UseExactEquals)
                        node5.Operator = "!==";
                }

                return node5;
            }
            else if (res.Operands.Count == 3)
            {
                if (res.OperatorType == System.Linq.Expressions.ExpressionType.Conditional)
                {
                    var node5 = new JConditionalExpression { Condition = VisitExpression(res.Operands[0]), TrueExpression = VisitExpression(res.Operands[1]), FalseExpression = VisitExpression(res.Operands[2]) };
                    return node5;

                }
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// Wraps a setter invocation with a js function that returns the setter value back, if another assignment operation occurs
        /// var x = contact.Name = "Shooki";
        /// var x = contact.setName("Shooki"); //error
        /// var x = (function(arg){contact.setName(arg);return arg;}).call(this, "Shooki");
        /// </summary>
        /// <param name="res"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private JNode WrapSetterToReturnValueIfNeeded(OperatorResolveResult res, JNode node2)
        {
            var node3 = node2 as JInvocationExpression;
            if (node3 == null)
                return node2;
            var parent = res.GetParent(Project);
            if (parent is OperatorResolveResult)
            {
                var parentOp = (OperatorResolveResult)parent;
                if (RequiresWrapSetterToReturnValueIfNeeded(res, parentOp))
                {
                    var lastArg = node3.Arguments.Last();
                    var prmName = "$p" + ParameterNameCounter++;
                    node3.Arguments[node3.Arguments.Count - 1] = J.Member(prmName);

                    var func = J.Function(prmName).Add(((JExpression)node2).Statement());
                    func.Add(J.Return(J.Member(prmName)));
                    node2 = WrapFunctionAndInvoke(res, func.Block, lastArg);
                }
            }
            return node2;
        }

        JExpression WrapFunctionAndInvoke(ResolveResult instanceContext, JBlock func, params JExpression[] args)
        {
            if (args.IsNotNullOrEmpty())
                throw new NotImplementedException();
            var retType = instanceContext.Type;
            return new JMultiStatementExpression { Statements = func.Statements.ToList(), Type = retType.JAccess() };
            //var retType2 = J.CreateTypeRef(retType);
            //return J.CreateFunc(retType2, func).Member("invoke").Invoke();
            //////return Js.Code("new java.lang.Runnable(){@Override public void run(){
            ////JExpression instanceContext2 = null;
            ////var me = instanceContext.GetCurrentMethod();
            ////if (me == null)
            ////{
            ////    //TODO: WARN
            ////    instanceContext2 = J.This();
            ////}
            ////else if (IsNonStatic(me))
            ////    instanceContext2 = J.This();

            ////return func.Parentheses().InvokeWithContextIfNeeded(instanceContext2, args);
        }

        bool RequiresWrapSetterToReturnValueIfNeeded(OperatorResolveResult op, OperatorResolveResult parentOp)
        {
            if (parentOp.OperatorType == System.Linq.Expressions.ExpressionType.Assign)
                return true;
            if (parentOp.OperatorType == System.Linq.Expressions.ExpressionType.Conditional && parentOp.Operands.IndexOf(op) > 0)
                return true;
            return false;
        }

        public JNode VisitLambdaResolveResult(LambdaResolveResult res)
        {
            //var prmTypes = res.Parameters.Select(t => t.Type).ToArray();
            //var retType = res.GetInferredReturnType(prmTypes);
            //var conv = res.IsValid(prmTypes, retType, CSharpConversions.Get(Compiler.Project.Compilation));
            //return Visit(conv);
            var func = new JFunction { Parameters = res.Parameters.Select(t => JsIdentifier(t.Name)).ToList() };
            var body = res.Body;
            JNode body2;
            var info = res.GetInfo();
            if (body.GetType() == typeof(ResolveResult) && body.Type.IsVoid())
            {
                var x = res.GetType().GetProperty("BodyExpression", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(res, null);
                var astNode = (AstNode)x;
                body2 = Visit(astNode);
            }
            else
            {
                body2 = Visit(res.Body);
            }


            var currentMember = res.GetCurrentMember();
            var delType = info.ConversionTargetType;
            var delMethod = delType.GetDelegateInvokeMethod();
            var returnType = delMethod.ReturnType;
            if (returnType.IsVoid())
                returnType = null;
            if (body2 is JExpression)
            {
                JStatement st = new JReturnStatement { Expression = (JExpression)body2 };
                if (delMethod != null && delMethod.ReturnType.IsVoid())
                    st = new JExpressionStatement { Expression = (JExpression)body2 };

                func.Block = new JBlock { Statements = new List<JStatement> { st } };
            }
            else if (body2 is JBlock)
            {
                func.Block = (JBlock)body2;
            }
            else
            {
                throw new Exception();
            }
            var del2 = CreateDelegate(delType, res.Parameters, returnType, func.Block);
            return del2;
        }

        JNewAnonymousClassExpression CreateDelegate(IType delType, IList<IParameter> prms, IType returnType, JBlock block)
        {
            var del2 = J.CreateDelegate(delType.JAccess(), prms.Select(t => new JParameterDeclaration { Name = t.Name, Type = t.Type.JAccessNonPrimitive() }).ToArray(), returnType.JAccessNonPrimitive(), block);
            return del2;
        }

        public JNode VisitTypeResolveResult(TypeResolveResult res)
        {
            return JNaming.JAccess(res.Type);
            //throw new NotImplementedException();
        }

        public JNode VisitResolveResult(ResolveResult res)
        {
            if (res.Type == SpecialType.NullType)
                return J.Null();
            else if (res.Type.IsVoid())
            {
                Log.Warn("void");
                return J.Code("void");
            }
            else if (res.Type.Kind == TypeKind.Dynamic)
            {
                var info = res.GetInfo();
                if (info == null || info.Nodes.Count != 1)
                    throw new NotImplementedException("Dynamics");
                var node2 = Visit(info.Nodes[0]);
                return node2;
            }
            throw new NotImplementedException();
        }
        public static string JsIdentifier(string name)
        {
            return name.Replace("<", "$").Replace(">", "$");
        }
        public JNode VisitLocalResolveResult(LocalResolveResult res)
        {
            var node2 = J.Member(JsIdentifier(res.Variable.Name));
            if (res.Variable != null && res.Variable.Type.Kind == TypeKind.ByReference)
            {
                node2 = node2.Member("Value");
            }
            return node2;
        }

        public JNode VisitConversionResolveResult(ConversionResolveResult res)
        {
            var input = res.Input;
            var conversion = res.Conversion;
            var conversionType = res.Type;

            var info = res.GetInfo();
            if (info == null)
            {
                info = new ResolveResultInfo { Conversion = conversion, ResolveResult = res, ConversionTargetType = res.Type };
                res.SetInfo(info);

            }
            var info2 = input.GetInfo();
            if (info2 == null)
            {
                input.SetInfo(new ResolveResultInfo { Nodes = info.Nodes.ToList(), Conversion = conversion, ConversionTargetType = res.Type, ResolveResult = input });
            }
            if (conversion.IsUserDefined && res.Type.FullName == "JSharp.Java.JCode" && input is ConstantResolveResult)
            {
                var value = ((ConstantResolveResult)input).ConstantValue;
                var node3 = J.Code(value == null ? "null" : value.ToString());
                return node3;
            }
            return VisitConversion(input, conversion, conversionType);
        }

        private JNode VisitConversion(ResolveResult input, Conversion conversion, IType conversionType)
        {
            ////TODO: HACK: https://github.com/icsharpcode/NRefactory/issues/183
            //var isImplicit = res.Conversion.IsImplicit;
            //if (!isImplicit && res.Conversion.IsExplicit && res.Conversion.Method != null && res.Conversion.Method.Name != null && res.Conversion.Method.Name.Contains("Implicit"))
            //    isImplicit = true;
            if (conversion.IsMethodGroupConversion)
            {
                var me = conversion.Method;
                var delType = conversionType;
                var exp = me.JAccess().Invoke(me.Parameters.Select(t=>J.Member(t.Name)).ToArray());
                var st = J.Return(exp);
                var block = J.Block().Add(st);
                var del2 = CreateDelegate(delType, me.Parameters, me.ReturnType, block);
                return del2;
                //J.CreateDelegate(conversionType.JAccess(), me.Parameters.Select(t=>t.Acc
               //TODO:  J.CreateDelegate(conversionType.JAccess(), 
            }
            else if (conversion.IsUserDefined)
            {
                ITypeDefinition typeDef;
                if (conversion.Method != null && conversion.Method.DeclaringType != null)
                {
                    typeDef = conversion.Method.DeclaringType.GetDefinitionOrArrayType(Compiler);
                }
                else
                {
                    typeDef = conversionType.GetDefinitionOrArrayType(Compiler);
                }
                var nativeOverloads = JMeta.UseNativeOperatorOverloads(typeDef);
                if (nativeOverloads)
                    return Visit(input);
                var fake = conversion.Method.InvokeMethod(null, input);
                var node2 = Visit(fake);
                return node2;
            }
            else if (conversion.IsTryCast || conversion.IsExplicit)
            {
                var typeDef = conversionType.GetDefinitionOrArrayType(Compiler);
                var omitCasts = JMeta.OmitCasts(typeDef, Project);
                if (omitCasts)
                    return Visit(input);
                if (true)//Sk.NativeCasts(typeDef))
                {
                    var exp2 = VisitExpression(input);
                    var type2 = JNaming.JAccess(conversionType);
                    if (conversion.IsTryCast)
                    {
                        var node2 = exp2.InstanceOf(type2).Conditional(exp2, J.Null());
                        return node2;
                    }
                    else
                    {
                        return J.Cast(exp2, type2);
                    }
                }
                //else
                //{
                //    var cast = conversion.IsTryCast ? "As" : "Cast";
                //    var node2 = J.Member(cast).Invoke(VisitExpression(input), JNaming.JAccess(conversionType));
                //    return node2;
                //}
            }
            return Visit(input);
        }

        public JNode VisitConstantResolveResult(ConstantResolveResult res)
        {
            var nodes = res.GetNodes();
            if (res.Type is DefaultTypeParameter)
            {
                return J.Member("Default").Invoke(JNaming.JAccess(res.Type));
            }
            if (res.Type != null && res.Type.Kind == TypeKind.Enum)
            {
                return Visit(JTypeImporter.GetValueTypeInitializer(res.Type, Compiler));
            }
            //var nodes = res.GetNodes();
            //if (nodes.IsNotNullOrEmpty())
            //{
            //    var node = nodes[0];
            //    if (node != null && node is PrimitiveExpression)
            //    {
            //        var node2 = Visit(node); //use literal value instead
            //        return node2;
            //    }
            //}
            return J.Value(res.ConstantValue);
        }

        public JNode VisitThisResolveResult(ThisResolveResult res)
        {
            return J.This();
        }

        public JNode VisitInvocationResolveResult(InvocationResolveResult res)
        {
            return Visit(res.ToCSharpInvocationResolveResult());
        }

        public JNode VisitCSharpInvocationResolveResult(CSharpInvocationResolveResult res)
        {
            if (res.Member.IsConstructor())
            {
                if (res.Type is AnonymousType)
                {
                    //TODO: check context class JsType.NativeJsons
                    var json = InitializersToJson(res.InitializerStatements, res.Type);
                    return json;
                }
                else
                {
                    return VisitInvocationResolveResultAsCtor(res);
                }
            }
            else
            {
                return VisitInvocationResolveResult(res);
            }
        }

        public JNode VisitMemberResolveResult(MemberResolveResult res)
        {
            var me = res.Member;
            JNode node2;
            bool enumValuesAsNames;

            if (me == null) //TODO: dynamics
            {
                throw new NotImplementedException();
                //var node3 = Js.Member(node.MemberName);
                //if (node.Target != null)
                //    node3.PreviousMember = VisitExpression(node.Target);
                //return node3;
            }
            else if (IsEntityFunctionProperty(res.Member, res))//(Entity)node.entity))
            {
                var pe = (IProperty)me;
                var xxx = new CSharpInvocationResolveResult(res.TargetResult, pe.Getter, null);
                node2 = Visit(xxx);
                return node2;
            }
            else if (me.IsEnumMember() && JMeta.UseJsonEnums(me, out enumValuesAsNames))
            {
                var me2 = (IField)me;
                if (enumValuesAsNames)
                    return J.String(JNaming.JName(me2));
                else
                    return J.Value(me2.ConstantValue);
            }
            //TODO: Support a way to override this (JsField.ConstantInlining=false)
            else if (res.IsCompileTimeConstant && !me.IsEnumMember())
            {
                return J.Value(res.ConstantValue);
            }
            else
            {
                var node3 = JNaming.JAccess(me);
                node2 = node3;
                if (res.TargetResult != null && !me.IsStatic())
                {
                    var instanceContext = VisitExpression(res.TargetResult);
                    if (node3.Name.IsNullOrEmpty()) //support Name=""
                        node2 = instanceContext;
                    else
                        node3.PreviousMember = instanceContext;
                }
            }
            return node2;
        }
        #endregion

        #region InvocationResolveResult (methods and ctors)

        private JNode VisitInvocationResolveResult(CSharpInvocationResolveResult res)
        {
            ////TODO: LET LINQ
            //var firstNode = res.GetFirstNode();
            //if (firstNode != null && firstNode is QueryLetClause)
            //{
            //    foreach (var arg in res.Arguments)
            //    {
            //        if (arg.GetInfo() == null)
            //            arg.SetInfo(new ResolveResultInfo { Nodes = { firstNode } });
            //    }
            //}
            var member = res.Member;
            var me = member as IMethod;
            if (me == null)
            {
                var pe = member as IProperty;
                if (pe != null)
                {
                    me = pe.Getter;
                    member = me;
                }
            }
            var att = me != null ? JMeta.GetJMethodAttribute(me) : null;

            if (att != null && att.InlineCode != null)
            {
                return J.Code(att.InlineCode);
            }
            //TODO: move defines locally
            var condAtt = me.GetMetadata<System.Diagnostics.ConditionalAttribute>();
            if (condAtt != null && Compiler != null && Compiler.Defines != null && !Compiler.Defines.Contains(condAtt.ConditionString))
                return null;
            if (att != null && att.OmitCalls)
            {
                if (me.IsStatic() && !me.IsExtensionMethod)
                    return null;
                if (me.IsExtensionMethod && !res.IsExtensionMethodInvocation)
                    return null;
                if (res.Arguments.IsEmpty() && res.TargetResult != null)
                    return VisitExpression(res.TargetResult);
                return Visit(res.Arguments[0]);
            }
            var jsMember = JNaming.JAccess(member);
            if (res.TargetResult != null && !member.IsStatic() && member.SymbolKind != SymbolKind.Constructor) //TargetResult==null when ctor
            {
                var target = VisitExpression(res.TargetResult);
                if (jsMember.PreviousMember != null)
                    throw new NotSupportedException();
                jsMember.PreviousMember = target;
            }
            var bindings = res.GetArgumentsForCall2();
            if (JMeta.OmitOptionalParameters(me))
            {
                bindings.RemoveAll(t => t.ArgResult == null);
            }
            if (JMeta.IsNativeParams(me))
            {
                var binding = bindings.Where(t => t.Parameter.IsParams).FirstOrDefault();
                if (binding != null)
                {
                    if (binding.CallResult is ArrayCreateResolveResult)
                    {
                        var arrayRes = (ArrayCreateResolveResult)binding.CallResult;
                        bindings.Remove(binding);
                        if (arrayRes.InitializerElements.IsNotNullOrEmpty())
                        {
                            foreach (var init in arrayRes.InitializerElements)
                            {
                                var b = binding.Clone();
                                b.CallResult = init;
                                bindings.Add(b);
                            }
                        }
                    }
                    else
                    {
                        Log.Warn(res.GetFirstNode(), "Invalid params parameter passed to method with NativeParams=true");
                    }

                }
            }
            var byRefs = new List<ByReferenceResolveResult>();
            List<int> refToRefs = new List<int>();
            var c = 0;
            foreach (var binding in bindings)
            {
                var byRef = binding.CallResult as ByReferenceResolveResult;
                if (byRef == null)
                {
                    c++;
                    continue;
                }
                var x = byRef.ElementResult as LocalResolveResult;
                if (x != null && x.Variable != null && x.Variable.Type.Kind == TypeKind.ByReference)
                {
                    if (binding.Parameter.IsRef || binding.Parameter.IsOut)
                        refToRefs.Add(c);
                    c++;
                    continue;
                }
                byRefs.Add(byRef);
                c++;
            }
            var callArgs = bindings.Select(t => t.CallResult).ToList();
            var node2 = new JInvocationExpression
            {
                Member = jsMember,
                Arguments = VisitExpressions(callArgs),
            };
            foreach (var i in refToRefs)
            {
                JMemberExpression jsmex = node2.Arguments[i] as JMemberExpression;
                if (jsmex != null)
                    node2.Arguments[i] = jsmex.PreviousMember;//remove the .Value ref wrapper

            }
            if (me != null && me.IsExtensionMethod && res.IsExtensionMethodInvocation && JMeta.ExtensionImplementedInInstance(me))
            {
                var arg = node2.Arguments[0];
                node2.Arguments.RemoveAt(0);
                if (jsMember.PreviousMember != null)
                    throw new NotImplementedException();
                jsMember.PreviousMember = arg;
            }

            TransformIntoBaseMethodCallIfNeeded(res, node2);
            if (att != null)
            {
                if (att.OmitParanthesis)
                    node2.OmitParanthesis = true;
                if (node2.Arguments == null)
                    node2.Arguments = new List<JExpression>();
                if (att.InsertArg2 != null)
                    node2.Arguments.InsertOrAdd(2, new JCodeExpression { Code = att.InsertArg2.ToString() });
                if (att.InsertArg1 != null)
                    node2.Arguments.InsertOrAdd(1, new JCodeExpression { Code = att.InsertArg1.ToString() });
                if (att.InsertArg0 != null)
                    node2.Arguments.InsertOrAdd(0, new JCodeExpression { Code = att.InsertArg0.ToString() });
                node2.OmitCommas = att.OmitCommas;
                node2.ArgumentsPrefix = att.ArgumentsPrefix;
                node2.ArgumentsSuffix = att.ArgumentsSuffix;
                if (att.InstanceImplementedAsExtension)
                {
                    var ext = (JMemberExpression)node2.Member;
                    node2.Arguments.Insert(0, ext.PreviousMember);
                    ext.PreviousMember = null;
                }
            }


            //if (me != null && me is SpecializedMethod && !Sk.IgnoreGenericMethodArguments(me))
            //{
            //    List<JsExpression> genericArgs;
            //    if (me.IsConstructor)
            //    {
            //        var ce = me.DeclaringType as ParameterizedType;
            //        if (ce != null)
            //            genericArgs = ce.TypeArguments.Select(t => SkJs.EntityTypeRefToMember(t, true)).ToList();
            //        else
            //            genericArgs = new List<JsExpression>();
            //    }
            //    else
            //    {
            //        var sme = (SpecializedMethod)me;
            //        genericArgs = sme.TypeArguments.Select(t => SkJs.EntityTypeRefToMember(t, true)).ToList();
            //    }
            //    if (node2.Arguments == null)
            //        node2.Arguments = new List<JsExpression>(genericArgs);
            //    else
            //        node2.Arguments.InsertRange(0, genericArgs);
            //}
            if (att != null && att.OmitDotOperator)
            {
                if (node2.Member is JMemberExpression && node2.Arguments.Count == 1 && att.OmitParanthesis)
                {
                    var meNode = (JMemberExpression)node2.Member;
                    var node3 = new JBinaryExpression { Left = meNode.PreviousMember, Operator = meNode.Name, Right = node2.Arguments[0] };
                    return node3;
                }
                else
                {
                    Log.Warn(res.GetFirstNode(), "TODO:OmitDotOperator is not supported in this syntax.");
                }
            }
            if (node2.Member is JMemberExpression)
            {
                var x = (JMemberExpression)node2.Member;
                if (x.Name.IsNullOrEmpty() && jsMember.PreviousMember != null)
                    node2.Member = x.PreviousMember;

            }
            if (res.Member.SymbolKind == SymbolKind.Indexer && JMeta.UseNativeIndexer((IProperty)res.Member))
            {
                var node3 = new JIndexerAccessExpression
                {
                    Member = node2.Member,
                    Arguments = node2.Arguments,
                };

                return node3;
            }

            if (byRefs.IsNotNullOrEmpty())
            {
                var func = J.Function();
                foreach (var byRef in byRefs)
                {
                    func.Add(J.Assign(VisitExpression(byRef), J.Json().Add("Value", VisitExpression(byRef))).Statement());
                }
                func.Add(J.Var("$res", res.Type.JAccess(), node2).Statement());
                foreach (var byRef in byRefs)
                {
                    func.Add(J.Assign(VisitExpression(byRef), VisitExpression(byRef).Member("Value")).Statement());
                }
                func.Add(J.Return(J.Member("$res")));
                var node5 = WrapFunctionAndInvoke(res, func.Block);
                return node5;
            }


            return node2;

        }


        IMethod GetParentMethod(ResolveResult res)
        {
            var node = res.GetFirstNode();
            if (node == null)
                return null;
            var meParent = node.GetCurrentMethod();
            return meParent;
        }

        private JNode VisitInvocationResolveResultAsCtor(CSharpInvocationResolveResult res)
        {
            if (res.Type.Kind == TypeKind.Delegate)
                return Visit(res.Arguments.Single());
            var me = (IMethod)res.Member;
            var meAtt = JMeta.GetJMethodAttribute(me);
            var ce = me.GetDeclaringTypeDefinition();
            var att = ce == null ? null : ce.GetJsTypeAttribute();
            if (att != null && att.Mode == JsMode.Json && (meAtt == null || meAtt.Name == null))
            {
                var node2 = VisitInvocationResolveResult(res);
                var json = InitializersToJson(res.InitializerStatements, res.Type);
                return json;
            }
            else
            {
                var invokeExp = (JInvocationExpression)VisitInvocationResolveResult(res);
                var newExp = new JNewObjectExpression { Invocation = invokeExp };
                JExpression finalExp;
                if (meAtt != null && meAtt.OmitNewOperator)
                    finalExp = invokeExp;
                else
                    finalExp = newExp;

                if (meAtt != null && meAtt.JsonInitializers)
                {
                    var json = InitializersToJson(res.InitializerStatements, res.Type);
                    invokeExp.Arguments.Add(json);
                }
                else if (res.InitializerStatements.IsNotNullOrEmpty())
                {
                    //var func = J.Function();
                    var func = J.Block();
                    var inits2 = res.InitializerStatements.Select(t => Visit(t)).ToList();
                    var init1 = res.InitializerStatements[0];

                    var target = FindInitializedObjectResolveResult(res);// ((init1 as OperatorResolveResult).Operands[0] as MemberResolveResult).TargetResult as InitializedObjectResolveResult;
                    var varName = Initializers[target];
                    func.Add(J.Var(varName, res.Type.JAccess(), finalExp).Statement());

                    foreach (var init in inits2)
                    {
                        var exp = ((JExpression)init);
                        func.Add(exp.Statement());
                    }
                    func.Add(J.Return(J.Member(varName)));
                    finalExp = WrapFunctionAndInvoke(res, func);
                }

                return finalExp;
            }

        }

        private JExpression InitializersToJson(IList<ResolveResult> initializerStatements, IType type)
        {
            if (type.GetMethods(t => t.Name == "Add").FirstOrDefault() != null)
            {
                var items = initializerStatements.Cast<CSharpInvocationResolveResult>().Select(t => t.Arguments[0]).ToList();
                var items2 = VisitExpressions(items);
                var arr = J.NewJsonArray(items2.ToArray());
                return arr;
            }
            else
            {
                var json = J.Json();
                foreach (var st in initializerStatements)
                {
                    if (st is OperatorResolveResult)
                    {
                        var op = (OperatorResolveResult)st;
                        var mrr = (MemberResolveResult)op.Operands[0];
                        var name = JNaming.JName(mrr.Member);
                        var value = VisitExpression(op.Operands[1]);
                        json.Add(name, value);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                //var inits2 = initializerStatements.Select(t => Visit(t)).ToList();
                //var namesValues = inits2.Cast<JsBinaryExpression>().Select(t => Js.JsonNameValue(((JsMemberExpression)t.Left).Name, t.Right)).ToList();
                //var json = Js.Json();
                //json.NamesValues = namesValues;
                return json;
            }
        }

        #endregion



        #region IResolveResultVisitor<JsNode> Members


        public JNode VisitByReferenceResolveResult(ByReferenceResolveResult res)
        {
            return Visit(res.ElementResult);
        }

        #endregion

        public JNode VisitNamedArgumentResolveResult(NamedArgumentResolveResult res)
        {
            throw new NotImplementedException("VisitNamedArgumentResolveResult");
        }


        #region ICSharpResolveResultVisitor<JsNode> Members


        public JNode VisitDynamicInvocationResolveResult(DynamicInvocationResolveResult res)
        {
            var target2 = VisitExpression(res.Target);
            var args2 = VisitExpressions(res.Arguments);
            if (res.InvocationType == DynamicInvocationType.Invocation)
            {
                var node2 = target2.Invoke(args2.ToArray());
                return node2;
            }
            else if (res.InvocationType == DynamicInvocationType.ObjectCreation)
            {
                throw new NotSupportedException();
                //var node2 = J.New(target2, args2.ToArray());
                //return node2;
            }
            else if (res.InvocationType == DynamicInvocationType.Indexing)
            {
                var node2 = target2.IndexerAccess(args2.Single());
                return node2;
            }
            else
                throw new NotImplementedException("Dynamics: " + res.InvocationType);
        }

        public JNode VisitDynamicMemberResolveResult(DynamicMemberResolveResult res)
        {
            var target2 = VisitExpression(res.Target);
            var node2 = target2.Member(res.Member);
            return node2;
        }

        #endregion


        JExpression GetNonStaticMethodContextJs(ResolveResult res)
        {
            if (IsInNonStaticMethodContext(res))
                return J.This();
            return J.Null();
        }
        bool IsInNonStaticMethodContext(ResolveResult res)
        {
            return IsNonStatic(res.GetCurrentMethod());
        }

    }

    static class Extensions5
    {
        public static CSharpInvocationResolveResult ToCSharpInvocationResolveResult(this InvocationResolveResult res)
        {
            if (res == null)
                return null;
            if (res is CSharpInvocationResolveResult)
                return (CSharpInvocationResolveResult)res;
            var res2 = new CSharpInvocationResolveResult(res.TargetResult, res.Member, res.Arguments, initializerStatements: res.InitializerStatements);
            res2.Tag = res.Tag;
            return res2;
        }
    }
}
