using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using WFARTHAconexionSAP.Entities;
using WFARTHAconexionSAP.Services;

namespace TATconexionSAP.Services
{
    public class Modelos
    {
        #region Variables Globales     
        private WFARTHAEntities db = new WFARTHAEntities();
        public string sap = "\\SAP";
        public string datasync = "\\DATA_SYNC";
        public string dataproc = "\\DATA_PROC";
        #endregion
        public List<string> leerArchivos()
        {
            List<string> errores = new List<string>();
            APPSETTING sett = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") & x.ACTIVO).FirstOrDefault();//RSG 30.07.2018
            if (sett == null) { errores.Add("Falta configuración de PATH!"); }
            //var cadena = ConfigurationManager.AppSettings["url"];
            var cadena = sett.VALUE;
            List<doc2> lstd = new List<doc2>();
            List<string> archivos2 = new List<string>();
            try
            {
                string[] archivos = Directory.GetFiles(cadena += sap += datasync, "*.txt", SearchOption.AllDirectories);
                Console.WriteLine(archivos.Length + " _1");//RSG 30.07.2018
                //en este for sabre cuales archivos usar
                for (int i = 0; i < archivos.Length; i++)
                {
                    //separo por carpetas
                    string[] varx = archivos[i].Split('\\');
                    //separo especificamente el nombre para saber si es BUDG O LOG
                    string[] varNA = varx[varx.Length - 1].Split('_');
                    if (varNA[1] == "LOG")
                    {
                        archivos2.Add(archivos[i]);
                    }
                }
                Console.WriteLine(archivos2.Count + " _2");//RSG 30.07.2018
                //en este for armo un objeto para posterior manipular hacia la bd
                for (int i = 0; i < archivos2.Count; i++)
                {
                    //Leo todas las lineas del archivo
                    //string[] readText = File.ReadAllLines(archivos2[i]);
                    //foreach (var item in readText)
                    foreach (var item in File.ReadLines(archivos2[i]))
                    {
                        doc2 d = new doc2();
                        string[] val = item.Split('|');
                        if (val != null)
                        {
                            if (val.Length < 2)
                                errores.Add("Archivo inválido" + archivos2[i]);
                            else
                            {
                                if (val[1] == "Error")
                                {
                                    d.numero_wf = val[0];
                                    d.accion = val[1];
                                    d.Mensaje = val[2];
                                    //d.Cuenta_cargo = Convert.ToInt64(val[5]);
                                    //d.Cuenta_abono = Convert.ToInt64(val[6]);
                                }
                                else
                                {
                                    d.numero_wf = val[0];
                                    d.accion = val[1];
                                    d.Mensaje = val[2];
                                    d.Num_doc_pre = decimal.Parse(val[3]);
                                    d.Sociedad_pre = val[4];
                                    d.Ejercicio_pre = val[5]; //MGC 11-10-2018 No enviar correos 


                                }
                                lstd.Add(d);
                            }
                        }
                    }
                }
                ValidarBd(lstd, archivos2);

               
            }

            catch (Exception ex)
            {
                errores.Add(ex.Message);
                throw new Exception(ex.Message);
            }
            return errores;
        }

        public void ValidarBd(List<doc2> lstd, List<string> archivos)
        {
            int x = 0;
            for (int i = 0; i < lstd.Count; i++)
            {
                decimal de = Convert.ToDecimal(lstd[i].numero_wf);
                //Corroboro que exista la informacion
                var dA = db.DOCUMENTOes.Where(y => y.NUM_DOC == de).FirstOrDefault();
                //si encuentra una coincidencia
                if (dA != null)
                {
                    //Eliminar los mensajes de la tabla 
                    try
                    {
                        db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(d => d.NUM_DOC == de));
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }

                    DOCUMENTOPRE dp = new DOCUMENTOPRE();

                    dp.NUM_DOC = de;
                    dp.POS = 1;
                    
                    //para el estatus E/X
                    if (lstd[i].Mensaje == string.Empty)
                    {
                        //dA.ESTATUS_SAP = "X";
                        //Creación del preliminar
                        if(lstd[i].accion == "P")
                        {
                            dp.MESSAGE = "Error Preliminar";
                        
                        //Cancelación del preliminar
                        }else if(lstd[i].accion == "C")
                        {
                            dp.MESSAGE = "Error Cancelación";
                        }else if(lstd[i].accion == "A")
                        {
                            dp.MESSAGE = "Error contabilización SAP";
                        }
                        
                        db.DOCUMENTOPREs.Add(dp);
                        db.SaveChanges();
                    }
                    else if (lstd[i].Mensaje != string.Empty)
                    {
                        if (lstd[i].Mensaje.Equals("Error"))
                        {
                            //dA.ESTATUS_SAP = "E";
                            if (lstd[i].accion == "P")
                            {
                                dp.MESSAGE = "Error Preliminar";

                            }
                            else if (lstd[i].accion == "C")
                            {
                                dp.MESSAGE = "Error Cancelación";
                            }
                            else if (lstd[i].accion == "A")
                            {
                                dp.MESSAGE = "Error contabilización SAP";
                            }
                            //dp.MESSAGE = "Error Preliminar";
                            db.DOCUMENTOPREs.Add(dp);
                            db.SaveChanges();
                        }
                        if (lstd[i].Mensaje.Equals("Success"))
                        {
                            //dA.ESTATUS_SAP = "X";
                            //Completar el flujo en el 
                            ProcesaFlujo pf = new ProcesaFlujo();
                            if (lstd[i].accion == "P")
                            {
                                //Procesa el flujo de autorización
                                pf.procesa2(dp.NUM_DOC);

                            }
                            else if (lstd[i].accion == "C")
                            {
                                //Procesa el flujo de cancelación
                                pf.procesaC(dp.NUM_DOC);
                            }
                            else if (lstd[i].accion == "A")
                            {
                                //Proceso de contabilización
                                pf.procesaA(dp.NUM_DOC);
                            }

                        }
                    }

                    


                    try
                    {
                        ////Hacemos el update en BD
                        dA.NUM_PRE= lstd[i].Num_doc_pre;
                        dA.SOCIEDAD_PRE = lstd[i].Sociedad_pre;
                        dA.EJERCICIO_PRE = lstd[i].Ejercicio_pre;//MGC 11-10-2018 No enviar correos 
                        db.Entry(dA).State = EntityState.Modified;//MGC 11-10-2018 No enviar correos 
                        x = x + db.SaveChanges();
                        ////Agregamos en la tabla los valores
                        //DOCUMENTOSAP ds = new DOCUMENTOSAP();
                        //ds.NUM_DOC = decimal.Parse(lstd[i].numero_wf);
                        //ds.BUKRS = lstd[i].Sociedad;
                        //ds.EJERCICIO = lstd[i].Año;
                        //ds.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                        //ds.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                        //ds.BLART = lstd[i].blart;
                        //ds.KUNNR = lstd[i].kunnr;
                        //ds.DESCR = lstd[i].desc;
                        //ds.IMPORTE = lstd[i].importe;
                        //try
                        //{
                        //    db.DOCUMENTOSAPs.Add(ds);
                        //    db.SaveChanges();
                        //    moverArchivo(archivos[i]);
                        //}
                        //catch
                        //{
                        //    DOCUMENTOSAP ds1 = db.DOCUMENTOSAPs.Find(ds.NUM_DOC);
                        //    ds1.BUKRS = lstd[i].Sociedad;
                        //    ds1.EJERCICIO = lstd[i].Año;
                        //    ds1.CUENTA_A = lstd[i].Cuenta_abono.ToString();
                        //    ds1.CUENTA_C = lstd[i].Cuenta_cargo.ToString();
                        //    ds.BLART = lstd[i].blart;
                        //    ds.KUNNR = lstd[i].kunnr;
                        //    ds.DESCR = lstd[i].desc;
                        //    ds.IMPORTE = lstd[i].importe;
                        //    db.Entry(ds1).State = EntityState.Modified;

                        //    db.SaveChanges();
                        //    moverArchivo(archivos[i]);
                        //}

                        moverArchivo(archivos[i]);

                        


                    }
                    catch (Exception varEx)
                    {
                        var ex = varEx.ToString();
                    }

                    //if (dA.DOCUMENTO_REF != null)
                    //{
                    //    if (dA.DOCUMENTO_REF > 0)
                    //    {
                    //        List<DOCUMENTO> rela = db.DOCUMENTOes.Where(a => a.DOCUMENTO_REF == dA.DOCUMENTO_REF).ToList();
                    //        DOCUMENTO parcial = rela.Where(a => a.TSOL_ID == "RP").FirstOrDefault();
                    //        if (parcial != null)
                    //        {
                    //            bool contabilizados = true;
                    //            foreach (DOCUMENTO rel in rela)
                    //            {
                    //                if (rel.TSOL_ID == "RP")
                    //                    if (rel.ESTATUS_SAP == "X")
                    //                        contabilizados = false;
                    //            }

                    //            if (contabilizados)
                    //            {
                    //                FLUJO f = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();
                    //                if (f != null)
                    //                {
                    //                    f.ESTATUS = "A";
                    //                    f.FECHAM = DateTime.Now;
                    //                    ProcesaFlujo p = new ProcesaFlujo();
                    //                    string res = p.procesa(f, "");

                    //                    if (res == "0" | res == "")
                    //                    {
                    //                        FLUJO f1 = db.FLUJOes.Where(a => a.NUM_DOC == parcial.NUM_DOC).OrderByDescending(a => a.POS).FirstOrDefault();

                    //                        f.ESTATUS = "A";
                    //                        f.FECHAM = DateTime.Now;
                    //                        res = p.procesa(f, "");
                    //                    }

                    //                    //if (res == "0" | res == "")
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                }
            }
            try
            {
                //if (x == lstd.Count)
                //{
                //    moverArchivos(archivos);
                //}
            }
            catch (Exception varEx)
            {
                var ex = varEx.ToString();
                throw new Exception(ex);
            }
        }

        public void moverArchivos(List<string> archivo)
        {
            for (int i = 0; i < archivo.Count; i++)
            {
                try
                {
                    var from = Path.Combine(archivo[i]);
                    var arc2 = archivo[i].Replace(datasync, dataproc);
                    var to = Path.Combine(arc2);

                    File.Move(from, to); // Try to move
                }
                catch (IOException ex)
                {
                    // Console.WriteLine(ex); // Write error
                    throw new Exception(ex.Message);
                }
            }
        }

        public void moverArchivo(string archivo)
        {
            try
            {
                var from = Path.Combine(archivo);
                var arc2 = archivo.Replace(datasync, dataproc);
                var to = Path.Combine(arc2);

                File.Move(from, to); // Try to move
            }
            catch (IOException ex)
            {
                // Console.WriteLine(ex); // Write error
                throw new Exception(ex.Message);
            }
        }
    }
}