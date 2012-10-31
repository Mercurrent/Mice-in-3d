using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace kinectCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            KinectSensor kinect = KinectSensor.KinectSensors [0];
            kinect.Start ();

            int ticks = Environment.TickCount;
            DepthCapture depthCam = new DepthCapture (kinect, ticks);
            VideoCapture videoCam = new VideoCapture (kinect, ticks);

            while (true)
                System.Threading.Thread.Sleep (10);

            //Console.ReadLine ();
        }
    }
}
