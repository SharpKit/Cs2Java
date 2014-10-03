using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.IO;
using JSharp.Utils.CSharp;

namespace JSharp.CSharp
{
    class CodeModelExporter
    {
        public CodeModelExporter()
        {
            ExportXmlDoc = true;
            ExportXmlDocRemarks = true;
            ExportedFiles = new List<string>();
        }
        public Action<Class> ExportingClass { get; set; }
        public Assembly Assembly { get; set; }
        HashSet<string> keywords = new HashSet<string>
        {
            "namespace","using",
            "object", "bool", "int", "short",
            "delegate", "event", "class", "struct", "interface",
            "default", "out",
            "is", "in", "unsafe", "as",
            "switch",
            "true", "false",
            "params","lock","checked","fixed","private", "protected", "public", "internal", "static",
            "return", "override","virtual", "sealed","string","this", "base", "ref",
            "typeof",
        };

        public string OutputDir { get; set; }
        #region Write

        private void Write(string s, params object[] args)
        {
            Writer.Write(s, args);
        }
        private void Write(string s)
        {
            Writer.Write(s);
        }
        private void WriteLine()
        {
            Writer.WriteLine();
        }
        private void WriteLine(string s, params object[] args)
        {
            Writer.WriteLine(s, args);
        }
        private void WriteLine(string s)
        {
            Writer.WriteLine(s);
        }
        private void BeginBlock()
        {
            WriteLine("{");
            Writer.Indent++;
        }
        private void EndBlock()
        {
            Writer.Indent--;
            WriteLine("}");
        }
        void WriteComma()
        {
            Write(", ");
        }

        #endregion

        Stack<Class> ContextClasses = new Stack<Class>();
        Class ContextClass
        {
            get
            {
                return ContextClasses.FirstOrDefault();
            }
            set
            {
                if (value == null)
                    ContextClasses.Pop();
                else
                    ContextClasses.Push(value);
            }
        }

        Class ObjectClass;
        //string Class(Class typeRef, Class context = null, bool forceJavaObject = false)
        //{
        //    return Class(typeRef, context, forceJavaObject);
        //    //if (context == null)
        //    //    context = ContextClass;
        //    //if (typeRef.IsArray)
        //    //{
        //    //    return Class(typeRef.ArrayItemType) + "[]";
        //    //}
        //    //if (typeRef.IsWildcardType)
        //    //{
        //    //    if (typeRef == null)
        //    //        return Class(typeRef.UpperBounds[0]);  //List.addAll(Collection<? extends E>)
        //    //}
        //    //var name = typeRef.Name;
        //    //if (typeRef != null)
        //    //{
        //    //    if (typeRef == ObjectClass && !forceJavaObject)
        //    //        return "global::System.Object";
        //    //    name = typeRef.FullName;
        //    //}
        //    //var tn = new TypeName(name);
        //    //name = tn.Name;
        //    //if (typeRef.GenericArguments.IsNotNullOrEmpty())
        //    //{
        //    //    if (name.Contains("`"))
        //    //        name = name.Substring(0, typeRef.Name.IndexOf("`"));
        //    //    name += typeRef.GenericArguments.StringConcat(t => Class(t), "<", ",", ">");
        //    //}
        //    //if (tn.DeclaringTypeName != null)
        //    //{
        //    //    tn = tn.DeclaringTypeName;
        //    //    name = tn.Name + "." + name;
        //    //}
        //    //if (tn.Namespace.IsNotNullOrEmpty())
        //    //{
        //    //    var addNs = false;
        //    //    if (context != null && !context.Namespace.StartsWith(tn.Namespace))
        //    //        addNs = true;
        //    //    else if (Assembly != null && GetClassesByName(name).Count > 1)
        //    //        addNs = true;
        //    //    if (addNs)
        //    //        name = "global::" + Identifiers(tn.Namespace, ".") + "." + name;
        //    //}
        //    //return name;
        //}

        string Class(Class ce, Class context = null, bool forceJavaObject = false)
        {
            if (ce == null)
                return "void";
            if (context == null)
                context = ContextClass;
            if (ce.IsArray)
            {
                return Class(ce.ArrayElementType) + "[]";
            }
            //if (ce.IsWildcardType)
            //{
            //    if (ce == null)
            //        return Class(ce.UpperBounds[0]);  //List.addAll(Collection<? extends E>)
            //}
            var name = ce.Name;
            if (ce == ObjectClass && !forceJavaObject)
                return "global::System.Object";
            name = ce.FullName;
            var tn = new TypeName(name);
            name = tn.Name;
            
            if (!ce.GenericArguments.IsEmpty())
            {
                if (name.Contains("`"))
                    name = name.Substring(0, ce.Name.IndexOf("`"));
                name += ce.GenericArguments.StringConcat(t => Class(t), "<", ",", ">");
            }
            if (tn.DeclaringTypeName != null)
            {
                tn = tn.DeclaringTypeName;
                name = tn.Name + "." + name;
            }
            if (tn.Namespace.IsNotNullOrEmpty())
            {
                var addNs = false;
                if (context != null && !context.Namespace.StartsWith(tn.Namespace))
                    addNs = true;
                else if (Assembly != null && GetClassesByName(name).Count > 1)
                    addNs = true;
                if (addNs)
                    name = "global::" + Identifiers(tn.Namespace, ".") + "." + name;
            }
            return name;
        }

        Dictionary<string, List<Class>> AllClassesByName;

        List<Class> EmptyList = new List<Class>();
        List<Class> GetClassesByName(string name)
        {
            if (AllClassesByName == null)
                Index();
            return AllClassesByName.TryGetValue(name) ?? EmptyList;
        }
        void Index()
        {
            AllClassesByName = new Dictionary<string, List<Class>>();
            foreach (var ce in Assembly.ClassesAndNested)
            {
                var list = AllClassesByName.TryGetValue(ce.Name);
                if (list == null)
                {
                    list = new List<Class>();
                    AllClassesByName[ce.Name] = list;
                }
                list.Add(ce);
            }
        }

        string Identifier(string s)
        {
            if (keywords.Contains(s))
                return "@" + s;
            s = s.Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("-", "_");
            return s;
        }
        string Identifiers(string s, string separator)
        {
            var tokens = s.Split(new string[] { separator }, StringSplitOptions.None);
            return tokens.Select(Identifier).StringConcat(separator);
        }

        void BeginRegion(string name)
        {
            WriteLine("#region {0}", name);
        }
        void EndRegion()
        {
            WriteLine("#endregion");
        }

        public AssemblyContext Context { get; set; }
        public void Export()
        {
            ObjectClass = Context.GetClass("java.lang.Object");
            //BeginFile("Assembly.cs");
            //ExportHeader();
            Assembly.Attributes.ForEach(Export);
            Export(Assembly.Classes.OrderBy(t => t.FullName).ToList());
            //asm.Classes.ForEach(Export);
            EndFile();
        }

        public List<string> ExportedFiles { get; set; }

        public bool SingleFile { get; set; }

        void BeginFile(string filename)
        {
            //Console.WriteLine(filename);
            filename = Path.Combine(OutputDir, filename);
            EndFile();
            //if (File.Exists(filename))
            //    throw new Exception();
            var dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Writer = new IndentedTextWriter(new StreamWriter(filename));
            ExportedFiles.Add(filename);
        }
        void EndFile()
        {
            if (Writer == null)
                return;
            Writer.Flush();
            Writer.Close();
            Writer = null;
        }


        public void Export(List<Class> list)
        {
            if (SingleFile)
            {
                var byNamespace = list.GroupBy(t => t.Namespace).OrderBy(t => t.Key).ToList();
                foreach (var nsGroup in byNamespace)
                {
                    WriteLine("#region {0}", nsGroup.Key);
                    Namespace(nsGroup.Key, delegate
                    {
                        nsGroup.ForEach(ce =>
                        {
                            WriteLine("#region {0}", ce.Name);
                            Export(ce);
                            WriteLine("#endregion");
                        });
                    });
                    WriteLine("#endregion");
                }
            }
            else
            {
                EndFile();
                var classesByFilename = list.GroupBy(GetFilename).ToList();
                foreach (var group in classesByFilename)
                {
                    //Console.WriteLine(filename);
                    BeginFile(group.Key);
                    group.ForEach(Export);
                    EndFile();
                }
            }
            if (CreateCsProj)
                CsProjectHelper.SaveCsProject(ExportedFiles, Path.Combine(OutputDir, new DirectoryInfo(OutputDir).Name + ".csproj"));
        }
        public bool CreateCsProj { get; set; }

        private string GetFilename(Class ce)
        {
            var ns = ce.Namespace;
            if (RootNamespace != null)
                ns = ns.Replace(RootNamespace, "");
            var tokens = ns.Split('.').ToList();
            tokens.Add(ce.Name + ".cs");
            var filename = Path.Combine(tokens.ToArray());
            return filename;
        }

        public string RootNamespace { get; set; }
        public void ExportHeader()
        {
            WriteLine("//***************************************************");
            WriteLine("//* This file was generated by JSharp");
            WriteLine("//***************************************************");
            Assembly.Usings.ForEach(t => WriteLine("using {0};", t));
        }
        public void Export(Class ce)
        {
            if (ExportingClass != null)
                ExportingClass(ce);
            if (!SingleFile && ce.DeclaringClass == null)
            {
                ExportHeader();
                BeginNamespace(ce.Namespace);
            }
            ContextClass = ce;
            ExportXmlDoc2(ce);
            ce.Attributes.ForEach(Export);
            ExportVisibility(ce);
            Write("{0}{1}", ce.IsNew.If("new "), ce.IsAbstract.IfTrue("abstract "));
            Write("partial {0} {1}", ce.IsInterface.If("interface", "class"), Identifier(ce.Name));

            if (ce.GenericArguments.IsNotNullOrEmpty())
                Write("{0}", ce.GenericArguments.StringConcat(t => t.Name, "<", ",", ">"));
            var bases = new List<string>();
            if (ce.BaseClass != null)
            {
                bases.Add(Class(ce.BaseClass, null, true));
            }
            bases.AddRange(ce.Interfaces.Select(t => Class(t)));
            if (bases.Count > 0)
            {
                Write(" : ");
                bases.ForEachJoin(t => Write("{0}", t), WriteComma);
            }
            WriteLine();
            BeginBlock();
            ce.Members.ForEach(Export);
            EndBlock();
            ContextClass = null;
            if (!SingleFile && ce.DeclaringClass == null)
                EndNamespace();
        }
        private void Export(Method me)
        {
            ExportXmlDoc2(me);
            me.Attributes.ForEach(Export);
            if (me.IsConstructor)
            {
                Write("public {0}(", Identifier(me.DeclaringClass.Name));
                me.Parameters.ForEachJoin(Export, WriteComma);
                Write(")");
                if (me.DeclaringClass.BaseClass != null)
                {
                    var baseCe = GetTypeDef(me.DeclaringClass.BaseClass);
                    if (baseCe != null)
                    {
                        var baseCtors = baseCe.Members.OfType<Method>().Where(t => t.IsConstructor).ToList();
                        if (baseCtors.Count > 0 && baseCtors.Where(t => t.Parameters.Count == 0).FirstOrDefault() == null)
                        {
                            Write(" : base(");
                            baseCtors.First().Parameters.ForEachJoin(p => Write(Default(p.Type)), WriteComma);
                            Write(")");
                        }
                    }
                }
                WriteLine("{}");
            }
            else
            {
                if (me.Name == "containsAll" && me.DeclaringClass.Name == "SynchronousQueue")
                {
                }
                if (me.ExplicitImplementationOfInterfaceMethod == null)
                    ExportVisibility(me);
                if (me.IsStatic)
                    Write("static ");
                if (me.IsVirtual && !me.DeclaringClass.IsInterface)
                    Write("virtual ");
                if (me.IsOverride)
                    Write("override ");
                if (me.IsNew)
                    Write("new ");
                if (me.Type == null)
                    Write("void");
                else
                    Write(Class(me.Type));
                Write(" ");
                if (me.ExplicitImplementationOfInterfaceMethod != null)
                {
                    var iface = me.ExplicitImplementationOfInterfaceMethod.DeclaringClass;
                    Write(Class(iface));
                    Write(".");
                }
                Write(Identifier(me.Name));
                if (me.GenericArguments.IsNotNullOrEmpty())
                {
                    Write("<");
                    me.GenericArguments.ForEachJoin(t => Write(Class(t)), WriteComma);
                    Write(">");
                }

                Write("(");
                me.Parameters.ForEachJoin(Export, WriteComma);
                Write(")");
                if (me.DeclaringClass.IsInterface)
                {
                    WriteLine(";");
                }
                else
                {
                    Write("{");
                    if (!me.Type.IsNullOrVoid())
                    {
                        Write("return {0};", Default(me.Type));
                    }
                    WriteLine("}");
                }
            }
        }
        string Default(Class tr)
        {
            return String.Format("default({0})", Class(tr));
        }

        private Class GetTypeDef(Class classRef)
        {
            return classRef;//TODO:
        }


        private void Export(Parameter prm)
        {
            Write("{0} {1}", Class(prm.Type), Identifier(prm.Name));
            if (prm.IsOptional)
            {
                if (prm.Type.Name == "bool")
                    Write("=false");
                else
                    Write("=null");
            }
        }

        void Export(Member el)
        {
            if (el is Method)
                Export((Method)el);
            else if (el is Property)
                Export((Property)el);
            else if (el is Class)
                Export((Class)el);
            else if (el is Event)
                Export((Event)el);
            else if (el is Field)
                Export((Field)el);
            else
                throw new Exception();
        }

        private void Export(Field pe)
        {
            ExportXmlDoc2(pe);
            pe.Attributes.ForEach(Export);
            Write("public {0}{1}{2}{3}{4} {5}", pe.IsStatic.If("static "), pe.IsVirtual.If("virtual "), pe.IsOverride.If("override "), pe.IsNew.If("new "), Class(pe.Type), Identifier(pe.Name));

            if (pe.Initializer.IsNotNullOrEmpty())
                Write("={0}", pe.Initializer);
            WriteLine(";");
        }
        private void Export(Property pe)
        {
            ExportXmlDoc2(pe);
            pe.Attributes.ForEach(Export);
            if (pe.ExplicitImplementationOfInterfaceProperty == null)
                ExportVisibility(pe);
            Write("{0}{1}{2}{3}{4} ", pe.IsStatic.If("static "), pe.IsVirtual.If("virtual "), pe.IsOverride.If("override "), pe.IsNew.If("new "), Class(pe.Type));
            if (pe.ExplicitImplementationOfInterfaceProperty != null)
            {
                var iface = pe.ExplicitImplementationOfInterfaceProperty.DeclaringClass;
                Write(Class(iface));
                Write(".");
            }
            else
            {
                Write(" ");
            }
            Write(Identifier(pe.Name));

            Write(" { get");

            if (pe.IsReadOnly)
            {
                if (pe.DeclaringClass.IsInterface)
                {
                    Write(";");
                }
                else
                {
                    if (pe.ExplicitImplementationOfInterfaceProperty != null)
                        Write("{{ return {0}; }}", Default(pe.Type));
                    else if (!pe.IsPrivate)
                        Write("; private set;");
                    else
                        Write("; set;");
                }
            }
            else
            {
                Write("; set;");
            }
            WriteLine("}");
        }

        private void ExportVisibility(Member el)
        {
            var ce = el.DeclaringClass;
            if (ce != null && ce.IsInterface)
                return;
            if (el.IsProtected)
                Write("protected ");
            else if (el.IsPrivate)
                Write("private ");
            else if (el.IsInternal)
                Write("internal ");
            else
                Write("public ");
        }
        private void Export(Event pe)
        {
            ExportXmlDoc2(pe);
            pe.Attributes.ForEach(Export);
            ExportVisibility(pe);
            WriteLine("event {0}{1}{2}{3}{4} {5};", pe.IsStatic.If("static "), pe.IsVirtual.If("virtual "), pe.IsOverride.If("override "), pe.IsNew.If("new "), Class(pe.Type), Identifier(pe.Name));
        }

        void BeginNamespace(string ns)
        {
            WriteLine("namespace {0}", Identifiers(ns, "."));
            BeginBlock();
        }
        void EndNamespace()
        {
            EndBlock();
        }
        private void Namespace(string ns, Action action)
        {
            if (ns.IsNotNullOrEmpty())
            {
                BeginNamespace(ns);
            }
            action();
            if (ns.IsNotNullOrEmpty())
            {
                EndNamespace();
            }
        }
        #region XmlDoc


        public bool ExportXmlDoc { get; set; }
        public bool ExportXmlDocRemarks { get; set; }

        void WriteXmlDoc(string ss)
        {
            ss.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ForEach(t =>
            {
                var s = t.Trim();
                if (s.IsNotNullOrEmpty())
                    WriteLine("/// " + s);
            });

        }
        void ExportXmlDoc2(Member me)
        {
            if (ExportXmlDoc)
            {
                if (me.Summary != null)
                {
                    WriteLine("/// <summary>");
                    WriteXmlDoc(me.Summary);
                    //WriteLine("/// " + me.Summary.Trim());
                    WriteLine("/// </summary>");
                }
                if (ExportXmlDocRemarks && me.Remarks != null)
                {
                    //WriteLine("/// <remarks>");
                    WriteXmlDoc(me.Remarks);
                    //WriteLine("/// </remarks>");
                }
            }
        }

        #endregion
        void Export(Attribute att)
        {
            Write("[{0}", att.Name);
            if (att.Parameters.Count > 0 || att.NamedParamters.Count > 0)
            {
                Write("(");
                att.Parameters.ForEachJoin(Write, WriteComma);
                if (att.Parameters.Count > 0 && att.NamedParamters.Count > 0)
                    Write(", ");
                att.NamedParamters.ForEachJoin(t => Write("{0}={1}", t.Key, t.Value), WriteComma);
                Write(")");
            }
            WriteLine("]");
        }

        public IndentedTextWriter Writer { get; set; }

    }
}
