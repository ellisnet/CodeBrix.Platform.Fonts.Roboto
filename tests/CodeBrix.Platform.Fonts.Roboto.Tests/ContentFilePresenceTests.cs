using System.IO;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Fonts.Roboto.Tests;

public class ContentFilePresenceTests
{
    [Fact]
    public void Variable_font_Roboto_ttf_is_present()
        => File.Exists(TestAssetPaths.VariableFontPath).Should().BeTrue();

    [Fact]
    public void Manifest_file_is_present()
        => File.Exists(TestAssetPaths.ManifestPath).Should().BeTrue();

    [Fact]
    public void Total_ttf_count_is_37()
    {
        //Arrange/Act
        var ttfFiles = Directory.GetFiles(TestAssetPaths.FontsFolder, "*.ttf");

        //Assert
        ttfFiles.Length.Should().Be(37);
    }

    [Fact]
    public void All_36_static_fonts_are_present()
    {
        //Arrange
        // Note the Roboto naming quirk (shared with the static font naming
        // convention used across these packages): the italic of the Regular
        // weight is just "Italic" (no "Regular" prefix), e.g.
        // Roboto-Italic.ttf, Roboto_Condensed-Italic.ttf. Every other weight
        // carries its weight name in the italic filename.
        var weights = new[] { "Light", "Regular", "Medium", "SemiBold", "Bold", "ExtraBold" };
        var styles = new[] { "", "Italic" };
        var stretches = new[] { "", "_Condensed", "_SemiCondensed" };

        //Act
        var missing = (
            from weight in weights
            from style in styles
            from stretch in stretches
            let weightSegment = (weight == "Regular" && style == "Italic") ? "" : weight
            let fileName = $"Roboto{stretch}-{weightSegment}{style}.ttf"
            let path = Path.Combine(TestAssetPaths.FontsFolder, fileName)
            where !File.Exists(path)
            select fileName
        ).ToList();

        //Assert
        missing.Should().BeEmpty();
    }

    [Fact]
    public void Uprimarker_file_is_present()
        => File.Exists(TestAssetPaths.UprimarkerPath).Should().BeTrue();

    [Fact]
    public void Uprimarker_file_is_empty()
    {
        //Arrange
        var info = new FileInfo(TestAssetPaths.UprimarkerPath);

        //Assert
        info.Length.Should().Be(0L);
    }

    [Fact]
    public void Variable_font_is_non_trivial_size()
    {
        //Arrange
        var info = new FileInfo(TestAssetPaths.VariableFontPath);

        //Assert
        info.Length.Should().BeGreaterThan(100_000L);
    }
}
