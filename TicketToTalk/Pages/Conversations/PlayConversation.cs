using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Play conversation.
	/// </summary>
	public class PlayConversation : ContentPage
	{
		StackLayout ticketStack;
		List<Ticket> tickets;
		Label ticketTitleLabel;
		int currentIndex = 0;
		Label descriptionLabel;

		TicketController ticketController = new TicketController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PlayConversation"/> class.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		/// <param name="tickets">Tickets.</param>
		public PlayConversation(Conversation conversation, List<Ticket> tickets)
		{
			tickets.Shuffle();
			this.tickets = tickets;
			
			Title = "Conversation";

			var next = new Button
			{
				Text = "Next",
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.4
			};
			next.Clicked += Next_Clicked;

			var previous = new Button
			{
				Text = "Previous",
				BackgroundColor = ProjectResource.color_dark,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.4
			};
			previous.Clicked += Previous_Clicked;

			ticketStack = new StackLayout
			{
				Padding = new Thickness(0),
				Spacing = 0,
			};

			ticketTitleLabel = new Label 
			{
				Text = tickets[currentIndex].title,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0,10,0,0)
			};

			descriptionLabel = new Label 
			{
				Text = tickets[currentIndex].description,
				TextColor = ProjectResource.color_dark,
				FontSize = 14,
				HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var buttonStack = new StackLayout 
			{
				Padding = new Thickness(10),
				Spacing = 5,
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children = 
				{
					previous, 
					next
				}
			};

			setTicketStack(tickets[currentIndex]);

			Content = new StackLayout
			{
				Children = {
					ticketStack,
					ticketTitleLabel,
					descriptionLabel,
					buttonStack
				}
			};
		}

		/// <summary>
		/// Sets the ticket stack.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		private void setTicketStack(Ticket ticket) 
		{
			Debug.WriteLine("PlayConversation: Ticket to display = " + ticket);
			switch (ticket.mediaType) 
			{
				case ("Picture"):
				case ("Photo"):

					var image = ticketController.getTicketImage(ticket);

					ticketStack.Children.Clear();
					ticketStack.Children.Add(image);
					break;
				case ("Sound"): 
				case ("Song"):
				case ("Audio"):
					
					var audioPlayer = new AudioPlayerLayout(ticket);
					ticketStack.Children.Clear();
					ticketStack.Children.Add(audioPlayer);
					break;
				case ("Video"):
					// TODO Display web player
					break;
			}

			ticketTitleLabel.Text = ticket.title;
			descriptionLabel.Text = ticket.description;
		}

		/// <summary>
		/// Go to next ticket.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Next_Clicked(object sender, EventArgs e)
		{
			currentIndex++;
			currentIndex = currentIndex % tickets.Count;

			Debug.WriteLine("PlayConversation: Index of next ticket" + currentIndex);

			setTicketStack(tickets[currentIndex]);
		}

		/// <summary>
		/// Go back to previous ticket.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Previous_Clicked(object sender, EventArgs e)
		{
			currentIndex--;
			currentIndex = currentIndex % tickets.Count;
			setTicketStack(tickets[currentIndex]);
		}
	}
}


