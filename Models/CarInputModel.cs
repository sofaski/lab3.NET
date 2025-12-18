using System.ComponentModel.DataAnnotations;

namespace CarValueML.Models;

public class CarInputModel
{
    [Required] public string Buying { get; set; } = "";
    [Required] public string Maint { get; set; } = "";
    [Required] public string Doors { get; set; } = "";
    [Required] public string Persons { get; set; } = "";
    [Required] public string LugBoot { get; set; } = "";
    [Required] public string Safety { get; set; } = "";
    [Required] public string EstimatedPriceCategory { get; set; } = ""; // low/medium/high
}
