using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace SystatusMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NotifyIcon? _notifyIcon;
    private MainWindow? _mainWindow;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // 创建系统托盘图标
        CreateNotifyIcon();

        // 创建并显示主窗口
        _mainWindow = new MainWindow();
        _mainWindow.Show();
    }

    /// <summary>
    /// 创建系统托盘图标
    /// </summary>
    private void CreateNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application, // 使用系统默认图标，后续可以替换为自定义图标
            Text = "SystatusMonitor - 系统监控",
            Visible = true
        };

        // 创建上下文菜单
        var contextMenu = new ContextMenuStrip();
        
        var showMenuItem = new ToolStripMenuItem("显示窗口");
        showMenuItem.Click += (s, e) => ShowMainWindow();
        contextMenu.Items.Add(showMenuItem);

        var hideMenuItem = new ToolStripMenuItem("隐藏窗口");
        hideMenuItem.Click += (s, e) => HideMainWindow();
        contextMenu.Items.Add(hideMenuItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        var exitMenuItem = new ToolStripMenuItem("退出");
        exitMenuItem.Click += (s, e) => Shutdown();
        contextMenu.Items.Add(exitMenuItem);

        _notifyIcon.ContextMenuStrip = contextMenu;

        // 双击图标显示/隐藏窗口
        _notifyIcon.DoubleClick += (s, e) =>
        {
            if (_mainWindow != null)
            {
                if (_mainWindow.Visibility == Visibility.Visible)
                {
                    HideMainWindow();
                }
                else
                {
                    ShowMainWindow();
                }
            }
        };
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    private void ShowMainWindow()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }
    }

    /// <summary>
    /// 隐藏主窗口
    /// </summary>
    private void HideMainWindow()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Hide();
        }
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        // 清理系统托盘图标
        _notifyIcon?.Dispose();
    }
}
