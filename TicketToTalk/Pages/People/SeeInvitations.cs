using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// See invitations.
	/// </summary>
	public class SeeInvitations : ContentPage
	{

		public static ObservableCollection<Invitation> invitations = new ObservableCollection<Invitation>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SeeInvitations"/> class.
		/// </summary>
		public SeeInvitations(List<Invitation> invites)
		{
			invitations.Clear();
			foreach (Invitation i in invites)
			{
				invitations.Add(i);
			}
			this.Title = "Invitations";

			var listView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(InvitationCell)),
				SeparatorColor = ProjectResource.color_grey,
				HasUnevenRows = true,
				RowHeight = 90
			};
			listView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			listView.BindingContext = invitations;
			listView.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
				}

				var inv = (Invitation)e.SelectedItem;
				await Navigation.PushAsync(new SeeInvite(inv));

				((ListView)sender).SelectedItem = null;
			};

			Content = new StackLayout
			{
				Children = {
					listView
				}
			};
		}
	}
}


