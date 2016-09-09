// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 06/09/2016
//
// SelectTicket.cs
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Select a ticket to add to a conversation.
	/// </summary>
	public class SelectTicket : ContentPage
	{

		TicketController ticketController = new TicketController();
		Conversation conversation;
		ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectTicket"/> class.
		/// </summary>
		public SelectTicket(Conversation conversation)
		{
			this.conversation = conversation;

			Title = "Select a Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			var rawTickets = ticketController.getTickets();
			rawTickets.Sort();

			foreach (Ticket t in rawTickets)
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

				tickets.Add(t);
			}

			var listView = new ListView();

			var cell = new DataTemplate(typeof(ImageCell));
			cell.SetBinding(TextCell.TextProperty, "title");
			cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			listView.SetBinding(ListView.ItemsSourceProperty, ".");
			listView.BindingContext = tickets;
			listView.ItemTemplate = cell;
			listView.ItemSelected += OnSelection;
			listView.SeparatorColor = Color.Transparent;


			Content = new StackLayout
			{
				Padding = new Thickness(20),
				Children = {
					listView			
				}
			};
		}

		/// <summary>
		/// Dismiss the page.
		/// </summary>
		void cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// On ticket selection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			var conversationController = new ConversationController();
			Ticket ticket = (Ticket)e.SelectedItem;

			var added = await conversationController.addTicketToConversationRemotely(conversation, ticket);
			if (added)
			{
				conversationController.addTicketToConversation(conversation, ticket);
				conversationController.updateConversationLocally(conversation);
				conversationController.addTicketToDisplayedConversation(conversation, ticket);

				await Navigation.PopModalAsync();
			}
			else 
			{
				await DisplayAlert("Conversation", "Ticket could not be added to the conversation", "OK");
				await Navigation.PopModalAsync();
			}
		}
	}
}


