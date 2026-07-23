using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateRecipeStepRequest
    {
        public string? Instruction {  get; set; }
        public int? StepNumber { get; set; }
        public bool? HasTimer { get; set; }
        public int? TimerDuration { get; set; }
        public bool? IsAsync { get; set; }
        public Guid? AsyncGroupId { get; set; }
    }
}
