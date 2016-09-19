using System.Collections.Generic;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Menu list view.
	/// </summary>
	public class MenuListView : ListView
	{
		/// <summary>
		/// Initializes a new instance of the menu list.
		/// </summary>
		public MenuListView()
		{
			List<NavMenuItem> data = new MenuListData();

			SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			BindingContext = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = ProjectResource.color_white;

			var cell = new DataTemplate(typeof(MenuCell));

			HasUnevenRows = true;
			RowHeight = 60;

			ItemTemplate = cell;
			SeparatorColor = Color.Transparent;
		}
	}
}
