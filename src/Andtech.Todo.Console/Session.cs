using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class Cache
	{
		public string LastAccessedTodoFile { get; set; }
	}

	internal class Session
	{
		public string ProjectPath { get; set; }
		public TodoList TodoList { get; set; }
		public bool CanWrite { get; set; }
		public string Log { get; set; }
		public Cache Cache { get; set; }
		public string CachePath { get; set; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".todo");

		public static Session Instance { get; set; }

		public void RememberTodoFilePath(string path)
		{
			Cache.LastAccessedTodoFile = path;

			var json = JsonSerializer.Serialize(Cache);
			File.WriteAllText(CachePath, json);
		}
	}
}
