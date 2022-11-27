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

		[Test]
		public void ParseWithMarkdown()
		{
			var task = TodoTask.Parse("* Call Saul: it *is* **important** that `we` speak to him urgently");
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
			Assert.That(task.Description, Is.EqualTo("it *is* **important** that `we` speak to him urgently"));
		}

		[Test]
		public void ParseTags()
		{
			var task = TodoTask.Parse("* Call Saul #work #legal");
			CollectionAssert.AreEqual(new string[] { "work", "legal" }, task.Tags);
		}

		[Test]
		public void ParseDate()
		{
			var task = TodoTask.Parse("* Call Saul due:2009-04-26");
			Assert.That(DateOnly.FromDateTime(task.DueDate), Is.EqualTo(DateOnly.Parse("2009-04-26")));
		}

		[Test]
		public void ParseAssignees()
		{
			var task = TodoTask.Parse("* Call Saul @me @jpinkman @swhite");
			CollectionAssert.AreEqual(new string[] { "me", "jpinkman", "swhite" }, task.Assignees);
		}

		[Test]
		public void ParseMultiple()
		{
			var task = TodoTask.Parse("* Call Saul #work #legal due:2009-04-26 @me @jpinkman @swhite");
			CollectionAssert.AreEqual(new string[] { "work", "legal" }, task.Tags);
			Assert.That(DateOnly.FromDateTime(task.DueDate), Is.EqualTo(DateOnly.Parse("2009-04-26")));
			CollectionAssert.AreEqual(new string[] { "me", "jpinkman", "swhite" }, task.Assignees);
		}

		[Test]
		public void ParseComplete()
		{
			var task = TodoTask.Parse("* [x] Call Saul: better call saul #work due:2009-04-26 @jpinkman");
			Assert.That(task.IsCompleted, Is.EqualTo(true));
			Assert.That(task.Title, Is.EqualTo("Call Saul"));
			Assert.That(task.Description, Is.EqualTo("better call saul"));
			CollectionAssert.AreEqual(new string[] { "work" }, task.Tags);
			Assert.That(DateOnly.FromDateTime(task.DueDate), Is.EqualTo(DateOnly.Parse("2009-04-26")));
			CollectionAssert.AreEqual(new string[] { "jpinkman" }, task.Assignees);
		}
	}
}