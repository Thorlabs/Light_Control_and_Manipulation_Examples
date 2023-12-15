%% Header
% Title: Deformable_Mirrors_NET
% Created Date: 2023-11-22
% Last modified date: 2023-11-23
% Matlab Version:R2022a
% Thorlabs DLL version:3.1.1837.58
%% Notes:The example connects to a deformable mirror, performs a relaxation and 
% sets voltages for a certain value of a Zernike coefficient. 
% The example uses methods from the .NET SDK 
% Tested for DMP40
%
clear all;

lib=NET.addAssembly('C:\Program Files\IVI Foundation\VISA\VisaCom64\Primary Interop Assemblies\Thorlabs.TLDFM_64.Interop.dll');
libx=NET.addAssembly('C:\Program Files\IVI Foundation\VISA\VisaCom64\Primary Interop Assemblies\Thorlabs.TLDFMX_64.Interop.dll');

import Thorlabs.TLDFM_64.Interop.*;
import Thorlabs.TLDFMX_64.Interop.*;

%Uncomment the next two lines to see an overview of the available functions
%methodsview('Thorlabs.TLDFM_64.Interop.TLDFM')
%methodsview('Thorlabs.TLDFMX_64.Interop.TLDFMX')

handle = System.IntPtr(0);
device = TLDFM(handle);

% Search for available devices
[~,devicecount]=device.get_device_count();
disp([num2str(devicecount),' device(s) found']);

if devicecount>0

    %get information about first available device
    manufacturer=System.Text.StringBuilder(256);
    instrumentName=System.Text.StringBuilder(256);
    serialNumber=System.Text.StringBuilder(256);
    resourceName=System.Text.StringBuilder(256);
    
    device.get_device_information(0, manufacturer, instrumentName, serialNumber,  resourceName);
    
    disp(manufacturer.ToString);
    disp(instrumentName.ToString);
    disp(serialNumber.ToString);
    disp(resourceName.ToString);
    
    
    %initialize device
    instrument=TLDFMX(resourceName.ToString(),true,true);
    
    %get number of segments and arms
    [~,mirrorcnt]=instrument.Device.get_segment_count;
    disp(['Number of mirrors: ',num2str(mirrorcnt)]);
    [~,armscnt]=instrument.Device.get_tilt_count;
    disp(['Number of arms: ',num2str(armscnt)]);
    
    
    
    %Relax mirror
    disp('Relax mirror');
    RelaxMirrorPattern=NET.createArray('System.Double',mirrorcnt);
    RelaxArmsPattern=NET.createArray('System.Double',armscnt);
    devpart=Thorlabs.TLDFM_64.Interop.DevicePart.Both;
    
    [err,remainingsteps]=Relax(instrument,devpart,true,false,RelaxMirrorPattern,RelaxArmsPattern);%calculate relaxpattern
    instrument.Device.set_voltages(RelaxMirrorPattern,RelaxArmsPattern);
    pause(0.01);
    while remainingsteps>0
        [err,remainingsteps]=Relax(instrument,devpart,false,false,RelaxMirrorPattern,RelaxArmsPattern);%calculate next relaxpattern
        instrument.Device.set_voltages(RelaxMirrorPattern,RelaxArmsPattern);
        pause(0.01);    
    end
    
    
    %Set pattern for Zernike Z4 with amplitude 0.7
    disp('Set Zernike Z4 to amplitude 0.7');
    MirrorPattern=NET.createArray('System.Double',mirrorcnt);
    zernike=Thorlabs.TLDFMX_64.Interop.ZernikeFlags.Astigmatism45Degree;%corresponds
    zernikeAmplitude=0.7;
    CalculateSingleZernikePattern(instrument,zernike,zernikeAmplitude,MirrorPattern);
    instrument.Device.set_segment_voltages(MirrorPattern);
    
    %disconnect
    instrument.Dispose();
end


