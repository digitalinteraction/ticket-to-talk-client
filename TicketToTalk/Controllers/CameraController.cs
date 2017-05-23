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
				Console.WriteLine("Camera is not available");
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

		///// <summary>
		///// Takes the photo.
		///// </summary>
		///// <param name="sender">Sender.</param>
		///// <param name="args">Arguments.</param>
		//public async void TakePhoto(object sender, EventArgs args)
		//{
		//	Device.BeginInvokeOnMainThread(async () =>
		//	{
		//		//Ask the user if they want to use the camera or pick from the gallery
		//		var action = await Application.Current.MainPage.DisplayActionSheet("Add Photo", "Cancel", null, "Choose Existing", "Take Photo");
		//		Console.WriteLine(action);

		//		if (action == "Choose Existing")
		//		{
		//			DependencyService.Get<CameraInterface>().BringUpPhotoGallery();
		//		}
		//		else if (action == "Take Photo")
		//		{
		//			DependencyService.Get<CameraInterface>().BringUpCamera();
		//		}
		//	});
		//}
	}
}

