using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorServerApplication.DB
{
    public interface IDataGetter
    {
        ClientSettings GetSettings(SettingsType settingsType);
    }
}
