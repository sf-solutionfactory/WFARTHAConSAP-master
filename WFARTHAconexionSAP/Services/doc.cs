using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFARTHAconexionSAP.Services
{
    public class doc
    {
        public string numero_TAT { get; set; }
        public string Mensaje { get; set; }
        public decimal Num_doc_SAP { get; set; }
        public string Sociedad { get; set; }
        public int Año { get; set; }
        public long Cuenta_cargo { get; set; }
        public long Cuenta_abono { get; set; }
        public string blart { get; set; }
        public string kunnr { get; set; }
        public string desc { get; set; }
        public decimal importe { get; set; }
    }
}
