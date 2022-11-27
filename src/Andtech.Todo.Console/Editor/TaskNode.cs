using Andtech.Todo;
using Andtech.Todo.Console.Editor;
using System;
using System.Linq;
using static Crayon.Output;

public class TaskNode : IEditorNode, IIndentable
{
	public string Text => text;

	private readonly TodoTask task;
	private string text;

	public TaskNode(TodoTask task)
	{
		this.task = task;
	}

	public void Rebuild(int width)
	{
		var symbol = task.IsCompleted ? "⦿" : "◯";
		var content = task.Title;
		var indentation = string.Join(string.Empty, Enumerable.Repeat("  ", task.Level));

		text = string.Join(" ",
			indentation,
			symbol,
			content,
			task.Description
			);

		if (text.Length > width - 1)
		{
			text = text.Substring(0, width - 1) + "…";
		}
		text += string.Join(string.Empty, Enumerable.Repeat(" ", width - text.Length));
	}

	void IEditorNode.Submit()
	{
		task.IsCompleted = !task.IsCompleted;
		Rebuild(Console.LargestWindowWidth);
	}

	void IIndentable.IncreaseLevel()
	{
		task.Level++;
		Rebuild(Console.LargestWindowWidth);
	}

	void IIndentable.DecreaseLevel()
	{
		task.Level--;
		task.Level = Math.Max(0, task.Level);
		Rebuild(Console.LargestWindowWidth);
	}
}

