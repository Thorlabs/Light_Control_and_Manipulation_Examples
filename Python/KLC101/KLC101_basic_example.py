import sys
import time
from ctypes import *


try:
    from KLCCommandLib64 import *
    import time
except OSError as ex:
    print("Warning:",ex)


def main():
    try:
        #Find devices
        devs = klcListDevices()
        print("Found devices:",devs,"\n")
        if(len(devs)<=0):
           print('There is no device connected')
           sys.exit()
        klc = devs[0]
        serialnumber = klc[0]
        
        #Connect device
        KLC_handle = klcOpen(serialnumber, 115200, 3)
        if(KLC_handle<0):
            print("open ", serialnumber, " failed")
            sys.exit()
        if(klcIsOpen(serialnumber) == 0):
            print("klcIsOpen failed")
            klcClose(KLC_handle)
            sys.exit()
        print("Connected to serial number ", serialnumber)

        # ------------ Disable/Enable global output -------------- #          
        klcSetEnable(KLC_handle,2)  # 1 enable, 2 disable

        print("Enable output\n")
        if(klcSetEnable(KLC_handle, 1)<0):
            print("klcSetEnable failed")

        en=[0]
        if(klcGetEnable(KLC_handle, en)<0):
            print("klcGetEnable failed")

        # ------------ Preset1 mode ----------------------------- # 
        print("Enable V1")
        klcSetChannelEnable(KLC_handle, 1) # 0x01 V1 Enable , 0x02  V2 Enable, 0x03  SW Enable, 0x00 channel output disable.
        
        #Set voltage 1 
        if(klcSetVoltage1(KLC_handle, 1)<0):
            print("klcSetVoltage1 failed")
        vol1=[0]
        if(klcGetVoltage1(KLC_handle, vol1)<0):
            print("klcGetVoltage1 failed")
        else:
            print("Set voltage 1 to ",vol1[0],"V")

        #set freqency 1
        if(klcSetFrequency1(KLC_handle, 2000)<0):
            print("klcSetFrequency1 failed")

        freq1=[0]
        if(klcGetFrequency1(KLC_handle, freq1)<0):
            print("klcGetFrequency1 failed")
        else:
            print("Frequency 1 set to", freq1[0]," Hz\n")

        time.sleep(1)
    

        # ------------ Preset2 mode ----------------------------- #
        print("Enable V2")
        klcSetChannelEnable(KLC_handle, 2)
        #set voltage 2 
        if(klcSetVoltage2(KLC_handle, 10)<0):
            print("klcSetVoltage2 failed")
        vol2=[0]
        if(klcGetVoltage2(KLC_handle, vol2)<0):
            print("klcGetVoltage2 failed")
        else:
            print("Set voltage 2 to", vol2[0], " V")

        #set freq2 
        if(klcSetFrequency2(KLC_handle, 2000)<0):
            print("klcSetFrequency2 failed")

        freq2=[0]
        if(klcGetFrequency2(KLC_handle, freq2)<0):
            print("klcGetFrequency2 failed")
        else:
            print("Frequency 2 set to ", freq2[0]," Hz\n")
       
        time.sleep(1)
        
        print("Enable V1")
        klcSetChannelEnable(KLC_handle, 1)

        time.sleep(1)

        print("Enable V2")
        klcSetChannelEnable(KLC_handle, 2)
        
        time.sleep(1)

        

        #close connection to device
        klcClose(KLC_handle)
        print("Connection closed")

    except Exception as ex:
        print("Warning:", ex)

if __name__ == "__main__":
    main()
