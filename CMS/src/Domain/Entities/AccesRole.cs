namespace CMS.src.Domain.Entities
{
    public class AccessRole
    {
        public int Id { get; set; }
        public string NameRol { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

        //Collection
        // Propiedad de navegación (opcional, para ver qué usuarios tienen este rol)
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
