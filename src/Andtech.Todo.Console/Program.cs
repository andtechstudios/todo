using Andtech.Common;
using Andtech.Todo;
using Andtech.Todo.Console;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public partial class Program
{

	private static CancellationTokenSource cts;

	public static async Task Main(string[] args)
	{
		Init();

		cts = new CancellationTokenSource();

		try
		{
			var token = cts.Token;
			await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => RunAsync(cancellationToken: token));
		}
		catch (Exception ex)
		{
			Log.Error.WriteLine(ex.Message);
		}

		cts.Cancel();
		cts.Dispose();
	}

	static void Init()
	{
		Session.Instance = new Session()
		{
			ProjectDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo"),
		};
		var session = Session.Instance;
		session.Lists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));
	}

	static async Task RunAsync(CancellationToken cancellationToken)
	{
		var screen = new TodoListScreen();
		var guiTask = screen.DrawGUIAsync(cancellationToken: cancellationToken);
		var input = new InputLogic();
		input.OnQuit += Input_OnQuit;
		var inputTask = input.RunAsync(cancellationToken: cancellationToken);

		await Task.WhenAny(inputTask, guiTask);
	}

	static void Input_OnQuit()
	{
		cts?.Cancel();
	}
}

