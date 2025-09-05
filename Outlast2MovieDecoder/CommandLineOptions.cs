using CommandLine;

namespace Outlast2MovieDecoder;

internal class CommandLineOptions
{
	[Value( 0 )]
	public string File { get; set; } = "";
}
