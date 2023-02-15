using Microsoft.Azure.Cosmos;
using MvcCoreCochesCosmosDb.Models;

namespace MvcCoreCochesCosmosDb.Services
{
    public class ServiceCochesCosmos
    {
        //SE TRABAJA CON ITEMS CONTAINERS
        //DENTRO DE ITEMS CONTAINERS, PODEMOS RECUPERAR EL CONTAINER
        //PARA ACCEDER SE UTILIZA UNA CLASE LLAMADA CosmosClient
        //DENTRO DE ESTE CLIENTE, SE ACCEDE A LOS CONTAINERS
        private Container containerCosmos;
        private CosmosClient client;
        public ServiceCochesCosmos(Container container, CosmosClient client)
        {
            this.containerCosmos = container;
            this.client = client;
        }

        //PARA TRABAJAR CON COTNAINER (CREACION DE CONTAINERS) SE UTILIZA EL CLIENT

        //METODO PARA CREAR LA BBDD Y EL CONTAINER
        public async Task CreateDataBaseAsync()
        {
            //AUNQUE TRABAJEMOS CON EL ID COMO PRIMARY KEY, ESTO ES OPCIONAL,
            //PODRIAMOS INDICAR QUE LA PRIMARY KEY ES OTRO CAMPO DEL OBJETO JSON
            ContainerProperties properties = new ContainerProperties("containercoches", "/id");
            //VAMOS A CREAR LA BASE DE DATOS Y UN CONTENEDOR EN SU INTERIOR
            await this.client.CreateDatabaseAsync("vehiculoscosmos");
            //DENTRO DE LA BASE DE DATOS, CREAMOS EL CONTAINER PARA LOS ITEMS
            await this.client.GetDatabase("vehiculoscosmos").CreateContainerAsync(properties);

        }

        //PARA TRABAJAR CON LOS ITEMS (Vehiculos) SE UTILIZA EL CONTAINER
        
        //METODO PARA INSERTAR VEHICULOS
        public async Task InsertVehiculoAsync(Vehiculo car)
        {
            //EN EL MOMENTO DE CREAR UN ITEM DEBEMOS
            //INDICAR LA CLASE DEL OBJETO A INSERTAR
            //Y EL PARTITION KEY QUE ES LA PRIMARY KEY (id)
            await this.containerCosmos.CreateItemAsync<Vehiculo>(car, new PartitionKey(car.Id));
        }

        //METODO PARA RECUPERAR TODOS LOS VEHICULOS
        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            //LOS ELEMENTOS SE RECORREN CON UN Iterator
            //QUE ES UN ELEMENTO QUE NO SABE CUANTOS REGISTROS EXISTEN SIMPLEMENTE VA HACIA ADELANTE
            var query = this.containerCosmos.GetItemQueryIterator<Vehiculo>();
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                //EXTRAEMOS CADA COCHE LEYENDO DE UNO EN UNO
                var results = await query.ReadNextAsync();
                coches.AddRange(results);
            }
            return coches;
        }

        public async Task UpdateVehiculoAsync (Vehiculo car)
        {
            //TENEMOS UN METODO LLAMADO Upsert, QUE ES UNA MEZCLA ENTRE UPDATE E INSERT
            //SI LO ENCUENTRA LO MODIFICA Y SI NO LO INSERTA
            await this.containerCosmos.UpsertItemAsync<Vehiculo>
                (car, new PartitionKey(car.Id));
        }

        //METODO PARA ELIMINAR
        public async Task DeleteVehiculoAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Vehiculo>(id, new PartitionKey(id));
        }

        //METODO PARA BUSCAR UN COCHE
        public async Task<Vehiculo> FindVehiculoAsync(string id)
        {
            ItemResponse<Vehiculo> response =
                await this.containerCosmos.ReadItemAsync<Vehiculo>(id, new PartitionKey(id));
            return response.Resource;
        }
        //BUSCAR MEDIANTE UNA CONSULTA SQL EN JSON COSMOS DB
        public async Task<List<Vehiculo>> FindVehiculoMarcaAsync(string marca)
        {
            //LAS CONSULTAS A JSON NO TIENEN PARAMETROS DEBEN REALIZARSE CONCATENANDO
            string sql = "select * from c where c.Marca = '" + marca + "'";
            //PARA FILTRAR SE UTILIZA QUERY DEFINITIONS
            QueryDefinition definition = new QueryDefinition(sql);
            //A PARTIR DE LA DEFINICION SE RECUPERAN LOS ELEMENTOS CON UN Iterator
            //PERO ENVIANDO DEFINITION
            var query = this.containerCosmos.GetItemQueryIterator<Vehiculo>(definition);
            List<Vehiculo> coches = new List<Vehiculo>();
            while (query.HasMoreResults)
            {
                var results = await query.ReadNextAsync();
                coches.AddRange(results);
            }
            return coches;

        }
    }
}
