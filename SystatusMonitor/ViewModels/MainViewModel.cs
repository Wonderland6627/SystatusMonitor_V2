using System.ComponentModel;
using System.Runtime.CompilerServices;
using SystatusMonitor.Models;

namespace SystatusMonitor.ViewModels;

/// <summary>
/// 主窗口视图模型
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private float _cpuUsage;
    private float _memoryUsage;
    private float _gpuUsage;
    private float _diskUsage;
    private long _downloadSpeed;
    private long _uploadSpeed;

    public float CpuUsage
    {
        get => _cpuUsage;
        set
        {
            _cpuUsage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CpuUsageText));
        }
    }

    public string CpuUsageText => $"{CpuUsage:F1}%";

    public float MemoryUsage
    {
        get => _memoryUsage;
        set
        {
            _memoryUsage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MemoryUsageText));
        }
    }

    public string MemoryUsageText => $"{MemoryUsage:F1}%";

    public float GpuUsage
    {
        get => _gpuUsage;
        set
        {
            _gpuUsage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(GpuUsageText));
        }
    }

    public string GpuUsageText => $"{GpuUsage:F1}%";

    public float DiskUsage
    {
        get => _diskUsage;
        set
        {
            _diskUsage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DiskUsageText));
        }
    }

    public string DiskUsageText => $"{DiskUsage:F1}%";

    public long DownloadSpeed
    {
        get => _downloadSpeed;
        set
        {
            _downloadSpeed = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NetworkSpeedText));
        }
    }

    public long UploadSpeed
    {
        get => _uploadSpeed;
        set
        {
            _uploadSpeed = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NetworkSpeedText));
        }
    }

    public string NetworkSpeedText
    {
        get
        {
            var down = FormatSpeed(DownloadSpeed);
            var up = FormatSpeed(UploadSpeed);
            return $"↓ {down} / ↑ {up}";
        }
    }

    /// <summary>
    /// 更新监控数据
    /// </summary>
    public void UpdateData(MonitorData data)
    {
        CpuUsage = data.CpuUsage;
        MemoryUsage = data.MemoryUsage;
        GpuUsage = data.GpuUsage;
        DiskUsage = data.DiskUsage;
        DownloadSpeed = data.DownloadSpeed;
        UploadSpeed = data.UploadSpeed;
    }

    /// <summary>
    /// 格式化网速显示
    /// </summary>
    private static string FormatSpeed(long bytesPerSecond)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (bytesPerSecond >= GB)
            return $"{bytesPerSecond / (double)GB:F2} GB/s";
        if (bytesPerSecond >= MB)
            return $"{bytesPerSecond / (double)MB:F2} MB/s";
        if (bytesPerSecond >= KB)
            return $"{bytesPerSecond / (double)KB:F2} KB/s";
        return $"{bytesPerSecond} B/s";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

