// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 29/03/2017
//
// EmptyClass.cs
using System;
using System.Runtime.Serialization;

namespace TicketToTalk
{
	public class APIUnauthorisedException: Exception
	{

		public APIUnauthorisedException()
		{
		}

		public APIUnauthorisedException(string message) : base(message)
		{
		}

		public APIUnauthorisedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected APIUnauthorisedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
