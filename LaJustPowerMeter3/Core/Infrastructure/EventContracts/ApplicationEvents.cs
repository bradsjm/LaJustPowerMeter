// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationEvents.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Application Events Container Class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;

    using Microsoft.Practices.Prism.Events;

    /// <summary>
    /// Application Events Container Class
    /// </summary>
    public static class ApplicationEvents
    {
        /// <summary>
        /// The application closing.
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class ApplicationClosing : CompositePresentationEvent<EventArgs>
        {
        }

        /// <summary>
        /// Requests notification to user (usually through a modal message box)
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class NotifyUserEvent : CompositePresentationEvent<NotifyUserEvent>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the buttons.
            /// </summary>
            /// <value>The buttons.</value>
            public MessageBoxButton Buttons { get; set; }

            /// <summary>
            /// Gets or sets the default result.
            /// </summary>
            /// <value>The default result.</value>
            public MessageBoxResult DefaultResult { get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            /// <value>The image.</value>
            public MessageBoxImage Image { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            /// <value>The message.</value>
            public string Message { get; set; }

            /// <summary>
            /// Gets or sets the result handler.
            /// </summary>
            /// <value>The result handler.</value>
            public Action<MessageBoxResult> ResultHandler { get; set; }

            #endregion
        }

        /// <summary>
        /// Published to indicate the application is busy or has finished being busy,
        /// normally surfaced using a busy icon by the shell.
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class ShowBusyEvent : CompositePresentationEvent<bool>
        {
        }

        /// <summary>
        /// Informs subscribers of system events such as when all modules have initialized, shutdown etc.
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class ShowOverlay : CompositePresentationEvent<string>
        {
        }
    }
}