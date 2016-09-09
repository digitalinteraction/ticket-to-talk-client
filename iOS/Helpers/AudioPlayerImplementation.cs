using System;
using System.Diagnostics;
using System.IO;
using AVFoundation;
using Foundation;
using TicketToTalk.iOS;
using Xamarin.Forms;

[assembly: Dependency (typeof (AudioPlayerImplementation))]
namespace TicketToTalk.iOS
{
	public class AudioPlayerImplementation : IAudioPlayer
	{
		AVAudioPlayer _player;

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
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
			Debug.WriteLine("AudioPlayer: Path - " + path);
			var url = NSUrl.FromString(path);
			Debug.WriteLine(url);

			_player = AVAudioPlayer.FromUrl(url);
			_player.Volume = 100;

			_player.FinishedPlaying += PlayerFinishedPlaying;
			_player.PrepareToPlay();
		}

		/// <summary>
		/// Plays the file
		/// </summary>
		/// <returns>The audio file.</returns>
		public void PlayAudioFile()
		{
			//string sFilePath = NSBundle.MainBundle.PathForResource
			//(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
			//var url = NSUrl.FromString(sFilePath);

			_player.Play();
		}

		/// <summary>
		/// Pauses the playback.
		/// </summary>
		/// <returns>The playback.</returns>
		public void PausePlayback()
		{
			_player.Pause();
		}

		/// <summary>
		/// Resumes the play back.
		/// </summary>
		/// <returns>The play back.</returns>
		public void ResumePlayBack()
		{
			_player.Play();
		}

		/// <summary>
		/// Stops the play back.
		/// </summary>
		/// <returns>The play back.</returns>
		public void StopPlayBack()
		{
			_player.Stop();
		}

		/// <summary>
		/// Players the finished playing.
		/// </summary>
		/// <returns>The finished playing.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void PlayerFinishedPlaying(object sender, AVStatusEventArgs e) 
		{
			//_player = null;
			_player.Stop();
			Debug.WriteLine("Audio Finished");
		}

		/// <summary>
		/// Ises the playing.
		/// </summary>
		/// <returns>The playing.</returns>
		public bool IsPlaying()
		{
			if (_player == null)
			{
				return false;
			}
			else 
			{
				return _player.Playing;
			}
		}

		/// <summary>
		/// Gets the duration.
		/// </summary>
		/// <returns>The duration.</returns>
		public int GetDuration()
		{
			return (int) _player.Duration;
		}

		/// <summary>
		/// Gets the current time.
		/// </summary>
		/// <returns>The current time.</returns>
		public int GetCurrentTime()
		{
			return (int) _player.CurrentTime;
		}
	}
}

