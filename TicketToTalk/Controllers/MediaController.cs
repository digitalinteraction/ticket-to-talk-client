using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace TicketToTalk
{
	/// <summary>
	/// Media controller.
	/// </summary>
	public class MediaController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.MediaController"/> class.
		/// </summary>
		public MediaController()
		{
		}

		/// <summary>
		/// Reads the bytes from file.
		/// </summary>
		/// <returns>The bytes from file.</returns>
		/// <param name="fileName">File name.</param>
		public static byte[] readBytesFromFile(string fileName)
		{
			string path = fileName;
			Debug.WriteLine("MediaController: Reading bytes from file with fileName - " + fileName);
#if __IOS__
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
#else

			if (fileName.StartsWith("p_", StringComparison.Ordinal) || fileName.StartsWith("t_", StringComparison.Ordinal) || fileName.StartsWith("u_", StringComparison.Ordinal))
			{
				path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
				path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
			}
#endif
			try
			{
				var bytes = File.ReadAllBytes(path);

				return bytes;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("MediaController: Could not find file at path: " + path);
				Debug.WriteLine(ex);
				return new byte[0];
			}
		}

		/// <summary>
		/// Writes the image to file.
		/// </summary>
		/// <returns><c>true</c>, if image was written to file, <c>false</c> otherwise.</returns>
		/// <param name="fileName">File name.</param>
		/// <param name="bytes">Bytes.</param>
		public static bool writeImageToFile(string fileName, byte[] bytes)
		{
			Debug.WriteLine("MediaController: Writing bytes to file.");
#if __IOS__
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
#else
			var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
#endif
			try
			{
				File.WriteAllBytes(path, bytes);
				Debug.WriteLine("MediaController: Wrote bytes");
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return false;
			}
		}

		/// <summary>
		/// Takes the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		public async Task<MediaFile> TakePicture()
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				return null;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{

				Directory = "TicketToTalk",
				Name = "ticket.jpg"
			});

			if (file == null)
				return null;

			return file;
		}

		/// <summary>
		/// Deletes the file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public void deleteFile(string fileName)
		{
#if __IOS__
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
#else
			var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
#endif
			File.Delete(path);
		}
	}
}

