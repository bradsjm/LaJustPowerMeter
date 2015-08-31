// <copyright file="RemoteControlService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ExternalDevices
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;

    /// <summary>
    /// Remote Control Buttons
    /// </summary>
    public enum RemoteButtons
    {
        /// <summary>
        /// An unknown Button
        /// </summary>
        Unknown,

        /// <summary>
        /// The Start Button
        /// </summary>
        Start,

        /// <summary>
        /// The Stop Button
        /// </summary>
        Stop,

        /// <summary>
        /// The Left Button
        /// </summary>
        Right,

        /// <summary>
        /// The Right Button
        /// </summary>
        Left,

        /// <summary>
        /// The Register Target Button
        /// </summary>
        RegisterTarget
    }

    /// <summary>
    /// Remote Control Handler
    /// </summary>
    public class RemoteControlHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteControlHandler"/> class.
        /// </summary>
        /// <param name="app">The app.</param>
        public RemoteControlHandler(Application app)
        {
            app.MainWindow.KeyDown += this.Window_KeyDown;
            app.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Play, this.MediaCommands_Play));
            app.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Stop, this.MediaCommands_Stop));
            app.MainWindow.CommandBindings.Add(new CommandBinding(MediaCommands.Record, this.MediaCommands_Record));
        }

        /// <summary>
        /// Fired when a button is pressed on the remote
        /// </summary>
        public event EventHandler<ButtonPressedEventArgs> ButtonPressed = delegate { };

        #region Event Handler Methods

        /// <summary>
        /// Handles the Play event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        protected void MediaCommands_Play(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Start });
        }

        /// <summary>
        /// Handles the Stop event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        protected void MediaCommands_Stop(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Stop });
        }

        /// <summary>
        /// Handles the Record event of the MediaCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        protected void MediaCommands_Record(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.RegisterTarget });
        }

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="keyEvent">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        protected void Window_KeyDown(object sender, KeyEventArgs keyEvent)
        {
            // Do not intercept key presses in text boxes
            if (!(keyEvent.OriginalSource is System.Windows.Controls.TextBox))
            {
                keyEvent.Handled = true;

                switch (keyEvent.Key)
                {
                    default:
                        keyEvent.Handled = false;
                        break;

                    case Key.B:                 // PowerPoint presenters
                        this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Start });
                        break;

                    case Key.F5:                // PowerPoint presenters use F5/Escape
                    case Key.Escape:
                        this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Stop });
                        break;

                    case Key.Right:
                    case Key.Next:
                        this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Right });
                        break;

                    case Key.Left:
                    case Key.Prior:
                        this.OnButtonPressed(this, new ButtonPressedEventArgs() { ButtonPressed = RemoteButtons.Left });
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Called when [button pressed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Infrastructure.ButtonPressedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            EventHandler<ButtonPressedEventArgs> handler = this.ButtonPressed;
            handler(this, e);
        }

        /// <summary>
        /// Button Pressed Event Args
        /// </summary>
        public class ButtonPressedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the button pressed.
            /// </summary>
            /// <value>The button pressed.</value>
            public RemoteButtons ButtonPressed { get; set; }
        }
    }
}
