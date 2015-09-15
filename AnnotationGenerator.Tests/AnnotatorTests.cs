using System;
using System.Linq;
using System.Xml.XPath;
using Ninject.Extensions.Logging;
using NUnit.Framework;
using static AnnotationGenerator.ParameterNotes;

namespace AnnotationGenerator.Tests
{
    [TestFixture]
    public class AnnotatorTests
    {
        [Test]
        public void CreatesAssemblyElement()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm => {});

            var doc = annotator.GetDocuments().First();
            var assemblyElement = doc.XPathSelectElement("/assembly");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void SetsAssemblyElementNameAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm => {});

            var doc = annotator.GetDocuments().First();
            var assemblyElement = doc.XPathSelectElement("/assembly[@name=\"Ninject.Extensions.Logging\"]");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfParameterAnnotated()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }        
        
        [Test]
        public void SetsMemberElementName()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value, Is.EqualTo("M:Ninject.Extensions.Logging.ILogger.Info(System.String,System.Object[])"));
        }        
        
        [Test]
        public void CreatesMemberElementIfMethodAnnotated()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILoggerFactory>(type => type.Annotate(i => i.GetLogger(Some<Type>()) == NotNull<ILogger>()));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void DoesNotThrowsExceptionIfAnnotationContainsNoAdvice()
        {
            Assert.DoesNotThrow(() =>
            {
                var annotator = new Annotator();

                annotator.AnnotateAssemblyContaining<ILogger>(asm =>
                {
                    asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(Some<string>(), Some<object[]>())));
                });
            });
        }

        [Test]
        public void CreatesStringFormatMethodAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
        }

        [Test]
        public void SetsAttributeCtorAttribute()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value, Is.EqualTo("M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"));
        }
        
        [Test]
        public void CreatesStringFormatMethodArgumentElement()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString(), Some<object[]>())));
            });

            var doc = annotator.GetDocuments().First();
            var argumentElement = doc.XPathSelectElement("/assembly/member/attribute/argument");

            Assert.That(argumentElement, Is.Not.Null);
            Assert.That(argumentElement.FirstNode.ToString(), Is.EqualTo("format"), "content should match method argument name");
        }

        [Test]
        public void CreatesNotNullAnnotationWhenAppliedToMethod()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILoggerFactory>(type => type.Annotate(i => i.GetLogger(Some<Type>()) == NotNull<ILogger>()));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value, Is.EqualTo("M:JetBrains.Annotations.NotNullAttribute.#ctor"));
        }

    }
}
