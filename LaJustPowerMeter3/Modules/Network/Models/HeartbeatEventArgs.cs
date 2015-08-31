using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Network
{
    public class HeartbeatEventArgs : EventArgs
    {
        public Guid NodeId { get; private set; }

        public HeartbeatEventArgs(Guid nodeId)
        {
            this.NodeId = nodeId;
        }
    }
}
