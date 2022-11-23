using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo.Console.Editor
{

	internal interface IIndentable
	{

		void IncreaseLevel();

		void DecreaseLevel();
	}
}
