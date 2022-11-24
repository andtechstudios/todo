using CommandLine;
using Spectre.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class InteractiveOperation
	{
		private static CancellationTokenSource cts;

		[Verb("interactive", isDefault: true)]
		public class Options : BaseOptions
		{

		}

		public static async Task OnParseAsync(Options options)
		{
			await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, ChooseAsync);

			cts = new CancellationTokenSource();
			var token = cts.Token;
			var subroutine = new EditorSubroutine(Session.Instance.TodoList);
			await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, () => subroutine.RunAsync(cancellationToken: token));
			cts.Cancel();
			cts.Dispose();

			if (Session.Instance.CanWrite)
			{
				var markdownWriter = new MarkdownWriter();

				var list = Session.Instance.TodoList;
				var fileWriter = new StreamWriter(list.Path);
				foreach (var task in list.Tasks)
				{
					fileWriter.WriteLine(markdownWriter.ToString(task));
				}
				fileWriter.Close();
			}
		}

		public static async Task ChooseAsync()
		{
			if (File.Exists(Session.Instance.ProjectPath))
			{
				Session.Instance.TodoList = TodoList.Read(Session.Instance.ProjectPath);
			}
			else
			{
				var files = Directory.EnumerateFiles(Session.Instance.ProjectPath, "*.md");

				var result = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Choose todo file:")
						.AddChoices(files)
				);

				Session.Instance.TodoList = TodoList.Read(result);
			}
		}
	}
}
