using GestionePizzeria.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestionePizzeria.Validations
{
    public class UsernameDuplicity : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object Username, ValidationContext validationContext) 
        {
            System.Diagnostics.Debug.WriteLine("Username: " + Username.ToString());
            string usernameToCheck = Username.ToString();
            ModelDBContext db = new ModelDBContext();

            var UsernameCheck = db.Utente.Any(u => u.Username == usernameToCheck);
            

            if (UsernameCheck)
            {
                return new ValidationResult("Username già in uso. Selezionarne un altro.");
            }
            else 
            {
                return ValidationResult.Success;
                
            }
        }

        
    }
}