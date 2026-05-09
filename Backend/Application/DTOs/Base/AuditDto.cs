using AutoMapper.Configuration.Annotations;
using System.Text.Json.Serialization;

namespace DBS_Task.Application.DTOs.Base
{
    public class AuditDto : BaseDto
    {
        public DateTime CreatedAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CreatedBy { get; set; }
        
    }
}
