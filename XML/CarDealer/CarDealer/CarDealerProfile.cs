using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<supplilerDto, Supplier>();
            CreateMap<partsDto, Part>();
            CreateMap<carsDto, Car>();
            CreateMap<customersDto, Customer>();
            CreateMap<importSalesDto, Sale>();

        }
    }
}
