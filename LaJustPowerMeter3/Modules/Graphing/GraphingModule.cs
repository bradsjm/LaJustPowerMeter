// <copyright file="GraphingModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System.ComponentModel.Composition;

    using Infrastructure;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Receiver Module providing the service to interface to the hardware receivers
    /// </summary>
    [Export]
    public sealed class GraphingModule : IModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphingModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public GraphingModule()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
        }
    }
}
