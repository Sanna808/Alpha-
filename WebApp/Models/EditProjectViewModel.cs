using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class EditProjectViewModel
{
    public string Id { get; set; } = null!;

    public string? Image { get; set; }

    [Required(ErrorMessage = "Required")]
    [DataType(DataType.Text)]
    [Display(Name = "Project Name", Prompt = "Enter project name")]

    public string ProjectName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Description")]

    public string? Description { get; set; }


    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

    public DateTime StartDate { get; set; }


    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

    public DateTime? EndDate { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Budget", Prompt = "0")]
    public decimal? Budget { get; set; }

    [Required(ErrorMessage = "Choose a client")]
    [Display(Name = "Client")]
    public string ClientId { get; set; } = null!;

    [Required(ErrorMessage = "Choose a member")]
    [Display(Name = "Member")]
    public string MemberId { get; set; } = null!;

    [Required(ErrorMessage = "Choose a status")]
    [Display(Name = "Status")]
    public int StatusId { get; set; }


    public IEnumerable<SelectListItem> Clients { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Members { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Statuses { get; set; } = Enumerable.Empty<SelectListItem>();
}
