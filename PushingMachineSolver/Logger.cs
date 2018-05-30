using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushingMachineSolver
{
	class Logger
	{
		private string filename;
		public Logger(string filename)
		{
			this.filename = filename;
			//if it exists, delete it
			if (File.Exists(filename))
				File.Delete(filename);
		}
		public void log(string msg)
		{
			using (var fw = new StreamWriter(filename, true))
			{
				//msg = msg.Replace('\n', ' ').Replace('\r', ' ');
				fw.WriteLine(msg);
				Console.WriteLine(msg);
			}
		}
	}
}
