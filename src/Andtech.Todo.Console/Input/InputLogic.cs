using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class InputLogic
{
	public Dictionary<Command, Action> Actions => actions;
	private readonly Dictionary<Command, Action> actions = new Dictionary<Command, Action>(new Command.Comparer());

	public async Task RunAsync(CancellationToken cancellationToken = default)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			var keyInfo = await AnsiConsole.Console.Input.ReadKeyAsync(true, cancellationToken: cancellationToken);

			if (keyInfo.HasValue)
			{
				var command = new Command(keyInfo.Value.Key, keyInfo.Value.Modifiers);

				if (Actions.TryGetValue(command, out var action))
				{
					action?.Invoke();
				}
			}
		}
	}
}

