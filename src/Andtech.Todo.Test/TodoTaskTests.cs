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
			Assert.Pass();
		}

		[Test]
		public void ParseDash()
		{
			var task = TodoTask.Parse("- Call Saul");
			Assert.Pass();
		}

		[Test]
		public void ParseNumbering()
		{
			var task = TodoTask.Parse("22. Call Saul");
			Assert.Pass();
		}

		[Test]
		public void ParseTaskWithoutStatus()
		{
			var task = TodoTask.Parse("* Call Saul");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
			Assert.That(task.IsCompleted, Is.EqualTo(false));
		}

		[Test]
		public void ParseWithMarkdownLink()
		{
			var task = TodoTask.Parse("* Call Saul: visit [Saul's Website](http://bettercallsaul.amc.com/) for details.");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
			Assert.That(task.Description, Is.EqualTo("visit [Saul's Website](http://bettercallsaul.amc.com/) for details."));
		}
	}
}