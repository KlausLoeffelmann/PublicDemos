// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

public partial class MainForm
{
    /// <summary>Small JSON-serialized snapshot of the window's restore state.</summary>
    private sealed record WindowSettings
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public bool Maximized { get; init; }
    }
}
