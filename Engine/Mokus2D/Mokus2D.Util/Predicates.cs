using System;

namespace Mokus2D.Util;

public static class Predicates<T>
{
	public static readonly Predicate<T> True = (T o) => true;

	public static readonly Predicate<T> False = (T o) => false;
}
