using Newtonsoft.Json;
using OAuth;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Extensions.Configuration;
namespace ECF_Server
{
    public class RESTconsumer
    {
        private static string consumerKey;
        private static string consumerSecret;
        private static string oauthToken;
        private static string oauthTokenSecret;
        private static string mainURL;

        private IConfiguration configuration;
        public RESTconsumer(IConfiguration iConfig)
        {
            configuration = iConfig;
            consumerKey = configuration.GetValue<string>("WooCommerce:consumerKey");
            consumerSecret = configuration.GetValue<string>("WooCommerce:consumerSecret");
            oauthToken = configuration.GetValue<string>("WooCommerce:oauthToken");
            oauthTokenSecret = configuration.GetValue<string>("WooCommerce:oauthTokenSecret");
            mainURL = configuration.GetValue<string>("WooCommerce:mainURL");
        }

        /// <summary>
        /// This method is used for make GET API calls.
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="URL">The API endpoint URL.</param>
        /// <returns>The response of the API request.</returns>
        public static IRestResponse sendRequest(string HTTPMethod, Uri URL)
        {
            var client = new RestClient(URL);
            var request = new RestRequest(Method.GET);

            if (HTTPMethod.Equals("POST"))
            {
                request = new RestRequest(Method.POST);

            }

            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedURl;
            string normalizedParameter;

            string signature = oAuth.GenerateSignature(URL, consumerKey, consumerSecret, oauthToken, oauthTokenSecret, HTTPMethod, timeStamp, nonce, out normalizedURl, out normalizedParameter);
            signature = HttpUtility.UrlEncode(signature);
          
            request.AddParameter("oauth_consumer_key", consumerKey);
            request.AddParameter("oauth_token", oauthToken);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_timestamp", timeStamp);
            request.AddParameter("oauth_nonce", nonce);
            
            request.AddParameter("oauth_version", "1.0");
            request.AddParameter("oauth_signature", signature);
     
            return client.Execute(request);
        }

        /// <summary>
        /// This method is used to send put request.
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="URL">The API endpoint URL.</param>
        /// <param name="body">The body of the request.</param>
        /// <returns> The response of the request.</returns>
        public static IRestResponse sendPut(string HTTPMethod, Uri URL, string body)
        {
            // It should be fused with the one above it.
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string normalizedURl;
            string normalizedParameter;

            string signature = oAuth.GenerateSignature(URL, consumerKey, consumerSecret, oauthToken, oauthTokenSecret, HTTPMethod, timeStamp, nonce, out normalizedURl, out normalizedParameter);
            signature = HttpUtility.UrlEncode(signature);

            String url = URL + "?oauth_consumer_key=" + consumerKey + "&oauth_token=" + oauthToken + "&oauth_signature_method=HMAC-SHA1" + "&oauth_timestamp=" + timeStamp + "&oauth_nonce=" + nonce + "&oauth_version=1.0" + "&oauth_signature=" + signature;
            var client = new RestClient(url);

            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            return client.Execute(request);
        }

        /// <summary>
        /// This method is used for getting the list of all orders.
        /// </summary>
        /// <param name="HTTPMethod"> HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL"> The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        /// <returns> list of all the orders in the WooCommerce database</returns>
        public List<RootOrder> apiRequestOrderList(string HTTPMethod, string requestURL)
        {
            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = sendRequest(HTTPMethod, BASE_URL);

            return JsonConvert.DeserializeObject<List<RootOrder>>(response.Content);
        }

        /// <summary>
        /// This method is used for getting a single order using its ID
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL">The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        /// <returns> The order and all its parameters</returns>
        public RootOrder apiRequestOrder(string HTTPMethod, string requestURL)
        {

            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = sendRequest(HTTPMethod, BASE_URL);
            //Console.WriteLine(response.Content);
            return JsonConvert.DeserializeObject<RootOrder>(response.Content);
        }

        /// <summary>
        /// This method is used for updataing the order status to completed
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL">The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        public void apiUpdateOrder(string HTTPMethod, string requestURL)
        {

            var BASE_URL = new Uri(mainURL + requestURL);
            string body = "{\n  \"status\": \"completed\"\n}";
            IRestResponse response = sendPut(HTTPMethod, BASE_URL, body);
        }

        /// <summary>
        /// This method is used to create a delivery note on the WooCommerce site
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL">The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        /// <param name="note">The delivery note the is saved to the database</param>
        public void apiCreateOrderNote(string HTTPMethod, string requestURL, string note)
        {

            var BASE_URL = new Uri(mainURL + requestURL);
            string body = "{" +
                "\"meta_data\":[ " +
                "{ \"key\":\"ecf_delivery_notes\", \"value\": \"" + note + "\"}" +
                "]" +
                "}";
            IRestResponse response = sendPut(HTTPMethod, BASE_URL, body);
        }
        
        /// <summary>
        /// This method is used to set the earliset time window for a order
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL">The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        /// <param name="timeConstraint">The time that is to be set</param>
        public void apiSetDelieveryWindowEarliest(string HTTPMethod, string requestURL, string timeConstraint)
        {

            var BASE_URL = new Uri(mainURL + requestURL);
            string body = "{" +
                "\"meta_data\":[ " +
                "{ \"key\":\"ecf_schedule_time_earliest\", \"value\": \""+ timeConstraint + "\"}" +
                "]" +
                "}";
            IRestResponse response = sendPut(HTTPMethod, BASE_URL, body);
        }

        /// <summary>
        /// This method is used to set the latest time window for a order
        /// </summary>
        /// <param name="HTTPMethod">HTTP request method e.g GET, POST, PUT, etc.</param>
        /// <param name="requestURL">The extension to be added to the base URL. Where the base URL is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/ </param>
        /// <param name="timeConstraint">The time that is to be set</param>
        public void apiSetDelieveryWindowLatest(string HTTPMethod, string requestURL, string timeConstraint)
        {

            var BASE_URL = new Uri(mainURL + requestURL);
            string body = "{" +
                "\"meta_data\":[ " +
                "{ \"key\":\"ecf_schedule_time_latest\", \"value\": \"" + timeConstraint + "\"}" +
                "]" +
                "}";
            IRestResponse response = sendPut(HTTPMethod, BASE_URL, body);
        }
    }
}
