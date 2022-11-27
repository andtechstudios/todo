using NUnit.Framework.Internal;
using System.Text;

namespace Andtech.Todo.Test
{

	public class TodoListTests
	{

		[Test]
		public void ParseBasic()
		{
			var lines = string.Join(Environment.NewLine,
				"* Call Saul"
			);

			var list = TodoList.Parse(lines);
			Assert.That(list.Tasks[0].Title, Is.EqualTo("Call Saul"));
		}

		[Test]
		public void ParseNested()
		{
			var lines = string.Join(Environment.NewLine,
				"* Call Saul",
				"	* [ ] Ask for Viktor"
			);

			var list = TodoList.Parse(lines);
			Assert.That(list.Tasks[0].Title, Is.EqualTo("Call Saul"));
			Assert.That(list.Tasks[1].Title, Is.EqualTo("Ask for Viktor"));
		}

		[Test]
		public void ParseLevels()
		{
			var lines = string.Join(Environment.NewLine,
				"* Call Saul",
				"	* [ ] Ask for Viktor",
				"		* [ ] Tell 'em Jimmy sent 'ya",
				"	* [ ] Ask for Gizelle",
				"* Gimme Jimmy"
			);

			var list = TodoList.Parse(lines);
			Assert.That(list.Tasks[0].Level, Is.EqualTo(0));
			Assert.That(list.Tasks[1].Level, Is.EqualTo(1));
			Assert.That(list.Tasks[2].Level, Is.EqualTo(2));
			Assert.That(list.Tasks[3].Level, Is.EqualTo(1));
			Assert.That(list.Tasks[4].Level, Is.EqualTo(0));
		}
	}
}