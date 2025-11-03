// --------------------------------------------------------------------------
// 
//                               █▀▀█ ░█─── ▀█▀ ░█▀▀▀█
//                              ░█▄▄█ ░█─── ░█─ ─▀▀▀▄▄
//                              ░█─░█ ░█▄▄█ ▄█▄ ░█▄▄▄█
// 
//  --------------------------------------------------------------------------
//  File: Program.cs
// 
//  Author: Pablo Perdomo Falcón
//  Web: https://www.pabllopf.dev/
// 
//  Copyright (c) 2021 GNU General Public License v3.0
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program. If not, see <http://www.gnu.org/licenses/>.
// 
//  --------------------------------------------------------------------------

using System;
using Alis.Core.Graphic.OpenGL;
using Alis.Core.Graphic.OpenGL.Enums;
using Alis.Core.Graphic.Platforms;
using Alis.Sample.OpenGL.Samples;

namespace Alis.Sample.OpenGL
{
    /// <summary>
    ///     The program class (entry point) for the OpenGL samples.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     Main entry point.
        /// </summary>
        private static void Main()
        {
            // Select the native platform implementation based on the compilation symbols.
            // Each platform provides window/context creation, input and GL function resolution.
            INativePlatform platform;
#if osxarm64 || osxarm || osxx64 || osx || osxarm || osxx64 || osx
            platform = new Alis.Core.Graphic.Platforms.Osx.MacNativePlatform();
#elif winx64 || winx86 || winarm64 || winarm || win
            platform = new Alis.Core.Graphic.Platforms.Win.WinNativePlatform();
#elif linuxx64 || linuxx86 || linuxarm64 || linuxarm || linux
            platform = new Alis.Core.Graphic.Platforms.Linux.LinuxNativePlatform();
#else
            throw new Exception("Unsupported operating system");
#endif

            // Ask the user which example to run. These examples demonstrate basic OpenGL usage.
            Console.WriteLine("Choose the example to show:");
            Console.WriteLine("0: Red background");
            Console.WriteLine("1: White triangle");
            Console.WriteLine("2: Cube (empty)");
            Console.WriteLine("3: Unfilled square");
            Console.WriteLine("4: Custom texture (BMP)");
            Console.WriteLine("5: Load font with custom BMP");
            Console.WriteLine("6: Load font with custom BMP 2");
            Console.WriteLine("7: Load font with timer");
            Console.Write("Option: ");

            int option = 0;
            string input = Console.ReadLine();
            int.TryParse(input, out option);

            // Create the chosen example instance. Default is the simple red background example.
            IExample example = option switch
            {
                1 => new TriangleExample(),
                2 => new CubeExample(),
                3 => new SquareUnfilledExample(),
                4 => new TextureSampleCustomBmpExample(),
                5 => new LoadFontWithCustomBmpExample(),
                6 => new LoadFontWithCustomBmpExample2(),
                7 => new LoadFontwithTimerExample(),
                _ => new SimpleRedExample()
            };

            // Initialize the native platform (window + OpenGL context).
            bool ok = platform.Initialize(800, 600, "C# + OpenGL Platform");
            if (!ok)
            {
                Console.WriteLine("Could not initialize the window or OpenGL context. The program will exit.");
                platform.Cleanup();
                return;
            }

            // Make the GL context current and initialize the GL function loader.
            platform.MakeContextCurrent();
            Gl.Initialize(platform.GetProcAddress);

            // Set viewport and enable depth testing as a reasonable default for samples.
            Gl.GlViewport(0, 0, platform.GetWindowWidth(), platform.GetWindowHeight());
            Gl.GlEnable(EnableCap.DepthTest);

            // Initialize and show the example window.
            example.Initialize();
            platform.ShowWindow();
            platform.SetTitle("C# + OpenGL Platform - Example " + option);

            // Main loop: poll events, handle simple key reporting, draw the example and swap buffers.
            bool running = true;
            while (running)
            {
                // Poll system events; returns false when the window should close.
                running = platform.PollEvents();

                // If a key was pressed, log it to the console (simple debug info).
                if (platform.TryGetLastKeyPressed(out ConsoleKey key))
                {
                    Console.WriteLine($"Key pressed: {key}");
                }

                // Draw the sample and present the frame.
                example.Draw();
                platform.SwapBuffers();

                // Query for any GL errors after presenting the frame and log them if present.
                int glError = Gl.GlGetError();
                if (glError != 0)
                {
                    Console.WriteLine($"OpenGL error after swap buffers: 0x{glError:X}");
                }
            }

            // Cleanup example resources and platform resources before exit.
            example.Cleanup();
            platform.Cleanup();
        }
    }
}