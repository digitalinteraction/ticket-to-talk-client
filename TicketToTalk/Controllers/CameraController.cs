using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{

	public class CameraController
	{
		public event MediaReadyHandler MediaReady;
		public delegate void MediaReadyHandler(MediaFile file);

		public CameraController()
		{
		}

		/// <summary>
		/// Takes the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		/// <param name="name">Name.</param>
		public async Task<MediaFile> TakePicture(string name)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
                await Application.Current.MainPage.DisplayAlert("Take a Photo", "The camera is not available.", "OK");
				return null;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "TicketToTalk",
				Name = name
			});

			try
			{
				//file = await CrossMedia.Current.PickPhotoAsync();
				Device.BeginInvokeOnMainThread( async () => { 
					var f = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						Directory = "TicketToTalk",
						Name = name
					});
					MediaReady(f);
				});
			}
			catch (Exception e) 
			{
				Console.WriteLine(e);
			}

			if (file == null)
			{
				return null;
			}
			else
			{
				return file;
			}
		}

		/// <summary>
		/// Selects the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		public async Task<MediaFile> SelectPicture()
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				return null;
			}

			MediaFile file = null;
			try
			{
				//file = await CrossMedia.Current.PickPhotoAsync();
				Device.BeginInvokeOnMainThread( async () => { 
					var f = await CrossMedia.Current.PickPhotoAsync();
					MediaReady(f);
				});
			}
			catch (Exception e) 
			{
				Console.WriteLine(e);
			}

			if (file == null)
			{
				return null;
			}
			else
			{
				return file;
			}
		}
	}
}

