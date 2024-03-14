namespace GestionePizzeria.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordine")]
    public partial class Ordine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordine()
        {
            DettaglioOrdine = new HashSet<DettaglioOrdine>();
        }

        [Key]
        public int idOrdine { get; set; }

        public int idUtente { get; set; }

        [Column(TypeName = "date")]

        public DateTime DataOridine { get; set; }

        [Column(TypeName = "money")]
        public decimal Importo { get; set; }

        [Required(ErrorMessage = "L'indirizzo di consegna è obbligatorio")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "L'idirizzo deve essere di almeno 5 caratteri")]
        [Display(Name ="Indirizzo di consegna")]
        public string IndirizzoConsegna { get; set; }

        public string Note { get; set; } //può essere null sul db

        public bool Evaso { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DettaglioOrdine> DettaglioOrdine { get; set; }

        public virtual Utente Utente { get; set; }
    }
}
