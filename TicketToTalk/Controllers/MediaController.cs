﻿using System;
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
		public static byte[] ReadBytesFromFile(string fileName)
		{
			string path = fileName;
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
				Debug.WriteLine(ex.StackTrace);
				return new byte[0];
			}
		}

		/// <summary>
		/// Writes the image to file.
		/// </summary>
		/// <returns><c>true</c>, if image was written to file, <c>false</c> otherwise.</returns>
		/// <param name="fileName">File name.</param>
		/// <param name="bytes">Bytes.</param>
		public static bool WriteImageToFile(string fileName, byte[] bytes)
		{
#if __IOS__
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
#else
			var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
#endif
			try
			{
				File.WriteAllBytes(path, bytes);
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
				return false;
			}
		}

		/// <summary>
		/// Deletes the file.
		/// </summary>
		/// <param name="fileName">File name.</param>
		public void DeleteFile(string fileName)
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

