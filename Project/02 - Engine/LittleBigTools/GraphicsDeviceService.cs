using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace LBT
{
    /// <summary>
    /// Helper class responsible for creating and managing the GraphicsDevice.
    /// All GraphicsDeviceControl instances share the same GraphicsDeviceService,
    /// so even though there can be many controls, there will only ever be a 
    /// single underlying GraphicsDevice. This implements the standard 
    /// IGraphicsDeviceService interface, which provides notification events for 
    /// when the device is reset or disposed.
    /// 
    /// Source: http://blogs.msdn.com/b/nicgrave/archive/2010/07/25/rendering-with-xna-framework-4-0-inside-of-a-wpf-application.aspx
    /// </summary>
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        // Singleton device service instance.
        private static GraphicsDeviceService singletonInstance;

        // Keep track of how many controls are sharing the singletonInstance.
        private static int referenceCount;

        /// <summary>
        /// Gets the single instance of the service class for the application.
        /// </summary>
        public static GraphicsDeviceService Instance
        {
            get
            {
                if (singletonInstance == null)
                    singletonInstance = new GraphicsDeviceService();
                return singletonInstance;
            }
        }

        // Store the current device settings.
        private PresentationParameters parameters;

        /// <summary>
        /// Gets the current graphics device.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        // IGraphicsDeviceService events.
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        /// <summary>
        /// Constructor is private, because this is a singleton class:
        /// client controls should use the public AddRef method instead.
        /// </summary>
        GraphicsDeviceService() { }

        /// <summary>
        /// Creates the GraphicsDevice for the service.
        /// </summary>
        private void CreateDevice(IntPtr windowHandle)
        {
            parameters = new PresentationParameters();

            // since we're using render targets anyway, the 
            // backbuffer size is somewhat irrelevant
            parameters.BackBufferWidth = 480;
            parameters.BackBufferHeight = 320;
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DeviceWindowHandle = windowHandle;
            parameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            parameters.IsFullScreen = false;

            //Constructor with parameters not available in MonoGame
            GraphicsDevice = new GraphicsDevice();

            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets a reference to the singleton instance.
        /// </summary>
        public static GraphicsDeviceService AddRef(IntPtr windowHandle)
        {
            // Increment the "how many controls sharing the device" 
            // reference count.
            if (Interlocked.Increment(ref referenceCount) == 1)
            {
                // If this is the first control to start using the
                // device, we must create the device.
                Instance.CreateDevice(windowHandle);
            }

            return singletonInstance;
        }

        /// <summary>
        /// Releases a reference to the singleton instance.
        /// </summary>
        public void Release()
        {
            // Decrement the "how many controls sharing the device" 
            // reference count.
            if (Interlocked.Decrement(ref referenceCount) == 0)
            {
                // If this is the last control to finish using the
                // device, we should dispose the singleton instance.
                if (DeviceDisposing != null)
                    DeviceDisposing(this, EventArgs.Empty);

                GraphicsDevice.Dispose();

                GraphicsDevice = null;
            }
        }
    }
}
