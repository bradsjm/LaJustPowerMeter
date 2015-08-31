// <copyright file="VideoModule.cs" company="LaJust Sports America">
// Copyright (c) 2011 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Network
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Infrastructure;
    using InterAppComms;
    using InterAppComms.Contracts;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Network module handles connectivity to other network devices
    /// </summary>
    [ModuleExport(typeof(VideoModule))]
    public sealed class VideoModule : IModule
    {
        /// <summary>
        /// Application configuration settings
        /// </summary>
        private readonly IScoreKeeperService scoreKeeperService;

        private readonly IStopWatchService stopWatchService;

        private PipeSender<ICommandService> pipeCommandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public VideoModule(
            IScoreKeeperService scoreKeeperService,
            IStopWatchService stopWatchService)
        {
            this.scoreKeeperService = scoreKeeperService;
            this.stopWatchService = stopWatchService;
            this.pipeCommandService = null;
        }

        /// <summary>
        /// Notifies the module to initialize.
        /// </summary>
        public void Initialize()
        {
            if (this.StartVideoCapture())
            {
                this.stopWatchService.OnChanged(p => p.IsRunning).Do( 
                    o => this.NotifyVideoCapture("RECORD", o.IsRunning.ToString()));
            }
        }

        /// <summary>
        /// Starts the video capture application.
        /// </summary>
        private bool StartVideoCapture()
        {
            if (this.GetVideoCaptureProcess() != null)
            {
                return true;
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                        {
                            FileName = GetVideoCaptureExe(),
                            WindowStyle = ProcessWindowStyle.Minimized
                        });
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("VideoModule: " + ex.GetBaseException());
                }
                return false;
            }
        }

        /// <summary>
        /// Notifies the video capture process.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="p">The p.</param>
        private void NotifyVideoCapture(string command, params string[] p)
        {
            Process videoCaptureProcess = this.GetVideoCaptureProcess();
            if (videoCaptureProcess != null && videoCaptureProcess.Responding)
            {
                try
                {
                    if (this.pipeCommandService == null)
                    {
                        this.pipeCommandService = new PipeSender<ICommandService>();
                    }
                    this.pipeCommandService.Instance.Execute(command, p);
                }
                catch (Exception ex) 
                {
                    System.Diagnostics.Debug.WriteLine("VideoModule: " + ex.GetBaseException());
                    this.pipeCommandService = null;
                }
            }
        }

        /// <summary>
        /// Gets the video capture process.
        /// </summary>
        /// <returns></returns>
        private Process GetVideoCaptureProcess()
        {
            Process videoCaptureProcess;

            try
            {
                videoCaptureProcess =
                    Process.GetProcessesByName("VideoCapture").First(
                        p => p.MainModule.FileName.ToLower().Equals(GetVideoCaptureExe()));
                return videoCaptureProcess;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets the video capture exe location.
        /// </summary>
        /// <returns></returns>
        private string GetVideoCaptureExe()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return Path.Combine(appPath, "videocapture.exe").ToLower();
        }
    }
}
