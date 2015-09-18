using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Core.FileGeneration
{
    public static class GenericDefinitionHelper
    {
        static readonly BindingFlags bindingFlagsAllMembers =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

        public static Type GetGenericDefinition([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.IsNested)
            {
                Debug.Assert(type.DeclaringType != null, "A nested type without declaring type should not exist");
                var declaringType = GetGenericDefinition(type.DeclaringType);
                return declaringType.GetNestedType(type.Name, BindingFlags.NonPublic | BindingFlags.Public);
            }

            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static MethodBase GetGenericDefinitionCore(MethodBase methodBase)
        {
            var methodInfo = methodBase as MethodInfo;
            return methodInfo?.IsGenericMethod == true ? methodInfo.GetGenericMethodDefinition() : methodBase;
        }

        public static MethodBase GetGenericDefinition(MethodBase methodBase)
        {
            var genericMethod = GetGenericDefinitionCore(methodBase);
            if (genericMethod.DeclaringType == null)
            {
                // Might be possible for C# Script
                return genericMethod;
            }

            var type = GetGenericDefinition(genericMethod.DeclaringType);
            if (type == genericMethod.DeclaringType)
            {
                // There was nothing generic in the types, it's a simple case
                return genericMethod;
            }

            // We have the generic definition, now new need to find our method on it
            // ReSharper disable once RedundantEnumerableCastCall
            var methods = genericMethod.IsConstructor
                ? type.GetConstructors(bindingFlagsAllMembers).Cast<MethodBase>()
                : type.GetMethods(bindingFlagsAllMembers);

            return methods.Single(m => IsSameOverload(m, genericMethod));
        }

        static bool IsSameOverload(MethodBase a, MethodBase b)
        {
            if (a.Name != b.Name)
            {
                return false;
            }

            return a.IsGenericMethod == b.IsGenericMethod
                   && ArrayEqual(a.GetParameters(), b.GetParameters(), ParameterEquals)
                   && ArrayEqual(a.GetGenericArguments(), b.GetGenericArguments());
        }

        static bool ParameterEquals(ParameterInfo a, ParameterInfo b)
        {
            return a.Name == b.Name
                   && a.HasDefaultValue == b.HasDefaultValue
                   && a.IsIn == b.IsIn
                   && a.IsLcid == b.IsLcid
                   && a.IsOptional == b.IsOptional
                   && a.IsOut == b.IsOut
                   && a.IsRetval == b.IsRetval
                   && GetGenericDefinition(a.ParameterType) == GetGenericDefinition(b.ParameterType);
       }

        static bool ArrayEqual<TElement>(TElement[] a, TElement[] b, Func<TElement, TElement, bool> equals = null)
        {
            if (equals == null)
            {
                equals = (x, y) => x.Equals(y);
            }
            return a.Length == b.Length
                && a.Zip(b, (x, y) => equals(x, y)).All(_ => _);
        }
    }
}