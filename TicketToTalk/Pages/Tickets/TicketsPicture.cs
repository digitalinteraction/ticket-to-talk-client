﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets picture.
	/// </summary>
	public class TicketsPicture : ContentPage
	{
		
		public static ObservableCollection<Ticket> pictureTickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsPicture"/> class.
		/// </summary>
		public TicketsPicture(List<Ticket> tickets)
		{
			pictureTickets.Clear();
			foreach (Ticket t in tickets) 
			{
				pictureTickets.Add(t);
			}
			
			// Set Padding
			Padding = new Thickness(20);
			Title = "Pictures";

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
			foreach (Ticket t in pictureTickets)
			{
				switch (t.mediaType)
				{
					case "Photo":
						t.displayIcon = "photo_icon.png";
						break;
					case "Picture":
						t.displayIcon = "photo_icon.png";
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
			ticketsListView.BindingContext = pictureTickets;
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
				
			Navigation.PushAsync(new ViewPhotoTicket(ticket));

			//Navigation.PushAsync(new ViewTicket(ticket));
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


