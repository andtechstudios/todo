using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console
{

	internal class Session
	{
		public string ProjectPath { get; set; }
		public TodoList TodoList { get; set; }
		public bool CanWrite { get; set; }
		public string Log { get; set; }

		public static Session Instance { get; set; }
	}
}
