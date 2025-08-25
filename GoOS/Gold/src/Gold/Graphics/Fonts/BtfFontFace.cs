/*
 *  Gold - A powerful & lightweight graphics library based on PrismAPI.
 *  Copyright © 2024 Mobren
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or (at
 *  your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along
 *  with this program; if not, write to the Free Software Foundation, Inc.,
 *  51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Gold.Graphics.Fonts;

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
    public BtfFontFace(byte[] binary, ushort size, int SpacingModifier = 0)
    {
        _binary = binary;
        _spacingModifier = SpacingModifier;
        
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
        //Console.Write("1");
        
        // Create new emp ty glyph.
        List<(int X, int Y)> Points = new();
        ushort Width = 0;
        
        //Console.Write("2");
        
        // Get the index of the character in the font.
        int Index = DefaultCharset.IndexOf(c);
        
        //Console.Write("3");
        
        // Check if there is a glyph for the given character.
        if (Index < 0)
        {
            //Console.Write("3.5");
            // Return an empty glyph.
            return new(0, 0, _size / 2, _size, Points);
        }
        
        //Console.Write("4");
        
        ushort SizePerFont = (ushort)(_size * _size8 * Index);

        //Console.Write("5");
        
        for (int i = 0; i < _size * _size8; i++)
        {
            //Console.Write("6");
            
            int X = i % _size8;
            int Y = i / _size8;
            
            //Console.Write("7");
            
            for (int ww = 0; ww < 8; ww++)
            {
                //Console.Write("8");
                
                int index = SizePerFont + Y * _size8 + X;

                try
                {
                    if ((_binary[SizePerFont + Y * _size8 + X] & (0x80 >> ww)) == 0) continue;
                }
                catch (Exception e)
                {
                    continue;
                }

                //Console.Write("9");
                
                int Max = (X * 8) + ww;
                
                //Console.Write("10");
                
                Points.Add((Max, Y));
                
                //Console.Write("11");
                
                // Get max font width used.
                Width = (ushort)Math.Max(Width, Max);
                
                //Console.Write("12");
            }
        }
        
        //Console.Write("13");

        // Return the glyph.
        return new(0, 0, Width, _size, Points);
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

    public override ushort MeasureString(string s)
    {
        ushort returnVal = 0;

        for (int i = 0; i < s.Length; i++) returnVal = (ushort)(returnVal + (GetGlyph(s[i])!.Width + 2));

        return (ushort)(returnVal + (s.Length * SpacingModifier()));
    }

    public override int SpacingModifier()
    {
        return _spacingModifier;
    }

    /// <summary>
    /// The standard charset of all fonts.
    /// </summary>
    public const string DefaultCharset =
        "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

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

    /// <summary>
    /// Character spacing offset.
    /// </summary>
    private int _spacingModifier = 0;
}