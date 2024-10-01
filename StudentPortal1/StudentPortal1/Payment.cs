using System.ComponentModel.DataAnnotations;

namespace StudentPortal1
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; } 

        public int Amount { get; set; }

        public DateTime Date { get; set; }

       
        public Student Student { get; set; }
    }
}
