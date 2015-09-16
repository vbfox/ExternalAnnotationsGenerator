using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Expressions;
using AnnotationGenerator.Notes;
using NUnit.Framework;
using static AnnotationGenerator.ParameterNotes;

namespace AnnotationGenerator.Tests.Expressions
{
    [TestFixture]
    public class ExpressionParserTests
    {
        private ExpressionParsingResult Parse<T>(Expression<Action<T>> expression) => ExpressionParser.Parse(expression);
        private ExpressionParsingResult Parse<TIn, TOut>(Expression<Func<TIn, TOut>> expression) => ExpressionParser.Parse(expression);

        [Test]
        public void ParseCallToVoidMethod()
        {
            var result = Parse((TestClass t) => t.VoidMethod());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Is.Empty);
        }

        [Test]
        public void ParseCallToVoidMethodWithSomeParameter()
        {
            var result = Parse((TestClass t) => t.VoidMethod(Some<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (ParameterAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToVoidMethodWithNotNullParameter()
        {
            var result = Parse((TestClass t) => t.VoidMethod(NotNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (ParameterAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToVoidMethodWithCanBeNullParameter()
        {
            var result = Parse((TestClass t) => t.VoidMethod(CanBeNull<string>()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (ParameterAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsFormatString, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }


        [Test]
        public void ParseCallToVoidMethodWithFormatStringParameter()
        {
            var result = Parse((TestClass t) => t.VoidMethod(FormatString()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (ParameterAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsFormatString, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToVoidMethodWithNullableFormatStringParameter()
        {
            var result = Parse((TestClass t) => t.VoidMethod(NullableFormatString()));

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("VoidMethod"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (ParameterAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.ParameterName, Is.EqualTo("str"));
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsFormatString, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToGetStringMethodWithSome()
        {
            var result = Parse((TestClass t) => t.GetString() == Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseCallToGetStringMethodWithNotNull()
        {
            var result = Parse((TestClass t) => t.GetString() == NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseCallToGetStringMethodWithCanBeNull()
        {
            var result = Parse((TestClass t) => t.GetString() == CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Method));
            Assert.That(result.Member.Name, Is.EqualTo("GetString"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParsePropertyWithSome()
        {
            var result = Parse((TestClass t) => t.StringProperty == Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParsePropertyWithNotNull()
        {
            var result = Parse((TestClass t) => t.StringProperty == NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParsePropertyWithCanBeNull()
        {
            var result = Parse((TestClass t) => t.StringProperty == CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Property));
            Assert.That(result.Member.Name, Is.EqualTo("StringProperty"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseFieldWithSome()
        {
            var result = Parse((TestClass t) => t.StringField == Some<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        [Test]
        public void ParseFieldWithNotNull()
        {
            var result = Parse((TestClass t) => t.StringField == NotNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.False);
            Assert.That(paramAnnotation.IsNotNull, Is.True);
        }

        [Test]
        public void ParseFieldWithCanBeNull()
        {
            var result = Parse((TestClass t) => t.StringField == CanBeNull<string>());

            Assert.That(result.Member.MemberType, Is.EqualTo(MemberTypes.Field));
            Assert.That(result.Member.Name, Is.EqualTo("StringField"));
            Assert.That(result.Annotations, Has.Count.EqualTo(1));
            var paramAnnotation = (MemberAnnotationInfo)result.Annotations.Single();
            Assert.That(paramAnnotation.CanBeNull, Is.True);
            Assert.That(paramAnnotation.IsNotNull, Is.False);
        }

        public class TestClass
        {
            public string StringProperty { get; set; }

            public string StringField;

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
