/*
Example Title
Example Date of Creation(YYYY-MM-DD): 2023-09-27
Example Date of Last Modification on Github: 2023-09-27
Version of .NET Framework: 4.7.2
Version of the Thorlabs SDK used: 1.0.0
==================
Example Description: The example shows how to use the C++ SDK in C#. The connected KLC devices are found. 
The first KLC will be connected, enabled and a voltage is set. 
*/

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace KLC101_sample
{
    class Program
    {
        //Copy the file KLCCommandLib_x64.dll from  C:\Program Files(x86)\Thorlabs\KLC\Sample\Thorlabs_KLC_C++SDK into the project folder
        public const string klcdll = "../../../KLCCommandLib_x64.dll";

       [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int List(StringBuilder listStr, int size);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Open(string serialnumber, int nBaud, int timeout);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IsOpen(string serialnumber);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Close(int handle);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetEnable(int handle, byte enable);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetChannelEnable(int handle, byte enablestate);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetVoltage1(int handle, float voltage);
        [DllImport(klcdll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetVoltage1(int handle, out float voltage);

        static void Main(string[] args)
        {
            char[] buf = new char[2048];
            StringBuilder listStr = new StringBuilder(2048);
            int count = List(listStr, 2048);
            if (count<=0)
            {
                Console.WriteLine("No KLC device found");
                Thread.Sleep(2000);
                System.Environment.Exit(0);               
            }
            Console.WriteLine("{0} device(s) found",count);
           
           
            //open device
            string serialnumber = listStr.ToString().Split(new char[] { ',' })[0];

            int klc_handle = Open(serialnumber, 115200, 3000);

            int opentest = IsOpen(serialnumber);
            Console.WriteLine(opentest.ToString());
            if (opentest == 0)
            {
                Console.WriteLine("Failed to open device.");
                System.Environment.Exit(0);
            }
            Console.WriteLine("Device s/n {0} opened",serialnumber);
            
            //Enable global output
            SetEnable(klc_handle, 1);// 1 enable, 2 disable
            //Enable V1
            SetChannelEnable(klc_handle, 1);// 0x01 V1 Enable, 0x02  V2 Enable, 0x03  SW Enable, 0x00 channel output disable.
            Console.WriteLine("Channel 1 enabled");
            //Set Voltage 1
            SetVoltage1(klc_handle, 3.0f);
            float setvoltage;
            GetVoltage1(klc_handle, out setvoltage);
            Console.WriteLine("Voltage 1 is set to {0} V",setvoltage);
            Thread.Sleep(3000);

            //Close connection
            Close(klc_handle);
            Console.WriteLine("Connection closed");
            Console.ReadKey();

        }
    }
}
