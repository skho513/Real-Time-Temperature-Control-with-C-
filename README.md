# Real-Time-Temperature-Control-with-C-

The aim of this project is to develop programme using C# for real time digital control of temperature in a temperature chamber at a nearly constant level above room temperature. The control tasks involved in this project include: real time data sensing of temperature, signal conditioning using digital filters, and control of heating and cooling elements. A user interacts with the programme via a graphical interface. A temperature chamber is provided.

Interfacing with the Elvis board:
The interfacing with the Elvis board can be done using the “NationalInstruments.DAQmx“ assembly for .NET.

Software Specifications

Tasks:
• Continuously read the values from the temperature sensors. The sampling rate of the signal reading must be 10 Hz or higher.
• Denoise (i.e. reduce noise contained in a signal) the signals read from the sensors by applying digital filters then convert them to temperatures.
• Display the current temperatures measured by the three sensors on screen in Celsius and store the history in separate files for further analysis.
• Allow the user to set the desired steady state temperature value range of 2° to 5°C above room temperature.
• Activate the heating element and/or fan to eventually maintain the temperature within ± 0.25°C of the desired temperature. Initial rise time and overshoot should be minimised.
• See more complete specifications below for the user interface.

User Interface:
The user interface should be constructed using a C# Forms application. It should contain components that satisfy the following specifications:

• Display the status of the heater and fan at any time. Both the fan and heater should be off when the programme starts.
• Display the current temperature values from the sensors. It should be updated at 0.5 second intervals. Perturbation of the displayed temperature by noise should be minimised. Every reading should also be written to a file, for subsequent plotting (using Excel or Matlab for example.)
• Display and allow the user to set the desired temperature value.
• Allow the user to set the parameters of the digital filters by reading a parameter file.
• Allow the user to choose which locations of the sensors in the chamber the system controls.
• Allow the user to initiate and terminate the control process.
• Allow the user to interactively adjust the parameters of the control process and to reset any options with neither interrupting the control process while the user makes the changes nor causing stress for the user of waiting for the system to respond to.
• When programme terminates, the heater must be turned off and the fan must run enough time to return the temperature value to around room temperature.
