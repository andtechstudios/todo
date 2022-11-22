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
