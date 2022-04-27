using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace LCC25_Serial_Commands
{
    internal class Program
    {
        static SerialPort lcc;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter name of port to connect (e.g. COM3): ");
            String portName = Console.ReadLine().Trim();

            lcc = new SerialPort();
            lcc.PortName = portName;
            lcc.DataBits = 8;
            lcc.StopBits = StopBits.One;
            lcc.BaudRate = 115200;
            lcc.Parity = Parity.None;
            lcc.Handshake = Handshake.None;
            lcc.ReadTimeout = 1000;
            lcc.WriteTimeout = 500;
            lcc.RtsEnable = true;
            lcc.NewLine = "\r"; //Returns from the LCC are ended with a carriage return character
            lcc.Open();

            //The "Enter" key needs to be sent to enable communication with the device. This is represented by a single carriage return
            lcc.Write("\r");
            //The return from this character is ended differently than the others so a different method is used to read it out. This always prints "Command error CMD_NOT_DEFINED"
            WriteOutCharacters();

            //All commands will return an echo of the sent string before returning any other data. This is used to store that if needed
            string echo = "";

            //Read the ID of the port
            lcc.Write("*idn?\r");
            echo = lcc.ReadLine();
            Console.WriteLine("Device is a: " + lcc.ReadLine());

            //Set the voltage
            lcc.Write("volt1=15\r");
            echo = lcc.ReadLine();

            //Get the voltage and print the return
            lcc.Write("volt1?\r");
            echo = lcc.ReadLine();
            Console.WriteLine("Returned Voltage is: " + lcc.ReadLine());

            //Close the port
            lcc.Close();
            Console.WriteLine("Press Any Key to Exit");
            Console.ReadKey();
        }

        private static void WriteOutCharacters()
        {
            //Writes out all available characters until the buffer is empty. Characters are stored in a string to be read out at the end. 
            string output = "";
            bool bytesAvailable = true;
            while (bytesAvailable)
            {
                try
                {
                    char curChar = (char)lcc.ReadChar();
                    //Ignore characters that would start or end the line 
                    if (curChar == '>' || curChar == '\n' || curChar == '\r')
                    { }
                    else
                    {
                        output += curChar;
                    }
                }
                catch(Exception) 
                {
                    bytesAvailable = false;
                }
            }
            Console.WriteLine(output);
        }
    }
}
