using System;
using System.Runtime.InteropServices;

using DirectShowLib;
using WindowsMediaLib;
using System.IO;
using System.Reflection;

namespace VideoCapture
{
    /// <summary>
    /// Video Capture Class
    /// </summary>
    public class VideoCapture: IDisposable
    {
        #region Member variables

        public IMediaControl MediaControl = null;

        private IFilterGraph2 FilterGraph = null;
        private bool Running = false;

        #if DEBUG
        private DsROTEntry DsRot = null;
        #endif

        #endregion

        #region Constructor

        /// <summary>
        /// Create capture object
        /// </summary>
        /// <param name="iDeviceNum">Zero based index of capture device</param>
        /// <param name="szFileName">Output ASF file name</param>
        public void Configure(int iDeviceNum, string szOutputFileName)
        {
            DsDevice[] capDevices;

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (iDeviceNum + 1 > capDevices.Length)
            {
                throw new Exception("No video capture devices found at that index!");
            }

            try
            {
                this.SetupGraph(capDevices[iDeviceNum], szOutputFileName);
                this.Running = false;
            }
            catch
            {
                this.Dispose();
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.CloseInterfaces();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="VideoCapture"/> is reclaimed by garbage collection.
        /// </summary>
        ~VideoCapture()
        {
            this.Dispose();
        }

        /// <summary>
        /// Starts the capture.
        /// </summary>
        public void StartCapture()
        {
            if (!Running)
            {
                int hr = MediaControl.Run();
                Marshal.ThrowExceptionForHR( hr );

                this.Running = true;
            }
        }

        /// <summary>
        /// Pause the capture graph.
        /// Running the graph takes up a lot of resources.  Pause it when it
        /// isn't needed.
        /// </summary>
        public void PauseCapture()
        {
            if (Running)
            {
                IMediaControl mediaCtrl = FilterGraph as IMediaControl;

                int hr = mediaCtrl.Pause();
                Marshal.ThrowExceptionForHR( hr );

                this.Running = false;
            }
        }

        /// <summary>
        /// Setup the capture graph.
        /// </summary>
        /// <param name="dev">The dev.</param>
        /// <param name="szOutputFileName">Name of the sz output file.</param>
        private void SetupGraph(DsDevice dev, string szOutputFileName)
        {
            int hr;

            IBaseFilter capFilter = null;
            IBaseFilter asfWriter = null;
            ICaptureGraphBuilder2 capGraph = null;

            // Get the graph builder object
            this.FilterGraph = (IFilterGraph2) new FilterGraph();

            #if DEBUG
            this.DsRot = new DsROTEntry(this.FilterGraph);
            #endif

            try
            {
                // Get the ICaptureGraphBuilder2
                capGraph = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();

                // Start building the graph
                hr = capGraph.SetFiltergraph(this.FilterGraph);
                Marshal.ThrowExceptionForHR(hr);

                // Add the capture device to the graph
                hr = this.FilterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
                Marshal.ThrowExceptionForHR(hr);

                // Set capture device to highest resolution it supports
                InitializeResolution(capGraph, capFilter);

                // Configure WMV Asf output writer
                asfWriter = this.ConfigAsf(capGraph, szOutputFileName);

                // Connect the pins for rendering the stream
                hr = capGraph.RenderStream(PinCategory.Capture, MediaType.Video, capFilter, null, asfWriter);
                Marshal.ThrowExceptionForHR(hr);

                this.MediaControl = this.FilterGraph as IMediaControl;

                // Pre-roll ready for activation
                this.MediaControl.Run();
                this.MediaControl.Pause();
            }
            finally
            {
                if (capFilter != null)
                {
                    Marshal.ReleaseComObject(capFilter);
                    capFilter = null;
                }
                if (asfWriter != null)
                {
                    Marshal.ReleaseComObject(asfWriter);
                    asfWriter = null;
                }
                if (capGraph != null)
                {
                    Marshal.ReleaseComObject(capGraph);
                    capGraph = null;
                }
            }
        }

        /// <summary>
        /// Initializes the resolution.
        /// </summary>
        /// <param name="capGraph">The cap graph.</param>
        /// <param name="capFilter">The cap filter.</param>
        void InitializeResolution(ICaptureGraphBuilder2 capGraph, IBaseFilter capFilter)
        {
            AMMediaType mediaType = null;
            IAMStreamConfig videoStreamConfig = null;
            IntPtr ptr;
            int iCount = 0;
            int iSize = 0;
            int maxWidth = 0;
            int maxHeight = 0;
            int streamID = 0;
            object obj;

            capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out obj);
            videoStreamConfig = obj as IAMStreamConfig;
            videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);
            ptr = Marshal.AllocCoTaskMem(iSize);

            for (int i = 0; i < iCount; i++)
            {
                videoStreamConfig.GetStreamCaps(i, out mediaType, ptr);
                VideoInfoHeader videoInfo = new VideoInfoHeader();
                Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);
                if (videoInfo.BmiHeader.Width > maxWidth && videoInfo.BmiHeader.Height > maxHeight)
                {
                    streamID = i;
                    maxWidth = videoInfo.BmiHeader.Width;
                    maxHeight = videoInfo.BmiHeader.Height;
                }
            }

            videoStreamConfig.GetStreamCaps(streamID, out mediaType, ptr);
            int hr = videoStreamConfig.SetFormat(mediaType);
            Marshal.FreeCoTaskMem(ptr);

            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mediaType);
            mediaType = null;
        }

        /// <summary>
        /// Configs the asf.
        /// </summary>
        /// <param name="capGraph">The cap graph.</param>
        /// <param name="szOutputFileName">Name of the sz output file.</param>
        /// <returns></returns>
        private IBaseFilter ConfigAsf(ICaptureGraphBuilder2 capGraph, string szOutputFileName)
        {
            IBaseFilter asfWriter = null;
            IFileSinkFilter pTmpSink = null;
            IWMProfileManager ppProfileManager = null;
            IWMProfile ppProfile = null;

            try
            {
                WindowsMediaLib.WMUtils.WMCreateProfileManager(out ppProfileManager);
                string prx = File.ReadAllText(Path.Combine(Path.GetDirectoryName(
                    Assembly.GetEntryAssembly().Location), "profile.prx"));
                ppProfileManager.LoadProfileByData(prx, out ppProfile);

                int hr = capGraph.SetOutputFileName(MediaSubType.Asf, szOutputFileName, out asfWriter, out pTmpSink);
                Marshal.ThrowExceptionForHR(hr);

                WindowsMediaLib.IConfigAsfWriter lConfig = asfWriter as WindowsMediaLib.IConfigAsfWriter;
                lConfig.ConfigureFilterUsingProfile(ppProfile);
            }
            finally
            {
                Marshal.ReleaseComObject(pTmpSink);
                Marshal.ReleaseComObject(ppProfile);
                Marshal.ReleaseComObject(ppProfileManager);
            }

            return asfWriter;
        }

        /// <summary>
        /// Closes the interfaces.
        /// </summary>
        private void CloseInterfaces()
        {
            if (this.MediaControl != null)
            {
                try
                {
                    this.MediaControl.Stop();
                    this.Running = false;
                }
                catch
                {
                }
            }

            #if DEBUG
            if (this.DsRot != null)
            {
                this.DsRot.Dispose();
                this.DsRot = null;
            }
            #endif

            if (this.FilterGraph != null)
            {
                Marshal.ReleaseComObject(this.FilterGraph);
                this.FilterGraph = null;
            }
        }
    }
}
