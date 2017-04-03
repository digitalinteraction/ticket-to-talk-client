// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 02/04/2017
//
// InspirationController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TicketToTalk
{
	public class InspirationController
	{

		NetworkController net = new NetworkController();

		public InspirationController()
		{
		}

		/// <summary>
		/// Gets all inspirations.
		/// </summary>
		/// <returns>The all inspirations.</returns>
		public List<Inspiration> GetAllInspirations()
		{

			List<Inspiration> inspirations = new List<Inspiration>();

			lock (Session.Connection)
			{
				var q = from i in Session.Connection.Table<Inspiration>() select i;

				foreach (Inspiration i in q) 
				{
					inspirations.Add(i);
				}
			}

			return inspirations;
		}

		/// <summary>
		/// Gets a random inspiration.
		/// </summary>
		/// <returns>The random inspiration.</returns>
		public Inspiration GetRandomInspiration()
		{

			var inspirations = GetAllInspirations();

			if (inspirations.Count > 0)
			{
				var idx = new Random().Next(inspirations.Count);
				return inspirations[idx];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the inspirations from server.
		/// </summary>
		public async Task GetInspirationsFromServer()
		{
			// Send get request for inspirations
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			JObject jobject = null;

			try
			{
				jobject = await net.SendGetRequest("inspiration/get", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

			var data = jobject.GetData();
			var inspirations = data["inspirations"].ToObject<List<Inspiration>>();

			foreach (Inspiration ins in inspirations)
			{
				var stored = GetInspiration(ins.id);
				if (stored == null)
				{
					AddInspirationLocally(ins);
				}
				else if (stored.GetHashCode() != ins.GetHashCode())
				{
					UpdateInspirationLocally(ins);
				}
			}
		}

		/// <summary>
		/// Gets the inspiration.
		/// </summary>
		/// <returns>The inspiration.</returns>
		/// <param name="id">Identifier.</param>
		public Inspiration GetInspiration(int id)
		{
			Inspiration inspiration = null;

			lock (Session.Connection)
			{
				inspiration = (from i in Session.Connection.Table<Inspiration>() where i.id == id select i).FirstOrDefault();
			}

			return inspiration;
		}

		/// <summary>
		/// Adds the inspiration locally.
		/// </summary>
		/// <param name="ins">Ins.</param>
		public void AddInspirationLocally(Inspiration ins)
		{
			lock (Session.Connection)
			{
				Session.Connection.Insert(ins);
			}
		}

		/// <summary>
		/// Updates the inspiration locally.
		/// </summary>
		/// <param name="ins">Ins.</param>
		public void UpdateInspirationLocally(Inspiration ins)
		{
			lock (Session.Connection)
			{
				Session.Connection.Update(ins);
			}
		}
	}
}
