using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class Session
	{
		public string ProjectDir { get; set; }
		public List<TodoList> Lists { get; set; } = new List<TodoList>();

		public LinearWindow Window { get; set; }
		public RawScreen Screen { get; set; }

		public static Session Instance { get; set; }
	}
}
