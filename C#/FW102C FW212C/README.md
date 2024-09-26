## Example Description
This example shows how you can control a Thorlabs FW102C or FW212C Motorized Filter Wheel in C Sharp using serial commands.  
The example contains the basic initialization, it also allows setting necessary parameters and setting the filter position.  
>This example is NOT compatible with FW103, please refer to the KST101/KST201 examples under Motion_Control_Examples repository if you are using the FW103. 


## Instructions for Use

System.IO.Ports Namespace is necessary for this example. If you are using Visual Studio, you can download it from Tools>>NuGet Package Manager>>Manage NuGet Package for solution.
In the "Browse" menu, enter "System.IO.Ports", and install the first package named System.IO.Ports.

Besides, you need to check the COM port number of your device from the Windows Device Manager prior to running.  
