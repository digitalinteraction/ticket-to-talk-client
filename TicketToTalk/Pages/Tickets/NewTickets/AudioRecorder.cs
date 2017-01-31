using System;
using Plugin.Permissions;
using Xamarin.Forms;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TicketToTalk
{
	public class AudioRecorder : ContentPage
	{
		private bool recording = false;
		private Image record;
		private Label inf;
		private StackLayout labelStack;
		private Button saveAudio;
		private string fileName;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AudioRecorder"/> class.
		/// </summary>
		public AudioRecorder()
		{
			Title = "New Audio Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			inf = new Label
			{
				Text = "Tap the mic to start recording.",
				TextColor = ProjectResource.color_red,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			};

			record = new Image
			{
				Source = "mic_icon.png",
				HeightRequest = 50,
				WidthRequest = 50,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			record.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Record_Clicked) });

			saveAudio = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = (Session.ScreenWidth * 0.5),
				Margin = new Thickness(0, 0, 0, 10),
				IsEnabled = false
			};
			saveAudio.Clicked += Finished_Clicked;

			labelStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.End,
				Padding = new Thickness(0, 5),
				Spacing = 10,
				Children =
				{
					inf,
					saveAudio
				}
			};

			Content = new StackLayout
			{
				Children = {
					record,
					labelStack,
				}
			};

			var audioAccess = Task.Run(() => checkAudioPerms()).Result;
			var storageAccess = Task.Run(() => checkStoragePerms()).Result;

			if (!(audioAccess && storageAccess))
			{
				Navigation.PopAsync();
			}
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		private void Record_Clicked()
		{
			if (recording)
			{
				record.Source = "mic_icon.png";
				fileName = DependencyService.Get<IAudioRecorder>().FinishRecording();
				recording = false;
				inf.Text = "Finished Recording";
				saveAudio.BackgroundColor = ProjectResource.color_blue;
				saveAudio.IsEnabled = true;

			}
			else
			{
				record.Source = "stop_icon.png";
				recording = true;
				DependencyService.Get<IAudioRecorder>().Record("u_" + Session.activeUser.id + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav");
				inf.Text = "Recording...";
				saveAudio.BackgroundColor = ProjectResource.color_grey;
				saveAudio.IsEnabled = false;
			}
		}

		private void Finished_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new NewTicket("Sound", fileName));
			Navigation.RemovePage(this);
		}

		/// <summary>
		/// Checks the audio permissions.
		/// </summary>
		/// <returns>The audio perms.</returns>
		private async Task<bool> checkAudioPerms()
		{
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
					{
						await DisplayAlert("Microphone", "Ticket to Talk needs access to the microphone to record audio.", "OK");
					}
					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Microphone });
					status = results[Permission.Microphone];
				}

				if (status == PermissionStatus.Granted)
				{
					return true;
				}
				else if (status != PermissionStatus.Unknown)
				{
					await DisplayAlert("Microphone Denied", "Cannot record audio without microphone permissions.", "OK");
					return false;
				}
			}
			catch (Exception e)
			{
				return false;
			}

			return false;
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
						await DisplayAlert("Storage", "Ticket to Talk needs access to storage to save tickets.", "OK");
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
					await DisplayAlert("Storage Denied", "Cannot save tickets without access to audio.", "OK");
					return false;
				}
			}
			catch (Exception e)
			{
				return false;
			}

			return false;
		}
	}
}


