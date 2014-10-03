using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SystemTools.Text;

namespace modelgen
{

    public class ModelGen : CodeWriter
    {
        public ModelGen()
        {
            Usings = new List<string>
            {
                "System",
                "System.Collections.Generic",
                "System.Diagnostics",
            };
        }
        public string SourceFilename { get; set; }
        public string SourceText { get; set; }
        internal ModelClass RootNodeType { get; set; }
        public string RootNodeTypeName { get; set; }
        public string Namespace { get; set; }
        internal ModelNode Tree { get; set; }
        public bool GenerateEnum { get; set; }
        public bool GenerateProperties { get; set; }
        public bool GenerateLinq { get; set; }
        public string Output { get; set; }

        void Load()
        {
            string[] lines;
            if (SourceText != null)
            {
                lines = SourceText.Lines().ToArray();
            }
            else
            {
                lines = File.ReadAllLines(SourceFilename);
            }
            Tree = new ModelNode();
            var currentNode = Tree;
            foreach (var line in lines)
            {
                var node = new ModelNode { Text = line.Trim() };
                if (line.StartsWith("\t") || line.StartsWith(" "))
                {
                    currentNode.Add(node);
                }
                else
                {
                    Tree.Add(node);
                    currentNode = node;
                }
            }
            foreach (var ce in Tree.Nodes)
            {
                var ceTokens = ce.Text.Split(new string[] { " : " }, StringSplitOptions.None);
                ce.Name = ceTokens[0];
                if (ceTokens.Length > 1)
                    ce.Type = ceTokens[1];
                foreach (var pe in ce.Nodes)
                {
                    var peTokens = pe.Text.Split(new string[] { " " }, StringSplitOptions.None);
                    pe.Name = peTokens[1];
                    pe.Type = peTokens[0];
                }
            }
        }
        public List<string> Usings { get; set; }
        public void Run()
        {
            InnerWriter = new LineWriter(new StringWriter());
            Load();
            Convert();
            if (RootNodeTypeName == null)
            {
                RootNodeType = Classes.First();
                RootNodeTypeName = Classes.First().Name;
            }
            else
            {
                RootNodeType = GetClass(RootNodeTypeName);
            }
            Usings.ForEach(t => WriteLine("using {0};", t));
            WriteLine("namespace {0}", Namespace);
            BeginBlock();

            foreach (var ce in Classes)
            {
                GenerateClass(ce);
            }
            if (GenerateEnum)
                GenerateEnumDecl();
            GenerateVisitorInterfaces();
            EndBlock();
            Output = ((StringWriter)InnerWriter.InnerWriter).GetStringBuilder().ToString();
            if (OutputFilename.IsNotNullOrEmpty())
                File.WriteAllText(OutputFilename, Output);
        }
        public string OutputFilename { get; set; }

        private void Convert()
        {
            Classes = Tree.Nodes.Select(t => new ModelClass { Name = t.Name }).ToList();
            foreach (var node in Tree.Nodes)
            {
                var ce = GetClass(node.Name);
                ce.BaseClass = GetClass(node.Type);
                ce.Properties = node.Nodes.Select(t => new ModelProperty { Name = t.Name, Type = GetCreateClass(t.Type) }).ToList();
            }

        }

        private ModelClass GetClass(string p)
        {
            return Classes.Where(t => t.Name == p).FirstOrDefault(); ;
        }
        private ModelClass GetCreateClass(string p)
        {
            var ce = GetClass(p);
            if (ce == null)
                ce = new ModelClass { Name = p };
            return ce;
        }
        List<ModelClass> Classes;
        private void GenerateEnumDecl()
        {
            WriteLine("enum JNodeType");
            BeginBlock();
            foreach (var ce in Classes)
            {
                WriteLine("{0},", ce.Name.EnumName());
            }
            EndBlock();
        }


        private void GenerateClass(ModelClass ce)
        {
            WriteLine("[DebuggerStepThrough]");
            Write("partial class {0}", ce.Name);
            if (ce.BaseClass != null)
                Write(" : {0}", ce.BaseClass.Name);
            WriteLine("");
            BeginBlock();
            if (GenerateProperties)
                DoGenerateProperties(ce);
            GenerateCtor(ce);
            if (GenerateLinq)
                DoGenerateLinq(ce);
            GenerateVisitorImpl(ce);
            EndBlock();
        }

        private void GenerateVisitorImpl(ModelClass ce)
        {
            if (!IsDerivedFromBaseNode(ce))
                return;
            var virt = GetVirtualOrOverride(ce);

            WriteLine("public {1} R AcceptVisitor<R>({2}<R> visitor) {{ return visitor.Visit{0}(this); }}", ce.Name.EnumName(), virt, GetVisitorTypeName());
            WriteLine("public {1} void AcceptVisitor({2} visitor) {{ visitor.Visit{0}(this); }}", ce.Name.EnumName(), virt, GetVisitorTypeName());
        }

        private string GetVirtualOrOverride(ModelClass ce)
        {
            var virt = "override";
            if (IsBaseNode(ce))
                virt = "virtual";
            return virt;
        }

        private void DoGenerateProperties(ModelClass ce)
        {
            foreach (var pe in ce.Properties)
            {
                if (SupportParent && pe.Name!="Parent" && (IsListOfNodes(pe.Type) || IsDerivedFromBaseNode(pe.Type)))
                {
                    WriteLine("{0} _{1};", pe.Type.Name, pe.Name);
                    if(IsListOfNodes(pe.Type))
                        WriteLine("public {0} {1} {{ get {{ return _{1}; }} set {{ _{1}.SetItems(value);}} }}", pe.Type.Name, pe.Name);
                    else
                        WriteLine("public {0} {1} {{ get {{ return _{1}; }} set {{ if(_{1}!=null) _{1}.Parent=null; _{1}= value;if(_{1}!=null) _{1}.Parent=this;}} }}", pe.Type.Name, pe.Name);
                }
                else
                {
                    WriteLine("public {0} {1} {{get;set;}}", pe.Type.Name, pe.Name);
                }

            }
        }
        public bool SupportParent { get; set; }

        string GetVisitorTypeName()
        {
            return String.Format("I{0}Visitor", RootNodeType.Name);
        }
        void GenerateVisitorInterfaces()
        {
            WriteLine("interface {0}<out R>", GetVisitorTypeName());
            BeginBlock();
            foreach (var ce in Classes)
            {
                WriteLine("R Visit{0}({1} node);", ce.Name.EnumName(), ce.Name);
            }
            EndBlock();

            WriteLine("interface {0}", GetVisitorTypeName());
            BeginBlock();
            foreach (var ce in Classes)
            {
                WriteLine("void Visit{0}({1} node);", ce.Name.EnumName(), ce.Name);
            }
            EndBlock();

        }
        private void GenerateCtor(ModelClass ce)
        {
            var listProps = ce.Properties.Where(t => IsList(t.Type)).ToList();
            if (listProps.Count == 0 && !GenerateEnum)
                return;
            WriteLine("public {0}()", ce.Name);
            BeginBlock();
            if (GenerateEnum)
                WriteLine("NodeType = JNodeType.{0};", ce.Name.EnumName());
            WriteLine("Init();");
            foreach (var pe in listProps)
            {
                if (IsListOfNodes(pe.Type) && SupportParent)
                    WriteLine("_{0} =  new CompositionList<{1}>(t=>t.Parent=this, t=>t.Parent=null);", pe.Name, GetListItemType(pe.Type).Name);
                else
                    WriteLine("{0} =  new {1}();", pe.Name, pe.Type.Name.Replace("IList", "List"));
            }
            EndBlock();
            WriteLine("partial void Init();");
        }

        private void DoGenerateLinq(ModelClass ce)
        {
            if (!IsDerivedFromBaseNode(ce))
                return;
            var list = new List<ModelProperty>();
            foreach (var pe in ce.Properties)
            {
                if (pe.Name == "Parent")
                    continue;
                if (!IsDerivedFromBaseNode(pe.Type) && !IsListOfNodes(pe.Type))
                    continue;
                list.Add(pe);
            }
            var virt = GetVirtualOrOverride(ce);
            if (list.Count == 0 && virt == "override")
                return;

            WriteLine("public {0} IEnumerable<{1}> Children()", virt, RootNodeType.Name);
            BeginBlock();
            //var found = false;
            if (!IsBaseNode(ce))
                WriteLine("foreach(var x in base.Children()) yield return x;");
            else if (list.Count == 0)
                WriteLine("yield break;");
            foreach (var pe in ce.Properties)
            {
                if (pe.Name == "Parent")
                    continue;
                if (IsDerivedFromBaseNode(pe.Type))
                {
                    WriteLine("if({0}!=null) yield return {0};", pe.Name);
                    //found = true;
                }
                else if (IsListOfNodes(pe.Type))
                {
                    WriteLine("if({0}!=null) foreach(var x in {0}) yield return x;", pe.Name);
                    //found = true;
                }
            }
            EndBlock();
        }


        bool IsDerivedFromBaseNode(ModelClass ce)
        {
            while (ce != null)
            {
                if (IsBaseNode(ce))
                    return true;
                ce = ce.BaseClass;
            }
            return false;
        }
        bool IsBaseNode(ModelClass type)
        {
            return type == RootNodeType;
        }
        bool IsNodeLeaf(ModelClass ce)
        {
            return GetDerivedClasses(ce).FirstOrDefault() == null;
        }
        IEnumerable<ModelClass> GetDerivedClasses(ModelClass ce)
        {
            return Classes.Where(t => t.BaseClass == ce);
        }
        bool IsList(ModelClass ce)
        {
            return GetListItemType(ce) != null;
        }
        bool IsListOfNodes(ModelClass ce)
        {
            var ce2 = GetListItemType(ce);
            return ce2 != null && IsDerivedFromBaseNode(ce2);
        }
        ModelClass GetListItemType(ModelClass ce)
        {
            if (!ce.Name.StartsWith("List") && !ce.Name.StartsWith("IList"))
                return null;
            var name = ce.Name.SubstringBetween("<", ">");
            return GetCreateClass(name);
        }


    }

    class ModelMember
    {
        public string Name { get; set; }
    }
    class ModelClass : ModelMember
    {
        public ModelClass BaseClass { get; set; }
        public ModelClass()
        {
            Properties = new List<ModelProperty>();
        }
        public List<ModelProperty> Properties { get; set; }
    }
    class ModelProperty : ModelMember
    {
        public ModelClass Type { get; set; }
    }

    static class Extensions
    {
        public static string EnumName(this string s)
        {
            if (s.StartsWith("Js"))
                return s.Substring(2);
            else if (s.StartsWith("J"))
                return s.Substring(1);
            return s;
        }
    }
}