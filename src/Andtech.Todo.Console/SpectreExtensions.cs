using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.Todo
{

	internal sealed class ControlCode : Renderable
	{
		private readonly Segment _segment;

		public ControlCode(string control)
		{
			_segment = Segment.Control(control);
		}

		protected override Measurement Measure(RenderOptions options, int maxWidth)
		{
			return new Measurement(0, 0);
		}

		protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
		{
			if (options.Ansi)
			{
				yield return _segment;
			}
		}
	}

	public static class SpectreExtensions
	{

		/// <summary>
		/// Switches to an alternate screen buffer asynchronously if the terminal supports it.
		/// </summary>
		/// <param name="console">The console.</param>
		/// <param name="action">The action to execute within the alternate screen buffer.</param>
		/// <returns>The result of the function.</returns>
		public static async Task AlternateScreenAsync(this IAnsiConsole console, Func<Task> action)
		{
			if (console is null)
			{
				throw new ArgumentNullException(nameof(console));
			}

			if (!console.Profile.Capabilities.Ansi)
			{
				throw new NotSupportedException("Alternate buffers are not supported since your terminal does not support ANSI.");
			}

			if (!console.Profile.Capabilities.AlternateBuffer)
			{
				throw new NotSupportedException("Alternate buffers are not supported by your terminal.");
			}

			// Switch to alternate screen
			console.Write(new ControlCode("\u001b[?1049h\u001b[H"));

			try
			{
				// Execute custom action
				await action();
			}
			finally
			{
				// Switch back to primary screen
				console.Write(new ControlCode("\u001b[?1049l"));
			}
		}
	}
}
