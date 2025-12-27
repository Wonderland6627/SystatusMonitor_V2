using System.Diagnostics;

namespace SystatusMonitor.Services;

/// <summary>
/// 内存使用率监控器
/// </summary>
public class MemoryMonitor : IDisposable
{
    private PerformanceCounter? _memoryCounter;
    private bool _disposed = false;

    public MemoryMonitor()
    {
        try
        {
            _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize Memory counter: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取内存使用率 (0-100)
    /// </summary>
    public float GetUsage()
    {
        try
        {
            if (_memoryCounter != null)
            {
                var availableMB = _memoryCounter.NextValue();
                var totalMemory = GetTotalMemoryMB();
                if (totalMemory > 0)
                {
                    var usedMemory = totalMemory - availableMB;
                    return (float)(usedMemory / totalMemory * 100.0);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting Memory usage: {ex.Message}");
        }
        return 0;
    }

    /// <summary>
    /// 获取总内存 (MB)
    /// </summary>
    private static float GetTotalMemoryMB()
    {
        try
        {
            var pc = new PerformanceCounter("Memory", "Total MBytes");
            var total = pc.NextValue();
            pc.Dispose();
            return total;
        }
        catch
        {
            return 0;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _memoryCounter?.Dispose();
            _disposed = true;
        }
    }
}

