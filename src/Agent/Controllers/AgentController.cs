using Agent.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Agent.Controllers
{
    [Route("agent")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        /// <summary>
        /// Get device info
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            var device = new Device
            {
                TotalMem = GetTotalMem(),
                AvailableMem = GetAvailableMem(),
                CpuCores = GetCpuCores(),
                CpuModel = GetCpuModel(),
                CpuFrequency = GetCpuFrequency(),
                CpuCacheSize = GetCpuCpuCacheSize()
            };

            return Ok(device);
        }

        private int GetTotalMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemTotal' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        private int GetAvailableMem()
        {
            var result = GetDevice("cat /proc/meminfo | grep 'MemFree' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            var value = result.ToLower().Replace("kb", string.Empty).Trim();

            return int.Parse(value) / 1024;
        }

        private int GetCpuCores()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'cpu cores' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            return int.Parse(result);
        }

        private string GetCpuModel()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'model name' | awk -F':' '{print $2}'");

            return result?.Trim();
        }

        private double GetCpuFrequency()
        {
            var result = GetDevice("cat /proc/cpuinfo | grep -m1 'cpu MHz' | awk -F':' '{print $2}'");

            if (result == null)
            {
                return 0;
            }

            return double.Parse(result);
        }

        private int GetCpuCpuCacheSize()
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
                var (code, message) = ExecuteCommand(command);

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

        private (int, string) ExecuteCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var message = process.StandardOutput.ReadToEnd();
            if (process.ExitCode != 0)
            {
                message = process.StandardError.ReadToEnd();
            }

            return (process.ExitCode, message);
        }
    }
}
