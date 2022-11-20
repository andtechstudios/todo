using Andtech.Todo.Console;
using Spectre.Console;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Program;
using System.Xml.Linq;

public class TodoListScreen
{
	private Table table;
	private List<TaskRenderer> elements;

	static int lineNumber = 0;

	public async Task DrawGUIAsync(CancellationToken cancellationToken = default)
	{
		var layout = new Layout();

		AnsiConsole.Write(new Rule("[white]General[/]"));

		var taskList = Session.Instance.Lists[0];
		elements = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			elements.Add(element);
		}

		int n = elements.Count;
		table = new Table()
			.Expand()
			.NoBorder()
			.HideHeaders();
		table.AddColumn("Content");
		foreach (var renderer in elements)
		{
			var text = renderer.Render();

			table.AddRow(text);
		}

		await AnsiConsole.Live(table)
			.AutoClear(true)
			.Overflow(VerticalOverflow.Ellipsis)
			.Cropping(VerticalOverflowCropping.Bottom)
			.StartAsync(ctx => RefreshAsync(ctx, cancellationToken: cancellationToken));
	}

	void Rebuild()
	{
		for (int i = 0; i < table.Rows.Count; i++)
		{
			var row = i + 0;

			var text = elements[row].Render();
			if (row == lineNumber)
			{
				text = $"[blue]{text}[/]";
			}

			table.Rows.Update(row, 0, new Markup(text));
		}
	}

	async Task RefreshAsync(LiveDisplayContext context, CancellationToken cancellationToken = default)
	{
		// Continously update the table
		while (true)
		{
			// Refresh and wait for a while
			context.Refresh();
			await Task.Delay(16, cancellationToken: cancellationToken);

			Rebuild();
		}
	}

}

