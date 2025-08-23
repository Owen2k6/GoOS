using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Gold.Compression;

namespace Gold.Graphics;

public unsafe static class Image
{
    #region Structure

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct TGAHeader
    {
        public char Magic1; // must be zero
        public char ColorMap; // must be zero
        public char Encoding; // must be 2
        public short CMaporig, CMaplen, CMapent; // must be zero
        public short X; // must be zero
        public short Y; // image's height
        public short Height; // image's height
        public short Width; // image's width
        public char ColorDepth; // must be 32
        public char PixelType; // must be 40
    }

    #endregion

    #region Methods

    /// <summary>
    /// Loads a bitmap file.
    /// Based on: https://github.com/CosmosOS/Cosmos/blob/master/source/Cosmos.System2/Graphics/Bitmap.cs
    /// </summary>
    /// <param name="Binary">Raw file data.</param>
    /// <returns>BMP file as a <see cref="Canvas"/> instance.</returns>
public static Canvas FromBitmap(byte[] binary, bool useBGR = false)
{
    using var reader = new System.IO.BinaryReader(new System.IO.MemoryStream(binary));

    // --- FILE HEADER (14 bytes) ---
    if (reader.ReadByte() != (byte)'B' || reader.ReadByte() != (byte)'M')
        throw new FormatException("Not a BMP.");

    uint fileSize = reader.ReadUInt32();         // not strictly needed
    reader.ReadUInt16();                         // reserved1
    reader.ReadUInt16();                         // reserved2
    uint pixelTableOffset = reader.ReadUInt32(); // where pixel array starts

    // --- DIB HEADER (at least 40 bytes for BITMAPINFOHEADER) ---
    uint infoHeaderSize = reader.ReadUInt32();
    if (infoHeaderSize != 40 && infoHeaderSize != 56 && infoHeaderSize != 124)
        throw new FormatException("Unsupported DIB header size.");

    // width/height are little-endian signed in BMP; height can be negative (top-down)
    int width = reader.ReadInt32();
    int heightSigned = reader.ReadInt32();
    bool topDown = heightSigned < 0;
    int height = topDown ? -heightSigned : heightSigned;

    ushort planes = reader.ReadUInt16();
    if (planes != 1) throw new FormatException("Planes != 1");

    ushort bpp = reader.ReadUInt16();                  // bits per pixel
    uint compression = reader.ReadUInt32();            // 0 = BI_RGB, 3 = BI_BITFIELDS
    uint biSizeImage = reader.ReadUInt32();            // may be 0 for BI_RGB
    reader.ReadInt32();                                // biXPelsPerMeter
    reader.ReadInt32();                                // biYPelsPerMeter
    uint clrUsed = reader.ReadUInt32();                // palette colors used (0 = default)
    reader.ReadUInt32();                               // clrImportant

    if (compression != 0 && compression != 3)
        throw new NotImplementedException("Compressed BMP not supported.");

    if (width <= 0 || height <= 0)
        throw new FormatException("Invalid image dimensions.");

    // --- PALETTE (for <= 8bpp) ---
    uint[] palette = null;
    if (bpp <= 8)
    {
        // palette size: if clrUsed == 0 -> default 2^bpp
        int paletteEntries = (clrUsed != 0) ? (int)clrUsed : (1 << bpp);

        // palette starts immediately after DIB header
        long palettePos = 14 + infoHeaderSize;
        reader.BaseStream.Position = palettePos;

        palette = new uint[paletteEntries];
        for (int i = 0; i < paletteEntries; i++)
        {
            byte b = reader.ReadByte();
            byte g = reader.ReadByte();
            byte r = reader.ReadByte();
            reader.ReadByte(); // reserved
            palette[i] = (uint)((255 << 24) | (r << 16) | (g << 8) | b); // ARGB
        }
    }

    // --- ROW LAYOUT / PADDING ---
    // data bytes per row (without padding)
    int rowDataBytes =
        bpp == 32 ? width * 4 :
        bpp == 24 ? width * 3 :
        bpp == 8  ? width :
        bpp == 4  ? (width + 1) / 2 :
        bpp == 1  ? (width + 7) / 8 :
        throw new NotSupportedException($"Unsupported bpp: {bpp}");

    // each row is padded to 4-byte boundary
    int paddedRowBytes = (rowDataBytes + 3) & ~3;
    int paddingPerRow = paddedRowBytes - rowDataBytes;

    // move to pixel array
    reader.BaseStream.Position = pixelTableOffset;

    // prepare canvas
    var temp = new Canvas((ushort)width, (ushort)height);

    // buffer for one row (just the actual data bytes; we'll skip padding separately)
    byte[] row = new byte[rowDataBytes];

    // reading order: BMP is bottom-up unless height negative (top-down)
    for (int y = 0; y < height; y++)
    {
        // read one row of pixel data
        if (rowDataBytes > 0)
        {
            int read = reader.Read(row, 0, rowDataBytes);
            if (read != rowDataBytes) throw new FormatException("Unexpected EOF in pixel data.");
        }

        // skip padding bytes
        if (paddingPerRow > 0) reader.BaseStream.Position += paddingPerRow;

        // destination Y in our buffer
        int dstRow = topDown ? y : (height - 1 - y);

        switch (bpp)
        {
            case 32:
            {
                int p = 0;
                for (int x = 0; x < width; x++)
                {
                    int b = row[p++];
                    int g = row[p++];
                    int r = row[p++];
                    int a = row[p++];
                    temp.Internal[x + dstRow * width] = (uint)((a << 24) | (r << 16) | (g << 8) | b);
                }
                break;
            }

            case 24:
            {
                int p = 0;
                for (int x = 0; x < width; x++)
                {
                    int b = row[p++];
                    int g = row[p++];
                    int r = row[p++];
                    uint argb = useBGR
                        ? (uint)((255 << 24) | (b << 16) | (g << 8) | r)   // atypical mode you had
                        : (uint)((255 << 24) | (r << 16) | (g << 8) | b); // normal
                    temp.Internal[x + dstRow * width] = argb;
                }
                break;
            }

            case 8:
            {
                if (palette == null) throw new FormatException("Missing palette for 8bpp.");
                for (int x = 0; x < width; x++)
                {
                    int idx = row[x];
                    temp.Internal[x + dstRow * width] = palette[idx];
                }
                break;
            }

            case 4:
            {
                if (palette == null) throw new FormatException("Missing palette for 4bpp.");
                int p = 0;
                for (int x = 0; x < width; x += 2)
                {
                    int packed = row[p++];

                    int idx1 = (packed >> 4) & 0x0F; // high nibble
                    temp.Internal[x + dstRow * width] = palette[idx1];

                    if (x + 1 < width)
                    {
                        int idx2 = packed & 0x0F;   // low nibble
                        temp.Internal[x + 1 + dstRow * width] = palette[idx2];
                    }
                }
                break;
            }

            case 1:
            {
                if (palette == null) throw new FormatException("Missing palette for 1bpp.");
                int p = 0;
                int outX = 0;
                while (outX < width)
                {
                    int packed = row[p++];
                    // MSB first
                    for (int bit = 7; bit >= 0 && outX < width; bit--)
                    {
                        int idx = (packed >> bit) & 1;
                        temp.Internal[outX++ + dstRow * width] = palette[idx];
                    }
                }
                break;
            }
        }
    }

    return temp;
}



    /// <summary>
    /// Loads a PNG file.
    /// </summary>
    /// <param name="Binary">Raw file data.</param>
    /// <returns>PNG file as a <see cref="Canvas"/> instance.</returns>
    public static Canvas FromPNG(byte[] Binary)
    {
        // Check header for invalid magic data.
        if (Binary[0] != 137 ||
            Binary[1] != 80 ||
            Binary[2] != 78 ||
            Binary[3] != 71 ||
            Binary[4] != 13 ||
            Binary[5] != 10 ||
            Binary[6] != 26 ||
            Binary[7] != 10)
        {
            throw new("Invalid header magic!");
        }

        BinaryReader Reader = new(new MemoryStream(Binary));
        Reader.BaseStream.Position = 8;
        Canvas Result = new(0, 0);

        while (Reader.BaseStream.Position < Reader.BaseStream.Length)
        {
            uint Length = Reader.ReadUInt32();
            long Position = Reader.BaseStream.Position;
            bool IsDone = false;

            switch (Encoding.ASCII.GetString(Reader.ReadBytes(4)))
            {
                case "IHDR":
                    Result.Width = (ushort)Reader.ReadUInt32();
                    Result.Height = (ushort)Reader.ReadUInt32();
                    Reader.BaseStream.Position += 5;
                    break;
                case "PLTE":
                    break;
                case "IDAT":
                    List<byte> Buffer = new();
                    Reader.BaseStream.Position += 2;
                    for (int i = 2; i < Length; i++)
                    {
                        Buffer.Add(Reader.ReadByte());
                    }

                    List<byte> Data = DeflateStream.Inflate(Buffer);

                    BinaryReader D = new(new MemoryStream(Data.ToArray()));

                    var totalScanlines = Data.Count / (Result.Width + 1) / 4;

                    var prevScanline = new List<byte>();

                    for (int y = 0; y < totalScanlines; y++)
                    {
                        var filter = D.ReadByte();

                        var dat = new List<byte>();

                        for (int x = 0; x < Result.Width * 4; x++)
                        {
                            dat.Add(D.ReadByte());
                        }

                        var scanline = new List<byte>();

                        if (filter == 1)
                        {
                            scanline.Add(dat[0]);
                            for (var index = 1; index < dat.Count; index++)
                            {
                                scanline.Add((byte)((scanline[index - 4 > 0 ? index - 4 : 0] + dat[index - 1]) % 256));
                                //scanline.Add((byte) (255));
                            }
                        }
                        else if (filter == 2)
                        {
                            for (var index = 0; index < dat.Count; index++)
                            {
                                scanline.Add((byte)((prevScanline[index] + dat[index]) % 256));
                                //scanline.Add((byte) (255));
                            }
                        }
                        else
                        {
                        }

                        var line = new BinaryReader(new MemoryStream(scanline.ToArray()));
                        prevScanline.Clear();
                        prevScanline.AddRange(scanline);

                        for (int x = 0; x < Result.Width; x++)
                        {
                            // Read ARGB color
                            Result[x, y] = new(line.ReadUInt32());
                        }
                    }

                    break;
                case "IEND":
                    IsDone = true;
                    break;
            }

            if (IsDone)
            {
                break;
            }

            Reader.BaseStream.Position = (int)(Position + Length) + 4;
        }

        return Result;
    }

    /// <summary>
    /// Loads a TGA file.
    /// </summary>
    /// <param name="Binary">Raw file data.</param>
    /// <returns>TGA file as a <see cref="Canvas"/> instance.</returns>
    public static Canvas FromTGA(byte[] Binary)
    {
        Canvas Result = new(0, 0);
        TGAHeader* Header;

        fixed (byte* P = Binary)
        {
            Header = (TGAHeader*)P;
        }

        Result.Height = (ushort)Header->Height;
        Result.Width = (ushort)Header->Width;

        switch (Header->ColorDepth)
        {
            case (char)32:
                for (uint I = 0; I < Result.Width * Result.Height * 4; I++)
                {
                    Result[I] = new(Binary[I + 22], Binary[I + 21], Binary[I + 20], Binary[I + 19]);
                }

                break;
            case (char)24:
                for (uint I = 0; I < Result.Width * Result.Height * 3; I++)
                {
                    Result[I] = new(255, Binary[I + 21], Binary[I + 20], Binary[I + 19]);
                }

                break;
        }

        return Result;
    }

    /// <summary>
    /// Loads a PPM file.
    /// </summary>
    /// <param name="Binary">Raw file data.</param>
    /// <returns>PPM file as a <see cref="Canvas"/> instance.</returns>
    public static Canvas FromPPM(byte[] Binary)
    {
        BinaryReader Reader = new(new MemoryStream(Binary));

        if (Reader.ReadChar() != 'P' || Reader.ReadChar() != '6')
        {
            throw new("Not a PPM image!");
        }

        Reader.ReadChar(); // Skip Newline
        string widths = "", heights = "";

        for (char TMP = '\0'; TMP != ' '; TMP = Reader.ReadChar())
        {
            if (TMP == '#')
            {
                while (Reader.ReadChar() != '\n') ;
            }
            else
            {
                widths += TMP;
            }
        }

        for (char TMP = '\0'; TMP != '0' && TMP != '9'; TMP = Reader.ReadChar())
        {
            heights += TMP;
        }

        if (Reader.ReadChar() != '2' || Reader.ReadChar() != '5' || Reader.ReadChar() != '5')
        {
            throw new("Improper file data!");
        }

        Reader.ReadChar(); // Skip Newline

        Canvas Result = new((ushort)uint.Parse(widths), (ushort)uint.Parse(heights));

        for (int Y = 0; Y < Result.Height; Y++)
        {
            for (int X = 0; X < Result.Width; X++)
            {
                Result[X, Y] = new(Reader.ReadByte(), Reader.ReadByte(), Reader.ReadByte());
            }
        }

        return Result;
    }

    #endregion
}