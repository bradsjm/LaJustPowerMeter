namespace VideoCapture
{
    using System;
    using System.Threading;
    using System.Windows.Forms;
    
    static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            ControlReceiver controlReceiver = new ControlReceiver();
            VideoCapture videoCapture = new VideoCapture();

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (!IsOnlyInstance())
			{
				return;
			}
			
			try
			{
                ThreadPool.QueueUserWorkItem(o =>
                    {
                        videoCapture.Configure(0, "temp.wmv");
                        controlReceiver.Initialize(videoCapture);
                    });
                Application.Run(new CustomApplicationContext());
            }
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Program Terminated Unexpectedly",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Gets the assembly GUID.
		/// </summary>
		static private bool IsOnlyInstance()
		{
			const string MutexGuid = @"Global\FD11F255-A3AC-435E-80F5-F64C294795B8";
			bool onlyInstance = false;
			Mutex mutex = new Mutex(true, MutexGuid, out onlyInstance);
			GC.KeepAlive(mutex);
			return onlyInstance;
		}
	}
}
