using System;
using System.Diagnostics;
using Android.Media;
using TicketToTalk.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AudioPlayerImplementation))]
namespace TicketToTalk.Droid
{
	/// <summary>
	/// Audio player implementation.
	/// </summary>
	public class AudioPlayerImplementation : IAudioPlayer
	{
		MediaPlayer player = new MediaPlayer();
		private bool finished = false;
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
			var path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;
		
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

			finished = true;

			MessagingCenter.Send<AudioPlayerImplementation, bool>(this, "finished_playback", finished);
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		/// <returns>The current time.</returns>
		public int GetCurrentTime()
		{
			return player.CurrentPosition;
		}

		/// <summary>
		/// Hases the finished playing.
		/// </summary>
		/// <returns><c>true</c>, if finished playing was hased, <c>false</c> otherwise.</returns>
		public bool HasFinishedPlaying()
		{
			return finished;
		}
	}
}

