using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Update
{
    /// <summary>
    /// Log4Net帮助类
    /// </summary>
    public class Log
    {
        public static readonly log4net.ILog Logdebug = log4net.LogManager.GetLogger("logdebug");
        public static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("loginfo");
        public static readonly log4net.ILog Logwarn = log4net.LogManager.GetLogger("logwarn");
        public static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");
        public static readonly log4net.ILog Logfatal = log4net.LogManager.GetLogger("logfatal");
        public static readonly log4net.ILog LogOracle = log4net.LogManager.GetLogger("WarningDatail_Oracle");


        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        public static void Info(string info)
        {
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(info);
            }
        }

        /// <summary>
        /// DEBUG （调试信息）：记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
        /// </summary>
        /// <param name="info"></param>
        public static void Debug(string info)
        {
            if (Logdebug.IsDebugEnabled)
            {
                Logdebug.Debug(info);
            }
        }

        /// <summary>
        /// WARN（警告）：记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
        /// </summary>
        /// <param name="info"></param>
        public static void Warn(string info)
        {
            if (Logwarn.IsWarnEnabled)
            {
                Logwarn.Warn(info);
            }
        }

        /// <summary>
        /// ERROR（一般错误）：记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void Error(string info, Exception se)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(info, se);
            }
        }

        /// <summary>
        /// 错误记录
        /// </summary>
        /// <param name="info"></param>
        public static void Error(string info)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(info);
            }
        }

        /// <summary>
        /// FATAL（致命错误）：记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="se"></param>
        public static void Fatal(string info, Exception se)
        {
            if (Logfatal.IsFatalEnabled)
            {
                Logfatal.Fatal(info, se);
            }
        }
    }
}