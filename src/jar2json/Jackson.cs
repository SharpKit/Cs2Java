
using java.lang;
using JSharp;
using org.codehaus.jackson.map.introspect;
using system;

namespace org.codehaus.jackson.map.introspect
{
    /*import java.io.InputStream;
import java.io.Writer;

import org.codehaus.jackson.map.DeserializationConfig;
import org.codehaus.jackson.map.MapperConfig;
import org.codehaus.jackson.map.ObjectMapper;
import org.codehaus.jackson.map.ObjectWriter;
import org.codehaus.jackson.map.PropertyNamingStrategy;
import org.codehaus.jackson.map.annotate.JsonSerialize.Inclusion;
import org.codehaus.jackson.map.introspect.AnnotatedField;
import org.codehaus.jackson.map.introspect.AnnotatedMethod;
*/
    [JType(Export = false)]
    class AnnotatedMethod
    {
        public int getAnnotationCount()
        {
            throw new NotImplementedException();
        }

        public String getName()
        {
            throw new NotImplementedException();
        }
    }

    [JType(Export = false)]
    class AnnotatedField
    {
    }
}
namespace org.codehaus.jackson.map.annotate
{
    [JType(Export = false)]
    class JsonSerialize
    {
        [JType(Export = false)]
        public class Inclusion
        {
            [JField(Name = "NON_DEFAULT")]
            public static object NON_DEFAULT;
        }
    }
}
namespace org.codehaus.jackson.map
{
    [JType(Export = false)]
    class DeserializationConfig
    {
        [JType(Export = false)]
        public class Feature
        {
            [JField(Name = "FAIL_ON_UNKNOWN_PROPERTIES")]
            public static object FAIL_ON_UNKNOWN_PROPERTIES;
        }
    }


    [JType(Export = false)]
    class PropertyNamingStrategy
    {
        public virtual String nameForField(MapperConfig<Q> config, AnnotatedField field, String defaultName) { return null; }
        public virtual String nameForGetterMethod(MapperConfig<Q> config, AnnotatedMethod method, String defaultName) { return null; }
        public virtual String nameForSetterMethod(MapperConfig<Q> config, AnnotatedMethod method, String defaultName) { return null; }

    }

    [JType(Export = false)]
    class ObjectWriter
    {
        public void writeValue(java.io.Writer writer, object obj)
        {
            throw new NotImplementedException();
        }
    }

    [JType(Export = false)]
    class MapperConfig<T>
    {
    }

    [JType(Export = false)]
    class ObjectMapper
    {
        public void configure(object p1, bool p2)
        {
            throw new NotImplementedException();
        }

        public T readValue<T>(java.io.InputStream stream, Class<T> ce)
        {
            throw new NotImplementedException();
        }

        public void setPropertyNamingStrategy(PropertyNamingStrategy wcfNamingStrategy)
        {
            throw new NotImplementedException();
        }

        public void setSerializationInclusion(object p)
        {
            throw new NotImplementedException();
        }

        public void writeValue(java.io.Writer writer, object obj)
        {
            throw new NotImplementedException();
        }

        public ObjectWriter writerWithDefaultPrettyPrinter()
        {
            throw new NotImplementedException();
        }

        public String writeValueAsString(object obj)
        {
            throw new NotImplementedException();
        }

        public T readValue<T>(String stream, Class<T> ce)
        {
            throw new NotImplementedException();
        }



    }


}
