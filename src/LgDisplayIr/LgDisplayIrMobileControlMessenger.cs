using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;

namespace PepperDash.Essentials.Plugins.Lg.Display
{
    public class LgDisplayIrMobileControlMessenger : MessengerBase
    {
        private readonly LgDisplayIrController device;
        public LgDisplayIrMobileControlMessenger(string key, string messagePath, LgDisplayIrController device)
                : base(key, messagePath, device)
        {
            this.device = device;
            Debug.LogInformation("Constructing messenger for {0}", device.Key);
        }

        protected override void RegisterActions()
        {
            Debug.LogInformation("Registering actions for {0}", device.Key);

            // Register action to send IR command
            // - this is composited with the /device/{device-key} path to handle the correct message. ie, the full path from the frontend is /device/{device-key}/irCommand. The path in the constructor MUST follow the basic pattern defined there
            // AddAction("/powerToggle", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.PowerTogglePress));
            // AddAction("/irCommand/powerOn", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.PowerOnPress));
            // AddAction("/irCommand/powerOff", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.PowerOffPress));
            // AddAction("/irCommand/hdmi1", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputHdmi1));
            // AddAction("/irCommand/hdmi2", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputHdmi2));
            // AddAction("/irCommand/hdmi3", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputHdmi3));
            // AddAction("/irCommand/hdmi4", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputHdmi4));
            // AddAction("/irCommand/tv", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputTv));
            // AddAction("/irCommand/antenna", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputAntenna));
            // AddAction("/irCommand/netflix", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputNetflix));
            // AddAction("/irCommand/primeVideo", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.InputPrimeVideo));
            AddAction("/volumeUp", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.VolumeUp));
            AddAction("/volumeDown", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.VolumeDown));
            AddAction("/muteToggle", (id, content) => device.MuteToggle());

            // Tuner/Channel Control Actions
            AddAction("/keypad/0", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress0));
            AddAction("/keypad/1", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress1));
            AddAction("/keypad/2", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress2));
            AddAction("/keypad/3", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress3));
            AddAction("/keypad/4", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress4));
            AddAction("/keypad/5", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress5));
            AddAction("/keypad/6", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress6));
            AddAction("/keypad/7", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress7));
            AddAction("/keypad/8", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress8));
            AddAction("/keypad/9", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.KeypadPress9));
            AddAction("/channelUp", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.ChannelUp));
            AddAction("/channelDown", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.ChannelDown));
            AddAction("/guide", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.Guide));
            AddAction("/last", (id, content) => PressAndHoldHandler.HandlePressAndHold(DeviceKey, content, device.Last));
        }
    }
}

