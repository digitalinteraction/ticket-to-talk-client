using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Person.
	/// </summary>
	public class Person : INotifyPropertyChanged, IComparable
	{
		private string _name;
		private string _birthYear;
		private string _birthPlace;
		private string _pathToPhoto;
		private string _notes;
		private string _displayString;
		private ImageSource _imageSource;
		private string _imageHash;

		[PrimaryKey]
		public int id { get; set; }
		public int admin_id { get; set; }

		public string name
		{
			get
			{
				return _name;
			}
			set
			{
				if (value != _name)
				{
					_name = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string birthYear
		{
			get
			{
				return _birthYear;
			}
			set
			{
				if (value != _birthYear)
				{
					_birthYear = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string birthPlace
		{
			get
			{
				return _birthPlace;
			}
			set
			{
				if (value != _birthPlace)
				{
					_birthPlace = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string pathToPhoto
		{
			get
			{
				return _pathToPhoto;
			}
			set
			{
				if (value != _pathToPhoto)
				{
					_pathToPhoto = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string area { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public string notes
		{
			get
			{
				return _notes;
			}
			set
			{
				if (value != _notes)
				{
					_notes = value;
					NotifyPropertyChanged();
				}
			}
		}
		[Ignore]
		public Pivot pivot { get; set; }
		[Ignore]
		public string relation { get; set; }
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
		[Ignore]
		public ImageSource imageSource
		{
			get
			{
				return _imageSource;
			}
			set
			{
				_imageSource = value;
				NotifyPropertyChanged();
			}
		}

		public string imageHash
		{
			get
			{
				return _imageHash;
			}
			set
			{
				if (value != _imageHash)
				{
					_imageHash = value;
					NotifyPropertyChanged();
				}
			}
		}

		public class Pivot
		{
			public string user_type { get; set; }
			public string relation { get; set; }

			public override string ToString()
			{
				return string.Format("[Pivot: user_type={0}, relation={1}]", user_type, relation);
			}
		}

		/// <summary>
		/// Initializes a new instance of a Person.
		/// </summary>
		public Person()
		{
		}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Person"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Person"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Person: id={0}, admin_id={1}, name={2}, birthYear={3}, birthPlace={4}, pathToPhoto={5}, area={6}, created_at={7}, updated_at={8}, notes={9}, pivot={10}, relation={11}, displayString={12}, imageHash={13}]",
				id, admin_id, name, birthYear, birthPlace, pathToPhoto, area, created_at, updated_at, notes, pivot, relation, displayString, imageHash);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + name.GetHashCode();
			hash = (hash * 7) + birthYear.GetHashCode();
			hash = (hash * 7) + birthPlace.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();
			hash = (hash * 7) + updated_at.GetHashCode();
			hash = (hash * 7) + notes.GetHashCode();

			return hash;
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.Person"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:TicketToTalk.Person"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.Person"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			var rhs = obj as Person;

			return id == rhs.id;
			//&& admin_id == rhs.admin_id
			//&& name.Equals(rhs.name)
			//&& birthYear.Equals(rhs.birthYear)
			//&& birthPlace.Equals(rhs.birthPlace)
			//&& area.Equals(rhs.area)
			//&& notes.Equals(rhs.notes);
		}

		/// <summary>
		/// Compares to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo(object obj)
		{
			var lhs = obj as Person;
			var comp = string.Compare(name, lhs.name, StringComparison.Ordinal);

			if (comp == 0)
			{
				comp = string.Compare(birthYear, lhs.birthYear, StringComparison.Ordinal);
			}

			return comp;
		}
	}
}

