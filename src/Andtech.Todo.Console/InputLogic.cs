using Spectre.Console;
using System;
using System.Threading;
using System.Threading.Tasks;

public class InputLogic
{

	public event Action OnSubmit;
	public event Action OnQuit;
	public event Action OnLineUp;
	public event Action OnLineDown;

	public async Task RunAsync(CancellationToken cancellationToken = default)
	{
		while (true)
		{
			var keyInfo = await AnsiConsole.Console.Input.ReadKeyAsync(true, cancellationToken: cancellationToken);

			if (keyInfo.HasValue)
			{
				if (keyInfo.Value.Modifiers == 0)
				{
					switch (keyInfo.Value.Key)
					{
						case ConsoleKey.DownArrow:
							OnLineUp?.Invoke();
							break;
						case ConsoleKey.UpArrow:
							OnLineDown?.Invoke();
							break;
						case ConsoleKey.Spacebar:
							OnSubmit?.Invoke();
							break;
						case ConsoleKey.Enter:
							break;
					}
				}
				else
				{
					switch (keyInfo.Value.Modifiers)
					{
						case ConsoleModifiers.Control:
							if (keyInfo.Value.Key == ConsoleKey.Q)
							{
								OnQuit?.Invoke();
							}
							break;
					}
				}
			}
		}
	}
}

