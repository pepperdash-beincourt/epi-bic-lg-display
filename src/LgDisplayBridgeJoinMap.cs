using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace Epi.Display.Lg
{
    public class LgDisplayBridgeJoinMap : DisplayControllerJoinMap
    {
        #region Digitals

        //[JoinName("PowerOff")]
        //public JoinDataComplete PowerOff = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 1,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Power Off",
        //        JoinCapabilities = eJoinCapabilities.FromSIMPL,
        //        JoinType = eJoinType.Digital
        //    });

        //[JoinName("PowerOn")]
        //public JoinDataComplete PowerOn = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 2,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Power On",
        //        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
        //        JoinType = eJoinType.Digital
        //    });

        //[JoinName("IsTwoWayDisplay")]
        //public JoinDataComplete IsTwoWayDisplay = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 3,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Is Two Way Display",
        //        JoinCapabilities = eJoinCapabilities.ToSIMPL,
        //        JoinType = eJoinType.Digital
        //    });

        [JoinName("IsCoolingDown")]
        public JoinDataComplete IsCoolingDown = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 4,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Display is Cooling Down",
               JoinCapabilities = eJoinCapabilities.ToSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("IsWarmingUp")]
        public JoinDataComplete IsWarmingUp = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 5,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Display is Warming Up",
               JoinCapabilities = eJoinCapabilities.ToSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress0")]
        public JoinDataComplete KeypadPress0 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 21,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 0",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress1")]
        public JoinDataComplete KeypadPress1 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 22,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 1",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress2")]
        public JoinDataComplete KeypadPress2 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 23,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 2",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress3")]
        public JoinDataComplete KeypadPress3 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 24,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 3",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress4")]
        public JoinDataComplete KeypadPress4 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 25,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 4",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress5")]
        public JoinDataComplete KeypadPress5 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 26,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 5",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress6")]
        public JoinDataComplete KeypadPress6 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 27,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 6",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress7")]
        public JoinDataComplete KeypadPress7 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 28,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 7",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress8")]
        public JoinDataComplete KeypadPress8 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 29,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 8",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("KeypadPress9")]
        public JoinDataComplete KeypadPress9 = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 30,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Keypad 9",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("ChannelUp")]
        public JoinDataComplete ChannelUp = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 31,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Channel Up",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("ChannelDown")]
        public JoinDataComplete ChannelDown = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 32,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Channel Down",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("Guide")]
        public JoinDataComplete Guide = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 33,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Guide",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        [JoinName("Last")]
        public JoinDataComplete Last = new JoinDataComplete(
           new JoinData
           {
               JoinNumber = 34,
               JoinSpan = 1
           },
           new JoinMetadata
           {
               Description = "Last Channel",
               JoinCapabilities = eJoinCapabilities.FromSIMPL,
               JoinType = eJoinType.Digital
           });

        //[JoinName("InputSelectOffset")]
        //public JoinDataComplete InputSelectOffset = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 11,
        //        JoinSpan = 10
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Input Select",
        //        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
        //        JoinType = eJoinType.Digital
        //    });

        //[JoinName("ButtonVisibilityOffset")]
        //public JoinDataComplete ButtonVisibilityOffset = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 41,
        //        JoinSpan = 10
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Button Visibility Offset",
        //        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
        //        JoinType = eJoinType.DigitalSerial
        //    });

        //[JoinName("IsOnline")]
        //public JoinDataComplete IsOnline = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 50,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Is Online",
        //        JoinCapabilities = eJoinCapabilities.ToSIMPL,
        //        JoinType = eJoinType.Digital
        //    });

        #endregion


        #region Analogs

        //[JoinName("InputSelect")]
        //public JoinDataComplete InputSelect = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 11,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Input Select",
        //        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
        //        JoinType = eJoinType.Analog
        //    });

        #endregion


        #region Serials

        //[JoinName("Name")]
        //public JoinDataComplete Name = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 1,
        //        JoinSpan = 1
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Name",
        //        JoinCapabilities = eJoinCapabilities.ToSIMPL,
        //        JoinType = eJoinType.Serial
        //    });

        //[JoinName("InputNamesOffset")]
        //public JoinDataComplete InputNamesOffset = new JoinDataComplete(
        //    new JoinData
        //    {
        //        JoinNumber = 11,
        //        JoinSpan = 10
        //    },
        //    new JoinMetadata
        //    {
        //        Description = "Input Names Offset",
        //        JoinCapabilities = eJoinCapabilities.ToSIMPL,
        //        JoinType = eJoinType.Serial
        //    });

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="joinStart"></param>
        public LgDisplayBridgeJoinMap(uint joinStart)
            : base(joinStart, typeof(LgDisplayBridgeJoinMap))
        {

        }
}
}