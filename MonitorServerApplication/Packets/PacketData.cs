using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorServerApplication.Packets
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
}
