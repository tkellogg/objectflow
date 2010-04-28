using System;
using Rainbow.ObjectFlow.Interfaces;

namespace Rainbow.ObjectFlow.Framework
{
    internal static class Check
    {
        public static void IsNotNull<T>(IOperation<T> operation, string paramName)
        {
            if (operation == null)
            {
                DefaultNullArgumentAction(paramName);
            }
        }

        public static void IsNotNull<T>(ICheckContraint operation, string paramName)
        {
            if (operation == null)
            {
                DefaultNullArgumentAction(paramName);
            }
        }

        private static void DefaultNullArgumentAction(string paramName)
        {
            string message = string.Format("Argument [{0}] cannot be null", paramName);
            throw new ArgumentNullException(paramName, message);
        }
    }
}