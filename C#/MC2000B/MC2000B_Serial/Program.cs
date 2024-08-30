// Title: MC2000B Csharp serial command example. 
// Created Date: 2024 - 08 - 28
// Last modified date: 2024 - 08 - 28
// .NET version: 4.7.2
// Tested with MC1F10HP blade
// Notes:The example connects to a Thorlabs MC2000B chopper, set necessary parameters and display the frequency. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

namespace MC2000B_Serial
{
    internal class Program
    {
        //Set the blade Type
        //0:MC1F2, 1:MC1F10, 2:MC1F15, 3:MC1F30, 4:MC1F60, 5:MC1F100, 6:MC1F10HP (The default blade), 7:MC1F2P10,
        //8:MC1F6P10, 9:MC1F10A, 10:MC2F330, 11:MC2F47, 12:MC2F57B, 13:MC2F860, 14:MC2F5360
        static string bladeType = "6"; //MC1F10HP (The default blade)

        //Set the Reference-In Signal to INT-OUTER
        //For MC1F10HP: 0 = INT-OUTER, 1 = INT-INNER, 2 = EXT-OUTER, 3 = EXT-INNER
        //Please find the available reference-in signal for other blades from Chapter 8.3. of the manual
        //The manual can be found here: https://www.thorlabs.com/thorproduct.cfm?partnumber=MC2000B
        static string refIn = "0";

        //Set the Reference-Out Signal to Outer
        //For MC1F10HP: 0 = Target, 1 = Outer, 2 = Inner
        //Please find the available reference-in signal for other blades from Chapter 8.4. of the manual
        //The manual can be found here: https://www.thorlabs.com/thorproduct.cfm?partnumber=MC2000B
        static string refOut = "1";

        //Shared Variables
        static SerialPort device;
        //a shared variable to control when to stop the data fetching and exit the program.
        static bool running = true;

        static int Main()
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
                NewLine = "\r" // Returns from the MC2000B are ended with a carriage return character
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
                Thread.Sleep(100);
                echo = device.ReadLine();
                Console.WriteLine("Device is a: " + device.ReadLine());

                //Set the internal reference frequency
                Console.Write("Set the frequency(Hz):");
                string frequency = Console.ReadLine();
                device.Write("freq=" + frequency +"\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the frequency.");
                }

                //Set the Blade Type
                device.Write("blade=" + bladeType + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the blade type.");
                }

                //Set the Reference-In Signal
                device.Write("ref=" + refIn + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the reference-in signal.");
                }

                //Set the Reference-Out Signal
                device.Write("output=" + refOut + "\r");
                Thread.Sleep(300);
                echo = device.ReadExisting();
                if (echo.Contains("error"))
                {
                    Console.WriteLine("Fail to set the reference-out signal.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Clear buffers;
            device.DiscardInBuffer();
            device.DiscardOutBuffer();

            try
            {
                //Set Enable
                Console.WriteLine("Enter \"Y\" to enable the Chopper. Enter any other character to close the program. ");
                string enable = Console.ReadLine();
                if (enable == "Y" || enable == "y")
                {
                    EnableDevice();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Close the deivce
            device.Write("enable=0\r");//0 = disabled, 1 = enabled
            Thread.Sleep(300);
            device.Close();
            Console.WriteLine("\nProgram finishes.");
            Console.ReadLine();
            return 1;

        }

        static int EnableDevice()
        {
            //Enable the device
            device.Write("enable=1\r");//0 = disabled, 1 = enabled
            Thread.Sleep(300);
            string echo = device.ReadLine();
            //Check if the device is enabled
            device.Write("enable?\r");
            Thread.Sleep(300);
            echo = device.ReadLine();
            string isEnabled = device.ReadLine();
            if (isEnabled == "1")
            {
                Console.WriteLine("The chopper is enabled. Press \"s\" key to stop the chopper.");

                //Clear buffers;
                device.DiscardInBuffer();
                device.DiscardOutBuffer();

                //Start fetching the reference out frequency
                Task task = Task.Run(() => FetchFrequency());

                //Stop fetching if "s" is pressed
                while (running)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(intercept: true).Key;
                        //Stop fetching the reference out frequency
                        if (key == ConsoleKey.S) running = false;
                    }
                }
                return 1;
            }
            else
            {
                Console.WriteLine("Fail to enable the chopper");
                return -1;
            }

            
        }

        static void FetchFrequency()
        {
            string echo;
            //Get the cursor position
            int currentLine = Console.CursorTop;
            int currentColumn = Console.CursorLeft;

            //Get the reference out frequency
            device.Write("refoutfreq?\r");
            echo = device.ReadLine();

            while (running)
            {
                Console.Write("\rThe reference out frequency is " + device.ReadLine() + " Hz. ");
                //Get the reference out frequency
                device.Write("refoutfreq?\r");
                echo = device.ReadLine();
                Thread.Sleep(100);

                //Clear the text by filling with spaces
                Console.Write("\r" + new string(' ', Console.WindowWidth));
                //Reset the cursor position
                Console.SetCursorPosition(currentColumn, currentLine);
            }
        }
    }
}
