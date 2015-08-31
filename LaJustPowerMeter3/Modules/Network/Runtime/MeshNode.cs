
namespace Network
{
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using System.Diagnostics;
    using LaJust.EIDSS.Communications.Entities;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MeshNode : IMeshNode
    {
        #region Public Events

        public event EventHandler<HeartbeatEventArgs> HeartbeatReceived = delegate { };

        public event EventHandler<HeartbeatEventArgs> EventReceived = delegate { };

        #endregion

        /// <summary>
        /// Called to receive the heart beat status
        /// </summary>
        /// <param name="nodeId">The sender node id.</param>
        void IMeshNode.Heartbeat(Guid nodeId)
        {
            System.Diagnostics.Trace.WriteLine("Received heart beat signal from " + nodeId);
            this.OnHeartbeatReceived(this, new HeartbeatEventArgs(nodeId));
        }

        void IMeshNode.DeviceRegistered(DeviceRegistrationEventArgs args)
        {
            System.Diagnostics.Trace.WriteLine("Received DeviceRegistered");
        }

        #region Protected Event Raising Methods

        protected virtual void OnHeartbeatReceived(object sender, HeartbeatEventArgs e)
        {
            EventHandler<HeartbeatEventArgs> handler = this.HeartbeatReceived;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnHeartbeatReceived: {1}", "MeshNode", ex.GetBaseException());
                throw;
            }
        }

        #endregion
    }
}