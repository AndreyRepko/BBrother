using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorServerApplication
{
    internal static class OldProtocolConst
    {
//Максимальное количество передаваемых файлов
        public const Int32 MAX_NUMB_FILES = 50;
        // Константы реализующие новый протокол
        //сигнатуры
        public const Int32 Con_Begin         = 0x00100001; // Начало соединения  +
        public const Int32 Con_End           = 0x00100002; // Конец соединения
        public const Int32 Con_Data_Archive  = 0x00100011; // Архивные данные
        public const Int32 Con_Data_Timer    = 0x00100012; // Таймер             +
        public const Int32 Con_Data_WindChng = 0x00100013; // Переключение окна  +
        public const Int32 Con_Sett_Reqest   = 0x00100021; // Запрос на настройки  +/-
        public const Int32 Con_Sett_Recive   = 0x00100022; // Получение настроек
        public const Int32 Con_Info_Msg      = 0x00100030; // Информационное сообщение
        public const Int32 Con_File          = 0x00100040; // Файл с обновлениями

// типы пакетов(для занесенияв СУБД)    -1 - пока не используются
        public const Int32 TR_Con_Begin         = -1; // Начало соединения
        public const Int32 TR_Con_End           = -1; // Конец соединения
        public const Int32 TR_Con_Data_Archive  = 11; // Архивные данные
        public const Int32 TR_Con_Data_Timer    = 12; // Таймер
        public const Int32 TR_Con_Data_WindChng = 10; // Переключение окна
        public const Int32 TR_Con_Sett_Reqest   = -1; // Запрос на настройки
        public const Int32 TR_Con_Sett_Recive   = -1; // Получение настроек
        public const Int32 TR_Con_Info_Msg      = -1; // Информационное сообщение

// Доп. константы
        public const string Con_OK = "OK";// Подтверждение приема
        public const string Con_Failed ="FD"; // Подтверждение приема

// типы пакетов
        public const Int32 Pack_Inform      = 0x00000001; // Пакет с информационным сообщением
        public const Int32 Pack_Data        = 0x00000002; // Пакет с данными

//типы запросов на настройки
        public const Int32 Sett_ForUser     = 0x00000001; // Пользовательские
        public const Int32 Sett_Default     = 0x00000002; // По умолчанию
        public const Int32 Sett_ForUserOld  = 0x00000003; // Старые пользовательские

//строковые константы параметров 
        public const string c_server_port         = "SERVER_PORT"; //ma_settings.serverport
        public const string c_server_name         = "SERVER_NAME"; //ma_settings.ipserver
        public const string c_res_server_port     = "RES_SERVER_PORT"; //ma_settings.ipresserver
        public const string c_res_server_name     = "RES_SERVER_NAME"; //ma_settings.resserverport
        public const string c_server_scan         = "SERVER_SCAN"; //ma_settings.scanperiod
        public const string c_count_records       = "COUNT_RECORDS_FOR_SEND_ARC"; //ma_settings.sendrecordcount
        public const string c_st_period           = "ST_PERIOD";//ma_settings.snapshottime
        public const string c_no_registry_time    = "NO_REGISTRY_TIME"; //ma_settings.changemintime
        public const string c_set_test_regime     = "SET_TEST_REGIME"; //ma_settings.workmode
        public const string c_need_screenshot     = "NEED_SCREENSHOT"; //ma_settings.needscreenshot
        public const string c_admin_password      = "ADMIN_PASSWORD"; //ma_settings.adminpassword
        public const string c_inactive_delay      = "INACTIVE_DELAY"; //ma_settings.inactivedelay
        public const string c_none_active_time    = "NONE_ACTIVE_TIME"; //ma_settings.nonactivetime
        public const string c_save_to_file_period = "SAVE_TO_FILE_PERIOD";
        public const string c_min_free_space      = "MIN_FREE_SPACE";
        public const string c_sett_recive         = "SETTING_RECIVE";
        public const string c_gif_colors          = "GIF_COLOR_COUNT";
        public const string c_dithering           = "GIF_DITHERING";
        public const string c_jpg_qaul            = "JPEG_QUALITY";
        public const string c_snapshot_time_disp  = "ST_PERIOD_RANDOMIZER";
        public const string c_options_scan_time   = "OPTIONS_SCAN_TIME";
//Супер новые настройки
        public const string c_servertime          = "SERVER_TIME";
        public const string c_jpg_only            = "JPG_ONLY";
        public const string c_allow_public        = "ALLOW_public";
        public const string c_public_day_limit    = "public_DAY_LIMIT";

//      Номер порта, используемого по умолчанию
//const DefaultPortNum 3143
        public const Int32 DefaultPortNumUDP  = 5001;
        public const Int32 DefaultPortNum     = 9876;

        public const Int32 InfoMessage_LastApplication     = -10;
        public const Int32 InfoMessage_TimeIntervalProblem = 17;
        public const Int32 InfoMessage_UpdateComponent     = 20; // Обновление файлов
        public const Int32 InfoMessage_publicEnd           = 19; // Пользователь досрочно отключил приват режим
        public const Int32 InfoMessage_publicStart         = 18;// Приват режим перешол в состояние "включен"
    }
}
