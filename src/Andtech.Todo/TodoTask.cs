﻿using System.Collections.Generic;
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

		private static readonly Regex TaskRegex = new Regex(@"^\s*(\*|-)\s*(\[(?<complete> |x)\]\s*)?(?<title>.*)\s*$");

		public static TodoTask Parse(string text)
		{
			var regex = TaskRegex.Match(text);

			return new TodoTask()
			{
				IsCompleted = regex.Groups["complete"].Value != " ",
				Title = regex.Groups["title"].Value,
				Tags = new List<string>(),
			};
		}
	}
}
