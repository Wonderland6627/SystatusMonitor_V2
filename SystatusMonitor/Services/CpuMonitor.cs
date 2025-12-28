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
            // Windows 10/11 任务管理器使用 "Processor Information" 类别的 "% Processor Utility" 计数器
            _cpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total");
            _cpuCounter.NextValue(); // 第一次调用返回0，用于初始化
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize CPU counter: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取CPU使用率 (0-100)
    /// PerformanceCounter.NextValue() 返回的是自上次调用以来的平均值
    /// 这与Windows任务管理器使用的计算方法相同
    /// 注意：为了获得准确的值，调用间隔应该至少1秒
    /// </summary>
    public float GetUsage()
    {
        try
        {
            if (_cpuCounter != null)
            {
                // PerformanceCounter.NextValue() 返回自上次调用以来的平均值
                // 这与任务管理器使用的计算方法完全相同
                // 如果调用间隔是1秒，返回的就是过去1秒的平均CPU使用率
                var value = _cpuCounter.NextValue();
                
                return Math.Clamp(value, 0, 100);
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

