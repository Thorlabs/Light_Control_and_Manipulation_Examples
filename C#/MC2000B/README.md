## Example Description
This example shows how you can control a Thorlabs MC2000B Optical Chopper in C Sharp using serial commands.  
The example contains the basic initialization, it also allows setting the frequency and enabling the chopper.   
>This example sets the blade to **MC1F10HP**, please modify the blade type in the code if you are using other blades. 


## Instructions for Use

System.IO.Ports Namespace is necessary for this example. If you are using Visual Studio, you can download it from Tools>>NuGet Package Manager>>Manage NuGet Package for solution.
In the "Browse" menu, enter "System.IO.Ports", and install the first package named System.IO.Ports.

Besides, you need to check the COM port number of your device from the Windows Device Manager prior to running.  


