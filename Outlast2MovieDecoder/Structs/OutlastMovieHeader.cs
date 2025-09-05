namespace Outlast2MovieDecoder.Structs;

internal class OutlastMovieHeader
{
	public char[] Header { get; set; } = new char[8];
	public uint Version { get; set; }
	public uint NumFrames { get; set; }
	public uint ResolutionX { get; set; }
	public uint ResolutionY { get; set; }
	public uint AudioOffset { get; set; }
	public uint AudioSize { get; set; }
	public uint MaxJpegSize { get; set; }

	public void Read( BinaryReader Reader )
	{
		Header = Reader.ReadChars( Header.Length );

		Version = Reader.ReadUInt32();
		NumFrames = Reader.ReadUInt32();
		ResolutionX = Reader.ReadUInt32();
		ResolutionY = Reader.ReadUInt32();
		AudioOffset = Reader.ReadUInt32();
		AudioSize = Reader.ReadUInt32();
		MaxJpegSize = Reader.ReadUInt32();
	}
}
