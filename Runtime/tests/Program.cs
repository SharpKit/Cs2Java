using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    class Program
    {
        static void Main(string[] args)
        {
        }
        public string Name { get; set; }
        public string Name2 { get { return null; } set { } }
        public event EventHandler MyEvent;
        public event EventHandler MyEvent2 { add { } remove { } }
    }
}
