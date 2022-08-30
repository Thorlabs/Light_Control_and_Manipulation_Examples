# -*- coding: utf-8 -*-
"""
Example of C Libraries for DMP40 Deformable Mirror in Python with CTypes

Except for the tilt arms, this example can also be used with the DMH40 mirrors. ...

"""
import os
import time
from ctypes import *

# Please note that the DMP40 devices have two driver files:
#
# TLDFM: Driver with the basic functions to control the DMP40 mirror segments.
# TLDFMX: Extended driver for mirror segment control using Zernike coefficient.

#os.chdir(r"C:\Program Files\IVI Foundation\VISA\Win64\Bin")
lib = cdll.LoadLibrary("C:\Program Files\IVI Foundation\VISA\Win64\Bin\TLDFM_64.dll")
libX = cdll.LoadLibrary("C:\Program Files\IVI Foundation\VISA\Win64\Bin\TLDFMX_64.dll")

# Detect and initialize DMP40 device
instrumentHandle = c_ulong()
IDQuery = True
resetDevice = False
resource = c_char_p(b"")
deviceCount = c_int()

# Check how many DMP40 are connected
lib.TLDFM_get_device_count(instrumentHandle, byref(deviceCount))
if deviceCount.value < 1 :
    print("No DMP40 device found.")
    exit()
else:
    print(deviceCount.value, "DMP40 device(s) found.")
    print("")

# Connect to the first available DMP40
# Use TLDFMX_init to use functions in the extended driver as well
lib.TLDFM_get_device_information(instrumentHandle, 0, 0, 0, 0, 0, resource)

if (0 == libX.TLDFMX_init(resource.value, IDQuery, resetDevice, byref(instrumentHandle))):
    print("Connection to first DMP40 initialized.")
else:
    print("Error with initialization.")
    exit()
print("")

# Relax DMP40
# devicePart determines which part of the mirror is relaxed
# 0: only mirror, 1: only bimorph tilt arms, 2: both
devicePart = c_uint32(2)
isFirstStep = c_bool(True)
reload = c_bool(False)
# Determine how many segments the mirror has and how many tilt arms.
segmentCount = c_uint32()
lib.TLDFM_get_segment_count(instrumentHandle, byref(segmentCount))
print("Segment count:", segmentCount.value)
tiltCount = c_uint32()
lib.TLDFM_get_tilt_count(instrumentHandle, byref(tiltCount))
print("Tilt count:", tiltCount.value)
print()
# Create arrays for the mirror segment and tilt arm patterns
relaxPatternMirror = (c_double*(segmentCount.value))()
relaxPatternArms = (c_double*(tiltCount.value))()

remainingSteps = c_int32()
counter = 1

# First relax step.
print("Relaxing the DMP40.")
print()
libX.TLDFMX_relax(instrumentHandle, devicePart, isFirstStep, reload,
             relaxPatternMirror, relaxPatternArms, byref(remainingSteps))

lib.TLDFM_set_segment_voltages(instrumentHandle, relaxPatternMirror)
lib.TLDFM_set_tilt_voltages(instrumentHandle, relaxPatternArms)
print("Relax step: ", counter)
counter = counter + 1

isFirstStep = c_bool(False)

# The following relax steps are made in a loop until the relaxation is complete.
while remainingSteps.value > 0:
    libX.TLDFMX_relax(instrumentHandle, devicePart, isFirstStep, reload,
             relaxPatternMirror, relaxPatternArms, byref(remainingSteps))
    lib.TLDFM_set_segment_voltages(instrumentHandle, relaxPatternMirror)
    lib.TLDFM_set_tilt_voltages(instrumentHandle, relaxPatternArms)
    print("Relax step:", counter)
    counter = counter + 1
print()
print("Relaxing complete.")
print()
time.sleep(1)

# Set mirror to a deformation with an amplitude for astigmatism 0° of 0.99
mirrorPattern = (c_double*(segmentCount.value))()
# Flag for astigmatism 0°, see driver manual for flag definition
zernike = c_uint32(4)
# Amplitude for astigmatism 0° is set to 0.99
zernikeAmplitude = c_double(0.99)
libX.TLDFMX_calculate_single_zernike_pattern(instrumentHandle, zernike,
                                             zernikeAmplitude, mirrorPattern)
lib.TLDFM_set_segment_voltages(instrumentHandle, mirrorPattern)
print("Example pattern is set.")
print()

# Prints the voltages applied to each segment
for x in range(segmentCount.value):
  print("Segment voltage in segment", x+1, "[V]:", mirrorPattern[x])
print()

# Close
libX.TLDFMX_close(instrumentHandle)
print("Connection closed. Program finished.")
