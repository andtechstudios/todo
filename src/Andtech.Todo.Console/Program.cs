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

		var token = cts.Token;

		await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => RunAsync(cancellationToken: token));

		//await RunAsync(cancellationToken: token);

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
		input.OnLineDown += Input_OnLineDown;
		input.OnLineUp += Input_OnLineUp;
		input.OnSubmit += Input_OnSubmit;
		var inputTask = input.RunAsync(cancellationToken: cancellationToken);

		await Task.WhenAny(inputTask, guiTask);

		void Input_OnSubmit()
		{
			var task = Session.Instance.Lists[0].Tasks[screen.CursorLineNumber];
			task.Complete = !task.Complete;
			screen.MarkDirty();
		}

		void Input_OnLineUp()
		{
			screen.CursorLineNumber--;
			screen.MarkDirty();
		}

		void Input_OnLineDown()
		{
			screen.CursorLineNumber++;
			screen.MarkDirty();
		}

		void Input_OnQuit()
		{
			cts?.Cancel();
		}
	}
}

