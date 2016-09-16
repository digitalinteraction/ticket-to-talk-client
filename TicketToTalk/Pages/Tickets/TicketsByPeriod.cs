using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets by period.
	/// </summary>
	public class TicketsByPeriod : ContentPage
	{
		public static List<Ticket> tickets;
		public static ObservableCollection<ticketPeriodContainer> periodContainers = new ObservableCollection<ticketPeriodContainer>();

		public class ticketPeriodContainer : INotifyPropertyChanged
		{
			private string _ticketCount = String.Empty;

			public Period period { get; set; }
			public string text { get; set; }
			public int tickets { get; set; }
			public string ticketCount
			{
				get
				{
					return this._ticketCount;
				}
				set
				{
					if (value != this._ticketCount)
					{
						this._ticketCount = value;
						NotifyPropertyChanged();
					}
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsByPeriod.ticketPeriodContainer"/> class.
			/// </summary>
			public ticketPeriodContainer() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsByPeriod.ticketPeriodContainer"/> class.
			/// </summary>
			/// <param name="period">Period.</param>
			/// <param name="ticketCount">Ticket count.</param>
			public ticketPeriodContainer(Period period, string ticketCount)
			{
				this.period = period;
				this.ticketCount = ticketCount;
			}

			/// <summary>
			/// Occurs when property changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Notifies the property changed.
			/// </summary>
			/// <param name="propertyName">Property name.</param>
			private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.TicketsByPeriod.ticketPeriodContainer"/>.
			/// </summary>
			/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.TicketsByPeriod.ticketPeriodContainer"/>.</returns>
			public override string ToString()
			{
				return string.Format("[ticketPeriodContainer: period={0}, text={1}, ticketCount={2}]", period, text, ticketCount);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsByPeriod"/> class.
		/// </summary>
		/// <param name="ticket_input">Tickets.</param>
		public TicketsByPeriod(List<Ticket> ticket_input)
		{
			periodContainers.Clear();
			tickets = ticket_input;
			if (Session.activePerson != null)
			{
				Padding = new Thickness(20);

				Title = "Periods";

				ToolbarItems.Add(new ToolbarItem
				{
					Text = "Add",
					Icon = "add_icon.png",
					Order = ToolbarItemOrder.Primary,
					Command = new Command(launchNewTicketView)
				});

				Debug.WriteLine("TicketByPeriod: Getting periods");

				var periodController = new PeriodController();
				var periods = periodController.getAllLocalPeriods();

				Debug.WriteLine("TicketByPeriod: Got periods - " + periods.Count);

				foreach (Period p in periods)
				{
					Debug.WriteLine("TicketByPeriod: Setting container for period - " + p);
					var tp = new ticketPeriodContainer();
					tp.text = p.text;
					tp.period = p;
					tp.tickets = periodController.getTicketsInPeriod(p.id).Count;

					Debug.WriteLine("TicketByPeriod: tickets in period - " + tp.tickets);

					if (tp.tickets == 0)
					{
						tp.ticketCount = tp.tickets + " tickets";
					}
					else if (tp.tickets == 1)
					{
						tp.ticketCount = tp.tickets + " ticket";
					}
					else
					{
						tp.ticketCount = tp.tickets + " tickets";
					}
					Debug.WriteLine("TicketByPeriod: Adding container - " + tp);
					periodContainers.Add(tp);
					Debug.WriteLine("TicketByPeriod: Added container");
				}

				Debug.WriteLine("TicketByPeriod: Set containers");

				var cell = new DataTemplate(typeof(TextCell));
				cell.SetBinding(TextCell.TextProperty, "text");
				cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_red);
				cell.SetBinding(TextCell.DetailProperty, "ticketCount");

				// ListView setup
				ListView periodsListView = new ListView();
				periodsListView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
				periodsListView.ItemTemplate = cell;
				periodsListView.BindingContext = periodContainers;
				periodsListView.SeparatorColor = Color.Transparent;
				periodsListView.ItemSelected += OnSelection;

				Content = new StackLayout
				{
					Spacing = 12,
					Children = {
						periodsListView
					}
				};
			}
		}

		/// <summary>
		/// Delegate for an item selection in ListView.
		/// </summary>
		/// <returns>void</returns>
		/// <param name="sender">Sender: Object</param>
		/// <param name="e">SelectedItemChangedEventArgs: Event arguments</param>
		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			var periodContainer = (ticketPeriodContainer)e.SelectedItem;

			var ticketsInPeriod = new List<Ticket>();
			// Check if ticket year falls within bounds.
			foreach (Ticket t in tickets)
			{
				if (t.period_id == periodContainer.period.id)
				{
					switch (t.mediaType)
					{
						case "Photo":
							t.displayIcon = "photograph_icon.png";
							break;
						case "Video":
							t.displayIcon = "video_icon.png";
							break;
						case "Audio":
							t.displayIcon = "audio_icon.png";
							break;
						case "Area":
							t.displayIcon = "area_icon.png";
							break;
					}
					// add to source
					ticketsInPeriod.Add(t);
				}
			}

			Navigation.PushAsync(new DisplayTickets(ticketsInPeriod, periodContainer.text));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launchs the new ticket view.
		/// </summary>
		/// <returns>The new ticket view.</returns>
		public void launchNewTicketView()
		{
			var nav = new NavigationPage(new SelectNewTicketType());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Adds the ticket.
		/// </summary>
		/// <param name="ticket">Ticke.</param>
		public static void addTicket(Ticket ticket)
		{
			foreach (ticketPeriodContainer tp in periodContainers)
			{
				if (tp.period.id == ticket.period_id)
				{
					tickets.Add(ticket);
					tp.tickets++;
					if (tp.tickets == 1)
					{
						tp.ticketCount = tp.ticketCount = tp.tickets + " ticket";
					}
					else
					{
						tp.ticketCount = tp.ticketCount = tp.tickets + " tickets";
					}
				}
			}
		}

		/// <summary>
		/// Removes the ticket.
		/// </summary>
		/// <param name="ticket">Ticket.</param>
		public static void removeTicket(Ticket ticket)
		{
			tickets.Remove(ticket);

			foreach (ticketPeriodContainer tp in periodContainers)
			{
				if (tp.period.id == ticket.period_id)
				{
					tp.tickets--;
					if (tp.tickets == 1)
					{
						tp.ticketCount = tp.ticketCount = tp.tickets + " ticket";
					}
					else
					{
						tp.ticketCount = tp.ticketCount = tp.tickets + " tickets";
					}
				}
			}
		}
	}
}
