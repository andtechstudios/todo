using Andtech.Todo;
using Andtech.Todo.Console;
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

	public Editor(TodoList list)
	{
		foreach (var task in list.Tasks)
		{
			var node = new TaskNode(task);
			node.Rebuild(Console.LargestWindowWidth);
			nodes.Add(node);
		}

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

		editor.Screen.DrawImmediate();

		// Main loop
		await input.RunAsync(cancellationToken: cts.Token);

		cts?.Dispose();

		void MoveLineUp()
		{
			editor.Swap(editor.Window.CursorLineNumber, editor.Window.CursorLineNumber - 1);
		}

		void MoveLineDown()
		{
			editor.Swap(editor.Window.CursorLineNumber, editor.Window.CursorLineNumber + 1);
		}

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

