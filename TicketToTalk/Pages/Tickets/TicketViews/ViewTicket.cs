using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View ticket.
	/// </summary>
	public partial class ViewTicket : TrackedContentPage
	{
		public static Ticket displayedTicket { get; set; }
		private TicketController ticketController = new TicketController();
		ContentView mediaContent = new ContentView();

		/// <summary>
		/// Initializes a view of the ticket content.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public ViewTicket(Ticket ticket)
		{

            TrackedName = "View Ticket";

			NavigationPage.SetHasBackButton(this, true);

			displayedTicket = ticket;
			Title = ticket.title;
			this.SetBinding(TitleProperty, "title");
			BindingContext = displayedTicket;

			// Add button to navigation bar.
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(DisplayInfo)
			});

			var contHolder = new ContentView();
			contHolder.SetBinding(ContentView.ContentProperty, "Content");
			contHolder.BindingContext = mediaContent;

			var content = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					contHolder,
					new TicketInfo()
				}
			};

			Content = new ScrollView
			{
				Content = content
			};
		}

		/// <summary>
		/// Checks the storage perms.
		/// </summary>
		/// <returns>The storage perms.</returns>
		private async Task<bool> CheckStoragePerms()
		{
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
				if (status != PermissionStatus.Granted)
				{
					if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
					{
						await DisplayAlert("Storage", "Ticket to Talk needs access to storage to save tickets.", "OK");
					}
					var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
					status = results[Permission.Storage];
				}

				if (status == PermissionStatus.Granted)
				{
					return true;
				}
				else if (status != PermissionStatus.Unknown)
				{
					await DisplayAlert("Storage Denied", "Cannot save tickets without access to audio.", "OK");
					return false;
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return false;
			}

			return false;
		}

		/// <summary>
		/// Display ticket info
		/// </summary>
		/// <returns>The info.</returns>
		private async void DisplayInfo()
		{
			var action = await DisplayActionSheet("Ticket Options", "Cancel", "Delete", "Edit Ticket", "Add to Conversation");

			switch (action)
			{
				case ("Delete"):

					var deleted = false;

					try
					{
						deleted = await ticketController.DestroyTicket(displayedTicket);
						if (deleted) 
						{
							await Navigation.PopAsync();
						}
					}
					catch (NoNetworkException ex)
					{
						await DisplayAlert("No Network", ex.Message, "Dismiss");
					}

					break;
				case ("Edit Ticket"):

					var nav = new NavigationPage(new EditTicket(displayedTicket));
					nav.BarBackgroundColor = ProjectResource.color_blue;
					nav.BarTextColor = ProjectResource.color_white;

					await Navigation.PushModalAsync(nav);

					break;
				case ("Add to Conversation"):

					nav = new NavigationPage(new ConversationSelect(displayedTicket));
					nav.BarBackgroundColor = ProjectResource.color_blue;
					nav.BarTextColor = ProjectResource.color_white;

					await Navigation.PushModalAsync(nav);

					break;
			}
		}

		/// <summary>
		/// Sets up ticket for display.
		/// </summary>
		public async Task<bool> SetUpTicketForDisplay()
		{
			if (displayedTicket.pathToFile.StartsWith("ticket_to_talk", StringComparison.Ordinal))
			{
				try
				{
					await ticketController.DownloadTicketContent(displayedTicket);
				}
				catch (NoNetworkException ex)
				{
					Debug.WriteLine(ex.StackTrace);
					return false;
				}

				var ext = displayedTicket.pathToFile.Substring(displayedTicket.pathToFile.LastIndexOf('.'));
				var fileName = String.Format("t_{0}{1}", displayedTicket.id, ext);

				displayedTicket.pathToFile = fileName;
				ticketController.UpdateTicketLocally(displayedTicket);
			}

			displayedTicket.displayString = ticketController.GetDisplayString(displayedTicket);

			//mediaContent = new ContentView();

			var hasPerms = await CheckStoragePerms();
			if (!hasPerms)
			{
				await Navigation.PopAsync();
			}
			else
			{
				switch (displayedTicket.mediaType)
				{
					case ("Picture"):
						mediaContent.Content = new PictureLayout(displayedTicket.pathToFile).Content;
						break;
					case ("Sound"):
						mediaContent.Content = new AudioPlayerLayout(displayedTicket).Content;
						break;
					case ("Video"):
						break;
					case ("YouTube"):
						mediaContent.Content = new YouTubePlayer(displayedTicket.pathToFile).Content;
						break;
				}
			}

			return true;
		}
	}
}