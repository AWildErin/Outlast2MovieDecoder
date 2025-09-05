using K4os.Compression.LZ4;

namespace Outlast2MovieDecoder.Structs;

internal class OutlastMovieFrame
{
	public float FrameTime { get; set; }
	public uint CameraMode { get; set; }
	public uint Offset { get; set; }
	public uint JpegSize { get; set; }
	public uint LZ4Size { get; set; }

	public byte[]? FrameData { get; set; }

	public void Read( BinaryReader Reader )
	{
		FrameTime = Reader.ReadSingle();
		CameraMode = Reader.ReadUInt32();
		Offset = Reader.ReadUInt32();
		JpegSize = Reader.ReadUInt32();
		LZ4Size = Reader.ReadUInt32();

		// Read the video data
		var currentPosition = Reader.BaseStream.Position;

		Reader.BaseStream.Seek( Offset, SeekOrigin.Begin );
		if ( LZ4Size > 0 )
		{
			byte[] compressedBuffer = new byte[LZ4Size];
			for ( uint i = 0; i < LZ4Size; i++ )
			{
				compressedBuffer[i] = Reader.ReadByte();
			}

			FrameData = new byte[JpegSize];

			LZ4Codec.Decode( compressedBuffer, FrameData );
		}
		else if ( JpegSize > 0 )
		{
			FrameData = new byte[JpegSize];

			// @todo Make this different, doing this because jpegsize is uint
			for ( uint i = 0; i < JpegSize; i++ )
			{
				FrameData[i] = Reader.ReadByte();
			}
		}
		else
		{
			throw new Exception( "Neither JpegSzie or LZ4Size were set!" );
		}

		// Go back to where we were
		Reader.BaseStream.Seek( currentPosition, SeekOrigin.Begin );
	}
}
