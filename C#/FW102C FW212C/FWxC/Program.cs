// Title: FW102C/FW212C Csharp serial command example. 
// Created Date: 2024 - 09 - 20
// Last modified date: 2024 - 10 - 12
// .NET version: 4.7.2
// Tested with FW102C
// Notes:The example connects to a Thorlabs FW102C or FW212C Motorized Filter Wheel,
// set necessary parameters and set the filter position. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace FWxC
{
    internal class Program
    {
        //Set Trigger mode
        //0 = set the external trigger to input mode, 1 = set the external trigger to output mode
        static string triggerMode = "0";

        //Set Speed mode
        //0 = slow speed, 1 = high speed
        static string speedMode = "1";

        //Set Sensor mode
        //0 = Sensors turn off when wheel is idle to eliminate stray light, 1 = sensors remain active
        static string sensorMode = "0";

        //Shared Variables
        static SerialPort device;

        static int Main(string[] args)
        {
            //Set the port name
            Console.Write("Enter name of port to connect (e.g. COM3): ");
            string portName = Console.ReadLine().Trim();

            //Set the serial port parameters
            device = new SerialPort
            {
                PortName = portName,
                DataBits = 8,
                StopBits = StopBits.One,
                BaudRate = 115200,
                Parity = Parity.None,
                Handshake = Handshake.None,
                ReadTimeout = 1000,
                WriteTimeout = 500,
                RtsEnable = true,
                NewLine = "\r" // Returns from the filter wheel are ended with a carriage return character
            };

            try
            {
                device.Open();
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return -1;
            }

            //All commands will return an echo of the sent string before returning any other data. This is used to store that if needed.
            string echo;

            //Clear buffers;
            device.DiscardInBuffer();
            device.DiscardOutBuffer();

            try
            {
                //Read the ID
                device.Write("id?\r");
                Thread.Sleep(300);
                echo = device.ReadLine();
                Console.WriteLine("Device is a: " + device.ReadLine());

                //Set the trigger mode
                device.Write("trig=" + triggerMode + "\r");
                if (triggerMode == "0")
                {
                    //while switching the trigger mode from output mode to input mode,
                    //the wheel position will move to the next position automatically.
                    //wait for 1.5 seconds for the wheel to stop moving
                    Thread.Sleep(1500);
                }
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the trigger mode.");
                }

                //Set speed mode
                device.Write("speed=" + speedMode + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the speed.");
                }

                //Set sensor mode
                device.Write("sensors=" + sensorMode + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the sensor mode.");
                }

                //Get position
                device.Write("pos?\r");
                Thread.Sleep(300);
                echo = device.ReadLine();
                Console.WriteLine("Current Filter Position:" + device.ReadLine());

                //Get position count
                device.Write("pcount?\r");
                Thread.Sleep(300);
                echo = device.ReadLine();
                int posCount = Convert.ToInt16(device.ReadLine());

                //Set Position
                Console.Write("Enter the target position(1 - {0:D}):", posCount);
                string targetPos = Console.ReadLine();
                device.Write("pos=" + targetPos + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to move to the target position.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Clear buffers;
            device.DiscardInBuffer();
            device.DiscardOutBuffer();

            //Close the port
            Thread.Sleep(300);
            device.Close();
            Console.WriteLine("\nProgram finished.");
            Console.ReadLine();
            return 1;

        }
    }
}
