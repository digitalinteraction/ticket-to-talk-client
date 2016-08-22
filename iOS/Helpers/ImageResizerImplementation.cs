using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using TicketToTalk.iOS;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizerImplementation))]
namespace TicketToTalk.iOS
{
	public class ImageResizerImplementation : IImageResizer
	{
		public ImageResizerImplementation()
		{
		}

		public byte[] ResizeImage(byte[] imageData, float width, float height)
		{
			//UIImage originalImage = ImageFromByteArray(imageData);
			UIImage originalImage = new UIImage(NSData.FromArray(imageData));
			UIImageOrientation orientation = originalImage.Orientation;

			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
												 (int)width, (int)height, 8,
												 (int)(4 * width), CGColorSpace.CreateDeviceRGB(),
												 CGImageAlphaInfo.PremultipliedFirst))
			{

				RectangleF imageRect = new RectangleF(0, 0, width, height);

				// draw the image
				context.DrawImage(imageRect, originalImage.CGImage);

				UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

				// save the image as a jpeg
				return resizedImage.AsJPEG().ToArray();
			}
		}
	}
}

