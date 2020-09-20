using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECF_Server
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Billing
    {
        public string first_name;
        public string last_name;
        public string company;
        public string address_1;
        public string address_2;
        public string city;
        public string state;
        public string postcode;
        public string country;
        public string email;
        public string phone;
    }

    public class Shipping
    {
        public string first_name;
        public string last_name;
        public string company;
        public string address_1;
        public string address_2;
        public string city;
        public string state;
        public string postcode;
        public string country;
    }

    public class MetaData
    {
        public int id;
        public string key;
        public string value;
    }

    public class Tax
    {
        public int id;
        public string total;
        public string subtotal;
    }

    public class MetaData2
    {
        public int id;
        public string key;
        public string value;
    }

    public class LineItem
    {
        public int id;
        public string name;
        public int product_id;
        public int variation_id;
        public int quantity;
        public string tax_class;
        public string subtotal;
        public string subtotal_tax;
        public string total;
        public string total_tax;
        public List<Tax> taxes;
        public List<MetaData2> meta_data;
        public string sku;
        public int price;
    }

    public class TaxLine
    {
        public int id;
        public string rate_code;
        public int rate_id;
        public string label;
        public bool compound;
        public string tax_total;
        public string shipping_tax_total;
        public List<object> meta_data;
    }

    public class ShippingLine
    {
        public int id;
        public string method_title;
        public string method_id;
        public string total;
        public string total_tax;
        public List<object> taxes;
        public List<object> meta_data;
    }

    public class Self
    {
        public string href;
    }

    public class Collection
    {
        public string href;
    }

    public class Links
    {
        public List<Self> self;
        public List<Collection> collection;
    }

    public class Refund
    {
        public int id;
        public string refund;
        public string total;
    }
    

    //--------order model class--------------------------------------------------------------
    public class RootOrder
    {
        public int id;
        public int parent_id;
        public string number;
        public string order_key;
        public string created_via;
        public string version;
        public string status;
        public string currency;
        //public DateTime date_created;
        //public DateTime date_created_gmt;
        //public DateTime date_modified;
        //public DateTime date_modified_gmt;
        public string discount_total;
        public string discount_tax;
        public string shipping_total;
        public string shipping_tax;
        public string cart_tax;
        public string total;
        public string total_tax;
        public bool prices_include_tax;
        public int customer_id;
        public string customer_ip_address;
        public string customer_user_agent;
        public string customer_note;
        public Billing billing;
        public Shipping shipping;
        public string payment_method;
        public string payment_method_title;
        public string transaction_id;
        public object date_completed;
        public object date_completed_gmt;
        public string cart_hash;
        public List<MetaData> meta_data;
        public List<LineItem> line_items;
        public List<TaxLine> tax_lines;
        public List<ShippingLine> shipping_lines;
        public List<object> fee_lines;
        public List<object> coupon_lines;
        public List<object> refunds;
        public Links _links;

        public string serializeOrder()
        {
            APIorder O = new APIorder
            {
                customer_id = this.customer_id,
                total = this.total,
                status = this.status,
                line_items = this.line_items
                
            };
            return JsonConvert.SerializeObject(O);
        }
    }

    public class APIorder{
        //This class is for easy formatting for the API response
        public string response = "ok";
        public int customer_id;
        public string total;
        public string status;
        public List<LineItem> line_items;
    }

    //--------customer model class--------------------------------------------------------------
    public class RootCustomer
    {
        public int id { get; set; }
        //public DateTime date_created { get; set; }
        //public DateTime date_created_gmt { get; set; }
        //public DateTime date_modified { get; set; }
        //public DateTime date_modified_gmt { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string role { get; set; }
        public string username { get; set; }
        public Billing billing { get; set; }
        public Shipping shipping { get; set; }
        public bool is_paying_customer { get; set; }
        public string avatar_url { get; set; }
        public List<object> meta_data { get; set; }
        public Links _links { get; set; }

        public string serializeOrder()
        {
            APIcustomer O = new APIcustomer
            {
                customer_id = this.id,
                name = this.first_name + " " + this.last_name,
                address = this.shipping.address_1 + " " + this.shipping.address_2 + " " + this.shipping.city + " " + this.shipping.state + " " + this.shipping.postcode + " " + this.shipping.country,
                phone = this.billing.phone
            };
            return JsonConvert.SerializeObject(O);
        }
    }

    public class APIcustomer
    {
        //This class is for easy formatting for the API response
        public string response = "ok";
        public int customer_id;
        public string name;
        public string address;
        public string phone;
    }
}
