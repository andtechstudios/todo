using Andtech.Todo;
using Andtech.Todo.Console;
using Spectre.Console;
using System;
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

        DoScreen();
    }

    static void DoScreen()
    {
        AnsiConsole.AlternateScreen(MainScreen);
    }

    static void MainScreen()
    {
        // Now we're in another terminal screen buffer
        AnsiConsole.Write(new Rule("[white]Todo[/]"));
        AnsiConsole.Write(new Panel(new Text("Left adjusted").LeftAligned())
            .NoBorder()
            .Padding(0, 0)
            .Expand());
        AnsiConsole.Write(new Panel(new Text("Right adjusted").RightAligned())
            .NoBorder()
            .Expand());

        var taskList = Session.Instance.Lists[0];
        foreach(var item in taskList.Tasks)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{item.Title}[/]");
        }
        
        AnsiConsole.MarkupLine("[grey]\nPress a key to return[/]");

        var keyInfo = AnsiConsole.Console.Input.ReadKey(true);

        AnsiConsole.MarkupLine(keyInfo.Value.Key.ToString());

        Thread.Sleep(1000);
    }

    static void DoTable()
    {


        table = new Table()
            .Border(TableBorder.None);
        table.AddColumn("Item");
        table.AddColumn("Price");
        table.AddRow("Apple", "$100");

        var display = AnsiConsole.Live(table);
        display.Start(Loop);

        Thread.Sleep(1000);
        Console.WriteLine("update");
        table.Rows.Update(0, 0, new Markup("Banana"));
        Thread.Sleep(1000);
    }

    static void Loop(LiveDisplayContext ctx)
    {
        while (true)
        {
            ctx.Refresh();
            Thread.Sleep(23);
        }
    }
}

