using System;
using System.Collections.Generic;
using PepperDash.Core;

namespace PepperDash.Essentials.Plugins.Lg.Display
{
    /// <summary>
    /// LG SIC "mc" (Remote Control Key Action) hex keycodes, for the RS232 driver.
    ///
    /// Values marked "sourced from community documentation" converge across multiple independent
    /// installer/community references (Just Add Power, Cisco Community, openHAB's lgtvserial
    /// binding, various DIY LG-RS232 projects) but were NOT cross-checked against LG's own
    /// primary manual PDF (extraction failed) or against this specific display model/firmware.
    /// Spot-check against the official manual before production deployment.
    ///
    /// Values still marked "XX" / TODO are genuinely unconfirmed - no source found (Home varies
    /// by model generation and wasn't confirmed anywhere; the four media-app buttons are NOT part
    /// of the standard remote-key table at all - LG's app-launch mechanism, if any, over RS232 is
    /// a separate, unresearched question, not just a missing hex code).
    ///
    /// Command names deliberately mirror IrStandardCommands.cs so the two tables stay easy to
    /// cross-check against each other and against the React app's brand-agnostic button set.
    /// </summary>
    public static class LgKeyCodes
    {
        public const string PowerToggle = "08"; // sourced from community documentation
        public const string KP1 = "11"; // sourced from community documentation
        public const string KP2 = "12"; // sourced from community documentation
        public const string KP3 = "13"; // sourced from community documentation
        public const string KP4 = "14"; // sourced from community documentation
        public const string KP5 = "15"; // sourced from community documentation
        public const string KP6 = "16"; // sourced from community documentation
        public const string KP7 = "17"; // sourced from community documentation
        public const string KP8 = "18"; // sourced from community documentation
        public const string KP9 = "19"; // sourced from community documentation
        public const string KP0 = "10"; // sourced from community documentation
        public const string ChannelUp = "00"; // sourced from community documentation (LG genuinely uses "00" for CH+)
        public const string ChannelDown = "01"; // sourced from community documentation
        public const string Last = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Home = "XX"; // TODO: verify against LG SIC reference (unconfirmed, varies by model generation) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Menu = "43"; // sourced from community documentation
        public const string DpadUp = "40"; // sourced from community documentation
        public const string DpadDown = "41"; // sourced from community documentation
        public const string DpadLeft = "07"; // sourced from community documentation
        public const string DpadRight = "06"; // sourced from community documentation
        public const string DpadSelect = "44"; // sourced from community documentation
        public const string Enter = "44"; // sourced from community documentation (same as DpadSelect)
        public const string Back = "28"; // sourced from community documentation
        public const string Exit = "5b"; // sourced from community documentation
        public const string Netflix = "XX"; // TODO: verify - "XX" placeholder; app launch is likely
                                             // NOT part of the standard mc table at all, see class summary above
        public const string PrimeVideo = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Disney = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string SamsungTvPlus = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Guide = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string FuncRed = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string FuncGreen = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string FuncYellow = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string FuncBlue = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Play = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Pause = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string FastForward = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code
        public const string Rewind = "XX"; // TODO: verify against LG SIC reference (unconfirmed) - "XX" is not a real hex byte, deliberately invalid so it can't be mistaken for a genuine code

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
