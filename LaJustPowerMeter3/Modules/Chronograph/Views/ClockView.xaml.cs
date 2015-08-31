﻿// <copyright file="ClockView.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Chronograph
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;
    using Infrastructure;

    /// <summary>
    /// Interaction logic for ClockView.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.ClockRegion)]
    public partial class ClockView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClockView"/> class.
        /// </summary>
        public ClockView()
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
        ClockViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
