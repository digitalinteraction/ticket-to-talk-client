using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Models a User.
	/// </summary>
	public class User : INotifyPropertyChanged
	{
		private string _name;
		private string _email;
		private string _password;
		private string _pathToPhoto;
		private ImageSource _imageSource;

		[PrimaryKey]
		public int id { get; set;}
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
		public string email 
		{ 
			get 
			{
				return _email;
			} 
			set 
			{
				if (value != _email) 
				{
					_email = value;
					NotifyPropertyChanged();
				}
			}
		}
		public string password 
		{ 
			get 
			{
				return _password;
			} 
			set 
			{
				if (value != _password) 
				{
					_password = value;
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
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		[Ignore]
		public ImageSource imageSource 
		{
			get 
			{
				return _imageSource;
			}
			set 
			{
				_imageSource = value;;
			}
		}

		[Ignore]
		public Pivot pivot { get; set;}

		/// <summary>
		/// Creates a new instance of a user.
		/// </summary>
		public User() { }

		/// <summary>
		/// Creates a new instance of a user.
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="Email">Email.</param>
		public User(int id, string Name, string Email)
		{
			this.id = id;
			this.name = Name;
			this.email = Email;
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
		/// User to string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[User: id={0}, name={1}, email={2}, password={3}, pathToPhoto={4}, created_at={5}, updated_at={6}, pivot={7}]", id, name, email, password, pathToPhoto, created_at, updated_at, pivot);
		}

		/// <summary>
		/// Class to hold pivot table information from api calls.
		/// </summary>
		public class Pivot 
		{
			public string user_type { get; set; }

			public override string ToString()
			{
				return string.Format("[Pivot: user_type={0}]", user_type);
			}
		}
	}
}