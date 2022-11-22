﻿using Andtech.Todo.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using static Crayon.Output;

public class RawScreen
{
	private readonly LinearWindow window;
	private readonly List<IEditorNode> nodes;

	public RawScreen(LinearWindow window, List<IEditorNode> nodes)
	{
		this.window = window;
		this.nodes = nodes;
	}

	public void MarkDirty()
	{
		DrawImmediate();
	}

	private int lastPosition = -1;

	public void DrawImmediate()
	{
		var taskList = Session.Instance.TodoLists[0];
		var tree = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			tree.Add(element);
		}

		var message = "";
		for (int visibleLineNumber = 0; visibleLineNumber < window.Height; visibleLineNumber++)
		{
			var index = visibleLineNumber + window.WindowLineNumber;

			if (index < tree.Count)
			{
				string text = nodes[index].Text;
				if (index == window.CursorLineNumber)
				{
					text = Blue(text);
				}

				message += text + Environment.NewLine;
			}
		}

		Console.CursorLeft = 0;
		Console.CursorTop = 0;
		Console.Write(message);
		Console.CursorVisible = false;
		Console.CursorTop = Console.BufferHeight - 1;
		Console.CursorLeft = 0;
		Console.Write($"Ln {window.CursorLineNumber + 1}");
		Console.SetCursorPosition(0, window.CursorLineNumber - window.WindowLineNumber);
	}
}

