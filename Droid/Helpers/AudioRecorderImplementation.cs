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
			//activity = Forms.Context as MainActivity;
			//activity = Android.App.Application.Context as MainActivity;
			//Debug.WriteLine("AndroidAudioRecorder: " + activity.PackageCodePath);

			//string[] recoderPermissions =
			//{
			//	Manifest.Permission.RecordAudio,
			//	Manifest.Permission.ReadExternalStorage,
			//	Manifest.Permission.WriteExternalStorage
			//};

			//if ((int)Android.OS.Build.VERSION.SdkInt >= 23)
			//{
			//	if (activity.CheckSelfPermission(recoderPermissions[0]) != (Permission.Granted))
			//	{
			//		Toast.MakeText(activity, "Audio recorder permission is required!", ToastLength.Long).Show();
			//		activity.RequestPermissions(recoderPermissions, requestAudioId);
			//	}
			//	else
			//	{
			//		return;
			//	}
			//}

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

		//private void record() 
		//{
		//	var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
		//	path = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath + "/" + fileName;

		//	Debug.WriteLine("AndroidAudioRecorder: path = " + path);
		//	_recorder.SetAudioSource(AudioSource.Mic);
		//	_recorder.SetOutputFormat(OutputFormat.ThreeGpp);
		//	_recorder.SetAudioEncoder(AudioEncoder.AmrNb);
		//	_recorder.SetOutputFile(path);
		//	_recorder.Prepare();
		//	_recorder.Start();
		//}

		public string FinishRecording()
		{
			_recorder.Stop();
			_recorder.Reset();
			_recorder.Release();
			return fileName;
		}

		//public async void PermissionsResultCallback([GeneratedEnum] Permission[] grantResults)
		//{
		//	if (grantResults[0] == Permission.Granted)
		//	{
		//		record();
		//	}
		//	else
		//	{
		//		PermissionsRefused();
		//	}
		//}

		//private async void PermissionsRefused()
		//{
		//	Toast.MakeText(activity, "Ticket to Talk needs audio recorder permissions", ToastLength.Long).Show();
		//}

	}


}




