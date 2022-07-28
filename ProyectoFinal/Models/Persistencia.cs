using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoFinal.Models
{
    public class Persistencia
    {
        //ID 1 ultimo usuario que inició sesión

        [PrimaryKey]
        public int Id { get; set; }
        public string Campo { get; set; }
    }
}
