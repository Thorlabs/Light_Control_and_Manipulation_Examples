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
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;

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

            //select the operation mode
            Console.WriteLine("Enter the number to choose the operation mode:");
            Console.WriteLine("1. Manual mode");
            Console.WriteLine("2. Sequence mode, with internal trigger");
            Console.WriteLine("3. Sequence mode, externally triggered");
            
            try 
            {
                //record the selected operation mode
                int mode = Convert.ToInt32(Console.ReadLine());

                switch (mode)
                {
                    case 1:
                        //set the operation mode to manual mode
                        ManualSet();
                        KURIOS.Write("OM=1\r");
                        string echo = KURIOS.ReadLine();
                        break;
                    case 2:
                        //set the operation mode to sequence mode with internal trigger
                        SequenceInt();
                        KURIOS.Write("OM=2\r");
                        echo = KURIOS.ReadLine();
                        Console.WriteLine("The sequence is set successfully.");
                        break;
                    case 3:
                        //set the opertation mode to sequence mode with external trigger
                        //filter output wavelength switches according to the External trigger but NOT the interval time
                        SequenceExt();
                        KURIOS.Write("OM=3\r");
                        echo = KURIOS.ReadLine();
                        Console.WriteLine("The sequence is set successfully.");
                        break;
                }
                Console.WriteLine("Press ANY KEY to exit");
                Console.ReadKey();
                KURIOS.Close();
            }
            catch
            {
                Console.WriteLine("Invalid number! The program will close.");
                KURIOS.Close();
                Console.ReadKey();
                return;
            }

        }


        /// <summary>
        /// Initialize the KURIOS.
        /// <para>The operation mode is set to manual mode.</para>
        /// The sequence are all deleted.
        /// </summary>
        private static void KURIOSInitialization()
        {
            KURIOS.Open();

            //Get device ID
            KURIOS.Write("*idn?\r");
            //Returns the model number, hardware and firmware versions
            Console.WriteLine("Device is a: " + KURIOS.ReadLine() + "\n");

            //Get Status
            KURIOS.Write("ST?\r");
            //Returns current filter status: 0 - initialization; 1 - warm up; 2 - ready
            //The format of the return statement is "ST=n". 
            //only the number will be recorded, so the string before the number needs to be skipped.
            int status = Convert.ToInt16(KURIOS.ReadLine().Substring(4));

            while (status == 0)
            {
                Console.Write("\rdevice is initializing. Please wait.  ");
                Thread.Sleep(500);
                Console.Write("\rdevice is initializing. Please wait.. ");
                Thread.Sleep(500);
                Console.Write("\rdevice is initializing. Please wait...");
                Thread.Sleep(500);
                //Get Status
                KURIOS.Write("ST?\r");
                status = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            }

            //Set the operation mode to manual mode
            KURIOS.Write("OM=1\r");
            //After a command is accepted by the controller, a prompt symbol (>) appears, indicating it is ready to receive the next command.
            string echo = KURIOS.ReadLine();

            //Clear all the sequence
            KURIOS.Write("DS=0\r");
            echo = KURIOS.ReadLine();
        }

        /// <summary>
        /// <para>This method corresponds to the parameter settings for Manual Mode. </para>
        /// The wavelength and bandwidth can be modified repeatedly, until manually commands the program to stop.
        /// </summary>
        private static void ManualSet()
        {
            //Get wavelength range
            KURIOS.WriteLine("SP?");
            Thread.Sleep(500);
            //Returns connected filter's wavelength range
            //The format of the return statement is:
            //WLmax = 730.000 
            //WLmin = 420.000
            //only the number will be recorded, so the string before the number needs to be skipped.
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            //Get Optical Head Type
            KURIOS.WriteLine("OH?");
            //Returns two bytes integer in form of "n = 0000 0000 0000 0000"
            //Bottom 8 bits represents available bandwidth mode:
            //0000 0001 = BLACK
            //0000 0010 = WIDE
            //0000 0100 = MEDIUM
            //0000 1000 = NARROW
            //only record the number and convert the value into binary form
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);

            //verify if the specs entered are available, if not, the user needs to enter again
            bool judge = true;

            while (judge)
            {
                Console.WriteLine("Enter the wavelenth (nm). {0} nm <= wavelength <= {1} nm",wavelengthMin,wavelengthMax);
                string wavelength = Console.ReadLine();

                //save the entered wavelength if it's within the range
                while (Convert.ToDouble(wavelength) < wavelengthMin || Convert.ToDouble(wavelength) > wavelengthMax) 
                {
                    Console.WriteLine("Invalid wavelength.");
                    Console.WriteLine("Enter the wavelenth (nm). {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
                    wavelength = Console.ReadLine();
                }

                //write the wavelength to the device
                KURIOS.WriteLine("WL=" + wavelength);
                //After a command is accepted by the controller, a prompt symbol (>) appears, indicating it is ready to receive the next command.
                string echo = KURIOS.ReadLine();
                Thread.Sleep(500);

                //get wavelength
                //Returns the current wavelength in form of WL=n
                KURIOS.WriteLine("WL?");
                Console.WriteLine("The wavelength is:" + KURIOS.ReadLine().Substring(4) + " nm.");

                string bandwidth="";
                Console.WriteLine("Please choose the bandwidth mode.");
                //read the selectable bandwidth verify if the mode selected is available
                //Only the last four bits of HeadType contains the informations about the available bandwidth modes
                //0001 = BLACK
                //0010 = WIDE
                //0100 = MEDIUM
                //1000 = NARROW
                switch (HeadType.Substring(HeadType.Length - 4))
                {
                    case "0011":
                        //in this case, BLACK mode and WIDE mode are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2
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
                        //in this case, BLACK mode and NARROW mode are available
                        //the decimal number of 0001 is 1; the decimal number of 1000 is 8
                        Console.WriteLine("1. BLACK mode, 8. NARROW mode");
                        bandwidth = Console.ReadLine();
                        while (bandwidth != "1" && bandwidth != "8")
                        {
                            Console.WriteLine("Invalid bandwidth. ");
                            Console.WriteLine("Please choose the bandwidth. ");
                            bandwidth = Console.ReadLine();
                        }
                        break;
                    case "1111":
                        //in this case, all four modes are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2; the decimal number of 0100 is 4;the decimal number of 1000 is 8
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
                    Console.WriteLine("Invalid number! The program will close.");
                    judge = false;
                }

            }

        }

        /// <summary>
        /// <para>This method corresponds to the parameter settings for Internal Triggered Sequence Mode. </para>
        /// Up to 1024 wavelengths can be set. The time interval and bandwidth mode for each wavelength can be set.
        /// </summary>
        private static void SequenceInt()
        {
            //Get wavelength range
            KURIOS.WriteLine("SP?");
            //Returns connected filter's wavelength range
            //The format of the return statement is:
            //WLmax = 730.000 
            //WLmin = 420.000
            //only the number will be recorded, so the string will be skipped.
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            //Get Optical Head Type
            KURIOS.WriteLine("OH?");
            //Returns two bytes integer in form of "n = 0000 0000 0000 0000"
            //Bottom 8 bits represents available bandwidth mode:
            //0000 0001 = BLACK
            //0000 0010 = WIDE
            //0000 0100 = MEDIUM
            //0000 1000 = NARROW
            //only record the number and convert the value into binary form
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);

            Console.WriteLine("\nPlease enter the number of the sequence. 1<= number <= 1024");
            int SequenceNumber = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("The wavelength, interval, and bandwidth of each sequence should be set. Only numbers and semicolons can be entered. e.g. 550;50;2");
            Console.WriteLine("*The wavelength is in nm. {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
            Console.WriteLine("*The interval is in ms. 1 ms <= interval <= 60000 ms");
            Console.WriteLine("*The bandwidth has four modes. The available bandwidth modes for the connected filter are: ");

            //Prompt the availabel bandwidth modes
            //Only the last four bits of HeadType contains the informations about the available bandwidth modes
            //0001 = BLACK
            //0010 = WIDE
            //0100 = MEDIUM
            //1000 = NARROW
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
                    string[] InputValue = cache.Split(';');

                    //prompt if all entered values are available 
                    bool WavelengthValid = true;
                    bool IntervalValid = true;
                    bool BandwidthValid = true;

                    //verify if the wavelength is in the range
                    if (Convert.ToDouble(InputValue[0]) < wavelengthMin || Convert.ToDouble(InputValue[0]) > wavelengthMax)
                    {
                        Console.Write("Invalid wavelength. ");
                        WavelengthValid = false;
                    }

                    //verify if the time interval is in the range
                    if (Convert.ToDouble(a[1]) < 1 || Convert.ToDouble(a[1]) > 60000)
                    {
                        Console.Write("Invalid interval. ");
                        IntervalValid = false;
                    }

                    //verify if the mode selected is available
                    //Only the last four bits of HeadType contains the informations about the available bandwidth modes
                    //0001 = BLACK
                    //0010 = WIDE
                    //0100 = MEDIUM
                    //1000 = NARROW
                    if (HeadType.Substring(HeadType.Length - 4) == "0011")
                    {
                        //in this case, BLACK mode and WIDE mode are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2
                        if (a[2] != "1" && a[2] != "2")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1001")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        //the decimal number of 0001 is 1; the decimal number of 1000 is 8
                        if (InputValue[2] != "1" && InputValue[2] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }   
                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1111")
                    {
                        //in this case, all four modes are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2; the decimal number of 0100 is 4;the decimal number of 1000 is 8
                        if (InputValue[2] != "1" && InputValue[2] != "2" && InputValue[2] != "4" && InputValue[2] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                    }

                    if (WavelengthValid == true && IntervalValid == true && BandwidthValid == true)
                    {
                        //write the data of sequence<index> to the device
                        //The Command is "SS = index wavelength interval bandwidth"
                        KURIOS.WriteLine("SS="+ Convert.ToString(i)+ " " + InputValue[0] + " " + InputValue[1] + " " + InputValue[2] + "*");
                        //After a command is accepted by the controller, a prompt symbol (>) appears, indicating it is ready to receive the next command.
                        string echo = KURIOS.ReadLine();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.Write("Please enter again.\n");
                        i = i - 1;
                    }
                }
                catch
                {
                    Console.WriteLine("The format of the entered string is invalid. Please inspect and enter again.");
                    i = i-1;

                }
            }

        }

        /// <summary>
        /// <para>This method corresponds to the parameter settings for External Triggered Sequence Mode. </para>
        /// Up to 1024 wavelengths can be set. The bandwidth mode for each wavelength can be set.The time interval for each wavelength is controlled by the interval of the trigger.
        /// </summary>
        private static void SequenceExt()
        {
            //Get wavelength range
            KURIOS.WriteLine("SP?");
            //Returns connected filter's wavelength range
            //The format of the return statement is:
            //WLmax = 730.000 
            //WLmin = 420.000
            //only the number will be recorded, so the string will be skipped.
            double wavelengthMax = Convert.ToDouble(KURIOS.ReadLine().Substring(7));
            double wavelengthMin = Convert.ToDouble(KURIOS.ReadLine().Substring(6));

            //Get Optical Head Type
            KURIOS.WriteLine("OH?");
            //Returns two bytes integer (int 16)
            //Bottom 8 bits represents available bandwidth mode:
            //0000 0001 = BLACK
            //0000 0010 = WIDE
            //0000 0100 = MEDIUM
            //0000 1000 = NARROW
            //only record the bottom 8 bits and convert the values into binary form
            int HeadTypeInt = Convert.ToInt16(KURIOS.ReadLine().Substring(4));
            string HeadType = Convert.ToString(HeadTypeInt, 2);

            Console.WriteLine("\nPlease enter the number of the sequence. 1<= number <= 1024");
            int SequenceNumber = Convert.ToInt16(Console.ReadLine());

            Console.WriteLine("The wavelength, and bandwidth of each sequence should be set. Only numbers and semicolons can be entered. e.g. 550;2");
            Console.WriteLine("*The wavelength is in nm. {0} nm <= wavelength <= {1} nm", wavelengthMin, wavelengthMax);
            Console.WriteLine("*The bandwidth has four modes. The available bandwidth modes for the connected device are: ");

            //Prompt the availabel bandwidth modes
            //Only the last four bits of HeadType contains the informations about the available bandwidth modes
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
                    string[] InputValue = cache.Split(';');

                    //prompt if all entered values are available 
                    bool WavelengthValid = true;
                    bool BandwidthValid = true;

                    //verify if the wavelength is in the range
                    if (Convert.ToDouble(InputValue[0]) < wavelengthMin || Convert.ToDouble(InputValue[0]) > wavelengthMax)
                    {
                        Console.Write("Invalid wavelength. ");
                        WavelengthValid = false;
                    }

                    //verify if the mode selected is available
                    //Only the last four bits of HeadType contains the informations about the available bandwidth modes
                    //0001 = BLACK
                    //0010 = WIDE
                    //0100 = MEDIUM
                    //1000 = NARROW
                    if (HeadType.Substring(HeadType.Length - 4) == "0011")
                    {
                        //in this case, BLACK mode and WIDE mode are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2
                        if (InputValue[1] != "1" && InputValue[1] != "2")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1001")
                    {
                        //in this case, BLACK mode and NARROW mode are available
                        //the decimal number of 0001 is 1; the decimal number of 1000 is 8
                        if (InputValue[1] != "1" && InputValue[1] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                    }
                    else if (HeadType.Substring(HeadType.Length - 4) == "1111")
                    {
                        //in this case, all four modes are available
                        //the decimal number of 0001 is 1; the decimal number of 0010 is 2; the decimal number of 0100 is 4;the decimal number of 1000 is 8
                        if (InputValue[1] != "1" && InputValue[1] != "2" && InputValue[1] != "4" && InputValue[1] != "8")
                        {
                            Console.Write("Invalid bandwidth. ");
                            BandwidthValid = false;
                        }
                    }

                    if (WavelengthValid == true && BandwidthValid == true)
                    {
                        //write the data of sequence<index> to the device
                        //The Command is "SS = index wavelength interval bandwidth"
                        //set the interval to default value 50 ms. This isn't the real interval. The real interval is controlled by the trigger
                        KURIOS.WriteLine("SS=" + Convert.ToString(i) + " " + InputValue[0] + " 50 " + InputValue[1] + "*");
                        //After a command is accepted by the controller, a prompt symbol (>) appears, indicating it is ready to receive the next command.
                        string echo = KURIOS.ReadLine();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.Write("Please enter again.\n");
                        i = i - 1;
                    }
                }
                catch
                {
                    Console.WriteLine("The format of the entered string is invalid. Please inspect and enter again.");
                    i = i - 1;
                }
            }

        }

    }
}
