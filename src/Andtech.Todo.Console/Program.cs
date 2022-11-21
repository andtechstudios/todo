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
using static Crayon.Output;

public class LinearWindow
{
	public int CursorLineNumber { get; set; }
	public int WindowLineNumber { get; set; }
	public int Capacity { get; set; }
	public int Height { get; set; }
	
	public LinearWindow(int capacity, int height)
	{
		Capacity = capacity;
		Height = height;
	}

	public void Rebuild()
	{
		if (CursorLineNumber < 0)
		{
			CursorLineNumber = 0;
		}
		if (CursorLineNumber > Capacity - 1)
		{
			CursorLineNumber = Capacity - 1;
		}

		if (CursorLineNumber < WindowLineNumber)
		{
			WindowLineNumber = CursorLineNumber;
		}

		if (CursorLineNumber >= WindowLineNumber + Height)
		{
			WindowLineNumber = CursorLineNumber - Height + 1;
		}
	}
}

public class RawScreen
{

	public void MarkDirty()
	{
		Rebuild();
	}

	public void Rebuild()
	{
		Session.Instance.Window.Rebuild();

		var taskList = Session.Instance.Lists[0];
		var tree = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			tree.Add(element);
		}

		var message = "";
		for (int visibleLineNumber = 0; visibleLineNumber < Session.Instance.Window.Height; visibleLineNumber++)
		{
			var index = visibleLineNumber + Session.Instance.Window.WindowLineNumber;

			if (index < tree.Count)
			{
				var text = tree[index].Render();
				if (index == Session.Instance.Window.CursorLineNumber)
				{
					text = Blue(text);
				}

				message += text + Environment.NewLine;
			}
		}

		Console.CursorLeft = 0;
		Console.CursorTop = 0;
		Console.Write(message);
		Console.CursorTop = Console.BufferHeight - 1;
		Console.CursorLeft = 0;
		Console.Write($"{Session.Instance.Window.CursorLineNumber} {Session.Instance.Window.WindowLineNumber} ({Session.Instance.Window.Height})");
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

		//await RunAsync(cancellationToken: token);

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
		session.Lists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));

		Session.Instance.Window = new LinearWindow(Session.Instance.Lists[0].Tasks.Count, Console.BufferHeight - 1);
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

		await inputTask;

		void Input_OnSubmit()
		{
			var task = Session.Instance.Lists[0].Tasks[Session.Instance.Window.CursorLineNumber];
			task.Complete = !task.Complete;
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

