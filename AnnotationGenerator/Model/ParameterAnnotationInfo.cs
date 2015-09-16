using System;
using JetBrains.Annotations;

namespace AnnotationGenerator.Model
{
    internal class ParameterAnnotationInfo
    {
        public string ParameterName { get; }
        public bool IsFormatString { get; }
        public bool IsNotNull { get; }
        public bool CanBeNull { get; }

        public ParameterAnnotationInfo([NotNull] string parameterName, bool isFormatString = false, bool isNotNull = false, bool canBeNull = false)
        {
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));

            ParameterName = parameterName;
            IsFormatString = isFormatString;
            IsNotNull = isNotNull;
            CanBeNull = canBeNull;
        }
    }
}
