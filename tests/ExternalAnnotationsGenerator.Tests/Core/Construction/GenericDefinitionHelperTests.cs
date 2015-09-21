using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Core.FileGeneration.GenericDefinitionHelper;

namespace ExternalAnnotationsGenerator.Tests.Core.Construction
{
    [TestFixture]
    public class GenericDefinitionHelperTests
    {
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedTypeParameter")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        class Nested
        {
            public class DoubleNestedTyped<T1>
            {
                public bool NormalMethod(T1 t1)
                {
                    return default(bool);
                }

                public T2 GenericMethod<T2>(T1 t1, T2 t2, string otherArg)
                {
                    return t2;
                }
            }

            public bool NormalMethod()
            {
                return default(bool);
            }

            public T1 GenericMethod<T1>(T1 t1, string otherArg)
            {
                return t1;
            }
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedTypeParameter")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        class NestedTyped<T1>
        {
            public class DoubleNestedTyped<T2>
            {
                public bool NormalMethod(T1 t1, T2 t2)
                {
                    return default(bool);
                }

                public T3 GenericMethod<T3>(T1 t1, T2 t2, T3 t3, string otherArg)
                {
                    return t3;
                }
            }

            public class DoubleNested
            {
                public bool NormalMethod()
                {
                    return default(bool);
                }

                public T2 GenericMethod<T2>(T1 t1, T2 t2, string otherArg)
                {
                    return t2;
                }
            }

            public bool NormalMethod(T1 t1)
            {
                return default(bool);
            }

            public T2 GenericMethod<T2>(T1 t1, T2 t2, string otherArg)
            {
                return t2;
            }
        }

        [Test]
        public void GetGenericDefinitionForMarkerType()
        {
            Assert.That(
                GetGenericDefinition(typeof(TClass)),
                Is.EqualTo(typeof(TClass)));
        }

        [Test]
        public void GetGenericDefinitionForNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof (Nested)),
                Is.EqualTo(typeof (Nested)));
        }

        [Test]
        public void GetGenericDefinitionForNormalMethodInNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(Nested).GetMethod("NormalMethod")),
                Is.EqualTo(typeof(Nested).GetMethod("NormalMethod")));
        }

        [Test]
        public void GetGenericDefinitionForGenericMethodInNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(Nested).GetMethod("GenericMethod").MakeGenericMethod(typeof(int))),
                Is.EqualTo(typeof(Nested).GetMethod("GenericMethod")));
        }

        [Test]
        public void GetGenericDefinitionForNormalMethodInGenericNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(NestedTyped<bool>).GetMethod("NormalMethod")),
                Is.EqualTo(typeof(NestedTyped<>).GetMethod("NormalMethod")));
        }

        [Test]
        public void GetGenericDefinitionForGenericMethodInGenericNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(NestedTyped<bool>).GetMethod("GenericMethod").MakeGenericMethod(typeof(int))),
                Is.EqualTo(typeof(NestedTyped<>).GetMethod("GenericMethod")));
        }

        [Test]
        public void GetGenericDefinitionForNormalMethodInGenericNestedTypeInsideGenericNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(NestedTyped<bool>.DoubleNestedTyped<int>).GetMethod("NormalMethod")),
                Is.EqualTo(typeof(NestedTyped<>.DoubleNestedTyped<>).GetMethod("NormalMethod")));
        }

        [Test]
        public void GetGenericDefinitionForGenericMethodInGenericNestedTypeInsideGenericNestedType()
        {
            Assert.That(
                GetGenericDefinition(typeof(NestedTyped<bool>.DoubleNestedTyped<int>).GetMethod("GenericMethod").MakeGenericMethod(typeof(int))),
                Is.EqualTo(typeof(NestedTyped<>.DoubleNestedTyped<>).GetMethod("GenericMethod")));
        }

        [Test]
        public void GetGenericDefinitionForNestedGenericType()
        {
            Assert.That(
                GetGenericDefinition(typeof(Nested.DoubleNestedTyped<int>)),
                Is.EqualTo(typeof(Nested.DoubleNestedTyped<>)));
        }

        [Test]
        public void GetGenericDefinitionForNestedGenericTypeInGenericType()
        {
            Assert.That(
                GetGenericDefinition(typeof(NestedTyped<bool>.DoubleNestedTyped<int>)),
                Is.EqualTo(typeof(NestedTyped<>.DoubleNestedTyped<>)));
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        [SuppressMessage("ReSharper", "UnusedTypeParameter")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        class ComplexCase<T1>
        {
            public class Inner<T2>
            {
                public bool Complex(T1 a, T2 b)
                {
                    return default(bool);
                }

                public bool Complex(T2 a, T1 b)
                {
                    return default(bool);
                }

                public bool ComplexList(List<T1> a, List<T2> b)
                {
                    return default(bool);
                }

                public bool ComplexList(List<T2> a, List<T1> b)
                {
                    return default(bool);
                }
            }
        }

        [Test]
        public void GetGenericDefinitionForComplexCase()
        {
            var first = typeof (ComplexCase<bool>.Inner<int>).GetMethod("Complex", new[] { typeof (bool), typeof (int) });
            var second = typeof(ComplexCase<bool>.Inner<int>).GetMethod("Complex", new[] { typeof(int), typeof(bool) });
            var expectedFirst =
                typeof (ComplexCase<>.Inner<>)
                    .GetMethods()
                    .Single(m => m.Name == "Complex" && m.GetParameters().First().ParameterType.Name == "T1");
            var expectedSecond =
                typeof(ComplexCase<>.Inner<>)
                    .GetMethods()
                    .Single(m => m.Name == "Complex" && m.GetParameters().First().ParameterType.Name == "T2");
            
            Assert.That(
                GetGenericDefinition(first),
                Is.EqualTo(expectedFirst));

            Assert.That(
                GetGenericDefinition(second),
                Is.EqualTo(expectedSecond));
        }

        [Test]
        public void GetGenericDefinitionForComplexListCase()
        {
            var first = typeof(ComplexCase<bool>.Inner<int>).GetMethod("ComplexList", new[] { typeof(List<bool>), typeof(List<int>) });
            var second = typeof(ComplexCase<bool>.Inner<int>).GetMethod("ComplexList", new[] { typeof(List<int>), typeof(List<bool>) });
            var expectedFirst =
                typeof(ComplexCase<>.Inner<>)
                    .GetMethods()
                    .Single(m => m.Name == "ComplexList" && m.GetParameters().First().ParameterType.GetGenericArguments().First().Name == "T1");
            var expectedSecond =
                typeof(ComplexCase<>.Inner<>)
                    .GetMethods()
                    .Single(m => m.Name == "ComplexList" && m.GetParameters().First().ParameterType.GetGenericArguments().First().Name == "T2");

            Assert.That(
                GetGenericDefinition(first),
                Is.EqualTo(expectedFirst));

            Assert.That(
                GetGenericDefinition(second),
                Is.EqualTo(expectedSecond));
        }
    }
}
