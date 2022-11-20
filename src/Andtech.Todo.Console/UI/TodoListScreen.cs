using Andtech.Common;
using Andtech.Todo.Console;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class TodoListScreen
{
	private Layout layout;
	private Table table;
	private List<TaskRenderer> elements;

	public int LineNumber { get; set; }
	public int WindowLineNumberBegin { get; set; }

	LiveDisplayContext context;

	public async Task DrawGUIAsync(CancellationToken cancellationToken = default)
	{
		try
		{
		table = GetTodoListTable();
		layout = GetLayout();

			await AnsiConsole.Live(layout)
				.AutoClear(false)
				.Cropping(VerticalOverflowCropping.Bottom)
				.StartAsync(ctx => RefreshAsync(ctx, cancellationToken: cancellationToken));
		}
		catch (Exception ex)
		{
			Log.Error.WriteLine(ex.Message);
			AnsiConsole.Console.Input.ReadKey(true);
		}
	}

	Layout GetLayout()
	{
		var layout = new Layout();

		layout.SplitRows(
			new Layout("Ribbon").Size(4),
			new Layout("Body").Ratio(100),
			new Layout("Bottom").Size(4)
		);

		layout["Ribbon"].Update(
			new Panel(
					new Text("General | Work | School"))
				//.NoBorder()
				.Padding(0, 0)
				.Expand());

		layout["Body"].Update(table);

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

		var height = Console.BufferHeight;
		table.AddColumn("Content");
		foreach (var i in Enumerable.Range(0, height))
		{
			table.AddRow("");
		}
		table.Expand();

		return table;
	}

	void Rebuild()
	{
		var height = Console.BufferHeight;
		for (int i = 0; i < height; i++)
		{
			var index = i + 0;

			if (index < elements.Count)
			{
				var text = elements[index].Render();
				if (index == LineNumber)
				{
					text = $"[blue]{text}[/]";
				}
				table.Rows.Update(i, 0, new Markup(text));
			}
			else
			{
				table.Rows.Update(i, 0, new Text(string.Empty));
			}
		}
	}

	bool isDirty;
	public void MarkDirty()
	{
		isDirty = true;
	}

	async Task RefreshAsync(LiveDisplayContext context, CancellationToken cancellationToken = default)
	{
		this.context = context;

		// Continously update the table
		while (true)
		{
			if (isDirty)
			{
				Rebuild();
				context.Refresh();
			}
			isDirty = false;

			await Task.Delay(10, cancellationToken: cancellationToken);

			// Refresh and wait for a while
		}
	}
}
