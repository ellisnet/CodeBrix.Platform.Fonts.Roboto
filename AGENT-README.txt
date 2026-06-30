========================================================================
AGENT-README: CodeBrix.Platform.Fonts.Roboto
A Comprehensive Guide for AI Coding Agents
========================================================================


OVERVIEW
========================================================================

CodeBrix.Platform.Fonts.Roboto is a .NET 10 redistribution of the Roboto
font family, packaged for the CodeBrix family. It supplies the Roboto
variable font and a curated set of static instances as build-time content
assets for CodeBrix.Platform-forked applications, and is equally usable as
a plain content-files NuGet in any .NET 10 project.

The library has effectively no managed code: the assembly is a metadata-
only .NET 10 DLL whose sole purpose is to host the bundled font content
files. The interesting payload lives in:

  - 37 `.ttf` font files (1 variable + 36 static) under
    lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/ inside the nupkg.
  - A `.ttf.manifest` JSON that maps font_style/font_weight/font_stretch
    triples to the matching static font file path.
  - A `.uprimarker` file that CodeBrix.Platform build pipelines use to
    discover UPRI-bearing font asset packages.
  - An MSBuild `.targets` file under buildTransitive/net10.0/ that hooks
    into the CodeBrix.Platform `_CodeBrixAddLibraryAssets` target and
    prunes the redundant static fonts at consumer-build time, depending on
    the `SupportsFontManifest` MSBuild property — while always keeping the
    variable `Roboto.ttf` present.


INSTALLATION
========================================================================

NuGet package: CodeBrix.Platform.Fonts.Roboto.OflLicenseForever

  dotnet add package CodeBrix.Platform.Fonts.Roboto.OflLicenseForever

The library namespace inside the assembly is `CodeBrix.Platform.Fonts.Roboto`
(without the `.OflLicenseForever` suffix; that suffix exists only on the
NuGet PackageId for license-disambiguation across the CodeBrix family).

Target framework: .NET 10.0 or higher.


KEY NAMESPACE
========================================================================

The library exposes no public managed types in its first iteration — the
assembly is metadata-only. Consumers reference the bundled font content
files via `ms-appx:///` URIs rooted at the assembly content folder:

  ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf
  ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto-Bold.ttf
  ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto_Condensed-Regular.ttf
  ...etc.


FONT INVENTORY
========================================================================

The package ships 37 `.ttf` files plus 1 `.ttf.manifest`:

Variable font (always present on every platform):
  Roboto.ttf  — covers the weight axis (100-900) plus the width axis.
                Renamed, byte-for-byte, from the upstream variable-font
                file `Roboto-VariableFont_wdth,wght.ttf`.

Static fonts (used where fonts are resolved via the static manifest):
  Six weights (Light, Regular, Medium, SemiBold, Bold, ExtraBold)
  in two styles (Normal, Italic) across three stretches:
    - Normal stretch:        Roboto-{Weight}{Italic?}.ttf      (12 files)
    - Condensed stretch:     Roboto_Condensed-{Weight}{Italic?}.ttf      (12 files)
    - SemiCondensed stretch: Roboto_SemiCondensed-{Weight}{Italic?}.ttf  (12 files)

  Note: upstream Roboto also ships Thin (100), ExtraLight (200), and
  Black (900) static instances; those are intentionally NOT bundled as
  statics here (they remain reachable through the variable font). This
  keeps the static set aligned with the sibling CodeBrix.Platform.Fonts
  packages.

Manifest:
  Roboto.ttf.manifest — JSON array of 36 entries mapping
    {font_style, font_weight, font_stretch} triples to the matching
    static font file's `ms-appx:///` URI.


CORE API REFERENCE
========================================================================

This library has no public managed API. Consumers interact with it only
through:

  1. NuGet content paths (`ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/...`)
     used as `FontFamily` values in XAML or in code that constructs XAML
     element trees, or by setting the CodeBrix.Platform default font:

       global::CodeBrix.Platform.UI.FeatureConfiguration.Font.DefaultTextFontFamily =
           "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf";

  2. The MSBuild `.targets` file under buildTransitive/net10.0/
     `CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets`, whose
     on-disk filename matches the NuGet PackageId so that NuGet's auto-
     import convention (NU5129) picks it up in consumer builds. It
     contains the target:

       <Target Name="CodeBrixRemoveUnusedRoboto"
               AfterTargets="_CodeBrixAddLibraryAssets">

     On platforms that do not support the font manifest, this target
     removes the static fonts (leaving only the variable font). The
     variable `Roboto.ttf` is never removed, so the direct
     `ms-appx:///.../Roboto.ttf` reference resolves on every platform.

If a future iteration of this library exposes a managed API (e.g. typed
accessors that return font streams or paths for non-CodeBrix.Platform
consumers), it will live under the `CodeBrix.Platform.Fonts.Roboto` root
namespace and be documented in this file.


ARCHITECTURE
========================================================================

Repository layout:

  CodeBrix.Platform.Fonts.Roboto/
    src/CodeBrix.Platform.Fonts.Roboto/
      CodeBrix.Platform.Fonts.Roboto.csproj
      InternalsVisibleTo.cs
      CodeBrix.Platform.Fonts.Roboto.uprimarker     (empty file)
      buildTransitive/
        net10.0/
          CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets
      Fonts/
        Roboto.ttf
        Roboto.ttf.manifest
        Roboto-{Light|Regular|Medium|SemiBold|Bold|ExtraBold}{Italic?}.ttf
        Roboto_Condensed-{Weight}{Italic?}.ttf
        Roboto_SemiCondensed-{Weight}{Italic?}.ttf
    tests/CodeBrix.Platform.Fonts.Roboto.Tests/
      CodeBrix.Platform.Fonts.Roboto.Tests.csproj
      AssemblyMetadataTests.cs
      ContentFilePresenceTests.cs
      ContentManifestTests.cs
      TargetsFileTests.cs
      TestAssetPaths.cs
    AGENT-README.txt
    LICENSE                  (SIL OFL 1.1)
    OFL.txt                  (SIL OFL 1.1; identical to LICENSE)
    README.md
    THIRD-PARTY-NOTICES.txt

Inside the produced NuGet (.nupkg), the file layout is:
  buildTransitive/net10.0/CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.dll
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.uprimarker
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/*.ttf
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf.manifest
  AGENT-README.txt
  README.md
  OFL.txt
  THIRD-PARTY-NOTICES.txt
  icon-codebrix-128.png

The `lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/` content layout is
load-bearing: the `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/...`
URIs that consumers reference resolve relative to the assembly name, so if
the assembly is renamed the content folder must be renamed in lockstep.


CODING CONVENTIONS (CodeBrix family)
========================================================================

This repository follows every CodeBrix family convention. Most are
inherited from the standard library scaffold; key points:

  * Target framework: net10.0 only. No multi-targeting.
  * Nullable reference types (NRT): OFF (do not set <Nullable>enable</Nullable>).
    No `?` annotations on reference types; no `!` null-forgiveness operator.
    Value-type nullables (`int?`, `DateOnly?`, etc.) are fine.
  * No global usings.
  * `<GenerateDocumentationFile>true</GenerateDocumentationFile>` is on.
    Every public/protected member of a public type needs an XML doc
    comment. CS1591 is fixed at source, never suppressed. (In this
    library's first iteration there are no public types, so CS1591
    is trivially clean.)
  * Tests use xUnit v3 + SilverAssertions; coverlet.collector for
    coverage; `TestContext.Current.CancellationToken` is threaded through
    any cancellable call inside a test.
  * No project-level warning suppression (`<NoWarn>`, `<WarningLevel>0</>`,
    `<TreatWarningsAsErrors>false</>`, etc. are all forbidden).
  * The whole package — wrapper code and bundled fonts alike — is licensed
    under SIL OFL 1.1; the csproj `<PackageLicenseExpression>` is `OFL-1.1`.
    The `<Copyright>` line preserves the upstream font attribution:
      Copyright (c) 2026 Jeremy Ellis and contributors. Roboto font (c)
      2011 The Roboto Project Authors, distributed under SIL OFL 1.1.

For the full list of family conventions see CODEBRIX_LIBRARY_OBSERVATIONS.txt
in the CodeBrix.Library.Dev-private repo.


TESTING
========================================================================

Tests live under tests/CodeBrix.Platform.Fonts.Roboto.Tests/. Run with:

  dotnet test CodeBrix.Platform.Fonts.Roboto.slnx

The test suite covers:

  * Manifest JSON: that Roboto.ttf.manifest deserializes cleanly, contains
    the expected number of entries (36), and that every entry's
    family_name path is rooted at
    `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/`.
  * Content-file presence: that all 36 static `.ttf` files referenced by
    the manifest plus the variable `Roboto.ttf` (37 total) exist on disk
    next to the test assembly's expected build-output font folder
    (resolved via `AppContext.BaseDirectory` + `TestAssets/Fonts/`,
    centralized in `TestAssetPaths`).
  * Assembly metadata: that the produced library assembly is named
    `CodeBrix.Platform.Fonts.Roboto` and exports no public types, and that
    its `.uprimarker` sibling file exists.
  * .targets file: that the buildTransitive .targets file is present next
    to the test assembly, that it declares the `CodeBrixRemoveUnusedRoboto`
    MSBuild target, that it hooks `AfterTargets="_CodeBrixAddLibraryAssets"`,
    and that it references only CodeBrix-named build targets.


PROVENANCE
========================================================================

This package is not a port of any upstream packaging project. The
`.csproj`, `.targets`, `.ttf.manifest`, `.uprimarker`, and documentation
are original CodeBrix-family files. The only third-party material is the
Roboto `.ttf` font binaries, which are redistributed bit-for-bit
unmodified. Their per-file provenance and the SIL OFL 1.1 terms are
recorded in THIRD-PARTY-NOTICES.txt (binary `.ttf` files cannot carry an
inline provenance comment).


KNOWN GOTCHAS
========================================================================

  * `ms-appx:///` URIs are resolved by the CodeBrix.Platform runtime, not
    by .NET itself. Outside a CodeBrix.Platform host, those URIs won't
    resolve. Plain .NET 10 console / test apps that reference this package
    can still access the .ttf files via the package's on-disk location
    (`<nuget-cache>/codebrix.platform.fonts.roboto.ofllicenseforever/<version>/lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/...`),
    but they have to do that lookup themselves.

  * The .targets file hooks `AfterTargets="_CodeBrixAddLibraryAssets"` —
    the asset target defined by the CodeBrix.Platform UI build tasks. If
    that internal MSBuild target name ever changes again, this .targets
    file must be updated in lockstep — otherwise the conditional pruning
    of static fonts will silently stop firing.

  * The variable `Roboto.ttf` is deliberately never pruned, because the
    CodeBrix.Platform default-font configuration and typical consumer XAML
    reference it by its direct `ms-appx:///.../Roboto.ttf` path. Do not add
    a branch that removes it, or those references will break on manifest-
    capable platforms.

  * Roboto's copyright statement declares no Reserved Font Name, so SIL OFL
    1.1 condition 3 does not restrict the display name. The `.ttf` binaries
    are nonetheless redistributed unmodified; do not alter the font bytes.
