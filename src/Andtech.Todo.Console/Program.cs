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
		var subroutine = new EditorSubroutine();
		await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => subroutine.RunAsync(cancellationToken: token));
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
	}
}

