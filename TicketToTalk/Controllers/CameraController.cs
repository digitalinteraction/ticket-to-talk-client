using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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

			if (!(await CheckStoragePerms()))
			{
				await App.Current.MainPage.DisplayAlert("Take a Photo", "Ticket to Talk does not have permission to take photos", "OK");
			}

            Debug.WriteLine("Got permission");

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
                await Application.Current.MainPage.DisplayAlert("Take a Photo", "The camera is not available.", "OK");
				return null;
			}

            MediaFile file = null;

			try
			{
				//file = await CrossMedia.Current.PickPhotoAsync();
				Device.BeginInvokeOnMainThread( async () => { 
					var f = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						Directory = "TicketToTalk",
						Name = name
					});
                    Debug.WriteLine(f.AlbumPath);
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

            Debug.WriteLine("Got permission");

            if(!(await CheckStoragePerms())) 
            {
                await App.Current.MainPage.DisplayAlert("Take a Photo", "Ticket to Talk does not have permission to take photos", "OK");
            }

			MediaFile file = null;
			try
			{
				//file = await CrossMedia.Current.PickPhotoAsync();
				Device.BeginInvokeOnMainThread( async () => { 
					var f = await CrossMedia.Current.PickPhotoAsync();

                    Debug.WriteLine(f.AlbumPath);
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

		private async Task<bool> CheckStoragePerms()
		{
			try
			{
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
				if (status != PermissionStatus.Granted)
				{
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
					{
                        await Application.Current.MainPage.DisplayAlert("Storage", "Ticket to Talk needs access to the camera.", "OK");
					}
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
				}

				if (status == PermissionStatus.Granted)
				{
					return true;
				}
				else if (status != PermissionStatus.Unknown)
				{
					await Application.Current.MainPage.DisplayAlert("Storage Denied", "Cannot save tickets without access to audio.", "OK");
					return false;
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return false;
			}

			return false;
		}
	}
}

