using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feudo.Elastic.Db.Address.Model.Entity;
using Feudo.Elastic.Db.Address.Osm;
using Feudo.Elastic.Db.Address.Osm.Factory;
using OsmSharp;
using OsmSharp.Streams;

namespace Feudo.Elastic.Address.Setup.Services
{
    public class OsmService : IOsmService
    {
        private readonly IOsmNodeElasticServiceFactory _osmNodeElasticServiceFactory;
        private readonly IOsmWayElasticServiceFactory _osmWayElasticServiceFactory;

        public OsmService(IOsmNodeElasticServiceFactory osmNodeElasticServiceFactory, IOsmWayElasticServiceFactory osmWayElasticServiceFactory)
        {
            _osmNodeElasticServiceFactory = osmNodeElasticServiceFactory;
            _osmWayElasticServiceFactory = osmWayElasticServiceFactory;
        }

        public async Task Index(string osmFilePath)
        {
            Stream fileStream;
            if (osmFilePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(osmFilePath, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                fileStream = await response.Content.ReadAsStreamAsync();
            }
            else
            {
                fileStream = File.OpenRead(osmFilePath);
            }
            var source = new PBFOsmStreamSource(fileStream);

            var osmNodeElasticService = _osmNodeElasticServiceFactory.Create();
            var osmWayElasticService = _osmWayElasticServiceFactory.Create();

            var nodeList = new List<OsmNode>();
            var wayList = new List<OsmWay>();

            int batchSize = 5000;
            foreach (var item in source)
            {
                switch (item.Type)
                {
                    case OsmGeoType.Node:
                        Node node = (Node)item;
                        if (node.Id != null && node.Latitude != null && node.Longitude != null)
                        {
                            node.Tags.TryGetValue("name", out string name);
                            node.Tags.TryGetValue("type", out string type);
                            nodeList.Add(new OsmNode()
                            {
                                Id = (long)node.Id,
                                Name = name,
                                Type = type,
                                Location = new Nest.GeoLocation((double)node.Latitude, (double)node.Longitude),
                            });
                        }
                        break;

                    case OsmGeoType.Way:
                        Way way = (Way)item;
                        if (way.Id != null)
                        {
                            way.Tags.TryGetValue("name", out string name);
                            way.Tags.TryGetValue("type", out string type);
                            var wayDocument = new OsmWay()
                            {
                                Id = (long)way.Id,
                                Name = name,
                                Type = type,
                                Nodes = way.Nodes.ToList(),
                            };
                            wayList.Add(wayDocument);
                        }
                        break;
                }

                if (nodeList.Count >= batchSize)
                {
                    await osmNodeElasticService.BulkIndex(nodeList);
                    nodeList.Clear();
                }

                if (wayList.Count >= batchSize)
                {
                    await osmWayElasticService.BulkIndex(wayList);
                    wayList.Clear();
                }
            }

            if (nodeList.Count > 0)
            {
                await osmNodeElasticService.BulkIndex(nodeList);
            }

            if (wayList.Count > 0)
            {
                await osmWayElasticService.BulkIndex(wayList);
            }
        }
    }
}
