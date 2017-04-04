using System;
using System.Diagnostics;
using Plugin.Media;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Select new ticket type.
	/// </summary>
	public class SelectNewTicketType : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectNewTicketType"/> class.
		/// </summary>
		public SelectNewTicketType()
		{

			Title = "New Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			var photoCell = new ImageCell
			{
				Text = "Picture",
				Detail = "Add or Select a Picture",
				DetailColor = ProjectResource.color_blue,
				ImageSource = "photo_icon.png",
			};
			photoCell.Tapped += PhotoButton_Clicked;

			var audioCell = new ImageCell
			{
				Text = "Sound",
				Detail = "Record a Sound",
				DetailColor = ProjectResource.color_blue,
				ImageSource = "audio_icon.png",
			};
			audioCell.Tapped += SongButton_Clicked;

			//var videoCell = new ImageCell
			//{
			//	Text = "Video",
			//	Detail = "Select a Video",
			//	DetailColor = ProjectResource.color_blue,
			//	ImageSource = "video_icon.png",
			//};

			var youTubeCell = new ImageCell
			{
				Text = "YouTube",
				Detail = "Add a YouTube Video",
				DetailColor = ProjectResource.color_blue,
				ImageSource = "video_icon.png",
			};
			youTubeCell.Tapped += YouTubeCell_Tapped;

			var table = new TableView
			{
				Root = new TableRoot { },
				Intent = TableIntent.Menu,
				HasUnevenRows = true,
				RowHeight = 60,
			};
			var section = new TableSection("Select a Ticket Type")
			{
				photoCell,
				audioCell,
				//videoCell,
				youTubeCell
			};
			table.Root.Add(section);

			Content = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					table,
				}
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Songs the button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SongButton_Clicked(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new AudioRecorder());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;
			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// On photos button click.
		/// </summary>
		/// <returns>The button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void PhotoButton_Clicked(object sender, EventArgs e)
		{
			var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
			switch (action)
			{
				case ("Take a Photo"):
					TakePicture();
					break;
				case ("Select a Photo From Library"):
					SelectPicture();
					break;
			}
		}

		/// <summary>
		/// Selects the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		private async void TakePicture()
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", "No camera avaialble.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{

				Directory = "TicketToTalk",
				Name = "ticket.jpg"
			});

			// App will not progress to new ticket screen on android without this...
			await DisplayAlert("File Location", "Photo Added!", "OK");

			var page = new NewTicket("Picture", file.Path);

			try
			{
				var nav = new NavigationPage(page);
				nav.BarTextColor = ProjectResource.color_white;
				nav.BarBackgroundColor = ProjectResource.color_blue;
				Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(nav));
				Navigation.RemovePage(this);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error taking picture");
				Debug.WriteLine(ex.StackTrace);
			}
		}

		/// <summary>
		/// Selects a picture from the library.
		/// </summary>
		public async void SelectPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Select Photo", "Photo select not supported", "OK");
				return;
			}

			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file == null) { return; }

			// App will not progress to new ticket screen on android without this...
			await DisplayAlert("File Location", "Photo Added!", "OK");

			Debug.WriteLine("SelectNewTicketType: File path = " + file.Path);
			var page = new NewTicket("Picture", file.Path);

			try
			{
				//var nav = new NavigationPage(page);
				//nav.BarTextColor = ProjectResource.color_white;
				//nav.BarBackgroundColor = ProjectResource.color_blue;
				//Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(nav));
				//Navigation.RemovePage(this);

				var nav = new NavigationPage(page);
				nav.SetNavHeaders();
				await Navigation.PushModalAsync(nav);
				//Navigation.RemovePage(this);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Yous the tube cell tapped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void YouTubeCell_Tapped(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new AddYoutubeLinkView());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}
	}
}

