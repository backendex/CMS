namespace CMS.src.Domain.Entities
{
    //Tabla para dynamic JSONB
    public class BaseDynamic
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SiteId { get; set; }

        public System.Text.Json.JsonDocument DynamicData { get; set; }
    }

}


