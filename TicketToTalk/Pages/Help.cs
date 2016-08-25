using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Help.
	/// </summary>
	public class Help : ContentPage
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Help"/> class.
		/// </summary>
		public Help()
		{

			Title = "Help";

			var title = new Label
			{
				Text = "Ticket to Talk",
				TextColor = ProjectResource.color_dark,
				FontSize = 22,
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var summary = new Label 
			{
				Text = "Help bridge gaps in conversation with Ticket to Talk.",
				TextColor = ProjectResource.color_dark,
				FontSize = 16,
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var intro = new Label 
			{
				Text = 
					"You can use this app to collect information about someone through adding tickets. " +
					"Add tickets to a conversation which you can use when you talk to the person you've added tickets for. " +
					"",
				TextColor = ProjectResource.color_dark,
				FontSize = 14,
			};

			var content = new StackLayout 
			{
				Padding = new Thickness(20),
				Spacing = 10,
				Children = 
				{
					title, 
					summary,
					intro
				}
			};

			Content = new ScrollView
			{
				Content = content
			};
		}
	}
}


