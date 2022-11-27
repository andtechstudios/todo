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
		public static readonly Regex MetadataRegex = new Regex(@$"(\s+({TagExpression}|{DueExpression}|{AssigneeExpression}))+$");
		public static readonly Regex BodySplitRegex = new Regex(@"(?<!(\[|\()[^[\\(]]*):(?![^[\]\)]*(\]|\)))");

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

			var tokens = BodySplitRegex.Split(contentMatch.Groups["body"].Value, 2);
			task.Title = tokens[0];
			task.Description = tokens.Length > 1 ? tokens[1].Trim() : string.Empty;

			return task;
		}
	}
}
