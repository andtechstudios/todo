using Andtech.Todo.Console;
using System;
using System.Threading;
using System.Threading.Tasks;

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

