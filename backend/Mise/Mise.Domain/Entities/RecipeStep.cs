using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class RecipeStep
    {
        public Guid StepId { get; set; }
        public Guid VersionId { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public bool IsAsync { get; set; } = false;
        public Guid? AsyncGroupId { get; set; }
        public bool HasTimer { get; set; } = false;
        public int? TimerDuration {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
