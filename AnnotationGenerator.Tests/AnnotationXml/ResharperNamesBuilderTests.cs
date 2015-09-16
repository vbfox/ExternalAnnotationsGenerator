using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AnnotationGenerator.Core.FileGeneration;
using JetBrains.Annotations;
using NUnit.Framework;

namespace AnnotationGenerator.Tests.AnnotationXml
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class ResharperNamesBuilderTests
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "UnassignedField.Global")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public class TestClass
        {
            public TestClass()
            {
                Console.WriteLine("hi");
            }

            public TestClass([NotNull] string str)
            {
                Console.WriteLine("hi");
            }

            public void VoidMethod()
            {
                Console.WriteLine("hi");
            }

            public string StringMethod()
            {
                Console.WriteLine("hi");
                return null;
            }

            public void MethodWithStandardArgs([NotNull] string str, int integer)
            {
                Console.WriteLine("hi");
            }

            public void MethodWithNullableArg([NotNull] int? integer)
            {
                Console.WriteLine("hi");
            }

            public void MethodWithListArg([NotNull] List<int> lst)
            {
                Console.WriteLine("hi");
            }

            public void MethodWithArrayArg([NotNull] int[] arr)
            {
            }

            [NotNull]
            public double? MethodWithTypedArg<T>(T x, [NotNull] T? y, [NotNull] List<T> lst)
                where T : struct
            {
                return 0;
            }

            public class InnerClass<TInner>
            {
                [NotNull]
                public double? MethodWithTypedArg<T>([NotNull] TInner x, [NotNull] T? y, [NotNull] List<T> lst)
                                where T : struct
                {
                    return 0;
                }
            }
        }

        private static readonly Type testType = typeof(TestClass);
        private static readonly string testTypeName = testType.FullName;
        private static readonly Type innerTestType = typeof(TestClass.InnerClass<>);
        private static readonly string innerTestTypeName = innerTestType.FullName;

        [Test]
        public void CanGetEmptyCtorName()
        {
            var expected = $"M:{testTypeName}.#ctor";
            var actual = ResharperNamesBuilder.GetMethodNameString(testType.GetConstructor(Type.EmptyTypes));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetCtorWithParameterName()
        {
            var expected = $"M:{testTypeName}.#ctor(System.String)";
            var actual = ResharperNamesBuilder.GetMethodNameString(testType.GetConstructor(new[] { typeof(string) }));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetMethodWithNullableArgName()
        {
            var expected = $"M:{testTypeName}.MethodWithNullableArg(System.Nullable{{System.Int32}})";
            var actual = ResharperNamesBuilder.GetMethodNameString(testType.GetMethod("MethodWithNullableArg"));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetMethodWithTypedArgName()
        {
            var expected = $"M:{testTypeName}.MethodWithTypedArg``1(``0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var actual = ResharperNamesBuilder.GetMethodNameString(testType.GetMethod("MethodWithTypedArg"));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetMethodWithTypedArgNameInInnerClass()
        {
            var expected = $"M:{innerTestTypeName}.MethodWithTypedArg``1(`0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var actual = ResharperNamesBuilder.GetMethodNameString(innerTestType.GetMethod("MethodWithTypedArg"));
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}