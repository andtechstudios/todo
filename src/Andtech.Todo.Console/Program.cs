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
		var symbol = task.IsCompleted ? "☒" : "☐";
		var content = task.Title;
		if (task.IsCompleted)
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
		await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => RunAsync(cancellationToken: token));
		cts.Cancel();
		cts.Dispose();

		if (Session.Instance.CanWrite)
		{
			var markdownWriter = new MarkdownWriter();
			foreach (var list in Session.Instance.TodoLists)
			{
				var fileWriter = new StreamWriter(list.Path);
				foreach (var task in list.Tasks)
				{
					fileWriter.WriteLine(markdownWriter.ToString(task));
				}
				fileWriter.Close();
			}
		}
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
		var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		var input = new InputLogic();
		input.Actions.Add(new Command(ConsoleKey.Q, ConsoleModifiers.Control), Input_OnQuit);
		input.Actions.Add(new Command(ConsoleKey.Spacebar), Input_OnSubmit);
		input.Actions.Add(new Command(ConsoleKey.DownArrow), Input_OnLineDown);
		input.Actions.Add(new Command(ConsoleKey.UpArrow), Input_OnLineUp);
		input.Actions.Add(new Command(ConsoleKey.S, ConsoleModifiers.Control), EnableSave);
		input.Actions.Add(new Command(ConsoleKey.DownArrow, ConsoleModifiers.Alt), MoveLineDown);
		input.Actions.Add(new Command(ConsoleKey.UpArrow, ConsoleModifiers.Alt), MoveLineUp);

		// Main loop
		Session.Instance.Screen.Rebuild();

		var inputTask = input.RunAsync(cancellationToken: cts.Token);

		await inputTask;
		
		cts?.Dispose();

		void MoveLineUp()
		{
			var previousLineNumber = Session.Instance.Window.CursorLineNumber;
			var nextLineNumber = --Session.Instance.Window.CursorLineNumber;

			if (previousLineNumber != nextLineNumber)
			{
				Session.Instance.TodoLists[0].Tasks.Swap(previousLineNumber, nextLineNumber);
				Session.Instance.PrintList.Printers.Swap(previousLineNumber, nextLineNumber);
				Session.Instance.Screen.MarkDirty();
			}
		}

		void MoveLineDown()
		{
			var previousLineNumber = Session.Instance.Window.CursorLineNumber;
			var nextLineNumber = ++Session.Instance.Window.CursorLineNumber;

			if (previousLineNumber != nextLineNumber)
			{
				Session.Instance.TodoLists[0].Tasks.Swap(previousLineNumber, nextLineNumber);
				Session.Instance.PrintList.Printers.Swap(previousLineNumber, nextLineNumber);
				Session.Instance.Screen.MarkDirty();
			}
		}

		void EnableSave()
		{
			Session.Instance.CanWrite = true;
		}

		void Input_OnSubmit()
		{
			var task = Session.Instance.TodoLists[0].Tasks[Session.Instance.Window.CursorLineNumber];
			task.IsCompleted = !task.IsCompleted;

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

