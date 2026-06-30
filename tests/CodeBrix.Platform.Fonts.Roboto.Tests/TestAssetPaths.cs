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
}
