using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace TicketToTalk
{
	public class CameraController
	{
		public CameraController()
		{
		}

		/// <summary>
		/// Selects the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		public static async Task<MediaFile> TakePicture(string name)
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				return null;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{

				Directory = "TicketToTalk",
				Name = name
			});

			if (file == null)
			{
				return null;
			}
			else
			{
				return file;
			}
		}

		public static async Task<MediaFile> SelectPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				return null;
			}

			var file = await CrossMedia.Current.PickPhotoAsync();
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

