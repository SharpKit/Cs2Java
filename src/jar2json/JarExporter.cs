using java.io;
using java.lang;
using java.lang.reflect;
using java.util;
using JSharpKit;
using system.linq;

namespace JSharpTest
{
    public class JarExporter
    {

        static int Count;
        public void run(String[] args)
        {
            try
            {
                var jar = new JJar();

                var jarFilename = args[0];
                var outputFilename = args[1];

                var names = JarHelper.getAllClassNames(jarFilename);
                names = names.OrderBy(t => t);

                names.ForEach(arg =>
                {
                    Count++;
                    WriteLine(Count + " " + arg);
                });
                //var loader = JarHelper.CreateClassLoader(jarFilename);
                var classes = JarHelper.TryLoadAllClassesByNames(names);
                classes = classes.Where(arg =>
                    {
                        try
                        {
                            return !arg.IsMemberClass;
                        }
                        catch (Throwable e)
                        {
                            return false;
                        }
                    }).ToList();

                jar.Classes = visitEachClass(classes);
                var fileWriter = new FileWriter(outputFilename);
                JsonHelper.serializePretty(fileWriter, jar, true);
                fileWriter.close();
                WriteLine("Saved");
            }
            catch (Exception e)
            {
                e.printStackTrace();
                throw new RuntimeException(e);
            }
        }


        static void WriteLine(string s)
        {
            Sys.@out.println(s);
        }
        private static JClass visitClass(Class<Q> ce)
        {
            JClass ce2 = new JClass();
            ce2.IsAnonymousClass = ce.IsAnonymousClass;
            ce2.IsSynthetic = ce.IsSynthetic;

            ce2.Interfaces = visitEachType(ce.GenericInterfaces);
            ce2.BaseClass = visitType(ce.GenericSuperclass);
            ce2.Name = ce.Name;
            visitModifiers(ce2, ce.Modifiers);
            ce2.IsInterface = ce.IsInterface;
            ce2.IsEnum = ce.IsEnum;
            ce2.IsArray = ce.IsArray;
            if (ce.IsArray)
                ce2.ArrayItemType = visitType(ce.ComponentType);
            ce2.GenericArguments.AddRange(visitEachTypeVariable(ce.TypeParameters.As<TypeVariable<Q>[]>()));
            ce2.Classes = visitEachClass(ce.DeclaredClasses.As<Class<Q>[]>().ToList());
            List<Constructor<Q>> ctors = ce.DeclaredConstructors.As<Constructor<Q>[]>().ToList();
            ce2.Ctors = visitEachCtor(ctors);
            //TODO:ce.getDeclaredAnnotations();
            ce2.Methods = visitEachMethod(ce.DeclaredMethods.ToList());
            ce2.Fields = visitEachField(ce.DeclaredFields.ToList());

            return ce2;
        }


        private static Collection<JClassRef> visitEachTypeVariable(TypeVariable<Q>[] typeParameters)
        {
            return typeParameters.ToList().Select(VisitTypeVariable).ToList();
        }

        private static JClassRef VisitTypeVariable(TypeVariable<Q> me)
        {
            var cr = new JClassRef();
            cr.IsTypeVariable = true;
            cr.Name = me.Name;
            if (me.GenericDeclaration is Method)
                cr.IsTypeVariableOwnedByMethod = true;
            return cr;
        }

        private static List<JClass> visitEachClass(List<Class<Q>> list)
        {
            return TrySelect(list, visitClass).ToList();
        }

        static List<R> TrySelect<T, R>(Iterable<T> list, system.Func<T, R> func)
        {
            var list2 = new ArrayList<R>();
            foreach (var item in list)
            {
                try
                {
                    list2.add(func(item));
                }
                catch(Throwable e)
                {
                }
            }
            return list2;
        }
        private static List<JMethod> visitEachMethod(List<Method> methods)
        {
            return TrySelect(methods, visitMethod).ToList();
        }
        private static List<JConstructor> visitEachCtor(List<Constructor<Q>> methods)
        {
            return TrySelect(methods, visitCtor);
        }

        private static List<R> visitEach<T, R>(List<T> items, system.Func<T, R> func)
        {
            return items.Select(func).ToList();
        }

        private static List<JClassRef> visitEachType(Type[] methods)
        {
            return methods.ToList().Select(visitType).ToList();
        }
        private static List<JField> visitEachField(List<Field> fields)
        {
            return visitEach(fields, visitField);
            //return fields.ToList().Select(visitField).ToList();
        }

        private static JField visitField(Field fe)
        {
            var fe2 = new JField();
            visitMember(fe, fe2);
            fe2.Type = visitType(fe.GenericType);
            return fe2;
        }

        private static JClassRef visitType(Type type)
        {
            if (type is Class<Q>)
            {
                var ce = (Class<Q>)type;
                var cr = new JClassRef();
                cr.Name = ce.Name;
                cr.IsArray = ce.IsArray;
                if (ce.IsArray)
                    cr.ArrayItemType = visitType(ce.ComponentType);
                return cr;
            }
            else if (type is GenericArrayType)
            {
                var ce = (GenericArrayType)type;
                var cr = new JClassRef();
                cr.IsArray = true;
                cr.ArrayItemType = visitType(ce.GenericComponentType);
                return cr;
            }
            else if (type is ParameterizedType)
            {
                var ce = (ParameterizedType)type;
                var cr = visitType(ce.RawType);
                cr.IsParameterizedType = true;
                cr.GenericArguments = visitEachType(ce.ActualTypeArguments);
                return cr;
            }
            else if (type is TypeVariable<Q>)
            {
                var ce = (TypeVariable<Q>)type;
                var cr = VisitTypeVariable(ce);
                return cr;
            }
            else if (type is WildcardType)
            {
                var ce = (WildcardType)type;
                var cr = new JClassRef();
                cr.IsWildcardType = true;
                cr.LowerBounds = visitEachType(ce.LowerBounds);
                cr.UpperBounds = visitEachType(ce.UpperBounds);
                return cr;
            }
            return null;
        }

        private static void visitMember(Member me, JMember me2)
        {
            me2.Name = me.Name;
            int modifiers = me.Modifiers;
            visitModifiers(me2, modifiers);
            me2.IsSynthetic = me.IsSynthetic;

        }

        private static void visitModifiers(JMember fe2, int modifiers)
        {
            fe2.IsPublic = Modifier.isPublic(modifiers);
            fe2.IsPrivate = Modifier.isPrivate(modifiers);
            fe2.IsProtected = Modifier.isProtected(modifiers);
            fe2.IsStatic = Modifier.isStatic(modifiers);
            fe2.IsFinal = Modifier.isFinal(modifiers);
            fe2.IsAbstract = Modifier.isAbstract(modifiers);
        }

        private static JMethod visitMethod(Method me)
        {
            var me2 = new JMethod();
            visitMember(me, me2);
            me2.TypeParameters = visitEachTypeVariable(me.TypeParameters.As<TypeVariable<Q>[]>()).ToList();
            me2.Type = visitType(me.GenericReturnType);
            visitParameters2(me.GenericParameterTypes, me2.Parameters);
            return me2;
        }
        private static JConstructor visitCtor(Constructor<Q> me)
        {
            JConstructor me2 = new JConstructor();
            visitMember(me, me2);
            visitParameters2(me.GenericParameterTypes, me2.Parameters);
            return me2;
        }

        private static void visitParameters2(Type[] prmTypes, List<JParameter> prms2)
        {
            List<JClassRef> prmTypes2 = visitEachType(prmTypes);
            foreach (JClassRef prmType2 in prmTypes2)
            {
                JParameter prm2 = new JParameter();
                prm2.Type = prmType2;
                prms2.add(prm2);
            }
        }

    }
}
