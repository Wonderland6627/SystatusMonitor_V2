using System;
using System.Threading;
using SystatusMonitor.Models;

namespace SystatusMonitor.Services;

/// <summary>
/// 监控数据更新事件参数
/// </summary>
public class MonitorDataEventArgs : EventArgs
{
    public MonitorData Data { get; }

    public MonitorDataEventArgs(MonitorData data)
    {
        Data = data;
    }
}

/// <summary>
/// 系统监控服务 - 协调所有监控器
/// </summary>
public class SystemMonitorService : IDisposable
{
    private readonly CpuMonitor _cpuMonitor;
    private readonly MemoryMonitor _memoryMonitor;
    private readonly GpuMonitor _gpuMonitor;
    private readonly DiskMonitor _diskMonitor;
    private readonly NetworkMonitor _networkMonitor;

    private System.Threading.Timer? _updateTimer;
    private bool _isRunning = false;
    private bool _disposed = false;

    /// <summary>
    /// 数据更新事件
    /// </summary>
    public event EventHandler<MonitorDataEventArgs>? DataUpdated;

    public SystemMonitorService()
    {
        _cpuMonitor = new CpuMonitor();
        _memoryMonitor = new MemoryMonitor();
        _gpuMonitor = new GpuMonitor();
        _diskMonitor = new DiskMonitor();
        _networkMonitor = new NetworkMonitor();
    }

    /// <summary>
    /// 启动监控
    /// </summary>
    public void Start()
    {
        if (_isRunning) return;

        _isRunning = true;
        
        // 每1秒更新一次数据
        _updateTimer = new System.Threading.Timer(UpdateData, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 停止监控
    /// </summary>
    public void Stop()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _updateTimer?.Dispose();
        _updateTimer = null;
    }

    /// <summary>
    /// 更新监控数据
    /// </summary>
    private void UpdateData(object? state)
    {
        if (!_isRunning) return;

        try
        {
            var data = new MonitorData
            {
                CpuUsage = _cpuMonitor.GetUsage(),
                MemoryUsage = _memoryMonitor.GetUsage(),
                GpuUsage = _gpuMonitor.GetUsage(),
                DiskUsage = _diskMonitor.GetUsage(),
                DownloadSpeed = _networkMonitor.GetDownloadSpeed(),
                UploadSpeed = _networkMonitor.GetUploadSpeed()
            };

            DataUpdated?.Invoke(this, new MonitorDataEventArgs(data));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating monitor data: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Stop();
            _cpuMonitor?.Dispose();
            _memoryMonitor?.Dispose();
            _gpuMonitor?.Dispose();
            _networkMonitor?.Dispose();
            _disposed = true;
        }
    }
}

