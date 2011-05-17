using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
public static class AssertionExtensions
{
    public static void ShouldBeNull(this object obj)
    {
        Assert.IsNull(obj);
    }

    public static void ShouldNotbeNull(this object obj)
    {
        Assert.IsNotNull(obj);
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

    public static void ShouldThrow<T>(this object obj, Action method)
    {
        Assert.Throws(typeof(T), method.Invoke);
    }

	public static void Should(this object obj, IResolveConstraint constraint)
	{
		Assert.That(obj, constraint);
	}

	public static bool None<T>(this IEnumerable<T> self, Func<T, bool> predicate)
	{
		return !self.Any(predicate);
	}
}