using System.Collections.Generic;

namespace Andtech.Todo
{
	public class TodoTask
	{
		public bool Complete { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public List<string> Tags { get; set; }

		public static TodoTask Parse(string text)
		{
			return new TodoTask()
			{
				Title = text,
				Tags = new List<string>(),
			};
		}
	}
}
