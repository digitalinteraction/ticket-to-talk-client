using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Conversation view.
	/// </summary>
	public class ConversationView : TrackedContentPage
	{
		public static bool tutorialShown = false;

		public static ObservableCollection<ConversationItem> conversationItems = new ObservableCollection<ConversationItem>();
		public static List<Ticket> tickets = new List<Ticket>();
		public static Conversation conversation;
		private ConversationController conversationController = new ConversationController();
		private ProgressSpinner indicator;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationView"/> class.
		/// </summary>
		/// <param name="conv">Conversation.</param>
		public ConversationView(Conversation conv)
		{

            TrackedName = "Conversation View";

			indicator = new ProgressSpinner(this, ProjectResource.color_white_transparent, ProjectResource.color_dark);

			conversation = conv;
			conversationItems.Clear();
			tickets.Clear();

			// Wait for new ticket to be returned if added through this view.
			//MessagingCenter.Subscribe<NewTicketInfo, Ticket>(this, "ticket_added", async (sender, returned_ticket) =>
			//{
			//	var added = await conversationController.AddTicketToConversationRemotely(conv, returned_ticket);
			//	if (added)
			//	{
			//		conversationController.AddTicketToConversation(conv, returned_ticket);
			//		conversationController.UpdateConversationViews(conv);
			//		conversationController.AddTicketToDisplayedConversation(conv, returned_ticket);
			//	}
			//});

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(ConversationOptions)
			});

			// Set title
			this.SetBinding(TitleProperty, "displayDate");
			this.BindingContext = conversation;

			var dateLabel = new Label
			{
				Text = "Time",
			};
            dateLabel.SetSubHeaderStyle();

			var date = new Label
			{
				Text = conversation.displayDate,
			};
            date.SetBodyStyle();
            date.TextColor = ProjectResource.color_red;
			date.SetBinding(Label.TextProperty, new Binding(path: "timestamp", stringFormat: "{0:hh:mm tt}" ));
			date.BindingContext = conversation;

			var notesLabel = new Label
			{
				Text = "Notes",
				Margin = new Thickness(0, 10, 0, 0)
			};
            notesLabel.SetSubHeaderStyle();

			var notes = new Label
			{
			};
            notes.SetBodyStyle();
            notes.TextColor = ProjectResource.color_red;
			notes.SetBinding(Label.TextProperty, "notes");
			notes.BindingContext = conversation;

			var ticketsLabel = new Label
			{
				Text = "Tickets",
				Margin = new Thickness(0, 10, 0, 0)
			};
            ticketsLabel.SetHeaderStyle();

			var newTicketLabel = new Label
			{
				Text = "Add a Ticket"
			};
            newTicketLabel.SetSubHeaderStyle();

			var newTicketIcon = new Image()
			{
				Source = "red_add.png",
				HeightRequest = 30,
				WidthRequest = 30,
				HorizontalOptions = LayoutOptions.EndAndExpand
			};
			newTicketIcon.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(NewTicket) });

			var newStack = new StackLayout
			{
				Padding = 10,
				Orientation = StackOrientation.Horizontal,
				Spacing = 0,
				HorizontalOptions = LayoutOptions.Fill,
				Children =
				{
					newTicketLabel,
					newTicketIcon
				}
			};

			// Format image cell
			var cell = new DataTemplate(typeof(ConversationItemCell));

			cell.SetBinding(TextCell.TextProperty, "title");
			cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			var ticketsListView = new ListView();

			ticketsListView.SetBinding(ListView.ItemsSourceProperty, ".");
			ticketsListView.BindingContext = conversationItems;

			ticketsListView.ItemTemplate = cell;
			ticketsListView.ItemSelected += OnSelection;
			ticketsListView.SeparatorColor = Color.Transparent;

			var startConversation = new Button
			{
				Text = "Start Conversation",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5
			};
            startConversation.SetStyle();
			startConversation.Clicked += StartConversation_Clicked;

			var stack = new StackLayout
			{
				Padding = new Thickness(20),
				Spacing = 5,
				Children =
				{
					dateLabel,
					date,
					notesLabel,
					notes,
					ticketsLabel,
					newStack,
					ticketsListView,
					startConversation
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
		/// Add a new ticket to the conversation.
		/// </summary>
		private async void NewTicket()
		{
			var action = await DisplayActionSheet("Add a New Ticket", "Cancel", null, "Add an Existing Ticket");

			switch (action)
			{
				//case "Create a New Ticket":
				//	var nav = new NavigationPage(new SelectNewTicketType());
				//	nav.SetNavHeaders();

				//	await Navigation.PushModalAsync(nav);

				//	break;
				case "Add an Existing Ticket":

					IsBusy = true;

					var page = new SelectTicket(conversation);
					var ready = await page.SetUpPage();

					if (ready) 
					{
						var nav = new NavigationPage(page);
						nav.SetNavHeaders();

						IsBusy = false;
						await Navigation.PushModalAsync(nav);
					}

					break;
			}
		}

		/// <summary>
		/// Ons the selection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			IsBusy = true;

			var convItem = (ConversationItem)e.SelectedItem;
			Ticket ticket = convItem.ticket;

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


			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launch the conversation view when the button has been clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void StartConversation_Clicked(object sender, EventArgs e)
		{
			if (!(string.IsNullOrEmpty(conversation.ticket_id_string)))
			{
				IsBusy = true;

				var ticketController = new TicketController();

				foreach (Ticket t in tickets) 
				{
					if (t.pathToFile.StartsWith("ticket_to_talk", StringComparison.Ordinal))
					{
						try
						{
							await Task.Run(() => ticketController.DownloadTicketContent(t));

							var ext = t.pathToFile.Substring(t.pathToFile.LastIndexOf('.'));
							t.pathToFile = string.Format("t_{0}{1}", t.id, ext);

						}
						catch (NoNetworkException ex)
						{
							tickets.Remove(t);
							Debug.WriteLine(ex);
						}
					}
				}

				IsBusy = false;

				await Navigation.PushAsync(new PlayConversation(conversation, tickets));
			}
			else
			{
				IsBusy = false;

				await DisplayAlert("Start Conversation", "You need to add tickets to the conversation before starting.", "OK");
			}
		}

		/// <summary>
		/// On back button pressed.
		/// </summary>
		/// <returns><c>true</c>, if back button pressed was oned, <c>false</c> otherwise.</returns>
		protected override bool OnBackButtonPressed()
		{
			MessagingCenter.Unsubscribe<NewTicketInfo, Ticket>(this, "ticket_added");

			return base.OnBackButtonPressed();
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !tutorialShown)
			{

				var text = "This is a conversation, you can add tickets by pressing the add ticket button and view more options by pressing the 'i' button.\n\nStart a conversation by pressing start!";

				Navigation.PushModalAsync(new HelpPopup(text, "chat_white_icon.png"));
				tutorialShown = true;
			}
		}

		public async Task<bool> SetUpConversation() 
		{
			var ticketController = new TicketController();

			tickets.Clear();
			conversationItems.Clear();

			if (conversation.ticket_id_string == null || conversation.ticket_id_string.Trim() == "") 
			{
				return true;
			}

			// Get tickets in conversation

			// Get tickets from server if connection
			List<Ticket> r_tickets = null;

			try
			{
				var s_tickets = await conversationController.getTicketsInConversationFromAPI(conversation);

				for (int i = 0; i < s_tickets.Count; i++) 
				{
					var localTicket = ticketController.GetTicket(s_tickets[i].id);
					if (localTicket != null) 
					{
						s_tickets[i] = localTicket;
					}
				}

				r_tickets = s_tickets;

			}
			// Get tickets locally if no connection
			catch (NoNetworkException ex)
			{
				Debug.WriteLine(ex);

				r_tickets = conversationController.getTicketsInConversationLocally(conversation);
			}

			// Download all ticket content
			foreach (Ticket t in r_tickets) 
			{
				if (t.pathToFile.StartsWith("ticket_to_talk", StringComparison.Ordinal)) 
				{
					try
					{
						await Task.Run(() => ticketController.DownloadTicketContent(t));

						var ext = t.pathToFile.Substring(t.pathToFile.LastIndexOf('.'));
						t.pathToFile = string.Format("t_{0}{1}", t.id, ext);

					}
					catch (NoNetworkException ex)
					{
						r_tickets.Remove(t);
						Debug.WriteLine(ex);
					}
				}
			}

			// Exit if network exception

			// Add tickets to conversation
			foreach (Ticket ticket in r_tickets) 
			{
				tickets.Add(ticket);

				switch (ticket.mediaType)
				{
					case "Photo":
					case "Picture":
						ticket.displayIcon = "photo_icon.png";
						break;
					case "Video":
					case "YouTube":
						ticket.displayIcon = "video_icon.png";
						break;
					case "Sound":
					case "Song":
						ticket.displayIcon = "audio_icon.png";
						break;
					case "Area":
						ticket.displayIcon = "area_icon.png";
						break;
				}

				var convItem = new ConversationItem(conversation, ticket);
				conversationItems.Add(convItem);
			}

			return true;
		}

		/// <summary>
		/// Conversations the options.
		/// </summary>
		private async void ConversationOptions()
		{
			var action = await DisplayActionSheet("Edit Conversation", "Cancel", "Delete", "Edit");

			switch (action)
			{
				case "Delete":

					try
					{
						await conversationController.DestroyConversation(conversation);
					}
					catch (NoNetworkException ex)
					{
						await DisplayAlert("No Network", ex.Message, "Dismiss");
					}

					break;
				case "Edit":
					var nav = new NavigationPage(new NewConversation(conversation));
					nav.SetNavHeaders();

					await Navigation.PushModalAsync(nav);
					break;
			}
		}

		/// <summary>
		/// Ons the disappearing.
		/// </summary>
		protected override void OnDisappearing()
		{
			//MessagingCenter.Unsubscribe<NewTicketInfo, Ticket>(this, "ticket_added");
		}
	}
}


