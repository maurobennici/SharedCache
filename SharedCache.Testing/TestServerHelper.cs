using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace SharedCache.Testing
{
	public static class TestServerHelper
	{
		public static Dictionary<string, string> configFiles = new Dictionary<string, string>();
		//private static string localDir = @"D:\Dev\codeplex\tfs02\SharedCache\SharedCache.Testing\bin\Debug\";
		public static object lockObject = new object();
		public static bool running = false;
		private static Process serverProcess = new Process();
		private static string defaultConfigFileName = @"SharedCache.WinService.exe.config";

		static TestServerHelper()
		{
			configFiles.Add("singleInstance", "singleInstance.SharedCache.WinService.exe.config");
		}

		public static void LoadServerAsConsole(string name)
		{
			string fileName = string.Empty;

			if (configFiles.ContainsKey(name))
			{
				fileName = configFiles[name];
			}
			if (fileName.Length == 0)
				throw new Exception("Configuration File is not available!");

			// todo: rename correct file for tests.
			bool available = false;
			string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "") + Path.DirectorySeparatorChar;
            available = File.Exists(workingDirectory + fileName);
			if (!available)
				return;
            if (File.Exists(workingDirectory + defaultConfigFileName))
                File.Delete(workingDirectory + defaultConfigFileName);

            File.Copy(workingDirectory + fileName, workingDirectory + defaultConfigFileName);

			if (running)
				return;

			running = true;
			
			serverProcess.StartInfo.WorkingDirectory = workingDirectory; //Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "");
			serverProcess.StartInfo.FileName = "SharedCache.WinService.exe";
			serverProcess.StartInfo.UseShellExecute = true;
			serverProcess.StartInfo.Arguments = "/local";
			//serverProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			serverProcess.Start();
		}

		public static void UnLoadServerAsConsole()
		{
            string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "") + Path.DirectorySeparatorChar;

            File.Delete(workingDirectory + defaultConfigFileName);
		    serverProcess.CloseMainWindow();
            serverProcess.Close();
			running = false;
		}
	}
}
