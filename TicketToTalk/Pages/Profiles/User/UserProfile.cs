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
		public UserProfile(User user)
		{
			Title = "Your Profile";

			var nameLabel = new Label
			{
				Text = user.name,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 0, 0, 5)
			};

			var email = new Label 
			{
				Text = user.email,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 12,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			Content = new StackLayout
			{
				Spacing = 0,
				Children = {
					new UserProfileImage(user, (Session.ScreenWidth * 0.8), null, ProjectResource.color_red),
					nameLabel,
					email
				}
			};
		}
	}
}


