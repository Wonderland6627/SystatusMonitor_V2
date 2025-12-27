# SystatusMonitor

一个轻量级的Windows系统性能监控工具，使用WPF开发，可以实时显示CPU、内存、GPU、硬盘使用率和网络速度。

## 功能特性

- ✅ **浮窗显示**：桌面右下角显示，始终置顶，可拖动调整位置
- ✅ **性能监控**：
  - CPU使用率（进度条+百分比）
  - 内存使用率（进度条+百分比）
  - GPU使用率（进度条+百分比，当前为占位实现）
  - 硬盘使用率（进度条+百分比）
  - 实时网速（下载/上传速度）
- ✅ **系统托盘**：支持最小化到系统托盘，双击图标显示/隐藏窗口

## 技术栈

- **.NET 8** + **WPF**
- **System.Diagnostics.PerformanceCounter** - CPU和内存监控
- **System.Management (WMI)** - 硬件信息查询
- **System.Net.NetworkInformation** - 网络速度监控
- **System.Windows.Forms** - 系统托盘支持

## 项目结构

```
SystatusMonitor/
├── Models/              # 数据模型
│   └── MonitorData.cs
├── ViewModels/          # 视图模型（MVVM）
│   └── MainViewModel.cs
├── Services/            # 监控服务
│   ├── SystemMonitorService.cs  # 主监控服务
│   ├── CpuMonitor.cs
│   ├── MemoryMonitor.cs
│   ├── GpuMonitor.cs
│   ├── DiskMonitor.cs
│   └── NetworkMonitor.cs
├── MainWindow.xaml      # 主窗口UI
└── MainWindow.xaml.cs   # 主窗口逻辑
```

## 开发环境要求

- Visual Studio 2022 或更高版本
- .NET 8 SDK
- Windows 10/11

## 运行项目

```bash
# 编译项目
dotnet build

# 运行项目
dotnet run --project SystatusMonitor/SystatusMonitor.csproj
```

或在Visual Studio中直接按F5运行。

## 已知限制

- **GPU监控**：当前使用WMI实现，无法直接获取GPU使用率，显示为0%。后续可以集成LibreHardwareMonitor或其他GPU监控库。
- **权限要求**：某些性能计数器可能需要管理员权限才能访问。

## 后续改进计划

- [ ] 集成LibreHardwareMonitor实现GPU监控
- [ ] 添加自定义托盘图标
- [ ] 支持配置保存（窗口位置、显示项等）
- [ ] 添加更多监控指标（温度、风扇转速等）
- [ ] 支持多显示器适配
- [ ] 添加图表显示历史数据

## 许可证

MIT License

