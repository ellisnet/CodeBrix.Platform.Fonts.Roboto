using System.IO;
using System.Linq;
using System.Text.Json;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Fonts.Roboto.Tests;

/// <summary>
/// Guards CODEBRIX-DEVELOP.json — the file CodeBrix.Develop reads to learn how to
/// wire this font into a generated application. Every claim it makes about a file
/// is checked against what the package actually ships, so the descriptor cannot
/// drift from the font set without a test failing.
/// </summary>
public class DescriptorTests
{
    private const string PackageId = "CodeBrix.Platform.Fonts.Roboto.OflLicenseForever";
    private const string PathPrefix = "ms-appx:///CodeBrix.Platform.Fonts.Roboto/Fonts/";

    [Fact]
    public void Descriptor_is_present()
        => File.Exists(TestAssetPaths.DescriptorPath).Should().BeTrue();

    [Fact]
    public void Descriptor_declares_schema_version_one()
        => Root().GetProperty("schemaVersion").GetInt32().Should().Be(1);

    [Fact]
    public void Descriptor_package_id_matches_the_published_package()
        => Root().GetProperty("packageId").GetString().Should().Be(PackageId);

    [Fact]
    public void Descriptor_display_name_is_the_typographic_family_name()
        => Root().GetProperty("displayName").GetString().Should().Be("Roboto");

    [Fact]
    public void Descriptor_resource_key_follows_the_family_convention()
        => Root().GetProperty("resourceKey").GetString().Should().Be("RobotoFont");

    [Fact]
    public void Font_family_uri_carries_no_family_fragment()
    {
        //Arrange — a "#Family" fragment breaks the startup manifest preload in
        //CodeBrix.Platform (the ".manifest" suffix lands inside the fragment and
        //is dropped), and buys nothing: font resolution strips it anyway.
        var uri = Root().GetProperty("fontFamilyUri").GetString();

        //Assert
        uri.Should().NotContain("#");
    }

    [Fact]
    public void Font_family_uri_points_at_a_font_this_package_ships()
    {
        //Arrange
        var uri = Root().GetProperty("fontFamilyUri").GetString()!;

        //Assert
        uri.Should().StartWith(PathPrefix);
        File.Exists(Path.Combine(TestAssetPaths.FontsFolder, Path.GetFileName(uri))).Should().BeTrue();
    }

    [Fact]
    public void Every_fallback_font_uri_points_at_a_font_this_package_ships()
    {
        //Arrange
        var missing = FallbackUris()
            .Where(uri => !File.Exists(Path.Combine(TestAssetPaths.FontsFolder, Path.GetFileName(uri))))
            .ToList();

        //Assert
        missing.Should().BeEmpty();
    }

    [Fact]
    public void Fallback_font_uris_carry_no_family_fragment()
        => FallbackUris().Where(uri => uri.Contains('#')).Should().BeEmpty();

    [Fact]
    public void Fallback_fonts_are_the_three_companion_families()
    {
        //Arrange — polytonic Greek, Armenian and Georgian are exactly the
        //scripts Roboto itself does not carry. Roboto covers only MONOTONIC
        //Greek, so Noto Sans is here for the Greek Extended block — the same
        //role Noto Serif plays in the sibling Merriweather package.
        var names = FallbackUris().Select(Path.GetFileNameWithoutExtension).OrderBy(n => n).ToArray();

        //Assert
        names.Should().BeEquivalentTo(new[] { "NotoSans", "NotoSansArmenian", "NotoSansGeorgian" });
    }

    [Fact]
    public void NotoSans_is_the_first_fallback()
    {
        //Arrange — Noto Sans is the widest companion, and it carries no
        //Armenian and only one Georgian code point, so it can lead the chain
        //without shadowing the two script-specific companions. Order is part of
        //the descriptor contract: the platform consults the list in order.
        var names = FallbackUris().Select(Path.GetFileNameWithoutExtension).ToArray();

        //Assert
        names[0].Should().Be("NotoSans");
    }

    [Fact]
    public void Keyboard_layouts_have_no_duplicates()
    {
        //Arrange
        var layouts = KeyboardLayouts();

        //Assert
        layouts.Distinct().Count().Should().Be(layouts.Length);
    }

    [Fact]
    public void Keyboard_layouts_include_the_scripts_the_companions_supply()
    {
        //Arrange — the companions exist precisely to add these two, so a
        //descriptor that ships them without claiming them is a packaging slip.
        var layouts = KeyboardLayouts();

        //Assert
        layouts.Should().Contain("ka");
        layouts.Should().Contain("hy");
    }

    [Fact]
    public void Keyboard_layouts_include_greek_from_roboto_itself()
    {
        //Arrange — Roboto carries MONOTONIC Greek natively, which is what the
        //"el" layout needs; this is not companion-supplied. (Polytonic Greek
        //comes from the Noto Sans companion and has no layout id of its own.)
        var layouts = KeyboardLayouts();

        //Assert
        layouts.Should().Contain("el");
    }

    private static JsonElement Root()
        => JsonDocument.Parse(File.ReadAllText(TestAssetPaths.DescriptorPath)).RootElement;

    private static string[] FallbackUris()
        => Root().GetProperty("fallbackFontUris").EnumerateArray()
            .Select(e => e.GetString() ?? string.Empty).ToArray();

    private static string[] KeyboardLayouts()
        => Root().GetProperty("keyboardLayouts").EnumerateArray()
            .Select(e => e.GetString() ?? string.Empty).ToArray();
}
