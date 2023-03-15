import sys
from ctypes import *


def main():
    lib = cdll.LoadLibrary(r"C:\Program Files (x86)\Thorlabs\LCC25\Sample\Thorlabs_LCC25_C++SDK\LCC25CommandLib_x64.dll")


    print("*** LCC25 simple python example ***")
    try:
        str = create_string_buffer(1024, '\0')
        result=lib.List(str,c_int(1024))
        if (result==0):
            print("no device found")
            sys.exit()
        
        #Connects to first device
        serialnumber=str.raw.decode("utf-8","ignore")[0:8]
        print("Connecting serial number ", serialnumber)

        baudrate=c_int(115200)
        timeout=c_int(3)
        lcc=lib.Open(serialnumber.encode('utf-8'), baudrate, timeout)
        if (lcc<0):
            print("open failed")
            sys.exit()
        if (lib.IsOpen(serialnumber)==0):
            print("IsOpen failed")
            lib.Close(lcc)
            sys.exit()

        #Set Voltage 1
        voltage1=c_double(14)#Voltage1 = 14 V
        lib.SetVoltage1(c_int(lcc), voltage1)

        #Read Voltage 1
        voltage1set = c_double(0)
        ret = lib.GetVoltage1(c_int(lcc), byref(voltage1set))
        print("Voltage 1 set to ",voltage1set.value,"V")
        
        #Set OutputMode
        outputmode=c_int(1)# 0:modulation,1:voltage1,2:voltage2
        result=lib.SetOutputMode(c_int(lcc), outputmode)
        if(result<0):
           print("Set OutputMode failed", result)
        else:
           print("Set OutputMode to Voltage1" )
    

        lib.Close(lcc)

    
    except Exception as ex:
        print("Warning:", ex)


if __name__ == "__main__":
    main()
