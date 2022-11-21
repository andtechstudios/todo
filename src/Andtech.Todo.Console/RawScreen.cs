using Andtech.Todo.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using static Crayon.Output;

public class RawScreen
{

	public void MarkDirty()
	{
		Rebuild();
	}

	private int lastPosition = -1;

	public void Rebuild()
	{
		Session.Instance.Window.Rebuild();

		var taskList = Session.Instance.TodoLists[0];
		var tree = new List<TaskRenderer>();
		foreach (var item in taskList.Tasks)
		{
			var element = new TaskRenderer(item);
			tree.Add(element);
		}

		var message = "";
		for (int visibleLineNumber = 0; visibleLineNumber < Session.Instance.Window.Height; visibleLineNumber++)
		{
			var index = visibleLineNumber + Session.Instance.Window.WindowLineNumber;

			if (index < tree.Count)
			{
				string text;
				if (index == Session.Instance.Window.CursorLineNumber)
				{
					text = Session.Instance.PrintList.Printers[index].Text;
				}
				else
				{
					text = Blue(Session.Instance.PrintList.Printers[index].Text);
				}

				//var space = string.Join(string.Empty, Enumerable.Repeat(" ", Console.BufferWidth - text.Length));

				message += text + Environment.NewLine;
			}
		}

		Console.CursorLeft = 0;
		Console.CursorTop = 0;
		Console.Write(message);
		Console.CursorVisible = false;
		Console.CursorTop = Console.BufferHeight - 1;
		Console.CursorLeft = 0;
		Console.Write($"{Session.Instance.Window.CursorLineNumber} {Session.Instance.Window.WindowLineNumber} ({Session.Instance.Window.Height})");
		Console.SetCursorPosition(0, Session.Instance.Window.CursorLineNumber - Session.Instance.Window.WindowLineNumber);
	}
}

