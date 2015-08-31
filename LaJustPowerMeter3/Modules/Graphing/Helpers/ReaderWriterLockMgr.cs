
namespace Graphing
{
    using System;
    using System.Threading;

    /// <summary>
    /// Reader Writer Lock Manager
    /// </summary>
    internal class ReaderWriterLockMgr : IDisposable
    {
        private ReaderWriterLockSlim _readerWriterLock = null;
        private bool _isDisposed = false;
        private enum LockTypes { None, Read, Write }
        LockTypes _enteredLockType = LockTypes.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriterLockMgr"/> class.
        /// </summary>
        /// <param name="ReaderWriterLock">The reader writer lock.</param>
        public ReaderWriterLockMgr(ReaderWriterLockSlim ReaderWriterLock)
        {
            _readerWriterLock = ReaderWriterLock;
        }

        /// <summary>
        /// Enters the read lock.
        /// </summary>
        public void EnterReadLock()
        {
            _readerWriterLock.EnterReadLock();
            _enteredLockType = LockTypes.Read;
        }

        /// <summary>
        /// Enters the write lock.
        /// </summary>
        public void EnterWriteLock()
        {
            _readerWriterLock.EnterWriteLock();
            _enteredLockType = LockTypes.Write;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    switch (_enteredLockType)
                    {
                        case LockTypes.Read:
                            _readerWriterLock.ExitReadLock();
                            break;
                        case LockTypes.Write:
                            _readerWriterLock.ExitWriteLock();
                            break;
                    }
                }
            }

            _isDisposed = true;
        }
    }
}
