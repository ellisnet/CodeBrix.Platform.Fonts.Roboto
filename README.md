# CodeBrix.Platform.Fonts.Roboto

A redistribution of the Roboto font family packaged as a CodeBrix-family NuGet library for .NET 10 applications.
CodeBrix.Platform.Fonts.Roboto is a content-files font package for CodeBrix.Platform-forked applications — supplying the Roboto variable font and its static instances as build-time assets — and is equally usable as a plain content-files NuGet in any .NET 10 project that wants the Roboto font set.
The library has no managed dependencies other than .NET, and is provided as a .NET 10 library and associated `CodeBrix.Platform.Fonts.Roboto.OflLicenseForever` NuGet package.

CodeBrix.Platform.Fonts.Roboto supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## CodeBrix.Platform.Fonts.Roboto supports:

* The Roboto variable font (`Roboto.ttf`) covering the full weight axis (100-900) and width axis, used directly on every platform.
* 36 static `.ttf` font files covering the Light/Regular/Medium/SemiBold/Bold/ExtraBold weights in Normal, Italic, Condensed, Condensed-Italic, SemiCondensed, and SemiCondensed-Italic stretches — for platforms that resolve fonts through the static-instance manifest.
* A `.ttf.manifest` JSON file that maps `font_style` / `font_weight` / `font_stretch` triples to the matching static font file.
* A `buildTransitive` MSBuild `.targets` file (hooking into the CodeBrix.Platform `_CodeBrixAddLibraryAssets` target) that prunes the redundant static font files at build time on platforms that don't need them, while always keeping the variable `Roboto.ttf` available.
* The CodeBrix `.uprimarker` file so CodeBrix.Platform build pipelines discover the package as a UPRI-bearing font asset library.

## Sample Code

### Reference the font from XAML (CodeBrix.Platform app)

```xml
<TextBlock Text="Hello, world."
           FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf#Roboto" />
```

### Reference a specific static weight

```xml
<TextBlock Text="Bold sample"
           FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto-Bold.ttf#Roboto" />
```

### Set Roboto as the default text font (CodeBrix.Platform app)

```csharp
global::CodeBrix.Platform.UI.FeatureConfiguration.Font.DefaultTextFontFamily =
    "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf";
```

## License

The entire package — the library code, the `.targets` file, the packaging wrapper, and the bundled Roboto `.ttf` font files — is licensed under the SIL Open Font License, Version 1.1. see: https://en.wikipedia.org/wiki/SIL_Open_Font_License

The full license text is bundled with this repository as `OFL.txt` at the repository root and is also packaged inside the produced NuGet under the same name. The package is published under the SPDX expression `OFL-1.1`.
