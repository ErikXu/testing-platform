namespace WebApi.Services
{
    public interface IDeviceService
    {
        public int GetTotalMem();

        public int GetAvailableMem();

        public int GetCpuCores();

        public string GetCpuModel();

        public double GetCpuFrequency();

        public int GetCpuCpuCacheSize();
    }

    public class DeviceService : IDeviceService
    {
        private readonly ICommandService _commandService;

        public DeviceService(ICommandService commandService)
        {
            _commandService = commandService;
        }

        public int GetTotalMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemTotal' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        public int GetAvailableMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemFree' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        public int GetCpuCores()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'cpu cores' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            return int.Parse(result);
        }

        public string GetCpuModel()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'model name' | awk -F':' '{print $2}'");

            return result?.Trim();
        }

        public double GetCpuFrequency()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'cpu MHz' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            return double.Parse(result);
        }

        public int GetCpuCpuCacheSize()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'cache size' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value);
        }

        private string GetDevice(string command)
        {
            try
            {
                var (code, message) = _commandService.ExecuteCommand(command);

                if (code == 0)
                {
                    return message;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
