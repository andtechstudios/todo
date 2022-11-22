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
	}
}
