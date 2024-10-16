//Example Date of Creation(YYYY - MM - DD) 2024 - 09 - 06
//Example Date of Last Modification on Github 2024 - 10 - 12
//Version of C++ used for Testing and IDE: C++ 14, Visual Studio 2022
//Version of the Thorlabs SDK used : Thorlabs MC2000B Software Version 2.1.0
//Example Description: This example shows how to connect a MC2000B chopper controller, 
//it also shows setting the necessary parameters and enabling the chopper.
// 
//If Visual Studio is used, and an error "cannot open source files stdafx.h" pops up while compiling
//please delete "#include "stdafx.h"" in the MC2000CommandLib.h, and make sure stdafx.h has been added in 
//Project\Properties\Configuration Properties\C/C++\Precompiled Headers\Precompiled Header File
#include "stdio.h"
#include "stdlib.h"
#include "iostream"
#include "MC2000CommandLib.h"

void Error(int,int);

//Set the blade Type
//0:MC1F2, 1:MC1F10, 2:MC1F15, 3:MC1F30, 4:MC1F60, 5:MC1F100, 6:MC1F10HP (The default blade), 7:MC1F2P10,
//8:MC1F6P10, 9:MC1F10A, 10:MC2F330, 11:MC2F47, 12:MC2F57B, 13:MC2F860, 14:MC2F5360
int bladeType = 6; //MC1F10HP (The default blade)

//Set the Reference-In Signal to INT-OUTER
//For MC1F10HP: 0 = INT-OUTER, 1 = INT-INNER, 2 = EXT-OUTER, 3 = EXT-INNER
//Please find the available reference-in signal for other blades from Chapter 8.3. of the manual
//The manual can be found here: https://www.thorlabs.com/thorproduct.cfm?partnumber=MC2000B
int refIn = 0;

//Set the Reference-Out Signal to Outer
//For MC1F10HP: 0 = Target, 1 = Outer, 2 = Inner
//Please find the available reference-in signal for other blades from Chapter 8.4. of the manual
//The manual can be found here: https://www.thorlabs.com/thorproduct.cfm?partnumber=MC2000B
int refOut = 1;

int main()
{	
	std::cout << "Enter name of port to connect (e.g. COM3): " ;
	char port[10];
	std::cin >> port;
	int device = Open(port, 115200, 3);
	if (device < 0)
	{
		std::cout << "Fail to open the COM port" << std::endl;
		return -1;
	}

	char id[100];
	int out = GetId(device, id);
	if (out == 0)
	{
		std::cout << "Device is a: " << id << std::endl;
	}
	else
	{
		Error(device,out);
		return -1;
	}

	//Set the internal reference frequency
	int frequency;
	std::cout << "Set the frequency (Hz): ";
	std::cin >> frequency;
	while (std::cin.fail())
	{
		// resets the error flags, allowing the cin stream to be used again for input
		std::cin.clear();
		// clear the input buffer
		std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
		std::cout << "Invalid Input! Please enter the number: ";
		std::cin >> frequency;
	}
	out = SetFrequency(device, frequency);
	if (out < 0)
	{
		Error(device,out);
		return -1;
	}

	//Set the blade type
	out = SetBladeType(device, bladeType);
	if (out < 0)
	{
		Error(device,out);
		return -1;
	}

	//Set the Reference-In Signal
	out = SetReference(device, refIn);
	if (out < 0)
	{
		Error(device,out);
		return -1;
	}

	//Set the Reference-Out Signal
	out = SetReferenceOutput(device, refOut);
	if (out < 0)
	{
		Error(device,out);
		return -1;
	}

	//Enable the device
	std::cout << "Enter \"Y\" to enable the Chopper. Enter any other character to close the program. " << std::endl;
	char character;
	std::cin >> character;
	if (character == 'Y' || character == 'y')
	{
		out = SetEnable(device, 1);//0 = disabled, 1 = enabled
		if (out < 0)
		{
			SetEnable(device, 0);//0 = disabled, 1 = enabled
			Error(device,out);
			return -1;
		}

		//Stop the chopper
		std::cout << "Enter \"N\" to stop the Chopper." << std::endl;
		while (true)
		{
			std::cin >> character;
			if (character == 'n' || character == 'N')
			{
				SetEnable(device, 0);//0 = disabled, 1 = enabled
				break;
			}
		}
	}

	//Close the device
	Close(device);	

	return 0;
}

void Error(int device,int out)
{
	std::cout << "Error: ";
	switch (out)
	{
		case -1:
			std::cout << "Invalid String Buffer. " << std::endl;
			break;
		case -2:
			std::cout << "Time out." << std::endl;
			break;
		case -3:
			std::cout << "Time out." << std::endl;
			break;
		case -4:
			std::cout << "CMD_NOT_DEFINED. " << std::endl;
			break;
	}
	Close(device);
}





