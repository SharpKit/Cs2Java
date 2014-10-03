using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modelgen
{
    class ModelNode
    {
        public ModelNode()
        {
            Nodes = new List<ModelNode>();
        }
        public List<ModelNode> Nodes { get; set; }
        public void Add(ModelNode node) { Nodes.Add(node); }
        public void Add(string text)
        {
            Add(new ModelNode { Text = text });
        }
        public string Text { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
