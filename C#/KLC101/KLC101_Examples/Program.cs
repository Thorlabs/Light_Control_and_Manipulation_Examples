using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLC101_Examples
{
    class Program
    {
        //Imports for method in C++ based dll
        [DllImport("KLCCommandLib_x64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "List")]
        public static extern int List(ref char[] list, int listSize);

        [DllImport("KLCCommandLib_x64.dll", EntryPoint = "Open")]
        public static extern int Open(char[] deviceString, int baudRate, int timeout);

        [DllImport("KLCCommandLib_x64.dll", EntryPoint = "Close")]
        public static extern int Close(int deviceHandle);

        [DllImport("KLCCommandLib_x64.dll", EntryPoint = "SetEnable")]
        public static extern int SetEnable(int deviceHandle, char enableState);

        [DllImport("KLCCommandLib_x64.dll", EntryPoint = "SetVoltage1")]
        public static extern int SetVoltage1(int deviceHandle, float voltage);

        static void Main(string[] args)
        {
            //Check for connected devices. If none are found, close
            char[] list = new char[2048];
            int numDevices = List(ref list, 2048);
            Console.WriteLine("Number of connected devices: {0}", numDevices);
            Console.WriteLine(list);

            if (numDevices == 0)
            {
                Console.WriteLine("No Devices connected");
                return;
            }

            //Get a handle to the device
            //int deviceHandle = Open("COM5".ToCharArray(), 9600, 3000);


            //Enable the device
            //SetEnable(deviceHandle, '1') ;

            //Set Voltage
            //SetVoltage1(deviceHandle, 5.5f);

            //Close the device
            //Close(deviceHandle);

        }
    }
}
