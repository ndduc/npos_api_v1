using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace POS_Api.Shared.HttpHelper
{
    public static class HttpResponseHelper
    {
        public static ObjectResult HttpResponse(dynamic body, HttpStatusCode statusCode)
        {
            ObjectResult result;
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
            dict.Add("statusCode", (int)statusCode);
            dict.Add("statusMessage", statusCode.ToString());
            dict.Add("body", body);
            result = new ObjectResult(dict)
            {
                StatusCode = (int)statusCode
            };
            return result;
        }
    }
}
