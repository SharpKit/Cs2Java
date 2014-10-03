using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace SystemUtils.Tools
{
    class ToolArgsParserTypeInfo
    {
        public ToolArgsParserTypeInfo(Type type)
        {
            Type = type;
            Properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(t => new ToolArgsParserPropertyInfo(t)).ToList();
            DefaultProperty = Properties.Where(t => t.Property.GetCustomAttribute<ToolArgDefaultAttribute>() != null).FirstOrDefault();
        }
        public ToolArgsParserPropertyInfo DefaultProperty { get; set; }
        public Type Type { get; set; }
        public List<ToolArgsParserPropertyInfo> Properties { get; set; }
    }
    class ToolArgsParserPropertyInfo
    {
        public ToolArgsParserPropertyInfo()
        {
        }
        public ToolArgsParserPropertyInfo(PropertyInfo pi)
        {
            Property = pi;
            TrimValueCharsAttribute = pi.GetCustomAttribute<ToolArgTrimValueCharsAttribute>();
            SyntaxAttribute = pi.GetCustomAttribute<ToolArgSyntaxAttribute>();
        }

        public PropertyInfo Property { get; set; }
        public ToolArgTrimValueCharsAttribute TrimValueCharsAttribute { get; set; }
        public ToolArgSyntaxAttribute SyntaxAttribute { get; set; }

        public bool SyntaxFormatHasParameter
        {
            get
            {
                return SyntaxFormat.Contains("{0}");
            }
        }

        public bool NeedsQuote
        {
            get
            {
                return Property.PropertyType != typeof(bool) && Property.PropertyType != typeof(bool?) &&
                    Property.PropertyType != typeof(int) && Property.PropertyType != typeof(int?);
            }
        }

        string _SyntaxFormat;
        public string SyntaxFormat
        {
            get
            {
                if (_SyntaxFormat == null)
                {
                    if(SyntaxAttribute != null)
                        _SyntaxFormat = SyntaxAttribute.Format;
                    else if (Property.PropertyType == typeof(bool))
                        _SyntaxFormat = String.Format("/{0}", Property.Name);
                    else //if (Property.PropertyType == typeof(bool?))
                        _SyntaxFormat = String.Format("/{0}:{{0}}", Property.Name);
                }
                return _SyntaxFormat;
            }
        }
    }

    public class ToolArgsParser
    {

        public ToolArgsParser(object target)
        {
            Target = target;
            TargetTypeInfo = new ToolArgsParserTypeInfo(Target.GetType());
        }
        public static T Parse<T>(params string[] args) where T : new()
        {
            var t = new T();
            var parser = new ToolArgsParser(t);
            parser.Parse(args);
            return t;
        }
        public static void ParseInto(object obj, string[] args)
        {
            var parser = new ToolArgsParser(obj);
            parser.Parse(args);
        }
        public static void ParseInto(object obj, string args)
        {
            var parser = new ToolArgsParser(obj);
            parser.Parse(args);
        }

        ToolArgsParserTypeInfo TargetTypeInfo;
        object Target { get; set; }

        bool IsCollection(PropertyInfo pi)
        {
            return pi.PropertyType.GetInterface("IList") != null;
        }

        void SetTargetPropertyValue(PropertyInfo pi, object value)
        {
            var att = pi.GetCustomAttribute<ToolArgTrimValueCharsAttribute>();
            if (att != null && att.TrimValueChars != null)
            {
                value = ((string)value).Trim(att.TrimValueChars);
            }
            if (IsCollection(pi))
            {
                var list = pi.GetValue(Target, null) as IList;
                if (list == null)
                {
                    list = Activator.CreateInstance(pi.PropertyType) as IList;
                    pi.SetValue(Target, list, null);
                }
                list.Add(value);
            }
            else
            {
                if (value != null && pi.PropertyType != value.GetType())
                {
                    value = TypeDescriptor.GetConverter(pi.PropertyType).ConvertFrom(value);
                    //value = Convert.ChangeType(value, pi.PropertyType);
                }
                pi.SetValue(Target, value, null);
            }
        }

        PropertyInfo FindTargetDefaultProperty()
        {
            return TargetTypeInfo.DefaultProperty.Property;//.Def.Where(t => t.GetCustomAttribute<DefaultCommandLineArgumentAttribute>() != null).FirstOrDefault();
        }

        PropertyInfo FindTargetProperty(string name, Type[] expectedPropertyTypes)
        {
            var pi = TargetTypeInfo.Properties.Where(t => t.Property.Name.ToLower() == name.ToLower()).FirstOrDefault();//.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (pi == null)
            {
                //Warn("cannot find property for argument: {0}", name);
                return null;
            }
            if (expectedPropertyTypes != null && expectedPropertyTypes.Length > 0 && expectedPropertyTypes.FirstOrDefault(t => t == pi.Property.PropertyType) == null)
                Warn("Property type is different than expected");
            return pi.Property;
        }

        void Parse(string argsText)
        {
            var tokenizer = new ToolArgsTokenizer();
            var args = tokenizer.Tokenize(argsText);
            Parse(args);
        }

        void Parse(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.IsNullOrEmpty())
                    continue;
                //var propsAndAttributes = TargetTypeInfo.Properties.FirstOrDefault(t=>.Select(t => new CommandLineArgumentsParserPropertyInfo { Property = t, SyntaxAttribute = t.GetCustomAttribute<CommandLineArgumentSyntaxAttribute>() });
                var syntaxProp = TargetTypeInfo.Properties.FirstOrDefault
                    (
                        t => t.SyntaxAttribute != null && arg.StartsWith(t.SyntaxAttribute.Format.Replace("{0}", ""))
                    );
                if (syntaxProp != null)
                {
                    if (!syntaxProp.SyntaxAttribute.Format.EndsWith("{0}"))
                    {
                        if (syntaxProp.Property.PropertyType == typeof(bool) || syntaxProp.Property.PropertyType == typeof(Nullable<bool>))
                        {
                            SetTargetPropertyValue(syntaxProp.Property, true);
                        }
                        else
                            throw new NotImplementedException("TODO: Implement");
                    }
                    else
                    {
                        var value = arg.Substring(syntaxProp.SyntaxAttribute.Format.IndexOf('{'));
                        value = RemoveQoutesIfNeeded(value);
                        SetTargetPropertyValue(syntaxProp.Property, value);
                    }
                }
                else if (arg.StartsWith("@"))
                {
                    var filename = arg.Substring(1);
                    filename = RemoveQoutesIfNeeded(filename);
                    string argsFromFile = File.ReadAllText(filename);
                    Parse(argsFromFile);
                }
                else if (arg.StartsWith("/"))
                {
                    var arg2 = arg.Substring(1);
                    if (arg2.Contains(":"))
                    {

                        var tokens = arg2.Split(':');
                        var name = tokens[0];
                        var value = tokens[1];
                        var pi = FindTargetProperty(name, new Type[] { typeof(bool), typeof(bool?), typeof(string) });
                        if (pi == null)
                            Warn("Property not found {0}", name);
                        else
                            SetTargetPropertyValue(pi, value);
                    }
                    else
                    {
                        var value = true;
                        if (arg2.EndsWith("-"))
                        {
                            value = false;
                            arg2 = arg2.Substring(0, arg2.Length - 1);
                        }
                        else if (arg2.EndsWith("+"))
                        {
                            //value = true;
                            arg2 = arg2.Substring(0, arg2.Length - 1);
                        }
                        var pi = FindTargetProperty(arg2, new Type[] { typeof(bool), typeof(bool?) });
                        if (pi == null)
                            Warn("Property not found {0}", arg2);
                        else
                            SetTargetPropertyValue(pi, value);
                    }
                }
                else
                {
                    var arg2 = RemoveQoutesIfNeeded(arg);
                    SetTargetPropertyValue(FindTargetDefaultProperty(), arg2);
                }
            }
        }

        private string RemoveQoutesIfNeeded(string arg)
        {
            if (IsQuoted(arg))
            {
                arg = arg.Substring(1);
                arg = arg.RemoveLast(1);
            }
            return arg;
        }

        private bool IsQuoted(string arg)
        {
            return arg.StartsWith("\"") && arg.EndsWith("\"");
        }

        void Warn(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

    }


    public class ToolArgsGenerator
    {
        public static string Generate(object target)
        {
            var sb = new StringBuilder();
            var typeInfo = new ToolArgsParserTypeInfo(target.GetType());
            if (typeInfo.DefaultProperty != null)
            {
                var value = typeInfo.DefaultProperty.Property.GetValue(target, null);
                sb.AppendFormat("\"{0}\" ", value);
            }
            foreach (var prop in typeInfo.Properties)
            {
                if (prop == typeInfo.DefaultProperty || prop.SyntaxFormat.IsNullOrEmpty())
                    continue;
                var value = prop.Property.GetValue(target, null);
                if (value != null)
                {
                    if (!prop.SyntaxFormatHasParameter && value is bool)
                    {
                        if ((bool)value == true)
                        {
                            sb.AppendFormat(prop.SyntaxFormat);
                            sb.Append(" ");
                        }
                    }
                    else
                    {
                        string sValue;
                        if (prop.NeedsQuote)
                        {
                            sValue = String.Format("\"{0}\"", value);
                        }
                        else
                        {
                            sValue = String.Format("{0}", value);
                        }
                        sb.AppendFormat(prop.SyntaxFormat, sValue);
                        sb.Append(" ");
                    }
                }
            }
            return sb.ToString();
        }

        public static string GenerateHelp<T>()
        {
            return GenerateHelp(typeof(T));
        }
        public static string GenerateHelp(Type type)
        {
            var sb = new StringWriter();
            GenerateHelp(type, sb);
            return sb.GetStringBuilder().ToString();
        }

        public static void GenerateHelp(Type argsType, TextWriter writer)
        {
            writer.WriteLine("Usage:");
            var typeInfo = new ToolArgsParserTypeInfo(argsType);
            if (typeInfo.DefaultProperty != null)
            {
                writer.Write("[{0}]", typeInfo.DefaultProperty.Property.Name);
            }
            foreach (var prop in typeInfo.Properties)
            {
                if (prop == typeInfo.DefaultProperty)
                {
                    continue;
                }
                writer.Write(" ");
                if (prop.SyntaxAttribute != null && prop.SyntaxAttribute.Format != null)
                {
                    if (prop.SyntaxAttribute.Format.Contains("{0}"))
                        writer.Write(prop.SyntaxAttribute.Format, prop.Property.Name);
                    else
                        writer.Write("[{0} ({1})]", prop.SyntaxAttribute.Format, prop.Property.Name);
                }
            }
            writer.WriteLine();
        }

        public static string GenerateHelp(object tool)
        {
            return GenerateHelp(tool.GetType());
        }
    }
}
