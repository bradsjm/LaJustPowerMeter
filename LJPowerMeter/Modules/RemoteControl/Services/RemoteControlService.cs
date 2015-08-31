/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
using System;
 */
namespace LaJust.PowerMeter.Modules.RemoteControl.Services
{
    using System.Windows;
    using System.Windows.Input;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;

    /// <summary>
    /// Remote Control Service for the PowerMeter application
    /// </summary>
    public class RemoteControlService
    {
        #region Private Properties

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public RemoteControlService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Application.Current.MainWindow.KeyDown += Window_KeyDown;
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Play, MediaCommands_Play));
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Stop, MediaCommands_Stop));
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Record, MediaCommands_Record));
        }

        #endregion

        #region Event Handler Methods

        /// <summary>
        /// Handles the Play event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void MediaCommands_Play(object sender, ExecutedRoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Start);
        }

        /// <summary>
        /// Handles the Stop event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void MediaCommands_Stop(object sender, ExecutedRoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Stop);
        }

        /// <summary>
        /// Handles the Record event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void MediaCommands_Record(object sender, ExecutedRoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.RegisterTarget);
        }

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs keyEvent)
        {
            // Do not intercept key presses in text boxes
            if (keyEvent.OriginalSource is System.Windows.Controls.TextBox)
                return;

            keyEvent.Handled = true;

            switch (keyEvent.Key)
            {
                default:
                    keyEvent.Handled = false;
                    break;

                case Key.B:                 // PowerPoint presenters
                case Key.OemPeriod:
                    _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Stop);
                    break;

                case Key.F5:                // PowerPoint presenters use F5/Escape
                case Key.Escape:
                    _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Start);
                    break;

                case Key.Right:
                case Key.Next:
                    _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Right);
                    break;

                case Key.Left:
                case Key.Prior:
                    _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Publish(RemoteEvents.Buttons.Left);
                    break;
            }
        }

        #endregion
    }
}
