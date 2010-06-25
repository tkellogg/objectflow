using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    internal static class Check
    {
        public static void IsNotNull(object shouldNoteNull, string paramName)
        {
            if (shouldNoteNull == null)
            {
                DefaultNullArgumentAction(paramName);
            }
        }

        private static void DefaultNullArgumentAction(string paramName)
        {
            string message = string.Format("Argument [{0}] cannot be null", paramName);
            throw new ArgumentNullException(paramName, message);
        }

        public static void IsInstanceOf<T>(object operation, string paramName)
        {
            if (!typeof(T).IsAssignableFrom(operation.GetType()))
            {
                string message = string.Format("Argument should be of type {0}.{1}", typeof(T).Namespace, typeof(T).Name);
                Exception ex = new InvalidCastException(message);
                throw (ex);
            }
        }
    }
}