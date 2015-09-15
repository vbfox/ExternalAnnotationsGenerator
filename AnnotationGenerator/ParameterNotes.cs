namespace AnnotationGenerator
{
    public static class ParameterNotes
    {
        public static string FormatString()
        {
            return default(string);
        }

        public static string NullableFormatString()
        {
            return default(string);
        }

        public static T Some<T>()
        {
            return default(T);
        }

        public static T NotNull<T>()
            where T : class
        {
            return default(T);
        }

        public static T CanBeNull<T>()
            where T : class
        {
            return default(T);
        }
    }
}
