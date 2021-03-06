﻿using Logging;
using Logging.Interfaces;
using Ninject;
using Ninject.Modules;
using Services;

namespace TestServices
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IAddressService>().To<AddressService>();
            Bind<IRouteService>().To<RouteService>();
            Bind<ILogger>().To<ProgressQueueLogger>();
        }
    }
}
