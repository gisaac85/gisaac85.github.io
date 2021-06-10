using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class OrderToUpdateDto
    {
        public int Id { get; set; }
        public string basketId { get; set; }
        public string Email { get; set; }      
        public string ShipToAddress_FirstName { get; set; }
        public string ShipToAddress_LastName { get; set; }
        public string ShipToAddress_Street { get; set; }
        public string ShipToAddress_City { get; set; }
        public string ShipToAddress_State { get; set; }
        public string ShipToAddress_Zipcode { get; set; }
        public int DeliveryMethodId { get; set; }          
    }
}
