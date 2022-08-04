namespace laserCraft_Control.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));
        private int _minS = 0;
        private int _maxS = 5000;
        private bool _isMetric = true;
        private string _baud = "115200";
        private string _dataBits = "8";
        private string _stopBits = "One";
        private string _parity = "None";
        private string _handShaking = "None";
        private int _pixelDelay = 20;
        private int _feedRate = 1500;
        private int _laserIntensity = 255;
        private string _language = "Vietnamese";
        private int _cutFeedrate = 400;
        private int _cutLaserIntensity = 255;
        private int _cutRepeat = 2;
        private double _maxWorkingX = 350;
        private double _maxWorkingY = 500;
        private bool _ignoreError = true;
        public static Settings Default =>
            defaultInstance;

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("0")]
        public int minS { get { return _minS; } set { _minS = value; } }
        

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("5000")]
        public int maxS { get { return _maxS; } set { _maxS = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool isMetric { get { return _isMetric; } set { _isMetric = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("115200")]
        public string baud { get { return _baud; } set { _baud = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("8")]
        public string dataBits { get { return _dataBits; } set { _dataBits = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("One")]
        public string stopBits { get { return _stopBits; } set { _stopBits = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("None")]
        public string parity { get { return _parity; } set { _parity = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("None")]
        public string handShaking { get { return _handShaking; } set { _handShaking = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("20")]
        public int pixelDelay { get { return _pixelDelay; } set { _pixelDelay = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("1500")]
        public int feedRate { get { return _feedRate; } set { _feedRate = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("255")]
        public int laserIntensity { get { return _laserIntensity; } set { _laserIntensity = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("Vietnamese")]
        public string language { get { return _language; } set { _language = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("400")]
        public int cutFeedrate { get { return _cutFeedrate; } set { _cutFeedrate = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("255")]
        public int cutLaserIntensity { get { return _cutLaserIntensity; } set { _cutLaserIntensity = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("2")]
        public int cutRepeat { get { return _cutRepeat; } set { _cutRepeat = value; } }
        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("350")]
        public double maxWorkingX { get { return _maxWorkingX; } set { _maxWorkingX = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("500")]
        public double maxWorkingY { get { return _maxWorkingY; } set { _maxWorkingY = value; } }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool ignoreError { get { return _ignoreError; } set { _ignoreError = value; } }
    }
}

