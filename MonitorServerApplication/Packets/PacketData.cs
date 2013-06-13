using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorServerApplication.PacketsDefinition
{
    public class PacketData
    {
        public int TaskId;
        public String NameExe; //имя exe файла
        public String ExePath;
        public String NewExe;             //новое окно exe файла
        public String CurUser;             //пользователь
        public String CompName;             //Название компьютера
        public String CompAdr;             //Адрес компьютера
        public DateTime StartPeriod;
        public DateTime EndPeriod;
        public int Keybs;
        public int Mouse;
        public byte[] Screenshot;
        public String Caption;
        public DateTime ActiveTime;
    }

    public class SettingsName
    {
        public const string CServerPort = "SERVER_PORT"; //ma_settings.serverport
        public const string CServerName = "SERVER_NAME"; //ma_settings.ipserver
        public const string c_res_server_port = "RES_SERVER_PORT"; //ma_settings.ipresserver
        public const string c_res_server_name = "RES_SERVER_NAME"; //ma_settings.resserverport
        public const string c_server_scan = "SERVER_SCAN"; //ma_settings.scanperiod
        public const string c_count_records = "COUNT_RECORDS_FOR_SEND_ARC"; //ma_settings.sendrecordcount
        public const string c_st_period = "ST_PERIOD";//ma_settings.snapshottime
        public const string c_no_registry_time = "NO_REGISTRY_TIME"; //ma_settings.changemintime
        public const string c_set_test_regime = "SET_TEST_REGIME"; //ma_settings.workmode
        public const string c_need_screenshot = "NEED_SCREENSHOT"; //ma_settings.needscreenshot
        public const string c_admin_password = "ADMIN_PASSWORD"; //ma_settings.adminpassword
        public const string c_inactive_delay = "INACTIVE_DELAY"; //ma_settings.inactivedelay
        public const string c_none_active_time = "NONE_ACTIVE_TIME"; //ma_settings.nonactivetime
        public const string c_save_to_file_period = "SAVE_TO_FILE_PERIOD";
        public const string c_min_free_space = "MIN_FREE_SPACE";
        public const string c_sett_recive = "SETTING_RECIVE";
        public const string c_gif_colors = "GIF_COLOR_COUNT";
        public const string c_dithering = "GIF_DITHERING";
        public const string c_jpg_qaul = "JPEG_QUALITY";
        public const string c_snapshot_time_disp = "ST_PERIOD_RANDOMIZER";
        public const string c_options_scan_time = "OPTIONS_SCAN_TIME";
        public const string c_servertime = "SERVER_TIME";
        public const string c_jpg_only = "JPG_ONLY";
        public const string c_allow_public = "ALLOW_public";
        public const string c_public_day_limit = "public_DAY_LIMIT";
        
    }
}
