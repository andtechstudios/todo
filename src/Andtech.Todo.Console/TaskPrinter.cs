using Andtech.Todo;
using static Crayon.Output;

public class TaskPrinter : Printer
{
	public override string Text => text;

	private readonly TodoTask task;
	private string text;

	public TaskPrinter(TodoTask task)
	{
		this.task = task;
	}

	public override void Rebuild(int width)
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
}

