using System;
using System.ComponentModel.DataAnnotations;

namespace AthGCalendar.Models
{
    public class GoogleEventInfo
    {
        [Display(Name = "Event title")]
        public string Title { get; set; }

        [Display(Name = "Event description")]
        public string Description { get; set; }

        [Display(Name = "Date from")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-mm-yyyy}")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Date to")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-mm-yyyy}")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Time from")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Time to")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}