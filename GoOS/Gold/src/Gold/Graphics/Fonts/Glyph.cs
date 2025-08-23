/*
 *  This file is part of the Mirage Desktop Environment.
 *  github.com/mirage-desktop/Mirage
 */

using System;
using System.Collections.Generic;

namespace Gold.Graphics.Fonts;

/// <summary>
/// Represents a single, reusable glyph for a character.
/// </summary>
public class Glyph
{
    /// <summary>
    /// Initializes a new glyph.
    /// </summary>
    /// <param name="left">The horizontal offset of the glyph's bitmap.</param>
    /// <param name="top">The vertical offset of the glyph's bitmap.</param>
    /// <param name="width">The width of the glyph's bitmap in pixels.</param>
    /// <param name="height">The height of the glyph's bitmap in pixels.</param>
    /// <param name="bitmap">The buffer of the glyph's bitmap, as an array of alpha values.</param>
    public Glyph(int left, int top, int width, int height, byte[] bitmap)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
        Bitmap = bitmap;
        Points = new List<(int X, int Y)>();
    }

    /// <summary>
    /// Initializes a new glyph.
    /// </summary>
    /// <param name="left">The horizontal offset of the glyph's bitmap.</param>
    /// <param name="top">The vertical offset of the glyph's bitmap.</param>
    /// <param name="width">The width of the glyph's bitmap in pixels.</param>
    /// <param name="height">The height of the glyph's bitmap in pixels.</param>
    /// <param name="points">The buffer of the glyph's bitmap, as a list of points.</param>
    public Glyph(int left, int top, int width, int height, List<(int X, int Y)> points)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
        Bitmap = Array.Empty<byte>();
        Points = points;
    }

    /// <summary>
    /// The horizontal offset of the glyph's bitmap.
    /// </summary>
    public readonly int Left;

    /// <summary>
    /// The vertical offset of the glyph's bitmap. Should be subtracted from the baseline.
    /// </summary>
    public readonly int Top;

    /// <summary>
    /// The width of the glyph's bitmap in pixels.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// The height of the glyph's bitmap in pixels.
    /// </summary>
    public readonly int Height;

    /// <summary>
    /// The buffer of the glyph's bitmap, as an array of alpha values.
    /// </summary>
    public readonly byte[] Bitmap;

    /// <summary>
    /// The buffer of the glyph's bitmap, as a list of points.
    /// </summary>
    public readonly List<(int X, int Y)> Points;
}