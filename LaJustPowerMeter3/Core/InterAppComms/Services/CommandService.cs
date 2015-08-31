namespace InterAppComms.Services
{
    using System;
    using System.ServiceModel;

    using InterAppComms.Contracts;
    using InterAppComms.Services;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommandService : BasicService, ICommandService
    {
        public class CommandEventArgs : EventArgs
        {
            public string Command { get; set; }

            public string[] Params { get; set; }
        }

        /// <summary>
        /// Occurs when [command received].
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandReceived = delegate { };

        #region ICommandService Implementation

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="param">The parameters.</param>
        public void Execute(string command, params string[] param)
        {
            this.OnCommandReceived(command, param);
        }

        #endregion

        /// <summary>
        /// Called when [ping received].
        /// </summary>
        protected void OnCommandReceived(string command, string[] param)
        {
            EventHandler<CommandEventArgs> handler = this.CommandReceived;
            handler(this, new CommandEventArgs() { Command = command, Params = param });
        }

    }
}