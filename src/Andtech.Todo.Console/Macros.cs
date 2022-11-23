using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal static class Macros
	{

		public static void Swap<T>(this List<T> list, int a, int b)
		{
			var temp = list[b];
			list[b] = list[a];
			list[a] = temp;
		}

		public static bool TryFindPreviousSibling(int lineNumber, IList<TodoTask> heirarchy, out int siblingLineNumber)
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

		public static bool TryFindNextSibling(int lineNumber, IList<TodoTask> heirarchy, out int siblingLineNumber)
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
