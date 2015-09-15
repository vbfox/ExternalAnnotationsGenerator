using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AnnotationGenerator.Notes;
using NUnit.Framework;

namespace AnnotationGenerator.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public class ResharperXmlBuilderTests
    {
        public class TestClass
        {
            public TestClass()
            {
            }

            public TestClass(string str)
            {

            }

            public void VoidMethod()
            {
            }

            public string StringMethod()
            {
                return null;
            }

            public void MethodWithStandardArgs(string str, int integer)
            {
            }

            public void MethodWithNullableArg(int? integer)
            {
            }

            public void MethodWithListArg(List<int> lst)
            {
            }

            public void MethodWithArrayArg(int[] arr)
            {
            }

            public void MethodWithTypedArg<T>(T x, T? y, List<T> lst)
                where T : struct
            {

            }
        }

        private static readonly Type testType = typeof(TestClass);
        private static readonly string testTypeName = testType.FullName;

        [Test]
        public void CanGetEmptyCtorName()
        {
            var expected = $"M:{testTypeName}.#ctor";
            var actual = ResharperXmlBuilder.GetMethodNameString(testType.GetConstructor(Type.EmptyTypes));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetCtorWithParameterName()
        {
            var expected = $"M:{testTypeName}.#ctor(System.String)";
            var actual = ResharperXmlBuilder.GetMethodNameString(testType.GetConstructor(new[] { typeof(string) }));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGetMethodWithNullableArgName()
        {
            var expected = $"M:{testTypeName}.MethodWithNullableArg(System.Nullable{{int}})";
            var actual = ResharperXmlBuilder.GetMethodNameString(testType.GetMethod("MethodWithNullableArg"));
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}