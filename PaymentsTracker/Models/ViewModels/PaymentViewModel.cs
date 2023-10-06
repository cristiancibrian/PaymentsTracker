using System.ComponentModel.DataAnnotations;

namespace PaymentsTracker.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="La descripción es obligatoria.")]
        [Display(Name="Descripción")]
        public string Description { get; set; }

    }
}
