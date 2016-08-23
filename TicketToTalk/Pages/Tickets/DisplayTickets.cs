using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets in area.
	/// </summary>
	public partial class DisplayTickets : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.DisplayTickets"/> class.
		/// </summary>
		/// <param name="tickets">Tickets.</param>
		/// <param name="title">Title.</param>
		public DisplayTickets(List<Ticket> tickets, string title)
		{
			// Set Padding
			Padding = new Thickness(20);
			Title = title;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(launchNewTicketView)
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

			ticketsListView.ItemsSource = tickets;
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

			Ticket ticket = (Ticket) e.SelectedItem;
			ToolbarItems.Clear();

			Navigation.PushAsync(new ViewTicket(ticket));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launchs the new ticket view.
		/// </summary>
		/// <returns>The new ticket view.</returns>
		public void launchNewTicketView()
		{
			Navigation.PushAsync(new SelectNewTicketType());
		}
	}
}