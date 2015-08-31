using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace VideoCapture
{
    /// <summary>
    /// Framework for running application as a tray app.
    /// </summary>
    /// <remarks>
    /// Tray app code adapted from "Creating Applications with NotifyIcon in Windows Forms", Jessica Fosler,
    /// http://windowsclient.net/articles/notifyiconapplications.aspx
    /// </remarks>
    public class CustomApplicationContext : ApplicationContext
    {
        private static readonly string IconFileName = "VideoCapture.App.ico";
        private static readonly string DefaultTooltip = "LaJust Video Capture Service";

        public ToolStripMenuItem statusItem = new ToolStripMenuItem("Status: Idle");

        /// <summary>
        /// This class should be created and passed into Application.Run( ... )
        /// </summary>
        public CustomApplicationContext()
        {
            InitializeContext();
            PopulateMenuStrip();
        }

        /// <summary>
        /// Populates the menu strip.
        /// </summary>
        private void PopulateMenuStrip()
        {
            notifyIcon.ContextMenuStrip.Items.Add(this.statusItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(
                new ToolStripMenuItem("&Exit", null, (s, e) => ExitThread()));
        }

        # region the child forms

        //private Form1 introForm;

        //private void ShowIntroForm()
        //{
        //    if (introForm == null)
        //    {
        //        introForm = new Form1();
        //        introForm.Closed += (s,e) => introForm = null;
        //        introForm.Show();
        //    }
        //    else 
        //    {
        //        introForm.Activate(); 
        //    }
        //}

        // From http://stackoverflow.com/questions/2208690/invoke-notifyicons-context-menu
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        # endregion the child forms

        # region generic code framework

        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon notifyIcon;				            // the icon that sits in the system tray

        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(IconFileName)),
                Text = DefaultTooltip,
                Visible = true
            };
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }

        /// <summary>
        /// When the application context is disposed, dispose things like the notify icon.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            // before we exit, let forms clean themselves up.
            //if (introForm != null) { introForm.Close(); }

            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }

        # endregion generic code framework
    }
}
