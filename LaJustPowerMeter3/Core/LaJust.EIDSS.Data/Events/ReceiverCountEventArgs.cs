// <copyright file="ReceiverCountEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Event triggered when a receiver is detected added or removed
    /// </summary>
    public class ReceiverCountEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the current count of receivers.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }
    }
}
