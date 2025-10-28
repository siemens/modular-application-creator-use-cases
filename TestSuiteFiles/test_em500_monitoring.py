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

class TestMonitoringModule(unittest.TestCase):
    def setUp(self):
        """Set up a new mock PLC for each test."""
        self.plc = MockPLC()
        # Set default values
        self.plc.set_tag("FB500_EM_Monitoring.Enable", False)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI", False)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", False)
        self.start_time = None

    def run_monitoring_logic(self):
        """Simulates one scan of the FB500_EM_Monitoring logic."""
        enable = self.plc.get_tag("FB500_EM_Monitoring.Enable")
        dirty_filter_di = self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI")

        alarm = self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm")

        if enable and dirty_filter_di:
            if self.start_time is None:
                self.start_time = time.time()

            if (time.time() - self.start_time) >= 10:
                alarm = True
        else:
            self.start_time = None

        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm", alarm)

    @patch('time.time')
    def test_dirty_filter_alarm(self, mock_time):
        """TC1: Test dirty filter alarm with mocked time delay."""
        # --- Test Setup ---
        self.plc.set_tag("FB500_EM_Monitoring.Enable", True)
        self.plc.set_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_DI", True)

        # --- Simulation ---
        # 1. First scan: Alarm should be false, timer starts
        mock_time.return_value = 1000.0
        self.run_monitoring_logic()
        self.assertFalse(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"), "Alarm should be OFF on first scan")

        # 2. Advance time by 11 seconds, timer should have expired
        mock_time.return_value = 1011.0
        self.run_monitoring_logic()

        # --- Verification ---
        self.assertTrue(self.plc.get_tag("FB500_EM_Monitoring.UDT.Dirty_Filter_Alm"), "Alarm should be ON after delay")

if __name__ == '__main__':
    unittest.main()
