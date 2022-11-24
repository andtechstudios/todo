using Andtech.Common;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class LintOperation
	{

		[Verb("lint")]
		public class Options : BaseOptions
		{

		}

		public static async Task OnParseAsync(Options options)
		{
			var list = TodoList.Read(Session.Instance.ProjectPath);
			foreach (var task in list.Tasks)
			{
				var indentation = string.Join(string.Empty, Enumerable.Repeat("  ", task.Level));
				Log.WriteLine(indentation + task.Title);
			}
		}
	}
}
