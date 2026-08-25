================================================================================
AGENT-README: CodeBrix.Platform.Fonts.Roboto
A Guide for AI Coding Agents — CONSUMING the
CodeBrix.Platform.Fonts.Roboto.OflLicenseForever NuGet package
================================================================================


OVERVIEW
========

CodeBrix.Platform.Fonts.Roboto is a redistribution of the Roboto font family,
packaged as a content-files NuGet library. It supplies the Roboto variable
font and a curated set of static instances as build-time content assets for
CodeBrix.Platform applications, and is equally usable as a plain content-files
NuGet in any .NET 10 project that wants the Roboto font set.

Target framework: .NET 10 or later.

Roboto covers the Latin, Greek and Cyrillic scripts but NOT Armenian or
Georgian. This package therefore also bundles two Noto Sans COMPANION
families that supply those scripts in a matching sans design. The companions
ship INSIDE this package — they are not separate NuGet packages, and a
consumer does not reference anything extra to get them. That is the one
structural difference from the sibling CodeBrix.Platform.Fonts.OpenSans
package, and it is the thing to understand first.

The package has effectively no managed code. Its assembly is a metadata-only
DLL whose sole purpose is to host the bundled font content files and to give
those files a stable content-folder name. Everything a consumer uses is data:

  - 51 `.ttf` font files (3 variable + 48 static) laid out under
    lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/ inside the nupkg.
  - Three `.ttf.manifest` JSON documents, one per family, mapping
    font_style / font_weight / font_stretch triples to the matching static
    font file, so a platform that cannot render variable fonts can still
    honour a weight/style/stretch request.
  - `CODEBRIX-DEVELOP.json` at the package root — the font's own description
    of how to wire it into an application, including the two companion URIs.
  - A `.uprimarker` marker file that CodeBrix.Platform build pipelines use to
    discover font asset packages.
  - An MSBuild `.targets` file under buildTransitive/net10.0/ that prunes the
    redundant static fonts at consumer-build time.

Provenance (one line): this package is not a port of any upstream packaging
project — the `.csproj`, `.targets`, manifests, descriptor and marker are
original CodeBrix-family files, and the only third-party material is the
Roboto, Noto Sans Armenian and Noto Sans Georgian `.ttf` binaries,
redistributed bit-for-bit unmodified.


INSTALLATION
============

NuGet PackageId: CodeBrix.Platform.Fonts.Roboto.OflLicenseForever

    dotnet add package CodeBrix.Platform.Fonts.Roboto.OflLicenseForever

License: OFL-1.1 (that exact SPDX expression is the package's declared
license). The whole package is under the SIL Open Font License 1.1 — the
wrapper assembly, the MSBuild `.targets` file and the packaging metadata as
well as the bundled Roboto and Noto Sans font files. `OFL.txt` (the full
license text) and `THIRD-PARTY-NOTICES.txt` ship inside the nupkg. The
package sets PackageRequireLicenseAcceptance, so a restore in an interactive
tool will prompt for license acceptance.

NuGet dependencies: none. The package has no PackageReference of its own, and
the Armenian and Georgian companion faces are files in THIS package — there
is no companion package to add.

Requirements and limits:
  - .NET 10.0 or later. There is no multi-targeting and no netstandard asset.
  - No native libraries, no OS restriction: the payload is font data.
  - The `ms-appx:///` URIs below are resolved by the CodeBrix.Platform
    runtime. Outside a CodeBrix.Platform host they are just strings — see
    COMMON PITFALLS TO AVOID for how a plain .NET app reaches the files.

The assembly (and therefore the content-folder name inside the nupkg) is
`CodeBrix.Platform.Fonts.Roboto`, without the `.OflLicenseForever` suffix.
That suffix exists only on the NuGet PackageId, to disambiguate license
variants across the CodeBrix family. Use the un-suffixed name in every URI;
use the suffixed name only in `dotnet add package` and in the `.targets`
filename.

See also: the sibling packages
`CodeBrix.Platform.Fonts.OpenSans.ApacheLicenseForever` (no companions, so no
Armenian or Georgian) and `CodeBrix.Platform.Fonts.RobotoMono.OflLicenseForever`
(the monospace counterpart).


KEY NAMESPACES / USINGS
=======================

There are no namespaces to import and no `using` directives to add. The
assembly exports no public types (a test in this repository asserts that
`GetExportedTypes()` is empty), so nothing in it can be referenced from C#.

The "namespace" a consumer actually works in is the `ms-appx:///` URI space
rooted at the assembly content-folder name:

    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/<FileName>.ttf

Examples:

    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf
    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto-Bold.ttf
    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto_Condensed-Regular.ttf
    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansArmenian.ttf
    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansGeorgian.ttf

Never append a `#FamilyName` fragment to these URIs. CodeBrix.Platform strips
the fragment before resolving the font, so it buys nothing — and on the value
assigned to `FeatureConfiguration.Font.DefaultTextFontFamily` it actively
breaks the startup font-manifest preload, because the ".manifest" suffix the
preload appends then lands inside the URI fragment and is dropped by
`Uri.PathAndQuery`.

The one C# symbol a consumer typically touches lives in CodeBrix.Platform,
not here:

    global::CodeBrix.Platform.UI.FeatureConfiguration.Font.DefaultTextFontFamily


CORE API REFERENCE
==================

This package has no managed API. Its complete consumer contract is these
four surfaces.

1. Font URIs
------------
Every `.ttf` in the package is addressable as

    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/<FileName>.ttf

and is valid anywhere CodeBrix.Platform accepts a `FontFamily` value: the
`FontFamily` property of `TextBlock`, `Run`, `TextBox`, `Button` and other
text-bearing controls; a `FontFamily` resource in a ResourceDictionary; and
`FeatureConfiguration.Font.DefaultTextFontFamily`. The full URI list is in
QUICK REFERENCE CARD below.

2. The static-instance manifests
--------------------------------
Each family has its own manifest, discovered by name (font file path +
".manifest"), sitting beside its variable font in the same Fonts folder:

    Roboto.ttf.manifest            36 entries
    NotoSansArmenian.ttf.manifest   6 entries
    NotoSansGeorgian.ttf.manifest   6 entries

Each file is a JSON OBJECT with a single `fonts` property holding an array —
NOT a bare JSON array. Each entry has exactly four properties:

    {
      "font_style":   "Normal" | "Italic",
      "font_weight":  300 | 400 | 500 | 600 | 700 | 800,
      "font_stretch": "Normal" | "Condensed" | "SemiCondensed",
      "family_name":  "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/<file>.ttf"
    }

Despite its name, `family_name` holds a URI, not a typographic family name.

The three key properties are exactly the XAML text properties `FontStyle`,
`FontWeight` and `FontStretch`, using the same value names and the same
numeric weight scale. That is the whole selection mechanism: you set those
properties on the element, CodeBrix.Platform looks the triple up in the
manifest of the font family you named, and renders the static instance the
matching entry points at. You never name a static file yourself unless you
want to pin one exactly.

Roboto's manifest is complete and rectangular:
2 styles x 6 weights x 3 stretches = 36 entries, one per static `.ttf`.

The two companion manifests are 6 entries each: six weights, `font_style`
"Normal" only, `font_stretch` "Normal" only. There is no italic and no
condensed companion face, because upstream publishes none. `FontStyle="Italic"`
on Armenian or Georgian text has no italic entry to resolve to.

3. The build-time `.targets` file
---------------------------------
`buildTransitive/net10.0/CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets`
is auto-imported into consumer builds by NuGet convention (its on-disk
filename matches the PackageId, per NU5129). It declares one target:

    <Target Name="CodeBrixRemoveUnusedRoboto"
            AfterTargets="_CodeBrixAddLibraryAssets">

Behaviour: when the MSBuild property `SupportsFontManifest` is not `'true'`,
the target removes every dash-bearing font filename — that is, all 48 static
instances across the three families — from the asset item list, leaving only
`Roboto.ttf`, `NotoSansArmenian.ttf` and `NotoSansGeorgian.ttf` in the
application output. When `SupportsFontManifest` is `'true'`, nothing is
removed and all 51 files ship.

Those three variable fonts are never removed on any platform, so a direct
`ms-appx:///.../Roboto.ttf` (or companion) reference always resolves. The
prune keys off the dash in the filename; that is why all three variable fonts
are named without one. For the companions this matters more than for Roboto:
they are the only source of Armenian and Georgian in the package, so pruning
them would silently drop two scripts rather than merely degrade weights.

Consumers do not set `SupportsFontManifest` themselves — the CodeBrix.Platform
head being built sets it. Treat it as an input you read, not one you write.

4. `CODEBRIX-DEVELOP.json`
--------------------------
Packed to the root of the nupkg. It is this font's self-description, read by
CodeBrix.Develop's "New CodeBrix.Platform Application" experience so the IDE
does not have to carry per-font wiring logic. Fields actually present in this
package:

  schemaVersion     1. A reader that does not recognise the value should
                    decline the font with a clear message rather than guess.
  packageId         "CodeBrix.Platform.Fonts.Roboto.OflLicenseForever" —
                    equal to the NuGet PackageId.
  displayName       "Roboto" — the typographic family name shown to a user
                    and written into generated source.
  fontFamilyUri     "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf"
                    — the primary font. No `#` fragment.
  resourceKey       "RobotoFont" — the App.xaml resource key a generated
                    application declares and binds to.
  fallbackFontUris  Ordered list of exactly two URIs:
                      ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansArmenian.ttf
                      ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansGeorgian.ttf
                    They are consulted, in order, for codepoints the primary
                    font lacks.
  keyboardLayouts   All 38 software-keyboard layout ids (listed in QUICK
                    REFERENCE CARD), as the UNION across the primary font and
                    its companions — the companions are what add `ka` and
                    `hy`. Ids absent from the list are not supported; there is
                    deliberately no "unsupported" list, so the complement of
                    the platform's layout set is always the correct answer.

WHAT A CONSUMER MUST DO ABOUT `fallbackFontUris`: nothing. There is no extra
package to reference, no MSBuild property to set and no registration call.
The two companion fonts are files in this same package; referencing this
package deploys them, and the `.targets` prune is written specifically so
they survive on every head. The descriptor exists so CodeBrix.Develop and the
platform can read the fallback chain; it is not a consumer to-do list.

If you want a GUARANTEE that a specific block of Armenian or Georgian text
renders — rather than relying on per-codepoint fallback — set `FontFamily`
explicitly to the companion URI on that element. See COMPLETE EXAMPLES,
Example 5.


FONT INVENTORY
==============

51 `.ttf` files plus 3 `.ttf.manifest` files, in three families.

PRIMARY FAMILY — Roboto (37 files)
-----------------------------------
Variable font (always shipped, never pruned):

  Roboto.ttf
      Two variation axes, read from the font's own `fvar` table:
        wght  100..900, default 400
        wdth   75..100, default 100  (100 = Normal, 75 = Condensed)
      There is NO italic or slant axis. Italic Roboto is available only from
      the static instances below. The font declares 18 named instances
      (Thin/ExtraLight/Light/Regular/Medium/SemiBold/Bold/ExtraBold/Black,
      each Normal and Condensed).

Static instances (36 files; pruned when SupportsFontManifest is not 'true'):

  weights    Light 300, Regular 400, Medium 500,
             SemiBold 600, Bold 700, ExtraBold 800
  styles     Normal, Italic
  stretches  Normal, Condensed, SemiCondensed

  Normal stretch         Roboto-<Weight>[Italic].ttf              12 files
  Condensed stretch      Roboto_Condensed-<Weight>[Italic].ttf    12 files
  SemiCondensed stretch  Roboto_SemiCondensed-<Weight>[Italic].ttf 12 files

Naming quirk to expect: the italic of the Regular weight drops the weight
word entirely — `Roboto-Italic.ttf`, `Roboto_Condensed-Italic.ttf`,
`Roboto_SemiCondensed-Italic.ttf`. Every other weight carries its weight word
in the italic filename (`Roboto-BoldItalic.ttf`, and so on). Do not construct
`Roboto-RegularItalic.ttf`; it does not exist.

Stretch is expressed in the filename with an underscore before the stretch
word and a dash before the weight word: `Roboto_SemiCondensed-Bold.ttf`.

Upstream Roboto also ships Thin (100), ExtraLight (200) and Black (900)
STATIC instances; those are deliberately not bundled, keeping the static set
aligned with the sibling CodeBrix.Platform.Fonts packages. Those weights
remain reachable through the variable font's 100..900 axis on a head that
renders variable fonts.

COMPANION FAMILIES (14 files)
------------------------------
  NotoSansArmenian.ttf + 6 statics   supplies the ARMENIAN script
  NotoSansGeorgian.ttf + 6 statics   supplies the GEORGIAN script

  Both variable fonts declare axes wght 100..900 (default 400) and
  wdth 62.5..100 (default 100), with 9 named instances each. Neither has an
  italic or slant axis.

  Both static sets are six weights (Light 300 through ExtraBold 800), Normal
  stretch, UPRIGHT ONLY — upstream publishes no italic face for either
  family, so italic text in those scripts renders upright. That is an
  upstream limitation, not a packaging defect.

  Roboto already covers Greek natively, so there is no Greek companion here
  (unlike the sibling Merriweather package).

Manifests
---------
  Roboto.ttf.manifest            36 entries (2 styles x 6 weights x 3 stretches)
  NotoSansArmenian.ttf.manifest   6 entries (Normal style, Normal stretch)
  NotoSansGeorgian.ttf.manifest   6 entries (Normal style, Normal stretch)


SCRIPT AND CODEPOINT COVERAGE
=============================

Derived by parsing the `cmap` table of the shipped font files. Within each
family, the variable font and every static instance carry the same character
set (Roboto: 927 mapped codepoints; Noto Sans Armenian: 430; Noto Sans
Georgian: 509 — the variable font and a static of each were parsed and
agree).

Scripts covered
---------------
  Latin      complete for Western, Central and Eastern European
             orthographies, plus Vietnamese                      (Roboto)
  Greek      modern monotonic Greek                              (Roboto)
  Cyrillic   essentially the whole base block (255 of 256
             codepoints), covering Russian, Ukrainian,
             Belarusian, Bulgarian, Serbian, Macedonian          (Roboto)
  Armenian   Noto Sans Armenian companion
  Georgian   Noto Sans Georgian companion, including the
             Mkhedruli, Asomtavruli, Nuskhuri and Mtavruli forms

Roboto — Unicode blocks present (mapped codepoints / block size)
----------------------------------------------------------------
  U+0020-U+007E  Basic Latin (ASCII)                        95 / 95
  U+00A0-U+00FF  Latin-1 Supplement                         96 / 96
  U+0100-U+017F  Latin Extended-A                          128 / 128
  U+0180-U+024F  Latin Extended-B                           18 / 208
  U+0250-U+02AF  IPA Extensions                              1 / 96
  U+02B0-U+02FF  Spacing Modifier Letters                   11 / 80
  U+0300-U+036F  Combining Diacritical Marks                 6 / 112
  U+0370-U+03FF  Greek and Coptic                           75 / 144
  U+0400-U+04FF  Cyrillic                                  255 / 256
  U+0500-U+052F  Cyrillic Supplement                        20 / 48
  U+1E00-U+1EFF  Latin Extended Additional                 101 / 256
  U+1F00-U+1FFF  Greek Extended                              1 / 256
  U+2000-U+206F  General Punctuation                        38 / 112
  U+2070-U+209F  Superscripts and Subscripts                28 / 48
  U+20A0-U+20CF  Currency Symbols                           15 / 48
  U+2100-U+214F  Letterlike Symbols                          6 / 80
  U+2150-U+218F  Number Forms                                4 / 64
  U+2200-U+22FF  Mathematical Operators                     12 / 256
  U+25A0-U+25FF  Geometric Shapes                            4 / 96
  U+FB00-U+FB4F  Alphabetic Presentation Forms               4 / 80
  U+FFF0-U+FFFF  Specials                                    2 / 16

Noto Sans Armenian companion — blocks present
----------------------------------------------
  U+0530-U+058F  Armenian                                   91 / 96
  U+FB13-U+FB17  Armenian ligatures (in Alphabetic
                 Presentation Forms)                          5 / 5
  plus a Latin subset for mixed-script runs: ASCII 95/95,
  Latin-1 Supplement 84/96, Latin Extended-A 97/128,
  Latin Extended-B 5/208, Latin Extended Additional 9/256,
  Combining Diacritical Marks 15/112, General Punctuation 14/112.
  It carries NO Cyrillic and NO Greek.

Noto Sans Georgian companion — blocks present
----------------------------------------------
  U+10A0-U+10CF  Georgian (Asomtavruli capitals)             40 / 48
  U+10D0-U+10FF  Georgian (Mkhedruli)                        48 / 48
  U+2D00-U+2D2F  Georgian Supplement (Nuskhuri)              40 / 48
  U+1C90-U+1CBF  Georgian Extended (Mtavruli)                46 / 48
  plus the same Latin subset shape as the Armenian companion.
  It carries NO Cyrillic and NO Greek.

Blocks absent from the whole package — text in these scripts will not render
at all:

  U+0590-U+05FF  Hebrew          (the sibling OpenSans package has this;
                                  Roboto does not)
  U+0600-U+06FF  Arabic
  U+0900-U+097F  Devanagari
  U+0E00-U+0E7F  Thai
  U+2190-U+21FF  Arrows
  U+1F300+       emoji and pictographs
  polytonic Greek (the Greek Extended block is essentially empty)

Spot-checked present in Roboto:  U+1EA1, U+1EF9, U+01B0, U+0110
(Vietnamese), U+03B1 (Greek), U+0410 (Cyrillic), U+0452 / U+045C / U+045E
(Serbian, Macedonian, Belarusian), U+0131 (Turkish dotless i), U+0126
(Maltese H-bar), U+00FE (Icelandic thorn), U+2116 (numero), U+FB01 (fi), and
a wide currency set including U+20AC euro, U+20B9 rupee, U+20BD ruble,
U+20BA lira, U+20A9 won, U+20AB dong (15 currency symbols in all — notably
more than the sibling OpenSans package's 6).

Spot-checked ABSENT from Roboto: U+05D0 (Hebrew), U+0531 (Armenian —
companion supplies it), U+10D0 (Georgian — companion supplies it), U+1F00
(polytonic Greek), U+20B4 (hryvnia), U+2190 (arrow), U+1F600 (emoji).

Missing-glyph behaviour: the `.notdef` glyph (glyph 0) of Roboto and of both
companions is a DRAWN glyph, not an empty one. An unsupported codepoint
therefore renders as a visible box ("tofu"), which makes coverage gaps
obvious on screen. (The sibling OpenSans package behaves the opposite way —
its `.notdef` is empty, so gaps there are invisible.) Never rely on a system
font to fill a gap; CodeBrix.Platform does not fall back to system fonts.


COMPLETE EXAMPLES
=================

Example 1 — a TextBlock and a Run, selecting a static instance by property
---------------------------------------------------------------------------
`FontWeight`, `FontStyle` and `FontStretch` are what drive static-instance
selection: their values are looked up as the {font_style, font_weight,
font_stretch} triple in `Roboto.ttf.manifest`, and the matching static `.ttf`
is used. Name the family once, by URI, and vary the properties.

    <StackPanel>

        <!-- Regular 400, upright, Normal stretch: the manifest entry for
             {Normal, 400, Normal} resolves to Roboto-Regular.ttf -->
        <TextBlock Text="Hello, world."
                   FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf" />

        <!-- Bold italic: {Italic, 700, Normal} -> Roboto-BoldItalic.ttf -->
        <TextBlock Text="Bold italic sample"
                   FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf"
                   FontWeight="Bold"
                   FontStyle="Italic" />

        <!-- Condensed SemiBold: {Normal, 600, Condensed}
             -> Roboto_Condensed-SemiBold.ttf -->
        <TextBlock Text="Narrow heading"
                   FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf"
                   FontWeight="SemiBold"
                   FontStretch="Condensed" />

        <!-- Mixed runs inside one paragraph -->
        <TextBlock FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf">
            <Run Text="Light " FontWeight="Light" />
            <Run Text="Medium " FontWeight="Medium" />
            <Run Text="ExtraBold " FontWeight="ExtraBold" />
            <Run Text="SemiCondensed italic"
                 FontStyle="Italic"
                 FontStretch="SemiCondensed" />
        </TextBlock>

    </StackPanel>

Weight words map to the manifest's numeric weights as
Light=300, Normal=400, Medium=500, SemiBold=600, Bold=700, ExtraBold=800.
Only those six numeric weights exist in the manifests. Thin (100),
ExtraLight (200) and Black (900) have NO manifest entry and no static file;
they exist only as positions on the variable font's axis.

Example 2 — pin one exact static file
--------------------------------------
When you want a specific face regardless of property resolution, name the
static file directly and leave the properties alone.

    <TextBlock Text="Pinned face"
               FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto_Condensed-ExtraBoldItalic.ttf" />

This is the only form that survives on a head where the statics are NOT
pruned; on a head that prunes them (SupportsFontManifest not 'true') a pinned
static file is not deployed. Prefer Example 1's property-driven form unless
you know the head keeps statics.

Example 3 — App.xaml resource, using the descriptor's resource key
-------------------------------------------------------------------
`CODEBRIX-DEVELOP.json` names the resource key an application should use for
this font: `RobotoFont`. Declare it once in App.xaml and reference it
everywhere by `{StaticResource}`, so the font URI appears exactly once in the
whole application. The two companion URIs from `fallbackFontUris` are worth
declaring alongside it if you ever set them explicitly (Example 5).

    <Application
        x:Class="MyApp.App"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
        <Application.Resources>
            <ResourceDictionary>

                <FontFamily x:Key="RobotoFont">ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf</FontFamily>

                <FontFamily x:Key="RobotoArmenianFont">ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansArmenian.ttf</FontFamily>
                <FontFamily x:Key="RobotoGeorgianFont">ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansGeorgian.ttf</FontFamily>

            </ResourceDictionary>
        </Application.Resources>
    </Application>

    <!-- and at every use site -->
    <TextBlock Text="Hello, world."
               FontFamily="{StaticResource RobotoFont}"
               FontWeight="SemiBold" />

The key name `RobotoFont` and the three URIs above are the values taken
verbatim from the descriptor; the surrounding ResourceDictionary declaration
is ordinary XAML, and the two companion key names are your choice (the
descriptor names only `RobotoFont`).

Example 4 — make Roboto the application-wide default text font
---------------------------------------------------------------
Set this before the first UI element is created (in the application entry
point, ahead of building the host).

    global::CodeBrix.Platform.UI.FeatureConfiguration.Font.DefaultTextFontFamily =
        "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/Roboto.ttf";

No `#FamilyName` fragment. See COMMON PITFALLS TO AVOID.

Example 5 — Armenian and Georgian text
---------------------------------------
Nothing is required to make the companions available: they ship in this
package and are never pruned. For text you KNOW is Armenian or Georgian, name
the companion family directly rather than relying on per-codepoint fallback —
it is deterministic and it survives every head.

    <StackPanel>

        <!-- Armenian, SemiBold: the companion manifest entry
             {Normal, 600, Normal} -> NotoSansArmenian-SemiBold.ttf -->
        <TextBlock Text="Armenian heading"
                   FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansArmenian.ttf"
                   FontWeight="SemiBold" />

        <!-- Georgian, Regular -->
        <TextBlock Text="Georgian body text"
                   FontFamily="ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/NotoSansGeorgian.ttf" />

    </StackPanel>

Do NOT set `FontStyle="Italic"` or a `FontStretch` other than `Normal` on
these: the companion manifests have six upright Normal-stretch entries each
and nothing else.


MINIMUM VIABLE PROJECT
======================

There is no code to write: adding the PackageReference is the whole
integration. A minimal consuming project file:

    <Project Sdk="Microsoft.NET.Sdk">

      <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
      </PropertyGroup>

      <ItemGroup>
        <PackageReference Include="CodeBrix.Platform.Fonts.Roboto.OflLicenseForever"
                          Version="..." />
      </ItemGroup>

    </Project>

Pin whatever version you resolved; this file never states versions because
they go stale. With that reference in place:

  * the 51 `.ttf` files and the three `.ttf.manifest` files are contributed to
    the application's asset set,
  * the buildTransitive `.targets` file is auto-imported and prunes the
    static fonts on heads that do not support the manifest,
  * every `ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/...` URI in your
    XAML resolves, companions included.

Then either set `DefaultTextFontFamily` (Example 4) or declare the
`RobotoFont` resource (Example 3) and use `{StaticResource RobotoFont}`.

Do not add any `<Content>`, `<None>` or `<EmbeddedResource>` item for the
fonts yourself, and do not copy the `.ttf` files into your project. The
package contributes them; a hand-rolled copy will duplicate assets and can
shadow the manifest lookup.


PERFORMANCE TIPS
================

  * Prefer ONE family URI plus `FontWeight`/`FontStyle`/`FontStretch` over a
    different file URI per face. Every distinct font URI is a separate font
    resource to load and cache; the property-driven form lets the platform
    resolve faces out of one already-loaded family.

  * Declare the family URI once as an App.xaml `FontFamily` resource
    (Example 3) and bind with `{StaticResource}`. Repeating the literal URI
    at dozens of use sites creates repeated string-to-FontFamily conversions
    and makes a later font swap a find-and-replace.

  * Setting `DefaultTextFontFamily` is cheaper than setting `FontFamily` on
    every element, and it lets the startup font-manifest preload warm the
    manifest once. Keeping the URI fragment-free is what makes that preload
    fire at all.

  * Let the head prune. On a head where `SupportsFontManifest` is not
    `'true'`, the 48 static files are removed from the output — that is a
    deliberate size win of roughly the whole static set. Do not defeat it by
    hard-referencing static files (Example 2) in code that also has to run on
    those heads.

  * Naming a companion family directly on Armenian/Georgian text (Example 5)
    avoids per-codepoint fallback work at layout time, on top of being more
    predictable.

  * The variable font covers weights 100-900 continuously, so on a
    manifest-capable head you can request Thin, ExtraLight, Black and every
    intermediate weight without shipping more files.


COMMON PITFALLS TO AVOID
========================

  * NEVER append `#FamilyName` to a font URI. CodeBrix.Platform strips the
    fragment during resolution, so it never helps — and on the value assigned
    to `FeatureConfiguration.Font.DefaultTextFontFamily` it silently disables
    the startup manifest preload, because the ".manifest" suffix the preload
    appends lands inside the fragment and is dropped by `Uri.PathAndQuery`.
    Older samples used a `#Roboto` form; that was inherited convention, not a
    fix for anything. The symptom is subtle: text still renders, but
    weight/style/stretch requests stop resolving to the right static instance.

  * There is no italic axis in any of the three variable fonts. On a head
    where the statics are pruned (`SupportsFontManifest` not `'true'`),
    `FontStyle="Italic"` has no italic face to resolve to and the platform
    will synthesise or ignore it. If real italics matter on such a head, this
    package cannot supply them.

  * The companions have NO italic and NO condensed faces at all, on any head.
    Italic Armenian and Georgian text renders upright. Six upright weights in
    Normal stretch is the entire companion surface.

  * `FontWeight="Thin"`, `"ExtraLight"` and `"Black"` have no static file and
    no manifest entry. They work only where the variable font is rendered
    with its axis applied; on a static-manifest head they fall back to the
    platform's nearest-match rule, not to a matching face.

  * `Roboto-RegularItalic.ttf` does not exist. The Regular italic is
    `Roboto-Italic.ttf` (and `Roboto_Condensed-Italic.ttf`,
    `Roboto_SemiCondensed-Italic.ttf`). Constructing filenames by
    concatenating weight + "Italic" breaks for exactly this one weight.

  * Stretch and weight use different separators in filenames: an underscore
    before the stretch, a dash before the weight — `Roboto_SemiCondensed-Bold.ttf`.
    `Roboto-SemiCondensed-Bold.ttf` and `Roboto_SemiCondensed_Bold.ttf` are
    both wrong. The companions use only the dash form
    (`NotoSansGeorgian-Bold.ttf`), because they have no stretch variants.

  * Each manifest is a JSON OBJECT with a `fonts` array, not a bare JSON
    array. Code that does `JsonDocument.Parse(json).RootElement.EnumerateArray()`
    on one throws; you must read the `fonts` property first. And there are
    THREE manifests here, one per family — code written against the sibling
    OpenSans package, which has one, will miss two.

  * `family_name` in a manifest entry holds a URI, not a typographic family
    name. Do not feed it to an API that expects "Roboto".

  * Hebrew is NOT covered by this package. Roboto has no Hebrew block and
    neither companion supplies one. The sibling
    `CodeBrix.Platform.Fonts.OpenSans.ApacheLicenseForever` does carry Hebrew.

  * `ms-appx:///` URIs are resolved by the CodeBrix.Platform runtime, not by
    .NET. In a plain .NET 10 console or test application that merely
    references this package, those URIs resolve to nothing. Such an
    application can still read the `.ttf` files out of the restored package
    folder under the NuGet cache — the package id is lower-cased in that
    path, and the fonts sit under
    lib/net10.0/CodeBrix.Platform.Fonts.Roboto/Fonts/ — but it must perform
    that lookup itself.

  * Do not alter the font bytes. Roboto's copyright statement declares no
    Reserved Font Name, so SIL OFL 1.1 condition 3 does not restrict the
    display name — but the binaries are redistributed unmodified and must
    stay that way. The same is true of both Noto Sans companions.


WHAT THIS PACKAGE DOES NOT DO
=============================

  * It exposes no managed API — no types, no methods, no font loader, no
    stream accessors. There is nothing to `using`.
  * It does not resolve `ms-appx:///` URIs. That is CodeBrix.Platform's job.
  * It does not register fonts with the operating system, and it does not
    install anything outside the application's own asset set.
  * It ships no Hebrew, Arabic, Devanagari, Thai, CJK or emoji glyphs, and no
    polytonic Greek or arrow glyphs.
  * It ships no italic or condensed companion faces — Armenian and Georgian
    are upright, Normal-stretch, six weights, full stop.
  * It ships no italic variable font for any family, and no Thin, ExtraLight
    or Black STATIC instances of Roboto.
  * It ships no monospace face; that is the sibling
    `CodeBrix.Platform.Fonts.RobotoMono.OflLicenseForever`.
  * It ships no `.otf`, `.woff` or `.woff2` files; TrueType `.ttf` only.
  * It does not set `SupportsFontManifest`; it only reads it.
  * It does not make itself the default font. You must set
    `DefaultTextFontFamily` or a `FontFamily` yourself.
  * It does not fall back to a system font for missing glyphs. Missing
    codepoints render as tofu boxes.


WORKING EXAMPLES ON GITHUB
==========================

The test project is the executable specification for everything above — every
claim about counts, URIs, manifest shape, descriptor fields and `.targets`
behaviour is asserted there.

  https://github.com/ellisnet/CodeBrix.Platform.Fonts.Roboto/tree/main/tests/CodeBrix.Platform.Fonts.Roboto.Tests

  ContentManifestTests.cs
      How to read a manifest correctly: parse the document, take the `fonts`
      property, enumerate the array, and project each entry's font_style /
      font_weight / font_stretch / family_name. Copy this reader if you need
      to consume a manifest yourself. Also asserts the 36/6/6 entry counts,
      the six weights, and that the two companion manifests are upright-only.
  DescriptorTests.cs
      The CODEBRIX-DEVELOP.json contract as a consumer sees it: schemaVersion
      1, packageId equal to the PackageId, displayName "Roboto", resourceKey
      "RobotoFont", `fontFamilyUri` and every `fallbackFontUri` fragment-free
      and pointing at a font this package actually ships, the fallback list
      being exactly the two companion families, and `keyboardLayouts`
      containing `ka`, `hy` and `el` with no duplicates.
  ContentFilePresenceTests.cs
      The 51-file inventory and the exact static filename grammar.
  TargetsFileTests.cs
      The `.targets` contract: target name, the `_CodeBrixAddLibraryAssets`
      hook, the `SupportsFontManifest` condition, and the assertion that
      none of the three variable fonts can ever be removed.
  AssemblyMetadataTests.cs
      That the assembly is named `CodeBrix.Platform.Fonts.Roboto`, targets
      .NET 10, and exports no public types.

  https://github.com/ellisnet/CodeBrix.Platform.Fonts.Roboto/blob/main/README.md
      Short human-facing samples of the same XAML and C# usage.


QUICK REFERENCE CARD
====================

PackageId   CodeBrix.Platform.Fonts.Roboto.OflLicenseForever
Assembly    CodeBrix.Platform.Fonts.Roboto
License     OFL-1.1
TFM         net10.0 or later
URI root    ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/
Resource    RobotoFont              Display name: Roboto
Descriptor  CODEBRIX-DEVELOP.json (nupkg root)
Targets     CodeBrixRemoveUnusedRoboto, AfterTargets=_CodeBrixAddLibraryAssets
Prune rule  static fonts removed when SupportsFontManifest != 'true'
Fallbacks   NotoSansArmenian.ttf then NotoSansGeorgian.ttf — in this package,
            nothing for a consumer to install or wire up

Weight words -> manifest font_weight
------------------------------------
  Light 300   Normal 400   Medium 500   SemiBold 600   Bold 700   ExtraBold 800
  (Thin 100, ExtraLight 200 and Black 900 exist only on the variable axis)

FontStretch values -> filename infix (Roboto only; companions have none)
-------------------------------------------------------------------------
  Normal          (none)                 Roboto-Bold.ttf
  Condensed       _Condensed             Roboto_Condensed-Bold.ttf
  SemiCondensed   _SemiCondensed         Roboto_SemiCondensed-Bold.ttf

All 51 URIs (prefix every one with the URI root above)
-------------------------------------------------------
  Variable fonts (never pruned)
    Roboto.ttf                                wght 100-900, wdth 75-100
    NotoSansArmenian.ttf                      wght 100-900, wdth 62.5-100
    NotoSansGeorgian.ttf                      wght 100-900, wdth 62.5-100

  Roboto, Normal stretch            style   weight
    Roboto-Light.ttf                  Normal   300
    Roboto-LightItalic.ttf            Italic   300
    Roboto-Regular.ttf                Normal   400
    Roboto-Italic.ttf                 Italic   400
    Roboto-Medium.ttf                 Normal   500
    Roboto-MediumItalic.ttf           Italic   500
    Roboto-SemiBold.ttf               Normal   600
    Roboto-SemiBoldItalic.ttf         Italic   600
    Roboto-Bold.ttf                   Normal   700
    Roboto-BoldItalic.ttf             Italic   700
    Roboto-ExtraBold.ttf              Normal   800
    Roboto-ExtraBoldItalic.ttf        Italic   800

  Roboto, Condensed stretch
    Roboto_Condensed-Light.ttf               Normal   300
    Roboto_Condensed-LightItalic.ttf         Italic   300
    Roboto_Condensed-Regular.ttf             Normal   400
    Roboto_Condensed-Italic.ttf              Italic   400
    Roboto_Condensed-Medium.ttf              Normal   500
    Roboto_Condensed-MediumItalic.ttf        Italic   500
    Roboto_Condensed-SemiBold.ttf            Normal   600
    Roboto_Condensed-SemiBoldItalic.ttf      Italic   600
    Roboto_Condensed-Bold.ttf                Normal   700
    Roboto_Condensed-BoldItalic.ttf          Italic   700
    Roboto_Condensed-ExtraBold.ttf           Normal   800
    Roboto_Condensed-ExtraBoldItalic.ttf     Italic   800

  Roboto, SemiCondensed stretch
    Roboto_SemiCondensed-Light.ttf              Normal   300
    Roboto_SemiCondensed-LightItalic.ttf        Italic   300
    Roboto_SemiCondensed-Regular.ttf            Normal   400
    Roboto_SemiCondensed-Italic.ttf             Italic   400
    Roboto_SemiCondensed-Medium.ttf             Normal   500
    Roboto_SemiCondensed-MediumItalic.ttf       Italic   500
    Roboto_SemiCondensed-SemiBold.ttf           Normal   600
    Roboto_SemiCondensed-SemiBoldItalic.ttf     Italic   600
    Roboto_SemiCondensed-Bold.ttf               Normal   700
    Roboto_SemiCondensed-BoldItalic.ttf         Italic   700
    Roboto_SemiCondensed-ExtraBold.ttf          Normal   800
    Roboto_SemiCondensed-ExtraBoldItalic.ttf    Italic   800

  Noto Sans Armenian companion (Normal stretch, upright only)
    NotoSansArmenian-Light.ttf               Normal   300
    NotoSansArmenian-Regular.ttf             Normal   400
    NotoSansArmenian-Medium.ttf              Normal   500
    NotoSansArmenian-SemiBold.ttf            Normal   600
    NotoSansArmenian-Bold.ttf                Normal   700
    NotoSansArmenian-ExtraBold.ttf           Normal   800

  Noto Sans Georgian companion (Normal stretch, upright only)
    NotoSansGeorgian-Light.ttf               Normal   300
    NotoSansGeorgian-Regular.ttf             Normal   400
    NotoSansGeorgian-Medium.ttf              Normal   500
    NotoSansGeorgian-SemiBold.ttf            Normal   600
    NotoSansGeorgian-Bold.ttf                Normal   700
    NotoSansGeorgian-ExtraBold.ttf           Normal   800

  Manifests (objects with a `fonts` array)
    Roboto.ttf.manifest                      36 entries
    NotoSansArmenian.ttf.manifest             6 entries
    NotoSansGeorgian.ttf.manifest             6 entries

Software-keyboard layouts declared supported (38 — all of them)
----------------------------------------------------------------
  en, en-GB, de, de-CH, fr, fr-BE, fr-CH, nl, es, pt, it, mt, sq, tr, el,
  da, no, sv, fi, is, lt, lv, et, pl, cs, sk, hu, ro, hr, sr-Latn, ru, uk,
  be, bg, sr, mk, ka, hy

  `ka` and `hy` come from the two companion families, not from Roboto.

Missing glyph renders as: a tofu box (drawn .notdef), never a blank gap.
