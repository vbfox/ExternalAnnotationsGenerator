using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.FileGeneration;
using NUnit.Framework;

namespace ExternalAnnotationsGenerator.Tests.Core.Construction
{
    public class NameBuilderFromExpressionTests
    {
        [Test]
        public void CanGeneratePredefinedMarkerClass()
        {
            var expected = $"M:{ResharperNamesBuilderTests.TestTypeName}.MethodWithTypedArgSimple``1(``0,System.Collections.Generic.List{{``0}})";
            var parsed = ParseHelper.Parse((ResharperNamesBuilderTests.TestClass c) => c.MethodWithTypedArgSimple(Annotations.Some<TClass>(), Annotations.Some<List<TClass>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGeneratePredefinedMarkerClassInInnerClass()
        {
            var expected = $"M:{ResharperNamesBuilderTests.InnerTestTypeName}.MethodWithTypedArg``1(`0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var parsed = ParseHelper.Parse((ResharperNamesBuilderTests.TestClass.InnerClass<TClass> c) => c.MethodWithTypedArg(Annotations.Some<TClass>(), Annotations.Some<TStruct?>(), Annotations.Some<List<TStruct>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGeneratePredefinedMarkerStruct()
        {
            var expected = $"M:{ResharperNamesBuilderTests.TestTypeName}.MethodWithTypedArg``1(``0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var parsed = ParseHelper.Parse((ResharperNamesBuilderTests.TestClass c) => c.MethodWithTypedArg(Annotations.Some<TStruct>(), Annotations.Some<TStruct?>(), Annotations.Some<List<TStruct>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        class Foo
        {

        }

        [Test]
        public void CanGenerateCustomMarkerClass()
        {
            var expected = $"M:{ResharperNamesBuilderTests.TestTypeName}.MethodWithTypedArgSimple``1(``0,System.Collections.Generic.List{{``0}})";
            var parsed = ParseHelper.Parse((ResharperNamesBuilderTests.TestClass c) => c.MethodWithTypedArgSimple(Annotations.Some<Foo>(), Annotations.Some<List<Foo>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}