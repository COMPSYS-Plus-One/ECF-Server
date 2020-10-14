using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using ECF_Server.Models;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECF_Server.Controllers
{
    [Route("api/route")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private IConfiguration configuration;

        private readonly IHttpClientFactory _httpClientFactory;
        public RouteController(IHttpClientFactory httpClientFactory, IConfiguration iConfig)
        {
            _httpClientFactory = httpClientFactory;
            configuration = iConfig;
        }


        // GET: api/<RouteController>
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };



        }

        // GET api/<RouteController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RouteController>
        [HttpPost]
        public List<string> Post([FromBody] RouteModel routeModel)
        {
            string googleApiKey = configuration.GetValue<string>("GoogleAPIKey");

            RouteOptimization routeOptimization = new RouteOptimization(_httpClientFactory.CreateClient(), googleApiKey);

            //List<string> sortedAddresses = routeOptimization.route(addressList.addresses);
            return routeOptimization.route(routeModel.addressList.addresses, routeModel.timeWindows);
        }


        // PUT api/<RouteController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RouteController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }



        [Route("orders")]
        [HttpPost]
        public List<DeliveryRoute> GetCurrentOrdersRoute([FromBody] DeliveryAreas deliveryAreas)
        {
            var areaGroups = deliveryAreas.deliveryAreas.Select(x => x.postcodes).ToList();
            var restConsumer = new RESTconsumer(configuration);
            var currentOrderList = restConsumer.apiRequestOrderList("GET", "orders").Where(x => x.status == "processing").ToList();


            //Sort orders by area HERE into a nested list of orders, then run this in a loop
            //Create a list of orders for each List of areas, plus an additional one for any non-conforming areas
            var ordersByArea = new List<List<RootOrder>>();

            for (int i = 0; i <= areaGroups.Count(); i++)
            {
                ordersByArea.Add(new List<RootOrder>());
            }
            //Go through each order and check the if the postcode is in each list, if so, copy it to correct list, else put it in spare list


            foreach (var order in currentOrderList)
            {
                bool isInArea = false;

                for (int i = 0; i < areaGroups.Count(); i++)
                {
                    if (areaGroups[i].Contains(order.shipping.postcode))
                    {
                        ordersByArea[i].Add(order);
                        isInArea = true;
                        break;
                    }
                }
                if (!isInArea)
                {
                    ordersByArea.Last().Add(order);
                }


            }

            //Replce this with DEPOT Adress, should probavbly be stored in config, or provided via the API call

            string depotAddress = configuration.GetValue<string>("Routing:DepotAddress");

            //Then loop through this list of orders, performing the route optimisation for each one.
            //Will need to add the depot location (ie ellis creek farms - we should put this values somewehere nice)

            //Structure to store order in
            int index = 0;
            var deliveryRoutes = new List<DeliveryRoute>();
            foreach (var orderList in ordersByArea)
            {
                string name;
                //Get the name of the delivery area
                if (index < deliveryAreas.deliveryAreas.Count())
                {
                    name = deliveryAreas.deliveryAreas[index].name;
                }
                else
                {
                    name = "Other";
                }
                index++;

                //If the list of orders for an area is emtpy, skip the area
                if (orderList.Count == 0)
                {
                    continue;
                }

                var deliveryRoute = new DeliveryRoute();
                deliveryRoute.name = name;

                List<string> addressList = new List<string>();
                addressList.Add(depotAddress);
                //Add each of the orders full address to the  list
                addressList.AddRange(orderList.Select(x => x.shipping.fullAddress).ToList());

                //Also need to add the depot location here
                string googleApiKey = configuration.GetValue<string>("GoogleAPIKey");
                RouteOptimization routeOptimization = new RouteOptimization(_httpClientFactory.CreateClient(), googleApiKey);

                long[,] timeWindows = new long[addressList.Count(), 2];
                timeWindows[0, 0] = 0;
                timeWindows[0, 1] = 999999999999;               // for the depot, no  time constraints
                for (int i = 1; i <= orderList.Count(); i++)
                {
                    if (orderList[i - 1].time_window is null)
                    {
                        timeWindows[i, 0] = 0;
                        timeWindows[i, 1] = 999999999999;
                    }
                    else
                    {
                        var startWindow = orderList[i - 1].time_window[0];
                        var endWindow = orderList[i - 1].time_window[1];

                        if (String.IsNullOrEmpty(startWindow))
                        {
                            timeWindows[i, 0] = 0;

                        }
                        else
                        {
                            timeWindows[i, 0] = (long)(TimeSpan.Parse(orderList[i - 1].time_window[0]).TotalSeconds);
                        }

                        if (String.IsNullOrEmpty(endWindow))
                        {
                            timeWindows[i, 1] = 999999999999;
                        }
                        else
                        {
                            timeWindows[i, 1] = (long)(TimeSpan.Parse(orderList[i - 1].time_window[1]).TotalSeconds);
                        }
                    }


                }

                List<string> optimisedRoute = routeOptimization.route(addressList, timeWindows);

                var optimisedOrders = orderList.OrderBy(x => optimisedRoute.IndexOf(x.shipping.fullAddress)).ToList();
                deliveryRoute.orders = optimisedOrders;
                deliveryRoute.optimisedRoute = optimisedRoute;

                deliveryRoutes.Add(deliveryRoute);
            }


            return deliveryRoutes;
        }


    }
}
