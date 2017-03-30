using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// User.
	/// </summary>
	public class User : INotifyPropertyChanged, IComparable
	{
		private string _name;
		private string _email;
		private string _password;
		private string _pathToPhoto;
		private ImageSource _imageSource;
		private string _imageHash;
		private bool _firstLogin;
		private string _api_key;
		private string _verified;

		[PrimaryKey]
		public int id { get; set; }
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

		public bool firstLogin
		{
			get
			{
				return _firstLogin;
			}
			set
			{
				if (value != _firstLogin)
				{
					_firstLogin = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string api_key 
		{
			get 
			{
				return _api_key;
			}
			set 
			{
				if (value != _api_key) 
				{
					_api_key = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string verified
		{
			get
			{
				return _verified;
			}
			set
			{
				if (value != _verified)
				{
					_verified = value;
					NotifyPropertyChanged();
				}
			}
		}

		[Ignore]
		public Pivot pivot { get; set; }

		/// <summary>
		/// Creates a new instance of a user.
		/// 
		/// Sets a default api key.
		/// </summary>
		public User() 
		{
			// TODO: Remove from constructor
			api_key = "a82ae536fc32c8c185920f3a440b0984bb51b9077517a6c8ce4880e41737438d";
		}

		/// <summary>
		/// Creates a new instance of a user.
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="Email">Email.</param>
		public User(int id, string Name, string Email)
		{
			this.id = id;
			name = Name;
			email = Email;
		}

		/// <summary>
		/// Notifies the property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public override string ToString()
		{
			return string.Format("[User: name={0}, email={1}, password={2}, pathToPhoto={3}, " +
			                     "created_at={4}, updated_at={5}, imageSource={6}, imageHash={7}, " +
			                     "firstLogin={8}, api_key={9}, pivot={10}]", 
			                     name, email, password, pathToPhoto, created_at, updated_at, 
			                     imageSource, imageHash, firstLogin, api_key, pivot);
		}

		/// <summary>
		/// Compares to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo(object obj)
		{
			var rhs = obj as User;
			var comp = string.Compare(name, rhs.name, StringComparison.Ordinal);

			if (comp == 0)
			{
				comp = string.Compare(email, rhs.email, StringComparison.Ordinal);
			}
			return _name.CompareTo(obj);
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