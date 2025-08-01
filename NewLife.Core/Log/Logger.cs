﻿using System.ComponentModel;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace NewLife.Log;

/// <summary>日志基类。提供日志的基本实现</summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class Logger : ILog
{
    #region 主方法
    /// <summary>调试日志</summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public virtual void Debug(String format, params Object?[] args) => Write(LogLevel.Debug, format, args);

    /// <summary>信息日志</summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public virtual void Info(String format, params Object?[] args) => Write(LogLevel.Info, format, args);

    /// <summary>警告日志</summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public virtual void Warn(String format, params Object?[] args) => Write(LogLevel.Warn, format, args);

    /// <summary>错误日志</summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public virtual void Error(String format, params Object?[] args) => Write(LogLevel.Error, format, args);

    /// <summary>严重错误日志</summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">格式化参数</param>
    public virtual void Fatal(String format, params Object?[] args) => Write(LogLevel.Fatal, format, args);
    #endregion

    #region 核心方法
    /// <summary>写日志</summary>
    /// <param name="level"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public virtual void Write(LogLevel level, String format, params Object?[] args)
    {
        if (Enable && level >= Level) OnWrite(level, format, args);
    }

    /// <summary>写日志</summary>
    /// <param name="level"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    protected abstract void OnWrite(LogLevel level, String format, params Object?[] args);
    #endregion

    #region 辅助方法
    /// <summary>格式化参数，特殊处理异常和时间</summary>
    /// <param name="format"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    internal protected virtual String Format(String format, Object?[]? args)
    {
        //处理时间的格式化
        if (args != null && args.Length > 0)
        {
            // 特殊处理异常
            if (args.Length == 1 && args[0] is Exception ex && (format.IsNullOrEmpty() || format == "{0}"))
                return ex.GetMessage();

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != null && args[i] is DateTime dt && format.Contains("{" + i + "}"))
                {
                    // 根据时间值的精确度选择不同的格式化输出
                    //var dt = (DateTime)args[i];
                    // 解决系统使用utc时间时，日志文件被跨天
                    dt = dt.AddHours(Setting.Current.UtcIntervalHours);
                    if (dt.Millisecond > 0)
                        args[i] = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    else if (dt.Hour > 0 || dt.Minute > 0 || dt.Second > 0)
                        args[i] = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    else
                        args[i] = dt.ToString("yyyy-MM-dd");
                }
            }
        }
        if (args == null || args.Length <= 0) return format;

        //format = format.Replace("{", "{{").Replace("}", "}}");

        return String.Format(format, args);
    }
    #endregion

    #region 属性
    /// <summary>是否启用日志。默认true</summary>
    public virtual Boolean Enable { get; set; } = true;

    private LogLevel? _Level;
    /// <summary>日志等级，只输出大于等于该级别的日志，默认Info</summary>
    public virtual LogLevel Level
    {
        get
        {
            if (_Level != null) return _Level.Value;

            return Setting.Current.LogLevel;
        }
        set { _Level = value; }
    }
    #endregion

    #region 静态空实现
    /// <summary>空日志实现</summary>
    public static ILog Null { get; } = new NullLogger();

    class NullLogger : Logger
    {
        public override Boolean Enable { get => false; set { } }

        protected override void OnWrite(LogLevel level, String format, params Object?[] args) { }
    }
    #endregion

    #region 日志头
    /// <summary>输出日志头，包含所有环境信息</summary>
    protected static String GetHead()
    {
        var process = System.Diagnostics.Process.GetCurrentProcess();
        var name = String.Empty;
        var ver = Environment.Version + "";
        var target = "";
        var asm = Assembly.GetEntryAssembly();
        if (asm != null)
        {
            if (String.IsNullOrEmpty(name))
            {
                var att = asm.GetCustomAttribute<AssemblyTitleAttribute>();
                if (att != null) name = att.Title;
            }

            if (String.IsNullOrEmpty(name))
            {
                var att = asm.GetCustomAttribute<AssemblyProductAttribute>();
                if (att != null) name = att.Product;
            }

            if (String.IsNullOrEmpty(name))
            {
                var att = asm.GetCustomAttribute<AssemblyDescriptionAttribute>();
                if (att != null) name = att.Description;
            }

            var tar = asm.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>();
            if (tar != null) target = !tar.FrameworkDisplayName.IsNullOrEmpty() ? tar.FrameworkDisplayName : tar.FrameworkName;
        }
#if !NETFRAMEWORK
        target = RuntimeInformation.FrameworkDescription;
#endif

        if (String.IsNullOrEmpty(name))
        {
            try
            {
                name = process.ProcessName;
            }
            catch { }
        }
        var sb = new StringBuilder();
        sb.AppendFormat("#Software: {0}\r\n", name);
        sb.AppendFormat("#ProcessID: {0}{1}\r\n", process.Id, Environment.Is64BitProcess ? " x64" : "");
        sb.AppendFormat("#AppDomain: {0}\r\n", AppDomain.CurrentDomain.FriendlyName);

        var fileName = String.Empty;
        // MonoAndroid无法识别MainModule，致命异常
        try
        {
            fileName = process.MainModule?.FileName;
        }
        catch { }
        if (fileName.IsNullOrEmpty() || fileName.EndsWithIgnoreCase("dotnet", "dotnet.exe"))
        {
            try
            {
                fileName = process.StartInfo.FileName;
            }
            catch { }
        }
        if (!fileName.IsNullOrEmpty()) sb.AppendFormat("#FileName: {0}\r\n", fileName);

        // 应用域目录
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        sb.AppendFormat("#BaseDirectory: {0}\r\n", baseDir);

        // 当前目录。如果由别的进程启动，默认的当前目录就是父级进程的当前目录
        var curDir = Environment.CurrentDirectory;
        //if (!curDir.EqualIC(baseDir) && !(curDir + "\\").EqualIC(baseDir))
        if (!baseDir.EqualIgnoreCase(curDir, curDir + "\\", curDir + "/"))
            sb.AppendFormat("#CurrentDirectory: {0}\r\n", curDir);

        var basePath = PathHelper.BasePath;
        if (basePath != baseDir)
            sb.AppendFormat("#BasePath: {0}\r\n", basePath);

        // 临时目录
        sb.AppendFormat("#TempPath: {0}\r\n", Path.GetTempPath());

        // 命令行不为空，也不是文件名时，才输出
        // 当使用cmd启动程序时，这里就是用户输入的整个命令行，所以可能包含空格和各种符号
        var line = Environment.CommandLine;
        if (!line.IsNullOrEmpty())
            sb.AppendFormat("#CommandLine: {0}\r\n", line);

        var apptype = "";
        if (Runtime.IsWeb)
            apptype = "Web";
        else if (!Environment.UserInteractive)
            apptype = "Service";
        else if (Runtime.IsConsole)
            apptype = "Console";
        else
            apptype = "WinForm";

        if (Runtime.Container) apptype += "(Container)";

        sb.AppendFormat("#ApplicationType: {0}\r\n", apptype);
        sb.AppendFormat("#CLR: {0}, {1}\r\n", ver, target);

        var os = "";
        // 获取丰富的机器信息，需要提注册 MachineInfo.RegisterAsync
        var mi = MachineInfo.Current;
        if (mi != null)
        {
            os = mi.OSName + " " + mi.OSVersion;
        }
        else
        {
            // 特别识别Linux发行版
            os = Environment.OSVersion + "";
            if (Runtime.Linux) os = MachineInfo.GetLinuxName();
        }

        sb.AppendFormat("#OS: {0}, {1}/{2}\r\n", os, Environment.MachineName, Environment.UserName);
        sb.AppendFormat("#CPU: {0}\r\n", Environment.ProcessorCount);
        if (mi != null)
        {
            sb.AppendFormat("#Memory: {0:n0}M/{1:n0}M\r\n", mi.AvailableMemory / 1024 / 1024, mi.Memory / 1024 / 1024);
            sb.AppendFormat("#Processor: {0}\r\n", mi.Processor);
            if (!mi.Product.IsNullOrEmpty()) sb.AppendFormat("#Product: {0} / {1}\r\n", mi.Product, mi.Vendor);
            if (mi.Temperature > 0) sb.AppendFormat("#Temperature: {0}\r\n", mi.Temperature);
        }
        sb.AppendFormat("#GC: IsServerGC={0}, LatencyMode={1}\r\n", GCSettings.IsServerGC, GCSettings.LatencyMode);

        ThreadPool.GetMinThreads(out var minWorker, out var minIO);
        ThreadPool.GetMaxThreads(out var maxWorker, out var maxIO);
        ThreadPool.GetAvailableThreads(out var avaWorker, out var avaIO);
        sb.AppendFormat("#ThreadPool: Min={0}/{1}, Max={2}/{3}, Available={4}/{5}\r\n", minWorker, minIO, maxWorker, maxIO, avaWorker, avaIO);

        var set = Setting.Current;
        sb.AppendFormat("#SystemStarted: {0}\r\n", TimeSpan.FromMilliseconds(Runtime.TickCount64));
        sb.AppendFormat("#Date: {0:yyyy-MM-dd}\r\n", DateTime.Now.AddHours(set.UtcIntervalHours));
        sb.AppendFormat("#详解：{0}\r\n", "https://newlifex.com/core/log");
        sb.AppendFormat("#字段: 时间 线程ID 线程池Y/网页W/普通N/定时T 线程名/任务ID 消息内容\r\n");
        //sb.AppendFormat("#Fields: Time ThreadID Kind Name Message\r\n");

        var format = set.LogLineFormat;
        if (format.IsNullOrEmpty()) format = "Time|ThreadId|Kind|Name|Message";
        sb.AppendFormat("#Fields: {0}\r\n", format.Replace('|', ' '));

        return sb.ToString();
    }
    #endregion
}