using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system.reflection;
using system.runtime.serialization;

namespace system
{
    /// <summary>Represents a delegate, which is a data structure that refers to a static method or to a class instance and an instance method of that class.</summary>
    /// <filterpriority>2</filterpriority>
    [Serializable]
    public abstract class Delegate : ICloneable, ISerializable
    {
        MethodInfo _Method;
        /// <summary>Gets the method represented by the delegate.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> describing the method represented by the delegate.</returns>
        /// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        /// <filterpriority>2</filterpriority>
        public MethodInfo Method
        {
            get
            {
                return _Method;
            }
        }
        object _Target;
        /// <summary>Gets the class instance on which the current delegate invokes the instance method.</summary>
        /// <returns>The object on which the current delegate invokes the instance method, if the delegate represents an instance method; null if the delegate represents a static method.</returns>
        /// <filterpriority>2</filterpriority>
        public object Target
        {
            get
            {
                return _Target;
            }
        }
        ///// <summary>Initializes a delegate that invokes the specified instance method on the specified class instance.</summary>
        ///// <param name="target">The class instance on which the delegate invokes <paramref name="method" />. </param>
        ///// <param name="method">The name of the instance method that the delegate represents. </param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">There was an error binding to the target method.</exception>
        //protected Delegate(object target, string method)
        //{
        //    if (target == null)
        //    {
        //        throw new ArgumentNullException("target");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("method");
        //    }
        //    //if (!this.BindToMethodName(target, target.GetType(), method, (DelegateBindingFlags)10))
        //    //{
        //    //    throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
        //    //}
        //}
        ///// <summary>Initializes a delegate that invokes the specified static method from the specified class.</summary>
        ///// <param name="target">The <see cref="T:System.Type" /> representing the class that defines <paramref name="method" />. </param>
        ///// <param name="method">The name of the static method that the delegate represents. </param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="target" /> is not a RuntimeType. See Runtime Types in Reflection.-or-<paramref name="target" /> represents an open generic type.</exception>
        //protected Delegate(Type target, string method)
        //{
        //    //if (target == null)
        //    //{
        //    //    throw new ArgumentNullException("target");
        //    //}
        //    //if (target.IsGenericType && target.ContainsGenericParameters)
        //    //{
        //    //    throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
        //    //}
        //    //if (method == null)
        //    //{
        //    //    throw new ArgumentNullException("method");
        //    //}
        //    //RuntimeType runtimeType = target as RuntimeType;
        //    //if (runtimeType == null)
        //    //{
        //    //    throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
        //    //}
        //    //this.BindToMethodName(null, runtimeType, method, (DelegateBindingFlags)37);
        //}
        ///// <summary>Dynamically invokes (late-bound) the method represented by the current delegate.</summary>
        ///// <returns>The object returned by the method represented by the delegate.</returns>
        ///// <param name="args">An array of objects that are the arguments to pass to the method represented by the current delegate.-or- null, if the method represented by the current delegate does not require arguments. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private).-or- The number, order, or type of parameters listed in <paramref name="args" /> is invalid. </exception>
        ///// <exception cref="T:System.ArgumentException">The method represented by the delegate is invoked on an object or a class that does not support it. </exception>
        ///// <exception cref="T:System.Reflection.TargetInvocationException">The method represented by the delegate is an instance method and the target object is null.-or- One of the encapsulated methods throws an exception. </exception>
        ///// <filterpriority>2</filterpriority>
        //public object DynamicInvoke(params object[] args)
        //{
        //    return this.DynamicInvokeImpl(args);
        //}
        ///// <summary>Dynamically invokes (late-bound) the method represented by the current delegate.</summary>
        ///// <returns>The object returned by the method represented by the delegate.</returns>
        ///// <param name="args">An array of objects that are the arguments to pass to the method represented by the current delegate.-or- null, if the method represented by the current delegate does not require arguments. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private).-or- The number, order, or type of parameters listed in <paramref name="args" /> is invalid. </exception>
        ///// <exception cref="T:System.ArgumentException">The method represented by the delegate is invoked on an object or a class that does not support it. </exception>
        ///// <exception cref="T:System.Reflection.TargetInvocationException">The method represented by the delegate is an instance method and the target object is null.-or- One of the encapsulated methods throws an exception. </exception>
        //protected virtual object DynamicInvokeImpl(object[] args)
        //{
        //    throw new Exception();
        //    //RuntimeMethodHandleInternal methodHandle = new RuntimeMethodHandleInternal(this.GetInvokeMethod());
        //    //RuntimeMethodInfo runtimeMethodInfo = (RuntimeMethodInfo)RuntimeType.GetMethodBase((RuntimeType)base.GetType(), methodHandle);
        //    //return runtimeMethodInfo.UnsafeInvoke(this, BindingFlags.Default, null, args, null);
        //}
        ///// <summary>Determines whether the specified object and the current delegate are of the same type and share the same targets, methods, and invocation list.</summary>
        ///// <returns>true if <paramref name="obj" /> and the current delegate have the same targets, methods, and invocation list; otherwise, false.</returns>
        ///// <param name="obj">The object to compare with the current delegate. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        ///// <filterpriority>2</filterpriority>
        //public override bool Equals(object obj)
        //{
        //    throw new Exception();
        //    //if (obj == null || !Delegate.InternalEqualTypes(this, obj))
        //    //{
        //    //    return false;
        //    //}
        //    //Delegate @delegate = (Delegate)obj;
        //    //if (this._target == @delegate._target && this._methodPtr == @delegate._methodPtr && this._methodPtrAux == @delegate._methodPtrAux)
        //    //{
        //    //    return true;
        //    //}
        //    //if (this._methodPtrAux.IsNull())
        //    //{
        //    //    if (!@delegate._methodPtrAux.IsNull())
        //    //    {
        //    //        return false;
        //    //    }
        //    //    if (this._target != @delegate._target)
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    if (@delegate._methodPtrAux.IsNull())
        //    //    {
        //    //        return false;
        //    //    }
        //    //    if (this._methodPtrAux == @delegate._methodPtrAux)
        //    //    {
        //    //        return true;
        //    //    }
        //    //}
        //    //if (this._methodBase == null || @delegate._methodBase == null || !(this._methodBase is MethodInfo) || !(@delegate._methodBase is MethodInfo))
        //    //{
        //    //    return Delegate.InternalEqualMethodHandles(this, @delegate);
        //    //}
        //    //return this._methodBase.Equals(@delegate._methodBase);
        //}
        ///// <summary>Returns a hash code for the delegate.</summary>
        ///// <returns>A hash code for the delegate.</returns>
        ///// <filterpriority>2</filterpriority>
        //public override int GetHashCode()
        //{
        //    return base.GetType().GetHashCode();
        //}
        ///// <summary>Concatenates the invocation lists of two delegates.</summary>
        ///// <returns>A new delegate with an invocation list that concatenates the invocation lists of <paramref name="a" /> and <paramref name="b" /> in that order. Returns <paramref name="a" /> if <paramref name="b" /> is null, returns <paramref name="b" /> if <paramref name="a" /> is a null reference, and returns a null reference if both <paramref name="a" /> and <paramref name="b" /> are null references.</returns>
        ///// <param name="a">The delegate whose invocation list comes first. </param>
        ///// <param name="b">The delegate whose invocation list comes last. </param>
        ///// <exception cref="T:System.ArgumentException">Both <paramref name="a" /> and <paramref name="b" /> are not null, and <paramref name="a" /> and <paramref name="b" /> are not instances of the same delegate type. </exception>
        ///// <filterpriority>1</filterpriority>
        //public static Delegate Combine(Delegate a, Delegate b)
        //{
        //    if (a == null)
        //    {
        //        return b;
        //    }
        //    return a.CombineImpl(b);
        //}
        ///// <summary>Concatenates the invocation lists of an array of delegates.</summary>
        ///// <returns>A new delegate with an invocation list that concatenates the invocation lists of the delegates in the <paramref name="delegates" /> array. Returns null if <paramref name="delegates" /> is null, if <paramref name="delegates" /> contains zero elements, or if every entry in <paramref name="delegates" /> is null.</returns>
        ///// <param name="delegates">The array of delegates to combine. </param>
        ///// <exception cref="T:System.ArgumentException">Not all the non-null entries in <paramref name="delegates" /> are instances of the same delegate type. </exception>
        ///// <filterpriority>1</filterpriority>
        //public static Delegate Combine(params Delegate[] delegates)
        //{
        //    if (delegates == null || delegates.Length == 0)
        //    {
        //        return null;
        //    }
        //    Delegate @delegate = delegates[0];
        //    for (int i = 1; i < delegates.Length; i++)
        //    {
        //        @delegate = Delegate.Combine(@delegate, delegates[i]);
        //    }
        //    return @delegate;
        //}
        ///// <summary>Returns the invocation list of the delegate.</summary>
        ///// <returns>An array of delegates representing the invocation list of the current delegate.</returns>
        ///// <filterpriority>2</filterpriority>
        //public virtual Delegate[] GetInvocationList()
        //{
        //    return new Delegate[]
        //    {
        //        this
        //    };
        //}
        ///// <summary>Gets the static method represented by the current delegate.</summary>
        ///// <returns>A <see cref="T:System.Reflection.MethodInfo" /> describing the static method represented by the current delegate.</returns>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        //protected virtual MethodInfo GetMethodImpl()
        //{
        //    throw new Exception();
        //    //if (this._methodBase == null || !(this._methodBase is MethodInfo))
        //    //{
        //    //    IRuntimeMethodInfo runtimeMethodInfo = this.FindMethodHandle();
        //    //    RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(runtimeMethodInfo);
        //    //    if ((RuntimeTypeHandle.IsGenericTypeDefinition(runtimeType) || RuntimeTypeHandle.HasInstantiation(runtimeType)) && (RuntimeMethodHandle.GetAttributes(runtimeMethodInfo) & MethodAttributes.Static) == MethodAttributes.PrivateScope)
        //    //    {
        //    //        if (this._methodPtrAux == (IntPtr)0)
        //    //        {
        //    //            Type type = this._target.GetType();
        //    //            Type genericTypeDefinition = runtimeType.GetGenericTypeDefinition();
        //    //            while (type != null)
        //    //            {
        //    //                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
        //    //                {
        //    //                    runtimeType = (type as RuntimeType);
        //    //                    break;
        //    //                }
        //    //                type = type.BaseType;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            MethodInfo method = base.GetType().GetMethod("Invoke");
        //    //            runtimeType = (RuntimeType)method.GetParameters()[0].ParameterType;
        //    //        }
        //    //    }
        //    //    this._methodBase = (MethodInfo)RuntimeType.GetMethodBase(runtimeType, runtimeMethodInfo);
        //    //}
        //    //return (MethodInfo)this._methodBase;
        //}
        ///// <summary>Removes the last occurrence of the invocation list of a delegate from the invocation list of another delegate.</summary>
        ///// <returns>A new delegate with an invocation list formed by taking the invocation list of <paramref name="source" /> and removing the last occurrence of the invocation list of <paramref name="value" />, if the invocation list of <paramref name="value" /> is found within the invocation list of <paramref name="source" />. Returns <paramref name="source" /> if <paramref name="value" /> is null or if the invocation list of <paramref name="value" /> is not found within the invocation list of <paramref name="source" />. Returns a null reference if the invocation list of <paramref name="value" /> is equal to the invocation list of <paramref name="source" /> or if <paramref name="source" /> is a null reference.</returns>
        ///// <param name="source">The delegate from which to remove the invocation list of <paramref name="value" />. </param>
        ///// <param name="value">The delegate that supplies the invocation list to remove from the invocation list of <paramref name="source" />. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        ///// <exception cref="T:System.ArgumentException">The delegate types do not match.</exception>
        ///// <filterpriority>1</filterpriority>
        //public static Delegate Remove(Delegate source, Delegate value)
        //{
        //    throw new Exception();
        //    //if (source == null)
        //    //{
        //    //    return null;
        //    //}
        //    //if (value == null)
        //    //{
        //    //    return source;
        //    //}
        //    //if (!Delegate.InternalEqualTypes(source, value))
        //    //{
        //    //    throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTypeMis"));
        //    //}
        //    //return source.RemoveImpl(value);
        //}
        ///// <summary>Removes all occurrences of the invocation list of a delegate from the invocation list of another delegate.</summary>
        ///// <returns>A new delegate with an invocation list formed by taking the invocation list of <paramref name="source" /> and removing all occurrences of the invocation list of <paramref name="value" />, if the invocation list of <paramref name="value" /> is found within the invocation list of <paramref name="source" />. Returns <paramref name="source" /> if <paramref name="value" /> is null or if the invocation list of <paramref name="value" /> is not found within the invocation list of <paramref name="source" />. Returns a null reference if the invocation list of <paramref name="value" /> is equal to the invocation list of <paramref name="source" />, if <paramref name="source" /> contains only a series of invocation lists that are equal to the invocation list of <paramref name="value" />, or if <paramref name="source" /> is a null reference.</returns>
        ///// <param name="source">The delegate from which to remove the invocation list of <paramref name="value" />. </param>
        ///// <param name="value">The delegate that supplies the invocation list to remove from the invocation list of <paramref name="source" />. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        ///// <exception cref="T:System.ArgumentException">The delegate types do not match.</exception>
        ///// <filterpriority>1</filterpriority>
        //public static Delegate RemoveAll(Delegate source, Delegate value)
        //{
        //    Delegate @delegate;
        //    do
        //    {
        //        @delegate = source;
        //        source = Delegate.Remove(source, value);
        //    }
        //    while (@delegate != source);
        //    return @delegate;
        //}
        ///// <summary>Concatenates the invocation lists of the specified multicast (combinable) delegate and the current multicast (combinable) delegate.</summary>
        ///// <returns>A new multicast (combinable) delegate with an invocation list that concatenates the invocation list of the current multicast (combinable) delegate and the invocation list of <paramref name="d" />, or the current multicast (combinable) delegate if <paramref name="d" /> is null.</returns>
        ///// <param name="d">The multicast (combinable) delegate whose invocation list to append to the end of the invocation list of the current multicast (combinable) delegate. </param>
        ///// <exception cref="T:System.MulticastNotSupportedException">Always thrown. </exception>
        //protected virtual Delegate CombineImpl(Delegate d)
        //{
        //    throw new MulticastNotSupportedException(Environment.GetResourceString("Multicast_Combine"));
        //}
        ///// <summary>Removes the invocation list of a delegate from the invocation list of another delegate.</summary>
        ///// <returns>A new delegate with an invocation list formed by taking the invocation list of the current delegate and removing the invocation list of <paramref name="value" />, if the invocation list of <paramref name="value" /> is found within the current delegate's invocation list. Returns the current delegate if <paramref name="value" /> is null or if the invocation list of <paramref name="value" /> is not found within the current delegate's invocation list. Returns null if the invocation list of <paramref name="value" /> is equal to the current delegate's invocation list.</returns>
        ///// <param name="d">The delegate that supplies the invocation list to remove from the invocation list of the current delegate. </param>
        ///// <exception cref="T:System.MemberAccessException">The caller does not have access to the method represented by the delegate (for example, if the method is private). </exception>
        //protected virtual Delegate RemoveImpl(Delegate d)
        //{
        //    if (!d.Equals(this))
        //    {
        //        return this;
        //    }
        //    return null;
        //}
        /// <summary>Creates a shallow copy of the delegate.</summary>
        /// <returns>A shallow copy of the delegate.</returns>
        /// <filterpriority>2</filterpriority>
        public virtual object Clone()
        {
            throw new Exception();
            //return base.MemberwiseClone();
        }
        ///// <summary>Creates a delegate of the specified type that represents the specified instance method to invoke on the specified class instance.</summary>
        ///// <returns>A delegate of the specified type that represents the specified instance method to invoke on the specified class instance.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The class instance on which <paramref name="method" /> is invoked. </param>
        ///// <param name="method">The name of the instance method that the delegate is to represent. </param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />. -or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection.-or- <paramref name="method" /> is not an instance method. -or-<paramref name="method" /> cannot be bound, for example because it cannot be found.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, object target, string method)
        //{
        //    return Delegate.CreateDelegate(type, target, method, false, true);
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified instance method to invoke on the specified class instance with the specified case-sensitivity.</summary>
        ///// <returns>A delegate of the specified type that represents the specified instance method to invoke on the specified class instance.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The class instance on which <paramref name="method" /> is invoked. </param>
        ///// <param name="method">The name of the instance method that the delegate is to represent. </param>
        ///// <param name="ignoreCase">A Boolean indicating whether to ignore the case when comparing the name of the method. </param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection.-or- <paramref name="method" /> is not an instance method. -or-<paramref name="method" /> cannot be bound, for example because it cannot be found.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase)
        //{
        //    return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified instance method to invoke on the specified class instance, with the specified case-sensitivity and the specified behavior on failure to bind.</summary>
        ///// <returns>A delegate of the specified type that represents the specified instance method to invoke on the specified class instance.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The class instance on which <paramref name="method" /> is invoked. </param>
        ///// <param name="method">The name of the instance method that the delegate is to represent. </param>
        ///// <param name="ignoreCase">A Boolean indicating whether to ignore the case when comparing the name of the method. </param>
        ///// <param name="throwOnBindFailure">true to throw an exception if <paramref name="method" /> cannot be bound; otherwise, false.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-  <paramref name="method" /> is not an instance method. -or-<paramref name="method" /> cannot be bound, for example because it cannot be found, and <paramref name="throwOnBindFailure" /> is true.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException("type");
        //    }
        //    if (target == null)
        //    {
        //        throw new ArgumentNullException("target");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("method");
        //    }
        //    RuntimeType runtimeType = type as RuntimeType;
        //    if (runtimeType == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
        //    }
        //    if (!runtimeType.IsDelegate())
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
        //    }
        //    Delegate @delegate = Delegate.InternalAlloc(runtimeType);
        //    if (!@delegate.BindToMethodName(target, (RuntimeType)target.GetType(), method, (DelegateBindingFlags)26 | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags)0))))
        //    {
        //        if (throwOnBindFailure)
        //        {
        //            throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
        //        }
        //        @delegate = null;
        //    }
        //    return @delegate;
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified static method of the specified class.</summary>
        ///// <returns>A delegate of the specified type that represents the specified static method of the specified class.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The <see cref="T:System.Type" /> representing the class that implements <paramref name="method" />. </param>
        ///// <param name="method">The name of the static method that the delegate is to represent. </param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or- <paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="target" /> is not a RuntimeType.-or-<paramref name="target" /> is an open generic type. That is, its <see cref="P:System.Type.ContainsGenericParameters" /> property is true.-or-<paramref name="method" /> is not a static method (Shared method in Visual Basic). -or-<paramref name="method" /> cannot be bound, for example because it cannot be found, and <paramref name="throwOnBindFailure" /> is true.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, Type target, string method)
        //{
        //    return Delegate.CreateDelegate(type, target, method, false, true);
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified static method of the specified class, with the specified case-sensitivity.</summary>
        ///// <returns>A delegate of the specified type that represents the specified static method of the specified class.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The <see cref="T:System.Type" /> representing the class that implements <paramref name="method" />. </param>
        ///// <param name="method">The name of the static method that the delegate is to represent. </param>
        ///// <param name="ignoreCase">A Boolean indicating whether to ignore the case when comparing the name of the method.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or- <paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="target" /> is not a RuntimeType.-or-<paramref name="target" /> is an open generic type. That is, its <see cref="P:System.Type.ContainsGenericParameters" /> property is true.-or-<paramref name="method" /> is not a static method (Shared method in Visual Basic). -or-<paramref name="method" /> cannot be bound, for example because it cannot be found.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase)
        //{
        //    return Delegate.CreateDelegate(type, target, method, ignoreCase, true);
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified static method of the specified class, with the specified case-sensitivity and the specified behavior on failure to bind.</summary>
        ///// <returns>A delegate of the specified type that represents the specified static method of the specified class.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="target">The <see cref="T:System.Type" /> representing the class that implements <paramref name="method" />. </param>
        ///// <param name="method">The name of the static method that the delegate is to represent. </param>
        ///// <param name="ignoreCase">A Boolean indicating whether to ignore the case when comparing the name of the method.</param>
        ///// <param name="throwOnBindFailure">true to throw an exception if <paramref name="method" /> cannot be bound; otherwise, false.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="target" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or- <paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="target" /> is not a RuntimeType.-or-<paramref name="target" /> is an open generic type. That is, its <see cref="P:System.Type.ContainsGenericParameters" /> property is true.-or-<paramref name="method" /> is not a static method (Shared method in Visual Basic). -or-<paramref name="method" /> cannot be bound, for example because it cannot be found, and <paramref name="throwOnBindFailure" /> is true. </exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, Type target, string method, bool ignoreCase, bool throwOnBindFailure)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException("type");
        //    }
        //    if (target == null)
        //    {
        //        throw new ArgumentNullException("target");
        //    }
        //    if (target.IsGenericType && target.ContainsGenericParameters)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_UnboundGenParam"), "target");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("method");
        //    }
        //    RuntimeType runtimeType = type as RuntimeType;
        //    RuntimeType runtimeType2 = target as RuntimeType;
        //    if (runtimeType == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
        //    }
        //    if (runtimeType2 == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "target");
        //    }
        //    if (!runtimeType.IsDelegate())
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
        //    }
        //    Delegate @delegate = Delegate.InternalAlloc(runtimeType);
        //    if (!@delegate.BindToMethodName(null, runtimeType2, method, (DelegateBindingFlags)5 | (ignoreCase ? DelegateBindingFlags.CaselessMatching : ((DelegateBindingFlags)0))))
        //    {
        //        if (throwOnBindFailure)
        //        {
        //            throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
        //        }
        //        @delegate = null;
        //    }
        //    return @delegate;
        //}
        ///// <summary>Creates a delegate of the specified type to represent the specified static method, with the specified behavior on failure to bind.</summary>
        ///// <returns>A delegate of the specified type to represent the specified static method.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> describing the static or instance method the delegate is to represent.</param>
        ///// <param name="throwOnBindFailure">true to throw an exception if <paramref name="method" /> cannot be bound; otherwise, false.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="method" /> cannot be bound, and <paramref name="throwOnBindFailure" /> is true.-or-<paramref name="method" /> is not a RuntimeMethodInfo. See Runtime Types in Reflection.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException("type");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("method");
        //    }
        //    RuntimeType runtimeType = type as RuntimeType;
        //    if (runtimeType == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
        //    }
        //    RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
        //    if (runtimeMethodInfo == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
        //    }
        //    if (!runtimeType.IsDelegate())
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
        //    }
        //    StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
        //    Delegate @delegate = Delegate.CreateDelegateInternal(runtimeType, runtimeMethodInfo, null, (DelegateBindingFlags)132, ref stackCrawlMark);
        //    if (@delegate == null && throwOnBindFailure)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
        //    }
        //    return @delegate;
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified static or instance method, with the specified first argument.</summary>
        ///// <returns>A delegate of the specified type that represents the specified static or instance method. </returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="firstArgument">The object to which the delegate is bound, or null to treat <paramref name="method" /> as static (Shared in Visual Basic). </param>
        ///// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> describing the static or instance method the delegate is to represent.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="method" /> cannot be bound.-or-<paramref name="method" /> is not a RuntimeMethodInfo. See Runtime Types in Reflection.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method)
        //{
        //    return Delegate.CreateDelegate(type, firstArgument, method, true);
        //}
        ///// <summary>Creates a delegate of the specified type that represents the specified static or instance method, with the specified first argument and the specified behavior on failure to bind.</summary>
        ///// <returns>A delegate of the specified type that represents the specified static or instance method, or null if <paramref name="throwOnBindFailure" /> is false and the delegate cannot be bound to <paramref name="method" />. </returns>
        ///// <param name="type">A <see cref="T:System.Type" /> representing the type of delegate to create. </param>
        ///// <param name="firstArgument">An <see cref="T:System.Object" /> that is the first argument of the method the delegate represents. For instance methods, it must be compatible with the instance type. </param>
        ///// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> describing the static or instance method the delegate is to represent.</param>
        ///// <param name="throwOnBindFailure">true to throw an exception if <paramref name="method" /> cannot be bound; otherwise, false.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or-<paramref name="method" /> cannot be bound, and <paramref name="throwOnBindFailure" /> is true.-or-<paramref name="method" /> is not a RuntimeMethodInfo. See Runtime Types in Reflection.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, object firstArgument, MethodInfo method, bool throwOnBindFailure)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException("type");
        //    }
        //    if (method == null)
        //    {
        //        throw new ArgumentNullException("method");
        //    }
        //    RuntimeType runtimeType = type as RuntimeType;
        //    if (runtimeType == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
        //    }
        //    RuntimeMethodInfo runtimeMethodInfo = method as RuntimeMethodInfo;
        //    if (runtimeMethodInfo == null)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "method");
        //    }
        //    if (!runtimeType.IsDelegate())
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDelegate"), "type");
        //    }
        //    StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
        //    Delegate @delegate = Delegate.CreateDelegateInternal(runtimeType, runtimeMethodInfo, firstArgument, DelegateBindingFlags.RelaxedSignature, ref stackCrawlMark);
        //    if (@delegate == null && throwOnBindFailure)
        //    {
        //        throw new ArgumentException(Environment.GetResourceString("Arg_DlgtTargMeth"));
        //    }
        //    return @delegate;
        //}
        ///// <summary>Determines whether the specified delegates are equal.</summary>
        ///// <returns>true if <paramref name="d1" /> is equal to <paramref name="d2" />; otherwise, false.</returns>
        ///// <param name="d1">The first delegate to compare. </param>
        ///// <param name="d2">The second delegate to compare. </param>
        ///// <filterpriority>3</filterpriority>
        //public static bool operator ==(Delegate d1, Delegate d2)
        //{
        //    if (d1 == null)
        //    {
        //        return d2 == null;
        //    }
        //    return d1.Equals(d2);
        //}
        ///// <summary>Determines whether the specified delegates are not equal.</summary>
        ///// <returns>true if <paramref name="d1" /> is not equal to <paramref name="d2" />; otherwise, false.</returns>
        ///// <param name="d1">The first delegate to compare. </param>
        ///// <param name="d2">The second delegate to compare. </param>
        ///// <filterpriority>3</filterpriority>
        //public static bool operator !=(Delegate d1, Delegate d2)
        //{
        //    if (d1 == null)
        //    {
        //        return d2 != null;
        //    }
        //    return !d1.Equals(d2);
        //}
        /////// <summary>Not supported.</summary>
        /////// <param name="info">Not supported. </param>
        /////// <param name="context">Not supported. </param>
        /////// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
        /////// <filterpriority>2</filterpriority>
        ////public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        ////{
        ////    throw new NotSupportedException();
        ////}
        ///// <summary>Creates a delegate of the specified type to represent the specified static method.</summary>
        ///// <returns>A delegate of the specified type to represent the specified static method.</returns>
        ///// <param name="type">The <see cref="T:System.Type" /> of delegate to create. </param>
        ///// <param name="method">The <see cref="T:System.Reflection.MethodInfo" /> describing the static or instance method the delegate is to represent. Only static methods are supported in the .NET Framework version 1.0 and 1.1.</param>
        ///// <exception cref="T:System.ArgumentNullException">
        /////   <paramref name="type" /> is null.-or- <paramref name="method" /> is null. </exception>
        ///// <exception cref="T:System.ArgumentException">
        /////   <paramref name="type" /> does not inherit <see cref="T:System.MulticastDelegate" />.-or-<paramref name="type" /> is not a RuntimeType. See Runtime Types in Reflection. -or- <paramref name="method" /> is not a static method, and the .NET Framework version is 1.0 or 1.1. -or-<paramref name="method" /> cannot be bound.-or-<paramref name="method" /> is not a RuntimeMethodInfo. See Runtime Types in Reflection.</exception>
        ///// <exception cref="T:System.MissingMethodException">The Invoke method of <paramref name="type" /> is not found. </exception>
        ///// <exception cref="T:System.MethodAccessException">The caller does not have the permissions necessary to access <paramref name="method" />. </exception>
        ///// <filterpriority>1</filterpriority>
        ///// <PermissionSet>
        /////   <IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess" />
        ///// </PermissionSet>
        //public static Delegate CreateDelegate(Type type, MethodInfo method)
        //{
        //    return Delegate.CreateDelegate(type, method, true);
        //}
    }
}
