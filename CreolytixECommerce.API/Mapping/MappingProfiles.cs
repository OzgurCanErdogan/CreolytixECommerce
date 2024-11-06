using AutoMapper;
using CreolytixECommerce.API.ResponseDtos.Inventory;
using CreolytixECommerce.API.ResponseDtos.Product;
using CreolytixECommerce.API.ResponseDtos.Reservation;
using CreolytixECommerce.API.ResponseDtos.Store;
using CreolytixECommerce.Application.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreolytixECommerce.API.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //Inventory Mapping
            CreateMap<InventoryDto, UpdateInventoryResponseDto>();

            //Product Mapping
            CreateMap<ProductDto, GetProductByIdResponseDto>();
            CreateMap<ProductDto, GetProductsByCategoryResponseDto>();
            CreateMap<AvailableStoreDto, GetProductAvailabilityResponseDto>();

            //Reservation Mapping
            CreateMap<ReservationDto, CreateReservationResponseDto>();
            CreateMap<ReservationDto, GetReservationByIdResponseDto>();

            //Store Mapping
            CreateMap<StoreDto, StoreProductResponseDto>();
            CreateMap<ProductInventoryDto, ProductInventoryResponseDto>();
            CreateMap<StoreProductDto, GetStoreProductsResponseDto>();
            CreateMap<StoreDto, GetNearbyStoresResponseDto>();
            CreateMap<StoreDto, GetStoreByIdResponseDto>();
        }
    }
}
