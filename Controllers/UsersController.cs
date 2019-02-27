using RideShareWebServices.DAC;
using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace RideShareWebServices.Controllers
{
    public class UsersController : ApiController
    {
        // GET: api/Users
        [HttpGet]
        public IHttpActionResult SelectAll()
        {
            List<Users> ulist = new List<Users>();
            ulist = new UsersDAC().SelectAllUsers();

            return Ok(ulist);
        }

        [HttpGet]
        public IHttpActionResult Login(string id, string pass, string token)
        {

            Users u = new UsersDAC().SelectByStdEmpIdAndPassword(id, pass);
            if (u == null)
            {
                return NotFound();
                
            }
            u.Token = token;
            new UsersDAC().UserUpdate(u);
            return Ok(u); 
        }

        // GET: api/Users/5
        [HttpGet]
        public IHttpActionResult StdEmpId(string id)
        {

            Users u = new UsersDAC().SelectByStdEmpId(id);
            if(u==null)
            {
                return NotFound();
            }
            return Ok(u);
        }

        // GET: api/Users/5
        [HttpGet]
        public IHttpActionResult SendNotification(string phone)
        {
            Users u = new UsersDAC().SelectByPhone(phone);
            if (u == null)
            {
                return NotFound();
            }
            SendNotificationFromFirebaseCloud(u.Token);
            return Ok();
            
            
        }

        //[HttpStringDecoderFilter]
        // POST: api/Users
        [HttpPost]
        public IHttpActionResult UserInsert([FromBody]Users users)
        {
            Users u = new Users();
            u.Student_EmployeeId = users.Student_EmployeeId;
            u.UserName = users.UserName;
            u.Phone = users.Phone;
            u.PhoneStatus = users.PhoneStatus;
            u.Password = users.Password;
            u.Token = users.Token;

            new UsersDAC().UserUpdate(u);
            Users urt = new UsersDAC().SelectByStdEmpId(users.Student_EmployeeId);
            return Ok(urt);
        }

        [HttpPost]
        public IHttpActionResult UpdateUserCredential(Users user)
        {
            if (user.Password != null)
            {
                new UsersDAC().UpdatePassword(user);
                return Ok();
            }
            if (user.Phone != null)
            {
                new UsersDAC().UpdatePhone(user);
                return Ok();
            }
            return null;
            
        }  

        // PUT: api/Users/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }

        //FirebaseMessaging.getInstance().subscribeToTopic("news");

        public String SendNotificationFromFirebaseCloud(string token)
        {
            var result = "-1";
            try
            {
                var applicationID = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";

                var senderId = "996473695124";


                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = token,

                    data = new

                    {
                        notifyTo = "AllowResetPassword",
                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;
                                result = str;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                string str = ex.Message;

            }

            return result;
            }
    }
}
