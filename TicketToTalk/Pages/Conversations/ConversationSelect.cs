using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Conversation select.
	/// </summary>
	public class ConversationSelect : ContentPage
	{
		private Button newConversation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationSelect"/> class.
		/// </summary>
		public ConversationSelect(Ticket ticket)
		{

			var conversationController = new ConversationController();
			var conversations = new ObservableCollection<Conversation>();

			var cs = Task.Run(() => conversationController.getRemoteConversations()).Result;

			//var rawConversations = conversationController.getConversations();
			foreach (Conversation converstaion in cs) 
			{
				conversations.Add(conversationController.setPropertiesForDisplay(converstaion));
			}

			newConversation = new Button 
			{
				Text = "New Conversation",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0,0,0,10)
			};

			var cell = new DataTemplate(typeof(ConversationCell));
			//cell.SetBinding(TextCell.TextProperty, "date");
			//cell.SetBinding(TextCell.DetailProperty, new Binding("year"));

			var listView = new ListView();
			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			listView.BindingContext = conversations;
			listView.SeparatorColor = Color.Transparent;
			listView.ItemTemplate = cell;
			listView.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
				}

				Conversation conversation = (Conversation)e.SelectedItem;

				Debug.WriteLine("ConversationSelect: conversation selected = " + conversation);

				await conversationController.addTicketToConversationRemotely(conversation, ticket);
				conversation = conversationController.addTicketToConversation(conversation, ticket);
				conversationController.updateConversationLocally(conversation);

				await Navigation.PopModalAsync();

				//Navigation.PushAsync(new ViewTicket(ticket));
				((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
			};


			Content = new StackLayout
			{
				Spacing = 10,
				Children = {
					new Label {
						Text = "Select a conversation",
						Margin = new Thickness(0,20,0,10),
						HorizontalOptions = LayoutOptions.CenterAndExpand
					},
					newConversation,
					listView
				}
			};
		}
	}
}


