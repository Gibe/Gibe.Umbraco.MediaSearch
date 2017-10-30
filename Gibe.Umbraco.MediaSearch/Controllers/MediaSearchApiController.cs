using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Umbraco.Web.WebApi;

namespace UmbracoIntelligentMedia.Controllers
{
	public class MediaSearchApiController : UmbracoApiController
	{
		private readonly ISearchIndexClient _client;

		public MediaSearchApiController()
		{
			_client = new SearchIndexClient("umbracomedia", "umbraco", new SearchCredentials("FCEB27B0FA292CA9149892B5494669F5"));
		}

		[HttpGet, HttpPost]
		public SearchResults Search([FromBody]IEnumerable<Facet> facets, [FromUri]string term)
		{
			var parameters = new SearchParameters
			{
				Facets = new[] { "tags,count:25", "categories,count:25", "primaryColour","backgroundColour","numberOfFaces"}
			};

			if (facets != null)
			{
				var queries = new List<string>();
				foreach (var facet in facets)
				{
					if (facet.Values.Any(f => f.Selected))
					{
						if (facet.Key == "tags" || facet.Key == "categories") // TODO : Base on azure search type
						{
							queries.AddRange(facet.Values.Where(f => f.Selected).Select(t => $"{facet.Key}/any(t: t eq '{t.Value}')"));
						}
						else if (facet.Key == "numberOfFaces")
						{
							queries.AddRange(facet.Values.Where(f => f.Selected).Select(t => $"{facet.Key} eq {t.Value}"));
						}
						else 
						{
							queries.AddRange(facet.Values.Where(f => f.Selected).Select(t => $"{facet.Key} eq '{t.Value}'"));
						}

					}
				}
				parameters.Filter = String.Join(" and ", queries);
			}
			
			var results = _client.Documents.Search<MediaResult>(String.IsNullOrEmpty(term)?"*":term, parameters);

			return new SearchResults(results.Results.Select(r => Convert.ToInt32(r.Document.Id)), ConvertFacets(results));
		}

		private IEnumerable<Facet> ConvertFacets(DocumentSearchResult<MediaResult> results)
		{
			var facets = new List<Facet>();
			foreach (var result in results.Facets.Keys)
			{
				facets.Add(new Facet
				{
					Key = result,
					Values = results
						.Facets[result]
						.Select(r => new FacetValue {Count = r.Count, Value = r.Value, Selected = false})
				});
			}
			return facets;
		}
	}

	

	public class SearchFilters
	{
		public IEnumerable<Facet> Facets { get; set; }
	}

	public class FacetFilter
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	[SerializePropertyNamesAsCamelCase]
	public class SearchResults
	{
		public SearchResults(IEnumerable<int> mediaIds, IEnumerable<Facet> facets)
		{
			MediaIds = mediaIds;
			Facets = facets;
		}

		public IEnumerable<int> MediaIds { get; }
		public IEnumerable<Facet> Facets { get; }
	}

	[SerializePropertyNamesAsCamelCase]
	public class Facet
	{
		public string Key { get; set; }
		public IEnumerable<FacetValue> Values { get; set; }
	}

	[SerializePropertyNamesAsCamelCase]
	public class FacetValue
	{
		public object Value { get; set; }
		public long? Count { get; set; }
		public bool Selected { get; set; }
	}

	[SerializePropertyNamesAsCamelCase]
	public class MediaResult
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string UmbracoFile { get; set; }
		public string[] Tags { get; set; }
		public string[] Categories { get; set; }
	}
}