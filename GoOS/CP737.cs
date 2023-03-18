/// <summary>
/// Prints characters on the CP737 code-page.
/// </summary>
/// 

using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.TCP;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
using System;
using System.Collections.Generic;
using Sys = Cosmos.System;
using System.IO;
using System.Linq.Expressions;
using Cosmos.Core.Memory;
using System.Drawing;
using IL2CPU.API.Attribs;
using System.Text;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using Cosmos.Core;
using Cosmos.System.Network.IPv4.UDP;
using System.Diagnostics;
using GoOS;
using Cosmos.HAL.BlockDevice.Registers;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Cosmos.System;
using Console = System.Console;
using System.Linq;
using Cosmos.HAL.BlockDevice;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;


public static class CP737Console
{
    static readonly Cosmos.System.Console console = new(null);

    static readonly Dictionary<char, byte> unicodeToCP737
        = new()
        {
            {  '░', 0xB0 }, {  '▒', 0xB1 },
            {  '▓', 0xB2 }, {  '│', 0xB3 },
            {  '┤', 0xB4 }, {  '╡', 0xB5 },
            {  '╢', 0xB6 }, {  '╖', 0xB7 },
            {  '╕', 0xB8 }, {  '╣', 0xB9 },
            {  '║', 0xBA }, {  '╗', 0xBB },
            {  '╝', 0xBC }, {  '╜', 0xBD },
            {  '╛', 0xBE }, {  '┐', 0xBF },
            {  '└', 0xC0 }, {  '┴', 0xC1 },
            {  '┬', 0xC2 }, {  '├', 0xC3 },
            {  '─', 0xC4 }, {  '┼', 0xC5 },
            {  '╞', 0xC6 }, {  '╟', 0xC7 },
            {  '╚', 0xC8 }, {  '╔', 0xC9 },
            {  '╩', 0xCA }, {  '╦', 0xCB },
            {  '╠', 0xCC }, {  '═', 0xCD },
            {  '╬', 0xCE }, {  '╧', 0xCF },
            {  '╨', 0xD0 }, {  '╤', 0xD1 },
            {  '╥', 0xD2 }, {  '╙', 0xD3 },
            {  '╘', 0xD4 }, {  '╒', 0xD5 },
            {  '╓', 0xD6 }, {  '╫', 0xD7 },
            {  '╪', 0xD8 }, {  '┘', 0xD9 },
            {  '┌', 0xDA }, {  '█', 0xDB },
            {  '▄', 0xDC }, {  '▌', 0xDD },
            {  '▐', 0xDE }, {  '▀', 0xDF },
            {  '■', 0xFE }
        };

    /// <summary>
    /// Writes the given characters at the current position.
    /// </summary>
    /// <param name="line">The line to write.</param>
    /// <param name="x">The X coordinate to write the text to. If set to a negative value, the current cursor position will be used.</param>
    /// <param name="y">The Y coordinate to write the text to. If set to a negative value, the current cursor position will be used.</param>
    public static void Write(string line, int x = -1, int y = -1)
    {
        console.Background = Console.BackgroundColor;
        console.Foreground = Console.ForegroundColor;

        if (x < 0) x = Console.CursorLeft;
        if (y < 0) y = Console.CursorTop;

        Span<byte> encodingBuffer = stackalloc byte[1];
        Span<char> inputBuffer = stackalloc char[1];

        for (int i = 0; i < line.Length; i++) {
            console.X = x;
            console.Y = y;

            if (line[i] == '\n') {
                x = 0;
                y++;
                continue;
            }

            if (unicodeToCP737.TryGetValue(line[i], out byte mapped)) {
                console.Write(mapped);
            } else {
                inputBuffer[0] = line[i];
                Encoding.ASCII.GetBytes(inputBuffer, encodingBuffer);
                console.Write(encodingBuffer[0]);
            }

            x++;
            if(x > Console.WindowWidth) {
                x = 0;
                y++;
            }

            if(y > Console.WindowHeight) {
                // clip character printing
                return;
            }
        }

        // advance the actual console cursor
        Console.CursorLeft = x;
        Console.CursorTop = y;
    }
}