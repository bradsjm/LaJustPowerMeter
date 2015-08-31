// <copyright file="SoundEffectsService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace MultiMedia
{
    using System;
    using System.IO;
    using System.Media;
    using System.Reflection;
    using System.Windows;
    using Infrastructure;

    /// <summary>
    /// Sound Effects Service
    /// </summary>
    public class SoundEffectsService
    {
        #region Public Constants

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string BUTTON_CLICK_WAV = "ButtonClick.wav";

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string SENSOR_CONNECTED_WAV = "Connected.wav";

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string SENSOR_LOST_WAV = "Disconnected.wav";

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string CLOCK_START_WAV = "StartTone.wav";

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string CLOCK_END_WAV = "EndTone.wav";

        /// <summary>
        /// Sound File Name
        /// </summary>
        public const string CLOCK_WARNING_WAV = "WarningTone.wav";

        /// <summary>
        /// Sound File Names
        /// </summary>
        public const string UGH_SOUND = "Punch1.wav";

        /// <summary>
        /// Sound File Names
        /// </summary>
        public static readonly string[] HIT_SOUNDS = { "Hit1.wav", "Hit2.wav" };

        #endregion

        #region Protected Fields

        /// <summary>
        /// Windows Sound Player
        /// </summary>
        protected readonly SoundPlayer SoundPlayer = new SoundPlayer();

        /// <summary>
        /// Random Number Generator
        /// </summary>
        protected readonly Random Random = new Random();

        /// <summary>
        /// Sound Directory
        /// </summary>
        protected readonly string SoundDirectory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffectsService"/> class.
        /// </summary>
        public SoundEffectsService()
        {
            this.SoundDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Sounds");
        }

        #endregion

        /// <summary>
        /// Plays the sound resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public void PlaySoundFile(string soundFile)
        {
            lock (this.SoundPlayer)
            {
                this.SoundPlayer.Stop();
                this.SoundPlayer.SoundLocation = Path.Combine(this.SoundDirectory, soundFile);
                this.SoundPlayer.Play();
            };
        }

        /// <summary>
        /// Plays a random one of the sound files provided.
        /// </summary>
        /// <param name="soundFiles">The sound files.</param>
        public void PlaySoundFile(string[] soundFiles)
        {
            this.PlaySoundFile(soundFiles[Random.Next(soundFiles.Length)]);
        }
    }
}