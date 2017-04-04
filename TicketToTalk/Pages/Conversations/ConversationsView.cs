using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
		private ProgressSpinner indicator;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationsView"/> class.
		/// </summary>
		public ConversationsView()
		{
			indicator = new ProgressSpinner(this, ProjectResource.color_white_transparent, ProjectResource.color_dark);

			Padding = new Thickness(20);
			conversations.Clear();

			List<Conversation> cs = null;

			//var cs = Task.Run(() => conversationController.GetRemoteConversations()).Result;

			var task = Task.Run(() => conversationController.GetRemoteConversations());

			try
			{
				cs = task.Result;	
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);

				cs = conversationController.GetLocalConversations();
			}

			if (cs == null) 
			{
				cs = new List<Conversation>();
			}

			Title = "Conversations";

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

			listView.ItemSelected += ListView_ItemSelected;

			var stack = new StackLayout
			{
				Children = {
					listView
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

		public async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.

			IsBusy = true;

			var conversation = (Conversation)e.SelectedItem;

			var nav = new ConversationView(conversation);
			var ready = false;

			try
			{
				ready = await nav.SetUpConversation();

				if (ready) 
				{
					IsBusy = false;
					await Navigation.PushAsync(nav);
				}
			}
			catch (NoNetworkException ex)
			{
				IsBusy = false;
				Debug.WriteLine(ex.StackTrace);
				await DisplayAlert("No Network", ex.Message, "Dismiss");
			}
		}
	}
}


