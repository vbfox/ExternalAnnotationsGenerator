using System;
using System.Collections.Generic;
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
            var genericTypes = new Dictionary<string, Type>();
            return GetGenericTypeDefinition(type, genericTypes);
        }

        public static MethodBase GetGenericDefinition(MethodBase methodBase)
        {
            return GetGenericMethodDefinition(methodBase);
        }

        private static Type GetGenericTypeDefinition([NotNull] Type type, Dictionary<string, Type> genericArguments)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var simpleGenericType = GetNaiveGenericTypeDefinition(type);

            if (type.IsGenericType)
            {
                var argTuples = simpleGenericType.GetGenericArguments().Zip(type.GetGenericArguments(), Tuple.Create);
                foreach (var argTuple in argTuples)
                {
                    if (argTuple.Item1.IsGenericParameter && !argTuple.Item2.IsGenericParameter)
                    {
                        // We want a way to know the value of each generic type
                        genericArguments.Add(argTuple.Item1.Name, argTuple.Item2);
                    }
                }
            }

            if (type.IsNested)
            {
                Debug.Assert(type.DeclaringType != null, "A nested type without declaring type should not exist");
                var declaringType = GetGenericTypeDefinition(type.DeclaringType, genericArguments);
                return declaringType.GetNestedType(type.Name, BindingFlags.NonPublic | BindingFlags.Public);
            }
            
            return simpleGenericType;
        }

        static Type GetNaiveGenericTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }

        private static MethodBase GetNaiveGenericMethodDefinition(MethodBase methodBase)
        {
            var methodInfo = methodBase as MethodInfo;
            return methodInfo?.IsGenericMethod == true ? methodInfo.GetGenericMethodDefinition() : methodBase;
        }

        static MethodBase GetGenericMethodDefinition(MethodBase methodBase)
        {
            var genericMethod = GetNaiveGenericMethodDefinition(methodBase);
            if (genericMethod.DeclaringType == null)
            {
                // Might be possible for C# Script
                return genericMethod;
            }

            var genericTypes = new Dictionary<string, Type>();
            var type = GetGenericTypeDefinition(genericMethod.DeclaringType, genericTypes);
            if (type == genericMethod.DeclaringType)
            {
                // There was nothing generic in the types, it's a simple case
                Debug.Assert(genericTypes.Count == 0);
                return genericMethod;
            }

            // We have the generic type definition, now we need to find our method on it
            return FindGenericDefinition(methodBase, genericMethod, type, genericTypes);
        }

        static MethodBase FindGenericDefinition(MethodBase methodBase, MethodBase genericMethod, Type type,
            Dictionary<string, Type> genericTypes)
        {
            // ReSharper disable once RedundantEnumerableCastCall
            var methods = genericMethod.IsConstructor
                ? type.GetConstructors(bindingFlagsAllMembers).Cast<MethodBase>()
                : type.GetMethods(bindingFlagsAllMembers);

            var candidates = methods.Where(m => IsSameOverloadSimple(m, genericMethod, genericTypes)).ToList();
            switch (candidates.Count)
            {
                case 1:
                    return candidates[0];

                default:
                    throw CreateUnableToFindMatchException(methodBase);
            }
        }

        static ArgumentException CreateUnableToFindMatchException(MethodBase methodBase)
        {
            return new ArgumentException($"Unable to find a matching generic definition for method {methodBase}",
                nameof(methodBase));
        }

        static bool IsSameOverloadSimple(MethodBase candidate, MethodBase target, Dictionary<string, Type> genericTypes)
        {
            return candidate.Name == target.Name
                && ArrayEqual(candidate.GetParameters(), target.GetParameters(), (x, y) => SimpleParameterEquals(x, y, genericTypes))
                && ArrayEqual(candidate.GetGenericArguments(), target.GetGenericArguments())
                && candidate.IsGenericMethod == target.IsGenericMethod
                && candidate.IsAbstract == target.IsAbstract
                && candidate.IsAssembly == target.IsAssembly
                && candidate.IsFamilyAndAssembly == target.IsFamilyAndAssembly
                && candidate.IsFamily == target.IsFamily
                && candidate.IsFamilyOrAssembly == target.IsFamilyOrAssembly
                && candidate.IsFinal == target.IsFinal
                && candidate.IsHideBySig == target.IsHideBySig
                && candidate.IsPrivate == target.IsPrivate
                && candidate.IsPublic == target.IsPublic
                && candidate.IsVirtual == target.IsVirtual;
        }

        static bool SimpleParameterEquals(ParameterInfo candidate, ParameterInfo target, Dictionary<string, Type> genericTypes)
        {
            return candidate.Name == target.Name
                   && candidate.HasDefaultValue == target.HasDefaultValue
                   && candidate.IsIn == target.IsIn
                   && candidate.IsLcid == target.IsLcid
                   && candidate.IsOptional == target.IsOptional
                   && candidate.IsOut == target.IsOut
                   && candidate.IsRetval == target.IsRetval
                   && ParameterTypesEquals(candidate.ParameterType, target.ParameterType, genericTypes);
        }

        static bool ParameterTypesEquals(Type candidate, Type target, Dictionary<string, Type> genericTypes)
        {
            // T1 == bool
            if (candidate.IsGenericParameter && !target.IsGenericParameter)
            {
                Type theoricalType;
                if (!genericTypes.TryGetValue(candidate.Name, out theoricalType))
                {
                    Debug.Assert(false, "We should know the real value of all parameters that were specified");
                    // ReSharper disable once HeuristicUnreachableCode
                    return false;
                }

                return ParameterTypesEquals(theoricalType, target, genericTypes);
            }

            // T1 == T1
            if (candidate.IsGenericParameter && target.IsGenericParameter)
            {
                return candidate == target;
            }

            if (candidate.IsNested != target.IsNested)
            {
                return false;
            }

            if (candidate.IsNested && !ParameterTypesEquals(candidate.DeclaringType, target.DeclaringType, genericTypes))
            {
                return false;
            }

            if (candidate.IsGenericType != target.IsGenericType)
            {
                return false;
            }

            if (candidate.IsGenericType)
            {
                var argTuples = candidate.GetGenericArguments().Zip(target.GetGenericArguments(), Tuple.Create);
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var argTuple in argTuples)
                {
                    if (!ParameterTypesEquals(argTuple.Item1, argTuple.Item2, genericTypes))
                    {
                        return false;
                    }
                }
            }

            return GetGenericDefinition(candidate) == GetGenericDefinition(target);
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