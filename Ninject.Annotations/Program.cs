using System;
using AnnotationGenerator;
using Ninject.Extensions.Logging;
using static AnnotationGenerator.ParameterNotes;

namespace Ninject.Annotations
{
    static class Program
    {
        static void Main(string[] args)
        {
            var annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type =>
                {
                    type.Annotate(logger => logger.Debug(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Debug(NotNull<Exception>(), FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Info(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Info(NotNull<Exception>(), FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Trace(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Trace(NotNull<Exception>(), FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Warn(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Warn(NotNull<Exception>(), FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Error(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Error(NotNull<Exception>(), FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Fatal(FormatString(), Some<object[]>()));
                    type.Annotate(logger => logger.Fatal(NotNull<Exception>(), FormatString(), Some<object[]>()));
                });

                asm.AnnotateType<ILoggerFactory>(type =>
                {
                    type.Annotate(logger => logger.GetLogger(NotNull<Type>()) == NotNull<ILogger>());
                });
            });

            var version = args.Length > 0 ? args[0] : "1.0.0.0";
            /*
            annotator.CreateNugetPackage(
                new NugetSpec(
                    version: version, 
                    id: "Ninject.Annotations", 
                    title: "Ninject Annotations",
                    authors: "Tom Rathbone", 
                    owners: "Tom Rathbone",
                    projectUrl: "https://github.com/chillitom/ReSharper.ExternalAnnotations.Generator/blob/master/Ninject.Annotations/Program.cs", 
                    iconUrl: "https://raw.githubusercontent.com/ninject/ninject/master/logos/Ninject-Logo32.png", 
                    description: "External Annotations for Ninject and Ninject Extensions"));*/
        }
    }
}
