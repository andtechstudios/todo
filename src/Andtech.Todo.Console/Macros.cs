using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal static class Macros
	{

		public static string TerminalURL(string caption, string url) => $"\u001B]8;;{url}\a{caption}\u001B]8;;\a";

		public static void Swap<T>(this List<T> list, int a, int b)
		{
			var temp = list[b];
			list[b] = list[a];
			list[a] = temp;
		}

		public static IEnumerable<int> Subtree(IList<TodoTask> heirarchy, int lineNumber)
		{
			yield return lineNumber;

			foreach (var child in GetChildren(heirarchy, lineNumber))
			{
				yield return child;
			}
		}

		public static IEnumerable<int> GetChildren(IList<TodoTask> heirarchy, int lineNumber)
		{
			var otherLineNumber = lineNumber;
			while (++otherLineNumber < heirarchy.Count)
			{
				if (heirarchy[otherLineNumber].Level > heirarchy[lineNumber].Level)
				{
					yield return otherLineNumber;
				}
				else
				{
					break;
				}
			}
		}

		public static int Depth(IList<TodoTask> heirarchy, int lineNumber) => 1 + ChildCount(heirarchy, lineNumber);

		public static int ChildCount(IList<TodoTask> heirarchy, int lineNumber)
		{
			var count = 0;
			var otherLineNumber = lineNumber;
			while (++otherLineNumber < heirarchy.Count)
			{
				if (heirarchy[otherLineNumber].Level > heirarchy[lineNumber].Level)
				{
					count++;
				}
				else
				{
					break;
				}
			}

			return count;
		}

		public static bool TryFindPreviousSibling(IList<TodoTask> heirarchy, int lineNumber, out int siblingLineNumber)
		{
			siblingLineNumber = lineNumber;
			while (--siblingLineNumber >= 0)
			{
				if (heirarchy[siblingLineNumber].Level == heirarchy[lineNumber].Level)
				{
					return true;
				}

				if (heirarchy[siblingLineNumber].Level < heirarchy[lineNumber].Level)
				{
					break;
				}
			}

			return false;
		}

		public static bool TryFindNextSibling(IList<TodoTask> heirarchy, int lineNumber, out int siblingLineNumber)
		{
			siblingLineNumber = lineNumber;
			while (++siblingLineNumber < heirarchy.Count)
			{
				if (heirarchy[siblingLineNumber].Level == heirarchy[lineNumber].Level)
				{
					return true;
				}

				if (heirarchy[siblingLineNumber].Level < heirarchy[lineNumber].Level)
				{
					break;
				}
			}

			return false;
		}
	}
}
