using System;
namespace TicketToTalk
{

	/// <summary>
	/// Audio player.
	/// </summary>
	public interface IAudioPlayer
	{
		void SetupPlayer(string fileName);
		void PlayAudioFile();
		void StopPlayBack();
		void PausePlayback();
		void ResumePlayBack();
		bool IsPlaying();
		int GetDuration();
		int GetCurrentTime();
		bool HasFinishedPlaying();
	}
}

