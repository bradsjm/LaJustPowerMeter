/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.EIDSS.Communications.Entities
{
    using System;
    using LaJust.EIDSS.Communications.Hardware;

    /// <summary>
    /// Event data to encapsulate a panel button press
    /// </summary>
    public class PanelButtonEventData : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <value>The receiver id.</value>
        public IReceiver Receiver { get; set; }
        /// <summary>
        /// Gets or sets the button pressed.
        /// </summary>
        /// <value>The button.</value>
        public PanelButtons Button { get; set; }
    }

    /// <summary>
    /// Receiver Panel Buttons
    /// </summary>
    public enum PanelButtons : byte
    {
        Unknown = 0x00,
        Start = 0xf0,
        Clocking = 0xf1,
        TimeLimit = 0xf2,
        ChungWin = 0xff,
        HongWin = 0xf3,
        ChungRegister = 0x3f,
        HongRegister = 0x5f,
        ChungScoreMinus = 0x31,
        ChungScorePlus = 0x32,
        ChungWarningMinus = 0x21,
        ChungWarningPlus = 0x22,
        HongScoreMinus = 0x51,
        HongScorePlus = 0x52,
        HongWarningMinus = 0x41,
        HongWarningPlus = 0x42,
        ChungTimeLimit = 0xd1,
        HongTimeLimit = 0xd2
    }

}
