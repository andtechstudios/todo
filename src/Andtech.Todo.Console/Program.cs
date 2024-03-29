﻿using Andtech.Common;
using Andtech.Todo;
using Andtech.Todo.Console;
using CommandLine;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class Program
{

	public static async Task Main(string[] args)
	{
		var result = Parser.Default.ParseArguments<BaseOptions, InteractiveOperation.Options, LintOperation.Options, ParseOperation.Options>(args);
		await result.WithParsedAsync<BaseOptions>(OnPreParseAsync);
		await result.WithParsedAsync<InteractiveOperation.Options>(InteractiveOperation.OnParseAsync);
		await result.WithParsedAsync<LintOperation.Options>(LintOperation.OnParseAsync);
		await result.WithParsedAsync<ParseOperation.Options>(ParseOperation.OnParseAsync);
	}

	static async Task OnPreParseAsync(BaseOptions options)
	{
		Log.Verbosity = options.Verbose ? Verbosity.verbose : options.Verbosity;
		DryRun.IsDryRun = options.DryRun;

		var path = string.IsNullOrEmpty(options.Path) ? Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo") : options.Path;

		Session.Instance = new Session()
		{
			ProjectPath = path,
		};

		try
		{
			var text = File.ReadAllText(Session.Instance.CachePath);
			Session.Instance.Cache = JsonSerializer.Deserialize<Cache>(text);
		}
		catch
		{
			Session.Instance.Cache = new Cache();
		}
	}
}

