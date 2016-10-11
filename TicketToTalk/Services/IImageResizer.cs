using System;
namespace TicketToTalk
{
	/// <summary>
	/// Image resizer.
	/// </summary>
	public interface IImageResizer
	{
		byte[] ResizeImage(byte[] imageData, float width, float height);
	}
}

