using Andtech.Todo;
using Andtech.Todo.Console;
using Andtech.Todo.Console.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


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

	public void Submit()
	{
		nodes[Window.CursorLineNumber].Submit();
		Screen.MarkDirty();
	}

	public void IncreaseLevel() => SetLevel(isIncrease: true);

	public void DecreaseLevel() => SetLevel(isIncrease: false);

	void SetLevel(bool isIncrease)
	{
		var offset = isIncrease ? 1 : -1;
		var initialLevel = tasks[Window.CursorLineNumber].Level;
		var parentLevel = Window.IsInBounds(Window.CursorLineNumber - 1) ? tasks[Window.CursorLineNumber - 1].Level : 0;
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

public interface IEditorNode
{
	string Text { get; }

	void Submit();
}

public class EditorSubroutine
{
	private readonly Editor editor;

	public EditorSubroutine()
	{
		editor = new Editor(Session.Instance.TodoLists[0]);
	}

	public async Task RunAsync(CancellationToken cancellationToken)
	{
		var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		var input = new InputLogic();
		input.Actions.Add(new Command(ConsoleKey.Q, ConsoleModifiers.Control), Input_OnQuit);
		input.Actions.Add(new Command(ConsoleKey.Spacebar), Input_OnSubmit);
		input.Actions.Add(new Command(ConsoleKey.DownArrow), Input_OnLineDown);
		input.Actions.Add(new Command(ConsoleKey.UpArrow), Input_OnLineUp);
		input.Actions.Add(new Command(ConsoleKey.S, ConsoleModifiers.Control), EnableSave);
		input.Actions.Add(new Command(ConsoleKey.DownArrow, ConsoleModifiers.Alt), MoveLineDown);
		input.Actions.Add(new Command(ConsoleKey.UpArrow, ConsoleModifiers.Alt), MoveLineUp);
		input.Actions.Add(new Command(ConsoleKey.Tab), editor.IncreaseLevel);
		input.Actions.Add(new Command(ConsoleKey.Tab, ConsoleModifiers.Shift), editor.DecreaseLevel);

		editor.Screen.DrawImmediate();

		// Main loop
		await input.RunAsync(cancellationToken: cts.Token);

		cts?.Dispose();

		// Local functions
		void MoveLineUp() => editor.Swap(editor.Window.CursorLineNumber, editor.Window.CursorLineNumber - 1);

		void MoveLineDown() => editor.Swap(editor.Window.CursorLineNumber, editor.Window.CursorLineNumber + 1);

		void EnableSave()
		{
			Session.Instance.CanWrite = true;
		}

		void Input_OnSubmit()
		{
			editor.Submit();
		}

		void Input_OnLineUp()
		{
			editor.SetCursor(editor.Window.CursorLineNumber - 1);
		}

		void Input_OnLineDown()
		{
			editor.SetCursor(editor.Window.CursorLineNumber + 1);
		}

		void Input_OnQuit()
		{
			cts?.Cancel();
		}
	}
}

