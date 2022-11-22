﻿public class LinearWindow
{
	public int CursorLineNumber
	{
		get => cursorLineNumber;
		set
		{
			cursorLineNumber = value;
			Rebuild();
		}
	}
	public int WindowLineNumber { get; set; }
	public int Capacity { get; set; }
	public int Height { get; set; }

	private int cursorLineNumber;
	
	public LinearWindow(int capacity, int height)
	{
		Capacity = capacity;
		Height = height;
	}

	public void Rebuild()
	{
		if (CursorLineNumber < 0)
		{
			CursorLineNumber = 0;
		}
		if (CursorLineNumber > Capacity - 1)
		{
			CursorLineNumber = Capacity - 1;
		}

		if (CursorLineNumber < WindowLineNumber)
		{
			WindowLineNumber = CursorLineNumber;
		}

		if (CursorLineNumber >= WindowLineNumber + Height)
		{
			WindowLineNumber = CursorLineNumber - Height + 1;
		}
	}
}
