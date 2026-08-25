================================================================================
MAINTAINER-README: CodeBrix.Platform.Fonts.Roboto
Notes for people and agents MAINTAINING this repository — not for package
consumers
================================================================================

If you are CONSUMING the NuGet package, stop here and read AGENT-README.txt
instead. This file is about changing the repository itself.


PURPOSE AND SCOPE
=================

This repository produces exactly one NuGet package:

  PackageId   CodeBrix.Platform.Fonts.Roboto.OflLicenseForever
  Assembly    CodeBrix.Platform.Fonts.Roboto
  Covered by  AGENT-README.txt (repository root)

The package is a font asset carrier. It has no managed code beyond an
assembly-level `InternalsVisibleTo` attribute; the deliverable is three font
families' `.ttf` sets, three static-instance manifests, a CodeBrix.Develop
descriptor, a `.uprimarker` marker and a buildTransitive MSBuild `.targets`
file. The assembly exists only so the content files get a stable,
assembly-named folder that `ms-appx:///` URIs can resolve against.

The structural fact that makes this repository different from its OpenSans
sibling: it ships COMPANION families. Roboto covers Latin, Greek and Cyrillic
but not Armenian or Georgian, so Noto Sans Armenian and Noto Sans Georgian are
bundled to supply those scripts. Three families means three manifests, three
variable fonts that must survive pruning, and a `fallbackFontUris` array in
the descriptor. Almost every non-obvious rule below follows from that.

Consequence worth internalising before editing anything: the assembly name,
the root namespace, the packed content-folder path, the `.uprimarker`
filename, every `family_name` URI in all three manifests, the `fontFamilyUri`
and both `fallbackFontUris` in CODEBRIX-DEVELOP.json, and the paths inside the
`.targets` file all encode the string `CodeBrix.Platform.Fonts.Roboto`.
Renaming the assembly means renaming all of them in lockstep, and the test
suite is written to catch a partial rename.


REPOSITORY LAYOUT
=================

  CodeBrix.Platform.Fonts.Roboto/
    CodeBrix.Platform.Fonts.Roboto.slnx
    AGENT-README.txt              consumer documentation (packed into nupkg)
    MAINTAINER-README.txt         this file (not packed)
    EXTRAS-README.txt             non-package content (not packed)
    README-INDEX.txt              map of the README files (not packed)
    README.md                     human-facing readme (packed; nuget.org)
    CODEBRIX-DEVELOP.json         font descriptor (packed to nupkg root)
    LICENSE                       SIL OFL 1.1
    OFL.txt                       SIL OFL 1.1, identical text (packed)
    THIRD-PARTY-NOTICES.txt       per-file attribution (packed)
    icon-codebrix-128.png         package icon (packed)
    AGENTS.md, CLAUDE.md, .clinerules, .cursorrules,
    .cursor/rules/agent-readme.mdc, .windsurfrules,
    .github/copilot-instructions.md, .junie/guidelines.md
                                  the 8 AI-agent pointer stubs; they all
                                  point at README-INDEX.txt and are
                                  maintained centrally — do not hand-edit

    src/CodeBrix.Platform.Fonts.Roboto/
      CodeBrix.Platform.Fonts.Roboto.csproj
      InternalsVisibleTo.cs       grants internals to the .Tests assembly
      CodeBrix.Platform.Fonts.Roboto.uprimarker       empty marker file
      buildTransitive/net10.0/
        CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets
      Fonts/
        Roboto.ttf                                   variable font
        Roboto.ttf.manifest                          36-entry manifest
        Roboto-<Weight>[Italic].ttf                  12 statics
        Roboto_Condensed-<Weight>[Italic].ttf        12 statics
        Roboto_SemiCondensed-<Weight>[Italic].ttf    12 statics
        NotoSansArmenian.ttf / .ttf.manifest         + 6 statics
        NotoSansGeorgian.ttf / .ttf.manifest         + 6 statics

    tests/CodeBrix.Platform.Fonts.Roboto.Tests/
      CodeBrix.Platform.Fonts.Roboto.Tests.csproj
      AssemblyMetadataTests.cs
      ContentFilePresenceTests.cs
      ContentManifestTests.cs
      DescriptorTests.cs
      TargetsFileTests.cs
      TestAssetPaths.cs           single place that computes test asset paths,
                                  including the CompanionFamilies array

The `.slnx` carries a "Solution Items" folder (AGENT-README.txt,
CODEBRIX-DEVELOP.json, icon-codebrix-128.png, LICENSE, OFL.txt, README.md,
THIRD-PARTY-NOTICES.txt), a nested "Solution Items/src" folder holding the
`.targets` file, a "Tests" folder holding the test project, and the library
project at the root.


BUILDING
========

    dotnet restore CodeBrix.Platform.Fonts.Roboto.slnx
    dotnet build   CodeBrix.Platform.Fonts.Roboto.slnx

The library project sets `GeneratePackageOnBuild=true`, so an ordinary build
also produces a `.nupkg` under src/.../bin/<Configuration>/. There is no
separate pack step to remember, and no build script.

Nothing in this repository's build reads CodeBrix.Platform. The `.targets`
file references CodeBrix.Platform MSBuild target names, but only as strings
that matter in a CONSUMER's build; there is no reference here.


TESTING
=======

    dotnet test CodeBrix.Platform.Fonts.Roboto.slnx

xUnit v3 with SilverAssertions; no opt-in environment variables, no special
prep, no network. Tests are pure file, JSON and metadata inspection and run
everywhere.

How the tests see the assets: the test `.csproj` links the font files, the
manifests, the `.uprimarker`, the descriptor and the `.targets` file into the
test output under `TestAssets/`, with `CopyToOutputDirectory=PreserveNewest`.
`TestAssetPaths` resolves everything from `AppContext.BaseDirectory` +
`TestAssets/`, and also exposes `CompanionFamilies` (the two Noto Sans family
names) plus `CompanionFontPath` / `CompanionManifestPath` helpers. If you add
a new packed asset, add the matching `<None ... Link="TestAssets\...">` item
or the tests will not see it.

What the suite pins:

  AssemblyMetadataTests    assembly loads by simple name; simple name is
                           `CodeBrix.Platform.Fonts.Roboto`; the assembly
                           exports no public types — this is what keeps the
                           package API-free; the `.uprimarker` sibling exists.
  ContentFilePresenceTests the 51-file inventory (48 statics + 3 variable
                           fonts) and the exact static filename grammar,
                           including the Regular-italic quirk where the
                           weight word is dropped.
  ContentManifestTests     all three manifests parse; entry counts 36 / 6 / 6;
                           the six weights are covered; every `family_name`
                           starts with
                           `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/`
                           and points at a file that exists on disk; and the
                           two companion manifests are UPRIGHT-ONLY — that
                           assertion is what keeps the missing companion
                           italics a recorded decision rather than an
                           accident.
  DescriptorTests          CODEBRIX-DEVELOP.json: schemaVersion 1; packageId
                           equals the published PackageId; displayName
                           "Roboto"; resourceKey "RobotoFont"; the
                           `fontFamilyUri` and every `fallbackFontUri` carry
                           no `#` fragment and point at fonts this package
                           actually ships; the fallback list is exactly
                           {NotoSansArmenian, NotoSansGeorgian};
                           `keyboardLayouts` has no duplicates and contains
                           `ka`, `hy` (companion-supplied) and `el` (native
                           to Roboto).
  TargetsFileTests         the `.targets` file exists; declares
                           `Name="CodeBrixRemoveUnusedRoboto"`; hooks
                           `AfterTargets="_CodeBrixAddLibraryAssets"`; uses
                           `lib\net10.0\CodeBrix.Platform.Fonts.Roboto\Fonts`
                           paths; carries the `$(SupportsFontManifest)`
                           condition; contains no foreign family token; and
                           never names `Fonts\Roboto.ttf"`,
                           `Fonts\NotoSansArmenian.ttf"` or
                           `Fonts\NotoSansGeorgian.ttf"` in a Remove
                           expression, so none of the three variable fonts
                           can ever be pruned.


PACKAGING AND PUBLISHING
========================

Pack driver: `GeneratePackageOnBuild=true` in the library csproj. Build in
Release and publish the resulting `.nupkg`. `IsPackable` is true,
`IncludeContentInPack` is true, and `PackageRequireLicenseAcceptance` is
true.

Versioning scheme: date-stamped and auto-incrementing, computed entirely from
`System.DateTime.UtcNow` in the csproj, in the form 1.<x>.<y>.<z> where

  1  major     pinned to 1 for this library
  x  minor     whole years since `_VersionBaseYear` (2026 = 0)
  y  build     UTC day of year, 1-based (Jan 1 = 1)
  z  revision  UTC minute of day, 0..1439

`Version`, `AssemblyVersion` and `FileVersion` all take that value. It is
strictly increasing over time but it is NOT SemVer — major/minor say nothing
about API compatibility. Two builds inside the same UTC minute produce the
same version, so never publish twice within one minute. Re-baseline by
changing `_VersionBaseYear`.

What ships in the nupkg:

  buildTransitive/net10.0/CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.dll
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto.uprimarker
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/*.ttf           (51)
  lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/*.ttf.manifest  (3)
  AGENT-README.txt          <- the consumer documentation that ships
  CODEBRIX-DEVELOP.json
  README.md
  OFL.txt
  THIRD-PARTY-NOTICES.txt
  icon-codebrix-128.png

MAINTAINER-README.txt, EXTRAS-README.txt and README-INDEX.txt are NOT packed;
only AGENT-README.txt is. If that ever needs to change it is a csproj edit in
the first `<ItemGroup>` of `<None Include="..\..\..." Pack="true"
PackagePath="" />` items.

Packing quirks to keep in mind:

  * Both `Fonts/*.ttf` and `Fonts/*.ttf.manifest` are wildcards here, so a new
    font or manifest dropped into the folder is packed automatically — and
    will then break the 51-file / 3-manifest tests until the expected counts
    are updated deliberately. (The sibling OpenSans repository names its
    single manifest explicitly instead; do not copy that pattern back here.)
  * The `.targets` file's on-disk NAME must stay equal to the PackageId, or
    NuGet's auto-import convention (NU5129) stops importing it in consumer
    builds and the prune silently never runs.
  * The content folder inside lib/net10.0/ must stay equal to the assembly
    name, because that is what `ms-appx:///` URIs resolve against.

License metadata: `PackageLicenseExpression` is `OFL-1.1` — the whole package,
wrapper and fonts alike. That is deliberate and differs from the OpenSans
sibling's `Apache-2.0 AND OFL-1.1`: nothing here derives from an Apache-2.0
upstream, so there is no second license to express.


PROVENANCE AND VENDORED SOURCES
===============================

This package is NOT a port of any upstream packaging project. The `.csproj`,
the buildTransitive `.targets` file, the three `.ttf.manifest` JSON files, the
`CODEBRIX-DEVELOP.json` descriptor, the `.uprimarker` marker, the
documentation and the packaging metadata are original CodeBrix-family files
and contain no third-party source code.

The only third-party material is font binaries, all SIL OFL 1.1, all
redistributed bit-for-bit unmodified. THIRD-PARTY-NOTICES.txt is authoritative
and enumerates every file; the short version:

1. Roboto (Christian Robertson / the Roboto Project). The upstream upright
   variable font `Roboto-VariableFont_wdth,wght.ttf` was RENAMED to
   `Roboto.ttf` — bytes unchanged. Only the six weights Light (300) through
   ExtraBold (800) are shipped as statics; upstream Thin (100), ExtraLight
   (200) and Black (900) statics are not included, and the separate upstream
   italic variable font is not included either. Roboto declares NO Reserved
   Font Name, so OFL condition 3 does not restrict the display name.

2. Noto Sans Armenian (the Noto Project) — the Armenian companion, bundled
   because Roboto has no Armenian coverage. Upstream variable font renamed to
   `NotoSansArmenian.ttf`. Six upright weights as statics; upstream publishes
   no italic face. No Reserved Font Name.

3. Noto Sans Georgian (the Noto Project) — the Georgian companion, same shape
   and same reasoning. Upstream variable font renamed to
   `NotoSansGeorgian.ttf`. No Reserved Font Name.

The variable-font renames are the load-bearing detail: the prune target keys
off a DASH in the filename, so a variable font must be named without one to
survive on non-manifest heads. That is why all three are `<Family>.ttf`.

Generated data: the `keyboardLayouts` array in CODEBRIX-DEVELOP.json is
GENERATED, not hand-written. It is computed by intersecting each
software-keyboard layout's required character set (from the layout
definitions in CodeBrix.Platform) against the `cmap` of every font this
package ships, then taking the UNION across the primary font and its
companions. Nothing in this repository's build performs that computation —
the array is produced by a developer-run tool and checked in as data.
Regenerate it whenever the platform's layout set changes or this package's
font set changes. It currently declares all 38 platform layouts; `ka` and
`hy` are there only because the companions supply them.

Verified font facts, for anyone regenerating or auditing (all derived from
the shipped binaries):

  Roboto.ttf              axes wght 100..900 (default 400),
                          wdth 75..100 (default 100); NO italic/slant axis;
                          18 named instances; 927 mapped codepoints, matched
                          by every static instance.
  NotoSansArmenian.ttf    axes wght 100..900, wdth 62.5..100; 9 named
                          instances; 430 mapped codepoints, including 91 of
                          the 96 Armenian block and the 5 Armenian ligatures
                          at U+FB13-U+FB17; no Cyrillic, no Greek.
  NotoSansGeorgian.ttf    axes wght 100..900, wdth 62.5..100; 9 named
                          instances; 509 mapped codepoints, covering
                          Mkhedruli (48/48), Asomtavruli (40/48), Nuskhuri
                          (40/48) and Mtavruli (46/48); no Cyrillic, no Greek.

  `.notdef` (glyph 0) is a DRAWN glyph in all three families, so unsupported
  codepoints render as visible tofu boxes. The OpenSans sibling is the
  opposite (empty `.notdef`, invisible gaps) — do not generalise coverage
  behaviour between the two packages.


CODING CONVENTIONS
==================

This repository follows every CodeBrix family convention. Most are inherited
from the standard library scaffold; the ones that matter here:

  * Target framework net10.0 only. No multi-targeting, no netstandard.
  * Nullable reference types OFF — do not add `<Nullable>enable</Nullable>`,
    no `?` annotations on reference types, no `!` null-forgiveness operator.
    Value-type nullables (`int?`, `DateOnly?`) are fine.
  * No global usings.
  * `<GenerateDocumentationFile>true</GenerateDocumentationFile>` is on.
    Every public/protected member of a public type needs an XML doc comment;
    CS1591 is fixed at source, never suppressed. There are no public types
    here, so it is trivially clean — that changes the moment anyone adds one.
  * No project-level warning suppression: `<NoWarn>`, `<WarningLevel>0`,
    `<TreatWarningsAsErrors>false` and friends are all forbidden.
  * Tests: xUnit v3 + SilverAssertions, `<Class>Tests.cs` filenames,
    snake_case test method names, `//Arrange` `//Act` `//Assert` comments in
    the body, and `TestContext.Current.CancellationToken` threaded through
    any cancellable call.
  * Every packaging library ships an `InternalsVisibleTo.cs` granting
    internals to its `.Tests` assembly. This one does, even though there are
    no internals yet.
  * `<PackageLicenseExpression>` is `OFL-1.1` — the whole package, wrapper
    code and bundled fonts alike.
  * The csproj `<Copyright>` value is, verbatim:
        Copyright (c) 2026 Jeremy Ellis and contributors. Roboto font (c)
        2011 The Roboto Project Authors; Noto Sans Armenian and Noto Sans
        Georgian fonts (c) 2022 The Noto Project Authors; all distributed
        under SIL OFL 1.1.
    The standard CodeBrix copyright line comes first, the upstream font
    attributions second.

For the full list of family conventions see CODEBRIX_LIBRARY_OBSERVATIONS.txt
in the CodeBrix.Library.Dev-private repository.


NOTES
=====

  * The static-font prune in the `.targets` file keys off a DASH in the
    filename. That is the entire rule. Any future font whose filename
    contains a dash will be pruned on non-manifest heads, and any font that
    must survive pruning must be named without one. For the companions this
    is not a nicety: they are the only source of Armenian and Georgian in the
    package, so pruning them would silently drop two whole scripts rather
    than merely degrade weights.
  * `SupportsFontManifest` is set by the CodeBrix.Platform head being built,
    not by this package and not by consumers.
  * `_CodeBrixAddLibraryAssets` is an internal CodeBrix.Platform MSBuild
    target name. It has been renamed before. If it is renamed again, this
    repository's `.targets` file and TargetsFileTests must be updated in
    lockstep — otherwise the prune stops firing silently, with no build
    error and no failing test in a consumer's build.
  * None of the three variable fonts has an italic axis. Any future decision
    to prune statics more aggressively would remove the only source of italic
    Roboto in this package, and the companions have no italics at all.
  * `keyboardLayouts` claiming `ka` and `hy` depends on the platform actually
    consulting `fallbackFontUris` for codepoints the primary font lacks. The
    descriptor was published deliberately ahead of that platform work, with
    the platform side following immediately; the DescriptorTests assertion
    that the companions are claimed is what keeps the two in step.
  * There is no font-generation, subsetting or regeneration step in this
    repository. The `.ttf` files are checked in as received; the only
    generated artefact is the `keyboardLayouts` array described above.
  * Ship with the family. CodeBrix.Platform and its font packages are
    published together as one version, one event.
