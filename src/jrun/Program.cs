using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jrun
{
    class Program
    {
        static void Main(string[] args)
        {
            JavaTool.AutoDetect(Directory.GetCurrentDirectory()).Run();
        }
    }



}
