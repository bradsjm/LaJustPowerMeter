/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.GameEngine.Default
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class DefaultGameService : IGameService
    {
        #region IGameService Members

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}