// <copyright file="MeterCollectionViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ImpactMeter
{
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Infrastructure;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;
    using LaJust.EIDSS.Communications.Entities;

    /// <summary>
    /// Impact Meter Collection View Model
    /// </summary>
    [Export]
    public class MeterCollectionViewModel : NotificationObject
    {
        /// <summary>
        /// Gets or sets the meters.
        /// </summary>
        /// <value>The meters.</value>
        public DispatchingObservableCollection<MeterView> Meters { get; private set; }

        private IScoreKeeperService scoreKeeperService;

        private CompositionContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterCollectionViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public MeterCollectionViewModel(CompositionContainer container, IScoreKeeperService scoreKeeperService)
        {
            this.Meters = new DispatchingObservableCollection<MeterView>();
            this.container = container;
            this.scoreKeeperService = scoreKeeperService;
            this.scoreKeeperService.Competitors.CollectionChanged += Competitors_CollectionChanged;

            foreach (CompetitorModel item in this.scoreKeeperService.Competitors)
            {
                this.Meters.Add(GetMeterView(item));
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Competitors property.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Competitors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.Meters.Clear();
                    foreach (CompetitorModel item in this.scoreKeeperService.Competitors)
                    {
                        this.Meters.Add(GetMeterView(item));
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    foreach (CompetitorModel item in e.NewItems)
                    {
                        this.Meters.Add(GetMeterView(item));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (CompetitorModel item in e.OldItems)
                    {
                        var meter = this.Meters.FirstOrDefault(m => m.ViewModel.Competitor == item);
                        if (meter != null) this.Meters.Remove(meter);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the meter view and wires up the competitor to it
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private MeterView GetMeterView(CompetitorModel model)
        {
            // Note: This should probably be refactored into a factory class somewhere!!!!!
            
            var meter = this.container.GetExportedValue<MeterView>();
            meter.ViewModel.HasPanels = model.DeviceType == OpCodes.TargetRegistered;
            meter.ViewModel.Competitor = model;
            meter.ViewModel.CloseMeterCommand = new DelegateCommand(delegate
            {
                model.UnPartner();
                this.scoreKeeperService.Competitors.Remove(model);
            });
            meter.ViewModel.MoveMeterRightCommand = new DelegateCommand(delegate
            {
                var pos = this.Meters.IndexOf(meter);
                if (pos + 1 < this.Meters.Count) this.Meters.Move(pos, pos + 1);
            });
            meter.ViewModel.MoveMeterLeftCommand = new DelegateCommand(delegate
            {
                var pos = this.Meters.IndexOf(meter);
                if (pos > 0) this.Meters.Move(pos, pos - 1);
            });

            return meter;
        }
    }
}
