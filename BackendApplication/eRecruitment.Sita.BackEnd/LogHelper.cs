using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace eRecruitmentLogger
{
    public static class LogHelper
    {
        #region Settings

        internal static string LogDirectory = @"c:\"; // String.Empty;
        private static object _lockObject = new object();
        private static bool _debug = false;

        #endregion

        #region Events

        public delegate void LineWrittenDelegate(string text, Exception exc);

        public static event LineWrittenDelegate LineWritten;

        private static void OnLineWritten(string text, Exception exc)
        {
            if (LineWritten != null)
                LineWritten(text, exc);
        }

        #endregion

        #region Log Methods

        private static object _syncRoot = new object();

        public static void WriteLineFormat(string text, params object[] parameters)
        {
            WriteLine(String.Format(text, parameters));
        }

        public static void WriteLine(string text)
        {
            WriteLine(text, null);
        }

        public static void WriteLine(string text, Exception exc)
        {
            lock (_syncRoot)
            {
                try
                {
                    string logFileName = DateTime.Now.ToString("yyyyMMdd") + "_log.txt";
                    string timeStamp = DateTime.Now.ToString("HH:mm:ss");
                    string tempLogDirectory = ConfigurationManager.AppSettings["LogDirectory"] ;
                    if (!string.IsNullOrEmpty(tempLogDirectory))
                    {
                        LogDirectory = tempLogDirectory;
                    }

                    if (!System.IO.Directory.Exists(LogDirectory))
                    {
                        System.IO.Directory.CreateDirectory(LogDirectory);
                    }
                    if (_debug)
                        System.Diagnostics.Debug.WriteLine(timeStamp + "\t" + text);

                    lock (_lockObject)
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(LogDirectory, logFileName), true))
                        {
                            sw.WriteLine(timeStamp + "\t" + text);
                            if (exc != null)
                                sw.WriteLine(timeStamp + "\t" + exc.ToString());
                        }
                    }

                    OnLineWritten(text, exc);
                }
                catch
                {
                    // Empty try ... catch, not a good idea to attempt to log a logging failure ;)
                }
            }
        }

        #endregion
    }
}