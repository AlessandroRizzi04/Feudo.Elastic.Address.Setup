using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feudo.Elastic.Location.Setup.Configuration
{
    public class ElasticIndexConfigurations
    {
        public string AddressPointIndex { get; set; }
        public string CityPointIndex { get; set; }
        public string CityPolygonIndex { get; set; }
        public string MunicipalityPolygonIndex { get; set; }
        public string ProvincePolygonIndex { get; set; }
        public string RegionPolygonIndex { get; set; }
        public string StreetLineIndex { get; set; }
        public string OsmNodeIndex { get; set; }
        public string OsmWayIndex { get; set; }
    }
}
