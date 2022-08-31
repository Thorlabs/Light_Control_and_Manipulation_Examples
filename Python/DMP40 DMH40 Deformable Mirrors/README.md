## Included Examples

### Thorlabs DMP40 / DMH40 Deformable Mirror Open and Read
This sample code shows how you can control a Thorlabs DMP40 / DMH40 Deformable Mirror in Python.
It uses the ctypes library to load the DLL file for these mirrors. This library needs to be installed separately on the computer.

Please note that the code connects to the first available DMP40 / DMH40 device. If you have more than one of these connected, you need to change the index number in this line:

lib.TLDFM_get_device_information(instrumentHandle, 0, ...)


