using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.src.Domain.Entities
{
    [Table("user_sites")]
    public class UserSite
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("site_id")]
        public Guid? SiteId { get; set; }
        public virtual User User { get; set; }
        public virtual Site SiteNavigate { get; set; }
    }
}
