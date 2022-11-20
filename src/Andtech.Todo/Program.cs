using Spectre.Console;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{

    static Table table;

	public static async Task Main(string[] args)
	{
        table = new Table().Centered();
        table.AddColumn("Item");
        table.AddColumn("Price");
        table.AddRow("Apple", "$100");

        var display = AnsiConsole.Live(table);
        display.Start(ctx =>
            {
                ctx.Refresh();
                Thread.Sleep(23);
            });

        Thread.Sleep(1000);
        Console.WriteLine("update");
        table.Rows.Update(0, 0, new Markup("Banana"));
        Thread.Sleep(1000);
    }
}

