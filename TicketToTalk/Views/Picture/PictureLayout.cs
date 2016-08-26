using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Picture layout.
	/// </summary>
	public class PictureLayout : ContentView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PictureLayout"/> class.
		/// </summary>
		public PictureLayout(string filePath)
		{
			
			Image image = new Image();
			loadImage(image, filePath);//loadImage(image, filePath);
			Content = image;
		}

		public PictureLayout(byte[] media) 
		{
			Image image = new Image();
			image.Source = ImageSource.FromStream(() => new MemoryStream(media));
			Content = image;
		}

		private void loadImage(Image image, string filePath) 
		{
			var rawBytes = MediaController.readBytesFromFile(filePath);
			image.Source = ImageSource.FromStream(() => new MemoryStream(rawBytes));
		}
	}
}


