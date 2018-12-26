using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using WFARTHAconexionSAP.Entities;
using WFARTHAconexionSAP.Services;

namespace TATconexionSAP.Services
{
    public class Modelos
    {
        #region Variables Globales     
        private WFARTHAEntities db = new WFARTHAEntities();
        public string sap = "\\SAP";
        public string datasync = "\\out";
        public string dataproc = "\\proc";
        #endregion
        public List<string> leerArchivos()
        {
            List<string> errores = new List<string>();
            //APPSETTING sett = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("filePath") & x.ACTIVO).FirstOrDefault();//RSG 30.07.2018
            //if (sett == null) { errores.Add("Falta configuración de PATH!"); }
            ////var cadena = ConfigurationManager.AppSettings["url"];
            //var cadena = sett.VALUE;

            //MGC 22-10-2018 Archivo local en servidor----------------------->
            //Obtener la configuración del server
            string carpeta = "out";//MGC 22-10-2018 Archivo local en servidor
            string dirFile = "";
            //Obtener la configuración de la url desde app setting
            string url_prel = "";
            try
            {
                //url_prel = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();//MGC 22-10-2018 Archivo local en servidor
                //url_prel += @"POSTING";//MGC 22-10-2018 Archivo local en servidor
                url_prel = getDir(carpeta);
                dirFile = url_prel;
                if (dirFile == null | dirFile == "") { errores.Add("Falta configuración de PATH!"); }
            }
            catch (Exception e)
            {
                dirFile = ConfigurationManager.AppSettings["URL_PREL"].ToString() + @"out";
            }



            List<doc2> lstd = new List<doc2>();
            List<string> archivos2 = new List<string>();
            try
            {
                ////MGC prueba FTP---------------------------------------------------------------------------------------------------------------------------------------->
                //string ftpServerIP = "";
                //try
                //{
                //    ftpServerIP = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_FTP_PRELIMINAR") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                //    string targetFileName = "/SAP/DATA_SYNC";
                //    ftpServerIP += targetFileName;
                //}
                //catch (Exception e)
                //{

                //}

                //string username = "matias.gallegos";
                //string password = "Mimapo-2179=p23";

                //Uri uri = new Uri(String.Format("ftp://{0}/", ftpServerIP));
                //// Get the object used to communicate with the server.
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                //request.Method = WebRequestMethods.Ftp.ListDirectory;

                //// This example assumes the FTP site uses anonymous logon.
                //request.Credentials = new NetworkCredential(username, password);

                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //Stream responseStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(responseStream);



                //string line = reader.ReadLine();

                //while (!string.IsNullOrEmpty(line))
                //{
                //    archivos2.Add(line);
                //    line = reader.ReadLine();
                //}

                //reader.Close();
                //response.Close();


                ////MGC prueba FTP----------------------------------------------------------------------------------------------------------------------------------------<

                ////MGC 13-12-2018 Modificaión nuevos directorios------------->

                //Credenciales cre = new Credenciales();

                //string user = "";
                //string pass = "";
                //string dom = "";

                //user = cre.getUserPrel();
                //pass = cre.getPassPrel();
                //dom = cre.getDomPrel();

                ////string[] archivos = Directory.GetFiles(dirFile, "*", SearchOption.AllDirectories);
                //string[] archivos = new string[]();

                //using (Impersonation.LogonUser(dom, user, pass, LogonType.NewCredentials))
                //{
                //    archivos = Directory.GetFiles(dirFile, "*", SearchOption.AllDirectories);
                //}
                List<string> archivos = new List<string>();

                archivos = getArchivos(dirFile); 

                ////MGC 13-12-2018 Modificaión nuevos directorios-------------<
                //string[] archivos = Directory.GetFiles(dirFile += sap += datasync, "*.txt", SearchOption.AllDirectories);
                Console.WriteLine(archivos.Count + " _1");//RSG 30.07.2018
                //en este for sabre cuales archivos usar
                for (int i = 0; i < archivos.Count; i++)
                {
                    try
                    {
                        //separo por carpetas
                        string[] varx = archivos[i].Split('\\');
                        //separo especificamente el nombre para saber si es BUDG O LOG
                        //string[] varNA = varx[varx.Length - 1].Split('_');
                        //if (varNA[1] == "LOG")
                        //{
                        //    archivos2.Add(archivos[i]);
                        //}
                        string name = varx[varx.Length - 1];

                        if (name.IndexOf("RESPONSE") >= 0)
                        {
                            archivos2.Add(archivos[i]);
                        }
                    }catch(Exception e)
                    {

                    }
                }


                Console.WriteLine(archivos2.Count + " _2");//RSG 30.07.2018
                //en este for armo un objeto para posterior manipular hacia la bd
                //List<string> item = new List<string>();
                for (int i = 0; i < archivos2.Count; i++)
                {
                    //Leo todas las lineas del archivo
                    //string[] readText = File.ReadAllLines(archivos2[i]);
                    //foreach (var item in readText)
                    doc2 d = new doc2();
                    bool head = false;
                    List<doce> le = new List<doce>();
                    foreach (var item in File.ReadLines(archivos2[i]))
                    {
                        
                        string[] val = item.Split('|');
 
                        if (val != null)
                        {
                            if (val[0] == "P")
                            {
                                try
                                {
                                    //Leer la cabecera
                                    if (val.Length < 2)
                                        errores.Add("Archivo inválido" + archivos2[i]);

                                    else
                                    {
                                        docp pp = new docp();

                                        pp.pos = val[0];
                                        pp.tiposol = val[1];
                                        pp.numero_wf = val[2];
                                        pp.status = val[3];
                                        pp.accion = val[4];
                                        pp.Num_doc_pre = val[5];
                                        pp.Sociedad_pre = val[7];
                                        pp.Ejercicio_pre = val[6];

                                        d.Posp = pp;

                                        head = true;
                                    }
                                }catch(Exception e)
                                {

                                }
                            }

                            try
                            {
                                //Mensajes
                                //if (val[0] == "M" && val[1] == "E" && head)
                                if (val[0] == "M" && head)
                                {
                                    doce e = new doce();
                                    //Leer los mensajes
                                    if (val.Length < 2)
                                        errores.Add("Archivo inválido" + archivos2[i]);
                                    else
                                    {
                                        e.pos = val[0];
                                        e.tipo = val[1];
                                        e.id = val[2];
                                        e.numero = val[3];
                                        e.mensaje = val[4];
                                    }

                                    le.Add(e);
                                }
                            }catch(Exception e)
                            {

                            }
                        }
                    }
                    d.Pose = le;
                    lstd.Add(d);

                    ////Leer cada uno de los archivos para leer individual
                    //string targetFileName = archivos2[i];
                    //Uri urifile = new Uri(String.Format("ftp://{0}/{1}", ftpServerIP, targetFileName));
                    //// Get the object used to communicate with the server.
                    //FtpWebRequest requestf = (FtpWebRequest)WebRequest.Create(urifile);
                    //requestf.Method = WebRequestMethods.Ftp.DownloadFile;

                    //// This example assumes the FTP site uses anonymous logon.
                    //requestf.Credentials = new NetworkCredential(username, password);

                    //FtpWebResponse responsef = (FtpWebResponse)requestf.GetResponse();

                    //Stream responseStreamf = responsef.GetResponseStream();
                    //StreamReader readerf = new StreamReader(responseStreamf);


                    //string linef = readerf.ReadLine();

                    //while (!string.IsNullOrEmpty(linef))
                    //{
                    //    item.Add(linef);
                    //    linef = readerf.ReadLine();
                    //}

                    //readerf.Close();
                    //responsef.Close();

                }


                //for (int i = 0; i < item.Count; i++)
                //{
                //    doc2 d = new doc2();
                //    string[] val = item[i].Split('|');
                //    if (val != null)
                //    {
                //        if (val.Length < 2)
                //            errores.Add("Archivo inválido" + archivos2[i]);
                //        else
                //        {
                //            if (val[1] == "Error")
                //            {
                //                d.numero_wf = val[0];
                //                d.accion = val[1];
                //                d.Mensaje = val[2];
                //                //d.Cuenta_cargo = Convert.ToInt64(val[5]);
                //                //d.Cuenta_abono = Convert.ToInt64(val[6]);
                //            }
                //            else
                //            {
                //                d.numero_wf = val[0];
                //                d.accion = val[1];
                //                d.Mensaje = val[2];
                //                d.Num_doc_pre = decimal.Parse(val[3]);
                //                d.Sociedad_pre = val[4];
                //                d.Ejercicio_pre = val[5]; //MGC 11-10-2018 No enviar correos 


                //            }
                //            lstd.Add(d);
                //        }
                //    }
                //}




                try
                {
                    ValidarBd(lstd, archivos2);
                }catch(Exception e)
                {

                } 

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
                string correcto = "X";

                decimal de = Convert.ToDecimal(lstd[i].Posp.numero_wf);
                //Corroboro que exista la informacion
                //var ddd = db.DOCUMENTOes.Where(dds => dds.NUM_DOC == 1000000936).FirstOrDefault();
                var dA = db.DOCUMENTOes.Where(y => y.NUM_DOC == de).FirstOrDefault();
                //si encuentra una coincidencia
                if (dA != null)
                {

                    //MGC 16-10-2018 Eliminar sección de eliminar
                    //MGC 16-10-2018

                    DOCUMENTOPRE dp = new DOCUMENTOPRE();

                    dp.NUM_DOC = de;
                    dp.POS = 1;

                    //para el estatus E/X
                    if (lstd[i].Posp.status == string.Empty)
                    {
                        //dA.ESTATUS_SAP = "X";
                        //Creación del preliminar
                        //if (lstd[i].accion == "P")
                        if (lstd[i].Posp.accion == "CREAR")
                        {
                            dp.MESSAGE = "Error Preliminar";

                            //Cancelación del preliminar
                        }
                        //else if (lstd[i].accion == "C")
                        else if (lstd[i].Posp.accion == "BORRAR" || lstd[i].Posp.accion == "BORRAR-CREAR")
                        {
                            dp.MESSAGE = "Error Cancelación";
                        }
                        //else if (lstd[i].accion == "A")
                        else if (lstd[i].Posp.accion == "CONTABILIZAR")
                        {
                            dp.MESSAGE = "Error contabilización SAP";
                        }

                        deleteMesg(de);//MGC 16-10-2018 Eliminar msg

                        db.DOCUMENTOPREs.Add(dp);
                        db.SaveChanges();
                    }
                    else if (lstd[i].Posp.status != string.Empty)
                    {
                        if (lstd[i].Posp.status.Equals("NOK"))
                        {
                            
                            
                            if (lstd[i].Posp.accion == "CREAR")
                            //if (lstd[i].accion == "P")
                            {
                                //MGC 30-10-2018 Modificación estatus, cambiar estatus SAP = E y estatus preliminar a E que quiere decir error SAP
                                dA.ESTATUS = "N";
                                dA.ESTATUS_SAP = "E";
                                dA.ESTATUS_PRE = "E";
                                dA.ESTATUS_WF = null;//MGC 26-11-2018 Marcar como error SAP
                                db.Entry(dA).State = EntityState.Modified;
                                db.SaveChanges();//MGC 26-11-2018 Marcar como error SAP
                                //dp.MESSAGE = "Error Preliminar";

                                //MGC 30-10-2018 Guardar los mensajes para log
                                for (int j = 0; j < lstd[i].Pose.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                        dl.NUM_DOC = dA.NUM_DOC;
                                        dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                        dl.TYPE = lstd[i].Pose[j].tipo;
                                        dl.NUMBER = lstd[i].Pose[j].numero;
                                        dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                        dl.FECHA = DateTime.Now;

                                        db.DOCUMENTOLOGs.Add(dl);
                                        db.SaveChanges();

                                    }
                                    catch(Exception e)
                                    {

                                    }
                                }

                            }
                            else if (lstd[i].Posp.accion == "BORRAR" || lstd[i].Posp.accion == "BORRAR-CREAR")
                            //else if (lstd[i].accion == "C")
                            {
                                //dp.MESSAGE = "Error Cancelación";



                                //MGC 03-12-2018 Guardar los mensajes para log
                                for (int j = 0; j < lstd[i].Pose.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                        dl.NUM_DOC = dA.NUM_DOC;
                                        dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                        dl.TYPE = lstd[i].Pose[j].tipo;
                                        dl.NUMBER = lstd[i].Pose[j].numero;
                                        dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                        dl.FECHA = DateTime.Now;

                                        db.DOCUMENTOLOGs.Add(dl);
                                        db.SaveChanges();

                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }

                                if(lstd[i].Posp.accion == "BORRAR-CREAR")
                                {
                                    dA.ESTATUS = "N";
                                    dA.ESTATUS_SAP = "E";
                                    dA.ESTATUS_PRE = "E";
                                    dA.ESTATUS_WF = null;
                                    db.Entry(dA).State = EntityState.Modified;
                                    db.SaveChanges();

                                }else if(lstd[i].Posp.accion == "BORRAR")
                                {
                                    dA.ESTATUS_C = "B";
                                    db.Entry(dA).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            else if (lstd[i].Posp.accion == "CONTABILIZAR")
                            //else if (lstd[i].accion == "A")
                            {
                                //MGC 03-12-2018 Guardar los mensajes para log
                                for (int j = 0; j < lstd[i].Pose.Count; j++)
                                {
                                    try
                                    {
                                        DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                        dl.NUM_DOC = dA.NUM_DOC;
                                        dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                        dl.TYPE = lstd[i].Pose[j].tipo;
                                        dl.NUMBER = lstd[i].Pose[j].numero;
                                        dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                        dl.FECHA = DateTime.Now;

                                        db.DOCUMENTOLOGs.Add(dl);
                                        db.SaveChanges();

                                    }
                                    catch (Exception e)
                                    {

                                    }


                                }

                                dA.ESTATUS = "C";
                                dA.ESTATUS_SAP = "E";
                                dA.ESTATUS_WF = "A";
                                dA.ESTATUS_PRE = "G";
                                db.Entry(dA).State = EntityState.Modified;

                                dp.MESSAGE = "Error contabilización SAP";
                            }
                            //dp.MESSAGE = "Error Preliminar";
                            //deleteMesg(de);//MGC 16-10-2018 Eliminar msg
                            //db.DOCUMENTOPREs.Add(dp);
                            db.SaveChanges();
                        }
                        if (lstd[i].Posp.status.Equals("OK"))
                        {
                            //dA.ESTATUS_SAP = "X";
                            //Completar el flujo en el 
                            try
                            {
                                ProcesaFlujo pf = new ProcesaFlujo();
                                if (lstd[i].Posp.accion == "CREAR")
                                //if (lstd[i].Posp.accion == "P")
                                {
                                    //MGC 30-10-2018 Guardar los mensajes para log
                                    for (int j = 0; j < lstd[i].Pose.Count; j++)
                                    {
                                        try
                                        {
                                            DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                            dl.NUM_DOC = dA.NUM_DOC;
                                            dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                            dl.TYPE = lstd[i].Pose[j].tipo;
                                            dl.NUMBER = lstd[i].Pose[j].numero;
                                            dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                            dl.FECHA = DateTime.Now;

                                            db.DOCUMENTOLOGs.Add(dl);
                                            db.SaveChanges();

                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }


                                    //Procesa el flujo de autorización
                                    correcto = pf.procesa2(dp.NUM_DOC, lstd[i]);//MGC 29-10-2018 Configuración de estatus

                                }
                                else if (lstd[i].Posp.accion == "BORRAR" || lstd[i].Posp.accion == "BORRAR-CREAR")
                                //else if (lstd[i].Posp.accion == "C")
                                {
                                    
                                    //MGC 30-10-2018 Guardar los mensajes para log
                                    for (int j = 0; j < lstd[i].Pose.Count; j++)
                                    {
                                        try
                                        {
                                            DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                            dl.NUM_DOC = dA.NUM_DOC;
                                            dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                            dl.TYPE = lstd[i].Pose[j].tipo;
                                            dl.NUMBER = lstd[i].Pose[j].numero;
                                            dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                            dl.FECHA = DateTime.Now;

                                            db.DOCUMENTOLOGs.Add(dl);
                                            db.SaveChanges();

                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                    if (lstd[i].Posp.accion == "BORRAR-CREAR")
                                    {
                                        //Procesa el flujo de cancelación
                                        correcto = pf.procesaC(dp.NUM_DOC, lstd[i]);//MGC 29-10-2018 Configuración de estatus
                                    }
                                    //MGC 06-12-2018 Eliminar la solicitud ------------>
                                    else if (lstd[i].Posp.accion == "BORRAR")
                                    {
                                        //Procesa el flujo de cancelación
                                        correcto = pf.procesaCB(dp.NUM_DOC, lstd[i]);//MGC 29-10-2018 Configuración de estatus
                                    }
                                    //MGC 06-12-2018 Eliminar la solicitud ------------<
                                }
                                else if (lstd[i].Posp.accion == "CONTABILIZAR")
                                //else if (lstd[i].Posp.accion == "A")
                                {
                                    //MGC 30-10-2018 Guardar los mensajes para log
                                    for (int j = 0; j < lstd[i].Pose.Count; j++)
                                    {
                                        try
                                        {
                                            DOCUMENTOLOG dl = new DOCUMENTOLOG();

                                            dl.NUM_DOC = dA.NUM_DOC;
                                            dl.TYPE_LINE = lstd[i].Pose[j].pos;
                                            dl.TYPE = lstd[i].Pose[j].tipo;
                                            dl.NUMBER = lstd[i].Pose[j].numero;
                                            dl.MESSAGE = lstd[i].Pose[j].mensaje;
                                            dl.FECHA = DateTime.Now;

                                            db.DOCUMENTOLOGs.Add(dl);
                                            db.SaveChanges();

                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }

                                    //Proceso de contabilización
                                    correcto = pf.procesaA(dp.NUM_DOC, lstd[i]);//MGC 29-10-2018 Configuración de estatus
                                }
                            } catch(Exception e)
                            {

                            }

                        }
                    }

                    try
                    {
                        //MGC 29-10-2018 Error porque mueve los datos ya actualizados en el flujo
                        //////Hacemos el update en BD
                        //if(lstd[i].Posp.accion == "CONTABILIZAR")
                        //{
                        //    dA.DOCUMENTO_SAP = lstd[i].Posp.Num_doc_pre;
                        //}
                        //else
                        //{
                        //    dA.NUM_PRE = lstd[i].Posp.Num_doc_pre;
                        //}
                        
                        //dA.SOCIEDAD_PRE = lstd[i].Posp.Sociedad_pre;
                        //dA.EJERCICIO_PRE = lstd[i].Posp.Ejercicio_pre;//MGC 11-10-2018 No enviar correos 
                        //db.Entry(dA).State = EntityState.Modified;//MGC 11-10-2018 No enviar correos 
                        //x = x + db.SaveChanges();
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
                        try
                        {
                            moverArchivo(archivos[i], correcto);
                        }
                        catch (Exception e)
                        {

                        }



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



        //MGC 16-10-2018 ---------------------------->
        public void deleteMesg(decimal numdoc)
        {
            //Eliminar los mensajes de la tabla 
            try
            {
                db.DOCUMENTOPREs.RemoveRange(db.DOCUMENTOPREs.Where(d => d.NUM_DOC == numdoc));
                db.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        //MGC 16-10-2018 <----------------------------

        public void moverArchivos(List<string> archivo)
        {
            for (int i = 0; i < archivo.Count; i++)
            {
                try
                {
                    var from = Path.Combine(archivo[i]);
                    var arc2 = archivo[i].Replace(datasync, dataproc);
                    var to = Path.Combine(arc2);

                    if (File.Exists(to))
                    {
                        File.Delete(to);
                    }

                    File.Move(from, to); // Try to move
                }
                catch (IOException ex)
                {
                    // Console.WriteLine(ex); // Write error
                    throw new Exception(ex.Message);
                }
            }
        }

        public void moverArchivo(string archivo, string c)
        {
            //try
            //{
            //    var from = Path.Combine(archivo);
            //    var arc2 = archivo.Replace(datasync, dataproc);
            //    var to = Path.Combine(arc2);
            //    if (File.Exists(to))
            //    {
            //        File.Delete(to);
            //    }

            //    File.Move(from, to); // Try to move
            //}
            //catch (IOException ex)
            //{
            //    // Console.WriteLine(ex); // Write error
            //    throw new Exception(ex.Message);
            //}

            //if (c != "X")
                if (true)
                {
                //MGC prueba FTP---------------------------------------------------------------------------------------------------------------------------------------->

                //string ftpServerIP = "";
                //try
                //{
                //    ftpServerIP = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_FTP_PRELIMINAR") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                //    string targetFileName = "/SAP/DATA_SYNC";
                //    ftpServerIP += targetFileName;
                //}
                //catch (Exception e)
                //{

                //}

                //string username = "matias.gallegos";
                //string password = "Mimapo-2179=p23";

                //Uri uri = new Uri(String.Format("ftp://{0}/{1}", ftpServerIP, archivo));
                //// Get the object used to communicate with the server.
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                //request.Method = WebRequestMethods.Ftp.DeleteFile;

                //// This example assumes the FTP site uses anonymous logon.
                //request.Credentials = new NetworkCredential(username, password);

                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //Stream responseStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(responseStream);

                ////MGC prueba FTP----------------------------------------------------------------------------------------------------------------------------------------<


                Credenciales cre = new Credenciales();

                string user = "";
                string pass = "";
                string dom = "";

                user = cre.getUserPrel();
                pass = cre.getPassPrel();
                dom = cre.getDomPrel();

                try
                {
                    //using (Impersonation.LogonUser(dom, user, pass, LogonType.NewCredentials))
                    //{

                        var from = Path.Combine(archivo);
                        var arc2 = archivo.Replace(datasync, dataproc);
                        var to = Path.Combine(arc2);
                        if (File.Exists(to))
                        {
                            File.Delete(to);
                        }

                        File.Move(from, to); // Try to move
                    //}
                }
                catch (IOException ex)
                {
                    // Console.WriteLine(ex); // Write error
                    throw new Exception(ex.Message);
                }
            }
        }

        private string getDir(string carpeta)
        {
            string dir = "";
            try
            {
                dir = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("URL_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();
                dir += carpeta;

            }
            catch (Exception e)
            {
                dir = "";
            }

            return dir;

        }

        //MGC 13-12-2018 Modificaión nuevos directorios------------->
        private List<string> getArchivos(string dirFile)
        {
            

            Credenciales cre = new Credenciales();

            string user = "";
            string pass = "";
            string dom = "";

            user = cre.getUserPrel();
            pass = cre.getPassPrel();
            dom = cre.getDomPrel();

            //string[] archivos = Directory.GetFiles(dirFile, "*", SearchOption.AllDirectories);
            List<string> larchivos = new List<string>();

            //using (Impersonation.LogonUser(dom, user, pass, LogonType.NewCredentials))
            //{
                string[] archivosl = Directory.GetFiles(dirFile, "*", SearchOption.AllDirectories);

                if(archivosl != null)
                {
                    for(int i = 0;i< archivosl.Count(); i++)
                    {
                        larchivos.Add(archivosl[i]);
                    }
                }

            //}

            return larchivos;
            
        }

      
        //MGC 13-12-2018 Modificaión nuevos directorios-------------<
    }
}