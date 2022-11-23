using System;

public class LinearWindow
{
	public int CursorLineNumber
	{
		get => cursorLineNumber;
		set
		{
			var oldValue = CursorLineNumber;
			var newValue = Clamp(value);

			if (oldValue != newValue)
			{
				cursorLineNumber = newValue;
				Rebuild();
			}
		}
	}
	public int WindowLineNumber { get; set; }
	public Func<int> GetCapacity { get; set; }
	public int Height { get; set; }

	private int cursorLineNumber;
	
	public LinearWindow(Func<int> getCapacity, int height)
	{
		GetCapacity = getCapacity;
		Height = height;
	}

	public bool IsInBounds(int lineNumber)
	{
		if (lineNumber < 0)
		{
			return false;
		}
		if (lineNumber > GetCapacity() - 1)
		{
			return false;
		}

		return true;
	}

	public int Clamp(int lineNumber)
	{
		if (lineNumber < 0)
		{
			lineNumber = 0;
		}
		if (lineNumber > GetCapacity() - 1)
		{
			lineNumber = GetCapacity() - 1;
		}

		return lineNumber;
	}

	void Rebuild()
	{
		CursorLineNumber = Clamp(CursorLineNumber);

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

