using System;
using System.Collections.Generic;

#nullable disable

namespace DevExtremeAspNetCoreApp3.Models.EF
{
    public partial class Student
    {
        public int StudentId { get; set; }
        public string Surename { get; set; }
        public string Name { get; set; }
        public int Course { get; set; }
    }
}
