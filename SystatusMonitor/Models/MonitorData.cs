namespace SystatusMonitor.Models;

/// <summary>
/// 系统监控数据模型
/// </summary>
public class MonitorData
{
    /// <summary>
    /// CPU使用率 (0-100)
    /// </summary>
    public float CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率 (0-100)
    /// </summary>
    public float MemoryUsage { get; set; }

    /// <summary>
    /// GPU使用率 (0-100)
    /// </summary>
    public float GpuUsage { get; set; }

    /// <summary>
    /// 硬盘使用率 (0-100)
    /// </summary>
    public float DiskUsage { get; set; }

    /// <summary>
    /// 下载速度 (字节/秒)
    /// </summary>
    public long DownloadSpeed { get; set; }

    /// <summary>
    /// 上传速度 (字节/秒)
    /// </summary>
    public long UploadSpeed { get; set; }
}

