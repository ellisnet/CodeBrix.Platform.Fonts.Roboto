using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Fonts.Roboto.Tests;

public class ContentManifestTests
{
    private const string CodeBrixPathPrefix = "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/";

    // This package was authored by mirroring the sibling OpenSans package, so
    // the realistic copy-paste regression is a stray "OpenSans" token, not an
    // upstream one.
    private const string ForeignFamilyToken = "OpenSans";

    [Fact]
    public void Manifest_file_exists_in_test_output()
        => File.Exists(TestAssetPaths.ManifestPath).Should().BeTrue();

    [Fact]
    public void Manifest_can_be_deserialized()
    {
        //Arrange
        var json = File.ReadAllText(TestAssetPaths.ManifestPath);

        //Act
        var doc = JsonDocument.Parse(json);

        //Assert
        doc.RootElement.TryGetProperty("fonts", out var fonts).Should().BeTrue();
        fonts.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public void Manifest_has_exactly_36_entries()
    {
        //Arrange
        var entries = ReadManifestEntries();

        //Act/Assert
        entries.Count.Should().Be(36);
    }

    [Fact]
    public void Manifest_every_family_name_uses_codebrix_namespace()
    {
        //Arrange
        var entries = ReadManifestEntries();

        //Act
        var nonMatching = entries
            .Where(e => !e.FamilyName.StartsWith(CodeBrixPathPrefix))
            .ToList();

        //Assert
        nonMatching.Should().BeEmpty();
    }

    [Fact]
    public void Manifest_contains_no_foreign_family_tokens()
    {
        //Arrange
        var json = File.ReadAllText(TestAssetPaths.ManifestPath);

        //Act/Assert
        json.Contains(ForeignFamilyToken).Should().BeFalse();
    }

    [Fact]
    public void Manifest_every_referenced_font_file_exists_on_disk()
    {
        //Arrange
        var entries = ReadManifestEntries();

        //Act
        var missing = entries
            .Select(e => Path.GetFileName(e.FamilyName))
            .Select(name => Path.Combine(TestAssetPaths.FontsFolder, name))
            .Where(path => !File.Exists(path))
            .ToList();

        //Assert
        missing.Should().BeEmpty();
    }

    [Fact]
    public void Manifest_covers_all_six_weights()
    {
        //Arrange
        var entries = ReadManifestEntries();
        var expectedWeights = new[] { 300, 400, 500, 600, 700, 800 };

        //Act
        var distinctWeights = entries.Select(e => e.FontWeight).Distinct().OrderBy(w => w).ToArray();

        //Assert
        distinctWeights.Should().BeEquivalentTo(expectedWeights);
    }

    [Fact]
    public void Manifest_covers_normal_and_italic_styles()
    {
        //Arrange
        var entries = ReadManifestEntries();

        //Act
        var distinctStyles = entries.Select(e => e.FontStyle).Distinct().OrderBy(s => s).ToArray();

        //Assert
        distinctStyles.Should().BeEquivalentTo(new[] { "Italic", "Normal" });
    }

    [Fact]
    public void Manifest_covers_normal_condensed_and_semicondensed_stretches()
    {
        //Arrange
        var entries = ReadManifestEntries();

        //Act
        var distinctStretches = entries.Select(e => e.FontStretch).Distinct().OrderBy(s => s).ToArray();

        //Assert
        distinctStretches.Should().BeEquivalentTo(new[] { "Condensed", "Normal", "SemiCondensed" });
    }

    private static List<ManifestEntry> ReadManifestEntries()
    {
        var json = File.ReadAllText(TestAssetPaths.ManifestPath);
        using var doc = JsonDocument.Parse(json);
        var fonts = doc.RootElement.GetProperty("fonts");

        var list = new List<ManifestEntry>(fonts.GetArrayLength());
        foreach (var entry in fonts.EnumerateArray())
        {
            list.Add(new ManifestEntry(
                entry.GetProperty("font_style").GetString() ?? string.Empty,
                entry.GetProperty("font_weight").GetInt32(),
                entry.GetProperty("font_stretch").GetString() ?? string.Empty,
                entry.GetProperty("family_name").GetString() ?? string.Empty));
        }
        return list;
    }

    private readonly record struct ManifestEntry(
        string FontStyle,
        int FontWeight,
        string FontStretch,
        string FamilyName);
}
