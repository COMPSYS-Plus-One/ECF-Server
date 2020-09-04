using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            routeOptimization.test(addressList.addresses);
            return addressList.addresses;
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
    }
}
