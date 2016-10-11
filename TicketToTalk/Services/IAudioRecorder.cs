using System;
namespace TicketToTalk
{
	/// <summary>
	/// Audio recorder.
	/// </summary>
	public interface IAudioRecorder
	{
		void Record(string path);
		string FinishRecording();
	}
}

