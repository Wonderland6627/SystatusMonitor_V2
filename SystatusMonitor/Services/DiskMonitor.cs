using System.Diagnostics;
using System.IO;

namespace SystatusMonitor.Services;

/// <summary>
/// 硬盘使用率监控器
/// </summary>
public class DiskMonitor
{
    /// <summary>
    /// 获取系统盘（通常是C盘）使用率 (0-100)
    /// </summary>
    public float GetUsage()
    {
        try
        {
            var drive = new DriveInfo("C:");
            if (drive.IsReady)
            {
                var totalSpace = drive.TotalSize;
                var freeSpace = drive.AvailableFreeSpace;
                var usedSpace = totalSpace - freeSpace;
                
                if (totalSpace > 0)
                {
                    return (float)(usedSpace / (double)totalSpace * 100.0);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting Disk usage: {ex.Message}");
        }
        return 0;
    }
}

