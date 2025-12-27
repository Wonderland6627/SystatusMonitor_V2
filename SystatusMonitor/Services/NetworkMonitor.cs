using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SystatusMonitor.Services;

/// <summary>
/// 网络速度监控器
/// </summary>
public class NetworkMonitor : IDisposable
{
    private long _lastBytesReceived = 0;
    private long _lastBytesSent = 0;
    private DateTime _lastCheckTime = DateTime.Now;
    private readonly object _lockObject = new object();
    private bool _disposed = false;

    public NetworkMonitor()
    {
        InitializeCounters();
    }

    private void InitializeCounters()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    var stats = ni.GetIPStatistics();
                    _lastBytesReceived += stats.BytesReceived;
                    _lastBytesSent += stats.BytesSent;
                }
            }
            _lastCheckTime = DateTime.Now;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing network counters: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取下载速度 (字节/秒)
    /// </summary>
    public long GetDownloadSpeed()
    {
        return GetNetworkSpeed().downloadSpeed;
    }

    /// <summary>
    /// 获取上传速度 (字节/秒)
    /// </summary>
    public long GetUploadSpeed()
    {
        return GetNetworkSpeed().uploadSpeed;
    }

    /// <summary>
    /// 获取网络速度
    /// </summary>
    private (long downloadSpeed, long uploadSpeed) GetNetworkSpeed()
    {
        lock (_lockObject)
        {
            try
            {
                long totalBytesReceived = 0;
                long totalBytesSent = 0;

                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var ni in interfaces)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var stats = ni.GetIPStatistics();
                        totalBytesReceived += stats.BytesReceived;
                        totalBytesSent += stats.BytesSent;
                    }
                }

                var now = DateTime.Now;
                var elapsed = (now - _lastCheckTime).TotalSeconds;

                if (elapsed > 0)
                {
                    var downloadSpeed = (long)((totalBytesReceived - _lastBytesReceived) / elapsed);
                    var uploadSpeed = (long)((totalBytesSent - _lastBytesSent) / elapsed);

                    _lastBytesReceived = totalBytesReceived;
                    _lastBytesSent = totalBytesSent;
                    _lastCheckTime = now;

                    // 确保速度不为负数（处理计数器重置的情况）
                    if (downloadSpeed < 0) downloadSpeed = 0;
                    if (uploadSpeed < 0) uploadSpeed = 0;

                    return (downloadSpeed, uploadSpeed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting network speed: {ex.Message}");
            }

            return (0, 0);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

