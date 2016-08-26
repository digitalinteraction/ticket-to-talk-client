﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Conversation view.
	/// </summary>
	public class ConversationView : ContentPage
	{
		public static ObservableCollection<ConversationItem> conversationItems = new ObservableCollection<ConversationItem>();
		List<Ticket> tickets;
		Conversation conversation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationView"/> class.
		/// </summary>
		/// <param name="conversation">Conversation.</param>
		public ConversationView(Conversation conversation)
		{
			this.conversation = conversation;
			conversationItems.Clear();
			Debug.WriteLine("ConversationView: Conversation = " + conversation);

			Title = conversation.displayDate;
			var dateLabel = new Label
			{
				Text = "Time",
				TextColor = ProjectResource.color_dark
			};

			char[] del = {' '};
			string[] dt = conversation.date.Split(del);
			del = new char[]{ ':' };
			string[] time = dt[1].Split(del);

			string hour = (Int32.Parse(time[0]) % 12).ToString();
			int afterMid = Int32.Parse(time[0]) / 12;

			if (Int32.Parse(hour) == 0 && afterMid == 1)
			{
				hour = "12";
			}

			var time_suffix = String.Empty;
			switch (afterMid)
			{
				case (0):
					time_suffix = "am";
					break;
				case (1):
					time_suffix = "pm";
					break;
				default:
					time_suffix = "";
					break;
			}

			var minutes = time[1];

			var date = new Label
			{
				Text = String.Format("{0}:{1} {2}", hour, minutes, time_suffix),
				TextColor = ProjectResource.color_red,
			};

			var notesLabel = new Label
			{
				Text = "Notes",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0),
			};

			var notes = new Label
			{
				Text = conversation.notes,
				TextColor = ProjectResource.color_red,
			};

			var ticketsLabel = new Label
			{
				Text = "Tickets",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0),
			};

			if (conversation.ticket_id_string != null)
			{
				char[] delims = { ' ' };
				string[] ticket_ids = conversation.ticket_id_string.Split(delims);
				foreach (string s in ticket_ids)
				{
					Debug.WriteLine("ConversationView: ticket_id " + s);
				}
				var ticketController = new TicketController();
				if (!(String.IsNullOrEmpty(ticket_ids[0])))
				{
					tickets = new List<Ticket>();
					foreach (string s in ticket_ids)
					{
						if (!(String.IsNullOrEmpty(s)))
						{
							var ticket = ticketController.getTicket(Int32.Parse(s));
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
					}
				}
			}

			// Format image cell
			var cell = new DataTemplate(typeof(ConversationItemCell));

			cell.SetBinding(TextCell.TextProperty, "title");
			cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			var ticketsListView = new ListView();
			ticketsListView.ItemsSource = conversationItems;
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
			startConversation.Clicked += StartConversation_Clicked;

			Content = new StackLayout
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
					ticketsListView,
					startConversation
				}
			};
		}

		/// <summary>
		/// Ons the selection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			var convItem = (ConversationItem)e.SelectedItem;
			Ticket ticket = convItem.ticket;

			switch (ticket.mediaType)
			{
				case ("Photo"):
					Navigation.PushAsync(new ViewPhotoTicket(ticket));
					break;

				// TODO: Implement type dependent views.
				case ("Video"):
					Navigation.PushAsync(new ViewTicket(ticket));
					break;
				case ("Song"):
					Navigation.PushAsync(new ViewAudioTicket(ticket));
					break;
				case ("Area"):
					Navigation.PushAsync(new ViewTicket(ticket));
					break;
			}
			//Navigation.PushAsync(new ViewTicket(ticket));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launch the conversation view when the button has been clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void StartConversation_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new PlayConversation(conversation, tickets));
		}
	}
}


