using System;
using System.Collections.Generic;
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
		public static ObservableCollection<Conversation> conversations = new ObservableCollection<Conversation>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationSelect"/> class.
		/// </summary>
		public ConversationSelect(Ticket ticket)
		{
			conversations.Clear();
			Title = "Conversations";
			var conversationController = new ConversationController();

			var task = Task.Run(() => conversationController.GetRemoteConversations());
			var cs = new List<Conversation>();
			try
			{
				cs = task.Result;
			}
			catch (Exception ex)
			{
				cs = conversationController.GetLocalConversations();
				Debug.WriteLine(ex);
			}

			foreach (Conversation converstaion in cs)
			{
				conversations.Add(conversationController.SetPropertiesForDisplay(converstaion));
			}

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			var cell = new DataTemplate(typeof(ConversationCell));

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

				var conversation = (Conversation)e.SelectedItem;

				var added = false;

				try
				{
					added = await conversationController.AddTicketToConversationRemotely(conversation, ticket);

					if (added) 
					{
						conversation = conversationController.AddTicketToConversation(conversation, ticket);
						conversationController.UpdateConversationViews(conversationController.SetPropertiesForDisplay(conversation));

						await Navigation.PopModalAsync();

					}

					await DisplayAlert("Select a Conversation", "Ticket could not be added to the selected conversation.", "OK");

					((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
				}
				catch (NoNetworkException ex)
				{
					await DisplayAlert("No Network", ex.Message, "Dismiss");
					((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
				}
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
			add_img.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(NewConversation) });

			var newStack = new StackLayout
			{
				Padding = 10,
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
				Children = {
					newStack,
					listView
				}
			};
		}

		/// <summary>
		/// Creates a new conversation.
		/// </summary>
		private void NewConversation()
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
		private void Cancel(object obj)
		{
			Navigation.PopModalAsync();
		}
	}
}


