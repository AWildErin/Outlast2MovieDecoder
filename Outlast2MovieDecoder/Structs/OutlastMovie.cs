namespace Outlast2MovieDecoder.Structs;

internal class OutlastMovie
{
	public OutlastMovieHeader Header { get; set; } = new();
	public List<OutlastMovieFrame> Frames { get; set; } = new();

	public byte[]? AudioData { get; set; }
}
