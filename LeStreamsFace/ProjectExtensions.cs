using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;

namespace LeStreamsFace
{
    internal static class ProjectExtensions
    {
        public static IEnumerable<Stream> Favorites(this IEnumerable<Stream> streamEnumeration)
        {
            return streamEnumeration.Where(stream => stream.IsFavorite);
        }

        public static void ThrowExceptions(this IRestResponse restResponse)
        {
            if (restResponse.ErrorException != null)
            {
                throw restResponse.ErrorException;
            }
            if (restResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new WebException("404");
            }
        }

        public static IRestResponse SinglePageResponse(this IRestClient restClient)
        {
            var response = restClient.Execute(new RestRequest());
            response.ThrowExceptions();
            return response;
        }
    }
}