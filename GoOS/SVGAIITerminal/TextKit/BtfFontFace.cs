/*
*  This code is licensed under the ekzFreeUse license.
*  If a license wasn't included with the program,
*  refer to https://github.com/9xbt/SVGAIITerminal/blob/main/LICENSE.md
*/

using Gold.Graphics.Fonts;
using System;
using System.IO;

namespace SVGAIITerminal.TextKit;

/// <summary>
/// A bitfont format font face.
/// </summary>
public class BtfFontFace : FontFace
{
    /// <summary>
    /// Initializes a new BTF (BitFont Font Format) font face.
    /// </summary>
    /// <param name="binary">The font data.</param>
    /// <param name="size">The size (height) of the font.</param>
    public BtfFontFace(byte[] binary, ushort size)
    {
        _binary = binary;

        ParseHeight(size);
        ParseGlyphs();
    }

    /// <summary>
    /// Parse the line height of the ACF font face.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown when the line height is zero.</exception>
    private void ParseHeight(ushort size)
    {
        _size = size;
        _size8 = (ushort)(size / 8);
        if (_size == 0)
            throw new InvalidDataException("Invalid font height!");
    }

    /// <summary>
    /// Parses the glyphs in the BTF font face from the stream.
    /// </summary>
    private void ParseGlyphs()
    {
        for (char c = (char)32; c < (char)128; c++)
            _glyphs[c - 32] = ParseGlyph(c);
    }

    /// <summary>
    /// Parses a single glyph in the BTF font face from the stream.
    /// </summary>
    /// <param name="c">Character to parse.</param>
    private Glyph ParseGlyph(char c)
    {
        // Create new empty glyph.
        Gold.Graphics.Fonts.Glyph Temp = new(0, _size);

        // Get the index of the character in the font.
        int Index = Gold.Graphics.Fonts.Font.DefaultCharset.IndexOf(c);

        // Check if there is a glyph for the given character.
        if (Index < 0)
        {
            // Return an empty glyph.
            return new(0, 0, _size / 2, _size, Temp.Points);
        }

        ushort SizePerFont = (ushort)(_size * _size8 * Index);

        for (int i = 0; i < _size * _size8; i++)
        {
            int X = i % _size8;
            int Y = i / _size8;

            for (int ww = 0; ww < 8; ww++)
            {
                if ((_binary[SizePerFont + (Y * _size8) + X] & (0x80 >> ww)) == 0) continue;

                int Max = (X * 8) + ww;

                Temp.Points.Add((Max, Y));

                // Get max font width used.
                Temp.Width = (ushort)Math.Max(Temp.Width, Max);
            }
        }

        // Return the glyph.
        return new(0, 0, Temp.Width, _size, Temp.Points);
    }

    public override string GetFamilyName() => "N/A";

    public override string GetStyleName() => "N/A";

    public override int GetHeight() => _size;

    public override Glyph? GetGlyph(char c)
    {
        if (c < 32 || c >= _glyphs.Length + 32)
        {
            return null;
        }

        return _glyphs[c - 32];
    }

    /// <summary>
    /// The binary data of the BTF font face.
    /// </summary>
    private readonly byte[] _binary;

    /// <summary>
    /// The line height of the font face.
    /// </summary>
    private ushort _size;

    /// <summary>
    /// The line height of the font face divided by eight.
    /// </summary>
    private ushort _size8;

    /// <summary>
    /// The glyphs of the font face in ASCII ranging from 0x20 to 0x7F.
    /// </summary>
    private readonly Glyph[] _glyphs = new Glyph[96];
}