// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteEvents.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Fired when a button is pressed on the receiver panel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System.ComponentModel.Composition;

    using Microsoft.Practices.Prism.Events;

    #region Public Events Fired by Receiver Module

    /// <summary>
    /// Fired when a button is pressed on the receiver panel
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ButtonPressed : CompositePresentationEvent<string>
    {
    }

    #endregion
}