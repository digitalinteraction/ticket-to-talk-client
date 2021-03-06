// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 06/09/2016
//
// SelectTicket.cs
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Select a ticket to add to a conversation.
	/// </summary>
	public class SelectTicket : TrackedContentPage
	{

		private TicketController ticketController = new TicketController();
		private Conversation conversation;
		private ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectTicket"/> class.
		/// </summary>
		public SelectTicket(Conversation conversation)
		{

            TrackedName = "Select Ticket";

			this.conversation = conversation;

			Title = "Select a Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			var listView = new ListView();

            var cell = new DataTemplate(typeof(StyledTicketCell));
			//cell.SetBinding(TextCell.TextProperty, "title");
			//cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			//cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			//cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			//cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			listView.SetBinding(ListView.ItemsSourceProperty, ".");
			listView.BindingContext = tickets;
			listView.ItemTemplate = cell;
			listView.ItemSelected += OnSelection;
			listView.SeparatorColor = Color.Transparent;
            listView.HasUnevenRows = true;


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
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// On ticket selection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			var conversationController = new ConversationController();
			Ticket ticket = (Ticket)e.SelectedItem;

			var added = await conversationController.AddTicketToConversationRemotely(conversation, ticket);
			if (added)
			{
				conversationController.AddTicketToConversation(conversation, ticket);
				conversationController.UpdateConversationViews(conversation);
				conversationController.AddTicketToDisplayedConversation(conversation, ticket);

				await Navigation.PopModalAsync();
			}
			else
			{
				await DisplayAlert("Conversation", "Ticket could not be added to the conversation", "OK");
				await Navigation.PopModalAsync();
			}
		}

		/// <summary>
		/// Sets up page.
		/// </summary>
		/// <returns>The up page.</returns>
		public async Task<bool> SetUpPage() 
		{

			try
			{
				await Task.Run(() => ticketController.GetRemoteTickets());
			}
			catch (NoNetworkException ex)
			{
				Debug.WriteLine(ex);
			}

			var local_tickets = ticketController.GetTickets();
			local_tickets.Sort();

			foreach (Ticket t in local_tickets)
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
					case "Sound":
						t.displayIcon = "audio_icon.png";
						break;
					case "Area":
						t.displayIcon = "area_icon.png";
						break;
				}
                t.imageSource = ImageSource.FromFile(t.displayIcon);
				tickets.Add(t);
			}

			return true;
		}
	}
}


