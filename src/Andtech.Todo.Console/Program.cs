﻿using Andtech.Todo;
using Andtech.Todo.Console;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Program
{

	public class TaskRenderer
	{
		private readonly TodoTask task;

		public TaskRenderer(TodoTask task)
		{
			this.task = task;
		}

		public void Submit()
		{
			task.Complete = !task.Complete;
		}

		public string Render()
		{
			var symbol = task.Complete ? "☒" : "☐";
			var text = string.Join(" ",
				symbol,
				task.Title,
				task.Description
				);
			var markup = $"{text}".EscapeMarkup();

			return markup;
		}
	}

	static Table table;

	private static List<TaskRenderer> elements;
	private static Task mainTask;

	public static async Task Main(string[] args)
	{
		Init();

		await SpectreExtensions.AlternateScreenAsync(AnsiConsole.Console, GuiLoopAsync);
	}

	static void Init()
	{
		Session.Instance = new Session()
		{
			ProjectDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo"),
		};
		var session = Session.Instance;
		session.Lists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));
	}

	static async Task InputLoopAsync()
	{
		while (true)
		{
			var keyInfo = await AnsiConsole.Console.Input.ReadKeyAsync(true, cancellationToken: CancellationToken.None);

			if (keyInfo.HasValue)
			{
				//AnsiConsole.MarkupLine($"{keyInfo.Value.Modifiers} + {keyInfo.Value.Key}");

				if (keyInfo.Value.Modifiers == 0)
				{
					switch (keyInfo.Value.Key)
					{
						case ConsoleKey.DownArrow:
							lineNumber++;
							break;
						case ConsoleKey.UpArrow:
							lineNumber--;
							break;
						case ConsoleKey.Spacebar:
							elements[lineNumber].Submit();
							break;
						case ConsoleKey.Enter:
							break;
					}
				}
				else
				{
					switch (keyInfo.Value.Modifiers)
					{
						case ConsoleModifiers.Control:
							if (keyInfo.Value.Key == ConsoleKey.Q)
							{
								return;
							}
							break;
					}
				}
			}
		}
	}

	static int lineNumber = 0;

	static async Task GuiLoopAsync()
	{
		var inputTask = InputLoopAsync();
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

		var liveTask = AnsiConsole.Live(table)
			.AutoClear(true)
			.Overflow(VerticalOverflow.Ellipsis)
			.Cropping(VerticalOverflowCropping.Bottom)
			.StartAsync(RefreshAsync);

		await Task.WhenAny(inputTask, liveTask);

		AnsiConsole.Clear();
	}

	static async Task RefreshAsync(LiveDisplayContext context)
	{
		// Continously update the table
		while (true)
		{
			// Refresh and wait for a while
			context.Refresh();
			await Task.Delay(16);

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
	}
}

