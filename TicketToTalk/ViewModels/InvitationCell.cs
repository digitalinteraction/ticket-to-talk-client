using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Invitation cell.
	/// </summary>
	public class InvitationCell : ViewCell
	{

		CircleImage personProfileImage;
		Label nameLabel;
		Label relation;

		public InvitationCell()
		{
			personProfileImage = new CircleImage
			{
				BorderColor = ProjectResource.color_blue,
				BorderThickness = 2,
				HeightRequest = 75,
				WidthRequest = 75,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			personProfileImage.SetBinding(Image.SourceProperty, "imageSource");

			nameLabel = new Label
			{
			};
            nameLabel.SetSubHeaderStyle();
            nameLabel.VerticalOptions = LayoutOptions.Start;
			nameLabel.SetBinding(Label.TextProperty, "person_name");

			relation = new Label
			{
			};
            relation.SetBodyStyle();
            relation.VerticalOptions = LayoutOptions.Start;
            relation.TextColor = ProjectResource.color_red;

			relation.SetBinding(Label.TextProperty, new Binding("name", stringFormat: "Invited by {0}"));

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

