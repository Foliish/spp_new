using System;

namespace TestingFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DynamicTestCaseAttribute : Attribute
    {
        public string SourceMethodName { get; }

        public DynamicTestCaseAttribute(string sourceMethodName)
        {
            SourceMethodName = sourceMethodName;
        }
    }
}
