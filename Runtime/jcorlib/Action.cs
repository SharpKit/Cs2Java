using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSharp;

namespace system
{
    public delegate void Action();
    [JType(Name = "system.Action1")]
    public delegate void Action<in T>(T arg);
    [JType(Name = "system.Action2")]
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    [JType(Name = "system.Action3")]
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate R Func<out R>();
    [JType(Name = "system.Func1")]
    public delegate R Func<in T, out R>(T arg);
    [JType(Name = "system.Func2")]
    public delegate R Func<in T1, in T2, out R>(T1 arg1, T2 arg2);

    public delegate void AsyncCallback(IAsyncResult ar);

    public interface IAsyncResult
    {
        bool IsCompleted { get; }
        object AsyncState { get; }
        bool CompletedSynchronously { get; }
    }

}
