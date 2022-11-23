using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

			var parents = new Stack<string>();
			parents.Push(string.Empty);

			var text = File.ReadAllText(path);
			foreach (var line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
			{
				var leadingWhitespaceMatch = Regex.Match(line, @"^\s+");
				var leadingWhitespace = leadingWhitespaceMatch.Success ? leadingWhitespaceMatch.Value : string.Empty;

				if (leadingWhitespace.Length > parents.Peek().Length)
				{
					parents.Push(leadingWhitespace);
				}
				else if (leadingWhitespace.Length < parents.Peek().Length)
				{
					parents.Pop();

					if (leadingWhitespace.Length > parents.Peek().Length)
					{
						parents.Push(leadingWhitespace);
					}
				}

				var task = TodoTask.Parse(line);
				task.Level = parents.Count - 1;

				list.Tasks.Add(task);
			}

			return list;
		}
	}
}
