using System;
using System.Diagnostics;
using Android.Media;
using TicketToTalk.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AudioPlayerImplementation))]
namespace TicketToTalk.Droid
{
	/// <summary>
	/// Audio player implementation.
	/// </summary>
	public class AudioPlayerImplementation : IAudioPlayer
	{
		MediaPlayer player = new MediaPlayer();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Droid.AudioPlayerImplementation"/> class.
		/// </summary>
		public AudioPlayerImplementation()
		{
			
		}

		/// <summary>
		/// Setups the player.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="fileName">File name.</param>
		public void SetupPlayer(string fileName)
		{
			player = new MediaPlayer();
			Debug.WriteLine("AndroidAudioPlayer: setting player");
			var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
			Debug.WriteLine("AndroidAudioPlayer: " + path);
			//path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
			//var fd = Android.App.Application.Context.Assets.OpenFd(path);
			Debug.WriteLine("AndroidAudioPlayer: Set up player to file - " + path);
			//player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
			player.SetDataSource(path);
			player.Prepare();
			player.Completion += Player_Completion;
		}

		/// <summary>
		/// Checks if the audio is playing
		/// </summary>
		/// <returns>The playing.</returns>
		public bool IsPlaying()
		{
			return player.IsPlaying;
		}

		/// <summary>
		/// Pauses the playback.
		/// </summary>
		/// <returns>The playback.</returns>
		public void PausePlayback()
		{
			player.Pause();
		}

		/// <summary>
		/// Plays the audio file
		/// </summary>
		/// <returns>The audio file.</returns>
		public void PlayAudioFile()
		{
			player.Start();
		}

		//public void PlayAudioFile(string fileName)
		//{
		//	var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
		//	player.Prepared += (s, e) =>
		//	{
		//		player.Start();
		//	};
		//	player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
		//	player.Prepare();
		//}

		/// <summary>
		/// Resumes the play back.
		/// </summary>
		/// <returns>The play back.</returns>
		public void ResumePlayBack()
		{
			player.Start();
		}

		/// <summary>
		/// Stops the play back.
		/// </summary>
		/// <returns>The play back.</returns>
		public void StopPlayBack()
		{
			player.Stop();
			player.Release();
			player = null;
		}

		/// <summary>
		/// Gets the duration.
		/// </summary>
		/// <returns>The duration.</returns>
		public int GetDuration()
		{
			return player.Duration;
		}

		/// <summary>
		/// On player completion
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Player_Completion(object sender, EventArgs e)
		{
			player.Stop();
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		/// <returns>The current time.</returns>
		public int GetCurrentTime()
		{
			return player.CurrentPosition;
		}
	}
}

