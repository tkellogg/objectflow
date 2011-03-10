using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rainbow.ObjectFlow.Interfaces;
using Rainbow.ObjectFlow.Engine;

namespace Rainbow.ObjectFlow.Helpers
{
    public static class Declare
    {
        /// <summary>
        /// Declare a step that will be defined later
        /// </summary>
        /// <returns>An uninitialized operation reference</returns>
        public static IDeclaredOperation Step()
        {
            return new DeclaredOperation();
        }
    }
}
