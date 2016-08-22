﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// My page.
	/// </summary>
	public class TicketsSounds : ContentPage
	{

		public static ObservableCollection<Ticket> soundTickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.MyPage"/> class.
		/// </summary>
		public TicketsSounds(List<Ticket> tickets)
		{
			soundTickets.Clear();
			foreach (Ticket t in tickets)
			{
				soundTickets.Add(t);
			}

			// Set Padding
			Padding = new Thickness(20);
			Title = "Sounds";

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
			foreach (Ticket t in soundTickets)
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
			ticketsListView.BindingContext = soundTickets;
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

			Navigation.PushAsync(new ViewAudioTicket(ticket));

			//Navigation.PushAsync(new ViewTicket(ticket));
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


