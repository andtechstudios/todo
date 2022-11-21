using Andtech.Common;
using Andtech.Todo;
using Andtech.Todo.Console;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Crayon.Output;

public class TaskPrinter : Printer
{
	public override string Text => text;

	private readonly TodoTask task;
	private string text;

	public TaskPrinter(TodoTask task)
	{
		this.task = task;
	}

	public override void Rebuild(int width)
	{
		var symbol = task.Complete ? "☒" : "☐";
		var content = task.Title;
		if (task.Complete)
		{
			content = Dim(content);
		}
		text = string.Join(" ",
			symbol,
			content,
			task.Description
			);
	}
}

public partial class Program
{
	private static CancellationTokenSource cts;

	public static async Task Main(string[] args)
	{
		Init();

		cts = new CancellationTokenSource();

		var token = cts.Token;

		//await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => RunAsync(cancellationToken: token));

		await RunAsync(cancellationToken: token);

		Console.Clear();
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
		session.TodoLists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));

		Session.Instance.PrintList = new PrintList();
		foreach (var task in session.TodoLists[0].Tasks)
		{
			var printer = new TaskPrinter(task);
			printer.Rebuild(Console.BufferWidth);
			Session.Instance.PrintList.Add(printer);
		}

		Session.Instance.Window = new LinearWindow(Session.Instance.TodoLists[0].Tasks.Count, Console.BufferHeight - 2);
		Session.Instance.Screen = new RawScreen();
	}

	static async Task RunAsync(CancellationToken cancellationToken)
	{
		var input = new InputLogic();
		input.OnQuit += Input_OnQuit;
		input.OnLineDown += Input_OnLineDown;
		input.OnLineUp += Input_OnLineUp;
		input.OnSubmit += Input_OnSubmit;
		var inputTask = input.RunAsync(cancellationToken: cancellationToken);

		Session.Instance.Screen.Rebuild();

		await inputTask;

		void Input_OnSubmit()
		{
			var task = Session.Instance.TodoLists[0].Tasks[Session.Instance.Window.CursorLineNumber];
			task.Complete = !task.Complete;

			Session.Instance.PrintList.Printers[Session.Instance.Window.CursorLineNumber].Rebuild(Console.LargestWindowWidth);

			Session.Instance.Screen.MarkDirty();
		}

		void Input_OnLineUp()
		{
			Session.Instance.Window.CursorLineNumber--;
			Session.Instance.Screen.MarkDirty();
		}

		void Input_OnLineDown()
		{
			Session.Instance.Window.CursorLineNumber++;
			Session.Instance.Screen.MarkDirty();
		}

		void Input_OnQuit()
		{
			cts?.Cancel();
		}
	}
}

