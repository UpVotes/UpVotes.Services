using log4net;
using System;

namespace UpVotes.DataModel.Utility
{
    public class Logger
    {
        private static volatile Logger _instance;
        private static object _syncRoot = new object();
        private static ILog _logger = null;
        private object _lock = new object();

        private Logger()
        {
            if (_logger == null)
            {
                _logger = LogManager.GetLogger(nameof(Logger));
            }
        }

        /// <summary>
        /// Log4Net wrapper initialization
        /// </summary>
        /// <returns></returns>
        public static Logger Instance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
            }

            return _instance;
        }

        /// <summary>
        /// Write Fatal
        /// </summary>
        /// <param name="exception">Exception</param>
        public void WriteFatal(Exception exception)
        {
            _logger.Fatal("Fatal  logging", exception);
        }

        /// <summary>
        /// Write Exception
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        public void WriteError(string message, Exception exception)
        {
            lock (_lock)
            {
                _logger.Error(string.Format("{0} - {1} ", "Error logging", message), exception);
            }
        }

        /// <summary>
        /// Write Info
        /// </summary>
        /// <param name="message">Message</param>
        public void WriteInfo(string message)
        {
            lock (_lock)
            {
                _logger.Info(message);
            }
        }
    }
}
