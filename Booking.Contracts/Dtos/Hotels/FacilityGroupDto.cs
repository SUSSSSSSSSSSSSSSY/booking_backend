using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Contracts.Dtos.Hotels;

public class FacilityGroupDto
{
    public string Title { get; set; } = default!;
    public string Icon { get; set; } = default!;
    public List<string> Items { get; set; } = [];
}