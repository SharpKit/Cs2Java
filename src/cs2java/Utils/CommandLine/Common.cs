using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using JSharp.Utils;

namespace JSharp.Utils.CommandLine
{
	internal class CommandLineArgumentSyntaxAttribute : Attribute
	{
		public CommandLineArgumentSyntaxAttribute(string format)
		{
			Format = format;
		}
		public string Format { get; private set; }

	}

	internal class DefaultCommandLineArgumentAttribute : Attribute
	{

	}
	internal class CommandLineArgumentTrimValueCharsAttribute : Attribute
	{
		public CommandLineArgumentTrimValueCharsAttribute(char[] chars)
		{
			TrimValueChars = chars;
		}
		public char[] TrimValueChars { get; private set; }

	}






}
