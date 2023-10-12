using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Socket.Quobject.EngineIoClientDotNet.Modules
{
	public class LogManager
	{
		private static readonly LogManager EmptyLogger = new LogManager(null);
		public static bool Enabled = false;
		private const string myFileName = "XunitTrace.txt";
		private string MyType;
		private static StreamWriter file;

		public static void SetupLogManager()
		{
		}

		public static LogManager GetLogger(string type)
		{
			return new LogManager(type);
		}

		public static LogManager GetLogger(Type type)
		{
			return LogManager.GetLogger(type.ToString());
		}

		public static LogManager GetLogger(MethodBase methodBase)
		{
			return LogManager.GetLogger(string.Format("{0}#{1}", methodBase.DeclaringType == null ? "" : (object)methodBase.DeclaringType.ToString(), methodBase.Name));
		}

		public LogManager(string type)
		{
			this.MyType = type;
		}

		[Conditional("DEBUG")]
		public void Info(string msg)
		{
			if (!LogManager.Enabled)
				return;
			if (LogManager.file == null)
			{
				LogManager.file = new StreamWriter("XunitTrace.txt", true);
				LogManager.file.AutoFlush = true;
			}
			msg = Global.StripInvalidUnicodeCharacters(msg);


			string str = string.Format("{0} [{3}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), MyType, msg, System.Threading.Thread.CurrentThread.ManagedThreadId);
			LogManager.file.WriteLine(str);
		}

		[Conditional("DEBUG")]
		public void Error(string p, Exception exception)
		{
			this.Info(string.Format("ERROR {0} {1} {2}", p, exception.Message, exception.StackTrace));
			if (exception.InnerException == null)
				return;
			this.Info(string.Format("ERROR exception.InnerException {0} {1} {2}", p, exception.InnerException.Message, exception.InnerException.StackTrace));
		}

		[Conditional("DEBUG")]
		internal void Error(Exception e)
		{
			this.Error("", e);
		}
	}
}
