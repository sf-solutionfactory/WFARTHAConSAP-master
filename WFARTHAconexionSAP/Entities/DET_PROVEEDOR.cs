//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFARTHAconexionSAP.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class DET_PROVEEDOR
    {
        public string ID_LIFNR { get; set; }
        public string ID_BUKRS { get; set; }
        public string COND_PAGO { get; set; }
    
        public virtual PROVEEDOR PROVEEDOR { get; set; }
        public virtual SOCIEDAD SOCIEDAD { get; set; }
    }
}
