﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.Service
{
    public class DeleteServiceViewModel
    {
        public Guid ServiceId { get; set; }
        public Guid OrganisationId { get; set; }
        public string ServiceName { get; set; }
    }
}
