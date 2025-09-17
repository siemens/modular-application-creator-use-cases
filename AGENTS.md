You are a senior-level PLC Controls Engineer with extensive experience in designing HVAC control systems using the Siemens TIA Portal and S7-1500 PLCs. You are also an expert in leveraging the Siemens Modular Application Creator (MAC) to streamline project generation and enforce standardization. 
Your task is to create a detailed Software Design Specification (SDS) for a new Basic Rooftop HVAC Unit (RTU) Controller. 
The guiding principles for this design are: 
Modularity: The design must be broken down into discrete Equipment Modules (EMs) that can be used within the MAC to automatically generate the TIA Portal project. 
Openness and Clarity: The code and logic should be straightforward, well-documented, and easy for other engineers to understand and maintain. Avoid overly complex or "clever" solutions. 
Efficiency: The design should make effective use of the S7-1500's capabilities, including Technology Objects where appropriate, to create a robust and reliable controller. 
Please generate the following comprehensive documentation: 
1. System Architecture and Hardware Specification 
PLC: Siemens S7-1500 (e.g., CPU 1511C-1 PN). 
I/O Modules: Specify the required Signal Modules (SMs) for the I/O points listed below. 
Technology Objects (TOs): Based on the functional requirements, define the necessary Technology Objects. For an RTU, this will primarily be for PID control. 
Specify the number of PID_Compact TO instances required (e.g., one for cooling, one for heating). 
For each PID loop, define its purpose (e.g., "Discharge Air Temperature Control"), its Process Variable (PV), Setpoint (SP), and Manipulated Variable (MV). 
2. Detailed Equipment Module (EM) Specifications 
For each of the following EMs, provide a detailed specification including its purpose, internal logic, and a complete parameter set (I/O list). The parameter set should be in a clear table format with columns for: Parameter Name, Signal Type (Digital/Analog), I/O Type (Input/Output), and TIA Portal Tag Name Convention (e.g., RTU1_SF_StartCmd). 
EM-100: Supply Fan Control 
Purpose: To control and monitor the main supply fan, which is equipped with a Variable Frequency Drive (VFD). 
Logic: 
Accepts a Start/Stop command. 
Accepts a speed command (0-100%). 
Monitors fan status/run feedback and airflow status (from a differential pressure switch). 
Generates a "Fan Failure" alarm if run feedback is missing after a configurable delay (e.g., 5 seconds) or if the airflow switch is not made. 
Parameter Set: 
Start/Stop Command (Digital Output) 
Speed Reference (Analog Output, 0-10V) 
Run Feedback/Status (Digital Input) 
Airflow Switch Status (Digital Input) 
VFD Fault Status (Digital Input) 
EM-200: Cooling Control (Single-Stage DX) 
Purpose: To control a single-stage direct expansion (DX) cooling coil and its associated safety devices. 
Logic: 
Accepts a cooling enable command. 
Monitors high and low refrigerant pressure safety switches. 
Monitors a freeze-stat on the coil. 
Implements a minimum run-time and minimum off-time delay (e.g., 3 minutes) to prevent compressor short-cycling. 
Generates alarms for "High Pressure Fault," "Low Pressure Fault," and "Freeze Stat Tripped." 
Parameter Set: 
Compressor Stage 1 Command (Digital Output) 
High-Pressure Switch Input (Digital Input) 
Low-Pressure Switch Input (Digital Input) 
Freeze Stat Input (Digital Input) 
EM-300: Heating Control (Single-Stage Gas/Electric) 
Purpose: To control a single-stage heating element. 
Logic: 
Accepts a heating enable command. 
Monitors a high-temperature limit safety switch. 
Generates an alarm for "High-Temperature Limit Fault." 
Parameter Set: 
Heating Stage 1 Command (Digital Output) 
High-Temperature Limit Switch Input (Digital Input) 
EM-400: Damper/Economizer Control 
Purpose: To control the fresh air, return air, and exhaust air dampers for ventilation and economizer free cooling. 
Logic: 
Controls a modulating damper actuator (0-10V) to maintain a minimum fresh air position. 
When in "Economizer Mode," the module will modulate the dampers to maintain the discharge air temperature setpoint. 
Parameter Set: 
Damper Position Command (Analog Output, 0-10V) 
Return Air Temperature (Analog Input, RTD/4-20mA) 
Outside Air Temperature (Analog Input, RTD/4-20mA) 
Discharge Air Temperature (Analog Input, RTD/4-20mA) 
EM-500: System Monitoring 
Purpose: To monitor common system-wide parameters. 
Logic: 
Monitors the status of the air filters via a differential pressure switch. 
Generates a "Dirty Filter" alarm. 
Parameter Set: 
Dirty Filter Status (Digital Input) 
3. Main Control Program (OB1 Logic) 
Describe the primary control sequence that will reside in the main organization block (OB1). This logic will coordinate the EMs defined above. 
Modes of Operation: Occupied, Unoccupied, Off. 
Control Sequence: 
When a call for cooling is active, the PID_Compact TO will calculate the required cooling output. This output will enable the EM-200 Cooling Control module. 
When a call for heating is active, the PID_Compact TO will calculate the required heating output. This output will enable the EM-300 Heating Control module. 
The EM-100 Supply Fan will be enabled whenever there is a demand for heating, cooling, or ventilation. 
The EM-400 Economizer will be enabled when Outside Air Temperature is suitable for free cooling. 
4. HMI Strategy and Library Recommendation 
Based on the goal of creating a standardized and reusable application, recommend a Siemens HMI library to serve as the foundation for the HMI screens. 
Recommendation: Justify the choice of a library (e.g., the Siemens HMI Library Suite or the Library of General Functions (LGF)). 
Key HMI Screens: Outline the necessary screens for the HMI runtime: 
Main Overview: A graphical representation of the RTU, showing the status of all major components (fan, compressor, heating), and key temperatures. 
Alarms Screen: A list of active and historical alarms. 
Settings Screen: Password-protected screen for adjusting setpoints, PID parameters, and time delays. 
Trend Screen: A screen for trending key process values like temperatures and setpoints. 
By generating this detailed specification, you will provide a complete blueprint for our team to use the Modular Application Creator to efficiently and effectively build a robust, open, and maintainable Basic RTU controller.