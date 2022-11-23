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

	public bool Swap(int a, int b, bool inverted = false)
	{
		// Make sure 'a' is before 'b'
		if (b < a)
		{
			return Swap(b, a, inverted: true);
		}

		a = Window.Clamp(a);
		b = Window.Clamp(b);
		if (a == b)
		{
			return false;
		}

		var lengthA = Macros.Depth(tasks, a);
		var lengthB = Macros.Depth(tasks, b);
		var rangeLnBegin = Math.Min(a, b);
		var rangeLnEnd = Math.Max(a + lengthA, b + lengthB);
		var rangeLength = rangeLnEnd - rangeLnBegin;
		var midLength = rangeLength - lengthA - lengthB;
		var midLnBegin = rangeLnBegin + lengthA;
		var midLnEnd = midLnBegin + midLength;

		var tempNodes = new LinkedList<IEditorNode>();
		var tempTasks = new LinkedList<TodoTask>();
		for (int i = b; i < b + lengthB; i++)
		{
			tempNodes.AddLast(nodes[i]);
			tempTasks.AddLast(tasks[i]);
		}
		for (int i = midLnBegin; i < midLnEnd; i++)
		{
			tempNodes.AddLast(nodes[i]);
			tempTasks.AddLast(tasks[i]);
		}
		for (int i = a; i < a + lengthA; i++)
		{
			tempNodes.AddLast(nodes[i]);
			tempTasks.AddLast(tasks[i]);
		}

		int index;
		index = 0;
		foreach (var node in tempNodes)
		{
			var ln = rangeLnBegin + index;
			nodes[ln] = node;

			index++;
		}
		index = 0;
		foreach (var task in tempTasks)
		{
			var ln = rangeLnBegin + index;
			tasks[ln] = task;

			index++;
		}

		if (inverted)
		{
			Window.CursorLineNumber = a;
		}
		else
		{
			Window.CursorLineNumber = a + lengthB + midLength;
		}

		Screen.MarkDirty();
		return true;
	}

	#region operations
	public void Submit()
	{
		nodes[Window.CursorLineNumber].Submit();
		Screen.MarkDirty();
	}

	public void MoveUp()
	{
		var ln = Window.CursorLineNumber;
		if (Macros.TryFindPreviousSibling(tasks, ln, out var otherLn))
		{
			Session.Instance.Log = otherLn.ToString();
			Swap(ln, otherLn);
		}
	}

	public void MoveDown()
	{
		var ln = Window.CursorLineNumber;
		if (Macros.TryFindNextSibling(tasks, ln, out var otherLn))
		{
			Swap(ln, otherLn);
		}
	}

	public void IncreaseLevel() => SetLevel(isIncrease: true);

	public void DecreaseLevel() => SetLevel(isIncrease: false);
	#endregion

	void SetLevel(bool isIncrease)
	{
		var ln = Window.CursorLineNumber;
		var offset = isIncrease ? 1 : -1;
		var initialLevel = tasks[ln].Level;
		var parentLevel = Window.IsInBounds(ln - 1) ? tasks[ln - 1].Level : -1;
		if (parentLevel > initialLevel)
		{
			parentLevel = initialLevel;
		}
		var desiredLevel = initialLevel + offset;

		if (Math.Abs(desiredLevel - parentLevel) > 1)
		{
			return;
		}

		foreach (int childLn in Macros.GetChildren(tasks, ln))
		{
			if (isIncrease)
			{
				(nodes[childLn] as IIndentable)?.IncreaseLevel();
			}
			else
			{
				(nodes[childLn] as IIndentable)?.DecreaseLevel();
			}
		}
		if (isIncrease)
		{
			(nodes[ln] as IIndentable)?.IncreaseLevel();
		}
		else
		{
			(nodes[ln] as IIndentable)?.DecreaseLevel();
		}
		Screen.MarkDirty();
	}
}

