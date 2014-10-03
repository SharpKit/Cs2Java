using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utils.Http
{
    class JsonServer
    {
        public object Service { get; set; }
        void ProcessRequest(HttpListenerContext context)
        {
            var action = context.Request.Url.PathAndQuery.Substring(1);
            var me = Service.GetType().GetMethod(action);
            var ser = new DataContractJsonSerializer(me.GetParameters()[0].ParameterType);
            var prm = ser.ReadObject(context.Request.InputStream);
            try
            {
                var returnValue = me.Invoke(Service, new object[] { prm });
                context.Response.StatusCode = 200;
                if (returnValue != null)
                {
                    var ser2 = new DataContractJsonSerializer(returnValue.GetType());
                    ser2.WriteObject(context.Response.OutputStream, returnValue);
                }
                context.Response.OutputStream.Close();
            }
            catch (TargetInvocationException e)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusDescription = e.Message;
                context.Response.OutputStream.Close();
            }
        }

        public void Run()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:7667/");
            listener.Start();
            while (true)
            {
                try
                {
                    var context = listener.GetContext();
                    Console.WriteLine("Started ", context.Request);
                    var ms = StopwatchHelper.TimeInMs(() => ProcessRequest(context));
                    Console.WriteLine("Finished {0}ms", ms);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
