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
            lcc.Open();

            //Read the ID of the port
            lcc.Write("\r\n");
            Thread.Sleep(1000);
            string output = lcc.ReadLine();
            Console.WriteLine(output);
            Thread.Sleep(1000);
            try
            {
                output = lcc.ReadLine();
                Console.WriteLine(output);
            }
            catch (Exception)
            { Console.WriteLine("Timeout"); }


            //Read the ID of the port
            lcc.Write("*idn?\r");
            Thread.Sleep(2000);
            try
            {
                output = lcc.ReadLine();
                Console.WriteLine(output);
            }
            catch (Exception)
            { Console.WriteLine("Timeout"); }
            try
            {
                output = lcc.ReadLine();
                Console.WriteLine(output);
            }
            catch (Exception)
            { Console.WriteLine("Timeout"); }

            /*
                        //Sets voltage 1 and then gets it
                        lcc.Write("volt1=10\r\n");
                        Thread.Sleep(1000);
                        lcc.Write("volt1?\r\n");
                        Thread.Sleep(1000);
                        output = lcc.ReadLine();
                        Console.WriteLine(output);*/

            lcc.Close();
            Console.ReadKey();
        }
    }
}
