using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AddRecipeStepRequest
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public bool HasTimer { get; set; } = false;
        public int? TimerDuration { get; set; }
        public bool IsAsync { get; set; } = false;
        public Guid? AsyncGroupId { get; set; }
    }
}
