using System;
using System.Collections.Generic;
using System.Text;

namespace TareaAplicada2
{
    // Modelo para guardar una consulta de clima como favorita
    // Los campos van de acuerdo a lo que trae la API de weatherapi.com
    public class ConsultaFavorita
    {
        public string Ciudad { get; set; }
        public double TemperaturaC { get; set; }
        public string Condicion { get; set; }
        public DateTime FechaConsulta { get; set; }

        // esto es solo para que se vea bonito si lo imprimo o lo pongo en un listbox
        public override string ToString()
        {
            return $"{Ciudad} - {TemperaturaC}°C - {Condicion} ({FechaConsulta:g})";
        }
    }
}
