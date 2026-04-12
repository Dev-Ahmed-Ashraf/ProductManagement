using System.ComponentModel.DataAnnotations;

namespace DBS_Task.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; private set; }

    }
}
