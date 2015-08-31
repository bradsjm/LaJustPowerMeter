// <copyright file="PanelButtons.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Receiver Panel Buttons
    /// </summary>
    public enum PanelButtons : byte
    {
        /// <summary>
        /// Panel Button Unknown
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// Panel Button
        /// </summary>
        Start = 0xf0,

        /// <summary>
        /// Panel Button
        /// </summary>
        Clocking = 0xf1,

        /// <summary>
        /// Panel Button
        /// </summary>
        TimeLimit = 0xf2,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungWin = 0xff,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongWin = 0xf3,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungRegister = 0x3f,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongRegister = 0x5f,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungScoreMinus = 0x31,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungScorePlus = 0x32,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungWarningMinus = 0x21,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungWarningPlus = 0x22,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongScoreMinus = 0x51,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongScorePlus = 0x52,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongWarningMinus = 0x41,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongWarningPlus = 0x42,

        /// <summary>
        /// Panel Button
        /// </summary>
        ChungTimeLimit = 0xd1,

        /// <summary>
        /// Panel Button
        /// </summary>
        HongTimeLimit = 0xd2
    }

    /// <summary>
    /// Event data to encapsulate a panel button press
    /// </summary>
    public class PanelButtonEventData : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <value>The receiver identifier.</value>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Gets or sets the button pressed.
        /// </summary>
        /// <value>The button.</value>
        public PanelButtons Button { get; set; }
    }
}
