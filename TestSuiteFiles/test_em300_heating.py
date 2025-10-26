import unittest

# Mock PLC class to simulate the TIA Portal environment
class MockPLC:
    def __init__(self):
        self.tags = {}

    def set_tag(self, tag_name, value):
        self.tags[tag_name] = value

    def get_tag(self, tag_name):
        return self.tags.get(tag_name, None)

# Global constants
PLC_INSTANCE_NAME = "AHU_Controller_Sim"

class TestHeatingValveModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values for the UDT and inputs
        self.plc.set_tag("FB300_EM_Heating.Enable", False)
        self.plc.set_tag("FB300_EM_Heating.Valve_Demand_In", 0.0)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Freeze_Stat_DI", False)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Valve_Fdbk_AI", 0.0)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO", 0.0)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Freeze_Alm", False)
        self.plc.set_tag("FB300_EM_Heating.UDT.Valve_Failure_Alm", False)
        self.plc.set_tag("FB300_EM_Heating.UDT.Fault_Delay_Sec", 5.0)
        self.plc.set_tag("FB300_EM_Heating.UDT.Fault_Tolerance_Perc", 5.0)
        self.mismatch_start_time = None

    def run_heating_logic(self, current_time):
        """Simulates one scan of the FB300_EM_Heating logic for the heat exchanger heating valve."""
        enable = self.plc.get_tag("FB300_EM_Heating.Enable")
        demand = self.plc.get_tag("FB300_EM_Heating.Valve_Demand_In")
        freeze_stat = self.plc.get_tag("FB300_EM_Heating.UDT.HW_Freeze_Stat_DI")
        valve_cmd = 0.0
        freeze_alm = False

        # Passthrough and safety logic
        if enable and not freeze_stat:
            valve_cmd = demand
        if freeze_stat:
            freeze_alm = True
            valve_cmd = 0.0 # Safety override
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO", valve_cmd)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Freeze_Alm", freeze_alm)

        # New Failure Alarm Logic
        valve_fdbk = self.plc.get_tag("FB300_EM_Heating.UDT.HW_Valve_Fdbk_AI")
        tolerance = self.plc.get_tag("FB300_EM_Heating.UDT.Fault_Tolerance_Perc")
        delay = self.plc.get_tag("FB300_EM_Heating.UDT.Fault_Delay_Sec")
        valve_failure_alm = self.plc.get_tag("FB300_EM_Heating.UDT.Valve_Failure_Alm")
        is_mismatch = abs(valve_cmd - valve_fdbk) > tolerance

        if is_mismatch and not valve_failure_alm:
            if self.mismatch_start_time is None:
                self.mismatch_start_time = current_time
            elif (current_time - self.mismatch_start_time) >= delay:
                valve_failure_alm = True
        elif not is_mismatch:
            self.mismatch_start_time = None # Reset timer
        self.plc.set_tag("FB300_EM_Heating.UDT.Valve_Failure_Alm", valve_failure_alm)

    def test_valve_passthrough(self):
        """TC1: Test that the heating valve command follows the demand input."""
        print(f"Running Heat Exchanger Heating Valve test on PLC instance: {PLC_INSTANCE_NAME}")
        self.plc.set_tag("FB300_EM_Heating.Enable", True)

        # Test 50% demand
        self.plc.set_tag("FB300_EM_Heating.Valve_Demand_In", 50.0)
        self.run_heating_logic(current_time=0.0)
        self.assertEqual(self.plc.get_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO"), 50.0)

        # Test 100% demand
        self.plc.set_tag("FB300_EM_Heating.Valve_Demand_In", 100.0)
        self.run_heating_logic(current_time=0.0)
        self.assertEqual(self.plc.get_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO"), 100.0)

    def test_freeze_safety_trip(self):
        """TC2: Test that the freeze stat safety overrides the demand for the heating valve."""
        self.plc.set_tag("FB300_EM_Heating.Enable", True)
        self.plc.set_tag("FB300_EM_Heating.Valve_Demand_In", 80.0)
        self.run_heating_logic(current_time=0.0)
        self.assertEqual(self.plc.get_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO"), 80.0, "Valve should be open initially")

        # Simulate freeze fault
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Freeze_Stat_DI", True)
        self.run_heating_logic(current_time=0.0)

        self.assertTrue(self.plc.get_tag("FB300_EM_Heating.UDT.HW_Freeze_Alm"), "Freeze alarm should be active")
        self.assertEqual(self.plc.get_tag("FB300_EM_Heating.UDT.HW_Valve_Cmd_AO"), 0.0, "Valve should close on freeze fault")

    def test_valve_failure_alarm(self):
        """TC3: Test that the heating valve failure alarm triggers on mismatch."""
        self.plc.set_tag("FB300_EM_Heating.Enable", True)

        # Command valve to 50%
        self.plc.set_tag("FB300_EM_Heating.Valve_Demand_In", 50.0)

        # Simulate feedback mismatch (e.g., valve stuck at 20%)
        self.plc.set_tag("FB300_EM_Heating.UDT.HW_Valve_Fdbk_AI", 20.0)

        # Simulate time passing
        current_time = 0.0

        # Run logic for 4.9 seconds, alarm should be false
        while current_time < 4.9:
            self.run_heating_logic(current_time=current_time)
            self.assertFalse(self.plc.get_tag("FB300_EM_Heating.UDT.Valve_Failure_Alm"))
            current_time += 0.1 # Simulate 100ms scan time

        # At 5 seconds, alarm should trigger
        self.run_heating_logic(current_time=5.0)
        self.assertTrue(self.plc.get_tag("FB300_EM_Heating.UDT.Valve_Failure_Alm"))

if __name__ == '__main__':
    unittest.main()
