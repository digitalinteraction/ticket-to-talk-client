using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets in area.
	/// </summary>
	public class DisplayTickets : ContentPage
	{

		public static ObservableCollection<Ticket> displayTickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.DisplayTickets"/> class.
		/// </summary>
		/// <param name="tickets">Tickets.</param>
		/// <param name="title">Title.</param>
		public DisplayTickets(List<Ticket> tickets, string title)
		{
			displayTickets.Clear();
			foreach (Ticket t in tickets)
			{
				displayTickets.Add(t);
			}

			// Set Padding
			Padding = new Thickness(20);
			Title = title;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(launchNewTicketView),
			});

			BackgroundColor = ProjectResource.color_white;

			var ticketsListView = new ListView();

			// Set display icons
			foreach (Ticket t in tickets)
			{
				switch (t.mediaType)
				{
					case "Photo":
					case "Picture":
						t.displayIcon = "photo_icon.png";
						break;
					case "Video":
					case "YouTube":
						t.displayIcon = "video_icon.png";
						break;
					case "Audio":
					case "Song":
						t.displayIcon = "audio_icon.png";
						break;
					case "Area":
						t.displayIcon = "area_icon.png";
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

			ticketsListView.SetBinding(ListView.ItemsSourceProperty, ".");
			ticketsListView.BindingContext = displayTickets;
			ticketsListView.ItemTemplate = cell;
			ticketsListView.ItemSelected += OnSelection;
			ticketsListView.SeparatorColor = Color.Transparent;

			Content = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					ticketsListView
				}
			};
		}

		/// <summary>
		/// On ticket selection, launch the view to see the ticket.
		/// </summary>
		/// <returns>The selection.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Ticket ticket = (Ticket)e.SelectedItem;
			ToolbarItems.Clear();

			var nav = new ViewTicket(ticket);

			Navigation.PushAsync(nav);
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launchs the new ticket view.
		/// </summary>
		/// <returns>The new ticket view.</returns>
		public void launchNewTicketView()
		{
			var nav = new NavigationPage(new SelectNewTicketType());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}
	}
}