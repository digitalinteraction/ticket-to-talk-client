using Xamarin.Forms;

namespace TicketToTalk
{
	public class EditArticle : ContentPage
	{
		public EditArticle(Article article)
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}


