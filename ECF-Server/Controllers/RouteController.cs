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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECF_Server.Controllers
{
    [Route("api/route")]
    [ApiController]
    public class RouteController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public RouteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            
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
        public List<string> Post([FromBody] AddressList addressList)
        {
            RouteOptimization routeOptimization = new RouteOptimization(_httpClientFactory.CreateClient());
            //List<string> sortedAddresses = routeOptimization.route(addressList.addresses);
            return routeOptimization.route(addressList.addresses);
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
        [HttpGet]
        public List<List<RootOrder>> GetCurrentOrdersRoute()
        {
            List<List<string>> areaGroups = new List<List<string>>()
            {
                new List<string>()
                {
                    "0630"
                },
                new List<string>()
                {
                    "0632"
                }
            };
            var restConsumer = new RESTconsumer();
            var currentOrderList = restConsumer.apiRequestOrderList("GET", "orders").Where(x => x.status == "processing").ToList();
            //Sort orders by area HERE into a nested list of orders, then run this in a loop
            //Create a list of orders for each List of areas, plus an additional one for any non-conforming areas
            var ordersByArea = new List<List<RootOrder>>();
           
            for(int i = 0; i <= areaGroups.Count(); i++)
            {
                ordersByArea.Add(new List<RootOrder>());
            }
            //Go through each order and check the if the postcode is in each list, if so, copy it to correct list, else put it in spare list
           

            foreach (var order in currentOrderList){                
                bool isInArea = false;

                for(int i = 0; i < areaGroups.Count(); i++)
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
            string depotAddress = "1 Grafton Road, Auckland CBD, Auckland 1010";

            //Then loop through this list of orders, performing the route optimisation for each one.
            //Will need to add the depot location (ie ellis creek farms - we should put this values somewehere nice)

            //Structure to store order in
            var optimisedOrdersList = new List<List<RootOrder>>();
            foreach (var orderList in ordersByArea)
            {
                if(orderList.Count == 0)
                {
                    break;
                }
                List<string> addressList = new List<string>();
                addressList.Add(depotAddress);
                //Add each of the orders full address to the  list
                addressList.AddRange(orderList.Select(x => x.shipping.fullAddress).ToList());
                
                //Also need to add the depot location here
                RouteOptimization routeOptimization = new RouteOptimization(_httpClientFactory.CreateClient());
                List<string> optimisedRoute = routeOptimization.route(addressList);
                foreach (string a in optimisedRoute)
                {
                    Console.WriteLine(a);
                }
                var optimisedOrders = orderList.OrderBy(x => optimisedRoute.IndexOf(x.shipping.fullAddress)).ToList();
                optimisedOrdersList.Add(optimisedOrders);
            }


            return optimisedOrdersList;
        }

       
    }
}
