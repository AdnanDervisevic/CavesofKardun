using System;

namespace The_Caves_of_Kardun
{
    /// <summary>
    /// Positive traits
    /// </summary>
    [Flags]
    public enum PositiveTraits
    { 
        SuperStrength       = 1,
        SuperLife           = 2,
        ElvenSpeed          = 4,
        TwentyTwentyVision  = 8,
        Ambidextrous        = 16,
    }

    /// <summary>
    /// Negative traits
    /// </summary>
    [Flags]
    public enum NegativeTraits
    {
        ColourBlind         = 1,
        MissingAnEye        = 2,
        MissingAnArm        = 4,
        NearSighted         = 8,
        SenseOfDirection    = 16,
    }
}
