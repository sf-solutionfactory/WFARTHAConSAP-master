using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WFARTHAconexionSAP;
using WFARTHAconexionSAP.Entities;

namespace WFARTHAconexionSAP.Services
{
    public class ProcesaFlujo
    {

        public string procesa2(decimal num_doc)
        {
            string correcto = String.Empty;
            WFARTHAEntities db = new WFARTHAEntities();
            FLUJO actual = new FLUJO();
            string recurrente = "";
            bool emails = false; //MGC 08-10-2018 Obtener los datos para el correo
            string emailsto = ""; //MGC 09-10-2018 Envío de correos
            if (true)//---------------------------NUEVO REGISTRO
            {
                DOCUMENTO d = db.DOCUMENTOes.Find(num_doc);
                //actual.NUM_DOC = f.NUM_DOC;
                //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                //actual.COMENTARIO = f.COMENTARIO;
                //actual.ESTATUS = f.ESTATUS;
                //actual.FECHAC = f.FECHAC;
                //actual.FECHAM = f.FECHAM;
                //actual.LOOP = f.LOOP;
                //actual.NUM_DOC = f.NUM_DOC;
                //actual.POS = f.POS;
                //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
                //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID)).OrderByDescending(a => a.VERSION).FirstOrDefault();
                //DET_AGENTEC dah = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(d.USUARIOD_ID) & a.PAIS_ID == d.PAIS_ID &
                //                    a.VKORG.Equals(d.VKORG) & a.VTWEG.Equals(d.VTWEG) & a.SPART.Equals(d.SPART) & a.KUNNR.Equals(d.PAYER_ID))
                //                    .OrderByDescending(a => a.VERSION).FirstOrDefault();

                //actual.DETPOS = 1;
                //actual.DETVER = dah.VERSION;
                //actual.USUARIOA_ID = f.USUARIOA_ID;
                //actual.USUARIOD_ID = f.USUARIOD_ID;
                //actual.WF_POS = f.WF_POS;
                //actual.WF_VERSION = f.WF_VERSION;
                //actual.WORKF_ID = f.WORKF_ID;
                //f.ESTATUS = "A";
                //actual.ESTATUS = f.ESTATUS;
                //db.FLUJOes.Add(actual);

                //actual = db.FLUJOes.Where(fl => fl.NUM_DOC == num_doc).FirstOrDefault();//MGC 05-10-2018 Modificación para work flow al ser editada
                actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).OrderByDescending(x => x.POS).FirstOrDefault();//MGC 05-10-2018 Modificación para work flow al ser editada


                //MGC 08-10-2018 Obtener los datos para el correo
                WORKFV wf = db.WORKFHs.Where(a => a.ID == actual.WORKF_ID).FirstOrDefault().WORKFVs.OrderByDescending(a => a.VERSION).FirstOrDefault();

                WORKFP wp = wf.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).OrderBy(a => a.POS).FirstOrDefault();
                string email = ""; //MGC 08-10-2018 Obtener el nombre del cliente
                email = wp.EMAIL; //MGC 08-10-2018 Obtener el nombre del cliente

                if (email == "X")
                {
                    emails = true;

                    ////MGC 09-10-2018 Envío de correos
                    ////Obtener el email del creador
                    //string emailc = "";
                    //emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                    //emailsto = emailc;

                }

                int step = 0;
                if(actual.STEP_AUTO == 0)
                {
                    step = Convert.ToInt32(actual.STEP_AUTO) + 1;  
                }

                List<DET_AGENTECA> dap = db.DET_AGENTECA.Where(a => a.VERSION == actual.RUTA_VERSION && a.ID_RUTA_AGENTE == actual.ID_RUTA_A && a.STEP_FASE == step).OrderByDescending(a => a.VERSION).ToList();
                DET_AGENTECA dah = new DET_AGENTECA();
                dah = detAgenteLimite(dap, Convert.ToDecimal(d.MONTO_DOC_MD));

                
                WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
                int next_step_a = 0;
                if (paso_a.NEXT_STEP != null)
                    next_step_a = (int)paso_a.NEXT_STEP;

                WORKFP next = new WORKFP();
                if (recurrente != "X")
                {
                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
                }
                //else
                //{
                //    WORKFP autoriza = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.ACCION_ID == 5).FirstOrDefault();
                //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == autoriza.NS_ACCEPT).FirstOrDefault();
                //}
                if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
                {
                    d.ESTATUS_WF = "A";
                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "2";
                    }
                }
                else
                {
                    //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
                    FLUJO nuevo = new FLUJO();
                    nuevo.WORKF_ID = next.ID;
                    nuevo.WF_VERSION = next.VERSION;
                    nuevo.WF_POS = next.POS;
                    nuevo.NUM_DOC = actual.NUM_DOC;
                    nuevo.POS = actual.POS + 1;
                    nuevo.LOOP = 1;

                    //Agregar autorización MGC
                    nuevo.ID_RUTA_A = actual.ID_RUTA_A;
                    nuevo.RUTA_VERSION = actual.RUTA_VERSION;

                    if (next.ACCION.TIPO == "E")
                    {
                        nuevo.USUARIOA_ID = null;
                        nuevo.DETPOS = 0;
                        nuevo.DETVER = 0;
                    }
                    else
                    {
                        if (recurrente != "X")
                        {
                            FLUJO detA = determinaAgenteI(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0, dah);
                            nuevo.USUARIOA_ID = detA.USUARIOA_ID;
                            nuevo.USUARIOD_ID = nuevo.USUARIOA_ID;
                            nuevo.STEP_AUTO = detA.STEP_AUTO;

                            DateTime fecha = DateTime.Now.Date;
                            DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
                            if (del != null)
                                nuevo.USUARIOA_ID = del.USUARIOD_ID;
                            else
                                nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;

                            nuevo.DETPOS = detA.DETPOS;
                            nuevo.DETVER = dah.VERSION;

                            //MGC 09-10-2018 Envío de correos
                            if (emails)
                            {
                                emails = true;

                                //MGC 09-10-2018 Envío de correos
                                //Obtener el email del creador
                                string emailc = "";
                                emailc = db.USUARIOs.Where(us => us.ID == d.USUARIOC_ID).FirstOrDefault().EMAIL;
                                emailsto = emailc;
                                emailc = "";

                                //Obtener el usuario aprobador
                                emailc = db.USUARIOs.Where(us => us.ID == nuevo.USUARIOA_ID).FirstOrDefault().EMAIL;
                                emailsto += "," + emailc;

                                //Obtener el usuario del siguiente aprobador 


                            }

                        }
                        else
                        {
                            nuevo.USUARIOA_ID = null;
                            nuevo.DETPOS = 0;
                            nuevo.DETVER = 0;
                        }
                    }
                    nuevo.ESTATUS = "P";
                    nuevo.FECHAC = DateTime.Now;
                    nuevo.FECHAM = DateTime.Now;

                    db.FLUJOes.Add(nuevo);
                    if (paso_a.EMAIL != null)
                    {
                        if (paso_a.EMAIL.Equals("X"))
                            correcto = "1";
                    }
                    d.ESTATUS_WF = "P";
                    db.Entry(d).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            //else if (f.ESTATUS.Equals("A"))   //---------------------EN PROCESO DE APROBACIÓN
            //{
            //    actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();

            //    if (actual.ESTATUS.Equals("A"))
            //        return "1";//-----------------YA FUE PROCESADA
            //    else
            //    {
            //        var wf = actual.WORKFP;
            //        actual.ESTATUS = f.ESTATUS;
            //        actual.FECHAM = f.FECHAM;
            //        actual.COMENTARIO = f.COMENTARIO;
            //        actual.USUARIOA_ID = f.USUARIOA_ID;
            //        db.Entry(actual).State = EntityState.Modified;

            //        WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();
            //        bool ban = true;
            //        while (ban)         //--------------PARA LOOP
            //        {
            //            int next_step_a = 0;
            //            int next_step_r = 0;
            //            if (paso_a.NEXT_STEP != null)
            //                next_step_a = (int)paso_a.NEXT_STEP;
            //            if (paso_a.NS_ACCEPT != null)
            //                next_step_a = (int)paso_a.NS_ACCEPT;
            //            if (paso_a.NS_REJECT != null)
            //                next_step_r = (int)paso_a.NS_REJECT;

            //            WORKFP next = new WORKFP();
            //            if (paso_a.ACCION.TIPO == "A" | paso_a.ACCION.TIPO == "N" | paso_a.ACCION.TIPO == "R" | paso_a.ACCION.TIPO == "T" | paso_a.ACCION.TIPO == "E")//Si está en proceso de aprobación
            //            {
            //                if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
            //                {
            //                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

            //                    FLUJO nuevo = new FLUJO();
            //                    nuevo.WORKF_ID = paso_a.ID;
            //                    nuevo.WF_VERSION = paso_a.VERSION;
            //                    nuevo.WF_POS = next.POS;
            //                    nuevo.NUM_DOC = actual.NUM_DOC;
            //                    nuevo.POS = actual.POS + 1;
            //                    nuevo.LOOP = 1;//-----------------------------------cc
            //                                   //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
            //                                   //if (loop >= next.LOOPS)
            //                                   //{
            //                                   //    paso_a = next;
            //                                   //    continue;
            //                                   //}
            //                                   //if (loop != 0)
            //                                   //    nuevo.LOOP = loop + 1;
            //                                   //else
            //                                   //    nuevo.LOOP = 1;
            //                    FLUJO detA = new FLUJO();
            //                    if (paso_a.ACCION.TIPO == "N")
            //                        actual.DETPOS = actual.DETPOS - 1;
            //                    int sop = 99;
            //                    if (next.ACCION.TIPO == "S")
            //                        sop = 98;
            //                    detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, actual.DETPOS, next.LOOPS, sop);
            //                    //nuevo.USUARIOA_ID = detA.USUARIOA_ID;






            //                    nuevo.USUARIOD_ID = detA.USUARIOA_ID;

            //                    DateTime fecha = DateTime.Now.Date;
            //                    DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
            //                    if (del != null)
            //                        nuevo.USUARIOA_ID = del.USUARIOD_ID;
            //                    else
            //                        nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;





            //                    nuevo.DETPOS = detA.DETPOS;
            //                    nuevo.DETVER = actual.DETVER;
            //                    if (paso_a.ACCION.TIPO == "N")
            //                    {
            //                        nuevo.DETPOS = nuevo.DETPOS + 1;
            //                        actual.DETPOS = actual.DETPOS + 1;
            //                    }

            //                    //if (d.DOCUMENTORECs.Count > 0)
            //                    //    recurrente = "X";

            //                    if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
            //                    {
            //                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
            //                        if (recurrente == "X" & next.ACCION.TIPO.Equals("P"))
            //                        {
            //                            next_step_a++;
            //                            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
            //                        }
            //                        if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
            //                        {
            //                            //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                            d.ESTATUS_WF = "A";
            //                            if (paso_a.EMAIL.Equals("X"))
            //                                correcto = "2";
            //                            if (recurrente == "X")
            //                            {
            //                                FLUJO nuevos = new FLUJO();
            //                                nuevos.WORKF_ID = paso_a.ID;
            //                                nuevos.WF_VERSION = paso_a.VERSION;
            //                                nuevos.WF_POS = next.POS;
            //                                nuevos.NUM_DOC = actual.NUM_DOC;
            //                                nuevos.POS = actual.POS + 1;
            //                                nuevos.ESTATUS = "A";
            //                                nuevos.FECHAC = DateTime.Now;
            //                                nuevos.FECHAM = DateTime.Now;

            //                                d.ESTATUS = "A";

            //                                db.FLUJOes.Add(nuevos);
            //                                //db.SaveChanges();
            //                                ban = false;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                            //FLUJO nuevo = new FLUJO();
            //                            nuevo.WORKF_ID = next.ID;
            //                            nuevo.WF_VERSION = next.VERSION;
            //                            nuevo.WF_POS = next.POS + detA.POS;
            //                            nuevo.NUM_DOC = actual.NUM_DOC;
            //                            nuevo.POS = actual.POS + 1;
            //                            nuevo.LOOP = 1;//-----------------------------------

            //                            //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
            //                            //nuevo.USUARIOA_ID = "admin";
            //                            //nuevo.DETPOS = 1;
            //                            bool finR = false;
            //                            d.ESTATUS_WF = "P";
            //                            if (next.ACCION.TIPO.Equals("T"))
            //                            {
            //                                //TAX_LAND tl = db.TAX_LAND.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault();
            //                                //if (tl != null)
            //                                //{
            //                                //    //nuevo.USUARIOA_ID = db.DET_TAX.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.PAIS_ID.Equals(d.PAIS_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
            //                                //    nuevo.USUARIOA_ID = db.DET_TAXEO.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PAIS_ID.Equals(d.PAIS_ID) & a.PUESTOC_ID == d.PUESTO_ID & a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.ACTIVO == true).FirstOrDefault().USUARIOA_ID;
            //                                //    d.ESTATUS_WF = "T";
            //                                //}
            //                                //else
            //                                //{
            //                                nuevo.WF_POS = nuevo.WF_POS + 1;
            //                                nuevo.USUARIOA_ID = null;
            //                                d.ESTATUS_WF = "A";
            //                                d.ESTATUS_SAP = "P";
            //                                if (recurrente == "X")
            //                                {
            //                                    nuevo.WF_POS++;
            //                                    d.ESTATUS_SAP = "";
            //                                    finR = true;
            //                                }
            //                                //}
            //                            }
            //                            else if (paso_a.ACCION.TIPO == "E")
            //                            {
            //                                nuevo.USUARIOA_ID = null;
            //                            }
            //                            else
            //                            {
            //                                if (nuevo.DETPOS == 0)
            //                                {
            //                                    nuevo.USUARIOA_ID = null;
            //                                    d.ESTATUS_WF = "A";
            //                                    d.ESTATUS_SAP = "P";
            //                                }
            //                            }
            //                            nuevo.ESTATUS = "P";
            //                            nuevo.FECHAC = DateTime.Now;
            //                            nuevo.FECHAM = DateTime.Now;

            //                            if (finR)
            //                            {
            //                                nuevo.ESTATUS = "A";
            //                                d.ESTATUS = "A";
            //                            }

            //                            db.FLUJOes.Add(nuevo);
            //                            if (paso_a.EMAIL.Equals("X"))
            //                                correcto = "1";
            //                        }
            //                    }
            //                    //else if(nuevo.DETPOS == 99)
            //                    //{
            //                    //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
            //                    //}
            //                    else
            //                    {
            //                        //nuevo.USUARIOD_ID
            //                        nuevo.ESTATUS = "P";
            //                        nuevo.FECHAC = DateTime.Now;
            //                        nuevo.FECHAM = DateTime.Now;
            //                        nuevo.WF_POS = nuevo.WF_POS + detA.POS;

            //                        db.FLUJOes.Add(nuevo);
            //                        if (paso_a.EMAIL.Equals("X"))
            //                            correcto = "1";

            //                        d.ESTATUS_WF = "P";
            //                    }
            //                    if (nuevo.DETPOS.Equals(98))
            //                        d.ESTATUS_WF = "S";
            //                    db.Entry(d).State = EntityState.Modified;

            //                    db.SaveChanges();
            //                    ban = false;
            //                }
            //            }
            //            else if (paso_a.ACCION.TIPO == "P")
            //            {
            //                if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
            //                {
            //                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);

            //                    //ArchivoContable sa = new ArchivoContable();
            //                    //string file = sa.generarArchivo(d.NUM_DOC, 0);
            //                    string file = "";

            //                    if (file == "")
            //                    {
            //                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

            //                        FLUJO nuevo = new FLUJO();
            //                        nuevo.WORKF_ID = paso_a.ID;
            //                        nuevo.WF_VERSION = paso_a.VERSION;
            //                        nuevo.WF_POS = next.POS;
            //                        nuevo.NUM_DOC = actual.NUM_DOC;
            //                        nuevo.POS = actual.POS + 1;
            //                        nuevo.ESTATUS = "A";
            //                        nuevo.FECHAC = DateTime.Now;
            //                        nuevo.FECHAM = DateTime.Now;

            //                        d.ESTATUS = "A";
            //                        correcto = file;

            //                        db.FLUJOes.Add(nuevo);
            //                        db.SaveChanges();
            //                        ban = false;
            //                    }
            //                    else
            //                    {
            //                        ban = false;
            //                        correcto = file;
            //                    }
            //                }
            //            }
            //            else if (paso_a.ACCION.TIPO == "S")
            //            {
            //                if (f.ESTATUS.Equals("A") | f.ESTATUS.Equals("N"))//APROBAR SOLICITUD
            //                {
            //                    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();

            //                    FLUJO nuevo = new FLUJO();
            //                    nuevo.WORKF_ID = paso_a.ID;
            //                    nuevo.WF_VERSION = paso_a.VERSION;
            //                    nuevo.WF_POS = next.POS;
            //                    nuevo.NUM_DOC = actual.NUM_DOC;
            //                    nuevo.POS = actual.POS + 1;
            //                    nuevo.LOOP = 1;//-----------------------------------cc
            //                    //int loop = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
            //                    //if (loop >= next.LOOPS)
            //                    //{
            //                    //    paso_a = next;
            //                    //    continue;
            //                    //}
            //                    //if (loop != 0)
            //                    //    nuevo.LOOP = loop + 1;
            //                    //else
            //                    //    nuevo.LOOP = 1;
            //                    if (paso_a.ACCION.TIPO == "N")
            //                        actual.DETPOS = actual.DETPOS - 1;
            //                    FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 98, next.LOOPS, 98);
            //                    //nuevo.USUARIOA_ID = detA.USUARIOA_ID;





            //                    nuevo.USUARIOD_ID = detA.USUARIOA_ID;

            //                    DateTime fecha = DateTime.Now.Date;
            //                    DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
            //                    if (del != null)
            //                        nuevo.USUARIOA_ID = del.USUARIOD_ID;
            //                    else
            //                        nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;





            //                    nuevo.DETPOS = detA.DETPOS;
            //                    nuevo.DETVER = actual.DETVER;
            //                    if (paso_a.ACCION.TIPO == "N")
            //                    {
            //                        nuevo.DETPOS = nuevo.DETPOS + 1;
            //                        actual.DETPOS = actual.DETPOS + 1;
            //                    }


            //                    if (nuevo.DETPOS == 0 | nuevo.DETPOS == 99)
            //                    {
            //                        next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
            //                        if (next.NEXT_STEP.Equals(99))//--------FIN DEL WORKFLOW
            //                        {
            //                            //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                            d.ESTATUS_WF = "A";
            //                            if (paso_a.EMAIL.Equals("X"))
            //                                correcto = "2";
            //                        }
            //                        else
            //                        {
            //                            //DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //                            //FLUJO nuevo = new FLUJO();
            //                            nuevo.WORKF_ID = next.ID;
            //                            nuevo.WF_VERSION = next.VERSION;
            //                            nuevo.WF_POS = next.POS + detA.POS;
            //                            nuevo.NUM_DOC = actual.NUM_DOC;
            //                            nuevo.POS = actual.POS + 1;
            //                            nuevo.LOOP = 1;//-----------------------------------
            //                                           //int loop1 = db.FLUJOes.Where(a => a.WORKF_ID.Equals(next.ID) & a.WF_VERSION.Equals(next.VERSION) & a.WF_POS == next.POS & a.NUM_DOC.Equals(actual.NUM_DOC) & a.ESTATUS.Equals("A")).Count();
            //                                           //if (loop1 >= next.LOOPS)
            //                                           //{
            //                                           //    paso_a = next;
            //                                           //    continue;
            //                                           //}
            //                                           //if (loop1 != 0)
            //                                           //    nuevo.LOOP = loop1 + 1;
            //                                           //else
            //                                           //    nuevo.LOOP = 1;

            //                            //FLUJO detA = determinaAgente(d, actual.USUARIOA_ID, actual.USUARIOD_ID, 0);
            //                            //nuevo.USUARIOA_ID = "admin";
            //                            //nuevo.DETPOS = 1;
            //                            d.ESTATUS_WF = "P";
            //                            if (nuevo.DETPOS == 0)
            //                            {
            //                                nuevo.USUARIOA_ID = null;
            //                                d.ESTATUS_WF = "A";
            //                                d.ESTATUS_SAP = "P";
            //                            }
            //                            nuevo.ESTATUS = "P";
            //                            nuevo.FECHAC = DateTime.Now;
            //                            nuevo.FECHAM = DateTime.Now;

            //                            db.FLUJOes.Add(nuevo);
            //                            if (paso_a.EMAIL.Equals("X"))
            //                                correcto = "1";
            //                        }
            //                    }
            //                    //else if(nuevo.DETPOS == 99)
            //                    //{
            //                    //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_a).FirstOrDefault();
            //                    //}
            //                    else
            //                    {
            //                        //nuevo.USUARIOD_ID
            //                        nuevo.ESTATUS = "P";
            //                        nuevo.FECHAC = DateTime.Now;
            //                        nuevo.FECHAM = DateTime.Now;
            //                        nuevo.WF_POS = nuevo.WF_POS + detA.POS;

            //                        db.FLUJOes.Add(nuevo);
            //                        if (paso_a.EMAIL.Equals("X"))
            //                            correcto = "1";

            //                        d.ESTATUS_WF = "P";
            //                    }
            //                    db.Entry(d).State = EntityState.Modified;

            //                    db.SaveChanges();
            //                    ban = false;
            //                }
            //            }

            //        }
            //    }
            //}
            //else if (f.ESTATUS.Equals("R"))//Rechazada
            //{
            //    actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
            //    WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();

            //    int next_step_a = 0;
            //    int next_step_r = 0;
            //    if (paso_a.NEXT_STEP != null)
            //        next_step_a = (int)paso_a.NEXT_STEP;
            //    if (paso_a.NS_ACCEPT != null)
            //        next_step_a = (int)paso_a.NS_ACCEPT;
            //    if (paso_a.NS_REJECT != null)
            //        next_step_r = (int)paso_a.NS_REJECT;

            //    WORKFP next = new WORKFP();
            //    next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

            //    correcto = "3";
            //    actual.ESTATUS = f.ESTATUS;
            //    actual.FECHAM = f.FECHAM;
            //    actual.COMENTARIO = f.COMENTARIO;

            //    FLUJO nuevo = new FLUJO();
            //    nuevo.WORKF_ID = next.ID;
            //    nuevo.WF_VERSION = next.VERSION;
            //    nuevo.WF_POS = next.POS;
            //    nuevo.NUM_DOC = actual.NUM_DOC;
            //    nuevo.POS = actual.POS + 1;
            //    nuevo.DETPOS = 1;
            //    nuevo.DETVER = actual.DETVER;
            //    nuevo.LOOP = 1;//-----------------------------------
            //    DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);
            //    nuevo.USUARIOD_ID = d.USUARIOD_ID;
            //    DateTime fecha = DateTime.Now.Date;
            //    DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
            //    if (del != null)
            //        nuevo.USUARIOA_ID = del.USUARIOD_ID;
            //    else
            //        nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;
            //    //nuevo.USUARIOD_ID
            //    nuevo.ESTATUS = "P";
            //    nuevo.FECHAC = DateTime.Now;
            //    nuevo.FECHAM = DateTime.Now;

            //    db.FLUJOes.Add(nuevo);
            //    db.Entry(actual).State = EntityState.Modified;
            //    d.ESTATUS_WF = "R";

            //    db.SaveChanges();

            //}

            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//
            //-------------------------------------------------------------------------------------------------------------------------------//

            //MGC 08-10-2018 Obtener los datos para el correo
            if (emails)
            {
                //Obtener el directorio desde la configuración


                //MGC 08-10-2018 Obtener los datos para el correo comentar provisional
                Email em = new Email();
                string UrlDirectory = getURLPortal();
                //string image = Server.MapPath("~/images/artha_logo.jpg");
                string image = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string page = "Index";

                //MGC 11-10-2018 No enviar correos 
                //em.enviaMailC(num_doc, true, "ES", UrlDirectory, page, image, emailsto);

            }


            return correcto;
        }
      

        public string procesaC(decimal num_doc)
        {
            WFARTHAEntities db = new WFARTHAEntities();

            FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC.Equals(num_doc)).OrderByDescending(a => a.POS).FirstOrDefault();

            string correcto = String.Empty;
            FLUJO actual = new FLUJO();
           
            actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(f.NUM_DOC)).OrderByDescending(a => a.POS).FirstOrDefault();
            WORKFP paso_a = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS.Equals(actual.WF_POS)).FirstOrDefault();

            int next_step_a = 0;
            int next_step_r = 0;
            if (paso_a.NEXT_STEP != null)
                next_step_a = (int)paso_a.NEXT_STEP;
            if (paso_a.NS_ACCEPT != null)
                next_step_a = (int)paso_a.NS_ACCEPT;
            if (paso_a.NS_REJECT != null)
                next_step_r = (int)paso_a.NS_REJECT;

            if (paso_a.NS_REJECT == null)
                next_step_r = 1;

            WORKFP next = new WORKFP();
            next = db.WORKFPs.Where(a => a.ID.Equals(actual.WORKF_ID) & a.VERSION.Equals(actual.WF_VERSION) & a.POS == next_step_r).FirstOrDefault();

            correcto = "3";
            actual.ESTATUS = f.ESTATUS;
            actual.FECHAM = f.FECHAM;
            actual.COMENTARIO = f.COMENTARIO;

            FLUJO nuevo = new FLUJO();
            nuevo.WORKF_ID = next.ID;
            nuevo.WF_VERSION = next.VERSION;
            nuevo.WF_POS = next.POS;
            nuevo.NUM_DOC = actual.NUM_DOC;
            nuevo.POS = actual.POS + 1;
            nuevo.DETPOS = 1;
            nuevo.DETVER = actual.DETVER;
            nuevo.LOOP = 1;//-----------------------------------
            DOCUMENTO d = db.DOCUMENTOes.Find(actual.NUM_DOC);

            //MGC 
            //nuevo.USUARIOD_ID = d.USUARIOD_ID;
            //Datos del flujo
            nuevo.USUARIOD_ID = d.USUARIOC_ID;
            nuevo.USUARIOA_ID = d.USUARIOC_ID;

            //Datos del flujo versiones
            nuevo.ID_RUTA_A = actual.ID_RUTA_A;
            nuevo.RUTA_VERSION = actual.RUTA_VERSION;
            nuevo.STEP_AUTO = 0;

            DateTime fecha = DateTime.Now.Date;
            DELEGAR del = db.DELEGARs.Where(a => a.USUARIO_ID.Equals(nuevo.USUARIOD_ID) & a.FECHAI <= fecha & a.FECHAF >= fecha & a.ACTIVO == true).FirstOrDefault();
            if (del != null)
                nuevo.USUARIOA_ID = del.USUARIOD_ID;
            else
                nuevo.USUARIOA_ID = nuevo.USUARIOD_ID;
            //nuevo.USUARIOD_ID
            nuevo.ESTATUS = "P";
            nuevo.FECHAC = DateTime.Now;
            nuevo.FECHAM = DateTime.Now;

            db.FLUJOes.Add(nuevo);//MGC Cancelar Preliminar

            db.Entry(actual).State = EntityState.Modified;
            d.ESTATUS_WF = "R";
            if (next.ACCION.TIPO == "S")
            {
                d.ESTATUS = "R";
                d.ESTATUS_WF = "S";
            }

            //MGC Cancelar Preliminar
            //Eliminar los mensajes de la tabla 
            try
            {
                db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(dpre => dpre.NUM_DOC == actual.NUM_DOC));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
            
            db.SaveChanges();

            return correcto;
        }

        public string procesaA(decimal num_doc)
        {
            WFARTHAEntities db = new WFARTHAEntities();
            DOCUMENTOPRE dp = new DOCUMENTOPRE();

            dp.NUM_DOC = num_doc;
            dp.POS = 1;
            dp.MESSAGE = "Contabilizado en SAP";

            db.DOCUMENTOPREs.Add(dp);
            db.SaveChanges();
            

            return "";
        }

        public FLUJO determinaAgente(DOCUMENTO d, string user, string delega, int pos, int? loop, int sop, int fase_det)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            FLUJO f_actual = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC)).FirstOrDefault();
            //DET_AGENTEH dah = db.DET_AGENTEH.Where(a => a.SOCIEDAD_ID.Equals(d.SOCIEDAD_ID) & a.PUESTOC_ID == d.PUESTO_ID &
            //                    a.USUARIOC_ID.Equals(d.USUARIOC_ID) & a.VERSION == f_actual.DETVER).FirstOrDefault();

            //Obtener lista de agentes y obtener uno dependiendo del monto -----------------------------------------------------
            List<DET_AGENTECA> dah = db.DET_AGENTECA.Where(a => a.VERSION == f_actual.RUTA_VERSION && a.ID_RUTA_AGENTE == f_actual.ID_RUTA_A && a.STEP_FASE == fase_det).ToList();

            if (dah != null || dah.Count > 0)
            {

                dap = detAgenteLimite(dah, Convert.ToDecimal(d.MONTO_DOC_MD));
            }
            else
            {
                //El flujo termino

            }
            int ppos = 0;
            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

            //if (pos.Equals(0))
            //{
            //    if (loop == null)
            //    {
            //        //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
            //        //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
            //        dap = dah.Where(a => a.POS == 1).FirstOrDefault();
            //        dap.POS = dap.POS - 1;
            //    }
            //    else
            //    {
            //        FLUJO ffl = db.FLUJOes.Where(a => a.NUM_DOC.Equals(d.NUM_DOC) & a.ESTATUS.Equals("R")).OrderByDescending(a => a.POS).FirstOrDefault();
            //        if (ffl.DETPOS == 99)
            //            ppos = 1;
            //        ffl.DETPOS = ffl.DETPOS - 1;
            //        fin = true;
            //        ffl.POS = ppos;

            //        return ffl;
            //    }
            //}
            //else if (pos.Equals(98))
            //{
            //    dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //}
            //else
            //{
            //    //DET_AGENTE actual = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos)).FirstOrDefault();
            //    DET_AGENTEC actual = dah.Where(a => a.POS == (pos)).FirstOrDefault();
            //    if (actual.POS == 99)
            //    {
            //        fin = true;
            //    }
            //    else if (actual.POS == 98)
            //    {
            //        //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //        dap = dah.Where(a => a.POS == (pos)).FirstOrDefault();
            //    }
            //    else
            //    {
            //        if (actual.MONTO != null)
            //            if (d.MONTO_DOC_ML2 > actual.MONTO)
            //            {
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //        //if (actual.PRESUPUESTO != null)
            //        if ((bool)actual.PRESUPUESTO)
            //            if (d.MONTO_DOC_MD > 100000)
            //            {
            //                //da = db.DET_AGENTE.Where(a => a.PUESTOC_ID == d.PUESTO_ID & a.AGROUP_ID == gaa & a.POS == (pos + 1)).FirstOrDefault();
            //                dap = dah.Where(a => a.POS == (pos + 1)).FirstOrDefault();
            //                ppos = -1;
            //            }
            //    }
            //}

            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;
            //if (!fin)
            //{
            //    if (dap != null)
            //    {
            //        if (dap.USUARIOA_ID != null)
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //        else
            //        {
            //            dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //            if (dap == null)
            //            {
            //                agente = d.USUARIOD_ID;
            //                f.DETPOS = 98;
            //            }
            //            else
            //            {
            //                //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //                agente = dap.USUARIOA_ID;
            //                f.DETPOS = dap.POS;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        dap = dah.Where(a => a.POS == (sop)).FirstOrDefault();
            //        if (dap == null)
            //        {
            //            agente = d.USUARIOD_ID;
            //            f.DETPOS = 98;
            //        }
            //        else
            //        {
            //            //agente = db.GAUTORIZACIONs.Where(a => a.ID == da.AGROUP_ID).FirstOrDefault().USUARIOs.Where(a => a.PUESTO_ID == da.PUESTOA_ID).First().ID;
            //            agente = dap.USUARIOA_ID;
            //            f.DETPOS = dap.POS;
            //        }
            //    }
            //}
            f.POS = ppos;
            if (agente != "")
                f.USUARIOA_ID = agente;
            else
                f.USUARIOA_ID = null;
            return f;
        }

        public FLUJO determinaAgenteI(DOCUMENTO d, string user, string delega, int pos, DET_AGENTECA dah)
        {
            if (delega != null)
                user = delega;
            bool fin = false;
            WFARTHAEntities db = new WFARTHAEntities();
            DET_AGENTECA dap = new DET_AGENTECA();
            USUARIO u = db.USUARIOs.Find(d.USUARIOC_ID);

            if (pos.Equals(0))
            {
                //dap = db.DET_AGENTEP.Where(a => a.SOCIEDAD_ID.Equals(dah.SOCIEDAD_ID) & a.PUESTOC_ID == dah.PUESTOC_ID &
                //                    a.VERSION == dah.VERSION & a.AGROUP_ID == dah.AGROUP_ID & a.POS == 1).FirstOrDefault();
                //dap = db.DET_AGENTEC.Where(a => a.USUARIOC_ID.Equals(delega) & a.PAIS_ID == dah.PAIS_ID &
                //                   a.VKORG.Equals(dah.VKORG) & a.VTWEG.Equals(dah.VTWEG) & a.SPART.Equals(dah.SPART) & a.KUNNR.Equals(dah.KUNNR) &
                //                   a.VERSION == dah.VERSION & a.POS == 1).FirstOrDefault();

                List<DET_AGENTECA> dal = db.DET_AGENTECA.Where(a => a.VERSION == dah.VERSION && a.ID_RUTA_AGENTE == dah.ID_RUTA_AGENTE && a.STEP_FASE == dah.STEP_FASE).OrderByDescending(a => a.VERSION).ToList();

                dap = detAgenteLimite(dal, Convert.ToDecimal(d.MONTO_DOC_MD));


            }

            //string agente = "";
            //FLUJO f = new FLUJO();
            //f.DETPOS = 0;
            //if (!fin)
            //{
            //    agente = dap.USUARIOA_ID;
            //    f.DETPOS = dap.POS;
            //}
            //f.USUARIOA_ID = agente;
            //return f;

            string agente = "";
            FLUJO f = new FLUJO();
            f.DETPOS = 0;
            if (!fin)
            {
                //agente = dap.USUARIOA_ID;
                agente = dap.AGENTE_SIG;
                //f.DETPOS = dap.POS;
                f.DETPOS = dap.STEP_FASE;
                f.STEP_AUTO = dap.STEP_FASE;
            }
            f.USUARIOA_ID = agente;
            return f;
        }

        //Obtener el agente que se ajusta de a partir del monto del documento
        public DET_AGENTECA detAgenteLimite(List<DET_AGENTECA> lag, decimal monto)
        {
            List<DET_AGENTECA> lr = new List<DET_AGENTECA>();
            foreach (DET_AGENTECA ag in lag)
            {
                if (monto < ag.LIM_SUP)
                {
                    lr.Add(ag);
                }
            }

            //Se obtuvieron los registros que pueden validar el monto
            DET_AGENTECA rv = new DET_AGENTECA();

            rv = lr.OrderBy(ll => ll.LIM_SUP).FirstOrDefault();


            return rv;
        }

        public string getURLPortal()
        {
            WFARTHAEntities db = new WFARTHAEntities();
            string url = "";

            try
            {
                url = db.APPSETTINGs.Where(app => app.NOMBRE == "urlPortal" && app.ACTIVO == true).FirstOrDefault().VALUE.ToString();
            }catch(Exception e)
            {

            }

            if(url == null | url == "")
            {
                url = "http://localhost:60621/Correos/Index/";
            }

            return url;
        }
}
}