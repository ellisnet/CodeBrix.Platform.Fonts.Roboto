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
    public void Total_ttf_count_is_64()
    {
        //Arrange/Act
        // 1 Roboto variable + 36 Roboto statics, then the three companion
        // families: Noto Sans (1 + 12), Noto Sans Armenian (1 + 6) and
        // Noto Sans Georgian (1 + 6).
        var ttfFiles = Directory.GetFiles(TestAssetPaths.FontsFolder, "*.ttf");

        //Assert
        ttfFiles.Length.Should().Be(64);
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

    [Theory]
    [InlineData("NotoSans")]
    [InlineData("NotoSansArmenian")]
    [InlineData("NotoSansGeorgian")]
    public void Companion_variable_font_is_present(string family)
        => File.Exists(TestAssetPaths.CompanionFontPath(family)).Should().BeTrue();

    [Theory]
    [InlineData("NotoSans")]
    [InlineData("NotoSansArmenian")]
    [InlineData("NotoSansGeorgian")]
    public void Companion_manifest_is_present(string family)
        => File.Exists(TestAssetPaths.CompanionManifestPath(family)).Should().BeTrue();

    [Fact]
    public void All_12_static_NotoSans_fonts_are_present()
    {
        //Arrange — Noto Sans supplies polytonic Greek, and unlike the Armenian
        //and Georgian companions it ships upright AND italic upstream.
        var weights = new[] { "Light", "Regular", "Medium", "SemiBold", "Bold", "ExtraBold" };
        var styles = new[] { "", "Italic" };

        //Act
        var missing = (
            from weight in weights
            from style in styles
            let weightSegment = (weight == "Regular" && style == "Italic") ? "" : weight
            let fileName = $"NotoSans-{weightSegment}{style}.ttf"
            where !File.Exists(Path.Combine(TestAssetPaths.FontsFolder, fileName))
            select fileName
        ).ToList();

        //Assert
        missing.Should().BeEmpty();
    }

    [Theory]
    [InlineData("NotoSansArmenian")]
    [InlineData("NotoSansGeorgian")]
    public void All_6_static_fonts_are_present_for(string family)
    {
        //Arrange — neither family has an italic face upstream, so only the six
        //upright weights ship.
        var weights = new[] { "Light", "Regular", "Medium", "SemiBold", "Bold", "ExtraBold" };

        //Act
        var missing = weights
            .Select(weight => $"{family}-{weight}.ttf")
            .Where(fileName => !File.Exists(Path.Combine(TestAssetPaths.FontsFolder, fileName)))
            .ToList();

        //Assert
        missing.Should().BeEmpty();
    }

    [Fact]
    public void No_NotoSans_static_collides_with_a_companion_family_name()
    {
        //Arrange — "NotoSans-*" is a prefix of "NotoSansArmenian"/"NotoSansGeorgian"
        //only if a dash is forgotten, so this pins that the three families stay
        //distinct on disk and that the manifest-per-family discovery used by
        //consumers (e.g. Html2Pdf) cannot mis-group them.
        var notoSansStatics = Directory.GetFiles(TestAssetPaths.FontsFolder, "NotoSans-*.ttf")
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        //Assert
        notoSansStatics.Count.Should().Be(12);
        notoSansStatics.Where(n => n!.Contains("Armenian") || n.Contains("Georgian")).Should().BeEmpty();
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
