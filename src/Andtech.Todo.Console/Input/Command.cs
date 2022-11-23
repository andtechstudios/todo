using System;
using System.Collections.Generic;

public struct Command
{
	private readonly ConsoleKey key;
	private readonly ConsoleModifiers modifiers;

	public Command(ConsoleKey key, ConsoleModifiers modifiers = 0)
	{
		this.key = key;
		this.modifiers = modifiers;
	}

	public class Comparer : IEqualityComparer<Command>
	{

		bool IEqualityComparer<Command>.Equals(Command x, Command y)
		{
			if (!x.key.Equals(y.key))
			{
				return false;
			}

			if (!x.modifiers.Equals(y.modifiers))
			{
				return false;
			}

			return true;
		}

		int IEqualityComparer<Command>.GetHashCode(Command obj)
		{
			return HashCode.Combine(obj.key, obj.modifiers);
		}
	}
}

