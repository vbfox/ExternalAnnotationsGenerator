using System;
using System.Linq;
using System.Xml.XPath;
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
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var assemblyElement = doc.XPathSelectElement("/assembly");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfMethodAnnotated()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILoggerFactory>(
                type =>
                    type.Annotate(i => i.GetLogger(ParameterNotes.Some<Type>()) == ParameterNotes.NotNull<ILogger>()));

            var doc = annotator.GenerateFiles().First().Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfParameterAnnotated()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void CreatesNotNullAnnotationWhenAppliedToMethod()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILoggerFactory>(
                type =>
                    type.Annotate(i => i.GetLogger(ParameterNotes.Some<Type>()) == ParameterNotes.NotNull<ILogger>()));

            var doc = annotator.GenerateFiles().First().Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value,
                Is.EqualTo("M:JetBrains.Annotations.NotNullAttribute.#ctor"));
        }

        [Test]
        public void CreatesStringFormatMethodArgumentElement()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var argumentElement = doc.XPathSelectElement("/assembly/member/attribute/argument");

            Assert.That(argumentElement, Is.Not.Null);
            Assert.That(argumentElement.FirstNode.ToString(), Is.EqualTo("format"),
                "content should match method argument name");
        }

        [Test]
        public void CreatesStringFormatMethodAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
        }

        [Test]
        public void DoesNotThrowsExceptionIfAnnotationContainsNoAdvice()
        {
            Assert.DoesNotThrow(() =>
            {
                var annotator = new Annotator();

                annotator.AnnotateType<ILogger>(
                    type => type.Annotate(i => i.Info(ParameterNotes.Some<string>(), ParameterNotes.Some<object[]>())));
            });
        }

        [Test]
        public void SetsAssemblyElementNameAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var assemblyElement = doc.XPathSelectElement("/assembly[@name=\"Ninject.Extensions.Logging\"]");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void SetsAttributeCtorAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value,
                Is.EqualTo("M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"));
        }

        [Test]
        public void SetsMemberElementName()
        {
            var annotator = new Annotator();

            annotator.AnnotateType<ILogger>(
                type => type.Annotate(i => i.Info(ParameterNotes.FormatString(), ParameterNotes.Some<object[]>())));

            var doc = annotator.GenerateFiles().First().Content;
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value,
                Is.EqualTo("M:Ninject.Extensions.Logging.ILogger.Info(System.String,System.Object[])"));
        }
    }
}