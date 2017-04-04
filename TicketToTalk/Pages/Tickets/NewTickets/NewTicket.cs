using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// New ticket.
	/// </summary>
	public class NewTicket : ContentPage
	{

		public static bool isInTutorial = false;
		private ContentView ticketInf = new ContentView();
		public static ActivityIndicator indicator;
		ScrollView scrollView;
		AbsoluteLayout layout = new AbsoluteLayout();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewTicket"/> class.
		/// </summary>
		/// <param name="mediaType">Media type.</param>
		/// <param name="filePath">File path.</param>
		public NewTicket(string mediaType, string filePath)
		{
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			// Set title.
			Title = "New Ticket";
			ContentView mediaContent = null;

			var hasPerms = Task.Run(() => checkStoragePerms()).Result;
			if (!hasPerms)
			{
				Navigation.PopAsync();
			}
			else
			{
				switch (mediaType)
				{
					case ("Picture"):
						var media = MediaController.ReadBytesFromFile(filePath);
						mediaContent = new PictureLayout(media);
						ticketInf = new NewTicketInfo(mediaType, media);
						break;
					case ("Sound"):
						mediaContent = new AudioPlayerLayout(new Ticket { pathToFile = filePath });
						ticketInf = new NewTicketInfo(mediaType, filePath);
						break;
					case ("Video"):
						break;
				}
			}

			indicator = new ActivityIndicator
			{
				BackgroundColor = ProjectResource.color_white_transparent,
				Color = ProjectResource.color_dark,
				IsVisible = false,
				IsEnabled = false
			};

			var stack = new StackLayout
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				Spacing = 0,
				Children =
				{
					mediaContent,
					ticketInf,
				}
			};

			scrollView = new ScrollView
			{
				Content = stack
			};

			layout.HeightRequest = Session.ScreenHeight;

			AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0.5, 0.5, 1.0, 1.0));
			AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);

			AbsoluteLayout.SetLayoutBounds(indicator, new Rectangle(0.5, 0.5, 1.0, 1.0));
			AbsoluteLayout.SetLayoutFlags(indicator, AbsoluteLayoutFlags.All);

			layout.Children.Add(scrollView);
			layout.Children.Add(indicator);

			Content = layout;
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewTicket"/> class.
		/// Used for creating new tickets that do not have data attached. (YouTube videos.)
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public NewTicket(Ticket ticket)
		{
			Title = "New Ticket";

			var stack = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					new YouTubePlayer(ticket.pathToFile),
					new NewTicketInfo("YouTube", ticket.pathToFile),
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
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
				Debug.WriteLine(e.StackTrace);
				return false;
			}

			return false;
		}

	}
}