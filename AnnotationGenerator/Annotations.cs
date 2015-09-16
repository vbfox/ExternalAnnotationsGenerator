using JetBrains.Annotations;

namespace AnnotationGenerator
{
    /// <summary>
    /// Annotation marker methods for the Fluent syntax
    /// </summary>
    public static class Annotations
    {
        /// <summary>
        /// Mark a parameter with <see cref="NotNullAttribute"/> and it's parent method with
        /// <see cref="StringFormatMethodAttribute"/>.
        /// </summary>
        public static string FormatString()
        {
            return default(string);
        }

        /// <summary>
        /// Mark a parameter with <see cref="CanBeNullAttribute"/> and it's parent method with
        /// <see cref="StringFormatMethodAttribute"/>.
        /// </summary>
        public static string NullableFormatString()
        {
            return default(string);
        }

        /// <summary>
        /// Placeholder for a parameter, field, property or method result with out annotation.
        /// </summary>
        public static T Some<T>()
        {
            return default(T);
        }

        /// <summary>
        /// Mark a parameter, field, property or method result with <see cref="NotNullAttribute"/>.
        /// </summary>
        public static T NotNull<T>()
            where T : class
        {
            return default(T);
        }

        /// <summary>
        /// Mark a parameter, field, property or method result with <see cref="CanBeNullAttribute"/>.
        /// </summary>
        public static T CanBeNull<T>()
            where T : class
        {
            return default(T);
        }
    }
}
