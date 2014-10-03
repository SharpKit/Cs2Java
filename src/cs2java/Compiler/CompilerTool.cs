using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JSharp.Java;
using ICSharpCode.NRefactory.TypeSystem;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Globalization;
using JSharp.Java.Ast;
using JSharp.Compiler.SourceMapping;
using System.Collections;
using System.Xml.Linq;
using JSharp.Utils.Http;
using ICSharpCode.NRefactory.CSharp;

namespace JSharp.Compiler
{
    class CompilerTool : ICompiler
    {
        public CompilerTool()
        {
            EmbeddedResourceFiles = new List<string>();
            CompilerConfig.LoadCurrent();
        }

        #region Properties
        //[Obsolete]
        //public static CompilerTool Current { get; set; }
        public static bool IsProductActivated { get; private set; }
        public CompilerToolArgs Args { get; set; }
        public List<JFileWrapper> SkJsFiles { get; set; }
        public CompilerLogger Log { get; set; }
        public NProject2 Project { get; set; }
        public List<string> Defines { get; set; }
        public CsExternalMetadata CsExternalMetadata { get; set; }
        public string SkcVersion { get; set; }
        public string[] CommandLineArguments { get; set; }
        public bool Debug { get; set; }


        public JStatement CombineDelegatesSupportStatement { get; set; }
        public JStatement RemoveDelegateSupportStatement { get; set; }
        public JStatement CreateMulticastDelegateFunctionSupportStatement { get; set; }
        public SourceMappingGenerator SourceMapsGenerator { get; set; }
        public JFileWrapper CodeInjectionFile { get; set; }
        public List<string> EmbeddedResourceFiles { get; set; }
        public string VersionKey { get; set; }

        #endregion

        #region Fields

        JModelImporter JsModelImporter;


        #endregion

        public int Run()
        {
            var x = InternalRun();
            if (BeforeExit != null)
                BeforeExit();
            return x;
        }
        int InternalRun()
        {
            Log = new CompilerLogger { Console = new QueuedConsole() };
            Log.Init();
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                SkcVersion = typeof(CompilerTool).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
                VersionKey = this.SkcVersion + "||" + File.GetLastWriteTime(Process.GetCurrentProcess().MainModule.FileName).ToBinary();


                if (Args == null)
                {
                    if (Debug)
                        Time(SaveLastInputArgs);
                    WriteArgs();
                    Time(ParseArgs);
                }
                if (Args.CheckForNewVersion)
                {
                    Time(CheckForNewVersion);
                    if (Args.Files.IsNullOrEmpty())
                        return 0;
                }

                if (Help())
                    return 0;
                if (Args.Service)
                {
                    RunInServiceMode();
                    return 0;
                }
                Time(CalculateMissingArgs);
                if (Time2(CheckManifest))
                    return 0;
                Time(LoadPlugins);

                Time(ParseCs);
                Time(ApplyExternalMetadata);
                Time(ConvertCsToJs);
                Time(Cleanup);
                //Time(MergeJsFiles);
                ////Time(ValidateUnits));
                //Time(InjectJsCode);
                //Time(OptimizeJsFiles);
                Time(SaveJsFiles);
                //Time(EmbedResources);
                //Time(GenerateSourceMappings);
                if (Log.Items.Where(t => t.Type == CompilerLogItemType.Error).FirstOrDefault() != null)
                    return -1;
                Time(SaveNewManifest);
                return 0;
            }
            catch (Exception e)
            {
                Log.Log(e);
                return -1;
            }
        }

        public void RunInServiceMode()
        {
            var server = new JsonServer { Service = new CompilerService() };
            server.Run();
        }

        bool Help()
        {
            if (Args.Help)
            {
                CompilerToolArgs.GenerateHelp(System.Console.Out);
                return true;
            }
            return false;
        }

        void CheckForNewVersion()
        {
            Console.WriteLine("CheckForNewVersion is not available");
        }

        void LoadPlugins()
        {
            if (Args.Plugins.IsNullOrEmpty())
                return;
            foreach (var plugin in Args.Plugins)
            {
                try
                {
                    Log.WriteLine("Loading plugin: " + plugin);
                    var type = Type.GetType(plugin);
                    Log.WriteLine("Found plugin: " + plugin);
                    var obj = Activator.CreateInstance(type, true);
                    Log.WriteLine("Created plugin: " + plugin);
                    Log.WriteLine("Started: Initialize plugin" + plugin);
                    var plugin2 = (ICompilerPlugin)obj;
                    plugin2.Init(this);
                    Log.WriteLine("Finished: Initialize plugin " + plugin);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to load plugin: " + plugin, e);
                }
            }
        }

        #region Manifest

        void SaveNewManifest()
        {
            TriggerEvent(BeforeSaveNewManifest);
            //TODO: CreateManifest(FileMerger.ExternalJsFiles.Select(t => t.JsFile.Filename).Concat(EmbeddedResourceFiles).ToList()).SaveToFile(Args.ManifestFile);
            TriggerEvent(AfterSaveNewManifest);
        }

        Manifest CreateManifest(List<string> externalFiles)
        {
            return new ManifestHelper { Args = Args, Log = Log, SkcVersion = SkcVersion, SkcFile = typeof(CompilerTool).Assembly.Location, ExternalFiles = externalFiles }.CreateManifest();
        }

        #endregion

        #region ParseArgs

        bool CheckManifest()
        {
            if (Args.rebuild.GetValueOrDefault())
                return false;
            if (!File.Exists(Args.ManifestFile))
                return false;
            var prev = Manifest.LoadFromFile(Args.ManifestFile);
            var current = CreateManifest(prev.ExternalFiles.Select(t => t.Filename).ToList());
            Trace.TraceInformation("[{0}] Comparing manifests", DateTime.Now);
            var diff = current.GetManifestDiff(prev);
            if (diff.AreManifestsEqual)
            {
                Log.WriteLine("Code was unmodified - build skipped");
                return true;
            }
            else
            {
                File.Delete(Args.ManifestFile);
                Log.WriteLine("Reasons for rebuild:\n" + diff.ToString());
            }
            return false;
        }

        void ParseArgs()
        {
            Args = CompilerToolArgs.Parse(CommandLineArguments);
        }

        void WriteArgs()
        {
            Log.WriteLine(Process.GetCurrentProcess().MainModule.FileName + " " + ArgsToString());
        }

        void SaveLastInputArgs()
        {
            var file = @"C:\Projects\c2j\src\cs2java\prms.txt";
            if (File.Exists(file))
            {
                var s = ArgsToString();
                File.WriteAllText(file, s);
            }
        }

        string ArgsToString()
        {
            var sb = new StringBuilder();
            CommandLineArguments.ForEachJoin(arg =>
            {
                if (arg.StartsWith("@"))
                    sb.Append(File.ReadAllText(arg.Substring(1)));
                else
                    sb.Append(arg);
            }, () => sb.Append(" "));
            var s = sb.ToString();
            if (!s.Contains("/dir"))
                s = String.Format("/dir:\"{0}\" ", Directory.GetCurrentDirectory()) + s;
            return s;
        }

        void CalculateMissingArgs()
        {
            if (Args.CurrentDirectory.IsNotNullOrEmpty())
                Directory.SetCurrentDirectory(Args.CurrentDirectory);
            if (Args.Output == null)
                Args.Output = "output.js";
            if (Args.ManifestFile == null)
                Args.ManifestFile = Path.Combine(Path.GetDirectoryName(Args.Output), Args.AssemblyName + ".skccache");
            if (Args.CodeAnalysisFile == null)
                Args.CodeAnalysisFile = Path.Combine(Path.GetDirectoryName(Args.Output), Args.AssemblyName + ".CodeAnalysis");
            if (Args.SecurityAnalysisFile == null)
                Args.SecurityAnalysisFile = Path.Combine(Path.GetDirectoryName(Args.Output), Args.AssemblyName + ".securityanalysis");

            Defines = Args.define != null ? Args.define.Split(';').ToList() : new List<string>();

        }

        #endregion


        int CountedLines;
        void JsWriter_StaticWriteLine()
        {
            CountedLines++;
            //if (CountedLines > 5000)
            //    throw new Exception(String.Format("5000 Lines of code limit reached"));
        }

        void ParseCs()
        {

            TriggerEvent(BeforeParseCs);
            _CustomAttributeProvider = new CustomAttributeProvider();
            Project = new NProject2
            {
                SourceFiles = Args.Files,
                Defines = Defines,
                References = Args.References,
                TargetFrameworkVersion = Args.TargetFrameworkVersion,
                AssemblyName = Args.AssemblyName,
                Compiler = this,
            };
            Project.Parse();
            var asm = Project.MainAssembly;
            if (asm != null && asm.AssemblyName == null)
            {
                throw new NotImplementedException();
                //asm.AssemblyName = Args.AssemblyName;
            }

            TriggerEvent(AfterParseCs);
        }

        void ApplyExternalMetadata()
        {

            TriggerEvent(BeforeApplyExternalMetadata);
            CsExternalMetadata = new CsExternalMetadata { Project = Project, Log = Log };
            CsExternalMetadata.Process();

            TriggerEvent(AfterApplyExternalMetadata);
        }

        void ConvertCsToJs()
        {

            TriggerEvent(BeforeConvertCsToJs);
            JsModelImporter = new JModelImporter
            {
                Compiler = this,
                Project = Project,
                Log = Log,
                ExternalMetadata = CsExternalMetadata,
                AssemblyName = Args.AssemblyName,
                BeforeExportTypes = byFile =>
                    {
                        var list = new List<ITypeDefinition>();
                        foreach (var list2 in byFile.Values)
                        {
                            list.AddRange(list2);
                        }
                        var skFiles = Project.GetNFiles(list);
                        Project.ApplyNavigator(skFiles);
                    }
            };
            JsModelImporter.ConfigureJsTypeImporter += new Action<JTypeImporter>(JsModelImporter_ConfigureJsTypeImporter);
            var att = JMeta.GetJsExportAttribute(Project.Compilation.MainAssembly);
            if (att != null)
            {
                JsModelImporter.ExportComments = att.ExportComments;
            }


            JsModelImporter.Process();
            SkJsFiles = new List<JFileWrapper>();
            foreach (var group in JsModelImporter.ExportedTypes.GroupBy(t => GetJFilename(t.Type)))
            {
                var file = new JFileWrapper { JsFile = new JFile { Filename = group.Key }, Compiler=this, };
                foreach (var et in group)
                {
                    var unit = new JCompilationUnit { PackageName = et.Type.GetPackageName(), Declarations = { et.ClassDeclaration } };
                    file.JsFile.Units.Add(unit);
                    SkJsFiles.Add(file);
                }
            }

            TriggerEvent(AfterConvertCsToJs);
        }

        void JsModelImporter_ConfigureJsTypeImporter(JTypeImporter obj)
        {
            obj.BeforeVisitEntity += me =>
            {
                if (BeforeConvertCsToJsEntity != null)
                    BeforeConvertCsToJsEntity(me);
            };
            //obj.AfterVisitEntity += (me, node) =>
            //{
            //    if (AfterConvertCsToJsEntity != null)
            //        AfterConvertCsToJsEntity(me, node);
            //};
            obj.JsCodeImporter.BeforeConvertCsToJsAstNode += node =>
            {
                if (BeforeConvertCsToJsAstNode != null)
                    BeforeConvertCsToJsAstNode(node);
            };
            obj.JsCodeImporter.AfterConvertCsToJsAstNode += (node, node2) =>
            {
                if (AfterConvertCsToJsAstNode != null)
                    AfterConvertCsToJsAstNode(node, node2);
            };
            obj.JsCodeImporter.BeforeConvertCsToJsResolveResult += res =>
            {
                if (BeforeConvertCsToJsResolveResult != null)
                    BeforeConvertCsToJsResolveResult(res);
            };
            obj.JsCodeImporter.AfterConvertCsToJsResolveResult += (res, node) =>
            {
                if (AfterConvertCsToJsResolveResult != null)
                    AfterConvertCsToJsResolveResult(res, node);
            };
        }


        void Cleanup()
        {
            foreach (var file in SkJsFiles)
                file.JsFile.Units.OfType<JCompilationUnit>().ForEach(Cleanup);

        }

        void ApplyJsMinifyAndSourceMap()
        {
            var att = Project.MainAssembly.GetMetadata<JExportAttribute>();
            if (att != null)
            {
                if (att.GenerateSourceMaps)
                    SkJsFiles.ForEach(t => t.GenerateSourceMap = true);
            }
        }


        #region ValidateUnits
        void ValidateUnits()
        {
            SkJsFiles.Select(t => t.JsFile).ToList().ForEach(ValidateUnits);
        }
        void ValidateUnits(JFile file)
        {
            file.Units.ForEach(ValidateUnit);
        }
        void ValidateUnit(JNode node)
        {
            if (node == null)
                throw new NotImplementedException();
            var children = node.Children().ToList();
            children.ForEach(ValidateUnit);
        }

        #endregion

        #region Optimize

        //void OptimizeClrJsTypesArrayVerification()
        //{
        //    if (JsModelImporter == null || JsModelImporter.Clr2Export == null)
        //        return;
        //    var st = JsModelImporter.Clr2Export.VerifyJsTypesArrayStatement;
        //    if (st == null)
        //        return;
        //    if (SkJsFiles.IsNullOrEmpty())
        //        return;
        //    foreach (var file in SkJsFiles)
        //    {
        //        if (file.JsFile == null || file.JsFile.Units == null)
        //            continue;
        //        foreach (var unit in file.JsFile.Units)
        //        {
        //            if (unit.Statements == null)
        //                continue;
        //            unit.Statements.RemoveDoubles(st);
        //        }
        //    }
        //}

        //void OptimizeJsFiles()
        //{

        //    TriggerEvent(BeforeOptimizeJsFiles);
        //    OptimizeClrJsTypesArrayVerification();
        //    OptimizeNamespaceVerification();

        //    TriggerEvent(AfterOptimizeJsFiles);
        //}

        void OptimizeNamespaceVerification()
        {
            if (SkJsFiles.IsNullOrEmpty())
                return;
            foreach (var file in SkJsFiles)
            {
                if (file.JsFile == null || file.JsFile.Units.IsNullOrEmpty())
                    continue;
                foreach (var unit in file.JsFile.Units)
                {
                    OptimizeNamespaceVerification(unit);
                }
            }
        }

        string GetNamespaceVerification(JStatement st)
        {
            var ex = st.Ex(false);
            if (ex != null)
                return ex.NamespaceVerification;
            return null;
        }

        void OptimizeNamespaceVerification(JUnit unit)
        {
            if (unit.Statements.IsNullOrEmpty())
                return;
            unit.Statements.RemoveDoublesByKey(t => GetNamespaceVerification(t));

        }
        #endregion

        void SaveJsFiles()
        {
            TriggerEvent(BeforeSaveJsFiles);

            foreach (var file in SkJsFiles)
                file.Save();
            TriggerEvent(AfterSaveJsFiles);
        }

        private void Cleanup(JCompilationUnit unit)
        {
            FixMultiStatements(unit);
            OrganizeImports(unit);
        }

        private void FixMultiStatements(JCompilationUnit unit)
        {
            JNode parent = unit;
            FixMultiStatements(parent);
        }

        private void FixMultiStatements(JNode parent)
        {
            foreach (var node in parent.Children().ToList())
            {
                if (node is JMultiStatementExpression)
                {
                    FixMultiStatements((JMultiStatementExpression)node, parent);
                }
                FixMultiStatements(node);
            }
        }

        private void FixMultiStatements(JMultiStatementExpression node, JNode parent)
        {
            var list = new List<JStatement>();
            foreach (var st in node.Statements)
            {
                if (st is JExpressionStatement)
                {
                    var stExp = (JExpressionStatement)st;
                    if (stExp.Expression is JInvocationExpression)
                    {
                        var expInvoke = (JInvocationExpression)stExp.Expression;
                        if (expInvoke.Arguments.Count == 1 && expInvoke.Arguments[0] is JMultiStatementExpression)
                        {
                            var mse = (JMultiStatementExpression)expInvoke.Arguments[0];
                            var stRet = (JReturnStatement)mse.Statements.Last();
                            list.AddRange(mse.Statements.Take(mse.Statements.Count - 1));
                            expInvoke.Arguments[0] = stRet.Expression;
                            continue;
                        }
                    }
                }
                list.Add(st);
            }
            node.Statements = list;
            var node2 = J.CreateActionOrFunc(new JBlock { Statements = node.Statements.ToList() }, null, node.Type, Project).Member("invoke").Invoke();
            ReplaceChild(parent, node, node2);
        }

        private void ReplaceChild(JNode parent, JNode oldNode, JNode newNode)
        {
            foreach (var pe in parent.GetType().GetProperties())
            {
                var value = pe.GetValue(parent, null);
                if (value is IList)
                {
                    var list = (IList)value;
                    var index = list.IndexOf(oldNode);
                    if (index >= 0)
                    {
                        list[index] = newNode;
                        return;
                    }
                }
                else if (value == oldNode)
                {
                    pe.SetValue(parent, newNode, null);
                    return;
                }
            }
            throw new NotImplementedException();
        }

        private void OrganizeImports(JCompilationUnit unit)
        {
            var imports = new HashSet<ITypeDefinition>();
            foreach (var ce in unit.Declarations)
            {
                foreach (var node in unit.Descendants<JMemberExpression>().ToList())
                {
                    var prevMember = node.PreviousMember as JMemberExpression;
                    if (prevMember != null && prevMember.TypeRef != null)
                    {
                        //cleanup type name when invoking static method on same type
                        if (node.TypeRef != null)
                            continue;
                        var typeDef2 = ((JMemberExpression)node.PreviousMember).TypeRef.GetDefinition();
                        if (typeDef2 == null)
                            continue;
                        if (typeDef2 == ce.TypeDefinition)
                            node.PreviousMember = null;
                        continue;
                    }
                    else if (node.TypeRef != null)
                    {
                        //cleanup type name and add to import
                        var typeDef = node.TypeRef.GetDefinition();
                        if (typeDef == null)
                            continue;
                        node.PreviousMember = null;
                        if (typeDef.IsKnownType(KnownTypeCode.Object) || typeDef.Namespace == "java.lang")
                            continue;
                        imports.Add(typeDef);
                    }
                }
            }
            var x = imports.Select(t => t.JAccessStatic()).Where(t => !t.IsWildcardArg()).ToList();
            var xx = x.Select(t => new { Type = t, Code = t.ToJs() }).ToList();
            xx = xx.OrderBy(t => t.Code).Distinct(t => t.Code).ToList();
            foreach (var node in xx)
            {
                unit.Imports.Add(new JImport { Type = node.Type });
            }

        }



        private string GetJFilename(ITypeDefinition ce)
        {
            var outputDir = "out";
            var att = JMeta.GetJsExportAttribute(Project.Compilation.MainAssembly);
            if (att != null && att.OutputDir != null)
                outputDir = att.OutputDir;
            return Path.Combine(outputDir, JNaming.JName2(ce).RemoveGenericArgs().ToJs().Replace(".", "\\") + ".java");
        }

        //void EmbedResources()
        //{
        //    TriggerEvent(BeforeEmbedResources);
        //    var atts = Project.Compilation.MainAssembly.GetMetadatas<JEmbeddedResourceAttribute>().ToList();
        //    if (atts.IsNotNullOrEmpty())
        //    {
        //        var asmFilename = Args.Output;
        //        Log.WriteLine("Loading assembly {0}", asmFilename);
        //        var asm = ModuleDefinition.ReadModule(asmFilename);
        //        var changed = false;
        //        foreach (var att in atts)
        //        {
        //            if (att.Filename.IsNullOrEmpty())
        //                throw new CompilerException(att.SourceAttribute, "JsEmbeddedResourceAttribute.Filename must be set");
        //            EmbeddedResourceFiles.Add(att.Filename);
        //            var resName = att.ResourceName ?? att.Filename;
        //            Log.WriteLine("Embedding {0} -> {1}", att.Filename, resName);
        //            var res = new EmbeddedResource(resName, ManifestResourceAttributes.Public, File.ReadAllBytes(att.Filename));
        //            var res2 = asm.Resources.Where(t => t.Name == res.Name).OfType<EmbeddedResource>().FirstOrDefault();
        //            if (res2 == null)
        //            {
        //                asm.Resources.Add(res);
        //            }
        //            else
        //            {
        //                IStructuralEquatable data2 = res2.GetResourceData();
        //                IStructuralEquatable data = res.GetResourceData();

        //                if (data.Equals(data2))
        //                    continue;
        //                asm.Resources.Remove(res2);
        //                asm.Resources.Add(res);
        //            }
        //            changed = true;

        //        }
        //        if (changed)
        //        {
        //            var prms = new WriterParameters { };//TODO:StrongNameKeyPair = new StrongNameKeyPair("Foo.snk") };
        //            var snkFile = Args.NoneFiles.Where(t => t.EndsWith(".snk", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        //            if (snkFile != null)
        //            {
        //                Log.WriteLine("Signing assembly with strong-name keyfile: {0}", snkFile);
        //                prms.StrongNameKeyPair = new StrongNameKeyPair(snkFile);
        //            }
        //            Log.WriteLine("Saving assembly {0}", asmFilename);
        //            asm.Write(asmFilename, prms);
        //        }
        //    }
        //    TriggerEvent(AfterEmbedResources);

        //}


        #region ICompiler Members

        public event Action BeforeParseCs;
        public event Action BeforeApplyExternalMetadata;
        public event Action BeforeConvertCsToJs;
        public event Action BeforeSaveJsFiles;
        public event Action BeforeSaveNewManifest;

        public event Action AfterParseCs;
        public event Action AfterApplyExternalMetadata;
        public event Action AfterConvertCsToJs;
        public event Action AfterSaveJsFiles;
        public event Action AfterSaveNewManifest;


        public ICompilation CsCompilation
        {
            get
            {
                if (Project == null)
                    return null;
                return Project.Compilation;
            }
        }

        #endregion

        #region Utils

        [DebuggerStepThrough]
         void Time(Action action)
        {
            var stopwatch = new Stopwatch();
            Log.WriteLine("{0:HH:mm:ss.fff}: {1}: Start: ", DateTime.Now, action.Method.Name);
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Log.WriteLine("{0:HH:mm:ss.fff}: {1}: End: {2}ms", DateTime.Now, action.Method.Name, stopwatch.ElapsedMilliseconds);
        }

        [DebuggerStepThrough]
         T Time2<T>(Func<T> action)
        {
            var stopwatch = new Stopwatch();
            Log.WriteLine("{0:HH:mm:ss.fff}: {1}: Start: ", DateTime.Now, action.Method.Name);
            stopwatch.Start();
            var t = action();
            stopwatch.Stop();
            Log.WriteLine("{0:HH:mm:ss.fff}: {1}: End: {2}ms", DateTime.Now, action.Method.Name, stopwatch.ElapsedMilliseconds);
            return t;
        }

        void TriggerEvent(Action ev)
        {
            if (ev != null)
                ev();
        }

        #endregion




        #region ICompiler Members


        public event Action<IEntity> BeforeConvertCsToJsEntity;

        //public event Action<IEntity, JNode> AfterConvertCsToJsEntity;

        #endregion

        #region ICompiler Members


        public event Action BeforeExit;

        #endregion

        #region ICompiler Members


        public event Action<ICSharpCode.NRefactory.CSharp.AstNode> BeforeConvertCsToJsAstNode;

        public event Action<ICSharpCode.NRefactory.CSharp.AstNode, JNode> AfterConvertCsToJsAstNode;

        public event Action<ICSharpCode.NRefactory.Semantics.ResolveResult> BeforeConvertCsToJsResolveResult;

        public event Action<ICSharpCode.NRefactory.Semantics.ResolveResult, JNode> AfterConvertCsToJsResolveResult;

        #endregion

        #region ICompiler Members

        CustomAttributeProvider _CustomAttributeProvider;
        public ICustomAttributeProvider CustomAttributeProvider
        {
            get { return _CustomAttributeProvider; }
        }

        #endregion
    }



}
