using Andtech.Todo;
using Spectre.Console;

public class TaskRenderer
{
	private readonly TodoTask task;

	public TaskRenderer(TodoTask task)
	{
		this.task = task;
	}

	public void Submit()
	{
		task.Complete = !task.Complete;
	}

	public string Render()
	{
		var symbol = task.Complete ? "☒" : "☐";
		var content = task.Title.EscapeMarkup();
		if (task.Complete)
		{
			content = $"[dim]{content}[/]";
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
