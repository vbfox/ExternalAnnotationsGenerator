using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExternalAnnotationsGenerator.Core.FileGeneration;
using ExternalAnnotationsGenerator.Core.Model;
using NUnit.Framework;
using static ExternalAnnotationsGenerator.Tests.XmlTestsHelpers;

namespace ExternalAnnotationsGenerator.Tests.Core.FileGeneration
{
    [TestFixture]
    public class AnnotationFileGeneratorTests
    {
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private class MemberDiversity
        {
            public string SomeProperty { get; }
#pragma warning disable 414
            public string SomeField;
#pragma warning restore 414
            public string SomeMethod(string str) => str;

            public MemberDiversity(string str)
            {
                SomeField = str;
                SomeProperty = str;
            }
        }

        [Test]
        public void CanAnnotateProperties()
        {
            MemberAnnotations memberAnnotations;
            var assemblyAnnotations = Init<MemberDiversity>(out memberAnnotations, t => t.GetMember("SomeProperty").Single());
            memberAnnotations.Annotations.Add(new MemberAnnotationInfo(true));

            // Act
            var file = new AnnotationFileGenerator(assemblyAnnotations).Generate();
            var actual = file.Content.Descendants("member").Single();

            // Assert
            var expected = @"
<member name=""P:ExternalAnnotationsGenerator.Tests.Core.FileGeneration.AnnotationFileGeneratorTests+MemberDiversity.SomeProperty"">
    <attribute ctor=""M:JetBrains.Annotations.NotNullAttribute.#ctor"" />
</member>";
            Assert.That(Normalize(actual), Is.EqualTo(Normalize(expected)));
        }

        private static AssemblyAnnotations Init<T>(out MemberAnnotations memberAnnotations, Func<Type, MemberInfo> getMember)
        {
            var t = typeof (T);
            var assemblyAnnotations = new AssemblyAnnotations(t.Assembly);
            memberAnnotations = new MemberAnnotations(getMember(t));
            assemblyAnnotations.Add(memberAnnotations);
            return assemblyAnnotations;
        }

        [Test]
        public void CanAnnotateMethod()
        {
            // Arrange
            MemberAnnotations memberAnnotations;
            var assemblyAnnotations = Init<MemberDiversity>(out memberAnnotations, t => t.GetMember("SomeMethod").Single());
            memberAnnotations.Annotations.Add(new MemberAnnotationInfo(true));
            memberAnnotations.ParameterAnnotations.Add(new ParameterAnnotationInfo("str", isNotNull:true));

            // Act
            var file = new AnnotationFileGenerator(assemblyAnnotations).Generate();
            var actual = file.Content.Descendants("member").Single();

            // Assert
            var expected = @"
<member name=""M:ExternalAnnotationsGenerator.Tests.Core.FileGeneration.AnnotationFileGeneratorTests+MemberDiversity.SomeMethod(System.String)"">
    <attribute ctor=""M:JetBrains.Annotations.NotNullAttribute.#ctor"" />
    <parameter name=""str"">
        <attribute ctor=""M:JetBrains.Annotations.NotNullAttribute.#ctor"" />
    </parameter>
</member>";
            Assert.That(Normalize(actual), Is.EqualTo(Normalize(expected)));
        }

        [Test]
        public void CanAnnotateField()
        {
            MemberAnnotations memberAnnotations;
            var assemblyAnnotations = Init<MemberDiversity>(out memberAnnotations, t => t.GetMember("SomeField").Single());
            memberAnnotations.Annotations.Add(new MemberAnnotationInfo(true));

            // Act
            var file = new AnnotationFileGenerator(assemblyAnnotations).Generate();
            var actual = file.Content.Descendants("member").Single();

            // Assert
            var expected = @"
<member name=""F:ExternalAnnotationsGenerator.Tests.Core.FileGeneration.AnnotationFileGeneratorTests+MemberDiversity.SomeField"">
    <attribute ctor=""M:JetBrains.Annotations.NotNullAttribute.#ctor"" />
</member>";
            Assert.That(Normalize(actual), Is.EqualTo(Normalize(expected)));
        }

        [Test]
        public void CanAnnotateCtor()
        {
            // Arrange
            MemberAnnotations memberAnnotations;
            var assemblyAnnotations = Init<MemberDiversity>(out memberAnnotations, t => t.GetConstructors().Single());
            memberAnnotations.ParameterAnnotations.Add(new ParameterAnnotationInfo("str", isNotNull:true));

            // Act
            var file = new AnnotationFileGenerator(assemblyAnnotations).Generate();
            var actual = file.Content.Descendants("member").Single();

            // Assert
            var expected = @"
<member name=""M:ExternalAnnotationsGenerator.Tests.Core.FileGeneration.AnnotationFileGeneratorTests+MemberDiversity.#ctor(System.String)"">
    <parameter name=""str"">
        <attribute ctor=""M:JetBrains.Annotations.NotNullAttribute.#ctor"" />
    </parameter>
</member>";
            Assert.That(Normalize(actual), Is.EqualTo(Normalize(expected)));
        }
    }
}
