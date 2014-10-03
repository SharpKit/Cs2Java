using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace SystemUtils.Tools
{
	public class ToolArgSyntaxAttribute : Attribute
	{
		public ToolArgSyntaxAttribute(string format)
		{
			Format = format;
		}
		public string Format { get; private set; }

	}

    public class ToolArgDefaultAttribute : Attribute
	{

	}
    public class ToolArgTrimValueCharsAttribute : Attribute
	{
		public ToolArgTrimValueCharsAttribute(char[] chars)
		{
			TrimValueChars = chars;
		}
		public char[] TrimValueChars { get; private set; }

	}






}
