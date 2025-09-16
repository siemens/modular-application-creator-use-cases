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

class TestCoolingModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values for the UDT and inputs
        self.plc.set_tag("FB200_EM_Cooling.Enable", False)
        self.plc.set_tag("FB200_EM_Cooling.Valve_Demand_In", 0.0)
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI", False)
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO", 0.0)
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm", False)

    def run_cooling_logic(self):
        """Simulates one scan of the FB200_EM_Cooling logic."""
        enable = self.plc.get_tag("FB200_EM_Cooling.Enable")
        demand = self.plc.get_tag("FB200_EM_Cooling.Valve_Demand_In")
        freeze_stat = self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI")

        valve_cmd = 0.0
        freeze_alm = False

        if enable and not freeze_stat:
            valve_cmd = demand

        if freeze_stat:
            freeze_alm = True
            valve_cmd = 0.0 # Safety override

        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO", valve_cmd)
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm", freeze_alm)

    def test_valve_passthrough(self):
        """TC1: Test that the valve command follows the demand input."""
        print(f"Running cooling test on PLC instance: {PLC_INSTANCE_NAME}")
        self.plc.set_tag("FB200_EM_Cooling.Enable", True)

        # Test 50% demand
        self.plc.set_tag("FB200_EM_Cooling.Valve_Demand_In", 50.0)
        self.run_cooling_logic()
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO"), 50.0)

        # Test 100% demand
        self.plc.set_tag("FB200_EM_Cooling.Valve_Demand_In", 100.0)
        self.run_cooling_logic()
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO"), 100.0)

    def test_freeze_safety_trip(self):
        """TC2: Test that the freeze stat safety overrides the demand."""
        self.plc.set_tag("FB200_EM_Cooling.Enable", True)
        self.plc.set_tag("FB200_EM_Cooling.Valve_Demand_In", 80.0)
        self.run_cooling_logic()
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO"), 80.0, "Valve should be open initially")

        # Simulate freeze fault
        self.plc.set_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Stat_DI", True)
        self.run_cooling_logic()

        self.assertTrue(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Freeze_Alm"), "Freeze alarm should be active")
        self.assertEqual(self.plc.get_tag("FB200_EM_Cooling.UDT.CHW_Valve_Cmd_AO"), 0.0, "Valve should close on freeze fault")

if __name__ == '__main__':
    unittest.main()
