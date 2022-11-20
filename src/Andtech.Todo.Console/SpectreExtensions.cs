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

		protected override Measurement Measure(Spectre.Console.Rendering.RenderOptions options, int maxWidth)
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

	internal class SpectreExtensions
	{
	}
}
