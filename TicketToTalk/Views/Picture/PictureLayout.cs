﻿using System.IO;
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
			image.WidthRequest = Session.ScreenWidth;
			image.HeightRequest = Session.ScreenWidth;
			image.Aspect = Aspect.AspectFill;
			Content = image;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PictureLayout"/> class.
		/// </summary>
		/// <param name="media">Media.</param>
		public PictureLayout(byte[] media)
		{
			Image image = new Image();
			image.Source = ImageSource.FromStream(() => new MemoryStream(media));
			Content = image;
		}

		/// <summary>
		/// Loads the image.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="filePath">File path.</param>
		private void loadImage(Image image, string filePath)
		{
			var rawBytes = MediaController.ReadBytesFromFile(filePath);
			image.Source = ImageSource.FromStream(() => new MemoryStream(rawBytes));
		}
	}
}


