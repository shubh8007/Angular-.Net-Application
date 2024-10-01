using System.ComponentModel.DataAnnotations;

namespace StudentPortal1
{
    public class Student
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Course { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
       
        

       
       /*public ICollection<Payment> Payments { get; set; }    */
    }
}
