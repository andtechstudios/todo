using Andtech.Todo;
using Spectre.Console;
using static Crayon.Output;

public class TaskRenderer
{
	private readonly TodoTask task;

	public TaskRenderer(TodoTask task)
	{
		this.task = task;
	}

	public void Submit()
	{
		task.IsCompleted = !task.IsCompleted;
	}

	public string Render()
	{
		var symbol = task.IsCompleted ? "☒" : "☐";
		var content = task.Title;
		if (task.IsCompleted)
		{
			content = Dim(content);
		}
		var text = string.Join(" ",
			symbol,
			content,
			task.Description
			);
		var markup = text;

		return markup;
	}
}
