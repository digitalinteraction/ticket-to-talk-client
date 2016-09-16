// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 16/09/2016
//
// PersonPivot.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TicketToTalk
{

	/// <summary>
	/// Person pivot.
	/// </summary>
	public class PersonPivot : INotifyPropertyChanged
	{
		private string _user_type;
		private string _relation;

		public string user_type
		{
			get
			{
				return _user_type;
			}
			set
			{
				if (value != _user_type)
				{
					_user_type = value;
					NotifyPropertyChanged();
				}
			}
		}
		public string relation
		{
			get
			{
				return _relation;
			}
			set
			{
				if (value != _relation)
				{
					_relation = value;
					NotifyPropertyChanged();
				}
			}
		}

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
			return string.Format("[Pivot: user_type={0}, relation={1}]", user_type, relation);
		}
	}
}
