using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECF_Server.Controllers
{
    [Route("api/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private IConfiguration configuration;
        public DataController(IConfiguration iConfig)
        {
            configuration = iConfig;
            RestCon = new RESTconsumer(configuration);
        }
        RESTconsumer RestCon;
        private List<RootOrder> orders;
        // GET: api/data
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "tes" };
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
        /*[HttpGet("load_customer_details/{id}")]
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
            
        }*/

        // GET api/data/get_order_note/5/5
        // get all the order notes by its order id
        /*[HttpGet("get_order_note/{order_id}/{note_id}")]
        public string GetOrderNotes( int order_id, int note_id)
        {
            try
            {
                var orderNote = RestCon.apiGetOrderNote("GET", "orders/" + order_id.ToString() + "/notes/" + note_id.ToString());
                return orderNote.serializeOrderNote();
            }
            catch
            {
                ErrorResponse E = new ErrorResponse
                {
                    messgae = "ID does not match an order"
                };
                return JsonConvert.SerializeObject(E);
            }
        }*/

        // PUT api/data/confirm_order/5
        [HttpPut("confirm_order/{id}")]
        public void ConfirmOrder(int id)
        {
            RestCon.apiUpdateOrder("PUT", "orders/" + id.ToString() );
        }

        // POST api/data/create_note/5
        // pass the order id and the json format message as the body
        [HttpPut("create_note/{id}")]
        public void CreateOrderNote([FromBody] string note, int id)
        {
            RestCon.apiCreateOrderNote("PUT", "orders/" + id.ToString(), note);
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

        // PUT api/data/set_delivery_time_window_earliest/5
        [HttpPut("set_delivery_time_window_earliest/{id}")]
        public void CreateDriverTimeEarliest([FromBody] string timeConstraint, int id)
        {
            RestCon.apiSetDelieveryWindowEarliest("PUT", "orders/" + id.ToString(), timeConstraint);
        }

        // PUT api/data/set_delivery_time_window_latest/5
        [HttpPut("set_delivery_time_window_latest/{id}")]
        public void CreateDriverTimeLatest([FromBody] string timeConstraint, int id)
        {
            RestCon.apiSetDelieveryWindowLatest("PUT", "orders/" + id.ToString(), timeConstraint);
        }

        /*
        // GET api/data/get_delivery_time_window_earliest/5
        // pass the order id and the json format message as the body
        [HttpPost("get_delivery_time_window_earliest/{id}")]
        public string GetDriverTimeWindow([FromBody] int id)
        {
            try
            {
                var orderNote = RestCon.apiGetDelieveryWindowEarliest("POST", "orders/" + id.ToString());
                return orderNote.serializeOrderNote();
            }
            catch
            {
                ErrorResponse E = new ErrorResponse
                {
                    messgae = "ID does not match an order"
                };
                return JsonConvert.SerializeObject(E);
            }
        }*/


    }

    public class ErrorResponse
    {
        public string response = "error";
        public string messgae;
    }
}
