using CommandLine;
using Spectre.Console;
using System.IO;
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
	}
}
