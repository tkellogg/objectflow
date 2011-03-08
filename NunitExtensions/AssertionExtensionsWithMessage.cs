using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Rainbow.Nunit.Extensions
{
    public static class AssertionExtensionsWithMessage
    {
        public static void ShouldBeNull(this object obj, string message)
        {
            Assert.IsNull(obj, message);
        }

        public static void ShouldNotbeNull(this object obj, string message)
        {
            Assert.IsNotNull(obj, message);
        }

        public static void ShouldBe(this object str, object value)
        {
            Assert.That(str.ToString(), Is.EqualTo(value));
        }

        public static void ShouldBe(this object i, int value)
        {
            Assert.That(i, Is.EqualTo(value));
        }

        public static void IsInstanceOf<T>(this object obj)
        {
            Assert.IsInstanceOf(typeof(T), obj);
        }

        public static void ShouldEqual(this object obj, object value)
        {
            Assert.AreEqual(obj, value);
        }

        public static void ShouldNotEqual(this object obj, object value)
        {
            Assert.AreNotEqual(obj, value);
        }

        public static void ShouldThrow<T>(this object obj, Action method, string message)
        {
            Assert.Throws(typeof(T), method.Invoke, message);
        }
    }
}
