using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	
	/// <summary>
	/// View audio ticket.
	/// </summary>
	public class ViewAudioTicket : ContentPage
	{
		string fileName;
		Ticket ticket;
		private bool download_finished = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ViewAudioTicket"/> class.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public ViewAudioTicket(Ticket ticket)
		{
			var hasStoragePerms = Task.Run(() => checkStoragePerms()).Result;
			if (!hasStoragePerms)
			{
				Navigation.PopAsync();
			}

			MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			{
				Debug.WriteLine("Image Downloaded");
				download_finished = finished;
			});

			this.ticket = ticket;
			Title = ticket.title;

			// Add button to navigation bar.
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(displayInfo)
			});

			Debug.WriteLine("ViewAudioTicket: Getting audio file.");

			if (ticket.pathToFile.StartsWith("storage", StringComparison.Ordinal))
			{
				NetworkController net = new NetworkController();
				var fileName = "t_" + ticket.id + ".wav";
				var task = Task.Run(() => net.downloadImage(ticket.pathToFile, fileName)).Result;
				//net.downloadImage(ticket.pathToFile, fileName);
				ticket.pathToFile = fileName;

				while (!download_finished)
				{
				}

				var ticketController = new TicketController();
				ticketController.updateTicketLocally(ticket);
			}
			Debug.WriteLine("ViewAudioTicket: Got file");
			fileName = ticket.pathToFile;
			DependencyService.Get<IAudioPlayer>().SetupPlayer(fileName);
			Debug.WriteLine("ViewAudioTicket: SetupPlayer");
			Content = new StackLayout
			{
				Children = {
					new AudioPlayerLayout(ticket),
					new TicketInfo(ticket),
				}
			};

			MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");
		}

		/// <summary>
		/// Display ticket info
		/// </summary>
		/// <returns>The info.</returns>
		public async void displayInfo()
		{
			var action = await DisplayActionSheet("Ticket Options", "Cancel", "Delete", "Display Information", "Add to Conversation");

			switch (action)
			{
				case ("Delete"):
					var ticketController = new TicketController();
					await Navigation.PopAsync();
					ticketController.destroyTicket(ticket);
					break;
				case ("Display Information"):
					await Navigation.PushAsync(new DisplayTicketInfo(ticket));
					break;
				case ("Add to Conversation"):
					await Navigation.PushModalAsync(new ConversationSelect(ticket));
					break;
			}
		}

		/// <summary>
		/// Checks the storage perms.
		/// </summary>
		/// <returns>The storage perms.</returns>
		private async Task<bool> checkStoragePerms()
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
	}
}


