using Andtech.Todo;
using Andtech.Todo.Console;
using Andtech.Todo.Console.Editor;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using static Crayon.Output;

public class TaskNode : IEditorNode, IIndentable
{
	public string Text => text;

	private readonly TodoTask task;
	private string text;

	private static readonly Regex LinkRegex = new Regex(@"\[(?<text>.+?)\]\((?<url>.+?)\)");

	public TaskNode(TodoTask task)
	{
		this.task = task;
	}

	public void Rebuild(int width)
	{
		var symbol = task.IsCompleted ? "⦿" : "◯";
		var title = ProcessMarkdown(task.Title);
		var description = ProcessMarkdown(task.Description);

		var content = title + (string.IsNullOrEmpty(description) ? string.Empty : ":");
		var indentation = string.Join(string.Empty, Enumerable.Repeat("  ", task.Level)); 

		string ProcessMarkdown(string text)
		{
			text = LinkRegex.Replace(text, ProcessURL);
			return text;

			string ProcessURL(Match match)
			{
				var text = match.Groups["text"].Value;
				var url = match.Groups["url"].Value;
				return Macros.TerminalURL(text, url);
			}
		}

		text = string.Join(" ",
			indentation,
			symbol,
			content,
			description
		);

		var count = Math.Max(0, width - text.Length);
		text += string.Join(string.Empty, Enumerable.Repeat(" ", count));
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

