using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.Todo
{
	public class TodoTask
	{
		public bool IsCompleted { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<string> Tags { get; private set; } = new List<string>();
		public List<string> Assignees { get; private set; } = new List<string>();
		public DateTime DueDate { get; set; }

		public int Level { get; set; }

		public static readonly Regex ContentRegex = new Regex(@$"({BulletExpression})?\s*({StatusExpression})?\s*(?<body>.+)");
		public static readonly Regex BodyRegex = new Regex(@"(?<title>[^:]+)(:\s+(?<description>.+))?");
		public static readonly Regex MetadataRegex = new Regex(@$"(\s+({TagExpression}|{DueExpression}|{AssigneeExpression}))+$");

		private const string BulletExpression = @"\*|-|\d+\.";
		private const string StatusExpression= @"\[(?<complete> |x)\]";
		private const string TagExpression = @"#(?<tag>[\w-_]+)";
		private const string DueExpression = @"due:(?<due>[\d-]+)";
		private const string AssigneeExpression = @"@(?<assignee>[\w-_]+)";

		public static TodoTask Parse(string text)
		{
			var task = new TodoTask();

			var metadataMatch = MetadataRegex.Match(text);
			if (metadataMatch.Success)
			{
				task.Tags.AddRange(metadataMatch.Groups["tag"].Captures.Select(x => x.Value));
				task.Assignees.AddRange(metadataMatch.Groups["assignee"].Captures.Select(x => x.Value));
				if (metadataMatch.Groups["due"].Success)
				{
					if (DateTime.TryParse(metadataMatch.Groups["due"].Value, out var due))
					{
						task.DueDate = due;
					}
				}

				text = MetadataRegex.Replace(text, string.Empty);
			}

			var contentMatch = ContentRegex.Match(text);
			var completionCode = contentMatch.Groups["complete"].Value;
			task.IsCompleted = !string.IsNullOrWhiteSpace(completionCode);

			var bodyMatch = BodyRegex.Match(contentMatch.Groups["body"].Value);
			task.Title = bodyMatch.Groups["title"].Value;
			task.Description = bodyMatch.Groups["description"].Value;

			return task;
		}
	}
}
