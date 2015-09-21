using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ExternalAnnotationsGenerator.Core.Construction;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Annotations;

namespace ExternalAnnotationsGenerator.Tests.Core.Model
{
    [TestFixture]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class AnnotatorTests
    {
        [Test]
        public void CanBeAnnotationOnParameter()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(CanBeNull<string>())));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.ParameterAnnotations.FirstOrDefault();

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.True);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void CanBeNullAnnotationOnMethodResult()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(
                type => type.Annotate(i => i.GetString(NotNull<string>()) == CanBeNull<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.Annotations.FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.True);
        }

        [Test]
        public void CanBeNullAnnotationOnProperty()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == CanBeNull<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.Annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.True);
        }

        [Test]
        public void CreatesMemberAnnotationsForNonVoidDelegate()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.GetInt()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetInt"));
        }

        [Test]
        public void CreatesMemberAnnotationsForNonVoidDelegateWithResultAnnotated()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(
                type => type.Annotate(i => i.GetString(NotNull<string>()) == NotNull<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetString"));
        }

        [Test]
        public void CreatesMemberAnnotationsForVoidMethod()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("VoidMethod"));
        }

        [Test]
        public void FormatStringAnnotationOnParameter()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(FormatString())));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.ParameterAnnotations.FirstOrDefault();

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.True);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.True);
        }

        [Test]
        public void NonNullAnnotationOnMethodResult()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(
                type => type.Annotate(i => i.GetString(NotNull<string>()) == NotNull<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.Annotations.FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.True);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void NonNullAnnotationOnProperty()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == NotNull<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.Annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.True);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void NotNullAnnotationOnParameter()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(NotNull<string>())));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.ParameterAnnotations.FirstOrDefault();

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.True);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void NullableFormatStringAnnotationOnParameter()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(NullableFormatString())));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.ParameterAnnotations.FirstOrDefault();

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.True);
            Assert.That(firstParamInfo.IsFormatString, Is.True);
        }

        [Test]
        public void SomeAnnotationOnMethodResult()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(
                type => type.Annotate(i => i.GetString(NotNull<string>()) == Some<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var resultInfo = annotations.FirstOrDefault()?.Annotations.FirstOrDefault();

            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void SomeAnnotationOnParameter()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.VoidMethod(Some<string>())));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var firstParamInfo = annotations.FirstOrDefault()?.ParameterAnnotations.FirstOrDefault();

            Assert.That(firstParamInfo, Is.Not.Null);
            Assert.That(firstParamInfo.ParameterName, Is.EqualTo("str"));
            Assert.That(firstParamInfo.IsNotNull, Is.False);
            Assert.That(firstParamInfo.CanBeNull, Is.False);
            Assert.That(firstParamInfo.IsFormatString, Is.False);
        }

        [Test]
        public void SomeAnnotationOnProperty()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateType<TestClass>(type => type.Annotate(i => i.StringProperty == Some<string>()));

            var annotations = ((AnnotationsBuilder) annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.Annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void CanAnnotateStaticVoidMethod()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateStatic(() => StaticTestClass.VoidMethod());

            var annotations = ((AnnotationsBuilder)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(memberAnnotations.Annotations.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanAnnotateStaticGetStringResult()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateStatic(() => StaticTestClass.GetString() == NotNull<string>());

            var annotations = ((AnnotationsBuilder)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.Annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetString"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        [Test]
        public void CanAnnotateStaticGetStringParameters()
        {
            var annotator = Annotator.Create();

            annotator.AnnotateStatic(() => StaticTestClass.GetString(NotNull<string>()));

            var annotations = ((AnnotationsBuilder)annotator).GetAnnotations().First();
            var memberAnnotations = annotations.FirstOrDefault();
            var resultInfo = memberAnnotations?.Annotations.FirstOrDefault();

            Assert.That(memberAnnotations, Is.Not.Null);
            Assert.That(memberAnnotations.Member.Name, Is.EqualTo("GetString"));
            Assert.That(resultInfo, Is.Not.Null);
            Assert.That(resultInfo.IsNotNull, Is.False);
            Assert.That(resultInfo.CanBeNull, Is.False);
        }

        public static class StaticTestClass
        {
            public static void VoidMethod()
            {
            }

            public static string GetString()
            {
                return default(string);
            }

            public static string GetString(string str)
            {
                return default(string);
            }
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