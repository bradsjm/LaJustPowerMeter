// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiverEvents.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Receiver Events from hardware devices
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System.ComponentModel.Composition;

    using LaJust.EIDSS.Communications.Entities;

    using Microsoft.Practices.Prism.Events;

    /// <summary>
    /// Receiver Events from hardware devices
    /// </summary>
    public static class ReceiverEvents
    {
        /// <summary>
        /// Fired when a button is pressed on the receiver panel
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class ButtonPressed : CompositePresentationEvent<PanelButtonEventData>
        {
        }

        /// <summary>
        /// Fired when a device (Hogu, Target etc.) is registered
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class DeviceRegistered : CompositePresentationEvent<DeviceRegistrationEventArgs>
        {
        }

        /// <summary>
        /// Fired when a device (Hogu, Target etc.) provides status information
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class DeviceStatusUpdate : CompositePresentationEvent<DeviceStatusEventArgs>
        {
        }

        /// <summary>
        /// Fired by client to request Receiver Module register a device
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class RegisterDevice : CompositePresentationEvent<RegistrationSettings>
        {
        }

        /// <summary>
        /// Fired when a sensor device reports a hit
        /// </summary>
        [Export]
        [PartCreationPolicy(CreationPolicy.Shared)]
        public class SensorHit : CompositePresentationEvent<DeviceDataEventArgs>
        {
        }
    }
}