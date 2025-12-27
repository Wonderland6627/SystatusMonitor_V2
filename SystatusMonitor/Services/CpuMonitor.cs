using System.Diagnostics;

namespace SystatusMonitor.Services;

/// <summary>
/// CPU使用率监控器
/// </summary>
public class CpuMonitor : IDisposable
{
    private PerformanceCounter? _cpuCounter;
    private bool _disposed = false;

    public CpuMonitor()
    {
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue(); // 第一次调用返回0，需要先调用一次
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize CPU counter: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取CPU使用率 (0-100)
    /// </summary>
    public float GetUsage()
    {
        try
        {
            if (_cpuCounter != null)
            {
                return _cpuCounter.NextValue();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting CPU usage: {ex.Message}");
        }
        return 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cpuCounter?.Dispose();
            _disposed = true;
        }
    }
}

