using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// App
	/// </summary>
	public class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.App"/> class.
		/// </summary>
		public App()
		{
			var periodController = new PeriodController();
			periodController.InitStockPeriods();

			var nav = new NavigationPage(new Login());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			//var conversationController = new ConversationController();
			//var conversation = new Conversation
			//{
			//	date = "19/09/2016 00:00:00"
			//};
			//conversationController.ParseDateToIntegers(conversation);

			var net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			var response = net.SendGetRequest("/", parameters);
			Debug.WriteLine(response);

			MainPage = nav;
		}
	}
}

