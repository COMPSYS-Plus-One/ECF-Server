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
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string company { get; set; }
        public string address_1 { get; set; }
        public string address_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class Shipping
    {
        public string fullAddress
        {
            get
            {
                return this.address_1 + " " + this.address_2 + " " + this.city + " " + this.postcode + " " + this.country;
            }
        }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string company { get; set; }
        public string address_1 { get; set; }
        public string address_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
    }

    public class MetaData
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public int id { get; set; }
        public string total { get; set; }
        public string subtotal { get; set; }
    }

    public class MetaData2
    {
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }

    public class LineItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public int product_id { get; set; }
        public int variation_id { get; set; }
        public int quantity { get; set; }
        public string tax_class { get; set; }
        public string subtotal { get; set; }
        public string subtotal_tax { get; set; }
        public string total { get; set; }
        public string total_tax { get; set; }
        public List<Tax> taxes { get; set; }
        public List<MetaData2> meta_data { get; set; }
        public string sku { get; set; }
        public float price { get; set; }
    }

    public class TaxLine
    {
        public int id { get; set; }
        public string rate_code { get; set; }
        public int rate_id { get; set; }
        public string label { get; set; }
        public bool compound { get; set; }
        public string tax_total { get; set; }
        public string shipping_tax_total { get; set; }
        public List<object> meta_data { get; set; }
    }

    public class ShippingLine
    {
        public int id { get; set; }
        public string method_title { get; set; }
        public string method_id { get; set; }
        public string total { get; set; }
        public string total_tax { get; set; }
        public List<object> taxes { get; set; }
        public List<object> meta_data { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Collection
    {
        public string href { get; set; }
    }

    public class Links
    {
        public List<Self> self { get; set; }
        public List<Collection> collection { get; set; }
    }

    public class Refund
    {
        public int id { get; set; }
        public string refund { get; set; }
        public string total { get; set; }
    }


    //--------order model class--------------------------------------------------------------
    public class RootOrder
    {
        public int id { get; set; }
        public int parent_id { get; set; }
        public string number { get; set; }
        public string order_key { get; set; }
        public string created_via { get; set; }
        public string version { get; set; }
        public string status { get; set; }
        public string currency { get; set; }

        //public DateTime date_created;
        //public DateTime date_created_gmt;
        //public DateTime date_modified;
        //public DateTime date_modified_gmt;
        public string discount_total { get; set; }
        public string discount_tax { get; set; }
        public string shipping_total { get; set; }
        public string shipping_tax { get; set; }
        public string cart_tax { get; set; }
        public string total { get; set; }
        public string total_tax { get; set; }
        public bool prices_include_tax { get; set; }
        public int customer_id { get; set; }
        public string customer_ip_address { get; set; }
        public string customer_user_agent { get; set; }
        public string customer_note { get; set; }
        public Billing billing { get; set; }
        public Shipping shipping { get; set; }
        public string payment_method { get; set; }
        public string payment_method_title { get; set; }
        public string transaction_id { get; set; }
        public object date_completed { get; set; }
        public object date_completed_gmt { get; set; }
        public string cart_hash { get; set; }
        public List<MetaData> meta_data { get; set; }
        public List<string> time_window
        {
            get
            {
                return new List<string>(new string[] { this.meta_data.Find(x => x.key == "ecf_schedule_time_earliest").value,
                                                       this.meta_data.Find(x => x.key == "ecf_schedule_time_latest").value });
            }
        }
        public List<LineItem> line_items { get; set; }
        public List<TaxLine> tax_lines { get; set; }
        public List<ShippingLine> shipping_lines { get; set; }
        public List<object> fee_lines { get; set; }
        public List<object> coupon_lines { get; set; }
        public List<object> refunds { get; set; }
        public Links _links { get; set; }

        public string serializeOrder()
        {
            APIorder O = new APIorder
            {
                customer_id = this.customer_id,
                total = this.total,
                status = this.status,
                line_items = this.line_items,
                firstName = this.shipping.first_name,
                lastName = this.shipping.last_name,
                address = this.shipping.address_1 + " " + this.shipping.address_2 + " " + this.shipping.city + " " + this.shipping.state + " " + this.shipping.postcode + " " + this.shipping.country,
                phone = this.billing.phone,
                customerNote = this.customer_note,
                date_completed = this.date_completed,
                meta_data = this.meta_data,
                time_window = new List<string>(new string[] { this.meta_data.Find(x => x.key == "ecf_schedule_time_earliest").value,
                                                       this.meta_data.Find(x => x.key == "ecf_schedule_time_latest").value })
            };
            return JsonConvert.SerializeObject(O);
        }
    }

    public class APIorder
    {
        //This class is for easy formatting for the API response
        public string response = "ok";
        public int customer_id;
        public string total;
        public string status;
        public string firstName;
        public string lastName;
        public string address;
        public string phone;
        public string customerNote;
        public object date_completed;
        public List<LineItem> line_items;
        public List<MetaData> meta_data;
        public List<string> time_window;
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

    //--------order note model class-----------------------------------------------------------
    public class RootOrderNote
    {
        public int id { get; set; }
        public string author { get; set; }
        public DateTime date_created;
        public DateTime date_created_gmt;
        public string note { get; set; }
        public bool customer_note { get; set; }
        public bool added_by_user { get; set; }

        public string serializeOrderNote()
        {
            APIOrderNote O = new APIOrderNote
            {
                note_id = this.id,
                author = this.author,
                date_created = this.date_created,
                date_created_gmt = this.date_created_gmt,
                note = this.note,
                customer_note = this.customer_note,

            };
            return JsonConvert.SerializeObject(O);
        }
    }

    public class APIOrderNote
    {
        //This class is for easy formatting for the API response
        public string response = "ok";
        public int note_id;
        public string author;
        public DateTime date_created;
        public DateTime date_created_gmt;
        public string note;
        public bool customer_note;
    }
}
