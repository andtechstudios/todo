using Andtech.Todo;
using System;
using static Crayon.Output;

public class TaskNode : IEditorNode
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
		text = string.Join(" ",
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
}

