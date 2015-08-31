// <copyright file="RosterService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Roster
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.IO;
    using Infrastructure;
    using Microsoft.Practices.Prism.Logging;

    /// <summary>
    /// Roster Service provides operations to retrieve and save rosters
    /// </summary>
    [Export]
    public class RosterService : IDisposable
    {
        /// <summary>
        /// The Composite Application Block Logger
        /// </summary>
        protected readonly ILoggerFacade Logger;

        /// <summary>
        /// The filename used to load and save the competitors
        /// </summary>
        protected const string CompetitorFileName = "competitors.txt";

        /// <summary>
        /// Location of the competitor file (we put it in My Documents\LaJust)
        /// </summary>
        private readonly string DataFileDirectory;

        /// <summary>
        /// Roster list of competitors
        /// </summary>
        private DispatchingObservableCollection<CompetitorModel> competitors = new DispatchingObservableCollection<CompetitorModel>();

        /// <summary>
        /// File system watcher service to watch the competitor file for outside changes
        /// </summary>
        private DelayedFileSystemWatcher fileWatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RosterService"/> class.
        /// </summary>
        [ImportingConstructor]
        public RosterService(ILoggerFacade logger)
        {
            this.Logger = logger;
            this.DataFileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LaJust");
            Directory.CreateDirectory(this.DataFileDirectory);

            this.fileWatcherService = new DelayedFileSystemWatcher(this.DataFileDirectory);
            this.fileWatcherService.Filter = CompetitorFileName;
            this.fileWatcherService.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.fileWatcherService.Changed += (s, e) => this.LoadCompetitors();
            this.fileWatcherService.EnableRaisingEvents = true;

            this.LoadCompetitors();
            this.competitors.CollectionChanged += (s, e) =>
                {
                    if (e.Action != NotifyCollectionChangedAction.Reset)
                    {
                        this.SaveCompetitors();
                    }
                };
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Receiver"/> is reclaimed by garbage collection.
        /// </summary>
        ~RosterService()
        {
            throw new InvalidOperationException("RosterService Dispose method not called.");
        }
        #endif

        /// <summary>
        /// Gets the competitors.
        /// </summary>
        /// <value>The competitors.</value>
        public DispatchingObservableCollection<CompetitorModel> Competitors
        {
            get { return this.competitors; }
        }

        /// <summary>
        /// Loads the competitors.
        /// </summary>
        public void LoadCompetitors()
        {
            this.fileWatcherService.EnableRaisingEvents = false;
            lock (this.competitors)
            {
                List<CompetitorModel> loadedCompetitors = new List<CompetitorModel>();

                string fileName = Path.Combine(this.DataFileDirectory, CompetitorFileName);
                if (File.Exists(fileName))
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        while (!reader.EndOfStream)
                        {
                            loadedCompetitors.Add(new CompetitorModel() { DisplayName = reader.ReadLine() });
                        }
                    }
                }

                this.competitors.SuppressOnCollectionChanged = true;
                this.competitors.Clear();
                this.competitors.AddRange(loadedCompetitors);
                this.competitors.SuppressOnCollectionChanged = false;
            }

            this.Logger.Log("RosterService loaded " + this.competitors.Count + " competitors", Category.Info, Priority.Low);
            this.fileWatcherService.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Saves the competitors.
        /// </summary>
        public void SaveCompetitors()
        {
            string fileName = Path.Combine(this.DataFileDirectory, CompetitorFileName);
            string tmpFileName = fileName + '~';

            if (this.competitors.Count > 0)
            {
                this.fileWatcherService.EnableRaisingEvents = false;
                lock (this.competitors)
                {
                    using (StreamWriter writer = new StreamWriter(tmpFileName))
                    {
                        foreach (CompetitorModel competitor in this.competitors)
                        {
                            writer.WriteLine(competitor.DisplayName);
                        }
                    }
                }
                File.Delete(fileName);
                File.Move(tmpFileName, fileName);
                this.fileWatcherService.EnableRaisingEvents = true;

                this.Logger.Log("RosterService saved " + this.competitors.Count + " competitors", Category.Info, Priority.Low);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.fileWatcherService != null)
                {
                    this.fileWatcherService.EnableRaisingEvents = false;
                    this.fileWatcherService.Dispose();
                }
            }
        }
    }
}
