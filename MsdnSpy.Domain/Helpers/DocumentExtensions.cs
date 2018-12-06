using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsdnSpy.Domain.Helpers
{
	public static class DocumentExtensions
	{
		public static IEnumerable<IElement> GetElementsBy(
			this IDocument document,
			string query,
			IDictionary<string, string> requiredAttributes = null)
		{
			if (document == null)
				throw new ArgumentNullException(nameof(document));

			IEnumerable<IElement> result = document.QuerySelectorAll(query);
			if (requiredAttributes != null)
				result = result.Where(element => requiredAttributes.All(attribute =>
					element.GetAttribute(attribute.Key) == attribute.Value));
			return result;
		}
	}
}
