using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo
{

	public class TodoList
	{
		public List<TodoTask> Tasks { get; private set; }
		public string Path { get; private set; }

		public static TodoList Read(string path)
		{
			var list = new TodoList()
			{
				Tasks = new List<TodoTask>(),
				Path = path,
			};

			var text = File.ReadAllText(path);
			foreach (var line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
			{
				var task = TodoTask.Parse(line);
				list.Tasks.Add(task);
			}

			return list;
		}
	}
}
