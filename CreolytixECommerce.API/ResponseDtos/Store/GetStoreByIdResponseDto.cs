﻿namespace CreolytixECommerce.API.ResponseDtos.Store
{
    public class GetStoreByIdResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
