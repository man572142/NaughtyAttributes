using System;

namespace NaughtyAttributes
{
    [Flags]
    public enum DisplayOptions
    {
        None = 0,

        // Presets
        Default = AtBottom | SingleLineHeight | FullWidth,
        MiniBottom = AtBottom | SingleLineHeight | HalfWidth,
        MiniTop = OnTop | SingleLineHeight | HalfWidth,
        FatBottom = AtBottom | DoubleLineHeight | HalfWidth,
        Big = AtBottom | TripleLineHeight | FullWidth,

        // Position
        OnTop = 1 << 0,
        AtBottom = 1 << 1,
        AlongSide = 1 << 2,

        // Height
        SingleLineHeight = 1 << 3,
        DoubleLineHeight = 1 << 4,
        TripleLineHeight = 1 << 5,

        // Width
        FullWidth = 1 << 6,
        HalfWidth = 1 << 7,
    }
}