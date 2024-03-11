namespace GestionePizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Prodotto")]
    public partial class Prodotto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prodotto()
        {
            DettaglioOrdine = new HashSet<DettaglioOrdine>();
        }

        [Key]
        public int idProdotto { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nome del Prodotto")]
        public string Nome { get; set; }

        [Display(Name = "Foto del Prodotto")]
        public string Foto { get; set; }


        [Column(TypeName = "money")]
        [Display(Name = "Prezzo del Prodotto")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Possono essere inseriti solo numeri")]
        public decimal Prezzo { get; set; }

        [Required]
        [Display(Name = "Tempo di consegna in minuti")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid Number")]
        public int TempoConsegna { get; set; }

        [Required]
        [Display(Name = "Ingredienti")]
        public string Ingredienti { get; set; }

        [Display(Name = "Spuntare se disponibile a Pranzo")]
        public bool Pranzo { get; set; }

        [Display(Name = "Spuntare se disponibile a Cena")]
        public bool Cena { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettaglioOrdine> DettaglioOrdine { get; set; }

       


    }
}
