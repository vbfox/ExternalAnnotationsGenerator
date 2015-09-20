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

            var candidates = methods.Where(m => IsSameOverloadSimple(m, genericMethod)).ToList();
            switch (candidates.Count)
            {
                case 0:
                    throw CreateUnableToFindMatchException(methodBase);

                case 1:
                    return candidates[0];

                default:
                    var result = candidates.SingleOrDefault(m => IsSameOverloadComplex(m, genericMethod));

                    if (result == null)
                    {
                        throw CreateUnableToFindMatchException(methodBase);
                    }

                    return result;
            }
        }

        static ArgumentException CreateUnableToFindMatchException(MethodBase methodBase)
        {
            return new ArgumentException($"Unable to find a matching generic definition for method {methodBase}",
                nameof(methodBase));
        }

        static bool IsSameOverloadSimple(MethodBase a, MethodBase b)
        {
            if (a.Name != b.Name)
            {
                return false;
            }

            return a.IsGenericMethod == b.IsGenericMethod
                && a.IsAbstract == b.IsAbstract
                && a.IsAssembly == b.IsAssembly
                && a.IsFamilyAndAssembly == b.IsFamilyAndAssembly
                && a.IsFamily == b.IsFamily
                && a.IsFamilyOrAssembly == b.IsFamilyOrAssembly
                && a.IsFinal == b.IsFinal
                && a.IsHideBySig == b.IsHideBySig
                && a.IsPrivate == b.IsPrivate
                && a.IsPublic == b.IsPublic
                && a.IsVirtual == b.IsVirtual
                && ArrayEqual(a.GetGenericArguments(), b.GetGenericArguments());
        }

        static bool IsSameOverloadComplex(MethodBase a, MethodBase b)
        {
            return ArrayEqual(a.GetParameters(), b.GetParameters(), ParameterEquals);
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