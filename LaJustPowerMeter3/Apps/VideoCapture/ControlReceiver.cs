using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterAppComms;
using InterAppComms.Contracts;
using InterAppComms.Services;
using System.Windows.Forms;

namespace VideoCapture
{
    public class ControlReceiver
    {
        private PipeReceiver<ICommandService, CommandService> receiver = new PipeReceiver<ICommandService, CommandService>();
        private VideoCapture videoCapture;

        public void Initialize(VideoCapture videoCapture)
        {
            this.videoCapture = videoCapture;
            receiver.Instance.CommandReceived += (s, e) => ExecuteRemoteCommand(e.Command, e.Params);
            try
            {
                receiver.StartService();
            }
            catch
            {
                return;
            }
        }

        void ExecuteRemoteCommand(string cmd, string[] param)
        {
            if (cmd.ToUpper() == "RECORD")
            {
                switch (param[0])
                {
                    case "True": this.videoCapture.StartCapture(); break;
                    case "False": this.videoCapture.PauseCapture(); break;
                }
            }
        }
    }
}
