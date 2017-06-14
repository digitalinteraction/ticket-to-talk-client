using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// User profile view.
	/// </summary>
	public class UserProfile : TrackedContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.UserProfile"/> class.
		/// </summary>
		/// <param name="user">User.</param>
		public UserProfile()
		{
            TrackedName = "User Profile";

			Title = "Your Profile";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Info",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Options)
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
            nameLabel.SetHeaderStyle();
            nameLabel.VerticalOptions = LayoutOptions.Start;
			nameLabel.SetBinding(Label.TextProperty, "name");
			nameLabel.BindingContext = Session.activeUser;

			var email = new Label 
			{
				HorizontalTextAlignment = TextAlignment.Center,
			};
            email.SetSubHeaderStyle();
            email.VerticalOptions = LayoutOptions.Start;
            email.HorizontalOptions = LayoutOptions.CenterAndExpand;
            email.HorizontalTextAlignment = TextAlignment.Center;
            email.TextColor = ProjectResource.color_red;
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
		private async void Options(object obj)
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