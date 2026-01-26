namespace CMS.src.Domain.Entities
{
    public class Site
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Domain { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string Country { get; private set; } = null!;
        public string DefaultLanguage { get; private set; } = "es";
        public bool IsActive { get; private set; }


        protected Site() { }


        public Site(string name, string domain, string city, string country)
        {
            Id = Guid.NewGuid();
            Name = name;
            Domain = domain;
            City = city;
            Country = country;
            IsActive = true;
        }


        public void Disable() => IsActive = false;
        public void Enable() => IsActive = true;
    }
}
