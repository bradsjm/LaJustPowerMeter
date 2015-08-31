// <copyright file="EventScreenView.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using Infrastructure;

    /// <summary>
    /// Interaction logic for EventScreenView.xaml
    /// </summary>
    [ViewExport(RegionName=RegionNames.PrimaryPageRegion, ViewName=PageNames.Event)]
    public partial class EventScreenView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventScreenView"/> class.
        /// </summary>
        public EventScreenView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF")]
        EventScreenViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
