//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using TATconexionSAP;

//namespace TAT001.Services
//{
//    public class ProcesaFlujo2
//    {
//        public string procesa(FLUJO f, string recurrente)
//        {
//            string correcto = String.Empty;
//            TAT001Entities db = new TAT001Entities();
//            FLUJO actual = new FLUJO();
//            if (f.ESTATUS.Equals("I"))//---------------------------NUEVO REGISTRO
//            {
//                actual.NUM_DOC = f.NUM_DOC;
//                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                actual.COMENTARIO = f.COMENTARIO;
//                actual.ESTATUS = f.ESTATUS;
//                actual.FECHAC = f.FECHAC;
//                actual.FECHAM = f.FECHAM;
//                actual.LOOP = f.LOOP;
//                actual.NUM_DOC = f.NUM_DOC;
//                actual.POS = f.POS;
//                DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
//                                    a.USUARIOC_ID.Equals(d.USUARIOC_ID)).OrderByDescending(a => a.VERSION).FirstOrDefault();
//                //DET_AGENTEP dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID.Equals(dah.PUESTOC_ID) &
//                //                    a.VERSION.Equals(dah.VERSION) & a.AGROUP_ID.Equals(dah.AGROUP_ID) & a.POS == 1).FirstOrDefault();
//                actual.DETPOS = 1;
//                actual.DETVER = dah.VERSION;
//                actual.USUARIOA_ID = f.USUARIOA_ID;
//                actual.USUARIOD_ID = f.USUARIOD_ID;
//                actual.WF_POS = f.WF_POS;
//                actual.WF_VERSION = f.WF_VERSION;
//                actual.WORKF_ID = f.WORKF_ID;
//                f.ESTATUS = "A";
//                actual.ESTATUS = f.ESTATUS;
//                db.FLUJOes.Add(actual);

//                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
//                int next_step_a = 0;
//                if (paso_a.NEXT_STEP != null)
//                    next_step_a = (int)paso_a.NEXT_STEP;

//                WORKFP next = new WORKFP();
//                if (recurrente != "X")
//                {
//                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                }
//                else
//                {
//                    WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.ACCION_ID == 5).FirstOrDefault();
//                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == autoriza.NS_ACCEPT).FirstOrDefault();
//                }
//                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
//                {
//                    d.ESTATUS_WF = "A";
//                    if (paso_a.EMAIL.Equals("X"))
//                        correcto = "2";
//                }
//                else
//                {
//                    //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                    FLUJO nuevo = new FLUJO();
//                    nuevo.WORKF_ID = next.ID;
//                    nuevo.WF_VERSION = next.VERSION;
//                    nuevo.WF_POS = next.POS;
//                    nuevo.NUM_DOC = actual.NUM_DOC;
//                    nuevo.POS = actual.POS + 1;
//                    nuevo.LOOP = 1;

//                    if (next.ACCION.TIPO == "E")
//                    {
//                        nuevo.USUARIOA_ID = null;
//                        nuevo.DETPOS = 0;
//                        nuevo.DETVER = 0;
//                    }
//                    else
//                    {
//                        if (recurrente != "X")
//                        {
//                            FLUJO detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah);
//                            nuevo.USUARIOA_ID = detA.USUARIOA_ID;
//                            nuevo.DETPOS = detA.DETPOS;
//                            nuevo.DETVER = dah.VERSION;
//                        }
//                        else
//                        {
//                            nuevo.USUARIOA_ID = null;
//                            nuevo.DETPOS = 0;
//                            nuevo.DETVER = 0;
//                        }
//                    }
//                    nuevo.ESTATUS = "P";
//                    nuevo.FECHAC = DateTime.Now;
//                    nuevo.FECHAM = DateTime.Now;

//                    db.FLUJOes.Add(nuevo);
//                    if (paso_a.EMAIL.Equals("X"))
//                        correcto = "1";
//                    d.ESTATUS_WF = "P";
//                    db.Entry(d).State = EntityState.Modified;

//                    db.SaveChanges();
//                }
//            }
//            else if (f.ESTATUS.Equals("A"))   //---------------------EN PROCESO DE APROBACIÓN
//            {
//                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

//                if (actual.ESTATUS.Equals("A"))
//                    return "1";//-----------------YA FUE PROCESADA
//                else
//                {
//                    var wf = actual.WORKFP;
//                    actual.ESTATUS = f.ESTATUS;
//                    actual.FECHAM = f.FECHAM;
//                    actual.COMENTARIO = f.COMENTARIO;
//                    actual.USUARIOA_ID = f.USUARIOA_ID;
//                    db.Entry(actual).State = EntityState.Modified;

//                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
//                    bool ban = true;
//                    while (ban)         //--------------PARA LOOP
//                    {
//                        int next_step_a = 0;
//                        int next_step_r = 0;
//                        if (paso_a.NEXT_STEP != null)
//                            next_step_a = (int)paso_a.NEXT_STEP;
//                        if (paso_a.NS_ACCEPT != null)
//                            next_step_a = (int)paso_a.NS_ACCEPT;
//                        if (paso_a.NS_REJECT != null)
//                            next_step_r = (int)paso_a.NS_REJECT;

//                        WORKFP next = new WORKFP();
//                        if (paso_a.ACCION.TIPO == "A" | paso_a.ACCION.TIPO == "N" | paso_a.ACCION.TIPO == "R" | paso_a.ACCION.TIPO == "T" | paso_a.ACCION.TIPO == "E")//Si está en proceso de aprobación
//                        {
//                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
//                            {
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

//                                FLUJO nuevo = new FLUJO();
//                                nuevo.WORKF_ID = paso_a.ID;
//                                nuevo.WF_VERSION = paso_a.VERSION;
//                                nuevo.WF_POS = next.POS;
//                                nuevo.NUM_DOC = actual.NUM_DOC;
//                                nuevo.POS = actual.POS + 1;
//                                nuevo.LOOP = 1;//-----------------------------------cc
//                                               //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                               //if (loop >= next.LOOPS)
//                                               //{
//                                               //    paso_a = next;
//                                               //    continue;
//                                               //}
//                                               //if (loop != 0)
//                                               //    nuevo.LOOP = loop + 1;
//                                               //else
//                                               //    nuevo.LOOP = 1;
//                                FLUJO detA = new FLUJO();
//                                if (paso_a.ACCION.TIPO == "N")
//                                    actual.DETPOS = actual.DETPOS - 1;
//                                int sop = 99;
//                                if (next.ACCION.TIPO == "S")
//                                    sop = 98;
//                                detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop);
//                                nuevo.USUARIOA_ID = detA.USUARIOA_ID;
//                                nuevo.DETPOS = detA.DETPOS;
//                                nuevo.DETVER = actual.DETVER;
//                                if (paso_a.ACCION.TIPO == "N")
//                                {
//                                    nuevo.DETPOS = nuevo.DETPOS + 1;
//                                    actual.DETPOS = actual.DETPOS + 1;
//                                }


//                                if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
//                                {
//                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        d.ESTATUS_WF = "A";
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "2";
//                                    }
//                                    else
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        //FLUJO nuevo = new FLUJO();
//                                        nuevo.WORKF_ID = next.ID;
//                                        nuevo.WF_VERSION = next.VERSION;
//                                        nuevo.WF_POS = next.POS + detA.POS;
//                                        nuevo.NUM_DOC = actual.NUM_DOC;
//                                        nuevo.POS = actual.POS + 1;
//                                        nuevo.LOOP = 1;//-----------------------------------

//                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
//                                        //nuevo.USUARIOA_ID = "admin";
//                                        //nuevo.DETPOS = 1;
//                                        d.ESTATUS_WF = "P";
//                                        if (next.ACCION.TIPO.Equals("T"))
//                                        {
//                                            TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault();
//                                            if (tl != null)
//                                            {
//                                                //nuevo.USUARIOA_ID = db.DET_TAX.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
//                                                nuevo.USUARIOA_ID = db.DET_TAXEO.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
//                                                d.ESTATUS_WF = "T";
//                                            }
//                                            else
//                                            {
//                                                nuevo.WF_POS = nuevo.WF_POS + 1;
//                                                nuevo.USUARIOA_ID = null;
//                                                d.ESTATUS_WF = "A";
//                                                d.ESTATUS_SAP = "P";
//                                            }
//                                        }
//                                        else if(paso_a.ACCION.TIPO == "E")
//                                        {
//                                            nuevo.USUARIOA_ID = null;
//                                        }
//                                        else
//                                        {
//                                            if (nuevo.DETPOS == 0)
//                                            {
//                                                nuevo.USUARIOA_ID = null;
//                                                d.ESTATUS_WF = "A";
//                                                d.ESTATUS_SAP = "P";
//                                            }
//                                        }
//                                        nuevo.ESTATUS = "P";
//                                        nuevo.FECHAC = DateTime.Now;
//                                        nuevo.FECHAM = DateTime.Now;

//                                        db.FLUJOes.Add(nuevo);
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "1";
//                                    }
//                                }
//                                //else if(nuevo.DETPOS == 99)
//                                //{
//                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                //}
//                                else
//                                {
//                                    //nuevo.USUARIOD_ID
//                                    nuevo.ESTATUS = "P";
//                                    nuevo.FECHAC = DateTime.Now;
//                                    nuevo.FECHAM = DateTime.Now;
//                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

//                                    db.FLUJOes.Add(nuevo);
//                                    if (paso_a.EMAIL.Equals("X"))
//                                        correcto = "1";

//                                    d.ESTATUS_WF = "P";
//                                }
//                                if (nuevo.DETPOS.Equals(98))
//                                    d.ESTATUS_WF = "S";
//                                db.Entry(d).State = EntityState.Modified;

//                                db.SaveChanges();
//                                ban = false;
//                            }
//                        }
//                        else if (paso_a.ACCION.TIPO == "P")
//                        {
//                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
//                            {
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);

//                                ArchivoContable sa = new ArchivoContable();
//                                string file = sa.generarArchivo(d.NUM_DOC, 0);

//                                if (file == "")
//                                {
//                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

//                                    FLUJO nuevo = new FLUJO();
//                                    nuevo.WORKF_ID = paso_a.ID;
//                                    nuevo.WF_VERSION = paso_a.VERSION;
//                                    nuevo.WF_POS = next.POS;
//                                    nuevo.NUM_DOC = actual.NUM_DOC;
//                                    nuevo.POS = actual.POS + 1;
//                                    nuevo.ESTATUS = "A";
//                                    nuevo.FECHAC = DateTime.Now;
//                                    nuevo.FECHAM = DateTime.Now;

//                                    d.ESTATUS = "A";
//                                    correcto = file;

//                                    db.FLUJOes.Add(nuevo);
//                                    db.SaveChanges();
//                                    ban = false;
//                                }
//                                else
//                                {
//                                    ban = false;
//                                    correcto = file;
//                                }
//                            }
//                        }
//                        else if (paso_a.ACCION.TIPO == "S")
//                        {
//                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
//                            {
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

//                                FLUJO nuevo = new FLUJO();
//                                nuevo.WORKF_ID = paso_a.ID;
//                                nuevo.WF_VERSION = paso_a.VERSION;
//                                nuevo.WF_POS = next.POS;
//                                nuevo.NUM_DOC = actual.NUM_DOC;
//                                nuevo.POS = actual.POS + 1;
//                                nuevo.LOOP = 1;//-----------------------------------cc
//                                //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                //if (loop >= next.LOOPS)
//                                //{
//                                //    paso_a = next;
//                                //    continue;
//                                //}
//                                //if (loop != 0)
//                                //    nuevo.LOOP = loop + 1;
//                                //else
//                                //    nuevo.LOOP = 1;
//                                if (paso_a.ACCION.TIPO == "N")
//                                    actual.DETPOS = actual.DETPOS - 1;
//                                FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 98, next.LOOPS, 98);
//                                nuevo.USUARIOA_ID = detA.USUARIOA_ID;
//                                nuevo.DETPOS = detA.DETPOS;
//                                nuevo.DETVER = actual.DETVER;
//                                if (paso_a.ACCION.TIPO == "N")
//                                {
//                                    nuevo.DETPOS = nuevo.DETPOS + 1;
//                                    actual.DETPOS = actual.DETPOS + 1;
//                                }


//                                if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
//                                {
//                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        d.ESTATUS_WF = "A";
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "2";
//                                    }
//                                    else
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        //FLUJO nuevo = new FLUJO();
//                                        nuevo.WORKF_ID = next.ID;
//                                        nuevo.WF_VERSION = next.VERSION;
//                                        nuevo.WF_POS = next.POS + detA.POS;
//                                        nuevo.NUM_DOC = actual.NUM_DOC;
//                                        nuevo.POS = actual.POS + 1;
//                                        nuevo.LOOP = 1;//-----------------------------------
//                                                       //int loop1 = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                                       //if (loop1 >= next.LOOPS)
//                                                       //{
//                                                       //    paso_a = next;
//                                                       //    continue;
//                                                       //}
//                                                       //if (loop1 != 0)
//                                                       //    nuevo.LOOP = loop1 + 1;
//                                                       //else
//                                                       //    nuevo.LOOP = 1;

//                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
//                                        //nuevo.USUARIOA_ID = "admin";
//                                        //nuevo.DETPOS = 1;
//                                        d.ESTATUS_WF = "P";
//                                        if (nuevo.DETPOS == 0)
//                                        {
//                                            nuevo.USUARIOA_ID = null;
//                                            d.ESTATUS_WF = "A";
//                                            d.ESTATUS_SAP = "P";
//                                        }
//                                        nuevo.ESTATUS = "P";
//                                        nuevo.FECHAC = DateTime.Now;
//                                        nuevo.FECHAM = DateTime.Now;

//                                        db.FLUJOes.Add(nuevo);
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "1";
//                                    }
//                                }
//                                //else if(nuevo.DETPOS == 99)
//                                //{
//                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                //}
//                                else
//                                {
//                                    //nuevo.USUARIOD_ID
//                                    nuevo.ESTATUS = "P";
//                                    nuevo.FECHAC = DateTime.Now;
//                                    nuevo.FECHAM = DateTime.Now;
//                                    nuevo.WF_POS = nuevo.WF_POS + detA.POS;

//                                    db.FLUJOes.Add(nuevo);
//                                    if (paso_a.EMAIL.Equals("X"))
//                                        correcto = "1";

//                                    d.ESTATUS_WF = "P";
//                                }
//                                db.Entry(d).State = EntityState.Modified;

//                                db.SaveChanges();
//                                ban = false;
//                            }
//                        }

//                    }
//                }
//            }
//            else if (f.ESTATUS.Equals("R"))//Rechazada
//            {
//                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
//                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();

//                int next_step_a = 0;
//                int next_step_r = 0;
//                if (paso_a.NEXT_STEP != null)
//                    next_step_a = (int)paso_a.NEXT_STEP;
//                if (paso_a.NS_ACCEPT != null)
//                    next_step_a = (int)paso_a.NS_ACCEPT;
//                if (paso_a.NS_REJECT != null)
//                    next_step_r = (int)paso_a.NS_REJECT;

//                WORKFP next = new WORKFP();
//                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

//                correcto = "3";
//                actual.ESTATUS = f.ESTATUS;
//                actual.FECHAM = f.FECHAM;
//                actual.COMENTARIO = f.COMENTARIO;

//                FLUJO nuevo = new FLUJO();
//                nuevo.WORKF_ID = next.ID;
//                nuevo.WF_VERSION = next.VERSION;
//                nuevo.WF_POS = next.POS;
//                nuevo.NUM_DOC = actual.NUM_DOC;
//                nuevo.POS = actual.POS + 1;
//                nuevo.DETPOS = 1;
//                nuevo.DETVER = actual.DETVER;
//                nuevo.LOOP = 1;//-----------------------------------
//                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                nuevo.USUARIOA_ID = d.USUARIOC_ID;
//                //nuevo.USUARIOD_ID
//                nuevo.ESTATUS = "P";
//                nuevo.FECHAC = DateTime.Now;
//                nuevo.FECHAM = DateTime.Now;

//                db.FLUJOes.Add(nuevo);
//                db.Entry(actual).State = EntityState.Modified;
//                d.ESTATUS_WF = "R";

//                db.SaveChanges();

//            }

//            //-------------------------------------------------------------------------------------------------------------------------------//
//            //-------------------------------------------------------------------------------------------------------------------------------//
//            //-------------------------------------------------------------------------------------------------------------------------------//
//            //-------------------------------------------------------------------------------------------------------------------------------//
//            //-------------------------------------------------------------------------------------------------------------------------------//

//            if (false)
//            {
//                if (actual.ESTATUS.Equals("A"))
//                    return "1";//Ya fue procesada
//                else
//                {
//                    if (actual.ESTATUS.Equals("I"))
//                    {
//                        //actual.ESTATUS = f.ESTATUS;
//                        //db.FLUJOes.Add(actual);
//                        //db.SaveChanges();
//                    }
//                    else
//                    {
//                        var wf = actual.WORKFP;
//                        actual.ESTATUS = f.ESTATUS;
//                        actual.FECHAM = f.FECHAM;
//                        actual.COMENTARIO = f.COMENTARIO;

//                        db.Entry(actual).State = EntityState.Modified;
//                    }

//                    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();

//                    bool ban = true;
//                    while (ban)
//                    {
//                        int next_step_a = 0;
//                        int next_step_r = 0;
//                        if (paso_a.NEXT_STEP != null)
//                            next_step_a = (int)paso_a.NEXT_STEP;
//                        if (paso_a.NS_ACCEPT != null)
//                            next_step_a = (int)paso_a.NS_ACCEPT;
//                        if (paso_a.NS_REJECT != null)
//                            next_step_r = (int)paso_a.NS_REJECT;

//                        WORKFP next = new WORKFP();
//                        if (paso_a.ACCION.TIPO == "A")//Si está en proceso de aprobación
//                        {
//                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//Proceso aprobado
//                            {
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

//                                FLUJO nuevo = new FLUJO();
//                                nuevo.WORKF_ID = paso_a.ID;
//                                nuevo.WF_VERSION = paso_a.VERSION;
//                                nuevo.WF_POS = paso_a.POS;
//                                nuevo.NUM_DOC = actual.NUM_DOC;
//                                nuevo.POS = actual.POS + 1;
//                                nuevo.LOOP = 1;//-----------------------------------cc
//                                int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                if (loop >= next.LOOPS)
//                                {
//                                    paso_a = next;
//                                    continue;
//                                }
//                                if (loop != 0)
//                                    nuevo.LOOP = loop + 1;
//                                else
//                                    nuevo.LOOP = 1;

//                                ////FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, 1);
//                                ////nuevo.USUARIOA_ID = detA.USUARIOA_ID;
//                                ////nuevo.DETPOS = detA.DETPOS;

//                                if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
//                                {
//                                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                    if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        d.ESTATUS_WF = "A";
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "2";
//                                    }
//                                    else
//                                    {
//                                        //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                        //FLUJO nuevo = new FLUJO();
//                                        nuevo.WORKF_ID = next.ID;
//                                        nuevo.WF_VERSION = next.VERSION;
//                                        nuevo.WF_POS = next.POS;
//                                        nuevo.NUM_DOC = actual.NUM_DOC;
//                                        nuevo.POS = actual.POS + 1;
//                                        /*nuevo.LOOP = 1;*///-----------------------------------
//                                        int loop1 = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                        if (loop1 >= next.LOOPS)
//                                        {
//                                            paso_a = next;
//                                            continue;
//                                        }
//                                        if (loop != 0)
//                                            nuevo.LOOP = loop1 + 1;
//                                        else
//                                            nuevo.LOOP = 1;

//                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
//                                        //nuevo.USUARIOA_ID = "admin";
//                                        //nuevo.DETPOS = 1;

//                                        //nuevo.USUARIOD_ID
//                                        nuevo.ESTATUS = "P";
//                                        nuevo.FECHAC = DateTime.Now;
//                                        nuevo.FECHAM = DateTime.Now;

//                                        db.FLUJOes.Add(nuevo);
//                                        if (paso_a.EMAIL.Equals("X"))
//                                            correcto = "1";
//                                    }
//                                }
//                                //else if(nuevo.DETPOS == 99)
//                                //{
//                                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                //}
//                                else
//                                {
//                                    //nuevo.USUARIOD_ID
//                                    nuevo.ESTATUS = "P";
//                                    nuevo.FECHAC = DateTime.Now;
//                                    nuevo.FECHAM = DateTime.Now;

//                                    db.FLUJOes.Add(nuevo);
//                                    if (paso_a.EMAIL.Equals("X"))
//                                        correcto = "1";

//                                }
//                                db.SaveChanges();
//                                ban = false;
//                            }
//                            else if (f.ESTATUS.Equals("R"))//Rechazada
//                            {
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

//                                correcto = "3";
//                                actual.ESTATUS = f.ESTATUS;
//                                actual.FECHAM = f.FECHAM;
//                                actual.COMENTARIO = f.COMENTARIO;

//                                FLUJO nuevo = new FLUJO();
//                                nuevo.WORKF_ID = next.ID;
//                                nuevo.WF_VERSION = next.VERSION;
//                                nuevo.WF_POS = next.POS;
//                                nuevo.NUM_DOC = actual.NUM_DOC;
//                                nuevo.POS = actual.POS + 1;
//                                /*nuevo.LOOP = 1;*///-----------------------------------
//                                int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                if (loop != 0)
//                                    nuevo.LOOP = loop + 1;
//                                else
//                                    nuevo.LOOP = 1;
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                nuevo.USUARIOA_ID = d.USUARIOC_ID;
//                                //nuevo.USUARIOD_ID
//                                nuevo.ESTATUS = "P";
//                                nuevo.FECHAC = DateTime.Now;
//                                nuevo.FECHAM = DateTime.Now;

//                                db.FLUJOes.Add(nuevo);

//                                db.Entry(actual).State = EntityState.Modified;

//                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                d.ESTATUS_WF = "R";

//                                db.SaveChanges();
//                                ban = false;

//                            }
//                        }
//                        else
//                        {

//                            if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//Proceso aprobado
//                            {
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
//                                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
//                                {
//                                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                    d.ESTATUS_WF = "A";
//                                    if (paso_a.EMAIL.Equals("X"))
//                                        correcto = "2";
//                                }
//                                else
//                                {
//                                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                    FLUJO nuevo = new FLUJO();
//                                    nuevo.WORKF_ID = next.ID;
//                                    nuevo.WF_VERSION = next.VERSION;
//                                    nuevo.WF_POS = next.POS;
//                                    nuevo.NUM_DOC = actual.NUM_DOC;
//                                    nuevo.POS = actual.POS + 1;
//                                    /*nuevo.LOOP = 1;*///-----------------------------------
//                                    int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                    if (loop >= next.LOOPS)
//                                    {
//                                        paso_a = next;
//                                        continue;
//                                    }
//                                    if (loop != 0)
//                                        nuevo.LOOP = loop + 1;
//                                    else
//                                        nuevo.LOOP = 1;

//                                    if (paso_a.ACCION.TIPO == "N")//Si está en creación
//                                    {
//                                        //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, 1);
//                                        //nuevo.USUARIOA_ID = detA.USUARIOA_ID;
//                                        //nuevo.DETPOS = detA.DETPOS;
//                                    }
//                                    else
//                                    {
//                                        nuevo.USUARIOA_ID = null;
//                                        nuevo.DETPOS = 1;
//                                    }

//                                    //nuevo.USUARIOD_ID
//                                    nuevo.ESTATUS = "P";
//                                    nuevo.FECHAC = DateTime.Now;
//                                    nuevo.FECHAM = DateTime.Now;

//                                    db.FLUJOes.Add(nuevo);
//                                    if (paso_a.EMAIL.Equals("X"))
//                                        correcto = "1";
//                                    d.ESTATUS_WF = "P";
//                                    db.Entry(d).State = EntityState.Modified;
//                                }
//                                db.SaveChanges();
//                                ban = false;
//                            }
//                            else if (f.ESTATUS.Equals("R"))//Rechazada
//                            {
//                                next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

//                                correcto = "3";
//                                actual.ESTATUS = f.ESTATUS;
//                                actual.FECHAM = f.FECHAM;
//                                actual.COMENTARIO = f.COMENTARIO;

//                                FLUJO nuevo = new FLUJO();
//                                nuevo.WORKF_ID = next.ID;
//                                nuevo.WF_VERSION = next.VERSION;
//                                nuevo.WF_POS = next.POS;
//                                nuevo.NUM_DOC = actual.NUM_DOC;
//                                nuevo.POS = actual.POS + 1;
//                                /*nuevo.LOOP = 1;*///-----------------------------------
//                                int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
//                                if (loop != 0)
//                                    nuevo.LOOP = loop + 1;
//                                else
//                                    nuevo.LOOP = 1;
//                                DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                nuevo.USUARIOA_ID = d.USUARIOC_ID;
//                                //nuevo.USUARIOD_ID
//                                nuevo.ESTATUS = "P";
//                                nuevo.FECHAC = DateTime.Now;
//                                nuevo.FECHAM = DateTime.Now;

//                                db.FLUJOes.Add(nuevo);

//                                db.Entry(actual).State = EntityState.Modified;

//                                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
//                                d.ESTATUS_WF = "R";

//                                db.SaveChanges();
//                                ban = false;

//                            }
//                        }
//                    }
//                }
//            }
//            return correcto;
//        }

//        public FLUJO determinaAgente(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop)
//        {
//            if (delega != null)
//                user = delega;
//            bool fin = false;
//            TAT001Entities db = new TAT001Entities();
//            DET_AGENTEP dap = new DET_AGENTEP();
//            FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
//            DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
//                                a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();

//            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
//            //GAUTORIZACION gg = u.GAUTORIZACIONs.Where(a => a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
//            long gaa = db.CREADOR2.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;
//            int ppos = 0;

//            if (pos.Equals(0))
//            {
//                if (loop == null)
//                {
//                    //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == 1).FirstOrDefault();
//                    //da.POS = da.POS - 1;

//                    dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                        a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
//                    dap.POS = dap.POS - 1;
//                }
//                else
//                {
//                    FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
//                    if (ffl.DETPOS == 99)
//                        ppos = 1;
//                    ffl.DETPOS = ffl.DETPOS - 1;
//                    fin = true;
//                    ffl.POS = ppos;

//                    return ffl;
//                }
//            }
//            else if (pos.Equals(98))
//            {
//                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
//                dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (pos + 1)).FirstOrDefault();
//            }
//            else
//            {
//                //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
//                DET_AGENTEP actual = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (pos)).FirstOrDefault();
//                if (actual.POS == 99)
//                {
//                    fin = true;
//                }
//                else if (actual.POS == 98)
//                {
//                    //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
//                    dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (pos)).FirstOrDefault();
//                }
//                else
//                {
//                    if (actual.MONTO != null)
//                        if (d.MONTO_DOC_ML2 > actual.MONTO)
//                        {
//                            //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
//                            dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                            a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (pos + 1)).FirstOrDefault();
//                            ppos = -1;
//                        }
//                    if (actual.PRESUPUESTO != null)
//                        if ((bool)actual.PRESUPUESTO)
//                            if (d.MONTO_DOC_MD > 10000)
//                            {
//                                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
//                                dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                            a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (pos + 1)).FirstOrDefault();
//                                ppos = -1;
//                            }
//                }
//            }

//            string agente = "";
//            FLUJO f = new FLUJO();
//            f.DETPOS = 0;
//            if (!fin)
//            {
//                if (dap.PUESTOA_ID != null)
//                {
//                    //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
//                    agente = dap.USUARIOA_ID;
//                    f.DETPOS = dap.POS;
//                }
//                else
//                {
//                    dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                           a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == (sop)).FirstOrDefault();
//                    if (dap == null)
//                    {
//                        agente = d.USUARIOC_ID;
//                        f.DETPOS = 98;
//                    }
//                    else
//                    {
//                        //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
//                        agente = dap.USUARIOA_ID;
//                        f.DETPOS = dap.POS;
//                    }
//                }
//            }
//            f.POS = ppos;
//            f.USUARIOA_ID = agente;
//            return f;
//        }

//        public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTEH dah)
//        {
//            if (delega != null)
//                user = delega;
//            bool fin = false;
//            TAT001Entities db = new TAT001Entities();
//            DET_AGENTEP dap = new DET_AGENTEP();
//            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);
//            //GAUTORIZACION gg = u.GAUTORIZACIONs.Where(a => a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID)).FirstOrDefault();
//            //long gaa = db.CREADORs.Where(a => a.ID.Equals(u.ID) & a.BUKRS.Equals(d.SOCIEDAD_ID) & a.LAND.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault().AGROUP_ID;

//            if (pos.Equals(0))
//            {
//                //dap = db.DET_AGENTEP.Where(a => a.PUESTOC_ID == u.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == 1).FirstOrDefault();
//                dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
//                                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();

//            }

//            string agente = "";
//            FLUJO f = new FLUJO();
//            f.DETPOS = 0;
//            if (!fin)
//            {
//                if (dap.PUESTOA_ID != 0)
//                {
//                    //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
//                    agente = dap.USUARIOA_ID;
//                    f.DETPOS = dap.POS;
//                }
//                else
//                {
//                    //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == u.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == 99).FirstOrDefault();
//                    ////agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
//                    //agente = da.USUARIOA;
//                    //f.DETPOS = da.POS;
//                }
//            }
//            f.USUARIOA_ID = agente;
//            return f;
//        }
//    }
//}