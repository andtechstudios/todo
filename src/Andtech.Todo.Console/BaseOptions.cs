using Andtech.Common;
using CommandLine;

public class BaseOptions
{
	[Option("verbosity", HelpText = "Sets the verbosity level of the command.")]
	public Verbosity Verbosity { get; set; }
	[Option('v', "verbose", HelpText = "Run the command with verbose output. (Same as running with --verbosity verbose)")]
	public bool Verbose { get; set; }
	[Option('n', "dry-run", HelpText = "Dry run the command.")]
	public bool DryRun { get; set; }
}

