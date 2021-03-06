﻿using System;
using System.Runtime.Serialization;

namespace TicketToTalk
{
	[Serializable]
	class NoNetworkException : Exception
	{
		public NoNetworkException()
		{
		}

		public NoNetworkException(string message) : base(message)
		{
		}

		public NoNetworkException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}