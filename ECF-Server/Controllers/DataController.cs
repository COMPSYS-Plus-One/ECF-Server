using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECF_Server.Controllers
{
    [Route("api/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        RESTconsumer RestCon = new RESTconsumer();
        private List<RootOrder> orders;
        // GET: api/data
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "tes", "test" };
        }

        // GET api/data/get_order/5
        [HttpGet("get_order/{id}")]
        public string GetOrder(int id)
        {
            try {
                var order = RestCon.apiRequestOrder("GET", "orders/"+id.ToString());
                return order.serializeOrder();
            }
            catch
            {
                ErrorResponse E = new ErrorResponse
                {
                    messgae = "ID does not match an order"
                };
                return JsonConvert.SerializeObject(E);
            }
        }

        // GET api/data/5
        [HttpGet("load_customer_details/{id}")]
        public string GetCustomer(int id)
        {
            try
            {
                var customer = RestCon.apiRequestCustomerInfo("GET", "customers/" + id.ToString());
                return customer.serializeOrder();
            }
            catch
            {
                ErrorResponse E = new ErrorResponse
                {
                    messgae = "ID does not match an order"
                };
                return JsonConvert.SerializeObject(E);
            }
        }

        // POST api/data
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/data/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/data/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class ErrorResponse
    {
        public string response = "error";
        public string messgae;
    }
}
