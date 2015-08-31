/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.RemoteControl.Services
{
    using System;
    using System.Windows;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;
    using System.Windows.Input;
    using System.Diagnostics;
    using System.Threading;
    using LaJust.PowerMeter.Common;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    #if DEBUG
    public class DemoService
    {
        #region Private Fields

        private Random _rnd = new Random();
        private Thread _testThread;

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DemoService(IEventAggregator aggregator)
        {
            EventAggregator = aggregator;
            Application.Current.MainWindow.KeyDown += Window_KeyDown;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="keyEvent">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs keyEvent)
        {
            keyEvent.Handled = true;
            switch (keyEvent.Key)
            {
                default:
                    keyEvent.Handled = false;
                    break;

                case Key.D1:
                case Key.D3:
                    if ( (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        EventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Publish(
                            new ReceiverEvents.DeviceStatusUpdate() { 
                                SensorId = "FF-FF-" + keyEvent.Key.ToString(), Status = ReceiverEvents.SensorDeviceStatus.NotResponding 
                            });
                    else
                        EventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Publish(
                            new ReceiverEvents.DeviceRegistered() { SensorId = "FF-FF-" + keyEvent.Key.ToString(), SensorType = ReceiverEvents.SensorDeviceType.Hong });
                    break;

                case Key.D2:
                case Key.D4:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        EventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Publish(
                            new ReceiverEvents.DeviceStatusUpdate() { SensorId = "FF-FF-" + keyEvent.Key.ToString(), Status = ReceiverEvents.SensorDeviceStatus.NotResponding });
                    else
                        EventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Publish(
                            new ReceiverEvents.DeviceRegistered() { SensorId = "FF-FF-" + keyEvent.Key.ToString(), SensorType = ReceiverEvents.SensorDeviceType.Chung });
                    break;

                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        EventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Publish(
                            new ReceiverEvents.DeviceStatusUpdate() { SensorId = "FF-FF-" + keyEvent.Key.ToString(), Status = ReceiverEvents.SensorDeviceStatus.NotResponding });
                    else
                        EventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Publish(
                            new ReceiverEvents.DeviceRegistered() { SensorId = "FF-FF-" + keyEvent.Key.ToString(), SensorType = ReceiverEvents.SensorDeviceType.Target });
                    break;

                case Key.D0:
                    EventAggregator.GetEvent<ProcessEvent>().Publish(ProcessEventType.SystemResumed);
                    break;

                // Demo impacts
                case Key.Q:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D1", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = ReceiverEvents.SensorPanel.Unknown, OpCode = "Demo" });
                    break;

                case Key.W:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D2", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = ReceiverEvents.SensorPanel.Unknown, OpCode = "Demo" });
                    break;

                case Key.E:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D3", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = ReceiverEvents.SensorPanel.Unknown, OpCode = "Demo" });
                    break;

                case Key.R:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D4", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = ReceiverEvents.SensorPanel.Unknown, OpCode = "Demo" });
                    break;

                case Key.T:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D5", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = (ReceiverEvents.SensorPanel)_rnd.Next(1, 6), OpCode = "Demo" });
                    break;

                case Key.Y:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D6", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = (ReceiverEvents.SensorPanel)_rnd.Next(1, 6), OpCode = "Demo" });
                    break;

                case Key.U:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D7", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = (ReceiverEvents.SensorPanel)_rnd.Next(1, 6), OpCode = "Demo" });
                    break;

                case Key.I:
                    EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(
                        new ReceiverEvents.SensorHit() { SensorId = "FF-FF-D8", ImpactLevel = (byte)_rnd.Next(10, 100), Panel = (ReceiverEvents.SensorPanel)_rnd.Next(1, 6), OpCode = "Demo" });
                    break;

                //case Key.D0:
                //    if (_testThread == null || _testThread.IsAlive == false)
                //    {
                //        _testThread = new Thread(RunTest);
                //        _testThread.IsBackground = true;
                //        _testThread.Start();
                //    }
                //    break;
            }
        }

        #endregion

        private void RunTest()
        {
        }
    }
    #endif
}
