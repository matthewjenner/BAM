using System.ComponentModel.DataAnnotations;

namespace StargateAPI.Business.Data
{
    public class LogEntry
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Message { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Exception { get; set; }
        
        [MaxLength(100)]
        public string? Source { get; set; }
        
        [MaxLength(50)]
        public string? UserId { get; set; }
        
        [MaxLength(100)]
        public string? RequestId { get; set; }
    }
}
