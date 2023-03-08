using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

//please make sure you have download the System.IO.Ports namespace
//if you are using Visual Studio, you can download it from Tools>>NuGet Package Manager>>Manage NuGet Package for solution.
//In the "Browse" menu, enter "System.IO.Ports", and install the first pacakge named System.IO.Ports.

namespace KURIOS
{
    internal class Program
    {
        static SerialPort KURIOS;

        static void Main(string[] args)
        {
            Console.Write("Enter the name of port to connect (e.g. COM5): ");
            string portName = Console.ReadLine().Trim();

            //serial port configuration
            KURIOS = new SerialPort();
            KURIOS.PortName= portName;
            KURIOS.DataBits = 8;
            KURIOS.Parity = Parity.None;
            KURIOS.StopBits = StopBits.One;
            KURIOS.BaudRate = 115200;
            KURIOS.Handshake= Handshake.None;

            KURIOS.ReadTimeout= 1000;
            KURIOS.WriteTimeout= 500;
            KURIOS.RtsEnable = true;
            KURIOS.NewLine= "\r";

            KURIOSInitialization(); 

            //Select the operation mode
            Console.WriteLine("Enter the number to choose the operation mode:");
            Console.WriteLine("1. Manual mode");
            Console.WriteLine("2. Sequence mode, with internal trigger");
            Console.WriteLine("3. Sequence mode, externally triggered");
            int mode = Convert.ToInt32(Console.ReadLine());

            switch (mode)
            {
                case 1:
                    //set the operation mode to manual mode
                    KURIOS.Write("OM=1\r");
                    string echo = KURIOS.ReadLine();
                    ManualSet();
                    break;
                case 2:
                    //set the operation mode to sequence mode with internal trigger
                    SequenceInt();
                    KURIOS.Write("OM=2\r");
                    echo = KURIOS.ReadLine();
                    break;
                case 3:
                    //set the opertation mode to sequence mode with external trigger
                    //filter output wavelength switches according to the External trigger but NOT the interval time
                    SequenceExt();
                    KURIOS.Write("OM=3\r");
                    echo = KURIOS.ReadLine();
                    break;
                default:
                    Console.WriteLine("invalid number! The program will close.");
                    break;
            }

            Console.WriteLine("Press ANY KEY to exit");
            Console.ReadKey();
            KURIOS.Close();

        }


        /// <summary>
        /// Waiting for the KURIOS to finish the initialization
        /// The the operation mode is set to manual mode
        /// The sequence are all deleted
        /// </summary>
        private static void KURIOSInitialization()
        {
            KURIOS.Open();

            KURIOS.Write("*idn?\r");
            Console.WriteLine("Device is a: " + KURIOS.ReadLine() + "\n");

            KURIOS.Write("ST?\r");
            int status = Convert.ToInt16(KURIOS.ReadLine().Substring(4));

            while (status == 0)
            {
                Console.Write("\rdevice is initializing. Please wait.  ");
                Thread.Sleep(500);
                Console.Write("\rdevice is initializing. Please wait.. ");
                Thread.Sleep(500);
                Console.Write("\rdevice is initializing. Please wait...");
                Thread.Sleep(500);
                KURIOS.Write("ST?\r");
                status = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            }
            //set the operation mode to manual mode
            KURIOS.Write("OM=1\r");
            string echo = KURIOS.ReadLine();

            //clear all the sequence
            KURIOS.Write("DS=0\r");
            echo = KURIOS.ReadLine();
        }

        /// <summary>
        /// This method enables manually setting the wavelengh. 
        /// The wavelength can be modified repeatedly, until manually commands the program to stop.
        /// </summary>
        private static void ManualSet()
        {
            KURIOS.WriteLine("SP?");
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            KURIOS.WriteLine("OH?");
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);
            

            bool judge = true;
            while (judge)
            {
                Console.WriteLine("Enter the wavelenth (nm). {0} nm <= wavelength <= {1} nm",wavelengthMin,wavelengthMax);
                string wavelength = Console.ReadLine();

                while (Convert.ToDouble(wavelength) < wavelengthMin || Convert.ToDouble(wavelength) > wavelengthMax) 
                {
                    Console.WriteLine("Invalid wavelength.");
                    Console.WriteLine("Enter the wavelenth (nm). {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
                    wavelength = Console.ReadLine();
                }

                KURIOS.WriteLine("WL=" + wavelength);
                string echo = KURIOS.ReadLine();
                Thread.Sleep(500);
                KURIOS.WriteLine("WL?");
                Console.WriteLine("The wavelength is:" + KURIOS.ReadLine().Substring(4) + " nm.");


                string bandwidth="";
                Console.WriteLine("Please choose the bandwidth mode.");
                //read the availabel bandwidth modes and write them
                switch (HeadType.Substring(HeadType.Length - 4))
                {
                    case "0011":
                        Console.WriteLine("1. BLACK mode, 2. WIDE mode");
                        bandwidth = Console.ReadLine();
                        while (bandwidth != "1" && bandwidth != "2")
                        {
                            Console.WriteLine("Invalid bandwidth. ");
                            Console.WriteLine("Please choose the bandwidth. ");
                            bandwidth = Console.ReadLine();
                        }
                        break;
                    case "1001":
                        Console.WriteLine("1. BLACK mode, 8. NARROW mode");
                        bandwidth = Console.ReadLine();
                        while (bandwidth != "1" && bandwidth != "2")
                        {
                            Console.WriteLine("Invalid bandwidth. ");
                            Console.WriteLine("Please choose the bandwidth. ");
                            bandwidth = Console.ReadLine();
                        }
                        break;
                    case "1111":
                        Console.WriteLine("1. BLACK mode, 2. WIDE mode, 4. MEDIUM mode, 8. NARROW mode");
                        bandwidth = Console.ReadLine();
                        while (bandwidth != "1" && bandwidth != "2" && bandwidth != "4" && bandwidth != "8")
                        {
                            Console.WriteLine("Invalid bandwidth. ");
                            Console.WriteLine("Please choose the bandwidth. ");
                            bandwidth = Console.ReadLine();
                        }
                        break;
                }  

                

                KURIOS.WriteLine("BW=" + bandwidth);
                echo = KURIOS.ReadLine();
                Thread.Sleep(500);
                KURIOS.WriteLine("BW?");
                Console.WriteLine("The bandwidth mode is:" + KURIOS.ReadLine().Substring(4));


                Console.WriteLine("Do you want to keep changing the wavelength and the bandwidth? (Y/N)");
                string key = Console.ReadLine();
                if (key == "N")
                    judge = false;
                else if (key != "Y")
                {
                    Console.WriteLine("Invalid character! The program will close.");
                    judge = false;
                }

            }

        }

        private static void SequenceInt()
        {
            KURIOS.WriteLine("SP?");
            Thread.Sleep(500);
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            KURIOS.WriteLine("OH?");
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);
            

            Console.WriteLine("\nPlease enter the number of the sequence. 1<= number <= 1024");
            int SequenceNumber = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("The wavelength, interval, and bandwidth of each sequence should be set. Only numbers and semicolons can be entered. e.g. 550;50;2");
            Console.WriteLine("*The wavelength is in nm. {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
            Console.WriteLine("*The interval is in ms. 1 ms <= interval <= 60000 ms");
            Console.WriteLine("*The bandwidth has four modes. The available bandwidth modes for the connected device are: ");

            //read the availabel bandwidth modes and write them
            switch (HeadType.Substring(HeadType.Length - 4))
            {
                case "0011":
                    Console.Write("1. BLACK mode, 2. WIDE mode\n");
                    break;
                case "1001":
                    Console.Write("1. BLACK mode, 8. NARROW mode\n");
                    break;
                case "1111":
                    Console.Write("1. BLACK mode, 2. WIDE mode, 4. MEDIUM mode, 8. NARROW mode\n");
                    break;
            }

            for (int i=1; i <= SequenceNumber; i++)
            {
                Console.WriteLine("Please enter the wavelength, interval, and bandwidth of sequence <{0}>:",i);
                try
                {
                    string cache = Console.ReadLine();
                    string[] a = cache.Split(';');

                    bool WavelengthValid = true;
                    bool IntervalValid = true;
                    bool BandwidthValid = true;

                    if (Convert.ToDouble(a[0]) < wavelengthMin || Convert.ToDouble(a[0]) > wavelengthMax)
                    {
                        Console.Write("Invalid wavelength. ");
                        WavelengthValid = false;
                    }

                    if (Convert.ToDouble(a[1]) < 1 || Convert.ToDouble(a[1]) > 60000)
                    {
                        Console.Write("Invalid interval. ");
                        IntervalValid = false;
                    }

                    if (HeadType.Substring(HeadType.Length - 4) == "0011")
                    {
                        //in this case, BLACK mode and WIDE mode are available
                        if (a[2] != "1" && a[2] != "2")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }

                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1001")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        if (a[2] != "1" && a[2] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                            
                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1111")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        if (a[2] != "1" && a[2] != "8" && a[2] != "1" && a[2] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }

                    }


                    if (WavelengthValid == true && IntervalValid == true && BandwidthValid == true)
                    {
                        KURIOS.WriteLine("SS="+ Convert.ToString(i)+ " " + a[0] + " " + a[1] + " " + a[2] + "*");
                        string echo = KURIOS.ReadLine();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.Write("Please insert again.\n");
                        i = i - 1;
                    }
                    
                }
                catch
                {
                    Console.WriteLine("The format of the entered string is invalid. Please inspect and redo.");
                    i = i-1;

                }
            }
            //sequence test
            //KURIOS.WriteLine("SS1?");
            //echo = KURIOS.ReadLine();
            //Console.WriteLine(KURIOS.ReadLine());


        }

        private static void SequenceExt()
        {
            KURIOS.WriteLine("SP?");
            Thread.Sleep(500);
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            KURIOS.WriteLine("OH?");
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);


            Console.WriteLine("\nPlease enter the number of the sequence. 1<= number <= 1024");
            int SequenceNumber = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("The wavelength, and bandwidth of each sequence should be set. Only numbers and semicolons can be entered. e.g. 550;2");
            Console.WriteLine("*The wavelength is in nm. {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
            Console.WriteLine("*The bandwidth has four modes. The available bandwidth modes for the connected device are: ");

            //read the availabel bandwidth modes and write them
            switch (HeadType.Substring(HeadType.Length - 4))
            {
                case "0011":
                    Console.Write("1. BLACK mode, 2. WIDE mode\n");
                    break;
                case "1001":
                    Console.Write("1. BLACK mode, 8. NARROW mode\n");
                    break;
                case "1111":
                    Console.Write("1. BLACK mode, 2. WIDE mode, 4. MEDIUM mode, 8. NARROW mode\n");
                    break;
            }

            for (int i = 1; i <= SequenceNumber; i++)
            {
                Console.WriteLine("Please enter the wavelength and bandwidth of sequence <{0}>:", i);
                try
                {
                    string cache = Console.ReadLine();
                    string[] a = cache.Split(';');

                    bool WavelengthValid = true;
                    bool BandwidthValid = true;

                    if (Convert.ToDouble(a[0]) < wavelengthMin || Convert.ToDouble(a[0]) > wavelengthMax)
                    {
                        Console.Write("Invalid wavelength. ");
                        WavelengthValid = false;
                    }

                    if (HeadType.Substring(HeadType.Length - 4) == "0011")
                    {
                        //in this case, BLACK mode and WIDE mode are available
                        if (a[1] != "1" && a[1] != "2")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }

                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1001")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        if (a[1] != "1" && a[1] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }

                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1111")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        if (a[1] != "1" && a[1] != "8" && a[1] != "1" && a[1] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }

                    }

                    if (WavelengthValid == true && BandwidthValid == true)
                    {
                        //set the interval to default value 50 ms. This isn't the real interval. The real interval is controlled by the trigger
                        KURIOS.WriteLine("SS=" + Convert.ToString(i) + " " + a[0] + " 50 " + a[1] + "*");
                        string echo = KURIOS.ReadLine();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.Write("Please insert again.\n");
                        i = i - 1;
                    }

                }
                catch
                {
                    Console.WriteLine("The format of the entered string is invalid. Please inspect and redo.");
                    i = i - 1;

                }
            }
            //sequence test
            //KURIOS.WriteLine("SS1?");
            //echo = KURIOS.ReadLine();
            //Console.WriteLine(KURIOS.ReadLine());


        }

    }
}
