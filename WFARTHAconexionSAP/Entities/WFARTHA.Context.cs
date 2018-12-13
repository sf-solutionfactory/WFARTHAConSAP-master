﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WFARTHAEntities : DbContext
    {
        public WFARTHAEntities()
            : base("name=WFARTHAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACCION> ACCIONs { get; set; }
        public virtual DbSet<APPSETTING> APPSETTINGs { get; set; }
        public virtual DbSet<BANCO> BANCOS { get; set; }
        public virtual DbSet<CAMPOS> CAMPOS { get; set; }
        public virtual DbSet<CARPETA> CARPETAs { get; set; }
        public virtual DbSet<CARPETAT> CARPETATs { get; set; }
        public virtual DbSet<CECO> CECOes { get; set; }
        public virtual DbSet<CLAVES_CONTA> CLAVES_CONTA { get; set; }
        public virtual DbSet<CONCEPTO> CONCEPTOes { get; set; }
        public virtual DbSet<CONMAIL> CONMAILs { get; set; }
        public virtual DbSet<DELEGAR> DELEGARs { get; set; }
        public virtual DbSet<DET_AGENTEC> DET_AGENTEC { get; set; }
        public virtual DbSet<DET_AGENTECA> DET_AGENTECA { get; set; }
        public virtual DbSet<DET_AGENTECC> DET_AGENTECC { get; set; }
        public virtual DbSet<DET_APROB> DET_APROB { get; set; }
        public virtual DbSet<DET_APROB0> DET_APROB0 { get; set; }
        public virtual DbSet<DET_K> DET_K { get; set; }
        public virtual DbSet<DET_PEP> DET_PEP { get; set; }
        public virtual DbSet<DET_PROVEEDOR> DET_PROVEEDOR { get; set; }
        public virtual DbSet<DET_SOCIEDAD> DET_SOCIEDAD { get; set; }
        public virtual DbSet<DET_TIPODOC> DET_TIPODOC { get; set; }
        public virtual DbSet<DET_TIPOPRESUPUESTO> DET_TIPOPRESUPUESTO { get; set; }
        public virtual DbSet<DOCUMENTO> DOCUMENTOes { get; set; }
        public virtual DbSet<DOCUMENTOA> DOCUMENTOAs { get; set; }
        public virtual DbSet<DOCUMENTOA1> DOCUMENTOAS1 { get; set; }
        public virtual DbSet<DOCUMENTOLOG> DOCUMENTOLOGs { get; set; }
        public virtual DbSet<DOCUMENTOP> DOCUMENTOPs { get; set; }
        public virtual DbSet<DOCUMENTOPRE> DOCUMENTOPREs { get; set; }
        public virtual DbSet<DOCUMENTOR> DOCUMENTORs { get; set; }
        public virtual DbSet<DOCUMENTORP> DOCUMENTORPs { get; set; }
        public virtual DbSet<DOCUMENTOSAP> DOCUMENTOSAPs { get; set; }
        public virtual DbSet<DOCUMENTOUUID> DOCUMENTOUUIDs { get; set; }
        public virtual DbSet<EKBE> EKBEs { get; set; }
        public virtual DbSet<EKBE_DUMMY> EKBE_DUMMY { get; set; }
        public virtual DbSet<EKKO> EKKOes { get; set; }
        public virtual DbSet<EKKO_DUMMY> EKKO_DUMMY { get; set; }
        public virtual DbSet<EKPO> EKPOes { get; set; }
        public virtual DbSet<EKPO_DUMMY> EKPO_DUMMY { get; set; }
        public virtual DbSet<ESTATUS_PAGO> ESTATUS_PAGO { get; set; }
        public virtual DbSet<FLUJO> FLUJOes { get; set; }
        public virtual DbSet<FORMATO> FORMATOes { get; set; }
        public virtual DbSet<FORO> FOROes { get; set; }
        public virtual DbSet<FOROD> FORODs { get; set; }
        public virtual DbSet<IMPUESTO> IMPUESTOes { get; set; }
        public virtual DbSet<IMPUESTOT> IMPUESTOTs { get; set; }
        public virtual DbSet<MIEMBRO> MIEMBROS { get; set; }
        public virtual DbSet<MONEDA> MONEDAs { get; set; }
        public virtual DbSet<PAGINA> PAGINAs { get; set; }
        public virtual DbSet<PAGINAT> PAGINATs { get; set; }
        public virtual DbSet<PERMISO_PAGINA> PERMISO_PAGINA { get; set; }
        public virtual DbSet<POSICION> POSICIONs { get; set; }
        public virtual DbSet<PROVEEDOR> PROVEEDORs { get; set; }
        public virtual DbSet<PROVEEDOR_BANCO> PROVEEDOR_BANCO { get; set; }
        public virtual DbSet<PROYECTO> PROYECTOes { get; set; }
        public virtual DbSet<PUESTO> PUESTOes { get; set; }
        public virtual DbSet<PUESTOT> PUESTOTs { get; set; }
        public virtual DbSet<RAMA_PEP> RAMA_PEP { get; set; }
        public virtual DbSet<RANGO> RANGOes { get; set; }
        public virtual DbSet<RETENCION> RETENCIONs { get; set; }
        public virtual DbSet<RETENCION_PROV> RETENCION_PROV { get; set; }
        public virtual DbSet<RETENCIONT> RETENCIONTs { get; set; }
        public virtual DbSet<ROL> ROLs { get; set; }
        public virtual DbSet<ROLT> ROLTs { get; set; }
        public virtual DbSet<SOCIEDAD> SOCIEDADs { get; set; }
        public virtual DbSet<SPRA> SPRAS { get; set; }
        public virtual DbSet<TCAMBIO> TCAMBIOs { get; set; }
        public virtual DbSet<TEXTO> TEXTOes { get; set; }
        public virtual DbSet<TIPOPRESUPUESTO> TIPOPRESUPUESTOes { get; set; }
        public virtual DbSet<TIPOPRESUPUESTOT> TIPOPRESUPUESTOTs { get; set; }
        public virtual DbSet<TSOL> TSOLs { get; set; }
        public virtual DbSet<TSOLT> TSOLTs { get; set; }
        public virtual DbSet<TSOPORTE> TSOPORTEs { get; set; }
        public virtual DbSet<TSOPORTET> TSOPORTETs { get; set; }
        public virtual DbSet<USUARIO> USUARIOs { get; set; }
        public virtual DbSet<WARNING> WARNINGs { get; set; }
        public virtual DbSet<WORKFH> WORKFHs { get; set; }
        public virtual DbSet<WORKFP> WORKFPs { get; set; }
        public virtual DbSet<WORKFT> WORKFTs { get; set; }
        public virtual DbSet<WORKFV> WORKFVs { get; set; }
        public virtual DbSet<CONDICIONES_PAGO> CONDICIONES_PAGO { get; set; }
        public virtual DbSet<IIMPUESTO> IIMPUESTOes { get; set; }
        public virtual DbSet<ASIGN_PROY_SOC> ASIGN_PROY_SOC { get; set; }
        public virtual DbSet<CARPETAV> CARPETAVs { get; set; }
        public virtual DbSet<DET_APROBV> DET_APROBV { get; set; }
        public virtual DbSet<DET_PROVEEDORV> DET_PROVEEDORV { get; set; }
        public virtual DbSet<DET_PROYECTO_DEC_V> DET_PROYECTO_DEC_V { get; set; }
        public virtual DbSet<DET_PROYECTOV> DET_PROYECTOV { get; set; }
        public virtual DbSet<DET_TIPOPRESUPUESTOV> DET_TIPOPRESUPUESTOV { get; set; }
        public virtual DbSet<DOCUMENTOV> DOCUMENTOVs { get; set; }
        public virtual DbSet<PAGINAV> PAGINAVs { get; set; }
        public virtual DbSet<WARNINGV> WARNINGVs { get; set; }
    }
}
