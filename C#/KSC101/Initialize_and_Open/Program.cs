using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.KCube.SolenoidCLI;

namespace Initialize_and_Open
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Try building the device list. Close if this fails
            try
            {
                DeviceManagerCLI.BuildDeviceList();
            }
            catch (Exception)
            {
                Console.WriteLine("Device list failed to build");
                return;
            }

            //Get all available devices that are KSC controllers. These are identified by the first 2 digits of the serial number. 
            List<string> serialNumbers = DeviceManagerCLI.GetDeviceList(68);

            //Close if no KSC devices are connected. 
            if (serialNumbers.Count > 0)
            {
                Console.WriteLine(serialNumbers[0]);
            }
            else
            {
                Console.WriteLine("No connected devices");
                return;
            }

            KCubeSolenoid ksc = KCubeSolenoid.CreateKCubeSolenoid(serialNumbers[0]);

            //Check if created device is null
            if (ksc != null)
            {
                ksc.Connect(serialNumbers[0]);
                ksc.WaitForSettingsInitialized(5000);
                ksc.StartPolling(100);
                Thread.Sleep(500);
                ksc.EnableDevice();
                Thread.Sleep(500);

                // get Device Configuration to load settings for the controller
                SolenoidConfiguration solenoidConfiguration = ksc.GetSolenoidConfiguration(serialNumbers[0]);
                ThorlabsKCubeSolenoidSettings currentDeviceSettings = ThorlabsKCubeSolenoidSettings.GetSettings(solenoidConfiguration);

                // Set the controller to operate in manual mode. In this mode, the state must be set manually. 
                ksc.SetOperatingMode(SolenoidStatus.OperatingModes.Manual);

                //Loop to turn the shutter on/off 10 times
                for (int i = 0; i < 10; i ++)
                {
                    ksc.SetOperatingState(SolenoidStatus.OperatingStates.Active);
                    Thread.Sleep(500);
                    ksc.SetOperatingState(SolenoidStatus.OperatingStates.Inactive);
                    Thread.Sleep(500);
                }

                // Tidy up and exit
                ksc.StopPolling();
                ksc.Disconnect(false);

            }
        }
    }
}
