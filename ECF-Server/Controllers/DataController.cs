using System.Collections.Generic;
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
            RestCon.apiCreateOrderNote("PUT", "orders/" + id.ToString() + "/notes", note);
        }
    }

    public class ErrorResponse
    {
        public string response = "error";
        public string messgae;
    }
}
