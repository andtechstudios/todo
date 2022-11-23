using Andtech.Common;
using Andtech.Todo;
using Andtech.Todo.Console;
using CommandLine;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public partial class Program
{

	public static async Task Main(string[] args)
	{
		var result = Parser.Default.ParseArguments<BaseOptions, InteractiveOperation.Options, LintOperation.Options>(args);
		await result.WithParsedAsync<BaseOptions>(OnPreParseAsync);
		await result.WithParsedAsync<InteractiveOperation.Options>(InteractiveOperation.OnParseAsync);
		await result.WithParsedAsync<LintOperation.Options>(LintOperation.OnParseAsync);
	}

	static async Task OnPreParseAsync(BaseOptions options)
	{
		Log.Verbosity = options.Verbose ? Verbosity.verbose : options.Verbosity;
		DryRun.IsDryRun = options.DryRun;

		Session.Instance = new Session()
		{
			ProjectDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo"),
		};
		var session = Session.Instance;
		session.TodoLists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));
	}
}

