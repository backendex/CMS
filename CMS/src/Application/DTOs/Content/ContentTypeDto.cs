using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CMS.src.Application.DTOs.Content
{
    public class ContentTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}