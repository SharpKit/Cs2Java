
using java.lang;
using java.net;
using java.util;
using java.util.jar;
using system.linq;

namespace JSharpTest
{

    public class JarHelper
    {

        public static List<Class<Q>> TryLoadAllClassesFromJar(String jarFilename)
        {
            var names = getAllClassNames(jarFilename);
            var classes = TryLoadAllClassesByNames(names);
            return classes;
        }

        public static List<Class<Q>> TryLoadAllClassesByNames(List<String> names)
        {
            var classes = new ArrayList<Class<Q>>();
            var errors = 0;
            foreach (var name in names)
            {
                Class<Q> ce = TryLoadClass(name);
                if (ce == null)
                {
                    errors++;
                    if (errors > 100)
                        break;
                    continue;
                }
                classes.add(ce);
            }
            return classes;
        }

        public static ClassLoader CreateClassLoader(String jarFilename)
        {
            try
            {
                return new URLClassLoader(new URL[] { new URL("jar:file:" + jarFilename + "!/") });
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }

        public static List<String> getAllClassNames(String jarFilename)
        {
            try
            {
                var classes = new ArrayList<String>();
                var pathToJar = jarFilename;
                var jarFile = new JarFile(pathToJar);
                foreach (var je in jarFile.entries().ToIterable())
                {
                    if (je.IsDirectory || !je.Name.endsWith(".class"))
                        continue;
                    String className = je.Name.substring(0, je.Name.length() - ".class".Length);
                    className = className.replace('/', '.');
                    classes.add(className);
                }
                jarFile.close();
                return classes;
            }
            catch (Exception e)
            {
                e.printStackTrace();
                throw new RuntimeException(e);
            }
        }
        public static Class<Q> TryLoadClass(String className)//, ClassLoader loader)
        {
            try
            {
                var c = Class<Q>.forName(className).As<Class<Q>>();//, false, null);//, false, loader);
                return c;
            }
            catch (Throwable e)
            {
                Sys.@out.println("Error loading class: " + className + " " + e);
                //e.printStackTrace();
                return null;
            }
        }

    }
}