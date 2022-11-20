using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Andtech.Todo
{
	public class TodoTask
	{
		public bool Complete { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<string> Tags { get; set; }

		private static readonly Regex TaskRegex = new Regex(@"^(\*|-)\s*(\[(?<complete> |x)\]\s*)?(?<title>.*)");

		public static TodoTask Parse(string text)
		{
			var regex = TaskRegex.Match(text);

			return new TodoTask()
			{
				Complete = regex.Groups["complete"].Value != " ",
				Title = regex.Groups["title"].Value,
				Tags = new List<string>(),
			};
		}
	}
}
