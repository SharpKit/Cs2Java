using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using JSharp.Compiler;
using JSharp.Utils;
using System.CodeDom.Compiler;
using JSharp.Java;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using JSharp.Java.Ast;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Extensions;

namespace JSharp.Compiler
{
    class JTypeImporter
    {
        public CompilerTool Compiler { get; set; }

        #region Properties

        public bool LongFunctionNames { get; set; }
        public JCodeImporter JsCodeImporter { get; set; }
        public string AssemblyName { get; set; }
        public CompilerLogger Log { get; set; }
        #endregion

        #region _Visit
        protected virtual List<JEntityDeclaration> _Visit(ITypeDefinition ce)
        {
            if (CompilerConfig.Current.EnableLogging)
            {
                Log.Debug("JsTypeImporter: Visit Type: " + ce.ToString());
            }
            List<JEntityDeclaration> node;
            if (ce.Kind == TypeKind.Class)
            {
                node = _VisitClass(ce);
            }
            else if (ce.Kind == TypeKind.Interface)
            {
                node = _VisitClass(ce);
            }
            else if (ce.Kind == TypeKind.Delegate)
            {
                node = _VisitDelegate(ce);
            }
            else if (ce.Kind == TypeKind.Struct)
            {
                node = _VisitStruct(ce);
            }
            else if (ce.Kind == TypeKind.Enum)
            {
                node = _VisitEnum(ce);
            }
            else
            {
                throw new NotImplementedException();
            }
            return node;
        }
        protected virtual List<JEntityDeclaration> _VisitStruct(ITypeDefinition ce)
        {
            throw new CompilerException(ce, "Member is not supported for export");
        }

        public virtual List<JEntityDeclaration> _VisitDelegate(ITypeDefinition ce)
        {
            return _VisitClass(ce);
        }

        public virtual List<JEntityDeclaration> _Visit(IMethod me)
        {
            if (me.IsConstructor)
                return ExportConstructor(me);
            return ExportMethod(me);
        }
        public virtual List<JEntityDeclaration> _Visit(IField me)
        {
            return _VisitField(me);
        }
        public virtual List<JEntityDeclaration> _Visit(IEvent me)
        {
            if (me.DeclaringType.Kind == TypeKind.Interface)
                return null;
            var list2 = new List<JEntityDeclaration>();
            if (me.AddAccessor != null)
                list2.Add(Visit(me.AddAccessor).Single());
            if (me.RemoveAccessor != null)
                list2.Add(Visit(me.RemoveAccessor).Single());
            return list2;
        }


        public virtual List<JEntityDeclaration> _VisitClass(ITypeDefinition ce)
        {
            var name = GetClassName(ce);
            var ce2 = new JClassDeclaration
            {
                TypeDefinition = ce,
                IsInterface = ce.IsInterface(),
                TypeParameters = ce.TypeParameters.Select(t => t.JAccess()).ToList(),
                Modifiers = { IsAbstract = ce.IsAbstract },
                Name = name.ToJs(),
            };
            ImportModifiers(ce, ce2);

            var extends = new List<IType>();
            var implements = new List<IType>();
            foreach (var baseCe in ce.DirectBaseTypes)
            {
                if (baseCe.IsKnownType(KnownTypeCode.Object) || baseCe.FullName == "java.lang.Object")
                    continue;
                if (baseCe.GetDefinition().IsInterface())
                    implements.Add(baseCe);
                else
                    extends.Add(baseCe);
            }
            ce2.Extends = extends.Select(t => t.JAccess()).ToList();
            ce2.Implements = implements.Select(t => t.JAccess()).ToList();

            var members = GetMembersToExport(ce);
            var x = VisitToUnit(members);
            foreach (var xx in x)
            {
                ce2.Declarations.Add(xx);
            }
            return new List<JEntityDeclaration> { ce2 };
        }

        private static JMemberExpression GetClassName(ITypeDefinition ce)
        {
            var name = JNaming.JName2(ce).RemoveGenericArgs();
            name.PreviousMember = null;
            return name;
        }


        public virtual List<JEntityDeclaration> _VisitEnum(ITypeDefinition ce)
        {
            var unit = new JUnit { Statements = new List<JStatement>() };
            ExportTypeNamespace(ce);
            var att = ce.GetMetadata<JEnumAttribute>();
            bool valuesAsNames;
            JMeta.UseJsonEnums(ce, out valuesAsNames);
            //var valuesAsNames = att != null && att.ValuesAsNames;
            var constants = ce.GetConstants().ToList();
            if (!valuesAsNames && constants.Where(t => t.ConstantValue == null).FirstOrDefault() != null)
            {
                var value = 0L;
                foreach (var c in constants)
                {
                    if (c.ConstantValue == null)
                        c.SetConstantValue(value);
                    else
                        value = Convert.ToInt64(c.ConstantValue);
                    value++;
                }
            }
            constants.RemoveAll(t => !JMeta.IsJsExported(t));
            var json = new JJsonObjectExpression { NamesValues = new List<JJsonNameValue>() };
            json.NamesValues.AddRange(constants.Select(t => Export(t, valuesAsNames)));
            var st = ce.JAccess().Assign(json).Statement();

            unit.Statements.Add(st);
            throw new NotSupportedException();
        }


        public virtual List<JEntityDeclaration> _VisitField(IField fld)
        {
            var init2 = GetCreateFieldInitializer(fld);
            JExpression initializer = null;
            if (init2 != null)
                initializer = JsCodeImporter.VisitExpression(init2);
            var fe2 = new JFieldDeclaration
            {
                FieldDefinition = fld,
                Initializer = initializer,
                Type = fld.Type.JAccess(),
                Name = JNaming.JName(fld)
            };
            ImportModifiers(fld, fe2);
            return new List<JEntityDeclaration> { fe2 };
        }


        public virtual List<JEntityDeclaration> ExportMethod(IMethod me)
        {
            var jma = JMeta.GetJMethodAttribute(me);
            if (jma != null && (jma.Global || jma.GlobalCode))
            {
                throw new NotSupportedException();

                //return CreateGlobalImporter().ExportMethod(me);
            }
            else
            {
                var ce = me.GetDeclaringTypeDefinition();
                var member = me.JAccess();

                var func = new JFunction();
                //func.Name = me.Name;
                func.Parameters = ExportMethodParameters(me);
                func.Block = ExportMethodBody(me);
                if (JsCodeImporter.SupportClrYield)
                    func = ApplyYield(func);
                var typeParams = me.TypeParameters.Select(t => t.JAccess()).ToList();
                var decl = new JMethodDeclaration
                {
                    Name = JNaming.JName(me),
                    MethodDefinition = me,
                    MethodBody = func.Block,
                    Parameters = ExportParameters(me.Parameters),
                    Type = me.ReturnType.JAccess(),
                    TypeParameters = typeParams,
                };
                ImportModifiers(me, decl);

                if (me.IsOverride || me.ImplementedInterfaceMembers.IsNotNullOrEmpty())
                    decl.Annotations.Add(new JAnnotationDeclaration { Name = "Override" });
                return new List<JEntityDeclaration> { decl };
            }
        }

        private static void ImportModifiers(IEntity me, JEntityDeclaration decl)
        {
            decl.Modifiers.IsAbstract = me.IsAbstract && !me.IsStatic; //occurs on static classes
            decl.Modifiers.IsPublic = me.IsPublic;
            decl.Modifiers.IsProtected = me.IsProtected;
            decl.Modifiers.IsPrivate = me.IsPrivate;
            decl.Modifiers.IsStatic = me.IsStatic && !(me is IType);
        }

        List<JParameterDeclaration> ExportParameters(IList<IParameter> prms)
        {
            return prms.Select(t => new JParameterDeclaration { Name = t.Name, Type = t.Type.JAccess(), Parameter = t }).ToList();
        }

        public virtual List<JEntityDeclaration> _Visit(IProperty pe)
        {
            var list = GetAccessorsToExport(pe);
            if (JMeta.IsNativeProperty(pe))
            {
                var statements = new List<JStatement>();

                statements.AddRange(list.Select(ExportMethod).Cast<JStatement>());

                var json = new JJsonObjectExpression();
                foreach (var accessor in list)
                {
                    //if (accessor == pe.Getter) json.Add("get", (JsFunction)ExportMethod(accessor));
                    //if (accessor == pe.Setter) json.Add("set", (JsFunction)ExportMethod(accessor));
                    if (accessor == pe.Getter) json.Add("get", ExportTypePrefix(pe.Getter.GetDeclaringTypeDefinition(), pe.IsStatic).Member("get_" + pe.Name));
                    if (accessor == pe.Setter) json.Add("set", ExportTypePrefix(pe.Setter.GetDeclaringTypeDefinition(), pe.IsStatic).Member("set_" + pe.Name));
                }
                if (JMeta.IsNativePropertyEnumerable(pe)) json.Add("enumerable", new JCodeExpression() { Code = "true" });
                //json.Add("configurable", new JsCodeExpression() { Code = "true" });

                var defineStatement = new JExpressionStatement()
                {
                    Expression = new JInvocationExpression()
                    {
                        Member = new JMemberExpression() { Name = "Object.defineProperty" },
                        Arguments = new List<JExpression>(new JExpression[] 
                        {
                            ExportTypePrefix(pe.GetDeclaringTypeDefinition(), pe.IsStatic),
                            new JStringExpression(){ Value = pe.Name },
                            json 
                        })
                    }
                };

                statements.Add(defineStatement);
                throw new NotSupportedException();
                //return new JsUnit() { Statements = statements };
            }
            else
            {
                var list2 = list.Select(ExportMethod).ToList();
                return list2.SelectMany(t => t).ToList();
            }
        }


        /// <summary>
        /// Generates backing fields for automatic properties, and fake fields for properties who are defined as fields
        /// </summary>
        /// <param name="ce"></param>
        /// <returns></returns>
        protected IEnumerable<IField> GeneratePropertyFields(ITypeDefinition ce, bool isStatic)
        {
            //var list = new List<IField>();
            foreach (var pe in ce.GetProperties(t => t.IsStatic == isStatic, GetMemberOptions.IgnoreInheritedMembers))
            {
                if (!JMeta.IsJsExported(pe))
                    continue;
                if (JMeta.IsNativeField(pe))
                    yield return GenerateFakeField(pe);
                else if (pe.IsAutomaticProperty(Compiler.Project))
                    yield return GenerateBackingField(pe);
            }
            //return list;

        }

        protected IEnumerable<IMember> GetExportedDeclaredAndGeneratedFields(ITypeDefinition ce, bool isStatic)
        {
            foreach (var pe in ce.GetFields(t => t.IsStatic == isStatic, GetMemberOptions.IgnoreInheritedMembers))
            {
                if (!JMeta.IsJsExported(pe))
                    continue;
                yield return pe;
            }

            foreach (var pe in ce.GetEvents(t => t.IsStatic == isStatic, GetMemberOptions.IgnoreInheritedMembers))
            {
                if (!JMeta.IsJsExported(pe))
                    continue;
                yield return pe;
            }

            foreach (var fe in GeneratePropertyFields(ce, isStatic))
                yield return fe;
        }

        protected FakeField GenerateFakeField(IProperty pe)
        {
            return new FakeField
            {
                Name = pe.Name,
                DeclaringTypeDefinition = pe.DeclaringTypeDefinition,
                DeclaringType = pe.DeclaringType,
                BodyRegion = pe.BodyRegion,
                Region = pe.Region,
                Type = pe.ReturnType,
                IsStatic = pe.IsStatic,
                IsPrivate = true,
                ParentAssembly = pe.ParentAssembly,
            };
        }
        protected IField GenerateBackingField(IProperty pe)
        {
            var field = GenerateFakeField(pe);
            field.Name = JNaming.JName(pe).ToJavaNaming();
            return field;
        }
        #endregion

        #region ExportMember

        public virtual List<JEntityDeclaration> ExportConstructor(IMethod ctor)
        {
            var ctorName = GetClassName(ctor.DeclaringTypeDefinition).ToJs();// SkJs.JName(ctor);
            var func = new JFunction { Parameters = new List<string>() };
            var me2 = new JMethodDeclaration { Name = ctorName, MethodDefinition = ctor, MethodBody = ExportConstructorBody(ctor) };
            me2.Parameters = ExportParameters(ctor.Parameters);
            ImportModifiers(ctor, me2);
            //ExportConstructorParameters(ctor, func);
            return new List<JEntityDeclaration> { me2 };
        }
        protected JBlock ExportConstructorBody(IMethod ctor)
        {
            var ctorNode = (ConstructorDeclaration)ctor.GetDeclaration();
            BlockStatement ccc = null;
            if (ctorNode != null)
                ccc = ctorNode.Body;
            //var ccc = ctor.GetDefinition();//.decl as CsConstructor;
            //var ccc = ctor.GetDefinition();
            var block2 = (JBlock)JsCodeImporter.Visit(ccc);
            if (block2 == null)
                block2 = new JBlock { Statements = new List<JStatement>() };
            var ce = ctor.GetDeclaringTypeDefinition();
            var isClr = JMeta.IsClrType(ce);
            var isPrototype = JMeta.IsNativeType(ce);
            var statements = new List<JStatement>();

            if (!ctor.IsStatic)
            {
                //TODO:
                //base/this ctor invocation
                var invocation = GetConstructorBaseOrThisInvocation2(ctor);
                if (invocation.Arguments.Count > 0)
                {
                    var exp2 = (JNewObjectExpression)JsCodeImporter.VisitExpression(invocation);
                    var exp3 = exp2.Invocation;
                    exp3.Member = J.Member("super");
                    statements.Insert(0, exp3.Statement());
                }
            }
            if (block2.Statements == null)
                block2.Statements = new List<JStatement>();
            block2.Statements.InsertRange(0, statements);

            return block2;
        }


        void ExportConstructorParameters(IMethod ctor, JFunction func)
        {
            var ce = ctor.GetDeclaringTypeDefinition();
            var list = new List<string>();
            if (!JMeta.IgnoreTypeArguments(ce))
            {
                //danel
                var gprms = ce.TypeParameters.ToList();//.GetGenericArguments().Where(ga => ga.isGenericParam()).ToList();
                if (gprms.IsNotNullOrEmpty())
                {
                    var i = 0;
                    foreach (var gprm in gprms)
                    {
                        func.Parameters.Add(gprm.Name);
                        if (!ctor.IsStatic && func.Block != null)
                        {
                            func.Block.Statements.Insert(i, J.This().Member(gprm.Name).Assign(J.Member(gprm.Name)).Statement());
                            i++;
                        }
                    }
                }
            }
            var prms = ctor.Parameters;
            if (prms != null)
            {
                func.Parameters.AddRange(prms.Select(t => t.Name));
            }
        }
        protected JFunction ApplyYield(JFunction func)
        {
            if (JsCodeImporter.SupportClrYield && func.Block.Descendants().OfType<JsYieldStatement>().FirstOrDefault() != null)
            {
                var yielder = new YieldRefactorer { BeforeFunction = func };
                yielder.Process();
                return yielder.AfterFunction;
            }
            return func;
        }
        protected JBlock ExportMethodBody(IMethod me)
        {
            if (me.DeclaringTypeDefinition.IsInterface())
            {
                return null;
            }
            if (CompilerConfig.Current.EnableLogging)
            {
                Log.Debug("JsTypeImporter: Visit Method: " + me.ToString());
            }
            var nativeCode = JMeta.GetNativeCode(me);
            if (nativeCode != null)
            {
                var block = J.Block().Add(J.Code(nativeCode).Statement()); //TODO: double semicolon?
                var x = block.ToJs();
                return block;
            }
            var def = me.GetDefinition();
            if (def == null || def.IsNull)
            {
                if (me.DeclaringTypeDefinition.IsDelegate())
                {
                    var block = J.Block();
                    if (!me.ReturnType.IsVoid())
                        block.Add(J.Return(J.Null()));
                    return block;
                }
                if (me.IsAutomaticEventAccessor())
                {
                    if (me.IsEventAddAccessor())
                    {
                        var node = GenerateAutomaticEventAccessor((IEvent)me.GetOwner(), false);
                        return node.Block;
                    }
                    else if (me.IsEventRemoveAccessor())
                    {
                        var node = GenerateAutomaticEventAccessor((IEvent)me.GetOwner(), true);
                        return node.Block;
                    }
                }
                else if (me.IsAutomaticPropertyAccessor())
                {
                    var bf = J.Member(JNaming.JName(me.AccessorOwner).ToJavaNaming());
                    if (!me.IsStatic)
                        bf.PreviousMember = J.This();
                    else if (!JMeta.IsGlobalMethod(me))
                        bf.PreviousMember = JNaming.JAccess(me.DeclaringType);
                    if (me.IsGetter())
                        return J.Block().Add(J.Return(bf));
                    else
                        return J.Block().Add(bf.Assign(J.Member("value")).Statement());
                }
                return null;
            }
            var block2 = (JBlock)JsCodeImporter.Visit(def);
            if (def.Descendants.OfType<YieldReturnStatement>().FirstOrDefault() != null)
            {
                if (!JsCodeImporter.SupportClrYield)
                {
                    if (block2.Statements == null)
                        block2.Statements = new List<JStatement>();
                    var arg = me.ReturnType.TypeArguments[0].JAccess();
                    var listType = J.Members("java.util.ArrayList").AddGenericArg(arg);
                    var yieldVar = J.Var("$yield", listType, J.New(listType)).Statement();
                    block2.Statements.Insert(0, yieldVar);
                    block2.Statements.Add(JCodeImporter.GenerateYieldReturnStatement(me));
                }
            }
            return block2;
        }
        protected List<string> ExportMethodParameters(IMethod me)
        {
            var list = new List<string>();
            if (!JMeta.IgnoreGenericMethodArguments(me) && me.GetGenericArguments().Count() > 0)
            {
                list.AddRange(me.GetGenericArguments().Select(t => t.Name));
            }
            //if (me.Parameters.Where(t => t.IsOut || t.IsRef).FirstOrDefault() != null)
            //{
            //    throw new CompilerException(me, "Out and ref parameters are not supported");
            //}
            list.AddRange(me.Parameters.Select(t => t.Name));
            return list;
        }

        #endregion

        #region ShouldExport

        protected bool ShouldExportMember(IEntity entity)
        {
            switch (entity.SymbolKind)
            {
                case SymbolKind.Field: return ShouldExportField((IField)entity);
                //case EntityType.ent_constant: return ShouldExportConstant((IConst)entity);
                case SymbolKind.Event: return ShouldExportEvent((IEvent)entity);
                case SymbolKind.Property:
                case SymbolKind.Indexer:
                    var pe = (IProperty)entity;
                    return ShouldExportProperty(pe);

                case SymbolKind.Constructor: return ShouldExportConstructor((IMethod)entity);
                case SymbolKind.Method: return ShouldExportMethod((IMethod)entity);
                case SymbolKind.Operator: return ShouldExportMethod((IMethod)entity);

            }
            return false;
        }

        protected bool ShouldExportProperty(IProperty pe)
        {
            if (pe.IsIndexer)
                return !pe.IsExplicitInterfaceImplementation;
            if (pe.IsExplicitInterfaceImplementation)
                return false;
            var att = pe.GetMetadata<JPropertyAttribute>();
            if (att != null && !att.Export)
                return false;
            if (JMeta.IsNativeField(pe))
                return false;
            //{

            //    if (Sk.InlineFields(pe.GetDeclaringTypeDefinition()))
            //        return true;
            //    return false;
            //}
            return true;
        }

        protected bool ShouldExportEvent(IEvent ev)
        {
            return !ev.IsExplicitInterfaceImplementation;
        }

        protected bool ShouldExportField(IField fe)
        {
            var att = fe.GetMetadata<JFieldAttribute>();
            if (att != null && att._Export != null)
                return att.Export;
            return true;
        }
        protected bool ShouldExportConstant(IField fe)
        {
            var att = fe.GetMetadata<JFieldAttribute>();
            if (att != null && att._Export != null)
                return att.Export;
            return true;
        }

        protected bool ShouldExportMethod(IMethod mde)
        {
            var ret = !mde.IsExplicitInterfaceImplementation;
            var body = mde.GetDeclarationBody();
            //if (body == null || body.IsNull)
            //    ret = false;
            if (mde.IsAutomaticAccessor())
                ret = false;
            if (mde.GetOwner() != null)
                return false;
            var att = mde.GetMetadata<JMethodAttribute>();
            if (att != null && !att.Export)
                return false;
            if (mde.DeclaringTypeDefinition.IsDelegate() && mde.Name == "Invoke")
                return true;
            return ret;
        }

        protected bool ShouldExportConstructor(IMethod ctor)
        {
            var att = ctor.GetMetadata<JMethodAttribute>();
            if (att != null && !att.Export)
                return false;
            if (ctor.IsGenerated(Compiler.Project))// && Sk.OmitDefaultConstructor(ctor.GetDeclaringTypeDefinition())
                return false;
            return true;
        }

        bool ShouldExportMethodBody(IMethod me)
        {
            return (JMeta.IsJsExported(me) && me.GetMethodDeclaration() != null && !me.IsAnonymousMethod());
        }



        #endregion

        #region Field initializers


        public ResolveResult GetCreateInitializer(IMember member)
        {
            if (member is IField) return GetCreateFieldInitializer((IField)member);
            if (member is IEvent) return GetCreateEventInitializer((IEvent)member);
            throw new NotImplementedException("Member type not supported");

            //if (fieldOrProperty.EntityType == EntityType.Field)
            //    return GetCreateFieldInitializer((IField)fieldOrProperty);
            //else if (fieldOrProperty.EntityType == EntityType.Property)
            //{
            //    return GetDefaultValueExpression(((IProperty)fieldOrProperty).ReturnType);
            //}
            //else
            //    throw new NotSupportedException();
        }

        MemberResolveResult AccessSelfFieldOrProperty(IMember fieldOrProperty)
        {
            //if (fieldOrProperty.EntityType == EntityType.Field)
            return fieldOrProperty.AccessSelfForceNonConst();
            //else if (fieldOrProperty.EntityType == EntityType.Property)
            //    return ((IProperty)fieldOrProperty).AccessSelf();
            throw new NotSupportedException();
        }

        public JNode ExportInitializer(IMember fieldOrProperty, BlockStatement ccBlock, bool isGlobal, bool isNative = false)
        {
            var initializer = GetCreateInitializer(fieldOrProperty);
            var init2 = AccessSelfFieldOrProperty(fieldOrProperty).Assign(initializer);

            var jsInit = JsCodeImporter.VisitExpression(init2);
            if (isGlobal)
            {
                //danel HACK
                var st = new JPreUnaryExpression { Operator = "var ", Right = jsInit }.Statement();
                return st;
            }
            else if (isNative)
            {
                var st = jsInit.Statement();
                return st;
            }
            else //clr
            {
                var st = jsInit.Statement();
                return st;
            }
        }

        public ResolveResult GetDefaultValueExpression(IType typeRef)
        {
            if (typeRef.Kind == TypeKind.Struct || typeRef.Kind == TypeKind.Enum)
                return GetValueTypeInitializer(typeRef, Compiler);
            return Cs.Null();
        }

        public ResolveResult GetCreateFieldInitializer(IField fe)
        {
            Expression initializer = null;
            var decl = (FieldDeclaration)fe.GetDeclaration();
            if (decl != null)
            {
                if (decl.Variables.Count > 1)
                    throw new CompilerException(fe, "Multiple field declarations is not supported: " + fe.FullName);
                var variable = decl.Variables.FirstOrDefault();

                initializer = variable.Initializer;
            }
            if (initializer == null || initializer.IsNull)
            {
                return null;
                //var res = GetDefaultValueExpression(fe.Type);
                //if (res == null)
                //{
                //    Log.Warn(fe, "Can't initialize field, initializing to null: " + fe.FullName);
                //    return Cs.Null();
                //}
                //return res;
            }
            else
            {
                var res = initializer.Resolve();
                return res;
            }
        }

        public ResolveResult GetCreateEventInitializer(IEvent fe)
        {
            return Cs.Null();
        }

        NProject Project { get { return Compiler.Project; } }
        public static ResolveResult GetValueTypeInitializer(IType ce, CompilerTool compiler)
        {
            var Project = compiler.Project;
            if (ce.FullName == "System.Nullable")
                return Cs.Null();
            if (ce is ITypeDefinition)
            {
                var def = (ITypeDefinition)ce;
                if (def.KnownTypeCode != KnownTypeCode.None)
                {
                    if (def.KnownTypeCode == KnownTypeCode.Boolean)
                    {
                        return Cs.Value(false, Project);
                    }
                    else if (def.KnownTypeCode == KnownTypeCode.Char)
                    {
                        return Cs.Value('\0', Project);
                    }
                    else if (def.KnownTypeCode == KnownTypeCode.SByte ||
                        def.KnownTypeCode == KnownTypeCode.Int16 ||
                        def.KnownTypeCode == KnownTypeCode.Int32 ||
                        def.KnownTypeCode == KnownTypeCode.Int64 ||
                        def.KnownTypeCode == KnownTypeCode.UInt16 ||
                        def.KnownTypeCode == KnownTypeCode.UInt32 ||
                        def.KnownTypeCode == KnownTypeCode.UInt64 ||
                        def.KnownTypeCode == KnownTypeCode.Byte ||
                        def.KnownTypeCode == KnownTypeCode.Decimal ||
                        def.KnownTypeCode == KnownTypeCode.Double ||
                        def.KnownTypeCode == KnownTypeCode.Single
                        )
                    {
                        return Cs.Value(0, Project);
                    }
                }
            }
            if (ce.Kind == TypeKind.Enum)
            {
                var en = ce;
                var enumMembers = en.GetFields();
                var defaultEnumMember = enumMembers.Where(t => (t.ConstantValue is int) && (int)t.ConstantValue == 0).FirstOrDefault() ?? enumMembers.FirstOrDefault();
                if (defaultEnumMember != null)
                    return defaultEnumMember.AccessSelf();//.Access().Member(c.CreateTypeRef(en), defaultEnumMember);
                else
                    return null;
            }
            else if (ce.GetEntityType().FullName == "System.DateTime")
            {
                var minDateFe = ce.GetFields(t => t.Name == "MinValue").First();
                return minDateFe.AccessSelf();// c.Member(c.Class(c.DateTimeType), minDateFe);
            }
            else
            {
                return Cs.New(ce);
            }
        }

        #endregion

        #region Utils

        protected JExpression Serialize(object obj)
        {
            if (obj == null)
                return J.Null();
            if (obj is JExpression)
            {
                return (JExpression)obj;
            }
            else if (obj is Dictionary<string, object>)
            {
                var obj2 = J.Json();
                var dic = (Dictionary<string, object>)obj;
                dic.ForEach(pair => obj2.Add(pair.Key, Serialize(pair.Value)));
                return obj2;
            }
            else if (obj is IList)
            {
                var list = (IList)obj;
                var array = J.NewJsonArray(list.Cast<object>().Select(Serialize).ToArray());
                return array;
            }
            else if (obj is Enum)
            {
                return J.String(obj.ToString());
            }
            else if (obj is string || obj is bool || obj is int)
            {
                return J.Value(obj);
            }
            else
            {
                var json = J.Json();
                obj.GetType().GetProperties().ForEach(pe =>
                {
                    var value = pe.GetValue(obj, null);
                    if (value != null)
                        json.Add(pe.Name, Serialize(value));
                });
                return json;
            }
        }

        protected List<IMember> GetMembersToExport(ITypeDefinition ce)
        {
            var members = ce.Members.Where(t => ShouldExportMember(t)).ToList();
            var fields = GeneratePropertyFields(ce, true).Concat(GeneratePropertyFields(ce, false)).ToList();
            members = members.Concat(fields).ToList();
            return members;
        }

        public static IMethod CreateStaticCtor(ITypeDefinition ce)
        {
            var cctor = new FakeMethod(SymbolKind.Constructor)
            {
                Name = ".cctor",
                IsStatic = true,
                DeclaringTypeDefinition = ce,
                Region = ce.Region,
                BodyRegion = ce.BodyRegion,
                DeclaringType = ce,
                ParentAssembly = ce.ParentAssembly,
            };
            //var cctor = (IMethod)new DefaultUnresolvedMethod
            //{
            //    Name = ".cctor",
            //    IsStatic = true,
            //    EntityType = EntityType.Constructor,
            //}.CreateResolved(ce.ParentAssembly.Compilation.TypeResolveContext.WithCurrentTypeDefinition(ce));
            return cctor;
        }

        /// <summary>
        /// Returns base type of a type, only if base type is Clr or Prototype
        /// </summary>
        /// <param name="ce"></param>
        /// <returns></returns>
        protected virtual IType GetBaseClassIfValid(ITypeDefinition ce, bool recursive)
        {
            var baseClass = ce.GetBaseType();
            while (baseClass != null)
            {
                if (JMeta.IsClrType(baseClass.GetDefinition()) || (JMeta.IsNativeType(baseClass.GetDefinition()) && !JMeta.IsGlobalType(baseClass.GetDefinition())) || !recursive)
                    return baseClass;
                baseClass = baseClass.GetBaseType();
            }
            return null;
        }

        //InvocationExpression GetConstructorBaseOrThisInvocation(IMethod ctor)
        //{
        //    var ctorNode = (ConstructorDeclaration)ctor.GetDeclaration();
        //    InvocationExpression node = null;
        //    if (ctorNode != null && ctorNode.Initializer != null && !ctorNode.Initializer.IsNull)
        //    {
        //        var xxx = (CSharpInvocationResolveResult)ctorNode.Initializer.Resolve();
        //        //throw new NotImplementedException();
        //        //danel
        //        var baseCtor = xxx.Member;
        //        var id = new IdentifierExpression(baseCtor.Name);
        //        id.SetResolveResult(new MemberResolveResult(null, baseCtor));
        //        node = new InvocationExpression(id);
        //        node.SetResolveResult(xxx);
        //        //{ entity = ctorNode.invoked_method, Target = ctorNode.invoked_method.Access() };
        //        //node.SetResolveResult(
        //        // node.Arguments.AddRange(ctorNode.Initializer.Arguments);
        //    }
        //    else
        //    {
        //        var ce = ctor.GetDeclaringTypeDefinition();
        //        if (Sk.OmitInheritance(ce))
        //            return null;
        //        var baseType = GetBaseClassIfValid(ce, true);
        //        if (baseType != null)
        //        {
        //            var baseCtor = baseType.GetConstructor();
        //            if (baseCtor != null)
        //            {
        //                //danel
        //                //throw new NotImplementedException();
        //                var id = new IdentifierExpression(baseCtor.Name);
        //                id.SetResolveResult(new MemberResolveResult(null, baseCtor));
        //                node = new InvocationExpression(id);// { entity = baseCtor, expression = baseCtor.Access() };
        //                node.SetResolveResult(new CSharpInvocationResolveResult(null, baseCtor, null));

        //            }
        //        }
        //    }
        //    return node;
        //}

        InvocationResolveResult GetConstructorBaseOrThisInvocation2(IMethod ctor)
        {
            var ctorNode = (ConstructorDeclaration)ctor.GetDeclaration();
            InvocationResolveResult node = null;
            if (ctorNode != null && ctorNode.Initializer != null && !ctorNode.Initializer.IsNull)
            {
                var xxx = (CSharpInvocationResolveResult)ctorNode.Initializer.Resolve();
                return xxx;
            }
            else
            {
                var ce = ctor.GetDeclaringTypeDefinition();
                if (JMeta.OmitInheritance(ce))
                    return null;
                var baseType = GetBaseClassIfValid(ce, true);
                if (baseType != null)
                {
                    var baseCtor = baseType.GetConstructors(t => t.Parameters.Count == 0, GetMemberOptions.IgnoreInheritedMembers).Where(t => !t.IsStatic).FirstOrDefault();
                    if (baseCtor != null)
                    {
                        return baseCtor.AccessSelf().Invoke();

                    }
                }
            }
            return node;
        }


        protected List<IMethod> GetAccessorsToExport(IProperty pe)
        {
            var list = new List<IMethod>();
            if (pe.IsAutomaticProperty(Compiler.Project) && !JMeta.IsNativeField(pe))
            {
                list.Add(pe.Getter);
                list.Add(pe.Setter);
            }
            else
            {
                if (pe.Getter != null)
                    list.Add(pe.Getter);
                if (pe.Setter != null)
                    list.Add(pe.Setter);
            }
            list.RemoveAll(t => !JMeta.IsJsExported(t));
            return list;
        }
        protected IMethod GenerateDefaultConstructor(ITypeDefinition ce)
        {
            var compilerGeneratedCtor = ce.GetConstructors(true, false).FirstOrDefault(t => t.GetDeclarationBody() == null); //TODO: where is this phantom ctor coming from??? (dan-el)
            if (compilerGeneratedCtor == null)
            {
                var cePart = ce.Parts.First();
                var me2 = new DefaultUnresolvedMethod(cePart, ".ctor");
                me2.SymbolKind = SymbolKind.Constructor;
                me2.IsSynthetic = true;
                me2.UnresolvedFile = cePart.UnresolvedFile;
                var x = new DefaultResolvedMethod(me2, Compiler.Project.Compilation.TypeResolveContext.WithCurrentTypeDefinition(ce));
                compilerGeneratedCtor = x;
            }
            return compilerGeneratedCtor;
        }

        #endregion

        #region Visit
        JsMode GetJsMode(ITypeDefinition ce)
        {
            var isGlobal = JMeta.IsGlobalType(ce);
            if (isGlobal)
                return JsMode.Global;
            var isNative = JMeta.IsNativeType(ce);
            if (isNative)
                return JsMode.Prototype;
            return JsMode.Clr;
        }

        int VisitDepth;
        const int MaxVisitDepth = 100;
        [DebuggerStepThrough]
        public List<JEntityDeclaration> Visit(IEntity node)
        {
            if (node == null)
                return null;
            VisitDepth++;
            if (VisitDepth > MaxVisitDepth)
                throw new Exception("StackOverflow imminent, depth>" + MaxVisitDepth);
            try
            {
                var node2 = UnsafeVisit(node);
                return node2;
            }
            catch (CompilerException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new CompilerException(node, "Error while processing node", e);
            }
            finally
            {
                VisitDepth--;
            }
        }

        public event Action<IEntity> BeforeVisitEntity;
        //public event Action<IEntity, JNode> AfterVisitEntity;
        public List<JEntityDeclaration> UnsafeVisit(IEntity me)
        {
            if (CompilerConfig.Current.EnableLogging)
            {
                Log.Debug("JsTypeImporter: Visit Entity: " + me.ToString());
            }
            if (BeforeVisitEntity != null)
                BeforeVisitEntity(me);
            List<JEntityDeclaration> node2 = null;
            switch (me.SymbolKind)
            {
                #region switch case
                case SymbolKind.TypeDefinition:
                    node2 = _Visit((ITypeDefinition)me); break;
                case SymbolKind.Event:
                    node2 = _Visit((IEvent)me); break;
                case SymbolKind.Method:
                case SymbolKind.Constructor:
                case SymbolKind.Operator:
                case SymbolKind.Accessor:
                    node2 = _Visit((IMethod)me); break;
                case SymbolKind.Property:
                case SymbolKind.Indexer:
                    node2 = _Visit((IProperty)me); break;
                case SymbolKind.Field:
                    node2 = _Visit((IField)me); break;
                #endregion
            }
            //if (AfterVisitEntity != null)
            //    AfterVisitEntity(me, node2);
            return node2;
        }

        protected List<JEntityDeclaration> VisitToUnit(List<IMember> list)
        {
            var unit = new List<JEntityDeclaration>();
            VisitToUnit(unit, list);
            return unit;
        }
        protected void VisitToUnit(List<JEntityDeclaration> unit, List<IMember> list)
        {
            var nodes = list.Select(Visit).ToList();
            ImportToUnit(unit, nodes);
        }

        protected void ImportToUnit(List<JEntityDeclaration> unit, List<List<JEntityDeclaration>> list)
        {
            foreach (var node in list)
            {
                if (node == null)
                    continue;
                unit.AddRange(node);
            }
        }

        #endregion

        protected virtual JFunction GenerateAutomaticEventAccessor(IEvent ee, bool isRemove)
        {
            if (isRemove)
            {
                var remover = J.Function("value").Add(J.This().Member(ee.Name).Assign(J.Member("$RemoveDelegate").Invoke(J.This().Member(ee.Name), J.Member("value"))).Statement());
                return remover;
            }
            var adder = J.Function("value").Add(J.This().Member(ee.Name).Assign(J.Member("$CombineDelegates").Invoke(J.This().Member(ee.Name), J.Member("value"))).Statement());
            return adder;
        }


        #region Native




        #region Utils
        JJsonNameValue Export(IField pe, bool valuesAsNames)
        {
            if (valuesAsNames)
            {
                return J.JsonNameValue(pe.Name, J.String(pe.Name));
            }
            else
            {
                return J.JsonNameValue(pe.Name, J.Value(pe.ConstantValue));
            }
        }
        JCompilationUnit ExportTypeNamespace(ITypeDefinition ce)
        {
            return new JCompilationUnit { PackageName = ce.GetPackageName() };
        }
        JMemberExpression ExportTypePrefix(ITypeDefinition ce, bool isStatic)
        {
            var me = ce.JAccess();
            //if (!isStatic)
            //    me = me.MemberOrSelf(Sk.GetPrototypeName(ce));
            return me;
        }

        #endregion

        #endregion

    }



}
