using CreolytixECommerce.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Domain.Entities
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string StoreId { get; set; }
        public string ProductId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
