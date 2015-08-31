namespace Network
{
    using System;

    /// <summary>
    /// Implementation of the Latch Pattern
    /// </summary>
    public class Latch
    {
        private readonly object _counterLock = new object();

        public int LatchCounter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is latched.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is latched; otherwise, <c>false</c>.
        /// </value>
        public bool IsLatched
        {
            get { return (LatchCounter > 0); }
        }

        /// <summary>
        /// Runs the latched action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void RunLatched(Action action)
        {
            try
            {
                lock (_counterLock) { LatchCounter++; }
                action();
            }
            finally
            {
                lock (_counterLock) { LatchCounter--; }
            }
        }

        /// <summary>
        /// Runs if not latched.
        /// </summary>
        /// <param name="action">The action.</param>
        public void RunIfNotLatched(Action action)
        {
            if (IsLatched)
                return;

            action();
        }
    }
}
