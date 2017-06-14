using System;
using System.Diagnostics;
using Plugin.Media;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Select new ticket type.
	/// </summary>
	public class SelectNewTicketType : TrackedContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectNewTicketType"/> class.
		/// </summary>
		public SelectNewTicketType()
		{

            TrackedName = "Select New Ticket Type";

			Title = "New Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

            var photoCell = new T3ImageCell("Picture", "Take or Choose a Picture", ImageSource.FromFile("photo_icon.png"));
            photoCell.Tapped += PhotoButton_Clicked;

            var audioCell = new T3ImageCell("Sound", "Record a Sound", ImageSource.FromFile("audio_icon.png"));
			audioCell.Tapped += SongButton_Clicked;

            var youTubeCell = new T3ImageCell("Video", "Add a YouTube Video", ImageSource.FromFile("video_icon.png"));
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
			var cameraController = new CameraController();

			cameraController.MediaReady += async (f) => 
			{

                if (f != null) 
                {
					var page = new NewTicket("Picture", f.Path);

					try
					{
						var nav = new NavigationPage(page);
						nav.SetNavHeaders();
						await Navigation.PushModalAsync(nav);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error taking picture");
                        Debug.WriteLine(ex);
					}
                }
				
			};

			var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
			switch (action)
			{
				case ("Take a Photo"):
					await cameraController.TakePicture("temp_ticket");
					break;
				case ("Select a Photo From Library"):
					await cameraController.SelectPicture();
					break;
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

