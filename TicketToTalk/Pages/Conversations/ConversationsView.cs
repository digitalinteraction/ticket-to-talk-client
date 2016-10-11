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
		public static bool tutorialShown = false;
		public static ObservableCollection<Conversation> conversations = new ObservableCollection<Conversation>();
		private ConversationController conversationController = new ConversationController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationsView"/> class.
		/// </summary>
		public ConversationsView()
		{
			Padding = new Thickness(20);
			conversations.Clear();

			var cs = Task.Run(() => conversationController.GetRemoteConversations()).Result;

			Title = "Converations";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(AddConversation)
			});

			foreach (Conversation c in cs)
			{
				conversations.Add(conversationController.SetPropertiesForDisplay(c));
			}

			var listView = new ListView();
			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			listView.HasUnevenRows = true;
			listView.RowHeight = 50;
			listView.BindingContext = conversations;
			listView.SeparatorColor = Color.Transparent;
			listView.ItemTemplate = new DataTemplate(typeof(ConversationCell));
			listView.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
				}

				var conversation = (Conversation)e.SelectedItem;
				await Navigation.PushAsync(new ConversationView(conversation));

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
		private async void AddConversation(object obj)
		{
			var nav = new NavigationPage(new NewConversation());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			await Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !tutorialShown)
			{

				var text = "Conversations lets you gather tickets into a group and make notes before you have an physical conversation with the person you want to talk to.\n\nClick the 'Add' button to add a new conversation.";

				Navigation.PushModalAsync(new HelpPopup(text, "chat_white_icon.png"));
				tutorialShown = true;
			}
		}
	}
}


