using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace ExternalAnnotationsGenerator.Tests.Core.Construction
{
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        public void ParseCallToStaticVoidMethod()
        {
            var result = ParseHelper.Parse(() => TestClass.StaticVoidMethod());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Annotations, Is.Empty);
        }

        [Test]
        public void ParseCallToStaticVoidMethodWithParameter()
        {
            var result = ParseHelper.Parse(() => TestClass.StaticVoidMethod(Annotations.NotNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToStaticGetStringMethodWithCondition()
        {
            var result = ParseHelper.Parse(() => TestClass.GetStringStatic() == Annotations.NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.ParameterAnnotations, Is.Empty);
            var annotation = result.Annotations.Single();
            Assert.That(annotation.CanBeNull, Is.False);
            Assert.That(annotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToStaticGetStringMethodWithParameterAndCondition()
        {
            var result = ParseHelper.Parse(() => TestClass.GetStringStatic(Annotations.NotNull<string>()) == Annotations.NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
            var annotation = result.Annotations.Single();
            Assert.That(annotation.CanBeNull, Is.False);
            Assert.That(annotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToStaticGetStringMethod()
        {
            var result = ParseHelper.Parse(() => TestClass.GetStringStatic());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Annotations, Is.Empty);
        }

        [Test]
        public void ParseCallToStaticGetStringMethodWithParameter()
        {
            var result = ParseHelper.Parse(() => TestClass.GetStringStatic(Annotations.NotNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToConstructor()
        {
            var result = ParseHelper.Parse(() => new TestClass());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Constructor));
            Assert.That(result.Annotations, Is.Empty);
        }

        [Test]
        public void ParseCallToConstructorWithParameter()
        {
            var result = ParseHelper.Parse(() => new TestClass(Annotations.NotNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Constructor));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToVoidMethod()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Is.Empty);
        }

        [Test]
        public void ParseCallToVoidMethodWithSomeParameter()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod(Annotations.Some<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.ParameterAnnotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToVoidMethodWithNotNullParameter()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod(Annotations.NotNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.ParameterAnnotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToVoidMethodWithCanBeNullParameter()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod(Annotations.CanBeNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.ParameterAnnotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }


        [Test]
        public void ParseCallToVoidMethodWithFormatStringParameter()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod(Annotations.FormatString()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.ParameterAnnotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToVoidMethodWithNullableFormatStringParameter()
        {
            var result = ParseHelper.Parse((TestClass t) => t.VoidMethod(Annotations.NullableFormatString()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.ParameterAnnotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.ParameterAnnotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsFormatString, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToGetStringMethodWithSome()
        {
            var result = ParseHelper.Parse((TestClass t) => t.GetString() == Annotations.Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToGetStringMethodWithNotNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.GetString() == Annotations.NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToGetStringMethodWithCanBeNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.GetString() == Annotations.CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParsePropertyWithSome()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringProperty == Annotations.Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParsePropertyWithNotNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringProperty == Annotations.NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParsePropertyWithCanBeNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringProperty == Annotations.CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseFieldWithSome()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringField == Annotations.Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseFieldWithNotNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringField == Annotations.NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseFieldWithCanBeNull()
        {
            var result = ParseHelper.Parse((TestClass t) => t.StringField == Annotations.CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "UnassignedField.Global")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public class TestClass
        {
            public string StringProperty { get; set; }

            public string StringField;

            public static void StaticVoidMethod()
            {

            }

            public static void StaticVoidMethod(string str)
            {

            }

            public static string GetStringStatic()
            {
                return default(string);
            }

            public static string GetStringStatic(string str)
            {
                return default(string);
            }

            public TestClass()
            {
            }

            public TestClass(string str)
            {
                StringProperty = str;
                StringField = str;
            }

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
