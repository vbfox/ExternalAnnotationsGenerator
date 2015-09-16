using System;
using System.Linq;
using System.Xml.XPath;
using AnnotationGenerator.Core;
using AnnotationGenerator.Core.FileGeneration;
using Ninject.Extensions.Logging;
using NUnit.Framework;

namespace AnnotationGenerator.Tests
{
    [TestFixture]
    public class FullWorkflowTests
    {
        [Test]
        public void CreatesAssemblyElement()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var assemblyElement = doc.XPathSelectElement("/assembly");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfMethodAnnotated()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<ILoggerFactory>(
                type =>
                    type.Annotate(i => i.GetLogger(Annotations.Some<Type>()) == Annotations.NotNull<ILogger>()));

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

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void CreatesNotNullAnnotationWhenAppliedToMethod()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<ILoggerFactory>(
                type =>
                    type.Annotate(i => i.GetLogger(Annotations.Some<Type>()) == Annotations.NotNull<ILogger>()));

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

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

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

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

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

                annotator.AnnotateType<ILogger>(
                    type => type.Annotate(i => i.Info(Annotations.Some<string>(), Annotations.Some<object[]>())));
            });
        }

        [Test]
        public void SetsAssemblyElementNameAttribute()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var assemblyElement = doc.XPathSelectElement("/assembly[@name=\"Ninject.Extensions.Logging\"]");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void SetsAttributeCtorAttribute()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

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

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(Annotations.FormatString(), Annotations.Some<object[]>())));

            var doc = GetFirstFile(annotator).Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value,
                Is.EqualTo("M:Ninject.Extensions.Logging.ILogger.Info(System.String,System.Object[])"));
        }
    }
}