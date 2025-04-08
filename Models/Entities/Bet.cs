using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RouletteTechTest.API.Models.Enums;

namespace RouletteTechTest.API.Models.Entities
{
    public class Bet
    {


        /*
        Estoy realizando una prueba técnica usando .Net 8 con sqllite, ya acabo de finalizar mis endpoints, los cuales son User (Tipo CRUD, encargado de la parte de usuarios, recibe un nombre y un balance,
         internamente le genera un Id) User tiene una relacion con Session muchos a muchos, debido a que una session puede tener muchos usuarios y un usuario puede tener muchas sesiones (claro una vez haya
          finalizado la anterior, no debe poder crear una session si ya tiene una abierta) Tambien tenemos session la logica de esta es almacenar una serie de rondas, implementacion similar no podemos crear
           una nueva ronda dentro de la session sino se ha termiando la anterior, debo mencionar que session lleva dentro las siguientes propiedades (su Id autogenerado, una fecha de inicio y de fin, una
            lista de rondas y una lista de usuarios)  acerca de las rondas tiene su Id, el numero de la rondaactual, una fecha de inicio y una de fin, una lista de Bets, y un resultado y finalizamos con Bet,
             dentro de una ronda pueden haber muchas apuestas pero antes se debe finalizar la anterior, dentro de la apuesta tenemos (Amount, UserName, un tipo de apuesta, un TimeStamp y un OutCome)  
        
        */


        [Key]
        public Guid Id { get; set; }

        // Monto apostado por el usuario
        [Required]
        [Range(1, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public BetType Type { get; set; } //1 Color   2par 3numero

        [Required]
        public string BetValue { get; set; } = null!;

        // Momento en el que se realizó la apuesta
        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [Required]
        public BetOutcome Outcome { get; set; } = BetOutcome.Pending;


        [Column(TypeName = "decimal(18,2)")]
        public decimal Prize { get; set; }


        [Required]
        public Guid RoundId { get; set; }

        [ForeignKey("RoundId")]
        public Round Round { get; set; } = null!;

        //FK para con User
        [Required]
        public Guid UserId { get; set; } 

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
