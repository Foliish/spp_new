using System;

namespace TestingFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PriorityAttribute : Attribute
    {
        public int Level { get; }

        public PriorityAttribute(int level)
        {
            Level = level;
        }
    }
}
