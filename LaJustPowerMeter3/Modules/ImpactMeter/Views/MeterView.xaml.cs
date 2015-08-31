// <copyright file="ImpactMeterModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ImpactMeter
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
    using Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Interaction logic for MeterView.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MeterView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeterView"/> class.
        /// </summary>
        public MeterView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        public MeterViewModel ViewModel
        {
            get { return this.DataContext as MeterViewModel; }
            set { this.DataContext = value; }
        }
    }
}
