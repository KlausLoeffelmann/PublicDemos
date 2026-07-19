// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Controls;

public sealed partial class FontTemplate
{
    private sealed class CachedFont(Font font)
    {
        public WeakReference<Font> Reference { get; } = new(font);
    }
}
