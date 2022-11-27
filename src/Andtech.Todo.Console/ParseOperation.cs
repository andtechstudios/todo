using Andtech.Common;
using CommandLine;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class ParseOperation
	{

		[Verb("parse", Hidden = true)]
		public class Options : BaseOptions
		{
			[Value(0, Required = true)]
			public string Expression { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			Log.WriteLine(TodoTask.ContentRegex.ToString(), Verbosity.diagnostic);
			Log.WriteLine(TodoTask.BodyRegex.ToString(), Verbosity.diagnostic);
			Log.WriteLine(TodoTask.MetadataRegex.ToString(), Verbosity.diagnostic);

			var task = TodoTask.Parse(options.Expression);
			Log.WriteLine($"       Title: {task.Title}");
			Log.WriteLine($" Description: {task.Description}");
			Log.WriteLine($"      Status: {task.IsCompleted}");
			Log.WriteLine($"        Tags: {string.Join(", ", task.Tags)}");
			Log.WriteLine($"    Due Date: {task.DueDate}");
			Log.WriteLine($"   Assignees: {string.Join(", ", task.Assignees)}");
		}
	}
}
