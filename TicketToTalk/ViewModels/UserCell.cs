using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// User cell.
	/// </summary>
	public class UserCell : ViewCell
	{
		public UserCell()
		{
			var personProfileImage = new CircleImage
			{
				BorderColor = ProjectResource.color_red,
				BorderThickness = 2,
				HeightRequest = 75,
				WidthRequest = 75,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			personProfileImage.SetBinding(Image.SourceProperty, "imageSource");

			var nameLabel = new Label
			{
				FontSize = 18,
				TextColor = ProjectResource.color_dark,
			};
			nameLabel.SetBinding(Label.TextProperty, "name");

			var relation = new Label
			{
				FontSize = 12,
				TextColor = ProjectResource.color_blue,
			};
			relation.SetBinding(Label.TextProperty, "email");

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(10, 0, 0, 0),
				Spacing = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					nameLabel,
					relation
				}
			};

			var cellLayout = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 5, 10, 5),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					personProfileImage,
					detailsStack
				}
			};

			this.View = cellLayout;
		}
	}
}