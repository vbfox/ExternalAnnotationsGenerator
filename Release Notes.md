### New in 2.3

* Add a full blown program structure to the library to avoid all project using
  it duplicating argument parsing.

### New in 2.2

* Generate packages files with ReSharper 9.0 format
  See https://www.jetbrains.com/resharper/devguide/Extensions/Packaging.html

### New in 2.1
* Don't create an extra directory in CreateNugetPackage

### New in 2.0
* Add xml documentation files to the package
* Add pdb files to the package with
  [SourceLink](https://ctaggart.github.io/SourceLink/).
* `AnnotateType` has been renamed to `Annotate`.
* Static methods in non-static types can now be annotated
  [Issue #1](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/1).
* Static methods in static types can now be annotated
  [Issue #2](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/2).
* Marker types `TClass` and `TStruct` added to standardize usage
  [Issue #3](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/3).
* Fields can now be annotated
  [Issue #6](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/6).
* Properties can now be annotated
  [Issue #7](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/7).
* **BUGFIX:** Ensure that complex generic imbrication of nested generic classes and
  generic methods can be annotated.
  [Issue #4](https://github.com/vbfox/ExternalAnnotationsGenerator/issues/4).


### New in 1.0

First release as an independent project.

* Added `CanBeNullAttribute` support
* Added support for annotations on constructor parameters
* Added support for annotations on method results with a new syntax.
* Simplified the annotator syntax to remove the assembly level.
* Generic classes and methods are correctly handled.
* Converted package management from NuGet to [Paket](https://fsprojects.github.io/Paket/).

### Original version (0.0.0.0)

Forked from [@chillitom][chillitom] project [at commit ba9cf8b852][ba9cf8b852].

[chillitom]: https://github.com/chillitom
[ba9cf8b852]: https://github.com/chillitom/ReSharper.ExternalAnnotations.Generator/tree/ba9cf8b852843fd6f93cd3a237d3b3079dffd58d