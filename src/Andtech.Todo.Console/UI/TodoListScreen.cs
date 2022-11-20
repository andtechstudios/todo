using Andtech.Common;
using Andtech.Todo.Console;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class TodoListScreen
{
	private Layout layout;
	private Table table;
	private List<TaskRenderer> tree;
	private int RibbonHeight => layout["ribbon"]?.Size ?? 0;
	private int PromptHeight => layout["prompt"]?.Size ?? 0;
	private int BodyHeight => Console.LargestWindowHeight - RibbonHeight - PromptHeight;

	public int CursorLineNumber { get; set; }
	public int WindowLineNumber { get; set; }

	LiveDisplayContext context;

	public async Task DrawGUIAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			table = GetTodoListTable();
			layout = GetLayout();

			AnsiConsole.Write(layout);

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
			new Layout("ribbon").Size(4),
			new Layout("body").Ratio(100),
			new Layout("prompt").Size(4)
		);

		layout["ribbon"].Update(
			new Panel(
					new Text("General | Work | School"))
				//.NoBorder()
				.Padding(0, 0)
				.Expand());

		layout["body"].Update(table);

		layout["prompt"].Update(
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
		tree = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			tree.Add(element);
		}

		int n = tree.Count;
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
		RebuildCursor();

		layout["prompt"].Update(new Text($"Line: {CursorLineNumber} (Window: {WindowLineNumber}) (Size: {BodyHeight}) {tree.Count}"));

		for (int visibleLineNumber = 0; visibleLineNumber < BodyHeight; visibleLineNumber++)
		{
			var index = visibleLineNumber + WindowLineNumber;

			if (index < tree.Count)
			{
				var text = tree[index].Render();
				if (index == CursorLineNumber)
				{
					text = $"[blue]{text}[/]";
				}
				table.Rows.Update(visibleLineNumber, 0, new Markup(text));
			}
			else
			{
				table.Rows.Update(visibleLineNumber, 0, new Text($"{visibleLineNumber + 1} of {BodyHeight} of {Console.LargestWindowHeight}"));
			}
		}
	}

	void RebuildCursor()
	{
		if (CursorLineNumber < 0)
		{
			CursorLineNumber = 0;
		}
		if (CursorLineNumber > tree.Count - 1)
		{
			CursorLineNumber = tree.Count - 1;
		}

		if (CursorLineNumber < WindowLineNumber)
		{
			WindowLineNumber = CursorLineNumber;
		}

		if (CursorLineNumber >= WindowLineNumber + BodyHeight)
		{
			WindowLineNumber = CursorLineNumber - BodyHeight + 1;
		}
	}

	bool isDirty;
	public void MarkDirty()
	{
		isDirty = true;
		AnsiConsole.Cursor.Hide();
		Rebuild();
		context.Refresh();
		AnsiConsole.Cursor.Show();
	}

	async Task RefreshAsync(LiveDisplayContext context, CancellationToken cancellationToken = default)
	{
		this.context = context;

		// Continously update the table
		while (true)
		{
			if (isDirty)
			{
				//Rebuild();
				//context.Refresh();
			}
			isDirty = false;

			await Task.Delay(1000, cancellationToken: cancellationToken);

			// Refresh and wait for a while
		}
	}
}
