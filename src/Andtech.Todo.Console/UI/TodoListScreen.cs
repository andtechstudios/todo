using Andtech.Todo.Console;
using Spectre.Console;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class TodoListScreen
{
	private Layout layout;
	private Table table;
	private List<TaskRenderer> elements;

	static int lineNumber = 0;

	public async Task DrawGUIAsync(CancellationToken cancellationToken = default)
	{
		table = GetTodoListTable();
		layout = GetLayout();

		await AnsiConsole.Live(layout)
			.AutoClear(true)
			.StartAsync(ctx => RefreshAsync(ctx, cancellationToken: cancellationToken));
	}

	Layout GetLayout()
	{
		var layout = new Layout();

		layout.SplitRows(
			new Layout("Ribbon").Size(4),
			new Layout("Body"),
			new Layout("Bottom").Size(4)
		);

		layout["Ribbon"].Update(
			new Panel(
					new Text("General | Work | School"))
				//.NoBorder()
				.Padding(0, 0)
				.Expand());

		layout["Body"].Update(
			new Panel(
					table)
				//.NoBorder()
				.Padding(0, 0)
				.Expand());

		layout["Bottom"].Update(
			new Panel(
					new Text("Prompt\nConsole"))
				//.NoBorder()
				.Padding(0, 0)
				.Expand());

		return layout;
	}

	Table GetTodoListTable()
	{
		var taskList = Session.Instance.Lists[0];
		elements = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			elements.Add(element);
		}

		int n = elements.Count;
		var table = new Table()
			.NoBorder()
			.HideHeaders();
		table.AddColumn("Content");
		foreach (var renderer in elements)
		{
			var text = renderer.Render();

			table.AddRow(text);
		}
		table.Expand();

		return table;
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
			Rebuild();

			await Task.Delay(500, cancellationToken: cancellationToken);
		}
	}
}
