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

class TestSupplyFan(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values for the UDT
        self.plc.set_tag("FB100_EM_SupplyFan.Enable", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Run_Fdbk_DI", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Airflow_Status_DI", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.VFD_Fault_DI", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Start_Cmd_DO", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm", False)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Is_Running", False)

    def run_fan_logic(self):
        """Simulates one scan of the FB100_EM_SupplyFan logic."""

        # Inputs from UDT
        enable = self.plc.get_tag("FB100_EM_SupplyFan.Enable")
        run_feedback = self.plc.get_tag("FB100_EM_SupplyFan.UDT.Run_Fdbk_DI")
        vfd_fault = self.plc.get_tag("FB100_EM_SupplyFan.UDT.VFD_Fault_DI")

        # Basic logic simulation
        is_running = False
        fan_failure = self.plc.get_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm")

        if enable and not vfd_fault:
            self.plc.set_tag("FB100_EM_SupplyFan.UDT.Start_Cmd_DO", True)
            if run_feedback:
                is_running = True
            else:
                # Simplified failure logic for demonstration
                fan_failure = True
        else:
            self.plc.set_tag("FB100_EM_SupplyFan.UDT.Start_Cmd_DO", False)
            is_running = False

        # Outputs to UDT
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Is_Running", is_running)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm", fan_failure)


    def test_normal_start_sequence(self):
        """TC1: Test normal fan start and run sequence."""
        print(f"Running test on PLC instance: {PLC_INSTANCE_NAME}")

        # Step 1: Command Fan ON
        self.plc.set_tag("FB100_EM_SupplyFan.Enable", True)
        self.run_fan_logic()
        self.assertTrue(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Start_Cmd_DO"), "Start command should be ON")
        self.assertFalse(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm"), "Fan failure alarm should be OFF")

        # Step 2: Simulate Feedback
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Run_Fdbk_DI", True)
        self.run_fan_logic()
        self.assertTrue(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Is_Running"), "Is_Running status should be TRUE")
        self.assertFalse(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm"), "Fan failure alarm should remain OFF")

    def test_fan_failure_no_feedback(self):
        """TC2: Test fan failure alarm when no feedback is received."""
        print(f"Running test on PLC instance: {PLC_INSTANCE_NAME}")

        # Command Fan ON with no feedback
        self.plc.set_tag("FB100_EM_SupplyFan.Enable", True)
        self.plc.set_tag("FB100_EM_SupplyFan.UDT.Run_Fdbk_DI", False)
        self.run_fan_logic()

        # Check for failure alarm
        self.assertTrue(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Fan_Failure_Alm"), "Fan failure alarm should be ON")
        self.assertFalse(self.plc.get_tag("FB100_EM_SupplyFan.UDT.Is_Running"), "Is_Running status should be FALSE")

if __name__ == '__main__':
    unittest.main()
