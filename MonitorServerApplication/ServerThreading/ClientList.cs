using System.Collections.Generic;
using System.Linq;

namespace MonitorServerApplication.ServerThreading
{
    class ClientList : List<ClientThread>
    {
        public void RemoveInnactive()
        {
            var matches = this.Where(clientThread => clientThread.IsFinished).ToList();
            foreach (var clientThread in matches)
            {
                this.Remove(clientThread);
            }
        }

        public void Stop()
        {
            foreach (var clientThread in this)
            {
                clientThread.IsShouldFinishWork = true;
            }
        }
    }
}
