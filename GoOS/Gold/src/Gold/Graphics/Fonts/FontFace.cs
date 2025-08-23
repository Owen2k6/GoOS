/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */

namespace Gold.Graphics.Fonts;

/// <summary>
/// A font face.
/// </summary>
public abstract class FontFace
{
    /// <summary>
    /// Get the family name of the font face.
    /// </summary>
    /// <returns>The family name of the font face.</returns>
    public abstract string GetFamilyName();

    /// <summary>
    /// Get the style name of the font face, for example Regular or Bold.
    /// </summary>
    /// <returns>The style name of the font face.</returns>
    public abstract string GetStyleName();

    /// <summary>
    /// Get the line height of the font face in pixels.
    /// </summary>
    /// <returns>The line height of the font face in pixels.</returns>
    public abstract int GetHeight();

    /// <summary>
    /// Get the glyph for a character.
    /// </summary>
    /// <param name="c">The character to get the glyph for.</param>
    /// <returns>The glyph, if any. Note that the glyph may not have a bitmap in the case of whitespace characters.</returns>
    public abstract Glyph? GetGlyph(char c);

    /// <summary>
    /// Measures a string.
    /// </summary>
    /// <param name="s">The string to measure.</param>
    /// <returns>The width of the measured string.</returns>
    public abstract ushort MeasureString(string s);

    /// <summary>
    /// Gets the spacing modifier for BitFont fonts.
    /// </summary>
    /// <returns>The spacing modifier for the font</returns>
    public abstract int SpacingModifier();
}