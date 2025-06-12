using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EmployeeDataProject.Models
{
   

    [Table("Company")]
    public class Company
    {
        [Key]
        public string CompanyCode { get; set; }

        public string CompanyDesc { get; set; }
    }

}
