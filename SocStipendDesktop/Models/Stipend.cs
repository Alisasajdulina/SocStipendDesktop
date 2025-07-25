﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocStipendDesktop.Models
{
    public class Stipend
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
        public DateTime? DtRef { get; set; }
        public DateTime? DtAssign { get; set; }
        public DateTime? DtEnd { get; set; }
        public DateTime? DtStop { get; set; }
        public bool? HasTravelCard { get; set; }
        [NotMapped]
        public int OrderNum { get; set; }
        [NotMapped]
        public string? StudentName { get; set; }
        [NotMapped]
        public string? StudentGroup { get; set; }
        [NotMapped]
        public string? Status { get; set; }
    }
}
