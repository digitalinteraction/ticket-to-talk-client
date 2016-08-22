using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class AudioPlayerLayout : ContentView
	{
		Timer timer;

		Image play_img;
		Image stop_img;
		bool playing = false;
		bool paused = false;
		string fileName;

		private int second_count;
		private int rawDuration;
		Clock clock;
		ProgressBar progressBar;

		public double EPSILON { get; private set; }

		internal class Clock : INotifyPropertyChanged
		{
			private string _current_time = "0:00";
			public string current_time
			{
				get
				{
					return _current_time;
				}
				set
				{
					if (value != _current_time)
					{
						_current_time = value;
						NotifyPropertyChanged();
					}
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		public AudioPlayerLayout(Ticket ticket)
		{
			timer = new Timer();
			timer.Interval = 500;
			timer.Elapsed += Timer_Elapsed;

			clock = new Clock();

			fileName = ticket.pathToFile;

			//var storePerms = Device.BeginInvokeOnMainThread(() => checkStoragePerms());
			//if (storePerms) 
			//{
			//	Debug.WriteLine("Has store perms");
			//}

			DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);

			play_img = new Image
			{
				Source = "play_icon.png",
				HeightRequest = 50,
				WidthRequest = 50,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			play_img.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Play_Clicked) });

			stop_img = new Image
			{
				Source = "stop_icon.png",
				HeightRequest = 50,
				WidthRequest = 50,
				//IsEnabled = false,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			stop_img.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Stop_Clicked) });

			rawDuration = DependencyService.Get<IAudioPlayer>().GetDuration();
			var mins = rawDuration / 60;
			var seconds = rawDuration % 60;
			var durString = String.Empty;

			if (seconds < 10)
			{
				durString = String.Format("{0}:0{1}", mins, seconds);
			}
			else 
			{
				durString = String.Format("{0}:{1}", mins, seconds);
			}

			var duration = new Label
			{
				Text = durString,
				TextColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var current = new Label
			{
				TextColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			current.SetBinding(Label.TextProperty, "current_time");
			current.BindingContext = clock;

			progressBar = new ProgressBar 
			{
				Progress = 0
			};

			var controlStack = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Margin = new Thickness(0, 20),
				Children =
				{
					play_img,
					stop_img
				}
			};

			var infoStack = new StackLayout
			{
				Padding = new Thickness(20),
				Orientation = StackOrientation.Horizontal,
				Children =
				{
					current,
					duration
				}
			};

			var progStack = new StackLayout
			{
				Padding = new Thickness(20,0),
				Children = 
				{
					progressBar
				}
			};

			var content = new StackLayout
			{
				Children =
				{
					controlStack,
					progStack,
					infoStack
				}
			};
			Content = content;
		}

		/// <summary>
		/// Stops audio playback.
		/// </summary>
		/// <returns>The clicked.</returns>
		void Stop_Clicked()
		{
			Debug.WriteLine("AudioPlayerLayout: Stop clicked");
			stopPlayBack();
		}

		/// <summary>
		/// Handles the play/pause button press.
		/// </summary>
		/// <returns>The clicked.</returns>
		void Play_Clicked()
		{
			// Pause playback
			if (playing && !paused)
			{
				paused = true;
				timer.Stop();
				DependencyService.Get<IAudioPlayer>().PausePlayback();
				play_img.Source = "play_icon.png";
				Debug.WriteLine("AudioPlayerLayout: Pausing playback");
			}
			// Resume playback
			else if (playing && paused)
			{
				play_img.Source = "pause_icon.png";
				paused = false;
				timer.Start();
				DependencyService.Get<IAudioPlayer>().ResumePlayBack();

				Debug.WriteLine("AudioPlayerLayout: Resuming playback");
			}
			// Play track.
			else
			{
				play_img.Source = "pause_icon.png";
				//stop_img.IsEnabled = true;

				DependencyService.Get<IAudioPlayer>().PlayAudioFile();
				progressBar.Progress = 0;
				playing = true;
				paused = false;

				timer.Start();

				Debug.WriteLine("AudioPlayerLayout: Starting playback");
			}
		}

		/// <summary>
		/// Update the elapsed time on each timer interval.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Debug.WriteLine("AudioPlayerLayout: Timer");
			var time = DependencyService.Get<IAudioPlayer>().GetCurrentTime();
			Debug.WriteLine("AudioPlayerLayout: Timer - " + time);
			second_count++;
			var mins = time / 60;
			var seconds = time % 60;

			if (seconds < 10)
			{
				clock.current_time = String.Format("{0}:0{1}", mins, seconds);
			}
			else 
			{
				clock.current_time = String.Format("{0}:{1}", mins, seconds);
			}

			EPSILON = 0.05;
			var progress = ((double)time / (double)rawDuration);
			//if (Math.Abs(progress) < EPSILON)
			//{
			//	stopPlayBack();
			//	progressBar.ProgressTo(1, 999, Easing.Linear);
			//}
			Debug.WriteLine("AudioPlayerLayout: Progress = " + progress);
			if (progress == 0)
			{
				progressBar.ProgressTo(1, 999, Easing.Linear);
				//stopPlayBack();
			}
			else 
			{
				progressBar.ProgressTo(progress, 999, Easing.Linear);
			}
		}

		/// <summary>
		/// Stops the play back.
		/// </summary>
		private void stopPlayBack() 
		{
			Debug.WriteLine("AudioPlayerLayout: playback button pressed.");
			playing = false;
			paused = false;
			play_img.Source = "play_icon.png";
			DependencyService.Get<IAudioPlayer>().StopPlayBack();
			DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);
			timer.Stop();
			clock.current_time = "0:00";
			second_count = 0;
			Debug.WriteLine("AudioPlayer: Stopping playback");
			//progressBar.ProgressTo(0, 250, Easing.Linear);
		}

		/// <summary>
		/// Checks the storage perms.
		/// </summary>
		/// <returns>The storage perms.</returns>
		private async Task<bool> checkStoragePerms()
		{
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
					{
						//await DisplayAlert("Storage", "Ticket to Talk needs access to storage to save tickets.", "OK");
					}
					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
					status = results[Permission.Storage];
				}

				if (status == PermissionStatus.Granted)
				{
					
					return true;
				}
				else if (status != PermissionStatus.Unknown)
				{
					//await DisplayAlert("Storage Denied", "Cannot save tickets without access to audio.", "OK");
					return false;
				}

				return false;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return false;
			}
		}
	}
}


