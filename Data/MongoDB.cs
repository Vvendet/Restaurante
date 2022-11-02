using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Restaurante.Data.Schemas;
using Restaurante.Domain.Entities;
using Restaurante.Domain.Enums;

namespace Restaurante.Data
{
    public class MongoDB
    {
        public IMongoDatabase DB { get; }
        public MongoDB(IConfiguration configuration)
        {
            try
            {
                var settings = MongoClientSettings.FromUrl(new MongoUrl(configuration["ConnectionString"]));
                var client = new MongoClient(settings);
                DB = client.GetDatabase(configuration["NomeBanco"]);
                MapClasses();
            }
            catch ( Exception ex)
            {
                throw new MongoException("Não foi possível conectar ao mongoDB", ex);
            }

        }
        private void MapClasses()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(RestaurantSchema)))
            {
                BsonClassMap.RegisterClassMap<RestaurantSchema>(i =>
                {
                    i.AutoMap();
                    i.MapIdMember(c => c.Id);
                    i.MapMember(c => c.Cozinha).SetSerializer(new EnumSerializer<ECozinha>(BsonType.Int32));
                    i.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
