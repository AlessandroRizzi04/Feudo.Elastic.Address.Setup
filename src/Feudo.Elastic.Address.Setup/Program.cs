using Feudo.Elastic.Address.Setup;
using Feudo.Elastic.Address.Setup.Services;
using Feudo.Elastic.Db.Address.Configuration;
using Feudo.Elastic.Db.Address.Osm.Factory;
using Feudo.Elastic.Db.Address.Service.Factory;
using Feudo.Elastic.Location.Setup;
using Feudo.Elastic.Location.Setup.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
IConfiguration configuration = builder.Configuration;

AddressElasticServicesConfigurations addressElasticServiceConfiguration = configuration.GetSection("Elasticsearch").Get<AddressElasticServicesConfigurations>();
ElasticIndexConfigurations elasticIndex = configuration.GetSection("ElasticsearchIndexes").Get<ElasticIndexConfigurations>();

builder.Services.AddSingleton<IAddressPointElasticServiceFactory, AddressPointElasticServiceFactory>(
    sp => new AddressPointElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.AddressPointIndex)
);
builder.Services.AddSingleton<ICityPointElasticServiceFactory, CityPointElasticServiceFactory>(
    sp => new CityPointElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.CityPointIndex)
);
builder.Services.AddSingleton<ICityPolygonElasticServiceFactory, CityPolygonElasticServiceFactory>(
    sp => new CityPolygonElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.CityPolygonIndex)
);
builder.Services.AddSingleton<IMunicipalityPolygonElasticServiceFactory, MunicipalityPolygonElasticServiceFactory>(
    sp => new MunicipalityPolygonElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.MunicipalityPolygonIndex)
);
builder.Services.AddSingleton<IProvincePolygonElasticServiceFactory, ProvincePolygonElasticServiceFactory>(
    sp => new ProvincePolygonElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.ProvincePolygonIndex)
);
builder.Services.AddSingleton<IRegionPolygonElasticServiceFactory, RegionPolygonElasticServiceFactory>(
    sp => new RegionPolygonElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.RegionPolygonIndex)
);
builder.Services.AddSingleton<IStreetLineElasticServiceFactory, StreetLineElasticServiceFactory>(
    sp => new StreetLineElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.StreetLineIndex)
);
builder.Services.AddSingleton<IOsmNodeElasticServiceFactory, OsmNodeElasticServiceFactory>(
    sp => new OsmNodeElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.OsmNodeIndex)
);
builder.Services.AddSingleton<IOsmWayElasticServiceFactory, OsmWayElasticServiceFactory>(
    sp => new OsmWayElasticServiceFactory(addressElasticServiceConfiguration, elasticIndex.OsmWayIndex)
);

builder.Services.AddTransient<IOsmService, OsmService>();
builder.Services.AddTransient<IAddressSetup, AddressSetup>();

using var app = builder.Build();
var service = app.Services.GetRequiredService<IAddressSetup>();
await service.Setup();