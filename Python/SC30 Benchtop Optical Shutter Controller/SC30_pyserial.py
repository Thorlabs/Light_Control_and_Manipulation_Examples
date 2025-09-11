# Title: SC30_pyserial
# Created Date: 2025 - 07 - 17
# Last modified date: 2025 - 07 - 17
# Python Version Used: Python 3.11
# Notes: This example demonstrates how to control a Thorlabs SC30 with serial commands. 
import serial
import time

#Make the COM port settings as required by the SC30 controllers.
baud_rate = 9600
data_bits = 8
stop_bits = 1
Parity = serial.PARITY_NONE
#Change this to the COM port of the SC30 controller.
#The COM port number can e.g. be seen in the device manager.
COM_port = "COM11"
#When all the settings are made, create a serial connection to COM port.
ser = serial.Serial(port = COM_port, baudrate = baud_rate, bytesize=data_bits, parity=Parity, stopbits=stop_bits,timeout=0.5)

ser.write(b'model?\r\n')
time.sleep(0.1)
print(" Item number: ", (ser.readline()).decode('utf-8').strip())
ser.write(b'serial?\r\n')
time.sleep(0.1)
print(" Serial number: ", (ser.readline()).decode('utf-8').strip())

ser.write(b'mode=0\r\n')#set manual mode
time.sleep(0.1)

ser.write(b'unlocked=1\r\n')#unlock the controller
time.sleep(1)

ser.write(b'enable1=0\r\n')#close the shutter
time.sleep(1)


ser.write(b'enable1=1\r\n')#open the shutter
time.sleep(1)

ser.write(b'enable1=0\r\n')#close the shutter
time.sleep(1)

ser.close()
del ser