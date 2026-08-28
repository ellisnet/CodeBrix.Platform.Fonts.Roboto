using System;
using System.IO;

namespace CodeBrix.Platform.Fonts.Roboto.Tests;

internal static class TestAssetPaths
{
    public static string TestAssetsRoot { get; } =
        Path.Combine(AppContext.BaseDirectory, "TestAssets");

    public static string FontsFolder { get; } =
        Path.Combine(TestAssetsRoot, "Fonts");

    public static string ManifestPath { get; } =
        Path.Combine(FontsFolder, "Roboto.ttf.manifest");

    public static string VariableFontPath { get; } =
        Path.Combine(FontsFolder, "Roboto.ttf");

    public static string UprimarkerPath { get; } =
        Path.Combine(TestAssetsRoot, "CodeBrix.Platform.Fonts.Roboto.uprimarker");

    public static string TargetsFilePath { get; } =
        Path.Combine(TestAssetsRoot, "buildTransitive", "net10.0", "CodeBrix.Platform.Fonts.Roboto.OflLicenseForever.targets");

    public static string DescriptorPath { get; } =
        Path.Combine(TestAssetsRoot, "CODEBRIX-DEVELOP.json");

    /// <summary>
    /// The companion families that supply the scripts Roboto itself does not
    /// carry: polytonic (ancient) Greek, Armenian and Georgian. Each ships a
    /// variable font plus its own manifest. Roboto covers only MONOTONIC
    /// Greek, so Noto Sans is here for the Greek Extended block — the same
    /// role Noto Serif plays in the sibling Merriweather package.
    /// </summary>
    public static string[] CompanionFamilies { get; } =
        ["NotoSans", "NotoSansArmenian", "NotoSansGeorgian"];

    /// <summary>
    /// The companion families that ship upright faces only, because upstream
    /// publishes no italic for them. Noto Sans is deliberately absent: it does
    /// have a full italic set.
    /// </summary>
    public static string[] UprightOnlyCompanionFamilies { get; } =
        ["NotoSansArmenian", "NotoSansGeorgian"];

    public static string CompanionFontPath(string family) =>
        Path.Combine(FontsFolder, family + ".ttf");

    public static string CompanionManifestPath(string family) =>
        Path.Combine(FontsFolder, family + ".ttf.manifest");
}
