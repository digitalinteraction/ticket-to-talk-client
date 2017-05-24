using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Media;
using Android.Runtime;
using Android.Widget;
using TicketToTalk.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AudioRecorderImplementation))]
namespace TicketToTalk.Droid
{
	public class AudioRecorderImplementation : IAudioRecorder
	{
		public MainActivity activity;
		public static int requestAudioId = 0;

		MediaRecorder _recorder = new MediaRecorder();
		String fileName;

		public AudioRecorderImplementation()
		{
			
		}

		public void Record(string fileName)
		{
			this.fileName = fileName;

			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
			path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;

			Debug.WriteLine("AndroidAudioRecorder: path = " + path);
			_recorder.SetAudioSource(AudioSource.Mic);
			_recorder.SetOutputFormat(OutputFormat.ThreeGpp);
			_recorder.SetAudioEncoder(AudioEncoder.AmrNb);
			_recorder.SetOutputFile(path);
			_recorder.Prepare();
			_recorder.Start();
		}

		public string FinishRecording()
		{
			_recorder.Stop();
			_recorder.Reset();
			_recorder.Release();
			return fileName;
		}
	}
}
