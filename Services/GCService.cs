namespace ChesnokMessengerAPI.Services
{
    public class GCService
    {
        private int wsConnections;
        public int WSConnections { get => wsConnections; }
        private TimeSpan collectionInterval;
        public GCService()
        {
            collectionInterval = TimeSpan.FromSeconds(3);
            wsConnections = 0;

            while (true)
            {
                if(wsConnections > 0)
                {
                    GC.Collect();
                }
                Thread.Sleep(collectionInterval);
            }
        }
        public void AddConnection()
        {
            wsConnections++;
        }
        public void RemoveConnection()
        {
            wsConnections--;
        }
    }
}
