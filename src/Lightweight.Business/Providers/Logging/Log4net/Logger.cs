using System;
using log4net;

namespace Lightweight.Business.Providers.Logging.Log4net
{
    public class Logger
    {
        private static object configured = false;
        private readonly ILog log;

        private static void Configurelog4net()
        {
            lock (configured)
            {
                if (!(bool)configured)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    configured = true;
                }
            }
        }

        public static Logger GetInstance(Type loggerType)
        {
            Configurelog4net();
            return new Logger(LogManager.GetLogger(loggerType));
        }

        public static Logger GetInstance(string loggerName)
        {
            Configurelog4net();
            return new Logger(LogManager.GetLogger(loggerName));
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }

        public void Error(Exception ex)
        {
            log.Error(ex.Message, ex);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public Logger(ILog log)
        {
            this.log = log;
        }
    }
}
