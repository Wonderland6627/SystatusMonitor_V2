using System.Diagnostics;
using System.Management;

namespace SystatusMonitor.Services;

/// <summary>
/// 内存使用率监控器
/// </summary>
public class MemoryMonitor : IDisposable
{
    private bool _disposed = false;

    public MemoryMonitor()
    {
        // 使用WMI查询内存信息，更可靠
    }

    /// <summary>
    /// 获取内存使用率 (0-100)
    /// </summary>
    public float GetUsage()
    {
        try
        {
            // 使用WMI查询操作系统内存信息
            using var searcher = new ManagementObjectSearcher(
                "SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            
            foreach (ManagementObject obj in searcher.Get())
            {
                var totalMemoryKB = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                var freeMemoryKB = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                
                if (totalMemoryKB > 0)
                {
                    var usedMemoryKB = totalMemoryKB - freeMemoryKB;
                    var usage = (float)(usedMemoryKB / (double)totalMemoryKB * 100.0);
                    return usage;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting Memory usage: {ex.Message}");
        }
        return 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

