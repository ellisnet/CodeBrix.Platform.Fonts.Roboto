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

Roboto covers the Latin, Greek and Cyrillic scripts but NOT Armenian or
Georgian. This package therefore also bundles two Noto Sans COMPANION
families that supply those scripts in a matching sans design. That is the
one structural difference from the sibling CodeBrix.Platform.Fonts.OpenSans
package, and it is the thing to understand before changing anything here.

The library has effectively no managed code: the assembly is a metadata-
only .NET 10 DLL whose sole purpose is to host the bundled font content
files. The interesting payload lives in:

  - 51 `.ttf` font files (3 variable + 48 static) under
    lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/ inside the nupkg.
  - Three `.ttf.manifest` JSON files (one per family) mapping
    font_style/font_weight/font_stretch triples to the matching static
    font file path.
  - A `CODEBRIX-DEVELOP.json` descriptor at the package root that tells
    CodeBrix.Develop how to wire this font into a generated application.
  - A `.uprimarker` file that CodeBrix.Platform build pipelines use to
    discover UPRI-bearing font asset packages.
  - An MSBuild `.targets` file under buildTransitive/net10.0/ that hooks
    into the CodeBrix.Platform `_CodeBrixAddLibraryAssets` target and
    prunes the redundant static fonts at consumer-build time, depending on
    the `SupportsFontManifest` MSBuild property — while always keeping all
    three variable fonts present.


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
  ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansGeorgian.ttf
  ...etc.

Do NOT append a `#FamilyName` fragment to these URIs. CodeBrix.Platform
strips the fragment before resolving the font, so it buys nothing — and
on the value assigned to `FeatureConfiguration.Font.DefaultTextFontFamily`
it actively breaks the startup font-manifest preload, because the
".manifest" suffix the preload appends lands inside the URI fragment and
is then dropped.


FONT INVENTORY
========================================================================

The package ships 51 `.ttf` files plus 3 `.ttf.manifest` files.

PRIMARY FAMILY — Roboto (37 files)

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

COMPANION FAMILIES (14 files)

  NotoSansArmenian.ttf + 6 statics  — supplies the ARMENIAN script.
                                      Six weights, Normal stretch,
                                      upright only.
  NotoSansGeorgian.ttf + 6 statics  — supplies the GEORGIAN script.
                                      Six weights, Normal stretch,
                                      upright only.

  Neither family has an italic face upstream, so italic text in those
  scripts renders upright. That is a known upstream limitation, not a
  packaging defect. Roboto already covers Greek natively, so there is no
  Greek companion here (unlike the sibling Merriweather package).

Manifests:
  Roboto.ttf.manifest            — 36 entries
  NotoSansArmenian.ttf.manifest  —  6 entries
  NotoSansGeorgian.ttf.manifest  —  6 entries

  Each is a JSON object with a `fonts` array mapping
  {font_style, font_weight, font_stretch} triples to the matching static
  font file's `ms-appx:///` URI.


CODEBRIX-DEVELOP.JSON
========================================================================

`CODEBRIX-DEVELOP.json` sits at the repository root and is packed to the
root of the nupkg. It is the font's self-description for CodeBrix.Develop's
"New CodeBrix.Platform Application" experience: the IDE reads it to learn
how to wire this font into a generated application, instead of carrying
per-font swap logic of its own.

  schemaVersion     Always 1 today. A consumer that does not recognise
                    the value should decline the font with a clear
                    message rather than guess.
  packageId         Must equal this package's NuGet PackageId.
  displayName       The typographic family name shown to the user, and
                    the authoritative value written into generated source.
  fontFamilyUri     The ms-appx URI of the primary font. No `#` fragment.
  resourceKey       The App.xaml resource key a generated application
                    uses (`RobotoFont`).
  fallbackFontUris  Ordered ms-appx URIs of the companion fonts, consulted
                    for codepoints the primary font lacks. Absent or empty
                    means the package has no companions.
  keyboardLayouts   The software-keyboard layout ids this package's glyph
                    coverage supports, as the UNION across the primary
                    font and its companions. Ids absent from this list are
                    not supported; there is deliberately no "unsupported"
                    list, so the complement of the platform's layout set
                    is always the correct answer.

The array is generated, not hand-written — see PROVENANCE below.

IMPORTANT: `keyboardLayouts` currently claims all 38 layouts, including
`ka` and `hy`, which are delivered by the companion fonts. Those two
require CodeBrix.Platform to consult `fallbackFontUris` when the primary
font lacks a glyph. If you are reading this before that support shipped,
the claim runs ahead of the runtime by design — it was published
deliberately, with the platform work following immediately.


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
        NotoSansArmenian.ttf / .ttf.manifest / NotoSansArmenian-{Weight}.ttf
        NotoSansGeorgian.ttf / .ttf.manifest / NotoSansGeorgian-{Weight}.ttf
    tests/CodeBrix.Platform.Fonts.Roboto.Tests/
      CodeBrix.Platform.Fonts.Roboto.Tests.csproj
      AssemblyMetadataTests.cs
      ContentFilePresenceTests.cs
      ContentManifestTests.cs
      DescriptorTests.cs
      TargetsFileTests.cs
      TestAssetPaths.cs
    AGENT-README.txt
    CODEBRIX-DEVELOP.json
    LICENSE                  (SIL OFL 1.1)
    OFL.txt                  (SIL OFL 1.1; identical to LICENSE)
    README.md
    THIRD-PARTY-NOTICES.txt

Inside the produced NuGet (.nupkg), the file layout is:
  buildTransitive/net10.0/CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.dll
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.uprimarker
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/*.ttf
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/*.ttf.manifest
  AGENT-README.txt
  CODEBRIX-DEVELOP.json
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

  * Manifest JSON: that all three `.ttf.manifest` files deserialize
    cleanly, carry the expected entry counts (36/6/6), cover the six
    weights, and that every entry's family_name path is rooted at
    `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/` and points at a
    file that exists on disk. Also that the two companion manifests are
    upright-only, so that limitation stays a decision rather than an
    accident.
  * Descriptor: that CODEBRIX-DEVELOP.json declares schemaVersion 1, its
    packageId matches the published PackageId, its fontFamilyUri and every
    fallbackFontUri carry no `#` fragment and point at fonts this package
    actually ships, and that keyboardLayouts has no duplicates and claims
    the scripts the companions exist to supply.
  * Content-file presence: that all 48 static `.ttf` files plus the three
    variable fonts (51 total) exist on disk
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
Roboto and Noto Sans `.ttf` font binaries, which are redistributed
bit-for-bit unmodified. Their per-file provenance and the SIL OFL 1.1
terms are recorded in THIRD-PARTY-NOTICES.txt (binary `.ttf` files cannot
carry an inline provenance comment).

The `keyboardLayouts` array in CODEBRIX-DEVELOP.json is GENERATED, not
hand-written: it is computed by intersecting each software-keyboard
layout's required character set (from the layout definitions in
CodeBrix.Platform) against the `cmap` of every font this package ships,
then taking the union across the primary font and its companions. Nothing
in this repository's build reads CodeBrix.Platform — the array is computed
by a developer-run tool and checked in as data. Regenerate it whenever the
platform's layout set changes or this package's font set changes.


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

  * The three variable fonts are deliberately never pruned. For Roboto.ttf
    that is the usual reason (the CodeBrix.Platform default-font
    configuration and typical consumer XAML reference it by its direct
    `ms-appx:///.../Roboto.ttf` path). For the two companions it matters
    MORE: they are the only source of Armenian and Georgian in this
    package, so pruning them would silently drop two scripts rather than
    merely degrade weights. The prune matches only dash-bearing filenames,
    which is why the companion variable fonts are named without a dash.

  * NEVER add a `#FamilyName` fragment to a font URI in this package's
    documentation or descriptor. CodeBrix.Platform strips it during font
    resolution, and on `DefaultTextFontFamily` it silently disables the
    startup manifest preload (the appended ".manifest" lands inside the
    fragment and is dropped by `Uri.PathAndQuery`). Earlier revisions of
    this README used the `#Roboto` form in its XAML samples; that was
    inherited convention, not a fix for anything.

  * Roboto's copyright statement declares no Reserved Font Name, so SIL OFL
    1.1 condition 3 does not restrict the display name. The `.ttf` binaries
    are nonetheless redistributed unmodified; do not alter the font bytes.
