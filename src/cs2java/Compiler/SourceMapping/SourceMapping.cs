using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Web.Script.Serialization;
using System.IO;
using System.Runtime.Serialization;

namespace JSharp.Compiler.SourceMapping
{
    [DataContract]
    class SourceMappingDocument
    {
        [DataMember]
        public List<SourceMappingItem> Mappings { get; set; }
    }
    [DataContract]
    class SourceMappingItem
    {
        [DataMember]
        public FileLocation GeneratedLocation { get; set; }
        [DataMember]
        public FileLocation SourceLocation { get; set; }
        [DataMember]
        public string SourceName { get; set; }
    }
    [DataContract]
    class FileLocation
    {
        public FileLocation()
        {
        }
        public FileLocation(string filename, int line, int col)
        {
            Filename = filename;
            Line = line;
            Column = col;
        }
        [DataMember]
        public string Filename { get; set; }
        [DataMember]
        public int Line { get; set; }
        [DataMember]
        public int Column { get; set; }
        public override string ToString()
        {
            return String.Format("{0} [{1},{2}]", Filename, Line, Column);
        }
    }
}
