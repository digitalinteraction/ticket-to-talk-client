// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 26/08/2016
//
// MenuCell.cs

using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Menu cell.
	/// </summary>
	public class MenuCell : ViewCell
	{
		public MenuCell()
		{
			var image = new Image
			{
				HeightRequest = 40,
				WidthRequest = 40,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			image.SetBinding(Image.SourceProperty, "IconSource");

			var label = new Label
			{
			};
            label.SetSubHeaderStyle();
			label.SetBinding(Label.TextProperty, "Title");

			var content = new StackLayout
			{
				Padding = new Thickness(16, 0, 0, 0),
				Orientation = StackOrientation.Horizontal,
				Spacing = 5,
				Children = 
				{
					image,
					label
				}
			};

			this.View = content;
		}
	}
}


