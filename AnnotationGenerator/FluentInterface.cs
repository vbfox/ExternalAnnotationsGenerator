using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AnnotationGenerator
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FluentInterface
    {
        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetType()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType() => base.GetType();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.ToString()"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ToString() => base.ToString();

        /// <summary>
        /// Redeclaration that hides the <see cref="object.Equals(object)"/> method from IntelliSense.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("ReSharper", "BaseObjectEqualsIsObjectEquals")]
        public new bool Equals(object obj) => base.Equals(obj);
    }
}
