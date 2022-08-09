using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Cuenta
    {
        [PrimaryKey]
        public string CodigoCuenta { get; set; }
        public int CodigoUsuario { get; set; }
        public string Moneda { get; set; }
        public double Saldo { get; set; }
        public string Tipo { get; set; }
    }
}
