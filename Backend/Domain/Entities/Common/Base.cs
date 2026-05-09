using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Domain.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; private set; }

    }
}
