using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Epi.Display.Lg;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using DisplayBase = PepperDash.Essentials.Devices.Common.Displays.DisplayBase;


namespace PepperDash.Essentials.Plugins.Lg.Display
{
    public class LgDisplayIrController : DisplayBase, IBasicVolumeControls, IBridgeAdvanced, IHasInputs<string>
    {
        private readonly LgDisplayPropertiesConfig propertiesConfig;

        private IrOutputPortController irController;

        public const int InputPowerOn = 101;
        public const int InputPowerOff = 102;

        public ISelectableItems<string> Inputs { get; private set; }


        private bool isWarmingUp;
        public bool IsWarmingUp
        {
            get { return isWarmingUp; }
            set
            {
                isWarmingUp = value;
                IsWarmingUpFeedback.FireUpdate();
            }
        }

        private bool isCoolingDown;
        public bool IsCoolingDown
        {
            get { return isCoolingDown; }
            set
            {
                isCoolingDown = value;
                IsCoolingDownFeedback.FireUpdate();
            }
        }


        public LgDisplayIrController(string key, string name, LgDisplayPropertiesConfig config, IrOutputPortController irController)
            : base(key, name)
        {
            this.propertiesConfig = config;
            if (propertiesConfig == null)
            {
                Debug.LogError(this, "Display configuration must be included");
                return;
            }

            this.irController = irController;
            if (this.irController == null)
            {
                Debug.LogError(this, "IrOutputPortController instance must be included");
                return;
            }

            SetupInputs();


            DeviceManager.AddDevice(irController);

            CooldownTime = propertiesConfig.coolingTimeMs > 0 ? propertiesConfig.coolingTimeMs : 10000;
            WarmupTime = propertiesConfig.warmingTimeMs > 0 ? propertiesConfig.warmingTimeMs : 8000;
        }


        // protected override void CreateMobileControlMessengers()
        // {
        //     var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();
        //     if (mc == null)
        //     {
        //         Debug.LogInformation("Mobile Control not found");
        //         return;
        //     }

        //     var messenger = new LgDisplayIrMobileControlMessenger($"{Key}", $"/device/{Key}", this);
        //     mc.AddDeviceMessenger(messenger);
        // }


        #region IBridgeAdvanced Members

        public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new LgDisplayBridgeJoinMap(joinStart);

            // This adds the join map to the collection on the bridge
            if (bridge != null)
            {
                bridge.AddJoinMap(Key, joinMap);
            }

            var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);
            if (customJoins != null)
            {
                joinMap.SetCustomJoinData(customJoins);
            }

            Debug.LogInformation(this, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));
            Debug.LogInformation(this, "Linking to Bridge Type {0}", GetType().Name);

            // links to bridge
            // device name
            trilist.SetString(joinMap.Name.JoinNumber, Name);

            // power off/on
            trilist.SetSigTrueAction(joinMap.PowerOff.JoinNumber, PowerOff);
            trilist.SetSigTrueAction(joinMap.PowerOn.JoinNumber, PowerOn);

            // warming/cooling feedback
            IsWarmingUpFeedback.LinkInputSig(trilist.BooleanInput[joinMap.IsWarmingUp.JoinNumber]);
            IsCoolingDownFeedback.LinkInputSig(trilist.BooleanInput[joinMap.IsCoolingDown.JoinNumber]);

            // input (digital select, digital feedback, names)
            for (var i = 0; i < InputPorts.Count; i++)
            {
                var inputIndex = i;
                var input = InputPorts.ElementAt(inputIndex);

                if (input == null) continue;

                trilist.SetSigTrueAction((ushort)(joinMap.InputSelectOffset.JoinNumber + inputIndex), () =>
                {
                    ExecuteSwitch(GetInputPort(inputIndex + 1).Selector);
                    //SetInput = inputIndex + 1;
                });

                trilist.StringInput[(ushort)(joinMap.InputNamesOffset.JoinNumber + inputIndex)].StringValue = string.IsNullOrEmpty(input.Key) ? string.Empty : input.Key;

            }

            // input (analog select)
            trilist.SetUShortSigAction(joinMap.InputSelect.JoinNumber, analogValue =>
            {
                ExecuteSwitch(GetInputPort(analogValue).Selector);
                //SetInput = analogValue;
            });

            // Tuner/Channel controls
            trilist.SetSigTrueAction(joinMap.KeypadPress0.JoinNumber, () => KeypadPress0());
            trilist.SetSigTrueAction(joinMap.KeypadPress1.JoinNumber, () => KeypadPress1());
            trilist.SetSigTrueAction(joinMap.KeypadPress2.JoinNumber, () => KeypadPress2());
            trilist.SetSigTrueAction(joinMap.KeypadPress3.JoinNumber, () => KeypadPress3());
            trilist.SetSigTrueAction(joinMap.KeypadPress4.JoinNumber, () => KeypadPress4());
            trilist.SetSigTrueAction(joinMap.KeypadPress5.JoinNumber, () => KeypadPress5());
            trilist.SetSigTrueAction(joinMap.KeypadPress6.JoinNumber, () => KeypadPress6());
            trilist.SetSigTrueAction(joinMap.KeypadPress7.JoinNumber, () => KeypadPress7());
            trilist.SetSigTrueAction(joinMap.KeypadPress8.JoinNumber, () => KeypadPress8());
            trilist.SetSigTrueAction(joinMap.KeypadPress9.JoinNumber, () => KeypadPress9());
            trilist.SetSigTrueAction(joinMap.ChannelUp.JoinNumber, () => ChannelUp());
            trilist.SetSigTrueAction(joinMap.ChannelDown.JoinNumber, () => ChannelDown());
            trilist.SetSigTrueAction(joinMap.Guide.JoinNumber, () => Guide());
            trilist.SetSigTrueAction(joinMap.Last.JoinNumber, () => Last());


            // bridge online change
            trilist.OnlineStatusChange += (sender, args) =>
            {
                if (!args.DeviceOnLine) return;

                // device name
                trilist.SetString(joinMap.Name.JoinNumber, Name);
            };
        }

        #endregion


        public void SendIrCommand(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                Debug.LogError(this, "SendIrCommand: ir command is null or empty");
                return;
            }

            Debug.LogInformation(this, "SendIrCommand: ir command '{0}'", cmd);

            irController?.PressRelease(cmd, true);
            irController?.PressRelease(cmd, false);
        }



        #region  Power Members

        /// <summary>
        /// Set Power On For Device
        /// </summary>
        public override void PowerOn()
        {
            Debug.LogInformation(this, "PowerOn: ir command '{0}'", IrStandardCommands.PowerOn);
            SendIrCommand(IrStandardCommands.PowerOn);
        }

        /// <summary>
        /// Set Power On for Device on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void PowerOnPress(bool pressRelease)
        {
            if (pressRelease) return;
            PowerOn();
        }


        /// <summary>
        /// Set Power Off for Device
        /// </summary>
        public override void PowerOff()
        {
            Debug.LogInformation(this, "PowerOff: ir command '{0}'", IrStandardCommands.PowerOff);
            SendIrCommand(IrStandardCommands.PowerOff);
        }

        /// <summary>
        /// Set Power Off for Device on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void PowerOffPress(bool pressRelease)
        {
            if (pressRelease) return;
            PowerOff();
        }

        /// <summary>
        /// Toggle current power state for device
        /// </summary>
        public override void PowerToggle()
        {
            Debug.LogInformation(this, "PowerToggle: ir command '{0}'", IrStandardCommands.PowerToggle);
            SendIrCommand(IrStandardCommands.PowerToggle);
        }

        /// <summary>
        /// Toggle current power state for device on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void PowerTogglePress(bool pressRelease)
        {
            if (pressRelease) return;
            PowerToggle();
        }

        protected override Func<bool> IsCoolingDownFeedbackFunc
        {
            get { return () => IsCoolingDown; }
        }

        protected override Func<bool> IsWarmingUpFeedbackFunc
        {
            get { return () => IsWarmingUp; }
        }

        #endregion



        #region Input Members

        private void AddRoutingInputPort(RoutingInputPort port, string fbMatch)
        {
            port.FeedbackMatchObject = fbMatch;
            InputPorts.Add(port);
        }

        private RoutingInputPort GetInputPort(int input)
        {
            return InputPorts.ElementAt(input);
        }

        /// <summary>
        /// Lists available input routing ports
        /// </summary>
        public void ListRoutingInputPorts()
        {
            foreach (var inputPort in InputPorts)
            {
                Debug.LogVerbose(this, "ListRoutingInputPorts: key-'{0}', connectionType-'{1}', feedbackMatchObject-'{2}'",
                    inputPort.Key, inputPort.ConnectionType, inputPort.FeedbackMatchObject);
            }
        }


        private void SetupInputs()
        {
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.HdmiIn1, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Hdmi, new Action(InputHdmi1), this), IrStandardCommands.InputHdmi1);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.HdmiIn2, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Hdmi, new Action(InputHdmi2), this), IrStandardCommands.InputHdmi2);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.HdmiIn3, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Hdmi, new Action(InputHdmi3), this), IrStandardCommands.InputHdmi3);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.HdmiIn4, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Hdmi, new Action(InputHdmi4), this), IrStandardCommands.InputHdmi4);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.AnyVideoIn, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Composite, new Action(InputTv), this), IrStandardCommands.InputTv);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.AntennaIn, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.None, new Action(InputAntenna), this), IrStandardCommands.InputAntenna);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.AnyVideoIn, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Streaming, new Action(InputNetflix), this), IrStandardCommands.Netflix);
            AddRoutingInputPort(
                new RoutingInputPort(RoutingPortNames.AnyVideoIn, eRoutingSignalType.Audio | eRoutingSignalType.Video,
                    eRoutingPortConnectionType.Streaming, new Action(InputPrimeVideo), this), IrStandardCommands.PrimeVideo);

            Inputs = new LgDisplayIrInputs
            {
                Items = new Dictionary<string, ISelectableItem>
                {
                    { "hdmi1", new LgDisplayIrInput("hdmi1", "HDMI 1", this) },
                    { "hdmi2", new LgDisplayIrInput("hdmi2", "HDMI 2", this) },
                    { "hdmi3", new LgDisplayIrInput("hdmi3", "HDMI 3", this) },
                    { "hdmi4", new LgDisplayIrInput("hdmi4", "HDMI 4", this) },
                    { "tv", new LgDisplayIrInput("tv", "TV", this) },
                    { "antenna", new LgDisplayIrInput("antenna", "Antenna", this) },
                    { "netflix", new LgDisplayIrInput("netflix", "Netflix", this) },
                    { "primeVideo", new LgDisplayIrInput("primeVideo", "Prime Video", this) }
                }
            };

            UpdateInputFriendlyNames(propertiesConfig);
        }
        private void UpdateInputFriendlyNames(LgDisplayPropertiesConfig config)
        {
            if (config?.FriendlyNames == null || Inputs?.Items == null)
                return;

            foreach (var item in config.FriendlyNames)
            {
                Debug.LogInformation(this, $"UpdateInputFriendlyNames: key '{item.InputKey}', name '{item.Name}', hideInput '{item.HideInput}'");

                if (string.IsNullOrEmpty(item.InputKey))
                {
                    Debug.LogError(this, "UpdateInputFriendlyNames: InputKey is null or empty");
                    continue;
                }

                if (item.HideInput)
                {
                    Inputs.Items.Remove(item.InputKey);
                }
                else if (Inputs.Items.TryGetValue(item.InputKey, out var inputItem))
                {
                    var updatedInputItem = new LgDisplayIrInput(item.InputKey, item.Name, this);
                    Inputs.Items[item.InputKey] = updatedInputItem;
                }
            }
        }

        /// <summary>
        /// Select Hdmi 1 Input
        /// </summary>
        public void InputHdmi1()
        {
            Debug.LogInformation(this, "InputHdmi1: ir command '{0}'", IrStandardCommands.InputHdmi1);
            SendIrCommand(IrStandardCommands.InputHdmi1);
        }

        /// <summary>
        /// Select Hdmi 1 Input on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputHdmi1(bool pressRelease)
        {
            if (pressRelease) return;
            InputHdmi1();
        }

        /// <summary>
        /// Select Hdmi 2 Input
        /// </summary>
        public void InputHdmi2()
        {
            Debug.LogInformation(this, "InputHdmi2: ir command '{0}'", IrStandardCommands.InputHdmi2);
            SendIrCommand(IrStandardCommands.InputHdmi2);
        }

        /// <summary>
        /// Select Hdmi 2 Input on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputHdmi2(bool pressRelease)
        {
            if (pressRelease) return;
            InputHdmi2();
        }

        /// <summary>
        /// Select Hdmi 3 Input
        /// </summary>
        public void InputHdmi3()
        {
            Debug.LogInformation(this, "InputHdmi3: ir command '{0}'", IrStandardCommands.InputHdmi3);
            SendIrCommand(IrStandardCommands.InputHdmi3);
        }

        /// <summary>
        /// Select Hdmi 3 Input on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputHdmi3(bool pressRelease)
        {
            if (pressRelease) return;
            InputHdmi3();
        }

        /// <summary>
        /// Select Hdmi 4 Input
        /// </summary>
        public void InputHdmi4()
        {
            Debug.LogInformation(this, "InputHdmi4: ir command '{0}'", IrStandardCommands.InputHdmi4);
            SendIrCommand(IrStandardCommands.InputHdmi4);
        }

        /// <summary>
        /// Select Hdmi 4 Input on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputHdmi4(bool pressRelease)
        {
            if (pressRelease) return;
            InputHdmi4();
        }

        /// <summary>
        /// Select Tv
        /// </summary>
        public void InputTv()
        {
            Debug.LogInformation(this, "InputTv: ir command '{0}'", IrStandardCommands.InputTv);
            SendIrCommand(IrStandardCommands.InputTv);
        }

        /// <summary>
        /// Select Tv on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputTv(bool pressRelease)
        {
            if (pressRelease) return;
            InputTv();
        }

        /// <summary>
        /// Select Antenna
        /// </summary>
        public void InputAntenna()
        {
            Debug.LogInformation(this, "InputAntenna: ir command '{0}'", IrStandardCommands.InputAntenna);
            SendIrCommand(IrStandardCommands.InputAntenna);
        }

        /// <summary>
        /// Select Antenna on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputAntenna(bool pressRelease)
        {
            if (pressRelease) return;
            InputAntenna();
        }

        /// <summary>
        /// Select Netflix
        /// </summary>
        public void InputNetflix()
        {
            Debug.LogInformation(this, "InputNetflix: ir command '{0}'", IrStandardCommands.Netflix);
            SendIrCommand(IrStandardCommands.Netflix);
        }

        /// <summary>
        /// Select Netflix on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputNetflix(bool pressRelease)
        {
            if (pressRelease) return;
            InputNetflix();
        }

        /// <summary>
        /// Select Amazon Prime Video
        /// </summary>
        public void InputPrimeVideo()
        {
            Debug.LogInformation(this, "InputPrimeVideo: ir command '{0}'", IrStandardCommands.PrimeVideo);
            SendIrCommand(IrStandardCommands.PrimeVideo);
        }

        /// <summary>
        /// Select Amazon Prime Video on press
        /// </summary>
        /// <param name="pressRelease"></param>
        public void InputPrimeVideo(bool pressRelease)
        {
            if (pressRelease) return;
            InputPrimeVideo();
        }

        public void InputToggle()
        {
            Debug.LogInformation(this, "InputToggle: ir command '{0}'", IrStandardCommands.InputToggle);
            SendIrCommand(IrStandardCommands.InputToggle);
        }

        public void InputToggle(bool pressRelease)
        {
            if (pressRelease) return;
            InputToggle();
        }

        /// <summary>
        /// Executes a switch, turning on display if necessary.
        /// </summary>
        /// <param name="selector"></param>
        public override void ExecuteSwitch(object selector)
        {
            Debug.LogInformation(this, $"ExecuteSwitch: selector '{selector}' type '{selector?.GetType().Name ?? "null"}'");



            string cmd = null;

            if (selector is RoutingInputPort port)
            {
                cmd = port.FeedbackMatchObject as string;
                if (string.IsNullOrEmpty(cmd))
                {
                    Debug.LogError(this, "ExecuteSwitch: command not found for input port '{0}'", port.Key);
                    return;
                }
            }
            else if (selector is string strCmd)
            {
                cmd = strCmd;
                if (string.IsNullOrEmpty(cmd))
                {
                    Debug.LogError(this, "ExecuteSwitch: selector is null or empty");
                    return;
                }
            }
            else if (selector is int intCmd)
            {
                cmd = intCmd.ToString();
            }
            else if (selector is ushort ushortCmd)
            {
                cmd = ushortCmd.ToString();
            }
            else
            {
                cmd = selector?.ToString();
                if (string.IsNullOrEmpty(cmd))
                {
                    Debug.LogError(this, "ExecuteSwitch: selector is null or empty");
                    return;
                }
            }

            // if already on, just send command
            SendIrCommand(cmd);


            // if warming up, wait for warmup to complete before sending command
            EventHandler<FeedbackEventArgs> handler = null; // necessary to allow reference inside lambda to handler
            handler = (o, a) =>
            {
                if (isWarmingUp)
                {
                    return;
                }

                IsWarmingUpFeedback.OutputChange -= handler;

                SendIrCommand(cmd);

            };
            IsWarmingUpFeedback.OutputChange += handler; // attach and wait for on FB
            PowerOn();
        }

        #endregion


        #region Tuner/Channel Control Members

        /// <summary>
        /// Send numeric keypad 0
        /// </summary>
        public void KeypadPress0()
        {
            Debug.LogInformation(this, "KeypadPress0: ir command '{0}'", IrStandardCommands.KP0);
            SendIrCommand(IrStandardCommands.KP0);
        }

        /// <summary>
        /// Send numeric keypad 0 on press
        /// </summary>
        public void KeypadPress0(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress0();
        }

        /// <summary>
        /// Send numeric keypad 1
        /// </summary>
        public void KeypadPress1()
        {
            Debug.LogInformation(this, "KeypadPress1: ir command '{0}'", IrStandardCommands.KP1);
            SendIrCommand(IrStandardCommands.KP1);
        }

        /// <summary>
        /// Send numeric keypad 1 on press
        /// </summary>
        public void KeypadPress1(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress1();
        }

        /// <summary>
        /// Send numeric keypad 2
        /// </summary>
        public void KeypadPress2()
        {
            Debug.LogInformation(this, "KeypadPress2: ir command '{0}'", IrStandardCommands.KP2);
            SendIrCommand(IrStandardCommands.KP2);
        }

        /// <summary>
        /// Send numeric keypad 2 on press
        /// </summary>
        public void KeypadPress2(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress2();
        }

        /// <summary>
        /// Send numeric keypad 3
        /// </summary>
        public void KeypadPress3()
        {
            Debug.LogInformation(this, "KeypadPress3: ir command '{0}'", IrStandardCommands.KP3);
            SendIrCommand(IrStandardCommands.KP3);
        }

        /// <summary>
        /// Send numeric keypad 3 on press
        /// </summary>
        public void KeypadPress3(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress3();
        }

        /// <summary>
        /// Send numeric keypad 4
        /// </summary>
        public void KeypadPress4()
        {
            Debug.LogInformation(this, "KeypadPress4: ir command '{0}'", IrStandardCommands.KP4);
            SendIrCommand(IrStandardCommands.KP4);
        }

        /// <summary>
        /// Send numeric keypad 4 on press
        /// </summary>
        public void KeypadPress4(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress4();
        }

        /// <summary>
        /// Send numeric keypad 5
        /// </summary>
        public void KeypadPress5()
        {
            Debug.LogInformation(this, "KeypadPress5: ir command '{0}'", IrStandardCommands.KP5);
            SendIrCommand(IrStandardCommands.KP5);
        }

        /// <summary>
        /// Send numeric keypad 5 on press
        /// </summary>
        public void KeypadPress5(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress5();
        }

        /// <summary>
        /// Send numeric keypad 6
        /// </summary>
        public void KeypadPress6()
        {
            Debug.LogInformation(this, "KeypadPress6: ir command '{0}'", IrStandardCommands.KP6);
            SendIrCommand(IrStandardCommands.KP6);
        }

        /// <summary>
        /// Send numeric keypad 6 on press
        /// </summary>
        public void KeypadPress6(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress6();
        }

        /// <summary>
        /// Send numeric keypad 7
        /// </summary>
        public void KeypadPress7()
        {
            Debug.LogInformation(this, "KeypadPress7: ir command '{0}'", IrStandardCommands.KP7);
            SendIrCommand(IrStandardCommands.KP7);
        }

        /// <summary>
        /// Send numeric keypad 7 on press
        /// </summary>
        public void KeypadPress7(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress7();
        }

        /// <summary>
        /// Send numeric keypad 8
        /// </summary>
        public void KeypadPress8()
        {
            Debug.LogInformation(this, "KeypadPress8: ir command '{0}'", IrStandardCommands.KP8);
            SendIrCommand(IrStandardCommands.KP8);
        }

        /// <summary>
        /// Send numeric keypad 8 on press
        /// </summary>
        public void KeypadPress8(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress8();
        }

        /// <summary>
        /// Send numeric keypad 9
        /// </summary>
        public void KeypadPress9()
        {
            Debug.LogInformation(this, "KeypadPress9: ir command '{0}'", IrStandardCommands.KP9);
            SendIrCommand(IrStandardCommands.KP9);
        }

        /// <summary>
        /// Send numeric keypad 9 on press
        /// </summary>
        public void KeypadPress9(bool pressRelease)
        {
            if (pressRelease) return;
            KeypadPress9();
        }

        /// <summary>
        /// Channel Up
        /// </summary>
        public void ChannelUp()
        {
            Debug.LogInformation(this, "ChannelUp: ir command '{0}'", IrStandardCommands.ChannelUp);
            SendIrCommand(IrStandardCommands.ChannelUp);
        }

        /// <summary>
        /// Channel Up on press
        /// </summary>
        public void ChannelUp(bool pressRelease)
        {
            if (pressRelease) return;
            ChannelUp();
        }

        /// <summary>
        /// Channel Down
        /// </summary>
        public void ChannelDown()
        {
            Debug.LogInformation(this, "ChannelDown: ir command '{0}'", IrStandardCommands.ChannelDown);
            SendIrCommand(IrStandardCommands.ChannelDown);
        }

        /// <summary>
        /// Channel Down on press
        /// </summary>
        public void ChannelDown(bool pressRelease)
        {
            if (pressRelease) return;
            ChannelDown();
        }

        /// <summary>
        /// Open channel guide
        /// </summary>
        public void Guide()
        {
            Debug.LogInformation(this, "Guide: ir command '{0}'", IrStandardCommands.Guide);
            SendIrCommand(IrStandardCommands.Guide);
        }

        /// <summary>
        /// Open channel guide on press
        /// </summary>
        public void Guide(bool pressRelease)
        {
            if (pressRelease) return;
            Guide();
        }

        /// <summary>
        /// Last channel/input
        /// </summary>
        public void Last()
        {
            Debug.LogInformation(this, "Last: ir command '{0}'", IrStandardCommands.Last);
            SendIrCommand(IrStandardCommands.Last);
        }

        /// <summary>
        /// Last channel/input on press
        /// </summary>
        public void Last(bool pressRelease)
        {
            if (pressRelease) return;
            Last();
        }

        #endregion


        #region Volume Members

        public void VolumeUp(bool pressRelease)
        {
            if (pressRelease) return;

            Debug.LogInformation(this, "VolumeUp: ir command '{0}'", IrStandardCommands.VolumeUp);
            SendIrCommand(IrStandardCommands.VolumeUp);
        }

        public void VolumeDown(bool pressRelease)
        {
            if (pressRelease) return;

            Debug.LogInformation(this, "VolumeDown: ir command '{0}'", IrStandardCommands.VolumeDown);
            SendIrCommand(IrStandardCommands.VolumeDown);
        }

        public void MuteToggle()
        {
            Debug.LogInformation(this, "MuteToggle: ir command '{0}'", IrStandardCommands.MuteToggle);
            SendIrCommand(IrStandardCommands.MuteToggle);
        }

        #endregion
    }
}
