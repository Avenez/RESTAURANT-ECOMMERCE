namespace GestionePizzeria.Models
{
    using GestionePizzeria.Validations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static GestionePizzeria.Validations.UsernameDuplicity;

    [Table("Utente")]
    public partial class Utente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utente()
        {
            Ordine = new HashSet<Ordine>();
        }

        [Key]
        [ScaffoldColumn(false)]
        public int idUtente { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3 , ErrorMessage = "Lo username deve avere almeno tre caratteri")]
        [Display(Name ="Username")]
        [UsernameDuplicity]
        public string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "La password deve avere almeno tre caratteri")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public string Ruolo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordine> Ordine { get; set; }

        
    }
}
