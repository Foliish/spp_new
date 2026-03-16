using System;

namespace TestingFramework.Exceptions
{
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message)
        {
        }
    }
}