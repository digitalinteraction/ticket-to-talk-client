using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Ticket.
	/// </summary>
	public class Ticket : IComparable, INotifyPropertyChanged
	{
		private string _title;
		private string _description;
		private string _year;
		private string _pathToFile;
		private string _area;
		private string _access_level;
		private int _period_id;
		private string _displayString;

		[PrimaryKey]
		public int id { get; set; }
		public string title
		{
			get
			{
				return _title;
			}
			set
			{
				if (value != _title)
				{
					_title = value;
					NotifyPropertyChanged();
				}
			}
		}
		public string description
		{
			get
			{
				return _description;
			}
			set
			{
				if (value != _description)
				{
					_description = value;
					NotifyPropertyChanged();
				}
			}
		}
		public string mediaType { get; set; }
		public string year
		{
			get
			{
				return _year;
			}
			set
			{
				if (value != _year)
				{
					_year = value;
					NotifyPropertyChanged();
				}
			}
		}
		public string pathToFile
		{
			get
			{
				return _pathToFile;
			}
			set
			{
				if (value != _pathToFile)
				{
					_pathToFile = value;
					NotifyPropertyChanged();
				}
			}
		}
		[Ignore]
		public string displayIcon { get; set; }
		public string access_level
		{
			get
			{
				return _access_level;
			}
			set
			{
				if (value != _access_level)
				{
					_access_level = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string area
		{
			get
			{
				return _area;
			}
			set
			{
				if (value != _area)
				{
					_area = value;
					NotifyPropertyChanged();
				}
			}
		}

		//public int area_id { get; set; }
		public int person_id { get; set; }
		public int period_id
		{
			get
			{
				return _period_id;
			}
			set
			{
				if (value != _period_id)
				{
					_period_id = value;
					NotifyPropertyChanged();
				}
			}
		}
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		[Ignore]
		public Tag[] tags { get; set; }
		[Ignore]
		public string displayString
		{
			get
			{
				return _displayString;
			}
			set
			{
				if (value != _displayString)
				{
					_displayString = value;
					NotifyPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Creates a new instance of a ticket.
		/// </summary>
		public Ticket()
		{
		}

		/// <summary>
		/// Creates a new instance of a ticket.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="mediaType">Media type.</param>
		/// <param name="dateAdded">Date added.</param>
		/// <param name="year">Year.</param>
		public Ticket(string title, string mediaType, DateTime dateAdded, string year)
		{
			this.title = title;
			this.mediaType = mediaType;
			this.created_at = dateAdded;
			this.year = year;
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
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Ticket"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Ticket"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Ticket: id={0}, title={1}, description={2}, mediaType={3}, year={4}, pathToFile={5}, " +
								 "displayIcon={6}, access_level={7}, area={8}, person_id={9}, period_id={10}, " +
								 "created_at={11}, updated_at={12}, displayString={13}]", id, title, description,
								 mediaType, year, pathToFile, displayIcon, access_level, area, person_id, period_id,
								 created_at, updated_at, displayString);
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.Ticket"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:TicketToTalk.Ticket"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.Ticket"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="T:TicketToTalk.Ticket"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + title.GetHashCode();
			hash = (hash * 7) + description.GetHashCode();
			hash = (hash * 7) + mediaType.GetHashCode();
			hash = (hash * 7) + access_level.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();
			hash = (hash * 7) + updated_at.GetHashCode();

			return hash;
		}

		/// <summary>
		/// Implements the comparable interface.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo(object obj)
		{
			Ticket lhs = obj as Ticket;

			var comp = string.Compare(title, lhs.title, StringComparison.Ordinal);
			if (comp == 0)
			{
				comp = string.Compare(year, lhs.year, StringComparison.Ordinal);
			}
			return comp;
		}
	}
}

