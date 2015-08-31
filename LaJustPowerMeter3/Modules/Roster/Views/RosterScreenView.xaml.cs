// <copyright file="RosterScreenView.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Roster
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
    /// Interaction logic for RosterScreenView.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.PrimaryPageRegion, ViewName=PageNames.Roster)]
    public partial class RosterScreenView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RosterScreenView"/> class.
        /// </summary>
        public RosterScreenView()
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
        RosterScreenViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }

    }
}
