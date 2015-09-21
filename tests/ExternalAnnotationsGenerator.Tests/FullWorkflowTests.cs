using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.XPath;
using ExternalAnnotationsGenerator.Core;
using ExternalAnnotationsGenerator.Core.FileGeneration;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Annotations;

namespace ExternalAnnotationsGenerator.Tests
{
    [TestFixture]
    public class FullWorkflowTests
    {
        [Test]
        public void CreatesAssemblyElement()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var assemblyElement = doc.XPathSelectElement("/assembly");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfMethodAnnotated()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type =>
                    type.Annotate(i => i.GetLogger(Some<Type>()) == NotNull<string>()));

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        private static AnnotationFile GetFirstFile(IAnnotator annotator)
        {
            return CoreHelper.GetAnnotations(annotator).GenerateFiles().First();
        }

        [Test]
        public void CreatesMemberElementIfParameterAnnotated()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void CreatesNotNullAnnotationWhenAppliedToMethod()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type =>
                    type.Annotate(i => i.GetLogger(Some<Type>()) == NotNull<string>()));

            var doc = GetFirstFile(annotator).Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value,
                Is.EqualTo("M:JetBrains.Annotations.NotNullAttribute.#ctor"));
        }

        [Test]
        public void CreatesStringFormatMethodArgumentElement()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var argumentElement = doc.XPathSelectElement("/assembly/member/attribute/argument");

            Assert.That(argumentElement, Is.Not.Null);
            Assert.That(argumentElement.FirstNode.ToString(), Is.EqualTo("format"),
                "content should match method argument name");
        }

        [Test]
        public void CreatesStringFormatMethodAttribute()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>()))
                );

            var doc = GetFirstFile(annotator).Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
        }

        [Test]
        public void DoesNotThrowsExceptionIfAnnotationContainsNoAdvice()
        {
            Assert.DoesNotThrow(() =>
            {
                var annotator = Annotator.Create();

                annotator.Annotate<TestClass>(
                    type => type.Annotate(i => i.Info(Some<string>(), Some<object[]>())));
            });
        }

        [Test]
        public void SetsAssemblyElementNameAttribute()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>()))
                );

            var doc = GetFirstFile(annotator).Content;
            var assemblyElement = doc.XPathSelectElement("/assembly[@name=\"ExternalAnnotationsGenerator.Tests\"]");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void SetsAttributeCtorAttribute()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>()))
                );

            var doc = GetFirstFile(annotator).Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value,
                Is.EqualTo("M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"));
        }

        [Test]
        public void SetsMemberElementName()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => i.Info(FormatString(), Some<object[]>()))
                );

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value,
                Is.EqualTo($"M:{testClassFullname}.Info(System.String,System.Object[])"));
        }

        [Test]
        public void CanAnnotateStaticMethods()
        {
            var annotator = Annotator.Create();

            annotator.Annotate<TestClass>(
                type => type.Annotate(i => TestClass.GetLoggerStatic(NotNull<Type>()))
                );

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value,
                Is.EqualTo($"M:{testClassFullname}.GetLoggerStatic(System.Type)"));
        }

        private static readonly string testClassFullname = typeof(TestClass).FullName;

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "UnassignedField.Global")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class TestClass
        {
            public string GetLogger(Type type)
            {
                return default(string);
            }

            public static string GetLoggerStatic(Type type)
            {
                return default(string);
            }

            public void Info(string format, params object[] args)
            {

            }

            public void Info(string format, object arg)
            {

            }

            public void Info<T>(string format, T arg)
            {

            }
        }
    }
}