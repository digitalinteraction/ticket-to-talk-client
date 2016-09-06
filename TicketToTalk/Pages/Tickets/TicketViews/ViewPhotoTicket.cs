﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View photo ticket.
	/// </summary>
	public class ViewPhotoTicket : ContentPage
	{
		Ticket Ticket { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ViewPhotoTicket"/> class.
		/// </summary>
		public ViewPhotoTicket(Ticket ticket)
		{

			Ticket = ticket;
			this.Title = ticket.title;

			NavigationPage.SetHasNavigationBar(this, true);
			NavigationPage.SetHasBackButton(this, true);
		
			// Add button to navigation bar.
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(displayInfo)
			});

			var ticketController = new TicketController();
			var ticket_photo = ticketController.getTicketImage(Ticket);

			var stack = new StackLayout()
			{
				Spacing = 0,
				Children = 
				{
					ticket_photo,
					new TicketInfo()
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
		}

		/// <summary>
		/// Display ticket info
		/// </summary>
		/// <returns>The info.</returns>
		public async void displayInfo()
		{
			var action = await DisplayActionSheet("Ticket Options", "Cancel", "Delete", "Display Information", "Add to Conversation");

			switch (action) 
			{
				case ("Delete"):
					var ticketController = new TicketController();
					await Navigation.PopAsync();
					ticketController.destroyTicket(Ticket);
					break;
				case ("Display Information"):
					await Navigation.PushAsync(new DisplayTicketInfo(Ticket));
					break;
				case ("Add to Conversation"):
					await Navigation.PushModalAsync(new ConversationSelect(Ticket));
					break;
			}
		}
	}
}