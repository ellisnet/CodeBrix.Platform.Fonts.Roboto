================================================================================
EXTRAS-README: CodeBrix.Platform.Fonts.Roboto
Samples, tools and other content in this repository that is not part of a
NuGet package
================================================================================

This repository contains NO samples, demo applications, tools, scripts or
optional test-data sets.

Everything in the repository is either packaged content, packaging metadata,
documentation, or the test project.


TESTS — the only non-package content
====================================

  tests/CodeBrix.Platform.Fonts.Roboto.Tests/

The test project is not shipped in the NuGet package. It is an asset-, JSON-
and metadata-inspection suite: it links the packaged font files, the three
manifests, the `.uprimarker`, the CODEBRIX-DEVELOP.json descriptor and the
buildTransitive `.targets` file into its own output under `TestAssets/`, then
asserts the file inventory, the manifest shapes, the descriptor contract and
the `.targets` contract.

Run it with:

    dotnet test CodeBrix.Platform.Fonts.Roboto.slnx

There are no opt-in environment variables and no special preparation; the
suite needs no network and no display.

It doubles as the worked example of how to read a `.ttf.manifest` correctly
and of what CODEBRIX-DEVELOP.json guarantees — see ContentManifestTests.cs
and DescriptorTests.cs, and the WORKING EXAMPLES ON GITHUB section of
AGENT-README.txt. Details of what each test class pins are in
MAINTAINER-README.txt.


WHAT YOU WILL NOT FIND HERE
===========================

  * No sample application. To see the fonts in use, reference the NuGet
    package from a CodeBrix.Platform application and follow the COMPLETE
    EXAMPLES section of AGENT-README.txt — including Example 5, which shows
    the Armenian and Georgian companion families.
  * No font-generation, subsetting or conversion tool. The `.ttf` files are
    checked in exactly as received from upstream, apart from the three
    variable-font filename renames recorded in THIRD-PARTY-NOTICES.txt.
  * No generator for the `keyboardLayouts` array in CODEBRIX-DEVELOP.json.
    That array is produced by a developer-run tool that lives outside this
    repository and is checked in here as data.
  * No benchmark project and no optional test-data downloads.
