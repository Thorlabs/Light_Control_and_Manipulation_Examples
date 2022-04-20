#pragma once
#ifdef __cplusplus
#ifdef KLCCOMMANDLIB_EXPORTS
#define COMMANDLIB_API extern "C" __declspec( dllexport )
#else
#define COMMANDLIB_API extern "C" __declspec( dllimport )
#endif
#else
#define COMMANDLIB_API 
#endif

/// <summary>
/// list all the possible port on this computer.
/// </summary>
/// <param name="buf">port list returned string include serial number and device descriptor, separated by comma</param>
/// <param name="size">size of the buf</param>
/// <returns>non-negative number: number of device in the list; negative number: failed.</returns>
COMMANDLIB_API int List(char *buf, int size);

/// <summary>
///  open port function.
/// </summary>
/// <param name="serialNo">serial number of the device to be opened, use GetPorts function to get exist list first.</param>
/// <param name="nBaud">bit per second of port</param>
/// <param name="timeout">set timeout value in (s)</param>
/// <returns> non-negative number: hdl number returned Successfully; negative number: failed.</returns>
COMMANDLIB_API int Open(char* serialNo, int nBaud, int timeout);

/// <summary>
/// check opened status of port
/// </summary>
/// <param name="serialNo">serial number of the device to be checked.</param>
/// <returns> 0: port is not opened; 1: port is opened.</returns>
COMMANDLIB_API int IsOpen(unsigned char* serialNo);

/// <summary>
/// close current opened port
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int Close(int hdl);

/// <summary>
/// Set output enable state
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="enable">0x01 enable, 0x02 disable all voltage output.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetEnable(int hdl, unsigned char enable);

/// <summary>
/// Get output enable state
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="enable">0x01 enable, 0x02 disable.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetEnable(int hdl, unsigned char *enable);

/// <summary>
/// Get hardware information
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="sn">serial number.</param>
/// <param name="modeNumber">alphanumeric model number.</param>
/// <param name="type">45:multi‐channel controller motherboard;44:brushless DC controller</param>
/// <param name="firmwareVersion">firmaware version.</param>
/// <param name="hardwareVersion">hardware version.</param>
/// <param name="modeState">the modification of the hardware.</param>
/// <param name="nChs">number of channels.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetHardwareInfo(int hdl, long *sn, char* modeNumber, unsigned short type,
	unsigned char* firmwareVersion, unsigned short *hardwareVersion, unsigned short *modeState, unsigned short *nChs);

/// <summary>
/// Set output Voltage 1
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="voltage">the output voltage 0~25 V.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetVoltage1(int hdl, float voltage);

/// <summary>
/// Set output Voltage 2
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="voltage">the output voltage 0~25 V.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetVoltage2(int hdl, float voltage);

/// <summary>
/// Get output Voltage 1
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="voltage">the output voltage.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetVoltage1(int hdl, float * voltage);

/// <summary>
/// Get output Voltage 2
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="voltage">the output voltage.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetVoltage2(int hdl, float * voltage);

/// <summary>
/// Set frequency of output Voltage 1
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency 500~10000.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetFrequency1(int hdl, unsigned short freq);

/// <summary>
/// Set frequency of output Voltage 2
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency 500~10000.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetFrequency2(int hdl, unsigned short freq);

/// <summary>
/// Get frequency of output Voltage 1
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetFrequency1(int hdl, unsigned short * freq);

/// <summary>
/// Get frequency of output Voltage 2
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetFrequency2(int hdl, unsigned short * freq);

/// <summary>
/// Set frequency of switch mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency 0.1~150</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetSWFrequency(int hdl, float freq);

/// <summary>
/// Get frequency of switch mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="freq">the output frequency.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetSWFrequency(int hdl, float *freq);

/// <summary>
/// Set device analog input mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">input mode: 0 disable; 1 enable.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetInputMode(int hdl, unsigned char mode);

/// <summary>
/// Get device analog input mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">input mode: 0 disable; 1 enable.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetInputMode(int hdl, unsigned char* mode);

/// <summary>
/// Set device trigger pin mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">trigger mode: 01 -Trigger Pin1 output, Pin2 output; 02- Trigger Pin1 In, Pin2 out; 03- Trigger Pin1 out, Pin2 in.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetTrigIOConfigure(int hdl, unsigned char mode);

/// <summary>
/// Get device trigger pin mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">trigger mode: 01 -Trigger Pin1 output, Pin2 output; 02- Trigger Pin1 In, Pin2 out; 03- Trigger Pin1 out, Pin2 in.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetTrigIOConfigure(int hdl, unsigned char* mode);

/// <summary>
/// Get device ADC parameters
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="adcError">ADC error.</param>
/// <param name="adMaxValue">ADMAX_value</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetADCparams(int hdl, unsigned short *adcError, unsigned short *adMaxValue);

/// <summary>
/// Set device channel enable state
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="enableState">0x01 V1 Enable , 0x02  V2 Enable, 0x03  SW Enable, 0x00 channel output disable</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetChannelEnable(int hdl, unsigned char enableState);

/// <summary>
/// Get device channel enable state
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="enableState">0x01 V1 Enable , 0x02  V2 Enable, 0x03  SW Enable, 0x00 channel output disable</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetChannelEnable(int hdl, unsigned char *enableState);

/// <summary>
/// Set device configure the operating parameters 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="dispBrightness">display brightness 0~100</param>
/// <param name="dispTimeout">display timeout 1~480; set never should set to #FFFF.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetKcubeMMIParams(int hdl, unsigned short dispBrightness, unsigned short dispTimeout);

/// <summary>
/// Get device configure the operating parameters 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="dispBrightness">display brightness</param>
/// <param name="dispTimeout">display timeout</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetKcubeMMIParams(int hdl, unsigned short *dispBrightness, unsigned short *dispTimeout);

/// <summary>
/// Set the device lock/unlock the wheel control on the top pannel 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="lock">0x02 unlock the wheel;0x01 Lock the wheel</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetKcubeMMILock(int hdl, unsigned char lock);

/// <summary>
/// Get the device lock/unlock the wheel control on the top pannel 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="lock">0x02 unlock the wheel;0x01 Lock the wheel</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetKcubeMMILock(int hdl, unsigned char *lock);

/// <summary>
/// Set the device output status for software mode
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="voltage">output voltage</param>
/// <param name="frequency">output frequency</param>
/// <param name="frequencyFlag">0: no change; 1: frequency changed</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetOutPutStatus(int hdl, float voltage, unsigned short frequency, unsigned short frequencyFlag);

/// <summary>
/// Get the device output status 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="isOutputActive">0x00 Inactive; 0x01 Active</param>
/// <param name="outputVoltage">output voltage</param>
/// <param name="outputFrequency">output frequency</param>
/// <param name="errFlag">0: no erro; 1: error</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetOutPutStatus(int hdl, unsigned short *isOutputActive, float *outputVoltage, unsigned short *outputFrequency, unsigned short *errFlag);

/// <summary>
/// Save the operating parameters to eeproms 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetEEPRomParams(int hdl);

/// <summary>
/// Get the device status update value
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="channelenable">channel enable state.</param>
/// <param name="v1">output voltage 1.</param>
/// <param name="freq1">output frequency 1.</param>
/// <param name="v2">output voltage 2.</param>
/// <param name="freq2">output frequency 2.</param>
/// <param name="swFreq">switch frequency.</param>
/// <param name="dispBrightness">display pannel brightness.</param>
/// <param name="dipTimeout">display pannel timeout.</param>
/// <param name="adcMod">ADC mod in.</param>
/// <param name="trigConfig">trigger config.</param>
/// <param name="wheelMod">wheel locked mode.</param>
/// <param name="errFlag">0: no erro; 1: error</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetStatus(int hdl, unsigned char * channelenable, float *v1, unsigned short* freq1,
	float * v2, unsigned short * freq2, float * swFreq, unsigned short * dispBrightness, unsigned short * dipTimeout,
	unsigned char * adcMod, unsigned char * trigConfig, unsigned char * wheelMod, unsigned short * errFlag);

/// <summary>
/// Reset default factory seetings 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int RestoreFactorySettings(int hdl);

/// <summary>
/// Identify device 
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int Identify(int hdl);

/// <summary>
/// Update LUT value
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="index">index of the LUT array.</param>
/// <param name="vol">voltage 0~25.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int UpdateOutputLUT(int hdl, unsigned short index, float vol);

/// <summary>
/// Remove the last of LUT array
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="count">the count values will be removed from LUT array.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int RemoveLastOutputLUT(int hdl, unsigned short count);

/// <summary>
/// Set the LUT array data
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="vols">the LUT values array.</param>
/// <param name="size">the count of the array 0~512.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetOutputLUT(int hdl, float* vols, unsigned short size);

/// <summary>
/// Get the LUT array data
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="vols">the LUT values array.</param>
/// <param name="size">the count of the array 0~512.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetOutputLUT(int hdl, float* vols, unsigned short *size);

/// <summary>
/// Start LUT output
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int StartLUTOutput(int hdl);

/// <summary>
/// Stop LUT output
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int StopLUTOutput(int hdl);

/// <summary>
/// Set LUT parameters
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">run mode: 1 continuous; 2 cycle.</param>
/// <param name="numCycles">number of cycles 1~ 2147483648.</param>
/// <param name="delayTime">the sample intervals[ms] 1~ 2147483648.</param>
/// <param name="preCycleRest">the delay time before the cycle start[ms] 0~ 2147483648.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetOutputLUTParams(int hdl, unsigned short mode, unsigned long numCycles, unsigned long delayTime, unsigned long preCycleRest);

/// <summary>
/// Get LUT parameters
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="mode">run mode: 1 continuous; 2 cycle.</param>
/// <param name="numCycles">number of cycles 1~ 2147483648.</param>
/// <param name="delayTime">the sample intervals[ms] 1~ 2147483648.</param>
/// <param name="preCycleRest">the delay time before the cycle start[ms] 0~ 2147483648.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetOutputLUTParams(int hdl, unsigned short * mode, unsigned long * numCycles, unsigned long * delayTime, unsigned long * preCycleRest);

/// <summary>
/// Set device serial number
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="sn">Serial number.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int SetSerialNumber(int hdl, unsigned long sn);

/// <summary>
/// Get device serial number
/// </summary>
/// <param name="hdl">handle of port.</param>
/// <param name="sn">Serial number.</param>
/// <returns> 0: Success; negative number: failed.</returns>
COMMANDLIB_API int GetSerialNumber(int hdl, unsigned long* sn);
