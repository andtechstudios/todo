using Andtech.Common;
using Andtech.Todo;
using Andtech.Todo.Console;
using CommandLine;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		var path = string.IsNullOrEmpty(options.Path) ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo") : options.Path;

		Session.Instance = new Session()
		{
			ProjectDir = path,
		};
		var session = Session.Instance;

		IEnumerable<string> todoFilePaths;
		if (File.Exists(path))
		{
			todoFilePaths = Enumerable.Repeat(path, 1);
		}
		else
		{
			todoFilePaths = Directory.EnumerateFiles(path, "*.md");
		}

		foreach (var todoFilePath in todoFilePaths)
		{
			session.TodoLists.Add(TodoList.Read(todoFilePath));
		}
	}
}

