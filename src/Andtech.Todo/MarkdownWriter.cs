using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo
{

	public class MarkdownWriter
	{

		public string ToString(TodoTask task)
		{
			var builder = new StringBuilder();
			var indentation = string.Join(string.Empty, Enumerable.Repeat('\t', task.Level));

			builder.Append(indentation);
			builder.Append("*");
			builder.Append(task.IsCompleted ? " [x]" : " [ ]");
			builder.Append($" {task.Title}");
			if (!string.IsNullOrEmpty(task.Description))
			{
				builder.Append($": {task.Description}");
			}

			return builder.ToString();
		}
	}
}
