using System.Diagnostics;
using System.Management;

namespace SystatusMonitor.Services;

/// <summary>
/// GPU使用率监控器
/// </summary>
public class GpuMonitor : IDisposable
{
    private PerformanceCounter? _gpuCounter;
    private bool _disposed = false;

    public GpuMonitor()
    {
        try
        {
            // 尝试使用WMI获取GPU信息
            // 注意：Windows的GPU监控需要特定的性能计数器，可能因显卡驱动而异
            // 这里提供一个基础实现，可能需要根据实际情况调整
            InitializeGpuCounter();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize GPU counter: {ex.Message}");
        }
    }

    private void InitializeGpuCounter()
    {
        try
        {
            // 尝试查找GPU性能计数器
            // NVIDIA: "GPU Engine" 或 "GPU"
            // AMD: "GPU Engine" 或 "GPU"
            // Intel: "GPU Engine"
            
            var categoryNames = new[] { "GPU Engine", "GPU", "GPU Process Memory" };
            
            foreach (var categoryName in categoryNames)
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    var category = new PerformanceCounterCategory(categoryName);
                    var instanceNames = category.GetInstanceNames();
                    if (instanceNames.Length > 0)
                    {
                        // 尝试创建计数器（具体实现可能需要根据实际系统调整）
                        Debug.WriteLine($"Found GPU category: {categoryName}");
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing GPU counter: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取GPU使用率 (0-100)
    /// </summary>
    public float GetUsage()
    {
        try
        {
            // 使用WMI查询GPU使用率
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_VideoController");
            
            foreach (ManagementObject obj in searcher.Get())
            {
                // 注意：Win32_VideoController不直接提供使用率
                // 这里返回0，实际实现可能需要使用特定显卡厂商的API
                // 或者使用第三方库如LibreHardwareMonitor
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting GPU usage: {ex.Message}");
        }
        
        // 暂时返回0，后续可以集成LibreHardwareMonitor或其他库
        return 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _gpuCounter?.Dispose();
            _disposed = true;
        }
    }
}

