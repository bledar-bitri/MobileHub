using AutoMapper;
using Contracts;
using CustomerModel;

namespace Services
{
    public static class AutoMapperConfig
    {
        private static bool _isConfigured;

        public static void Configure()
        {
            if (_isConfigured) return;

            Mapper.Initialize(cfg => {

                cfg.CreateMap<Customer, CustomerContract>();

                cfg.CreateMap<CustomerModel.Address, AddressContract>()
                .ForMember(to => to.AddressId, opt => opt.MapFrom(from => from.Id))
                .ForMember(to => to.Id, opt => opt.Ignore());
                
            });

            _isConfigured = true;
        }
    }
}
