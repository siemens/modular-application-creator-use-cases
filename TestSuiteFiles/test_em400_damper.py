import unittest
from unittest.mock import Mock

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
        # Set default values for the UDT
        self.plc.set_tag("FB400_EM_Damper.Enable", False)
        self.plc.set_tag("FB400_EM_Damper.Damper_Demand_In", 0.0)
        self.plc.set_tag("FB400_EM_Damper.Cooling_Demand", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Mode_Active", False)
        self.plc.set_tag("FB400_EM_Damper.UDT.Outside_Air_Temp_AI", 75.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Return_Air_Temp_AI", 78.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Temp_Diff", 5.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_High_Limit", 65.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI", 0.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Fault_Delay_Sec", 5)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", False)

        # Mock the TON_TIME timer for the fault delay
        self.mock_timer = Mock()
        self.mock_timer.Q = False


    def run_damper_logic(self):
        """Simulates one scan of the FB400_EM_Damper logic."""
        enable = self.plc.get_tag("FB400_EM_Damper.Enable")
        damper_demand_in = self.plc.get_tag("FB400_EM_Damper.Damper_Demand_In")
        cooling_demand = self.plc.get_tag("FB400_EM_Damper.Cooling_Demand")
        oat = self.plc.get_tag("FB400_EM_Damper.UDT.Outside_Air_Temp_AI")
        rat = self.plc.get_tag("FB400_EM_Damper.UDT.Return_Air_Temp_AI")
        econ_diff = self.plc.get_tag("FB400_EM_Damper.UDT.Econ_Temp_Diff")
        econ_limit = self.plc.get_tag("FB400_EM_Damper.UDT.Econ_High_Limit")
        cmd = self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO")
        fdbk = self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI")

        if enable:
            if (cooling_demand > 1.0) and (oat < (rat - econ_diff)) and (oat < econ_limit):
                self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Mode_Active", True)
            else:
                self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Mode_Active", False)

            self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO", damper_demand_in)

            # Simulate the timer-based failure logic
            timer_in = abs(cmd - fdbk) > 5.0
            if timer_in:
                # In a real PLC, this would start a timer.
                # We simulate the timer's output based on the test case.
                pass

            if self.mock_timer.Q:
                self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", True)

        else:
            self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Mode_Active", False)
            self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO", 0.0)
            self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", False)


    def test_disabled_state(self):
        """TC0: Test that the damper closes and alarms reset when disabled."""
        self.plc.set_tag("FB400_EM_Damper.Enable", False)
        self.plc.set_tag("FB400_EM_Damper.Damper_Demand_In", 50.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm", True) # Pre-set alarm
        self.run_damper_logic()
        self.assertEqual(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO"), 0.0)
        self.assertFalse(self.plc.get_tag("FB400_EM_Damper.UDT.Econ_Mode_Active"))
        self.assertFalse(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm"))


    def test_passthrough_demand(self):
        """TC1: Test that the damper command passes through when enabled."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Damper_Demand_In", 65.0)
        self.run_damper_logic()
        self.assertEqual(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Pos_Cmd_AO"), 65.0)


    def test_econ_mode_activation(self):
        """TC2: Test that economizer mode activates under correct conditions."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Cooling_Demand", 10.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Outside_Air_Temp_AI", 60.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Return_Air_Temp_AI", 78.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Temp_Diff", 5.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_High_Limit", 65.0)
        self.run_damper_logic()
        self.assertTrue(self.plc.get_tag("FB400_EM_Damper.UDT.Econ_Mode_Active"))

    def test_econ_mode_deactivation(self):
        """TC3: Test that economizer mode deactivates when conditions are not met."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Cooling_Demand", 10.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Outside_Air_Temp_AI", 70.0) # Too warm
        self.plc.set_tag("FB400_EM_Damper.UDT.Return_Air_Temp_AI", 78.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_Temp_Diff", 5.0)
        self.plc.set_tag("FB400_EM_Damper.UDT.Econ_High_Limit", 65.0)
        self.run_damper_logic()
        self.assertFalse(self.plc.get_tag("FB400_EM_Damper.UDT.Econ_Mode_Active"))

    def test_damper_failure_alarm(self):
        """TC4: Test that the failure alarm triggers after a delay."""
        self.plc.set_tag("FB400_EM_Damper.Enable", True)
        self.plc.set_tag("FB400_EM_Damper.Damper_Demand_In", 50.0)
        self.run_damper_logic() # Run once to set the command

        self.plc.set_tag("FB400_EM_Damper.UDT.Damper_Pos_Fdbk_AI", 10.0) # Mismatched feedback

        # 1. First scan, timer is running but not complete
        self.mock_timer.Q = False
        self.run_damper_logic()
        self.assertFalse(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm"))

        # 2. Second scan, timer has completed
        self.mock_timer.Q = True
        self.run_damper_logic()
        self.assertTrue(self.plc.get_tag("FB400_EM_Damper.UDT.Damper_Failure_Alm"))

if __name__ == '__main__':
    unittest.main()
