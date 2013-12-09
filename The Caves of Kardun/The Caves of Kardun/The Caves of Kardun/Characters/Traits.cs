using System;

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// Positive traits
    /// </summary>
    [Flags]
    public enum PositiveTraits
    {
        SuperStrength,
        SuperLife,
        ElvenSpeed,
        TwentyTwentyVision,
        Ambidextrous,
    }

    /// <summary>
    /// Negative traits
    /// </summary>
    [Flags]
    public enum NegativeTraits
    {
        ColourBlind,
        MissingAnEye,
        MissingAnArm,
        NearSighted,
        SenseOfDirection,
    }
}
