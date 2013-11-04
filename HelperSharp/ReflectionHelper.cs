using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HelperSharp
{
    /// <summary>
    /// Reflection helpers.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gest the property with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property.</returns>
        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            PropertyInfo property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
            {
                throw new InvalidOperationException("The property '{0}' was not found on type '{1}'.".With(propertyName, type));
            }

            return property;
        }

        /// <summary>
        /// Gets the property's value.
        /// </summary>
        /// <param name="instance">The object.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property's value.</returns>
        public static object GetPropertyValue(object instance, string propertyName)
        {
            ExceptionHelper.ThrowIfNull("instance", instance);

            return GetProperty(instance.GetType(), propertyName).GetValue(instance, new object[0]);
        }

        /// <summary>
        /// Invokes the method with the specified parameters.
        /// </summary>
        /// <param name="target">The target object where the method will be called.</param>
        /// <param name="methodName">The method's name.</param>
        /// <param name="parameters">The method's parameters.</param>
        /// <returns>The method's result.</returns>
        public static object InvokeMethod(object target, string methodName, params object[] parameters)
        {
            ExceptionHelper.ThrowIfNull("target", target);
            ExceptionHelper.ThrowIfNull("methodName", methodName);

            Type objType = target.GetType();

            if (parameters == null)
            {
                parameters = new object[0];
            }

            MethodInfo method = objType.GetMethod(methodName, parameters.Select(p => p.GetType()).ToArray());

            if (method == null)
            {
                throw new InvalidOperationException("The method '{0}' was not found on type '{1}'.".With(methodName, objType.Name));
            }

            return method.Invoke(target, parameters);
        }

        /// <summary>
        /// Gets the subclasses of the specified type.
        /// </summary>
        /// <param name="type">The base type.</param>
        /// <param name="onlyConcrete">If should return only concrete subclasses or if should return abstract and interfaces too. Default is true.</param>
        /// <returns>The subclasses.</returns>
        public static IList<Type> GetSubclassesOf(Type type, bool onlyConcrete = true)
        {
            ExceptionHelper.ThrowIfNull("type", type);

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t != type);

            if (onlyConcrete)
            {
                types = types.Where(t => !t.IsInterface && !t.IsAbstract);
            }

            return types.ToList();
        }

        /// <summary>
        /// Gets the subclasses of the specified type.
        /// </summary>
        /// <typeparam name="TType">The base type.</typeparam>
        /// <param name="onlyConcrete">If should return only concrete subclasses or if should return abstract and interfaces too. Default is true.</param>
        /// <returns>The subclasses.</returns>
        public static IList<Type> GetSubclassesOf<TType>(bool onlyConcrete = true)
        {
            return GetSubclassesOf(typeof(TType), onlyConcrete);
        }
    }
}
