using System;
namespace TicketToTalk
{
	public interface IImageResizer
	{
		byte[] ResizeImage(byte[] imageData, float width, float height);
	}
}

