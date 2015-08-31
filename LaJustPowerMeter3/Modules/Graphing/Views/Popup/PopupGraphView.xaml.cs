// <copyright file="PopupGraphView.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
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
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Infrastructure;
    using Microsoft.Practices.Prism;
    using Microsoft.Practices.Prism.Events;

    /// <summary>
    /// Interaction logic for PopupGraphView.xaml
    /// </summary>
    public partial class PopupGraphView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupGraphView"/> class.
        /// </summary>
        public PopupGraphView()
        {
            this.InitializeComponent();
        }
    }
}