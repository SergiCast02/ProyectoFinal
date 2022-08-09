using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Transferencia
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Accion { get; set; } // La accion es relativa al usuario (en si es la cuenta) que envia si debito o credito
        public string Moneda { get; set; }
        public double Valor { get; set; }
        public string Envia { get; set; }
        public string Recibe { get; set; }
        public DateTime FechaHora { get; set; }
        public string Comentario { get; set; }
    }
}
