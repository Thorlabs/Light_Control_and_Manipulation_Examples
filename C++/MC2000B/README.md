# Example Description
The example code shows how to control an MC2000B Optical Chopper in C++. The example contains the basic initialization, it also allows setting the frequency and enabling the chopper.
>This example sets the blade to **MC1F10HP**, please modify the blade type in the code if other blades are used. 


# Instructions for Use
Guides written for this example is written with Microsoft's Visual Studio in mind. Other IDEs can be used, but instructions are not provided in this repository.
1. Create a new VC++ project file or open the existed VC++ project file;
2. Under the Solution Explorer, right click the Source Files, then add the MC2000B.cpp to the Source Files;
3. Set the path of the header file:  
a. Open Project\Properties\Configuration Properties\C/C++\General  
b. Enter the path of the header files into Additional include Directories (C:\Program Files (x86)\Thorlabs\MC2000B\Sample\Thorlabs_MC2000B_C++SDK)
>If an error "cannot open source files stdafx.h" pops up while compiling, please delete **#include "stdafx.h"** in the MC2000CommandLib.h, and make sure **stdafx.h** has been added in Project\Properties\Configuration Properties\C/C++\Precompiled Headers\Precompiled Header File
4. Set the path of the static library:  
a. Open Project\Properties\Configuration Properties\Linker\General  
b. Enter the path of the library files into Additional Library Directories (C:\Program Files (x86)\Thorlabs\MC2000B\Sample\Thorlabs_MC2000B_C++SDK)
5. Set the additional depended library:  
a. Open Project\Properties\Configuration Properties\Linker\Input  
b. Add the additional depended libraries into Additional Dependencies (MC2000CommandLibWin32.lib; or MC2000CommandLibWin64.lib;).
6. Set the path of the dynamic library:
a. Open Project\Properties\Configuration Properties\Debugging
b. Enter the path of the dynamic library into Working Directory (C:\Program Files (x86)\Thorlabs\MC2000B\Sample\Thorlabs_MC2000B_C++SDK\)
