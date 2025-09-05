using CommandLine;
using CommandLine.Text;
using FFMediaToolkit;

namespace Outlast2MovieDecoder;

internal class Program
{
	internal static void Main( string[] args )
	{
		Log.Initialize( false );

		var parser = new CommandLine.Parser( with => { with.HelpWriter = null; with.EnableDashDash = true; } );
		var result = parser.ParseArguments<CommandLineOptions>( args );

		result
			.WithParsed<CommandLineOptions>( OnParsed )
			.WithNotParsed( err => DisplayHelp( result, err ) );
	}

	public static void OnParsed( object Obj )
	{
		CommandLineOptions opts = (CommandLineOptions)Obj;

		string ffmpegDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "ffmpeg" );
		FFmpegLoader.FFmpegPath = ffmpegDir;

		string inputFile = opts.File;

		MovieLoader loader = new();
		var movie = loader.LoadMovie( inputFile );

		if ( movie is not null )
		{
			string outputPath = Path.Combine( Path.GetDirectoryName( inputFile )!, $"{Path.GetFileNameWithoutExtension( inputFile )}.webm" );
			Log.Info( $"Writing movie to {outputPath}" );
			loader.WriteMovieToWebm( movie, outputPath );

			outputPath = Path.Combine( Path.GetDirectoryName( inputFile )!, $"{Path.GetFileNameWithoutExtension( inputFile )}.wav" );
			Log.Info( $"Writing audio to {outputPath}" );
			loader.WriteAudioToWave( movie, outputPath );

			Log.Info( "Finished!" );
		}
	}

	private static void DisplayHelp<T>( ParserResult<T> Result, IEnumerable<Error> Errors )
	{
		var helpText = HelpText.AutoBuild( Result, h =>
		{
			h.AddNewLineBetweenHelpSections = false;
			h.AdditionalNewLineAfterOption = false;
			h.AddDashesToOption = true;
			h.Copyright = "";
			return HelpText.DefaultParsingErrorsHandler( Result, h );
		}, e => e, verbsIndex: true );

		var splitHelpText = helpText.ToString().Split( Environment.NewLine, StringSplitOptions.None );
		foreach ( var line in splitHelpText )
		{
			Log.Info( line );
		}
	}
}
