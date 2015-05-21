using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gem.Render
{
	public class DebuggingContext
	{
		public Action<String> LogMessage = null;
		public int LogLevel = 0;
		public Gem.Render.IRenderable Visualization = null;

		public void Log(int level, String msg)
		{
			if (level <= LogLevel && LogMessage != null) LogMessage(msg);
		}

		public void Log(String msg)
		{
			Log(0, msg);
		}
	}	
}
