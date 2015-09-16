using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnnotationGenerator.Notes;
using NUnit.Framework;
using static AnnotationGenerator.ParameterNotes;

namespace AnnotationGenerator.Tests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class AnnotatorTests
    {
        [Test]
        public void CreatesAssemblyAnnotations()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm => {});
            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().FirstOrDefault();

            Assert.That(annotations, Is.Not.Null);
            Assert.That(annotations.Assembly, Is.EqualTo(typeof(TestClass).Assembly));
        }

        [Test]
        public void CreatesMemberAnnotationsForVoidMethod()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("VoidMethod"));
        }

        [Test]
        public void CreatesMemberAnnotationsForNonVoidDelegate()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.GetInt()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetInt"));
        }

        [Test]
        public void CreatesMemberAnnotationsForNonVoidDelegateWithResultAnnotated()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.GetString(NotNull<string>()) == NotNull<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetString"));
        }

        [Test]
        public void SomeAnnotationOnParameter()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(Some<string>())));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.FirstOrDefault() as ParameterAnnotationInfo;

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void NotNullAnnotationOnParameter()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(NotNull<string>())));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.FirstOrDefault() as ParameterAnnotationInfo;

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.True);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void CanBeAnnotationOnParameter()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(CanBeNull<string>())));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.FirstOrDefault() as ParameterAnnotationInfo;

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.True);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void FormatStringAnnotationOnParameter()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(FormatString())));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.FirstOrDefault() as ParameterAnnotationInfo;

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.True);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.True);
        }

        [Test]
        public void NullableFormatStringAnnotationOnParameter()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(NullableFormatString())));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.FirstOrDefault() as ParameterAnnotationInfo;

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.True);
            Assert.That(firstParamInfo.IsFormatString, Is.True);
        }

        [Test]
        public void SomeAnnotationOnMethodResult()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.GetString(NotNull<string>()) == Some<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void NonNullAnnotationOnMethodResult()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.GetString(NotNull<string>()) == NotNull<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.True);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void CanBeNullAnnotationOnMethodResult()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.GetString(NotNull<string>()) == CanBeNull<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.True);
        }

        [Test]
        public void SomeAnnotationOnProperty()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == Some<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void NonNullAnnotationOnProperty()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == NotNull<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.True);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void CanBeNullAnnotationOnProperty()
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<TestClass>(asm =>
            {
                asm.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == CanBeNull<string>()));
            });

            var annotations = ((IAnnotatorAnnotations)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.OfType<MemberAnnotationInfo>().FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.True);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "UnassignedField.Global")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class TestClass
        {
            public string StringProperty { get; set; }

            public void VoidMethod()
            {
            }

            public void VoidMethod(string str)
            {
            }

            public int GetInt()
            {
                return default(int);
            }

            public int GetInt(string str)
            {
                return default(int);
            }

            public string GetString()
            {
                return default(string);
            }

            public string GetString(string str)
            {
                return default(string);
            }
        }
    }
}
