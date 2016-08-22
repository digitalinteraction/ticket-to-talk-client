using System;
namespace TicketToTalk
{
	public interface IAudioRecorder
	{
		void Record(string path);
		string FinishRecording();
	}
}

