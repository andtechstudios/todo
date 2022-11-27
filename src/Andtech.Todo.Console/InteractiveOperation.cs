using CommandLine;
using Spectre.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
			await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, AlternateScreen);
		}

		static string GetCachedFilePath()
		{
			if (File.Exists(Session.Instance.Cache.LastAccessedTodoFile))
			{
				return Session.Instance.Cache.LastAccessedTodoFile;
			}

			return null;
		}

		static async Task AlternateScreen()
		{
			var filePath = GetCachedFilePath();
			if (string.IsNullOrEmpty(filePath))
			{
				await LoadFileAsync();
			}
			else
			{
				Session.Instance.TodoList = TodoList.Read(filePath);
			}

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

		public static async Task LoadFileAsync()
		{
			string path;
			if (File.Exists(Session.Instance.ProjectPath))
			{
				path = Session.Instance.ProjectPath;
			}
			else
			{
				var files = Directory.EnumerateFiles(Session.Instance.ProjectPath, "*.md");
				path = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Choose todo file:")
						.AddChoices(files)
				);

			}
			Session.Instance.TodoList = TodoList.Read(path);
			Session.Instance.RememberTodoFilePath(path);
		}
	}
}
