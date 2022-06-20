namespace WebApi.Mongo.Entities
{
    public class Device
    {
        public int TotalMem { get; set; }

        public int AvailableMem { get; set; }

        public int CpuCores { get; set; }

        public string CpuModel { get; set; }

        public double CpuFrequency { get; set; }

        public double CpuCacheSize { get; set; }
    }
}
