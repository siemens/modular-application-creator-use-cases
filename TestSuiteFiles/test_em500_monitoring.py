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

class TestMonitoringModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values for the UDT
        self.plc.set_tag("FB500_EM_Monitoring.Enable", False)
        self.plc.set_tag("FB500_EM_Monitoring.Reset_Alarms", False)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI", False)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Filter_Delay_Sec", 10)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", False)

        # Mock the TON_TIME timer
        self.mock_timer = Mock()
        self.mock_timer.Q = False

    def run_monitoring_logic(self):
        """Simulates one scan of the FB500_EM_Monitoring logic."""
        enable = self.plc.get_tag("FB500_EM_Monitoring.Enable")
        reset_alarms = self.plc.get_tag("FB500_EM_Monitoring.Reset_Alarms")
        dirty_filter_di = self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI")

        if enable:
            # The timer is only considered "running" if the input is true.
            # The test case controls when the timer's output (.Q) becomes true.
            timer_is_done = self.mock_timer.Q if dirty_filter_di else False

            # If the timer finishes, latch the alarm.
            if timer_is_done:
                self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", True)

            # If the reset command is given, clear the alarm. This has higher priority.
            if reset_alarms:
                self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", False)
        else:
            # If the block is disabled, the alarm is always cleared.
            self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", False)

    def test_dirty_filter_alarm_with_delay(self):
        """TC1: Test that the dirty filter alarm activates after a delay."""
        self.plc.set_tag("FB500_EM_Monitoring.Enable", True)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI", True)

        # 1. First scan, timer is running but not complete
        self.mock_timer.Q = False
        self.run_monitoring_logic()
        self.assertFalse(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"))

        # 2. Second scan, timer has completed
        self.mock_timer.Q = True
        self.run_monitoring_logic()
        self.assertTrue(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"))

    def test_alarm_reset(self):
        """TC2: Test that the alarm can be reset."""
        self.plc.set_tag("FB500_EM_Monitoring.Enable", True)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", True) # Pre-set the alarm

        self.plc.set_tag("FB500_EM_Monitoring.Reset_Alarms", True)
        self.run_monitoring_logic()
        self.assertFalse(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"))

    def test_disabled_state(self):
        """TC3: Test that the alarm is cleared when the module is disabled."""
        self.plc.set_tag("FB500_EM_Monitoring.Enable", False)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", True) # Pre-set the alarm
        self.run_monitoring_logic()
        self.assertFalse(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"))

if __name__ == '__main__':
    unittest.main()
