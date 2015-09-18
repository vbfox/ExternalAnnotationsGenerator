using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ExternalAnnotationsGenerator.Core.FileGeneration;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Annotations;
using static ExternalAnnotationsGenerator.Tests.AnnotationXml.ResharperNamesBuilderTests;
using static ExternalAnnotationsGenerator.Tests.Expressions.ParseHelper;

namespace ExternalAnnotationsGenerator.Tests.AnnotationXml
{
    public class NameBuilderFromExpressionTests
    {
        [Test]
        public void CanGeneratePredefinedMarkerClass()
        {
            var expected = $"M:{TestTypeName}.MethodWithTypedArgSimple``1(``0,System.Collections.Generic.List{{``0}})";
            var parsed = Parse((TestClass c) => c.MethodWithTypedArgSimple(Some<T>(), Some<List<T>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGeneratePredefinedMarkerClassInInnerClass()
        {
            var expected = $"M:{InnerTestTypeName}.MethodWithTypedArg``1(`0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var parsed = Parse((TestClass.InnerClass<T> c) => c.MethodWithTypedArg<TStruct>(Some<T>(), Some<TStruct?>(), Some<List<TStruct>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanGeneratePredefinedMarkerStruct()
        {
            var expected = $"M:{TestTypeName}.MethodWithTypedArg``1(``0,System.Nullable{{``0}},System.Collections.Generic.List{{``0}})";
            var parsed = Parse((TestClass c) => c.MethodWithTypedArg(Some<TStruct>(), Some<TStruct?>(), Some<List<TStruct>>()));
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
            var expected = $"M:{TestTypeName}.MethodWithTypedArgSimple``1(``0,System.Collections.Generic.List{{``0}})";
            var parsed = Parse((TestClass c) => c.MethodWithTypedArgSimple(Some<Foo>(), Some<List<Foo>>()));
            var actual = ResharperNamesBuilder.GetMethodNameString((MethodInfo)parsed.Member);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}