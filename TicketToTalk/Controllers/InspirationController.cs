// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 02/04/2017
//
// InspirationController.cs
using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	public class InspirationController
	{
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

			lock (Session.connection)
			{
				var q = from i in Session.connection.Table<Inspiration>() select i;

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
		/// Gets the inspiration.
		/// </summary>
		/// <returns>The inspiration.</returns>
		/// <param name="id">Identifier.</param>
		public Inspiration GetInspiration(int id)
		{
			Inspiration inspiration = null;

			lock (Session.connection)
			{
				inspiration = (from i in Session.connection.Table<Inspiration>() where i.id == id select i).FirstOrDefault();
			}

			return inspiration;
		}

		/// <summary>
		/// Adds the inspiration locally.
		/// </summary>
		/// <param name="ins">Ins.</param>
		public void AddInspirationLocally(Inspiration ins)
		{
			lock (Session.connection)
			{
				Session.connection.Insert(ins);
			}
		}

		/// <summary>
		/// Updates the inspiration locally.
		/// </summary>
		/// <param name="ins">Ins.</param>
		public void UpdateInspirationLocally(Inspiration ins)
		{
			lock (Session.connection)
			{
				Session.connection.Update(ins);
			}
		}
	}
}
