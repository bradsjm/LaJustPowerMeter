// <copyright file="SpeechService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace MultiMedia
{
    using System.Speech.Synthesis;
    using Infrastructure;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class SpeechService
    {
        #region Protected Fields

        /// <summary>
        /// Windows Speach Synthesizer
        /// </summary>
        private readonly SpeechSynthesizer SpeechSynthesizer = new SpeechSynthesizer();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechService"/> class.
        /// </summary>
        public SpeechService()
        {
            this.SpeechSynthesizer.Volume = 100; // Maximum
            this.SpeechSynthesizer.Rate = 2; // Increase speed
        }

        #endregion

        /// <summary>
        /// Speaks the text.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        public void SpeakText(string text)
        {
            lock (this.SpeechSynthesizer)
            {
                this.SpeechSynthesizer.SpeakAsyncCancelAll();
                this.SpeechSynthesizer.SpeakAsync(text);
            }
        }
    }
}