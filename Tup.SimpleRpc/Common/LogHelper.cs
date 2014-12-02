using System;

using log4net;

namespace Tup.SimpleRpc.Common
{
    /// <summary>
    /// 日志模块类(Log4Net 封装)
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static class LogHelper
    {
        private const string LOG_REPOSITORY = "Default"; // this should likely be set in the web config.
        private readonly static ILog m_log = log4net.LogManager.GetLogger(typeof(object));
        /// <summary>
        /// 初始化日志系统
        /// 在系统运行开始初始化
        /// Global.asax Application_Start内
        /// </summary>
        static LogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        #region LogError
        /// <summary>
        /// WriteErrLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogError(string msg)
        {
            m_log.Error(msg);
        }
        /// <summary>
        /// WriteErrLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogError(string format, params object[] para)
        {
            m_log.ErrorFormat(format, para);
        }
        #endregion

        #region LogDebug
        /// <summary>
        /// WriteDebugLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogDebug(string msg)
        {
            m_log.Debug(msg);
        }
        /// <summary>
        /// WriteDebugLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogDebug(string format, params object[] para)
        {
            m_log.DebugFormat(format, para);
        }
        #endregion

        #region LogWarn
        /// <summary>
        /// WriteWarnLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogWarn(string msg)
        {
            m_log.Warn(msg);
        }
        /// <summary>
        /// WriteWarnLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogWarn(string format, params object[] para)
        {
            m_log.WarnFormat(format, para);
        }
        #endregion

        #region LogInfo
        /// <summary>
        /// WriteInfoLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogInfo(string msg)
        {
            m_log.Info(msg);
        }
        /// <summary>
        /// WriteInfoLog
        /// </summary>
        /// <param name="msg"></param>
        public static void LogInfo(string format, params object[] para)
        {
            m_log.InfoFormat(format, para);
        }
        #endregion
    }
}
