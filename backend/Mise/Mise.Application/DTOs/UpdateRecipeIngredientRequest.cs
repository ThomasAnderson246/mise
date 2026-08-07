using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateRecipeIngredientRequest
    {
        public decimal? Quantity {  get; set; }
        public Guid? UnitTypeId { get; set; }
        public int? DisplayOrder {  get; set; }
        public Guid? GroupId { get; set; }
        public bool? IsRatioAnchor { get; set; }
    }
}
