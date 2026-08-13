using System;
using System.Collections.Generic;
using PepperDash.Core;

namespace PepperDash.Essentials.Plugins.Lg.Display
{
    /// <summary>
    /// LG SIC "mc" (Remote Control Key Action) hex keycodes, for the RS232 driver.
    ///
    /// TODO: every value below is a PLACEHOLDER ("00") and MUST be replaced with the real
    /// hex code from LG's official SIC/RS232 protocol reference before this ships. Sending an
    /// incorrect code fails silently against real hardware - do not deploy with these unverified.
    /// Command names deliberately mirror IrStandardCommands.cs so the two tables stay easy to
    /// cross-check against each other and against the React app's brand-agnostic button set.
    /// </summary>
    public static class LgKeyCodes
    {
        public const string PowerToggle = "00"; // TODO: verify against LG SIC reference
        public const string KP1 = "00"; // TODO: verify against LG SIC reference
        public const string KP2 = "00"; // TODO: verify against LG SIC reference
        public const string KP3 = "00"; // TODO: verify against LG SIC reference
        public const string KP4 = "00"; // TODO: verify against LG SIC reference
        public const string KP5 = "00"; // TODO: verify against LG SIC reference
        public const string KP6 = "00"; // TODO: verify against LG SIC reference
        public const string KP7 = "00"; // TODO: verify against LG SIC reference
        public const string KP8 = "00"; // TODO: verify against LG SIC reference
        public const string KP9 = "00"; // TODO: verify against LG SIC reference
        public const string KP0 = "00"; // TODO: verify against LG SIC reference
        public const string ChannelUp = "00"; // TODO: verify against LG SIC reference
        public const string ChannelDown = "00"; // TODO: verify against LG SIC reference
        public const string Last = "00"; // TODO: verify against LG SIC reference
        public const string Home = "00"; // TODO: verify against LG SIC reference
        public const string Menu = "00"; // TODO: verify against LG SIC reference
        public const string DpadUp = "00"; // TODO: verify against LG SIC reference
        public const string DpadDown = "00"; // TODO: verify against LG SIC reference
        public const string DpadLeft = "00"; // TODO: verify against LG SIC reference
        public const string DpadRight = "00"; // TODO: verify against LG SIC reference
        public const string DpadSelect = "00"; // TODO: verify against LG SIC reference
        public const string Enter = "00"; // TODO: verify against LG SIC reference
        public const string Back = "00"; // TODO: verify against LG SIC reference
        public const string Exit = "00"; // TODO: verify against LG SIC reference
        public const string Netflix = "00"; // TODO: verify against LG SIC reference
        public const string PrimeVideo = "00"; // TODO: verify against LG SIC reference
        public const string Disney = "00"; // TODO: verify against LG SIC reference
        public const string SamsungTvPlus = "00"; // TODO: verify against LG SIC reference
        public const string Guide = "00"; // TODO: verify against LG SIC reference
        public const string FuncRed = "00"; // TODO: verify against LG SIC reference
        public const string FuncGreen = "00"; // TODO: verify against LG SIC reference
        public const string FuncYellow = "00"; // TODO: verify against LG SIC reference
        public const string FuncBlue = "00"; // TODO: verify against LG SIC reference
        public const string Play = "00"; // TODO: verify against LG SIC reference
        public const string Pause = "00"; // TODO: verify against LG SIC reference
        public const string FastForward = "00"; // TODO: verify against LG SIC reference
        public const string Rewind = "00"; // TODO: verify against LG SIC reference

        public static readonly Dictionary<string, string> CommandDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { nameof(PowerToggle), PowerToggle },
            { nameof(KP1), KP1 },
            { nameof(KP2), KP2 },
            { nameof(KP3), KP3 },
            { nameof(KP4), KP4 },
            { nameof(KP5), KP5 },
            { nameof(KP6), KP6 },
            { nameof(KP7), KP7 },
            { nameof(KP8), KP8 },
            { nameof(KP9), KP9 },
            { nameof(KP0), KP0 },
            { nameof(ChannelUp), ChannelUp },
            { nameof(ChannelDown), ChannelDown },
            { nameof(Last), Last },
            { nameof(Home), Home },
            { nameof(Menu), Menu },
            { nameof(DpadUp), DpadUp },
            { nameof(DpadDown), DpadDown },
            { nameof(DpadLeft), DpadLeft },
            { nameof(DpadRight), DpadRight },
            { nameof(DpadSelect), DpadSelect },
            { nameof(Enter), Enter },
            { nameof(Back), Back },
            { nameof(Exit), Exit },
            { nameof(Netflix), Netflix },
            { nameof(PrimeVideo), PrimeVideo },
            { nameof(Disney), Disney },
            { nameof(SamsungTvPlus), SamsungTvPlus },
            { nameof(Guide), Guide },
            { nameof(FuncRed), FuncRed },
            { nameof(FuncGreen), FuncGreen },
            { nameof(FuncYellow), FuncYellow },
            { nameof(FuncBlue), FuncBlue },
            { nameof(Play), Play },
            { nameof(Pause), Pause },
            { nameof(FastForward), FastForward },
            { nameof(Rewind), Rewind }
        };

        public static string GetCommandValue(string commandName)
        {
            Debug.LogInformation("LgKeyCodes: GetCommandValue() called for commandName-'{0}'", commandName);
            if (CommandDictionary.TryGetValue(commandName, out var value))
                return value;
            return null;
        }
    }
}
