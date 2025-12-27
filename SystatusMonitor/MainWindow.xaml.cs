using System;
using System.Windows;
using System.Windows.Input;
using SystatusMonitor.Services;
using SystatusMonitor.ViewModels;

namespace SystatusMonitor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SystemMonitorService _monitorService;
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        // 初始化ViewModel
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        
        // 初始化监控服务
        _monitorService = new SystemMonitorService();
        _monitorService.DataUpdated += OnMonitorDataUpdated;
        
        // 设置窗口位置到右下角
        SetWindowPositionToBottomRight();
        
        // 启动监控
        _monitorService.Start();
    }

    /// <summary>
    /// 设置窗口位置到屏幕右下角
    /// </summary>
    private void SetWindowPositionToBottomRight()
    {
        var workingArea = SystemParameters.WorkArea;
        Left = workingArea.Right - Width - 20;
        Top = workingArea.Bottom - Height - 20;
    }

    /// <summary>
    /// 处理监控数据更新
    /// </summary>
    private void OnMonitorDataUpdated(object? sender, MonitorDataEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            _viewModel.UpdateData(e.Data);
        });
    }

    /// <summary>
    /// 窗口拖动处理
    /// </summary>
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    /// <summary>
    /// 窗口关闭时清理资源
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        _monitorService?.Stop();
        _monitorService?.Dispose();
        base.OnClosed(e);
    }
}