using System;
using System.Collections;
using TestingFramework.Exceptions;

namespace TestingFramework.Assertions
{
    public static class Assert
    {
        public static void AreEqual(object expected, object actual)
        {
            if (!Equals(expected, actual))
                throw new AssertException($"Expected {expected}, but got {actual}");
        }

        public static void AreNotEqual(object notExpected, object actual)
        {
            if (Equals(notExpected, actual))
                throw new AssertException($"Value should not be {notExpected}");
        }

        public static void IsTrue(bool condition)
        {
            if (!condition)
                throw new AssertException("Condition is false but expected true");
        }

        public static void IsFalse(bool condition)
        {
            if (condition)
                throw new AssertException("Condition is true but expected false");
        }

        public static void IsNull(object value)
        {
            if (value != null)
                throw new AssertException("Value is not null");
        }

        public static void IsNotNull(object value)
        {
            if (value == null)
                throw new AssertException("Value is null");
        }

        public static void Greater(int a, int b)
        {
            if (a <= b)
                throw new AssertException($"{a} is not greater than {b}");
        }

        public static void Less(int a, int b)
        {
            if (a >= b)
                throw new AssertException($"{a} is not less than {b}");
        }

        public static void Contains(IEnumerable collection, object value)
        {
            foreach (var item in collection)
            {
                if (Equals(item, value))
                    return;
            }

            throw new AssertException($"Collection does not contain {value}");
        }
    }
}