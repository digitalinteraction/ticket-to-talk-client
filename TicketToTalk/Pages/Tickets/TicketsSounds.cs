using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// My page.
	/// </summary>
	public class TicketsSounds : TrackedContentPage
	{
        private ProgressSpinner indicator;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.MyPage"/> class.
		/// </summary>
		public TicketsSounds()
		{

            TrackedName = "Sound Tickets";

            indicator = new ProgressSpinner(this, ProjectResource.color_white, ProjectResource.color_dark);

			// Set Padding
			Padding = new Thickness(20);
			Title = "Sounds";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(LaunchNewTicketView)
			});

			BackgroundColor = ProjectResource.color_white;

			var ticketsListView = new ListView();

			// Set display icons
			foreach (Ticket t in ViewTickets.ticketSongs)
			{
				switch (t.mediaType)
				{
					case "Audio":
					case "Song":
					case "Sound":
						t.displayIcon = "audio_icon.png";
						break;
				}
			}

			// Format image cell
			var cell = new DataTemplate(typeof(TicketCell));
			cell.SetBinding(TextCell.TextProperty, "title");
			cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			//ticketsListView.ItemsSource = pictureTickets;
			ticketsListView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			ticketsListView.BindingContext = ViewTickets.ticketSongs;
			ticketsListView.ItemTemplate = cell;
			ticketsListView.ItemSelected += OnSelection;
			ticketsListView.SeparatorColor = Color.Transparent;


			var stack = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					ticketsListView
				}
			};

			var layout = new AbsoluteLayout();

			AbsoluteLayout.SetLayoutBounds(stack, new Rectangle(0.5, 0.5, 1, 1));
			AbsoluteLayout.SetLayoutFlags(stack, AbsoluteLayoutFlags.All);

			layout.Children.Add(stack);
			layout.Children.Add(indicator);

			Content = layout;
		}

		/// <summary>
		/// On ticket selection, launch the view to see the ticket.
		/// </summary>
		/// <returns>The selection.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Ticket ticket = (Ticket)e.SelectedItem;
			ToolbarItems.Clear();

			IsBusy = true;

			var nav = new ViewTicket(ticket);
			var ready = await nav.SetUpTicketForDisplay();

			if (ready)
			{
				IsBusy = false;

				await Navigation.PushAsync(nav);
			}
			else
			{
				IsBusy = false;

				await DisplayAlert("No Network", "Ticket could not be downloaded", "OK");
			}

			//Navigation.PushAsync(new ViewTicket(ticket));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launchs the new ticket view.
		/// </summary>
		/// <returns>The new ticket view.</returns>
		private void LaunchNewTicketView()
		{
			var nav = new NavigationPage(new SelectNewTicketType());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}
	}
}


