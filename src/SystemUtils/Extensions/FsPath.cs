using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    public struct FsPath
    {
        public FsPath(string path)
        {
            _Value = path;
        }
        public static implicit operator FsPath(string value)
        {
            return new FsPath(value);
        }
        public static implicit operator string(FsPath path)
        {
            return path._Value;
        }
        string _Value;
        public static FsPath Create(string path)
        {
            return new FsPath(path);
        }
        public FsPath Parent
        {
            get
            {
                return Create(Path.GetDirectoryName(_Value));
            }
        }
        public FsPath this[string name]
        {
            get
            {
                return Child(name);
            }
        }
        public FsPath Child(string name)
        {
            return Create(Path.Combine(_Value, name));
        }
        public FsPath Sibling(string name)
        {
            return Parent.Child(name);
        }
        public FsPath FileNameWithoutExtension
        {
            get
            {
                return Create(Path.GetFileNameWithoutExtension(_Value));
            }
        }
        public FsPath FileName
        {
            get
            {
                return Create(Path.GetFileName(_Value));
            }
        }
        public FsPath ChangeExtension(string ext)
        {
            return Create(Path.ChangeExtension(_Value, ext));
        }
    }
}
