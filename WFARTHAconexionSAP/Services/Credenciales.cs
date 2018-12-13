using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFARTHAconexionSAP.Entities;

namespace WFARTHAconexionSAP.Services
{
    public class Credenciales
    {
        WFARTHAEntities db = new WFARTHAEntities();

        public string getUserPrel()
        {
            string user = "";
            try
            {
                user = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("USER_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                user = "";
            }

            return user;

        }

        public string getPassPrel()
        {
            string pass = "";
            try
            {
                pass = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("PASS_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                pass = "";
            }

            return pass;

        }

        public string getDomPrel()
        {
            string dom = "";
            try
            {
                dom = db.APPSETTINGs.Where(aps => aps.NOMBRE.Equals("DOMA_PREL") && aps.ACTIVO == true).FirstOrDefault().VALUE.ToString();

            }
            catch (Exception e)
            {
                dom = "";
            }

            return dom;

        }
    }
}
