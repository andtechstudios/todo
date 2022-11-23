using Andtech.Todo;
using Andtech.Todo.Console;
using Andtech.Todo.Console.Editor;
using System;
using System.Collections.Generic;

public class Editor
{
	public List<IEditorNode> Nodes { get; set; }
	public LinearWindow Window { get; set; }
	public RawScreen Screen { get; set; }

	public readonly List<IEditorNode> nodes = new List<IEditorNode>();
	private readonly List<TodoTask> tasks;

	public Editor(TodoList list)
	{
		foreach (var task in list.Tasks)
		{
			var node = new TaskNode(task);
			node.Rebuild(Console.LargestWindowWidth);
			nodes.Add(node);
		}
		tasks = list.Tasks;

		Window = new LinearWindow(() => nodes.Count, Console.BufferHeight - 2);
		Screen = new RawScreen(Window, nodes);
	}

	public bool SetCursor(int lineNumber)
	{
		var previousLineNumber = Window.CursorLineNumber;
		var nextLineNumber = Window.Clamp(lineNumber);

		if (previousLineNumber != nextLineNumber)
		{
			Window.CursorLineNumber = nextLineNumber;
			Screen.MarkDirty();
			return true;
		}

		return false;
	}

	public bool Swap(int a, int b)
	{
		var previousLineNumber = Window.Clamp(a);
		var nextLineNumber = Window.Clamp(b);

		if (previousLineNumber != nextLineNumber)
		{
			Window.CursorLineNumber = nextLineNumber;
			nodes.Swap(previousLineNumber, nextLineNumber);
			Screen.MarkDirty();
			return true;
		}

		return false;
	}

	#region operations
	public void Submit()
	{
		nodes[Window.CursorLineNumber].Submit();
		Screen.MarkDirty();
	}

	public void MoveUp()
	{
		if (Macros.TryFindPreviousSibling(Window.CursorLineNumber, tasks, out var nextLine))
		{
			Swap(Window.CursorLineNumber, nextLine);
		}
	}

	public void MoveDown()
	{
		if (Macros.TryFindNextSibling(Window.CursorLineNumber, tasks, out var nextLine))
		{
			Swap(Window.CursorLineNumber, nextLine);
		}
	}

	public void IncreaseLevel() => SetLevel(isIncrease: true);

	public void DecreaseLevel() => SetLevel(isIncrease: false);
	#endregion

	void SetLevel(bool isIncrease)
	{
		var offset = isIncrease ? 1 : -1;
		var initialLevel = tasks[Window.CursorLineNumber].Level;
		var parentLevel = Window.IsInBounds(Window.CursorLineNumber - 1) ? tasks[Window.CursorLineNumber - 1].Level : -1;
		if (parentLevel > initialLevel)
		{
			parentLevel = initialLevel;
		}
		var desiredLevel = initialLevel + offset;

		if (Math.Abs(desiredLevel - parentLevel) > 1)
		{
			return;
		}

		for (int i = Window.CursorLineNumber; i < nodes.Count; i++)
		{
			var task = tasks[i];
			if (i != Window.CursorLineNumber && task.Level <= initialLevel)
			{
				break;
			}

			if (isIncrease)
			{
				(nodes[i] as IIndentable)?.IncreaseLevel();
			}
			else
			{
				(nodes[i] as IIndentable)?.DecreaseLevel();
			}
		}
		Screen.MarkDirty();
	}
}

