using Andtech.Todo;
using Andtech.Todo.Console;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{

    static Table table;

    public static async Task Main(string[] args)
    {
        Session.Instance = new Session()
        {
            ProjectDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "todo"),
        };
        var session = Session.Instance;
        session.Lists.Add(TodoList.Read(session.ProjectDir + "/todo.md"));

        var thread = new Thread(InputLoop);
        thread.Start();

        DoScreen();
    }

    static void InputLoop()
	{
        while (true)
        {
            var keyInfo = AnsiConsole.Console.Input.ReadKey(true);
            if (keyInfo.HasValue)
            {
                AnsiConsole.MarkupLine($"{keyInfo.Value.Modifiers} + {keyInfo.Value.Key}");

                if (keyInfo.Value.Modifiers == 0)
				{
					switch (keyInfo.Value.Key)
					{
                        case ConsoleKey.DownArrow:
                            lineNumber++;
                            break;
                        case ConsoleKey.UpArrow:
                            lineNumber--;
                            break;
                    }
				}
				else
				{

				}
            }
        }
	}

    static int lineNumber = 0;

    static void DoScreen()
    {
        AnsiConsole.AlternateScreen(MainScreen);
    }

    static void MainScreen()
    {
        GoAsync();
    }

    static async Task GoAsync()
    {
        AnsiConsole.Write(new Rule("[white]General[/]"));

        var table = new Table().Expand().BorderColor(Color.Grey);
        table.AddColumn("[yellow]Content[/]");

        int n = 8;

        await AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Bottom)
            .StartAsync(async ctx =>
            {
                var taskList = Session.Instance.Lists[0];
                foreach (var item in taskList.Tasks)
                {
                    var symbol = item.Complete ? "✓" : " ";
                    var text = string.Join(" ",
                        $"[{symbol}]",
                        item.Title,
                        item.Description
                    );
                    var markup = $"• {text}".EscapeMarkup();

                    table.AddRow(markup);
                }

                // Continously update the table
                while (true)
                {
                    // Refresh and wait for a while
                    ctx.Refresh();
                    await Task.Delay(400);
                }
            });
    }
}

