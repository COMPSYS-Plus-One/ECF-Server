using Newtonsoft.Json;
using OAuth;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Web;

namespace ECF_Server
{
    public class RESTconsumer
    {
        private static string consumerKey = "pY4pDjtTksw8";
        private static string consumerSecret = "sDx7VxCNWxvJNOoKP4kf4L1kBSiKXOv0GDeZx9zYqkkyTo3s";
        private static string oauthToken = "TxE5rFEcrzsfEXqQyse691fd";
        private static string oauthTokenSecret = "ZFBNqeUHkYNmL5w94CdrtjwUa3ewMvDMPDUamgCpKYIQ5PJd";
        private static string mainURL = "https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/";

        public RESTconsumer()
        {

        }

        public static IRestResponse sendRequest(string HTTPMethod, Uri URL)
        {
            // This method is used for making api calls
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // URL - The api url for the request 
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

        public static IRestResponse confirmDelivery(string HTTPMethod, Uri URL)
        {
            // This method is used for making api calls
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // URL - The api url for the request 
         
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
            request.AddParameter("application/json", "{\n  \"status\": \"completed\"\n}", ParameterType.RequestBody);

            return client.Execute(request);
        }

        public static IRestResponse createOrderNote(string HTTPMethod, Uri URL, string note)
        {
            // This method is used for making api calls
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // URL - The api url for the request
            // note - The note added to the order

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
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\n  \"note\": \""+ note +"\"\n}", ParameterType.RequestBody);

            return client.Execute(request);
        }

        public List<RootOrder> apiRequestOrderList(string HTTPMethod, string requestURL)
        {
            // This method is used for the orders API call it returns a list of all the orders.
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // requestURL - The extension to be added to the base url. Where the base url is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/  

            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = sendRequest(HTTPMethod, BASE_URL);

            return JsonConvert.DeserializeObject<List<RootOrder>>(response.Content);
        }

        public RootOrder apiRequestOrder(string HTTPMethod, string requestURL)
        {
            // This method is used for the orders/{id} API call that returns the order data
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // requestURL - The extension to be added to the base url. Where the base url is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/  

            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = sendRequest(HTTPMethod, BASE_URL);
            //Console.WriteLine(response.Content);
            return JsonConvert.DeserializeObject<RootOrder>(response.Content);
        }

        public RootCustomer apiRequestCustomerInfo(string HTTPMethod, string requestURL)
        {
            // This method is used for the orders/{id} API call that returns the order data
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // requestURL - The extension to be added to the base url. Where the base url is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/  

            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = sendRequest(HTTPMethod, BASE_URL);
            //Console.WriteLine(response.Content);
            
            return JsonConvert.DeserializeObject<RootCustomer>(response.Content);
        }

        public RootOrder apiUpdateOrder(string HTTPMethod, string requestURL)
        {
            // This method is used for the orders/{id} API call that returns the order data
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // requestURL - The extension to be added to the base url. Where the base url is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/  

            var BASE_URL = new Uri(mainURL + requestURL);

            IRestResponse response = confirmDelivery(HTTPMethod, BASE_URL);
            //Console.WriteLine(response.Content);
            return JsonConvert.DeserializeObject<RootOrder>(response.Content);
        }

        public RootOrderNote apiCreateOrderNote(string HTTPMethod, string requestURL, string note)
        {
            // This method is used for the orders/{id} API call that returns the order data
            // HTTPMethod - The HTTP request method. e.g GET, POST, etc.
            // requestURL - The extension to be added to the base url. Where the base url is: https://epic.elliscreekfarm.co.nz/wp-json/wc/v3/  
            // note - The note added to the order

            var BASE_URL = new Uri(mainURL + requestURL);
            IRestResponse response = createOrderNote(HTTPMethod, BASE_URL, note);

            //Console.WriteLine(response.Content);
            return JsonConvert.DeserializeObject<RootOrderNote>(response.Content);
        }
    }
}
