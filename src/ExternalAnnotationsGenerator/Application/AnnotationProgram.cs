using System;
using System.IO;
using ExternalAnnotationsGenerator.Application.Options;
using JetBrains.Annotations;

namespace ExternalAnnotationsGenerator.Application
{
    public class AnnotationProgram
    {
        private readonly string exeName;
        private readonly Action<IAnnotator> annotate;
        private readonly NugetSpec nugetSpec;

        public AnnotationProgram([NotNull] string exeName, [NotNull] Action<IAnnotator> annotate,
            [NotNull] NugetSpec nugetSpec)
        {
            if (exeName == null) throw new ArgumentNullException(nameof(exeName));
            if (annotate == null) throw new ArgumentNullException(nameof(annotate));
            if (nugetSpec == null) throw new ArgumentNullException(nameof(nugetSpec));

            this.exeName = exeName;
            this.annotate = annotate;
            this.nugetSpec = nugetSpec;
        }

        private const int SuccessExitCode = 0;
        private const int ExceptionExitCode = 1;
        private const int InvalidArgumentsExitCode = 2;

        public static int Run([NotNull] string exeName, [NotNull, ItemNotNull] string[] args,
            [NotNull] Action<IAnnotator> annotate, [NotNull] NugetSpec nugetSpec)
        {
            if (exeName == null) throw new ArgumentNullException(nameof(exeName));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (annotate == null) throw new ArgumentNullException(nameof(annotate));
            if (nugetSpec == null) throw new ArgumentNullException(nameof(nugetSpec));

            return new AnnotationProgram(exeName, annotate, nugetSpec).Run(args);
        }

        public int Run([NotNull, ItemNotNull] string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            try
            {
                var parsedArgs = ParseArgs(args);
                if (parsedArgs.ShowHelp)
                {
                    ShowHelp(exeName, parsedArgs);
                    return parsedArgs.ParseError == null ? SuccessExitCode : InvalidArgumentsExitCode;
                }

                var annotator = GetAnnotator();
                RunGeneration(annotator, nugetSpec, parsedArgs);
                return SuccessExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return ExceptionExitCode;
            }
        }

        private IAnnotator GetAnnotator()
        {
            var annotator = Annotator.Create();
            annotate(annotator);
            return annotator;
        }

        private static void RunGeneration(IAnnotator annotator, NugetSpec nuspec, Args parsedArgs)
        {
            var version = parsedArgs.Version ?? new Version("1.0.0.0");
            var dir = parsedArgs.Directory ?? new DirectoryInfo(Environment.CurrentDirectory);
            var fixedSpec = nuspec.WithVersion(version);
            annotator.CreateNugetPackage(fixedSpec, dir);

            Console.WriteLine($"Generated version {version}  in {dir.FullName}");
        }

        private static void ShowHelp(string exeName, Args args)
        {
            var set = GetOptionSet(new Args());
            if (args.ParseError != null)
            {
                Console.WriteLine(args.ParseError);
                Console.WriteLine();
            }
            Console.WriteLine($"Usage: {exeName} [OPTIONS]+");
            Console.WriteLine("Generate the nuget annotation package");
            Console.WriteLine();
            Console.WriteLine("Options:");
            set.WriteOptionDescriptions(Console.Out);
        }

        private class Args
        {
            public bool ShowHelp { get; set; }
            public Version Version { get; set; }
            public string ParseError { get; set; }
            public DirectoryInfo Directory { get; set; }
        }

        private static OptionSet GetOptionSet(Args args)
        {
            return new OptionSet
            {
                {
                    "h|?|help",
                    "show this message and exit",
                    v => args.ShowHelp = v != null
                },
                {
                    "v|version=",
                    "Version of the generated package (Default: 1.0.0.0)",
                    v => args.Version = new Version(v)
                },
                {
                    "d|directory=",
                    "The root directory of the NuGet package (Default: Current directory)",
                    v => args.Directory = new DirectoryInfo(v)
                }
            };
        }

        private static Args ParseArgs(string[] args)
        {
            var result = new Args();
            try
            {
                var set = GetOptionSet(result);

                var extra = set.Parse(args);
                if (extra.Count != 0)
                {
                    result.ShowHelp = true;
                    result.ParseError = "Unknown parameters : " + string.Join(" ", extra);
                }

                return result;
            }
            catch (Exception exception)
            {
                result.ShowHelp = true;
                result.ParseError = "Error : " + exception.Message;
                return result;
            }
        }
    }
}
