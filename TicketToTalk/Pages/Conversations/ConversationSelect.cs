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
		public static ObservableCollection<Conversation> conversations = new ObservableCollection<Conversation>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationSelect"/> class.
		/// </summary>
		public ConversationSelect(Ticket ticket)
		{
			conversations.Clear();
			Title = "Conversations";
			var conversationController = new ConversationController();

			var cs = Task.Run(() => conversationController.getRemoteConversations()).Result;

			//var rawConversations = conversationController.getConversations();
			foreach (Conversation converstaion in cs) 
			{
				conversations.Add(conversationController.setPropertiesForDisplay(converstaion));
			}

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			newConversation = new Button 
			{
				Text = "New Conversation",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_blue,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0,10,0,10)
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

			var label = new Label 
			{
				Text = "Select a conversation",
				TextColor = ProjectResource.color_dark,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var add_img = new Image() 
			{
				Source = "red_add.png",
				HeightRequest = 30,
				WidthRequest = 30,
				HorizontalOptions = LayoutOptions.EndAndExpand
			};
			add_img.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(newConvo) });

			var newStack = new StackLayout 
			{
				Padding = 20,
				Orientation = StackOrientation.Horizontal,
				Spacing = 0,
				HorizontalOptions = LayoutOptions.Fill,
				Children = 
				{
					label,
					add_img
				}
			};

			Content = new StackLayout
			{
				Spacing = 10,
				Children = {
					newStack,
					newConversation,
					listView
				}
			};
		}

		/// <summary>
		/// Creates a new conversation.
		/// </summary>
		void newConvo()
		{
			var nav = new NavigationPage(new NewConversation());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Cancels the operation
		/// </summary>
		/// <param name="obj">Object.</param>
		void cancel(object obj)
		{
			Navigation.PopModalAsync();
		}
	}
}


