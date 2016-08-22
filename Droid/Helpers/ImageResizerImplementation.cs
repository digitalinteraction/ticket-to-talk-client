using System;
using System.IO;
using Android.Graphics;
using TicketToTalk.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizerImplementation))]
namespace TicketToTalk.Droid
{
	public class ImageResizerImplementation : IImageResizer
	{
		public ImageResizerImplementation()
		{
		}

		public byte[] ResizeImage(byte[] imageData, float width, float height)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray();
			}
		}
	}
}

