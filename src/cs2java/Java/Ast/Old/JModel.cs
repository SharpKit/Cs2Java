//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ICSharpCode.NRefactory.TypeSystem;

//namespace SharpKit.Java.Ast
//{
//    public class JCompilationUnit : JsUnit
//    {
//        public JCompilationUnit()
//        {
//            NodeType = JNodeType.CompilationUnit;
//            Imports = new List<JImport>();
//            Declarations = new List<JClassDeclaration>();
//        }
//        public List<JImport> Imports { get; set; }
//        public List<JClassDeclaration> Declarations { get; set; }

//        public string PackageName { get; set; }
//    }

//    public class JImport : JNode
//    {
//        public JImport()
//        {
//            NodeType = JNodeType.Import;
//        }
//        public ITypeDefinition TypeDefinition { get; set; }
//    }

//    public class JEntityDeclaration : JNode
//    {

//    }
//    public class JClassDeclaration : JEntityDeclaration
//    {
//        public JClassDeclaration()
//        {
//            NodeType = JNodeType.ClassDeclaration;
//            Declarations = new List<JEntityDeclaration>();
//        }
//        public ITypeDefinition TypeDefinition { get; set; }
//        public List<JEntityDeclaration> Declarations { get; set; }
//    }
//    public class JMethodDeclaration : JEntityDeclaration
//    {
//        public JMethodDeclaration()
//        {
//            NodeType = JNodeType.MethodDeclaration;
//        }
//        public IMethod MethodDefinition { get; set; }
//        public string CustomHeaderCode { get; set; }
//        public JBlock MethodBody { get; set; }
//    }

//    public class JFieldDeclaration : JEntityDeclaration
//    {
//        public JFieldDeclaration()
//        {
//            NodeType = JNodeType.FieldDeclaration;
//        }
//        public IField FieldDefinition { get; set; }
//        public JExpression Initializer { get; set; }

//    }

//    public partial class JNewAnonymousClassExpression : JNewObjectExpression
//    {
//        public JNewAnonymousClassExpression()
//        {
//            NodeType = JNodeType.NewAnonymousClassExpression;
//            Declarations = new List<JEntityDeclaration>();
//        }
//        public List<JEntityDeclaration> Declarations { get; set; }

//    }


//}
