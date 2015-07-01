namespace MonitorServerApplication.ServerThreading
{
    public static class TimingConstants
    {
        public const int MaxTimeToWait = 2 * 1000; // In milliseconds
        
        public const int ClientCommunicationTimeout = 30 * 1000; // in milliseconds

        public const int DefaultWaitTime = 100; //In milliseconds, increasing this Time will lead to little bit less overhead on re-waiting, but will increase (like *3-*5 times stop Time)
    }
}
