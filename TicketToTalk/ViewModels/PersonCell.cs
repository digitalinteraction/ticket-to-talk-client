using System;
using System.Diagnostics;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Person cell.
	/// </summary>
	public class PersonCell : ViewCell
	{
		CircleImage personProfileImage;
		Label nameLabel;
		Label relation;

		public Person person { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PersonCell"/> class.
		/// </summary>
		public PersonCell()
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
			personProfileImage.SetBinding(Image.SourceProperty, "pathToPhoto");

			nameLabel = new Label
			{
				FontSize = 18,
				TextColor = ProjectResource.color_dark,
			};
			nameLabel.SetBinding(Label.TextProperty, "name");

			relation = new Label
			{
				FontSize = 12,
				TextColor = ProjectResource.color_red,
			};
			relation.SetBinding(Label.TextProperty, "relation");

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

		void DeleteCell_Clicked(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		public PersonCell(Person p) : this()
		{
			this.person = p;

			if (Session.activePerson != null && person.id == Session.activePerson.id) 
			{
				View.BackgroundColor = ProjectResource.color_blue;
				personProfileImage.BorderColor = ProjectResource.color_white;
				nameLabel.TextColor = ProjectResource.color_white;
				relation.TextColor = ProjectResource.color_dark;
			}
		}
	}
}

