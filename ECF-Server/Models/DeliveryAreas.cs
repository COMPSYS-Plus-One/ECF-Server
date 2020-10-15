using System.Collections.Generic;

namespace ECF_Server.Models
{
    public class DeliveryAreas
    {
        public List<DeliveryArea> deliveryAreas { get; set; }
    }
    public class DeliveryArea
    {
        public string name { get; set; }
        public List<string> postcodes { get; set; }
    }
    public class DeliveryRoute
    {
        public string name { get; set; }
        public List<string> optimisedRoute { get; set; }
        public List<RootOrder> orders { get; set; }
    }
}
