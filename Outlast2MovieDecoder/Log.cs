using NLog;

namespace Outlast2MovieDecoder;

public static partial class Log
{
	private static readonly Logger logger = LogManager.GetCurrentClassLogger();

	private static string currentLogFile { get; set; } = string.Empty;
	public static string GetLogFilePath() => currentLogFile;

	/// <summary>
	/// Initializes the application with a standard NLog config
	/// </summary>
	/// <param name="logOutput">The log file name prefix</param>
	public static void Initialize( bool LogToFile = false )
	{
		var startDateTime = DateTime.Now;
		var fileName = $"logs/log_{startDateTime.ToString( "HH-mm-ss" )}.log";
		var logFormat = @"${date:format=HH\:mm\:ss} ${level} | ${message}";

		currentLogFile = fileName;

		LogLevel logToFileMinlevel = LogLevel.Info;
#if DEBUG
		logToFileMinlevel = LogLevel.Debug;
#endif

		LogManager.Setup().LoadConfiguration( builder =>
		{
			builder.ForLogger().FilterMinLevel( LogLevel.Debug ).WriteToColoredConsole( logFormat, enableAnsiOutput: true );
			if ( LogToFile )
			{
				builder.ForLogger().FilterMinLevel( logToFileMinlevel ).WriteToFile( fileName, logFormat );
			}
		} );

		if ( LogToFile )
		{
			Debug( $"Logger initialised, log file is at: {fileName}" );
		}
		else
		{
			Debug( "Logger initialised, will not log to file." );
		}
	}

	public static void Trace( object val ) => logger.Trace( val );
	public static void Debug( object val ) => logger.Debug( val );
	public static void Info( object val ) => logger.Info( val );
	public static void Warn( object val ) => logger.Warn( val );
	public static void Error( object val ) => logger.Error( val );
	public static void Fatal( object val ) => logger.Fatal( val );

}

