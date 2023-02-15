using Newtonsoft.Json;

namespace MvcCoreCochesCosmosDb.Models
{
    public class Vehiculo
    {
        //AUNQUE SE LLAME ID, DEBEMOS DEBEMOS MAPEARLA CON JSON PARA QUE LO ALMACENE EN MINUSCULA
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Imagen { get; set; }
        public int VelocidadMaxima { get; set; }
        // EL MOTOR SERA OPCIONAL
        public Motor Motor { get; set; }
    }
}
