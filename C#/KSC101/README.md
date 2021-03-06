## Included Examples

### Initialize and Enable: 
This example shows the basic initialization and use of the KSC101 Shutter Controller. This includes opening the device, initializing the settings, enabling the device, and looping to open/close the shutter 10 times. 
This example uses libraries found in the Kinesis software install found here: https://www.thorlabs.com/software_pages/ViewSoftwarePage.cfm?Code=Motion_Control

## Build Instructions
1. Set the desired startup file. 
2. Set Project Platform under Project -> Properties -> Build. This should be selected to match the bit-version of the dll's you plan to use (e.g. x64 for 64-bit dll's). 
3. Copy the following dll's from the Kinesis installation folder to the bin of the startup project e.g \Light_Sources_Examples\C#\KLS101\Initialize_and_Enable\bin\Debug:
   * Thorlabs.MotionControl.DeviceManager.dll
   * Thorlabs.MotionControl.DeviceManagerCLI.dll
   * Thorlabs.MotionControl.KCube.Solenoid.dll
   * Thorlabs.MotionControl.KCube.SolenoidCLI.dll
   * Thorlabs.MotionControl.PrivateInternal.dll

4. The References are already added to the project, but in the event they need to be re-added you will need the following: 
   * Thorlabs.MotionControl.DeviceManagerCLI
   * Thorlabs.MotionControl.KCube.SolenoidCLI

