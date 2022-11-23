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
		var symbol = task.IsCompleted ? "☒" : "☐";
		var content = task.Title;
		if (task.IsCompleted)
		{
			content = Dim(content);
		}
		var indentation = string.Join(string.Empty, Enumerable.Repeat("  ", task.Level));

		text = string.Join(" ",
			indentation,
			symbol,
			content,
			task.Description
			);
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

