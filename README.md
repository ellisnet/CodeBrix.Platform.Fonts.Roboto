# CodeBrix.Platform.Fonts.Roboto

A redistribution of the Roboto font family packaged as a CodeBrix-family NuGet library for .NET 10 applications.
CodeBrix.Platform.Fonts.Roboto is a content-files font package for CodeBrix.Platform applications — supplying the Roboto variable font and its static instances as build-time assets — and is equally usable as a plain content-files NuGet in any .NET 10 project that wants the Roboto font set.
Roboto covers the Latin and Cyrillic scripts and modern (monotonic) Greek, but not the polytonic Greek of the Greek Extended block, and not Armenian or Georgian, so this package also bundles three Noto Sans companion families that supply those scripts in a matching sans design.
The library has no managed dependencies other than .NET, and is provided as a .NET 10 library and associated `CodeBrix.Platform.Fonts.Roboto.OflLicenseForever` NuGet package.

CodeBrix.Platform.Fonts.Roboto supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## Installation

```
dotnet add package CodeBrix.Platform.Fonts.Roboto.OflLicenseForever
```

Note that the NuGet package ID and the assembly name are different - there is no package named plain `CodeBrix.Platform.Fonts.Roboto`:

* NuGet package ID: `CodeBrix.Platform.Fonts.Roboto.OflLicenseForever`
* Assembly, content folder and font-URI name: `CodeBrix.Platform.Fonts.Roboto` - i.e. `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf`

The `.OflLicenseForever` suffix is a CodeBrix family convention that records the license the package will always be published under. Use the un-suffixed name in every font URI; use the suffixed name only in `dotnet add package` and in the `.targets` filename.

The package has no NuGet dependencies of its own, and there is no companion package to add: the Noto Sans, Noto Sans Armenian and Noto Sans Georgian faces are files inside this package. It also sets `PackageRequireLicenseAcceptance`, so a restore in an interactive tool prompts for license acceptance.

There is no API to call and no namespace to import - the payload is font data, a font descriptor and a build-time MSBuild `.targets` file.

## CodeBrix.Platform.Fonts.Roboto supports:

* The Roboto variable font (`Roboto.ttf`) covering the full weight axis (100-900) and width axis, used directly on every platform.
* 36 static `.ttf` font files covering the Light/Regular/Medium/SemiBold/Bold/ExtraBold weights in Normal, Italic, Condensed, Condensed-Italic, SemiCondensed, and SemiCondensed-Italic stretches — for platforms that resolve fonts through the static-instance manifest.
* Three companion font families that extend script coverage beyond what Roboto itself carries:
  * **Noto Sans** (`NotoSans.ttf` plus 12 static instances, upright and italic) — polytonic (ancient) Greek. Roboto carries only monotonic Greek: 75 code points of Greek and Coptic and a single code point of the Greek Extended block, with none of the combining marks polytonic would otherwise be composed from. Noto Sans supplies all 233 assigned Greek Extended code points.
  * **Noto Sans Armenian** (`NotoSansArmenian.ttf` plus 6 static instances) — the Armenian script.
  * **Noto Sans Georgian** (`NotoSansGeorgian.ttf` plus 6 static instances) — the Georgian script.
* A `.ttf.manifest` JSON file per family that maps `font_style` / `font_weight` / `font_stretch` triples to the matching static font file.
* A `CODEBRIX-DEVELOP.json` descriptor that tells CodeBrix.Develop how to wire this font into a generated application and which software-keyboard layouts the package's glyph coverage supports.
* A `buildTransitive` MSBuild `.targets` file (hooking into the CodeBrix.Platform `_CodeBrixAddLibraryAssets` target) that prunes the redundant static font files at build time on platforms that don't need them, while always keeping the four variable fonts available.
* The CodeBrix `.uprimarker` file so CodeBrix.Platform build pipelines discover the package as a UPRI-bearing font asset library.

## Sample Code

### Reference the font from XAML (CodeBrix.Platform app)

```xml
<TextBlock Text="Hello, world."
           FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf" />
```

### Reference a specific static weight

```xml
<TextBlock Text="Bold sample"
           FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto-Bold.ttf" />
```

### Render polytonic (ancient) Greek

Roboto has no polytonic glyphs, so ancient Greek must be set in the Noto Sans companion:

```xml
<TextBlock Text="μῆνιν ἄειδε θεὰ Πηληϊάδεω Ἀχιλῆος"
           FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSans.ttf" />
```

### Set Roboto as the default text font (CodeBrix.Platform app)

```csharp
global::CodeBrix.Platform.UI.FeatureConfiguration.Font.DefaultTextFontFamily =
    "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf";
```

Note that the font URI carries no `#FamilyName` fragment. CodeBrix.Platform strips such a fragment before resolving the font, and leaving it on the value assigned to `DefaultTextFontFamily` prevents the startup font-manifest preload from finding the manifest.

## Documentation

The NuGet package includes `AGENT-README.txt`, a complete reference and usage guide written for AI coding agents - point your agent at that file when it is wiring this font package into an application.

Additional sample code and usage examples are available in the `CodeBrix.Platform.Fonts.Roboto.Tests` project:
https://github.com/ellisnet/CodeBrix.Platform.Fonts.Roboto/tree/main/tests/CodeBrix.Platform.Fonts.Roboto.Tests

## License

CodeBrix.Platform.Fonts.Roboto is licensed under the SIL Open Font License, Version 1.1 - see the
[LICENSE](https://github.com/ellisnet/CodeBrix.Platform.Fonts.Roboto/blob/main/LICENSE) file.

The entire package — the library code, the `.targets` file, the packaging wrapper, and the bundled Roboto and Noto Sans `.ttf` font files — is covered by that license. Its full text is bundled with this repository as `OFL.txt` at the repository root and is also packaged inside the produced NuGet under the same name. The package is published under the SPDX expression `OFL-1.1`.

For licensing and provenance information about the open source code included in
this package, see [THIRD-PARTY-NOTICES.txt](https://github.com/ellisnet/CodeBrix.Platform.Fonts.Roboto/blob/main/THIRD-PARTY-NOTICES.txt).
