// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/08/2016
//
// AddYoutubeLinkView.cs

using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Add a youtube link view.
	/// </summary>
	public class AddYoutubeLinkView : TrackedContentPage
	{
		private Button button;
		private Entry link;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AddYoutubeLinkView"/> class.
		/// </summary>
		public AddYoutubeLinkView()
		{

            TrackedName = "Add YouTube Video";

			Title = "Add a Video";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			var description = new Label
			{
				Text = "Add a URL to YouTube Video",
				Margin = new Thickness(0, 10, 0, 0)
			};
            description.SetSubHeaderStyle();
            description.VerticalOptions = LayoutOptions.Start;

			link = new Entry
			{
				Placeholder = "https://youtube.com/7642u34h2",
				TextColor = ProjectResource.color_red,
				WidthRequest = Session.ScreenWidth * 0.8
			};
            link.SetStyle();
            link.VerticalOptions = LayoutOptions.Start;
			link.TextChanged += Link_TextChanged;

			button = new Button
			{
				Text = "Add Video",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.Center,
				WidthRequest = (Session.ScreenWidth * 0.5),
				IsEnabled = false,
				BackgroundColor = ProjectResource.color_grey
			};
            button.SetStyle();
            button.VerticalOptions = LayoutOptions.End;
			button.Clicked += Button_Clicked;

            var content = new StackLayout
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(20, 10, 20, 20),
				Children = {
					description,
					link
				}
			};

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					content,
					button
				}
			};
		}

		void cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Links the text changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Link_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!(string.IsNullOrEmpty(link.Text)))
			{
				button.IsEnabled = true;
				button.BackgroundColor = ProjectResource.color_blue;
			}
			else
			{
				button.BackgroundColor = ProjectResource.color_grey;
				button.IsEnabled = false;
			}
		}

		/// <summary>
		/// Parse the youtube link when the button is pressed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Button_Clicked(object sender, EventArgs e)
		{
			var ticketController = new TicketController();
			var ticket = ticketController.ParseYouTubeToTicket(link.Text);

			Navigation.PushAsync(new NewTicket(ticket));
		}
	}
}


