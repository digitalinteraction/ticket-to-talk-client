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
		private Timer timer;
		private int zeroCount = 0;
		private Image play_img;
		private Image stop_img;
		private bool playing = false;
		private bool paused = false;
		private bool stopped = false;
		private string fileName;

		private int second_count;
		private int rawDuration;
		private Clock clock;
		private ProgressBar progressBar;

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

			private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AudioPlayerLayout"/> class.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public AudioPlayerLayout(Ticket ticket)
		{

			timer = new Timer();
			timer.Interval = 50;
			timer.Elapsed += Timer_Elapsed;

			clock = new Clock();

			fileName = ticket.pathToFile;

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
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			stop_img.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Stop_Clicked) });

			rawDuration = DependencyService.Get<IAudioPlayer>().GetDuration();
			var mins = rawDuration / 60;
			var seconds = rawDuration % 60;
			var durString = string.Empty;

			if (seconds < 10)
			{
				durString = string.Format("{0}:0{1}", mins, seconds);
			}
			else
			{
				durString = string.Format("{0}:{1}", mins, seconds);
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
				Padding = new Thickness(20, 0),
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

			StopPlayBack();

#if __IOS__
			MessagingCenter.Unsubscribe<TicketToTalk.iOS.AudioPlayerImplementation, bool>(this, "finshed_playback");
#else
			MessagingCenter.Unsubscribe<TicketToTalk.Droid.AudioPlayerImplementation, bool>(this, "finshed_playback");
#endif
		}

		/// <summary>
		/// Handles the play/pause button press.
		/// </summary>
		/// <returns>The clicked.</returns>
		void Play_Clicked()
		{

#if __IOS__
			MessagingCenter.Subscribe<TicketToTalk.iOS.AudioPlayerImplementation, bool>(this, "finished_playback", (sender, finished) =>
			{

				timer.Stop();


				clock.current_time = "0:00";
				second_count = 0;

				playing = false;
				paused = false;

				play_img.Source = "play_icon.png";

				DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);
				progressBar.ProgressTo(0, 250, Easing.Linear);

				MessagingCenter.Unsubscribe<TicketToTalk.iOS.AudioPlayerImplementation, bool>(this, "finshed_playback");

			});
#else
			MessagingCenter.Subscribe<TicketToTalk.Droid.AudioPlayerImplementation, bool>(this, "finished_playback", (sender, finished) =>
			{

				timer.Stop();

				clock.current_time = "0:00";
				second_count = 0;

				playing = false;
				paused = false;

				play_img.Source = "play_icon.png";

				DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);
				progressBar.ProgressTo(0, 250, Easing.Linear);

				MessagingCenter.Unsubscribe<TicketToTalk.Droid.AudioPlayerImplementation, bool>(this, "finshed_playback");

			});
#endif

			// Pause playback
			if (playing && !paused)
			{
				paused = true;
				timer.Stop();
				DependencyService.Get<IAudioPlayer>().PausePlayback();
				play_img.Source = "play_icon.png";
			}
			// Resume playback
			else if (playing && paused)
			{
				stopped = false;

				play_img.Source = "pause_icon.png";
				paused = false;
				timer.Start();
				DependencyService.Get<IAudioPlayer>().ResumePlayBack();
			}
			// Play track.
			else
			{
				stopped = false;

				play_img.Source = "pause_icon.png";

				DependencyService.Get<IAudioPlayer>().PlayAudioFile();
				timer.Start();

				progressBar.Progress = 0;
				playing = true;
				paused = false;
			}
		}

		/// <summary>
		/// Update the elapsed time on each timer interval.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			var time = DependencyService.Get<IAudioPlayer>().GetCurrentTime();
			second_count++;
			var mins = time / 60;
			var seconds = time % 60;

			if (seconds < 10)
			{
				clock.current_time = string.Format("{0}:0{1}", mins, seconds);
			}
			else
			{
				clock.current_time = string.Format("{0}:{1}", mins, seconds);
			}

			EPSILON = 0.05;
			var progress = ((double)time / rawDuration);

			if (Math.Abs(progress) < EPSILON)
			{
				progressBar.ProgressTo(0, 999, Easing.Linear);
				zeroCount++;
			}
			else if (Math.Abs(progress - 1) < EPSILON && !stopped)
			{
			}
			else
			{
				progressBar.ProgressTo(progress, 999, Easing.Linear);
				zeroCount = 0;
			}
		}

		/// <summary>
		/// Stops the play back.
		/// </summary>
		public void StopPlayBack()
		{
			playing = false;
			paused = false;
			play_img.Source = "play_icon.png";
			DependencyService.Get<IAudioPlayer>().StopPlayBack();
			DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);
			timer.Stop();
			clock.current_time = "0:00";
			second_count = 0;
			progressBar.ProgressTo(0, 250, Easing.Linear);
		}

		/// <summary>
		/// Checks the storage perms.
		/// </summary>
		/// <returns>The storage perms.</returns>
		private async Task<bool> CheckStoragePerms()
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

					return false;
				}

				return false;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.StackTrace);
				return false;
			}
		}
	}
}


