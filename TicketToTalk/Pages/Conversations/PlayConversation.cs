using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Play conversation.
	/// </summary>
	public class PlayConversation : TrackedContentPage
	{
		private StackLayout ticketStack;
		private List<Ticket> tickets;
		private Label ticketTitleLabel;
		private int currentIndex = 0;
		private Label descriptionLabel;
		private ContentView mediaContent = new ContentView();

		private Conversation conversation;

		private TicketController ticketController = new TicketController();

		private ConversationLog conversationLog;
		private List<TicketLog> ticketLogs;
		private TicketLog currentTicket;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PlayConversation"/> class.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		/// <param name="tickets">Tickets.</param>
		public PlayConversation(Conversation conversation, List<Ticket> tickets)
		{

            TrackedName = "Play Conversation";

			tickets.Shuffle();
			this.tickets = tickets;
			this.conversation = conversation;
			ticketLogs = new List<TicketLog>();
			conversationLog = new ConversationLog
			{
				ConversationId = conversation.id,
				Start = DateTime.Now
			};
			currentTicket = new TicketLog();

			Title = "Conversation";

			var contentPlaceholder = new ContentView();
			contentPlaceholder.SetBinding(ContentView.ContentProperty, "Content");
			contentPlaceholder.BindingContext = mediaContent;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Finish",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(FinishConversation)
			});

			var next = new Button
			{
				Text = "Next",
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.4
			};
            next.SetStyle();
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
            previous.SetStyle();
			previous.Clicked += Previous_Clicked;

			ticketStack = new StackLayout
			{
				Padding = new Thickness(0),
				Spacing = 0,
				Children =
				{
					contentPlaceholder
				}
			};

			ticketTitleLabel = new Label
			{
				Text = tickets[currentIndex].title,
				//TextColor = ProjectResource.color_dark,
				//FontSize = 18,
				//HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 10, 0, 0)
			};
            ticketTitleLabel.SetHeaderStyle();

			descriptionLabel = new Label
			{
				Text = tickets[currentIndex].description,
				//TextColor = ProjectResource.color_dark,
				//FontSize = 14,
				//HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 10, 0, 0)
			};
            descriptionLabel.SetBodyStyle();
            descriptionLabel.HorizontalTextAlignment = TextAlignment.Center;
            descriptionLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;

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

			SetTicketStack(tickets[currentIndex]);

			var content = new StackLayout
			{
				Children = {
					ticketStack,
					ticketTitleLabel,
					descriptionLabel,
					buttonStack
				}
			};

			Content = new ScrollView
			{
				Content = content
			};
		}

		/// <summary>
		/// Finishs the conversation.
		/// </summary>
		/// <param name="obj">Object.</param>
		private async void FinishConversation(object obj)
		{
			var nav = new NavigationPage(new FinishConversation(conversation));
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			ticketLogs[ticketLogs.Count - 1].Finish = DateTime.Now;

			conversationLog.Finish = DateTime.Now;
			conversationLog.TicketLogs = ticketLogs;

			var controller = new ConversationController();
			var stored = await controller.storeConversationLog(conversationLog);

			await Navigation.PushModalAsync(nav);
			Navigation.RemovePage(this);
		}

		/// <summary>
		/// Sets the ticket stack.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		private void SetTicketStack(Ticket ticket)
		{
			// Start ticket log.

			var tLog = new TicketLog()
			{
				TicketId = ticket.id,
				Start = DateTime.Now
			};
			ticketLogs.Add(tLog);

			switch (ticket.mediaType)
			{
				case ("Picture"):
				case ("Photo"):

					mediaContent.Content = new PictureLayout(ticket.pathToFile);

					break;
				case ("Sound"):
				case ("Song"):
				case ("Audio"):

					mediaContent.Content = new AudioPlayerLayout(ticket);

					break;
				case ("Video"):
				case ("YouTube"):

					mediaContent.Content = new YouTubePlayer(ticket.pathToFile);

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

			ticketLogs[ticketLogs.Count - 1].Finish = DateTime.Now;

			SetTicketStack(tickets[currentIndex]);
		}

		/// <summary>
		/// Go back to previous ticket.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Previous_Clicked(object sender, EventArgs e)
		{
			currentIndex--;
			currentIndex = currentIndex % tickets.Count;

			ticketLogs[ticketLogs.Count - 1].Finish = DateTime.Now;

			SetTicketStack(tickets[currentIndex]);
		}
	}
}


