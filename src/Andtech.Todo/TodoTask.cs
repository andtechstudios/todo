using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Andtech.Todo
{
	public class TodoTask
	{
		public bool IsCompleted { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<string> Tags { get; set; }
		public int Level { get; set; }

		private static readonly Regex TaskRegex = new Regex(
			@"(\*|-|\d+\.)?\s*"
			+ @"(\[(?<complete> |x)\])?\s*"
			+ @"(?<title>.+?)\s*"
			+ @"(:?<description>.+?)?\s*"
			+ @""
		);
		private static readonly Regex MetadataRegex = new Regex(@"(\s*(#\w+|@\w+|due:[\d-]+)\s*)*$");

		public static TodoTask Parse(string text)
		{
			var task = new TodoTask();

			var metadataMatch = MetadataRegex.Match(text);
			if (metadataMatch.Success)
			{
				// TODO add metadata
				text = MetadataRegex.Replace(text, string.Empty);
			}

			var contentMatch = TaskRegex.Match(text);
			if (contentMatch.Success)
			{
				var completionCode = contentMatch.Groups["complete"].Value;
				task.IsCompleted = !string.IsNullOrWhiteSpace(completionCode);
				task.Title = contentMatch.Groups["title"].Value;
			}

			return task;
		}
	}
}
