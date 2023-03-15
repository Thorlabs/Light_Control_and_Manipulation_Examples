## Example Description
This example shows how you can control a Thorlabs KURIOS Liquid Crystal Tunable Filter in C Sharp using serial commands. 
The example contains the basic initialization, allows driving the filter in manual mode or trigger mode.
The example uses the SerialPort class from the .NET Framework to open a communication channel and set/receive data from the controller.


## Instructions for Use

System.IO.Ports Namespace is necessary for the this example. If you are using Visual Studio, you can download it from Tools>>NuGet Package Manager>>Manage NuGet Package for solution.
In the "Browse" menu, enter "System.IO.Ports", and install the first pacakge named System.IO.Ports.

Besides, you need to check the COM port number of your device from the Windows Device Manager prior to running.
