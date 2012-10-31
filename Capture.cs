using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace kinectCLI
{
    class DepthCapture
    {
        protected KinectSensor kinect;
        private int frameNumber = 0;
        private int baseTime;

        protected int unknownDepth;

        private short[] rawDepth = null;
        private byte[] rawImage = null;
        
        private void FrameIsReady (object sender, DepthImageFrameReadyEventArgs e)
        {
            int timeA, timeB;

            timeA = Environment.TickCount;

            DepthImageFrame frame = e.OpenDepthImageFrame ();

            if (rawImage == null || rawImage.Length != frame.PixelDataLength)
            {
                rawDepth = new short [frame.PixelDataLength];
                rawImage = new byte [frame.PixelDataLength * 4];
            }
                                  
            frame.CopyPixelDataTo (rawDepth);

            for (int jd = 0, ji = 0; jd < rawDepth.Length && ji < this.rawImage.Length; ++jd, ji += 4)
            {
                int depth = rawDepth [jd] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                byte r, g, b;

                if (depth != unknownDepth)
                {
                    r = (byte) ((depth >> 8) * 16);
                    g = 0;
                    b = (byte) (depth & 0xFF);
                }
                else
                {
                    r = 0;
                    g = 255;
                    b = 0;
                }

                rawImage [ji +2] = r;
                rawImage [ji +1] = g;
                rawImage [ji] = b;
                rawImage [ji +3] = 0;
            }

            WriteableBitmap image = new WriteableBitmap (640, 480, 96, 96, PixelFormats.Bgr32, null);
            image.WritePixels (new Int32Rect (0, 0, 640, 480), rawImage, 640 * 4, 0);
            
            FileStream fileStream = new FileStream ("depth\\" + frameNumber.ToString() +  "_" + 
                                                    (timeA - baseTime).ToString() + ".jpg", FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
            encoder.QualityLevel = 95;

            encoder.Frames.Clear ();
            encoder.Frames.Add (BitmapFrame.Create (image));
            encoder.Save (fileStream);
            frameNumber += 1;

            fileStream.Close();

            timeB = Environment.TickCount;

            Console.WriteLine ("Depth frame processed " + (timeB - timeA).ToString() + " ms");
        }

        public DepthCapture (KinectSensor kinect, int ticks)
        {
            this.kinect = kinect;
            baseTime = ticks;

            unknownDepth = kinect.DepthStream.UnknownDepth;

            kinect.DepthStream.Enable (DepthImageFormat.Resolution640x480Fps30);
            kinect.DepthFrameReady += new EventHandler <DepthImageFrameReadyEventArgs> (FrameIsReady);
            if (kinect.Status == KinectStatus.Connected)
                Console.WriteLine ("Connected");
        }
    }

    class VideoCapture
    {
        protected KinectSensor kinect;
        private int frameNumber = 0;
        private int baseTime;

        private byte[] rawImage = null;
        
        private void FrameIsReady (object sender, ColorImageFrameReadyEventArgs e)
        {
            int timeA, timeB;

            timeA = Environment.TickCount;

            ColorImageFrame frame = e.OpenColorImageFrame ();

            if (rawImage == null || rawImage.Length != frame.PixelDataLength)
                rawImage = new byte [frame.PixelDataLength];
            
            frame.CopyPixelDataTo (rawImage);
            WriteableBitmap image = new WriteableBitmap (640, 480, 96, 96, PixelFormats.Bgr32, null);
            image.WritePixels (new Int32Rect (0, 0, 640, 480), rawImage, 640 * 4, 0);
            
            // you may synchronize depth and colour frames
            FileStream fileStream = new FileStream ("color\\" + frameNumber.ToString() + "_" + 
                                                    (timeA - baseTime).ToString() + ".jpg", FileMode.Create);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
            encoder.QualityLevel = 95;

            encoder.Frames.Clear ();
            encoder.Frames.Add (BitmapFrame.Create (image));
            encoder.Save (fileStream);
            frameNumber += 1;

            fileStream.Close();

            timeB = Environment.TickCount;

            Console.WriteLine ("Video frame processed " + (timeB - timeA).ToString() + " ms");
        }

        public VideoCapture (KinectSensor kinect, int ticks)
        {
            this.kinect = kinect;
            baseTime = ticks;
            
            kinect.ColorStream.Enable (ColorImageFormat.RgbResolution640x480Fps30);
            kinect.ColorFrameReady += new EventHandler <ColorImageFrameReadyEventArgs> (FrameIsReady);
            if (kinect.Status == KinectStatus.Connected)
                Console.WriteLine ("Connected");
        }
    }
}
