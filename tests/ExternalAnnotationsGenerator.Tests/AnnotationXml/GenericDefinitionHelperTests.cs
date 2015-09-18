using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Core.FileGeneration.GenericDefinitionHelper;

namespace ExternalAnnotationsGenerator.Tests.AnnotationXml
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
                GetGenericDefinition(typeof(T)),
                Is.EqualTo(typeof(T)));
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

    }
}
