using System;
namespace TicketToTalk
{
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
	}
}

