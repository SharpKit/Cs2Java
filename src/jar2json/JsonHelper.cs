using java.io;
using java.lang;
using JSharp;
using JSharpKit;
using org.codehaus.jackson.map;
using org.codehaus.jackson.map.annotate;
using org.codehaus.jackson.map.introspect;

namespace JSharpTest
{


    public class JsonHelper
    {
        public static T deserializeWcf<T>(InputStream stream, Class<T> ce)
        {
            ObjectMapper mapper = createMapper(true);
            try
            {
                T obj = mapper.readValue(stream, ce);
                return obj;
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }

        public static T deserializeWcf<T>(String stream, Class<T> ce)
        {
            ObjectMapper mapper = createMapper(true);
            try
            {
                T obj = mapper.readValue(stream, ce);
                return obj;
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }

        public static T deserialize<T>(String stream, Class<T> ce, bool isWcf)
        {
            ObjectMapper mapper = createMapper(isWcf);
            try
            {
                T obj = mapper.readValue(stream, ce);
                return obj;
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }
        public static T deserialize<T>(String stream, Class<T> ce)
        {
            ObjectMapper mapper = createMapper(false);
            try
            {
                T obj = mapper.readValue(stream, ce);
                return obj;
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }


        private static ObjectMapper createMapper(bool isWcf)
        {
            ObjectMapper mapper = new ObjectMapper();
            mapper.configure(DeserializationConfig.Feature.FAIL_ON_UNKNOWN_PROPERTIES, false);
            mapper.setSerializationInclusion(JsonSerialize.Inclusion.NON_DEFAULT);
            if (isWcf)
                mapper.setPropertyNamingStrategy(new WcfNamingStrategy());
            return mapper;
        }

        public static void serializeWcf(Writer writer, object obj)
        {
            serialize(writer, obj, true);
        }

        public static void serializePretty(Writer writer, object obj, bool isWcf)
        {
            ObjectMapper mapper = createMapper(isWcf);
            try
            {
                ObjectWriter writer2 = mapper.writerWithDefaultPrettyPrinter();
                writer2.writeValue(writer, obj);
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }
        public static void serialize(Writer writer, object obj, bool isWcf)
        {
            ObjectMapper mapper = createMapper(isWcf);
            try
            {
                mapper.writeValue(writer, obj);
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }
        public static String toJsonWcf(object obj)
        {
            ObjectMapper mapper = createMapper(true);
            try
            {
                return mapper.writeValueAsString(obj);
            }
            catch (Exception e)
            {
                throw new RuntimeException(e);
            }
        }
    }

    /**
     * Provides a generic way to solve the naming issue of properties when sent from Wcf:
     * {Age : 7}   => setAge(7) 
     * instead of the default which is:
     * {age : 7}   => setAge(7)
     * @author dkhen
     *
     */
    class WcfNamingStrategy : PropertyNamingStrategy
    {
        public override String nameForField(MapperConfig<Q> config, AnnotatedField field, String defaultName)
        {
            return toPascalCasing(defaultName);
        }

        private bool skip(String name, AnnotatedMethod method)
        {
            if (method.getAnnotationCount() > 0)
                return true;
            return false;
        }
        public override String nameForGetterMethod(MapperConfig<Q> config, AnnotatedMethod method, String defaultName)
        {
            if (skip(defaultName, method))
                return defaultName;
            if (method.getName().startsWith("is"))
            {
                return toPascalCasing(method.getName());
            }
            return toPascalCasing(defaultName);
        }
        public override String nameForSetterMethod(MapperConfig<Q> config, AnnotatedMethod method, String defaultName)
        {
            if (skip(defaultName, method))
                return defaultName;
            return toPascalCasing(defaultName);
        }
        private  String toPascalCasing(String defaultName)
        {
            return defaultName.substring(0, 1).toUpperCase() + defaultName.substring(1);
        }
    }
}
