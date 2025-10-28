import unittest
from unittest.mock import patch
import time

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

class TestDamperModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values
        self.plc.set_tag("FB400_EM_Damper.Enable", False)
        self.plc.set_tag("FB400_EM_Damper.Econ_Mode_Active", False)
        self.plc.set_tag("FB400_EM_Damper.Econ_PID_Demand", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Min_Fresh_Air_Pos", 20.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", False)
        self.fault_timer_start = None

    def run_damper_logic(self):
        """Simulates one scan of the FB400_EM_Damper logic."""
        enable = self.plc.get_tag("FB400_EM_Damper.Enable")
        econ_active = self.plc.get_tag("FB400_EM_Damper.Econ_Mode_Active")
        pid_demand = self.plc.get_tag("FB400_EM_Damper.Econ_PID_Demand")
        min_pos = self.plc.get_tag("FB400_EM_Damper.UDT.Min_Fresh_Air_Pos")
        cmd = self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO")
        fdbk = self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI")

        failure_alm = self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm")
        pos_cmd = 0.0

        if enable:
            if econ_active:
                pos_cmd = pid_demand
            else:
                pos_cmd = min_pos

            # Time-delayed failure logic
            if abs(pos_cmd - fdbk) > 5.0: # 5% tolerance
                if self.fault_timer_start is None:
                    self.fault_timer_start = time.time()
                if (time.time() - self.fault_timer_start) >= 5:
                    failure_alm = True
            else:
                self.fault_timer_start = None
                failure_alm = False

        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO", pos_cmd)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", failure_alm)

    def test_min_vent_position(self):
        """TC1: Test minimum ventilation position."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Econ_Mode_Active", False)
        self.run_damper_logic()
        self.assertEqual(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO"), 20.0)

    def test_econ_mode(self):
        """TC2: Test economizer mode passthrough."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Econ_Mode_Active", True)
        self.plc.set_tag("FB400_EM_Damper.Econ_PID_Demand", 65.0)
        self.run_damper_logic()
        self.assertEqual(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO"), 65.0)

    @patch('time.time')
    def test_damper_failure(self, mock_time):
        """TC3: Test damper failure alarm with mocked time delay."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Econ_Mode_Active", True)
        self.plc.set_tag("FB400_EM_Damper.Econ_PID_Demand", 50.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI", 10.0) # Stuck

        # 1. First scan, timer starts
        mock_time.return_value = 2000.0
        self.run_damper_logic()
        self.assertFalse(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm"))

        # 2. Advance time by 6 seconds
        mock_time.return_value = 2006.0
        self.run_damper_logic()
        self.assertTrue(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm"))

if __name__ == '__main__':
    unittest.main()
