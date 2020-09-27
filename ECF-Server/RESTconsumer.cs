﻿using Newtonsoft.Json;
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

            request.AddParameter("oauth_nonce", nonce);
            request.AddParameter("oauth_timestamp", timeStamp);
            request.AddParameter("oauth_signature_method", "HMAC-SHA1");
            request.AddParameter("oauth_version", "1.0");
            request.AddParameter("oauth_signature", signature);

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
    }
}