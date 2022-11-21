using System.Collections.Generic;

public class PrintList
{
	public List<Printer> Printers => printers;

	private readonly List<Printer> printers = new List<Printer>();

	public void Add(Printer printer)
	{
		printers.Add(printer);
	}
}

