using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WFARTHAconexionSAP.Entities;

namespace WFARTHAconexionSAP.Services
{
    class MailErrores
    {
        public void enviarErrores(List<string> mensajes)
        {
            WFARTHAEntities db = new WFARTHAEntities();
            APPSETTING mailC = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mail") & x.ACTIVO).FirstOrDefault();
            if (mailC == null) { Console.Write("Falta configuración!"); }//RSG 30.07.2018

            string mailt = mailC.VALUE;//RSG 30.07.2018
            CONMAIL conmail = db.CONMAILs.Find(mailt);
            if (conmail != null)
            {
                APPSETTING mailAC = db.APPSETTINGs.Where(x => x.NOMBRE.Equals("mailAdmin") & x.ACTIVO).FirstOrDefault();
                MailMessage mail = new MailMessage(conmail.MAIL, mailAC.VALUE);
                SmtpClient client = new SmtpClient();
                if (conmail.SSL)
                {
                    client.Port = (int)conmail.PORT;
                    client.EnableSsl = conmail.SSL;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                else
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(conmail.MAIL, conmail.PASS);
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = conmail.HOST;
                try
                {

                    mail.IsBodyHtml = true;
                    foreach (string mensaje in mensajes)
                    {
                        mail.Body += "<br><span>" + mensaje + "</span>";
                    }
                    mail.Subject = "Error programa Negociaciones- " + DateTime.Now.ToString();
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    throw new Exception("No se ha podido enviar el email", ex.InnerException);
                }
            }
        }
    }
}