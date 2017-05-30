using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace thelab.core
{

    /// <summary>
    /// Generic version for the Reflection class. Accepts any type of objects.
    /// </summary>
    public class Reflection : Reflection<object> { }


    /// <summary>
    /// Wrapper class for name-based access to attributes, properties and methods of objects or static classes.
    /// </summary>
    /// <typeparam name="T">Type of the object being handled.</typeparam>
    public class Reflection<T> {

        #region static

        static private BindingFlags m_flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Checks if a given Type instance inherits from a given 'Base' class.
        /// </summary>
        /// <typeparam name="Base">Base class from whom p_type inherits.</typeparam>
        /// <param name="p_type">Type that will be checked.</param>
        /// <returns>Flag that indicates if the Type inherits from Base.</returns>
        static public bool InheritFrom<Base>(Type p_type)
        {
            Type t = p_type;
            Type ut = typeof(Base);
            if (t == ut) return true;
            return t.IsSubclassOf(ut);
        }

        /// <summary>
        /// Checks if a given Type inherits from a given 'Base' class.
        /// </summary>
        /// <typeparam name="Base">Base class from whom Type inherits.</typeparam>
        /// <typeparam name="Type">Type that will be checked.</typeparam>
        /// <returns>Flag that indicates if the Type inherits from Base.</returns>
        static public bool InheritFrom<Base, Type>()
        {
            return InheritFrom<Base>(typeof(Type));
        }

        /// <summary>
        /// Returns all methods from a given instance of type 'T'. Not only public methods, but all of them.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <returns>List of MethodInfo describing all object's methods.</returns>
        static public MethodInfo[] GetMethods(T p_target)
        {
            return p_target.GetType().GetMethods(m_flags);
        }

        /// <summary>
        /// Returns the first occurence of a method with name 'p_name' from a given instance of type 'T'. The method accessors does not matter.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_name">Name of the method.</param>        
        /// <returns>MethodInfo of the target method or null if it does not exists.</returns>
        static public MethodInfo GetMethod(T p_target, string p_name)
        {
            Type t = p_target.GetType();
            List<MethodInfo> ml = new List<MethodInfo>(t.GetMethods(m_flags));
            MethodInfo res = ml.Find(delegate(MethodInfo it)
            {
                return it.Name == p_name;
            });
            return res;
            //return p_signature.Length<=0 ? t.GetMethod(p_name,m_flags) : t.GetMethod(p_name,m_flags,Type.DefaultBinder,p_signature,null);
        }

        /// <summary>
        /// Returns the value of a property named 'p_property' of the given object instance.
        /// </summary>
        /// <typeparam name="U">Type of the property.</typeparam>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_property">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        static public U Get<U>(T p_target, string p_property)
        {
            Type t = p_target.GetType();
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) return (U)(object)fi.GetValue(p_target);
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) return (U)(object)pi.GetValue(p_target, null);
            return default(U);
        }

        /// <summary>
        /// Returns the value of a property named 'p_property' of the given object instance.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_property">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        static public object Get(T p_target, string p_property)
        {
            Type t = p_target.GetType();
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) return fi.GetValue(p_target);
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) return pi.GetValue(p_target, null);
            return null;
        }


        /// <summary>
        /// Returns the value of a static property named 'p_property' of the type 'T'.
        /// </summary>
        /// <typeparam name="U">Type of the property.</typeparam>
        /// <param name="p_property">Name of the property.</param>
        /// <returns>The value of the static property.</returns>
        static public U Get<U>(string p_property)
        {
            Type t = typeof(T);
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) return (U)(object)fi.GetValue(t);
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) return (U)(object)pi.GetValue(t, null);
            return default(U);
        }

        /// <summary>
        /// Checks if a given object instance contains a property named 'p_property'.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_property">Name of the property.</param>
        /// <returns>Flag that indicates if the property exists.</returns>
        static public bool Contains(T p_target, string p_property)
        {
            Type t = p_target.GetType();
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) return true;
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) return true;
            MethodInfo mi = t.GetMethod(p_property, m_flags);
            if (mi != null) return true;
            return false;
        }

        /// <summary>
        /// Checks if the type 'T' contains a static property named 'p_property'.
        /// </summary>
        /// <param name="p_property">Name of the property.</param>
        /// <returns>Flag that indicates if the property exists.</returns>
        static public bool Contains(string p_property)
        {
            Type t = typeof(T);
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) return true;
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) return true;
            MethodInfo mi = t.GetMethod(p_property, m_flags);
            if (mi != null) return true;
            return false;
        }

        /// <summary>
        /// Sets the value of a property named 'p_property' of the given object instance.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_property">Name of the property.</param>
        /// <param name="p_value">Value to be set.</param>
        /// <param name="p_number_implicity_cast">Flag that indicates if between numeric types the values will be internally cast to the correct type. If 'false' calls between int/float will cause errors.</param>
        static public void Set(T p_target, string p_property, object p_value, bool p_number_implicity_cast)
        {
            Type t = p_target.GetType();
            MemberInfo[] i = t.GetMember(p_property, MemberTypes.Field | MemberTypes.Property, m_flags);
            while (null != t)
            {
                if (i.Length > 0) break;
                t = t.BaseType;
                if (t == null) return;
                i = t.GetMember(p_property, MemberTypes.Field | MemberTypes.Property, m_flags);
            }
            switch (i[0].MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = ((PropertyInfo)i[0]);
                    if (!p_number_implicity_cast) pi.SetValue(p_target, p_value, null);
                    else
                    {
                        Type vtype = p_value.GetType();
                        Type mtype = pi.PropertyType;
                        if (vtype == mtype)
                        {
                            pi.SetValue(p_target, p_value, null);
                        }
                        else
                        {
                            if (mtype == typeof(float))
                            {
                                if (p_value is int) pi.SetValue(p_target, (float)(int)p_value, null);
                                else
                                    if (p_value is double) pi.SetValue(p_target, (float)(double)p_value, null);
                            }
                            else
                            {
                                if (p_value is float) pi.SetValue(p_target, (int)(float)p_value, null);
                                else
                                    if (p_value is double) pi.SetValue(p_target, (int)(double)p_value, null);
                            }
                        }
                    }
                    break;

                case MemberTypes.Field:
                    FieldInfo fi = ((FieldInfo)i[0]);
                    if (!p_number_implicity_cast) fi.SetValue(p_target, p_value);
                    else
                    {
                        Type vtype = p_value.GetType();
                        Type mtype = fi.FieldType;
                        if (vtype == mtype)
                        {
                            fi.SetValue(p_target, p_value);
                        }
                        else
                        {
                            if (mtype == typeof(float))
                            {
                                if (p_value is int) fi.SetValue(p_target, (float)(int)p_value);
                                else
                                    if (p_value is double) fi.SetValue(p_target, (float)(double)p_value);
                            }
                            else
                            {
                                if (p_value is float) fi.SetValue(p_target, (int)(float)p_value);
                                else
                                    if (p_value is double) fi.SetValue(p_target, (int)(double)p_value);
                            }
                        }
                    }
                    break;
            }

        }


        /// <summary>
        /// Sets the value of a property named 'p_property' of the given object instance. No type safety is checked for numeric values.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_property">Name of the property.</param>
        /// <param name="p_value">Value to be set.</param>
        static public void Set(T p_target, string p_property, object p_value)
        {
            Set(p_target, p_property, p_value, false);
        }



        /// <summary>
        /// Sets the value of a static property named 'p_property' of the given type 'T'.
        /// </summary>
        /// <param name="p_property">Name of the property.</param>
        /// <param name="p_value">Value to be set.</param>
        static public void Set(string p_property, object p_value)
        {
            Type t = typeof(T);
            FieldInfo fi = t.GetField(p_property, m_flags);
            if (fi != null) { fi.SetValue(t, p_value); return; }
            PropertyInfo pi = t.GetProperty(p_property, m_flags);
            if (pi != null) { pi.SetValue(t, p_value, null); return; }
        }

        /// <summary>
        /// Invokes the method 'p_method' inside the instance of a given object.
        /// </summary>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_method">Method to be called.</param>
        /// <param name="p_args">Method arguments.</param>
        /// <returns>Return value of the method call.</returns>
        static public object Invoke(T p_target, string p_method, params object[] p_args)
        {
            Type t = p_target.GetType();
            Type[] signature = new Type[p_args.Length];
            for (int i = 0; i < signature.Length; i++) signature[i] = p_args[i].GetType();

            //MethodInfo mi = t.GetMethod(p_method, m_flags);            
            MethodInfo mi = t.GetMethod(p_method, m_flags, Type.DefaultBinder, signature, null);

            //if (mi == null) return null;
            return mi.Invoke(p_target, p_args);
        }

        /// <summary>
        /// Invokes the method 'p_method' inside the instance of a given object.
        /// </summary>
        /// <typeparam name="U">Type of the method's return data.</typeparam>
        /// <param name="p_target">Instance of the object.</param>
        /// <param name="p_method">Method to be called.</param>
        /// <param name="p_args">Method arguments.</param>
        /// <returns>Return value of the method call.</returns>
        static public U Invoke<U>(T p_target, string p_method, params object[] p_args)
        {
            return (U)Invoke(p_target, p_method, p_args);
        }

        /// <summary>
        /// Invokes the static method 'p_method' of the type 'T'.
        /// </summary>
        /// <param name="p_method">Method to be called.</param>
        /// <param name="p_args">Method arguments.</param>
        /// <returns>Return value of the method call.</returns>
        static public object Invoke(string p_method, params object[] p_args)
        {
            Type t = typeof(T);
            Type[] signature = new Type[p_args.Length];
            for (int i = 0; i < signature.Length; i++) signature[i] = p_args[i].GetType();

            //MethodInfo mi = t.GetMethod(p_method, m_flags);            
            MethodInfo mi = t.GetMethod(p_method, m_flags, Type.DefaultBinder, signature, null);
            //if (mi == null) return null;
            return mi.Invoke(t, p_args);
        }

        /// <summary>
        /// Invokes the static method 'p_method' of the type 'T'.
        /// </summary>
        /// <typeparam name="U">Type of the method's return data.</typeparam>
        /// <param name="p_method">Method to be called.</param>
        /// <param name="p_args">Method arguments.</param>
        /// <returns>Return value of the method call.</returns>
        static public U Invoke<U>(string p_method, params object[] p_args)
        {
            return (U)Invoke(p_method, p_args);
        }


        /// <summary>
        /// Creates a new instance of type 'T'. This is similar to calling 'new'.
        /// </summary>
        /// <param name="p_args">Arguments of the constructor.</param>
        /// <returns>New instance of class 'T'.</returns>
        static public T New(params object[] p_args)
        {
            Type t = typeof(T);
            Type[] l = new Type[p_args.Length];
            for (int i = 0; i < l.Length; i++) l[i] = p_args[i].GetType();
            ConstructorInfo ci = t.GetConstructor(l);
            if (ci == null) return default(T);
            return (T)ci.Invoke(p_args);
        }

        /// <summary>
        /// Creates a new instance of type 'p_type'. This is similar to calling 'new'.
        /// </summary>
        /// <param name="p_type">Type of the class being instantiated.</param>
        /// <param name="p_args">Arguments of the constructor.</param>
        /// <returns>New instance of class 'p_type'.</returns>
        static public T New(Type p_type, params object[] p_args)
        {
            Type t = p_type;
            Type[] l = new Type[p_args.Length];
            for (int i = 0; i < l.Length; i++) l[i] = p_args[i].GetType();

            ConstructorInfo ci = t.GetConstructor(l);
            if (ci == null) return default(T);
            return (T)ci.Invoke(p_args);
        }


        #endregion

    }
}
