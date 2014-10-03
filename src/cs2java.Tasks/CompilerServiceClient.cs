using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using JSharp.Compiler;

namespace cs2java.Tasks
{
    class CompilerServiceClient
    {
        public CompilerServiceClient()
        {
            Client = new JsonClient();
        }
        public JsonClient Client { get; set; }
        public CompileResponse Compile(CompileRequest args)
        {
            return Client.Invoke<CompileResponse>("Compile", args);
        }
    }

    [DataContract]
    class CompileRequest
    {
        [DataMember]
        public string CommandLineArgs { get; set; }
        [DataMember]
        public CompilerToolArgs Args { get; set; }
    }
    [DataContract]
    class CompileResponse
    {
        [DataMember]
        public List<string> Output { get; set; }
        [DataMember]
        public int ExitCode { get; set; }
    }


    class JsonClient
    {
        public R Invoke<R>(string action, object arg)
        {
            var req = HttpWebRequest.CreateHttp("http://localhost:7667/" + action);
            req.Method = "POST";
            var ser = new DataContractJsonSerializer(arg.GetType());
            ser.WriteObject(req.GetRequestStream(), arg);
            var res = (HttpWebResponse)req.GetResponse();
            if (res.StatusCode != HttpStatusCode.OK)
                throw new Exception(res.StatusCode + ", " + res.StatusDescription);
            if (typeof(R) != typeof(object))
            {
                var ser2 = new DataContractJsonSerializer(typeof(R));
                var x = (R)ser2.ReadObject(res.GetResponseStream());
                return x;
            }
            return default(R);
        }
    }
}
