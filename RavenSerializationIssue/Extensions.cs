using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavenSerializationIssue
{
	public static class Extensions
	{
		public static void AddItem<T, I>(this Dictionary<T, I> source, T key, I value)
		{
			if (!source.ContainsKey(key))
			{
				source.Add(key, value);
			} else
			{
				source[key] = value;
			}
		}
	}
}
