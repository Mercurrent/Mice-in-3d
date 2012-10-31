Mice-in-3d
==========


c# code for writing video+depth stream from Kinect. Requires Kinect SDK.
Writes depth frames to 'depth' folder and rgb to 'color' folder. All frames named as x_y, 
where x is number in sequence of frames (of one type) and y is time in milliseconds from start 
when the frame was captured.


It was created as Visual Studio project. I can hardly imagine how to commit such projects properly.
So I added a binary.

Anyway, I'm going to change read-procedure. Using another library - OpenNI. So it's temp.