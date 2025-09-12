using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    /// Contains all the custom logic for generating the Basic RTU Controller project.
    /// </summary>
    public static class RtuGeneration
    {
        /// <summary>
        /// Generates the global settings Data Block for the RTU controller.
        /// This DB holds all key setpoints and configuration parameters.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void GenerateSystemSettingsDB(PlcDevice plcDevice)
        {
            // Create the Global DB object with the specified name from the SDS.
            var settingsDb = new XmlGlobalDB("DB_SystemSettings");

            // Get a reference to the static variable section of the DB interface.
            var itf = settingsDb.Interface[InterfaceSections.Static];

            // Add all the configurable parameters as defined in the project requirements.
            // Using descriptive names and appropriate data types.

            // Temperature Setpoints
            itf.Add(new InterfaceParameter("OccupiedCoolingSetpoint", "Real") { Comment = "Occupied mode cooling setpoint (°C)", DefaultValue = "24.0" });
            itf.Add(new InterfaceParameter("OccupiedHeatingSetpoint", "Real") { Comment = "Occupied mode heating setpoint (°C)", DefaultValue = "21.0" });
            itf.Add(new InterfaceParameter("UnoccupiedCoolingSetpoint", "Real") { Comment = "Unoccupied mode cooling setpoint (°C)", DefaultValue = "28.0" });
            itf.Add(new InterfaceParameter("UnoccupiedHeatingSetpoint", "Real") { Comment = "Unoccupied mode heating setpoint (°C)", DefaultValue = "18.0" });

            // Time Delays (in seconds)
            itf.Add(new InterfaceParameter("FanFailureDelay", "Time") { Comment = "Delay for fan failure alarm (s)", DefaultValue = "T#5s" });
            itf.Add(new InterfaceParameter("CompressorMinRunTime", "Time") { Comment = "Compressor minimum run time to prevent short cycling (s)", DefaultValue = "T#3m" });
            itf.Add(new InterfaceParameter("CompressorMinOffTime", "Time") { Comment = "Compressor minimum off time to prevent short cycling (s)", DefaultValue = "T#3m" });
            itf.Add(new InterfaceParameter("DirtyFilterDelay", "Time") { Comment = "Delay for dirty filter alarm (s)", DefaultValue = "T#10s" });

            // Damper Settings
            itf.Add(new InterfaceParameter("MinFreshAirPosition", "Real") { Comment = "Minimum fresh air damper position during occupied mode (%)", DefaultValue = "20.0" });
            itf.Add(new InterfaceParameter("EconomizerTempDifferential", "Real") { Comment = "Temp diff (OAT vs RAT) to enable economizer (°C)", DefaultValue = "2.0" });

            // Generate the actual block in the TIA project.
            settingsDb.GenerateXmlBlock(plcDevice);
        }

        /// <summary>
        /// Generates the FB for EM-100 Supply Fan Control.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void Generate_EM100_SupplyFan(PlcDevice plcDevice)
        {
            var fb = new XmlFB("FB_EM100_SupplyFan")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Equipment Module: Supply Fan Control (EM-100)" },
                BlockComments = { [TypeMapper.BaseCulture.Name] = "Controls and monitors the main supply fan VFD." }
            };

            // Define Interface
            var iface = fb.Interface;
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Enable", "Bool", "General enable for the module logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Start_Cmd", "Bool", "Command to start/stop the fan"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Speed_Ref", "Real", "Speed reference for the VFD (0-100%)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Run_Fdbk", "Bool", "Run feedback from the VFD (Digital Input)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Airflow_Sw", "Bool", "Airflow switch status (Digital Input)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("VFD_Fault", "Bool", "VFD fault status (Digital Input)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Failure_Delay_PT", "Time", "Preset time for fan failure detection"));

            iface[InterfaceSections.Output].Add(new InterfaceParameter("Start_Out", "Bool", "Command to the VFD Start/Stop (Digital Output)"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("Speed_Out", "Real", "Speed reference to the VFD (Analog Output)"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("Failure_Alarm", "Bool", "True if a fan failure condition is active"));

            // Internal static variables, including the timer instance
            iface[InterfaceSections.Static].Add(new InterfaceParameter("FailureTimer", "TON_TIME", "On-delay timer for failure detection"));

            // SCL Code
            var sclCode = @"
// Default all outputs to a safe state
#Start_Out := FALSE;
#Speed_Out := 0.0;
#Failure_Alarm := FALSE;

// Module logic only runs when enabled
IF #Enable THEN
    // Pass through start command and speed reference
    #Start_Out := #Start_Cmd;
    #Speed_Out := #Speed_Ref;

    // Failure Timer: The timer runs when a start is commanded but there is no run feedback.
    #FailureTimer(IN := #Start_Out AND NOT #Run_Fdbk,
                  PT := #Failure_Delay_PT);

    // Alarm Conditions:
    // 1. VFD Fault is active
    // 2. The failure timer has elapsed (fan commanded on, but no run feedback)
    // 3. The fan is proven to be running, but there is no airflow
    IF #VFD_Fault OR #FailureTimer.Q OR (#Run_Fdbk AND NOT #Airflow_Sw) THEN
        #Failure_Alarm := TRUE;
    END_IF;
END_IF;

// When the module is disabled, reset the timer by calling it with IN=FALSE to ensure a safe state.
IF NOT #Enable THEN
    #FailureTimer(IN := FALSE, PT := #Failure_Delay_PT);
END_IF;
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main fan logic" } };
            network.SclContent = sclCode;
            fb.Networks.Add(network);

            // Generate the actual block in the TIA project
            fb.GenerateSclBlock(plcDevice);
        }

        /// <summary>
        /// Generates the FB for EM-500 System Monitoring.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void Generate_EM500_SystemMonitoring(PlcDevice plcDevice)
        {
            var fb = new XmlFB("FB_EM500_SystemMonitoring")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Equipment Module: System Monitoring (EM-500)" },
                BlockComments = { [TypeMapper.BaseCulture.Name] = "Monitors common system-wide parameters like filter status." }
            };

            // Define Interface
            var iface = fb.Interface;
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Enable", "Bool", "General enable for the module logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("DirtyFilter_Sw", "Bool", "Dirty filter differential pressure switch input"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("DirtyFilter_Delay_PT", "Time", "Preset time for dirty filter alarm"));

            iface[InterfaceSections.Output].Add(new InterfaceParameter("DirtyFilter_Alarm", "Bool", "Dirty filter maintenance alarm"));

            // Internal static variables
            iface[InterfaceSections.Static].Add(new InterfaceParameter("DirtyFilterTimer", "TON_TIME", "On-delay timer for dirty filter alarm"));

            // SCL Code
            var sclCode = @"
// Default output to a safe state
#DirtyFilter_Alarm := FALSE;

// Only run logic if the module is enabled
IF #Enable THEN
    // Run the timer when the dirty filter switch is active
    #DirtyFilterTimer(IN := #DirtyFilter_Sw,
                      PT := #DirtyFilter_Delay_PT);

    // If the timer is done, latch the alarm
    IF #DirtyFilterTimer.Q THEN
        #DirtyFilter_Alarm := TRUE;
    END_IF;

ELSE
    // If disabled, reset the timer
    #DirtyFilterTimer(IN := FALSE, PT := #DirtyFilter_Delay_PT);
END_IF;
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main system monitoring logic" } };
            network.SclContent = sclCode;
            fb.Networks.Add(network);

            // Generate the actual block in the TIA project
            fb.GenerateSclBlock(plcDevice);
        }

        /// <summary>
        /// Generates the FB for EM-400 Damper Control.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void Generate_EM400_DamperControl(PlcDevice plcDevice)
        {
            var fb = new XmlFB("FB_EM400_DamperControl")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Equipment Module: Damper Control (EM-400)" },
                BlockComments = { [TypeMapper.BaseCulture.Name] = "Manages the economizer dampers and provides temperature data." }
            };

            // Define Interface
            var iface = fb.Interface;
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Enable", "Bool", "General enable for the module logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Damper_Pos_Cmd", "Real", "Damper position command from PID or min position logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Return_Air_Temp", "Real", "Return Air Temperature (Analog Input)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Outside_Air_Temp", "Real", "Outside Air Temperature (Analog Input)"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Discharge_Air_Temp", "Real", "Discharge Air Temperature (Analog Input)"));

            iface[InterfaceSections.Output].Add(new InterfaceParameter("Damper_Pos_Out", "Real", "Position to the damper actuator (Analog Output)"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("RAT_Out", "Real", "Return Air Temperature for use in main program"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("OAT_Out", "Real", "Outside Air Temperature for use in main program"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("DAT_Out", "Real", "Discharge Air Temperature for use as PID Process Variable"));

            // SCL Code
            var sclCode = @"
// Pass through the temperature values
#RAT_Out := #Return_Air_Temp;
#OAT_Out := #Outside_Air_Temp;
#DAT_Out := #Discharge_Air_Temp;

// Only command the damper if the module is enabled
IF #Enable THEN
    #Damper_Pos_Out := #Damper_Pos_Cmd;
ELSE
    // Drive damper closed if disabled
    #Damper_Pos_Out := 0.0;
END_IF;
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main damper logic" } };
            network.SclContent = sclCode;
            fb.Networks.Add(network);

            // Generate the actual block in the TIA project
            fb.GenerateSclBlock(plcDevice);
        }

        /// <summary>
        /// Generates the FB for EM-300 Heating Control.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void Generate_EM300_HeatingControl(PlcDevice plcDevice)
        {
            var fb = new XmlFB("FB_EM300_HeatingControl")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Equipment Module: Heating Control (EM-300)" },
                BlockComments = { [TypeMapper.BaseCulture.Name] = "Controls a single-stage heating element and its safeties." }
            };

            // Define Interface
            var iface = fb.Interface;
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Enable", "Bool", "General enable for the module logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Heat_Cmd", "Bool", "Command to enable heating"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("HighTemp_Sw", "Bool", "High temperature limit switch input"));

            iface[InterfaceSections.Output].Add(new InterfaceParameter("Heat_Out", "Bool", "Command to the heating contactor"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("HighTemp_Alarm", "Bool", "High temperature fault active"));

            // SCL Code
            var sclCode = @"
// Default outputs to a safe state
#Heat_Out := FALSE;
#HighTemp_Alarm := FALSE;

// Latch the safety alarm. Requires a manual reset.
IF #HighTemp_Sw THEN
    #HighTemp_Alarm := TRUE;
END_IF;

// Heating can only be enabled if the module is enabled and no alarms are active.
IF #Enable AND #Heat_Cmd AND NOT #HighTemp_Alarm THEN
    #Heat_Out := TRUE;
ELSE
    #Heat_Out := FALSE;
END_IF;
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main heating logic" } };
            network.SclContent = sclCode;
            fb.Networks.Add(network);

            // Generate the actual block in the TIA project
            fb.GenerateSclBlock(plcDevice);
        }

        /// <summary>
        /// Generates the FB for EM-200 Cooling Control.
        /// </summary>
        /// <param name="plcDevice">The target PLC device.</param>
        public static void Generate_EM200_CoolingControl(PlcDevice plcDevice)
        {
            var fb = new XmlFB("FB_EM200_CoolingControl")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Equipment Module: Cooling Control (EM-200)" },
                BlockComments = { [TypeMapper.BaseCulture.Name] = "Controls a single-stage DX cooling compressor and its safeties." }
            };

            // Define Interface
            var iface = fb.Interface;
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Enable", "Bool", "General enable for the module logic"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Cool_Cmd", "Bool", "Command to enable cooling"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("HighPress_Sw", "Bool", "High pressure switch input"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("LowPress_Sw", "Bool", "Low pressure switch input"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("Freeze_Stat", "Bool", "Freeze stat input"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("MinRunTime_PT", "Time", "Preset time for min run time"));
            iface[InterfaceSections.Input].Add(new InterfaceParameter("MinOffTime_PT", "Time", "Preset time for min off time"));

            iface[InterfaceSections.Output].Add(new InterfaceParameter("Comp_Out", "Bool", "Command to the compressor contactor"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("HighPress_Alarm", "Bool", "High pressure fault active"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("LowPress_Alarm", "Bool", "Low pressure fault active"));
            iface[InterfaceSections.Output].Add(new InterfaceParameter("Freeze_Alarm", "Bool", "Freeze stat alarm active"));

            // Internal static variables
            iface[InterfaceSections.Static].Add(new InterfaceParameter("MinRunTimer", "TON_TIME", "Timer for minimum run time"));
            iface[InterfaceSections.Static].Add(new InterfaceParameter("MinOffTimer", "TON_TIME", "Timer for minimum off time"));
            iface[InterfaceSections.Static].Add(new InterfaceParameter("Internal_Cool_Cmd", "Bool", "Latched cooling command for timing logic"));

            // SCL Code
            var sclCode = @"
// Default outputs to a safe state
#Comp_Out := FALSE;
#HighPress_Alarm := FALSE;
#LowPress_Alarm := FALSE;
#Freeze_Alarm := FALSE;

// Safety checks. These are latched alarms.
// A real implementation would require a manual reset input.
IF #HighPress_Sw THEN #HighPress_Alarm := TRUE; END_IF;
IF #LowPress_Sw THEN #LowPress_Alarm := TRUE; END_IF;
IF #Freeze_Stat THEN #Freeze_Alarm := TRUE; END_IF;

// The module must be enabled and no safety alarms can be active
IF #Enable AND NOT (#HighPress_Alarm OR #LowPress_Alarm OR #Freeze_Alarm) THEN

    // Minimum Off-Time Logic
    #MinOffTimer(IN := NOT #Internal_Cool_Cmd, PT := #MinOffTime_PT);

    // Latch the cool command if the off-time is satisfied
    IF #Cool_Cmd AND #MinOffTimer.Q THEN
        #Internal_Cool_Cmd := TRUE;
    END_IF;

    // Minimum Run-Time Logic
    #MinRunTimer(IN := #Internal_Cool_Cmd, PT := #MinRunTime_PT);

    // Unlatch the cool command if the run-time is satisfied
    IF NOT #Cool_Cmd AND #MinRunTimer.Q THEN
        #Internal_Cool_Cmd := FALSE;
    END_IF;

    // The final compressor command is the internal latched command
    #Comp_Out := #Internal_Cool_Cmd;

ELSE
    // If disabled or an alarm is active, force everything off and reset timers
    #Comp_Out := FALSE;
    #Internal_Cool_Cmd := FALSE;
    #MinRunTimer(IN := FALSE, PT := #MinRunTime_PT);
    #MinOffTimer(IN := FALSE, PT := #MinOffTime_PT);
END_IF;
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main cooling logic" } };
            network.SclContent = sclCode;
            fb.Networks.Add(network);

            // Generate the actual block in the TIA project
            fb.GenerateSclBlock(plcDevice);
        }

        /// <summary>
        /// Generates the Main OB1 block that contains the core RTU control logic.
        /// </summary>
        public static void Generate_OB1_Main(PlcDevice plcDevice, MAC_use_casesEM module)
        {
            // Create instance DBs for all our Equipment Modules
            // Note: In a real scenario, these FBs would be part of a library accessible via module.ResourceManagement
            var iEM100 = IntegrateLibraries.CreateInstanceDataBlock(module, "FB_EM100_SupplyFan", "iDB_EM100_SupplyFan");
            var iEM200 = IntegrateLibraries.CreateInstanceDataBlock(module, "FB_EM200_CoolingControl", "iDB_EM200_CoolingControl");
            var iEM300 = IntegrateLibraries.CreateInstanceDataBlock(module, "FB_EM300_HeatingControl", "iDB_EM300_HeatingControl");
            var iEM400 = IntegrateLibraries.CreateInstanceDataBlock(module, "FB_EM400_DamperControl", "iDB_EM400_DamperControl");
            var iEM500 = IntegrateLibraries.CreateInstanceDataBlock(module, "FB_EM500_SystemMonitoring", "iDB_EM500_SystemMonitoring");

            var mainOb = new XmlOB("Main")
            {
                BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.SCL },
                BlockTitles = { [TypeMapper.BaseCulture.Name] = "Main RTU Control Logic" }
            };
            ((OB_BlockAttributes)mainOb.BlockAttributes).BlockSecondaryType = "ProgramCycle";

            // Define temporary variables for OB1 logic
            var iface = mainOb.Interface[InterfaceSections.Temp];
            iface.Add(new InterfaceParameter("SystemEnable", "Bool", "Master enable for the RTU"));
            iface.Add(new InterfaceParameter("Occupied", "Bool", "Unit is in Occupied mode"));
            iface.Add(new InterfaceParameter("DAT_Setpoint", "Real", "Active Discharge Air Temp Setpoint"));
            iface.Add(new InterfaceParameter("CallForCool", "Bool", "System requires cooling"));
            iface.Add(new InterfaceParameter("CallForHeat", "Bool", "System requires heating"));
            iface.Add(new InterfaceParameter("EconomizerEnable", "Bool", "Free cooling is available"));

            var sclCode = @"
// --- Mode and Setpoint Logic ---
#SystemEnable := TRUE; // For simulation; would come from BAS/Schedule
#Occupied := TRUE;     // For simulation; would come from BAS/Schedule

IF #Occupied THEN
    #DAT_Setpoint := ""DB_SystemSettings"".OccupiedCoolingSetpoint; // Simplified: just use cooling setpoint
ELSE
    #DAT_Setpoint := ""DB_SystemSettings"".UnoccupiedCoolingSetpoint; // Simplified: just use cooling setpoint
END_IF;

// --- Demand Logic ---
// Simplified demand logic based on a 1-degree deadband
IF ""iDB_EM400_DamperControl"".DAT_Out > #DAT_Setpoint + 0.5 THEN
    #CallForCool := TRUE;
    #CallForHeat := FALSE;
ELSIF ""iDB_EM400_DamperControl"".DAT_Out < #DAT_Setpoint - 0.5 THEN
    #CallForCool := FALSE;
    #CallForHeat := TRUE;
ELSE
    #CallForCool := FALSE;
    #CallForHeat := FALSE;
END_IF;

// --- Economizer Logic ---
#EconomizerEnable := ""iDB_EM400_DamperControl"".OAT_Out < (""iDB_EM400_DamperControl"".RAT_Out - ""DB_SystemSettings"".EconomizerTempDifferential);

// --- Equipment Module Calls ---

// EM-400 Damper Control (must be called first to get latest temps)
""iDB_EM400_DamperControl""(Enable := #SystemEnable,
                        Damper_Pos_Cmd := ""DB_SystemSettings"".MinFreshAirPosition, // Simplified: no PID output for now
                        Return_Air_Temp := 22.5, // Simulated hardware input
                        Outside_Air_Temp := 15.0, // Simulated hardware input
                        Discharge_Air_Temp := 23.0); // Simulated hardware input

// EM-100 Supply Fan
""iDB_EM100_SupplyFan""(Enable := #SystemEnable,
                   Start_Cmd := #Occupied OR #CallForCool OR #CallForHeat,
                   Speed_Ref := 75.0, // Fixed speed for now
                   Run_Fdbk := TRUE, // Simulated hardware input
                   Airflow_Sw := TRUE, // Simulated hardware input
                   VFD_Fault := FALSE, // Simulated hardware input
                   Failure_Delay_PT := ""DB_SystemSettings"".FanFailureDelay);

// EM-200 Cooling Control
""iDB_EM200_CoolingControl""(Enable := #SystemEnable AND ""iDB_EM100_SupplyFan"".Start_Out, // Safety interlock with fan
                       Cool_Cmd := #CallForCool AND NOT #EconomizerEnable, // Use mechanical cooling if economizer isn't enough
                       HighPress_Sw := FALSE, // Simulated
                       LowPress_Sw := FALSE, // Simulated
                       Freeze_Stat := FALSE, // Simulated
                       MinRunTime_PT := ""DB_SystemSettings"".CompressorMinRunTime,
                       MinOffTime_PT := ""DB_SystemSettings"".CompressorMinOffTime);

// EM-300 Heating Control
""iDB_EM300_HeatingControl""(Enable := #SystemEnable AND ""iDB_EM100_SupplyFan"".Start_Out, // Safety interlock with fan
                       Heat_Cmd := #CallForHeat,
                       HighTemp_Sw := FALSE); // Simulated

// EM-500 System Monitoring
""iDB_EM500_SystemMonitoring""(Enable := #SystemEnable,
                         DirtyFilter_Sw := FALSE, // Simulated
                         DirtyFilter_Delay_PT := ""DB_SystemSettings"".DirtyFilterDelay);
";

            var network = new BlockNetwork() { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "Main RTU execution logic" } };
            network.SclContent = sclCode;
            mainOb.Networks.Add(network);

            // Generate the OB, merging with an existing Main OB if it exists.
            GenericBlockCreation.AddCodeBlockToOB(mainOb, module.ResourceManagement, "en-US", plcDevice);
        }
    }
}
