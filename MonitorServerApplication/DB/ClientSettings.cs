using System;
using System.Collections.Generic;
using System.Globalization;

namespace MonitorServerApplication.DB
{
    public enum SettingsType
    {
         Default = 0 , 
         Specific = 1
    }

    public interface ISettingReadOnly
    {
        string FieldName { get; }
        string DisplayName { get; }
        string GetStringValue();
    }
    
    public class SettingValue<T> : ISettingReadOnly
        where T: IConvertible
    {
        //Could be public if we would like to
        private readonly T Value;

        private readonly Func<T> ValueGetter;

        public string FieldName { get; private set; }

        public string DisplayName { get; private set; }

        public string GetStringValue()
        {
                var value = ValueGetter == null ? Value : ValueGetter();
                if (value == null)
                    throw new ArgumentNullException("Value", string.Format("Field name: {0}, Display name {1}", FieldName, DisplayName));
                return value.ToString(CultureInfo.InvariantCulture);
        }


        public SettingValue(string displayName, string fieldName, T defaultValue)
        {
            DisplayName = displayName;
            FieldName = fieldName;
            Value = defaultValue;
        }
        
        public SettingValue(string displayName, string fieldName, Func<T> dynamicValue)
        {
            DisplayName = displayName;
            FieldName = fieldName;
            ValueGetter = dynamicValue;
        }
    }
    
    public class ClientSettings
    {
        public List<ISettingReadOnly> Settings = new List<ISettingReadOnly>();

        public ClientSettings(bool createDefault = true)
        {
            Settings.Add(new SettingValue<uint>("Server Port", "SERVER_PORT", 2345));
            Settings.Add(new SettingValue<string>("ServerAdress", "SERVER_NAME", "127.0.0.1")); //ma_settings.ipserver
            Settings.Add(new SettingValue<uint>("ReserveServerPort", "RES_SERVER_PORT", 2345)); //ma_settings.ipresserver
            Settings.Add(new SettingValue<string>("ReserveServerAdress", "RES_SERVER_NAME", "192.168.1.1")); //ma_settings.resserverport
            Settings.Add(new SettingValue<uint>("ServerScanPeriod", "SERVER_SCAN", 30)); //ma_settings.scanperiod
            Settings.Add(new SettingValue<uint>("RecordsCountToSend", "COUNT_RECORDS_FOR_SEND_ARC", 2 * 60)); //ma_settings.sendrecordcount
            Settings.Add(new SettingValue<uint>("SnapshotPeriod", "ST_PERIOD", 5 * 60)); //ma_settings.snapshottime
            Settings.Add(new SettingValue<uint>("MinimumNonregistringTime", "NO_REGISTRY_TIME", 2)); //ma_settings.changemintime
            Settings.Add(new SettingValue<bool>("ActivateTest", "SET_TEST_REGIME", true)); //ma_settings.workmode
            Settings.Add(new SettingValue<bool>("MakeScrennshot", "NEED_SCREENSHOT", true)); //ma_settings.needscreenshot
            Settings.Add(new SettingValue<string>("AdminPassword", "ADMIN_PASSWORD", "qwerty")); //ma_settings.adminpassword
            Settings.Add(new SettingValue<uint>("InactivityDelay", "INACTIVE_DELAY", 2 * 60)); //ma_settings.inactivedelay
            Settings.Add(new SettingValue<uint>("NonActiveTime", "NONE_ACTIVE_TIME", 5 * 60)); //ma_settings.nonactivetime
            Settings.Add(new SettingValue<uint>("SaveToFilePeriod", "SAVE_TO_FILE_PERIOD", 10));
            Settings.Add(new SettingValue<UInt64>("MiniminFreeSpace", "MIN_FREE_SPACE", (UInt64) 10 * 1024 * 1024 * 1024)); //10Gb
            Settings.Add(new SettingValue<bool>("SettingsReceive", "SETTING_RECIVE", false));
            Settings.Add(new SettingValue<uint>("GIFColorsCount", "GIF_COLOR_COUNT", 256));
            Settings.Add(new SettingValue<bool>("GIFDithering", "GIF_DITHERING", true));
            Settings.Add(new SettingValue<uint>("JPGQuality", "JPEG_QUALITY", 100));
            Settings.Add(new SettingValue<uint>("SnapshotTimeRandomization", "ST_PERIOD_RANDOMIZER", 10)); //%
            Settings.Add(new SettingValue<uint>("OptionsScanTime", "OPTIONS_SCAN_TIME", 60));
            Settings.Add(new SettingValue<string>("ServerTime", "SERVER_TIME", () => DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")));
            Settings.Add(new SettingValue<bool>("TakeOnlyJPG", "JPG_ONLY", false));
            Settings.Add(new SettingValue<bool>("AllowPrivateRegime", "ALLOW_PRIVATE", true));
            Settings.Add(new SettingValue<uint>("PrivateRegimeTimeLimitation", "PRIVATE_DAY_LIMIT", 60));
        }
    }
}
