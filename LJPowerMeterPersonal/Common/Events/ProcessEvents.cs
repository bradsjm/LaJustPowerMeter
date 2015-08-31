/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    public enum ProcessEventType
    {
        ModulesInitialized,
        ApplicationShutdown,
        SystemResumed
    }

    public class ProcessEvent : CompositePresentationEvent<ProcessEventType> { }
}