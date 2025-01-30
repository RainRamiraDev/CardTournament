using CTDto.Tournaments;
using CTDto.Users.Organizer;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDto.Validations.Users.Users.Organizer
{
    public class OrganizerDtoValidation : AbstractValidator<OrganizerDto>
    {
        public OrganizerDtoValidation()
        {
            
        }

    }
}
