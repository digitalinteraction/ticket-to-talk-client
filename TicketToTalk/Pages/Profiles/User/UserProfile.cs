using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// User profile view.
	/// </summary>
	public class UserProfile : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.UserProfile"/> class.
		/// </summary>
		/// <param name="user">User.</param>
		public UserProfile()
		{
			Title = "Your Profile";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Info",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(options)
			});

			var nameLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 0, 0, 5)
			};
			nameLabel.SetBinding(Label.TextProperty, "name");
			nameLabel.BindingContext = Session.activeUser;

			var email = new Label 
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 12,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};
			email.SetBinding(Label.TextProperty, "email");
			email.BindingContext = Session.activeUser;

			Content = new StackLayout
			{
				Spacing = 0,
				Children = {
					new UserProfileImage((Session.ScreenWidth * 0.8), null, ProjectResource.color_red),
					nameLabel,
					email
				}
			};
		}

		/// <summary>
		/// Displays profile options.
		/// </summary>
		/// <param name="obj">Object.</param>
		async void options(object obj)
		{
			var action = await DisplayActionSheet("Your Profile", "Cancel", null, "Edit Profile");

			switch (action) 
			{
				case "Edit Profile":
					var nav = new NavigationPage(new EditProfile());
					nav.BarBackgroundColor = ProjectResource.color_blue;
					nav.BarTextColor = ProjectResource.color_white;

					await Navigation.PushModalAsync(nav);
					break;
			}
		}
	}
}