namespace Andtech.Todo.Test
{

	public class TodoTaskTests
	{

		[Test]
		public void ParseBasic()
		{
			var task = TodoTask.Parse("Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
		}

		[Test]
		public void ParseBullet()
		{
			var task = TodoTask.Parse("* Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
		}

		[Test]
		public void ParseDash()
		{
			var task = TodoTask.Parse("- Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
		}

		[Test]
		public void ParseNumbering()
		{
			var task = TodoTask.Parse("22. Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
		}

		[Test]
		public void ParseTaskWithoutStatus()
		{
			var task = TodoTask.Parse("* Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
			Assert.That(task.IsCompleted, Is.EqualTo(false));
		}
	}
}