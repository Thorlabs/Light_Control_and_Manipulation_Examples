"""
Example Title: OSW_example.py
Example Date of Creation: 2024-08-08
Example Date of Last Modification on Github:2024-08-08 
Version of Python used for Testing: 3.11
Version of the Thorlabs SDK used: --
Tested with OSW12-830E
==================
Example Description: Shows board type and how to switch the state with serial commands.
"""
import serial
import time

from serial.tools import list_ports

def main():
    plist = list(list_ports.comports())
    for comport in plist:
        if 'OSW' in comport.description:
            #print(comport.device,comport.description)
            OSW_comport = comport.device

    ser = serial.Serial(OSW_comport, 115200, timeout=2, rtscts=1) #Also opens the connection to the switch

    if not ser.is_open:
        print('Opening connection to switch')
        ser.open()


    # Valid commands
    command_setSwitchStateTo2 = b'S 2\n'
    command_setSwitchStateTo1 = b'S 1\n'
    command_querySwitchState = b'S? \n'
    command_queryTypeCode = b'T? \n'
    command_queryOSWboardName = b'I? \n'

    ser.write(command_queryOSWboardName) #Send command to OSW device
    time.sleep(0.1) #Ensure enough time passes for response.
    response = ser.read(100).decode("ascii") #Read response and decode
    print('Board name:',response)

    ser.write(command_queryTypeCode) 
    time.sleep(0.1)
    response = ser.read(100).decode("ascii")
    print('OSW type code:',response)

    ser.write(command_querySwitchState)
    time.sleep(0.1)
    response = ser.read(100).decode("ascii")
    print('Switch state:',response)

    if response == '1\r\n':
        ser.write(command_setSwitchStateTo2)
    else:
        ser.write(command_setSwitchStateTo1)

    ser.write(command_querySwitchState)
    time.sleep(0.1)
    response = ser.read(100).decode("ascii")
    print('New switch state: ',response)

    print('Closing connection to switch')
    ser.close()

if __name__ == "__main__":
    main()