using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using NAudio.Wave;
using Outlast2MovieDecoder.Structs;
using SkiaSharp;

namespace Outlast2MovieDecoder;

internal class MovieLoader
{
	public MovieLoader()
	{

	}

	public OutlastMovie? LoadMovie( string path )
	{
		Log.Info( $"Loading {path}" );

		using var stream = new FileStream( path, FileMode.Open, FileAccess.Read );
		using var reader = new BinaryReader( stream );

		OutlastMovie movie = new();

		movie.Header.Read( reader );

		for ( int i = 0; i < movie.Header.NumFrames; i++ )
		{
			OutlastMovieFrame frame = new();
			frame.Read( reader );
			movie.Frames.Add( frame );
		}

		reader.BaseStream.Seek( movie.Header.AudioOffset, SeekOrigin.Begin );
		movie.AudioData = reader.ReadBytes( (int)movie.Header.AudioSize );

		if (movie.AudioData is not null)
		{
			// @TODO TEMP, FIGURE OUT WHY THIS IS LIKE THIS
			movie.AudioData[20] = 1;
			movie.AudioData[21] = 0;
		}

		Log.Info( "Initial details:" );
		Log.Info( $"Header: {string.Join( ", ", movie.Header.Header )}" );
		Log.Info( $"Version: {movie.Header.Version}" );
		Log.Info( $"NumFrames: {movie.Header.NumFrames}" );
		Log.Info( $"ResolutionX: {movie.Header.ResolutionX}" );
		Log.Info( $"ResolutionY: {movie.Header.ResolutionY}" );
		Log.Info( $"AudioOffset: {movie.Header.AudioOffset}" );
		Log.Info( $"AudioSize: {movie.Header.AudioSize}" );
		Log.Info( $"MaxJpegSize: {movie.Header.MaxJpegSize}" );

		return movie;
	}

	public void WriteMovieToWebm( OutlastMovie Movie, string OutputPath )
	{
		int width = (int)Movie.Header.ResolutionX;
		int height = (int)Movie.Header.ResolutionY;

		var settings = new VideoEncoderSettings( width: width, height: height, framerate: 30, codec: VideoCodec.VP9 )
		{
			EncoderPreset = EncoderPreset.Fast,
			CRF = 17,
		};

		using var wavReader = new WaveFileReader( new MemoryStream( Movie.AudioData ) );
		var audSettings = new AudioEncoderSettings( wavReader.WaveFormat.SampleRate, wavReader.WaveFormat.Channels );

		using var file = MediaBuilder
			.CreateContainer( OutputPath )
			.WithVideo( settings )
			.WithAudio( audSettings )
			.Create();

		// Add video
		foreach ( var frame in Movie.Frames )
		{
			// @todo exception
			if ( frame.FrameData is null )
			{
				continue;
			}

			using var bmp = SKBitmap.Decode( frame.FrameData );
			file.Video.AddFrame( new ImageData( bmp.GetPixelSpan(), ImagePixelFormat.Bgra32, width, height ) );
		}

	}

	public void WriteAudioToWave( OutlastMovie Movie, string OutputPath )
	{
		if ( Movie.AudioData is not null )
		{
			File.WriteAllBytes( OutputPath, Movie.AudioData );
		}
	}
}
