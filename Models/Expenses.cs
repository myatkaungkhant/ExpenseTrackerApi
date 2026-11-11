using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApi.Models;

public class Expenses
{
    public int Id { get; set; } 

    [Required(AllowEmptyStrings = false)]
    public string Title {get; set;} = string.Empty;

    [Range(0.01,double.MaxValue)]
    public decimal? Amount {get; set;}

    [Required(AllowEmptyStrings = false)]
    public string Category {get;set;} = string.Empty;

}
