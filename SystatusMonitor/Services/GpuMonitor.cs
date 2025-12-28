using System.Diagnostics;

namespace SystatusMonitor.Services;

/// <summary>
/// GPU使用率监控器 - 仅监控3D引擎使用率（与任务管理器一致）
/// </summary>
public class GpuMonitor : IDisposable
{
    private List<PerformanceCounter>? _gpuCounters;
    private readonly Dictionary<PerformanceCounter, int> _counterPhysIdCache = new(); // 缓存physId，避免重复正则匹配
    private static readonly System.Text.RegularExpressions.Regex PhysIdRegex = new(@"phys_(\d+)", System.Text.RegularExpressions.RegexOptions.Compiled);
    private readonly Queue<float> _usageHistory = new();
    private const int SmoothingWindowSize = 1; // 使用最近3次的值进行平滑
    private bool _disposed = false;

    public GpuMonitor()
    {
        try
        {
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
            // 只使用3D引擎的使用率（与任务管理器一致）
            if (!PerformanceCounterCategory.Exists("GPU Engine"))
            {
                return;
            }

            var category = new PerformanceCounterCategory("GPU Engine");
            var instanceNames = category.GetInstanceNames();
            const string counterName = "Utilization Percentage";
            
            // 只收集3D引擎实例
            _gpuCounters = new List<PerformanceCounter>();
            
            foreach (var instanceName in instanceNames)
            {
                // 只收集3D引擎
                if (instanceName.Contains("engtype_3D"))
                {
                    try
                    {
                        var counter = new PerformanceCounter("GPU Engine", counterName, instanceName);
                        counter.NextValue(); // 第一次调用返回0，用于初始化
                        _gpuCounters.Add(counter);
                        
                        // 预提取并缓存physId，避免每次调用时重复正则匹配
                        var physMatch = PhysIdRegex.Match(instanceName);
                        int physId = 0;
                        if (physMatch.Success)
                        {
                            physId = int.Parse(physMatch.Groups[1].Value);
                        }
                        _counterPhysIdCache[counter] = physId;
                    }
                    catch
                    {
                        // 忽略无法创建的计数器
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
    /// 获取GPU使用率 (0-100) - 仅3D引擎
    /// 任务管理器可能显示的是所有活跃3D引擎实例的平均值或最大值
    /// </summary>
    public float GetUsage()
    {
        try
        {
            if (_gpuCounters == null || _gpuCounters.Count == 0)
            {
                return 0;
            }

            // 优化：在单次循环中完成所有计算，减少临时对象分配
            var gpuUsageByPhys = new Dictionary<int, float>(); // 直接存储累加值，而不是List
            var countersToRemove = new List<PerformanceCounter>();
            
            foreach (var counter in _gpuCounters)
            {
                try
                {
                    var value = counter.NextValue();
                    if (value >= 0 && value <= 100 && value > 0.1f) // 直接过滤，避免后续Where操作
                    {
                        // 使用缓存的physId，避免重复正则匹配
                        int physId = _counterPhysIdCache.TryGetValue(counter, out var cachedPhysId) 
                            ? cachedPhysId 
                            : 0;
                        
                        // 直接累加，避免创建List
                        if (!gpuUsageByPhys.ContainsKey(physId))
                        {
                            gpuUsageByPhys[physId] = 0f;
                        }
                        gpuUsageByPhys[physId] += value;
                    }
                }
                catch
                {
                    // 计数器已失效，标记为删除
                    countersToRemove.Add(counter);
                }
            }
            
            // 清理无效的计数器
            foreach (var counter in countersToRemove)
            {
                _gpuCounters.Remove(counter);
                _counterPhysIdCache.Remove(counter);
                try { counter.Dispose(); } catch { }
            }
            
            if (gpuUsageByPhys.Count > 0)
            {
                // 任务管理器累加所有活跃的3D引擎实例，但限制在100%以内
                // 直接使用主GPU（phys_0）的累加值
                var rawResult = Math.Min(
                    gpuUsageByPhys.ContainsKey(0) 
                        ? gpuUsageByPhys[0] 
                        : gpuUsageByPhys.Values.Max(), 
                    100f);
                
                // 使用移动平均平滑处理，减少波动
                _usageHistory.Enqueue(rawResult);
                if (_usageHistory.Count > SmoothingWindowSize)
                {
                    _usageHistory.Dequeue();
                }
                
                // 计算移动平均
                var smoothedResult = _usageHistory.Count > 0 
                    ? _usageHistory.Average() 
                    : rawResult;
                
                return Math.Min(smoothedResult, 100f);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting GPU usage: {ex.Message}");
        }
        
        return 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_gpuCounters != null)
            {
                foreach (var counter in _gpuCounters)
                {
                    try
                    {
                        counter.Dispose();
                    }
                    catch { }
                }
                _gpuCounters.Clear();
                _gpuCounters = null;
            }
            
            _disposed = true;
        }
    }
}

