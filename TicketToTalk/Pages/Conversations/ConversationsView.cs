using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Conversations view.
	/// </summary>
	public class ConversationsView : ContentPage
	{

		public static ObservableCollection<Conversation> conversations = new ObservableCollection<Conversation>();
		ConversationController conversationController = new ConversationController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationsView"/> class.
		/// </summary>
		public ConversationsView()
		{
			Padding = new Thickness(20);
			conversations.Clear();

			var cs = Task.Run(() => conversationController.getRemoteConversations()).Result;

			Title = "Converations";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(addConversation)
			});

			foreach (Conversation c in cs) 
			{
				conversations.Add(conversationController.setPropertiesForDisplay(c));
			}

			//var cell = new DataTemplate(typeof(ConversationCell));
			//cell.SetBinding(TextCell.TextProperty, "date");
			//cell.SetBinding(TextCell.DetailProperty, new Binding("year"));

			var listView = new ListView();
			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			listView.HasUnevenRows = true;
			listView.RowHeight = 50;
			listView.BindingContext = conversations;
			listView.SeparatorColor = Color.Transparent;
			listView.ItemTemplate = new DataTemplate(typeof (ConversationCell));
			listView.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
				}

				Conversation conversation = (Conversation)e.SelectedItem;
				await Navigation.PushAsync(new ConversationView(conversation));

				//Navigation.PushAsync(new ViewTicket(ticket));
				((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
			};

			Content = new StackLayout
			{
				Children = {
					listView
				}
			};
		}

		/// <summary>
		/// Adds the conversation.
		/// </summary>
		/// <param name="obj">Object.</param>
		async void addConversation(object obj)
		{
			var nav = new NavigationPage(new NewConversation());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			await Navigation.PushModalAsync(nav);
		}
	}
}


