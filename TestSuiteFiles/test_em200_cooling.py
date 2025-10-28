import unittest
import time

# Mock PLC class to simulate the TIA Portal environment
class MockPLC:
    def __init__(self):
        self.tags = {}

    def set_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def get_tag(self, tag_name):
        return self.tags.get(tag_name, 0.0) # Default to 0.0 for safety

# Global constants
PLC_INSTANCE_NAME = "AHU_Controller_Sim"

class TestChilledWaterValveModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test, representing a hydronic system."""
        self.plc = MockPLC()
        # All values are percentages (0-100) unless otherwise noted
        self.plc.set_tag("FB200_EM_Cooling.Enable", False)
        self.plc.set_tag("FB200_EM_Cooling.Cooling_Demand_In", 0.0) # Demand from main PID
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI", False) # Digital input
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI", 0.0) # Feedback
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Cmd_AO", 0.0) # Command
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm", False)
        self.plc.set_tag("FB200_EM_Cooling.UDT.Valve_Position_Failure_Alm", False)
        self.plc.set_tag("FB200_EM_Cooling.UDT.Fault_Delay_Sec", 5.0) # Seconds
        self.plc.set_tag("FB200_EM_Cooling.UDT.Fault_Tolerance_Perc", 5.0) # Percent
        self.mismatch_start_time = None

    def run_chilled_water_valve_logic(self, current_time):
        """
        Simulates one scan of the logic for a modulating chilled water valve.
        This now includes a simple simulation of the valve's physical movement.
        """
        enable = self.plc.get_tag("FB200_EM_Cooling.Enable")
        demand = self.plc.get_tag("FB200_EM_Cooling.Cooling_Demand_In")
        freeze_stat = self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI")
        valve_cmd = self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Cmd_AO")

        # --- PLC Logic Simulation ---
        if enable and not freeze_stat:
            valve_cmd = demand
        else:
            valve_cmd = 0.0 # Safety override
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Cmd_AO", valve_cmd)
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm", freeze_stat)

        # --- Physical Valve Simulation ---
        # A real valve takes time to move. We simulate this by moving it 10% per second.
        current_pos = self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI")
        if abs(valve_cmd - current_pos) > 1: # Only move if not already close
            if valve_cmd > current_pos:
                self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI", current_pos + 10) # Open 10%
            else:
                self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI", current_pos - 10) # Close 10%

        # --- Failure Alarm Logic ---
        valve_fdbk = self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI")
        tolerance = self.plc.get_tag("FB200_EM_Cooling.UDT.Fault_Tolerance_Perc")
        delay = self.plc.get_tag("FB200_EM_Cooling.UDT.Fault_Delay_Sec")
        valve_failure_alm = self.plc.get_tag("FB200_EM_Cooling.UDT.Valve_Position_Failure_Alm")
        is_mismatch = abs(valve_cmd - valve_fdbk) > tolerance

        if is_mismatch and not valve_failure_alm:
            if self.mismatch_start_time is None:
                self.mismatch_start_time = current_time
            elif (current_time - self.mismatch_start_time) >= delay:
                valve_failure_alm = True
        elif not is_mismatch:
            self.mismatch_start_time = None
        self.plc.set_tag("FB200_EM_Cooling.UDT.Valve_Position_Failure_Alm", valve_failure_alm)

    def test_valve_modulation(self):
        """TC1: Test that the valve modulates to the correct position."""
        print(f"Running Chilled Water Valve modulation test on PLC instance: {PLC_INSTANCE_NAME}")
        self.plc.set_tag("FB200_EM_Cooling.Enable", True)
        self.plc.set_tag("FB200_EM_Cooling.Cooling_Demand_In", 50.0)

        # Simulate 5 seconds of operation (5 scans)
        for i in range(5):
            self.run_chilled_water_valve_logic(current_time=i)
            time.sleep(0.1) # Simulate scan time

        # After 5 seconds, the valve should have reached its 50% position
        self.assertAlmostEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI"), 50.0, delta=1)

    def test_freeze_safety_trip(self):
        """TC2: Test that a freeze stat fault safely closes the chilled water valve."""
        self.plc.set_tag("FB200_EM_Cooling.Enable", True)
        self.plc.set_tag("FB200_EM_Cooling.Cooling_Demand_In", 80.0)
        self.run_chilled_water_valve_logic(current_time=0.0)
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Cmd_AO"), 80.0)

        # Simulate freeze fault
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI", True)
        self.run_chilled_water_valve_logic(current_time=1.0)

        self.assertTrue(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm"))
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Cmd_AO"), 0.0)

    def test_valve_failure_alarm(self):
        """TC3: Test that a stuck valve triggers the position failure alarm."""
        self.plc.set_tag("FB200_EM_Cooling.Enable", True)
        self.plc.set_tag("FB200_EM_Cooling.Cooling_Demand_In", 50.0)

        # Physically lock the valve's feedback at 20% to simulate a stuck valve
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI", 20.0)

        # Simulate time passing
        current_time = 0.0
        while current_time < 4.9:
            self.run_chilled_water_valve_logic(current_time=current_time)
            # In our simulation, we override the physical valve movement, so we manually keep it at 20%
            self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Position_Fdbk_AI", 20.0)
            self.assertFalse(self.plc.get_tag("FB200_EM_Cooling.UDT.Valve_Position_Failure_Alm"))
            current_time += 0.1

        # At 5 seconds, the alarm should trigger
        self.run_chilled_water_valve_logic(current_time=5.0)
        self.assertTrue(self.plc.get_tag("FB200_EM_Cooling.UDT.Valve_Position_Failure_Alm"))

if __name__ == '__main__':
    unittest.main()
