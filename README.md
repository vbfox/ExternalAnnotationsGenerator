# ReSharper external annotations generator Library

[![Join the chat in Gitter][GitterBadge]][Gitter]
[![Build status][AppVeyorBadge]][AppVeyor]
[![MIT License][LicenseBadge]](License.md)
[![Nuget Package][NuGetBadge]][NuGet]

Fluent Library for generating [JetBrain's ReSharper][R#] External annotation
files.

## Syntax

The syntax used to create an annotation is a fluent syntax like :

```csharp
var annotator = Annotator.Create();
            
annotator.Annotate<ILogger>(type =>
{
    type.Annotate(logger => logger.Debug(NotNull<Exception>(), FormatString(), Some<object[]>()));
});

annotator.Annotate<ILoggerFactory>(type =>
{
    type.Annotate(logger => logger.GetLogger(NotNull<Type>()) == NotNull<ILogger>());
});
```

The annotations that can be found in `AnnotationGenerator.Annotations` are :
* `Some<T>`: Used as a marker for a parameter without any special attribute.
* `NotNull<T>`: Adds `[NotNullAttribute]` to either a parameter, member or
  method result.
* `CanBeNull<T>`: Adds `[CanBeNullAttribute]` to either a parameter, member or
  method result.
* `FormatString`: Adds `[StringFormatMethodAttribute]` to the method with the
  corresponding parameter name and `[NotNullAttribute]` to the parameter.
* `NullableFormatString`: Adds `[StringFormatMethodAttribute]` to the method
  with the corresponding parameter name and `[CanBeNullAttribute]` to the parameter.

## Thanks

This project started as a fork of [ReSharper.ExternalAnnotations.Generator][Upstream] by
[@chillitom][chillitom] and might not have existed without it.

[GitterBadge]: https://badges.gitter.im/Join%20Chat.svg
[Gitter]: https://gitter.im/vbfox/ExternalAnnotationsGenerator?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[NuGetBadge]: https://img.shields.io/nuget/v/ExternalAnnotationsGenerator.svg
[NuGet]: https://www.nuget.org/packages/ExternalAnnotationsGenerator
[LicenseBadge]: https://img.shields.io/badge/license-MIT%20License-blue.svg
[AppVeyorBadge]: https://ci.appveyor.com/api/projects/status/9dqk508uujs5ql2w?svg=true
[AppVeyor]: https://ci.appveyor.com/project/vbfox/externalannotationsgenerator
[R#]: https://www.jetbrains.com/resharper/
[chillitom]: https://github.com/chillitom
[Upstream]: https://github.com/chillitom/ReSharper.ExternalAnnotations.Generator