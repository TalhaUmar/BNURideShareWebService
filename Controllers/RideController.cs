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
    public class RideController : ApiController
    {
        [HttpPost]
        public IHttpActionResult RidePost([FromBody]RideModel ride)
        {
            Ride r = new Ride();
            r.DepartureTime = ride.DepartureTime;
            r.DepartureDate = ride.DepartureDate;
            r.PostDate = ride.PostDate;
            r.AvailableSeats = ride.AvailableSeats;
            r.CostPerKm = ride.CostPerKm;
            r.RideFrequency = ride.RideFrequency;
            r.VehicleType = ride.VehicleType;
            r.Smoking = ride.Smoking;
            r.FoodDrinks = ride.FoodDrinks;
            r.UserId = ride.UserId;
            r.Source = ride.Source;
            r.Destination = ride.Destination;
            r.Checkpoints = ride.Checkpoints;
            r.RideStatusId = 1;

            new RideDAC().Insert(r);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SelectAllRides()
        {
            Ride r = new Ride();
            r.RideStatusId = 1;
            List<Ride> rides = new RideDAC().SelectAllRides(r);
            if (rides != null)
            {
                return Ok(rides);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult SelectPostedRides(int uid)
        {
            Ride r = new Ride();
            r.RideStatusId = 1;
            r.UserId = uid;
            List<Ride> rides = new RideDAC().SelectAllRidesByUserId(r);
            if (rides != null)
            {
                return Ok(rides);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult SelectJoinedRides(int uid)
        {
            RideRequest rr = new RideRequest();
            rr.PassengerId = uid;
            rr.RequestStatus ="2";
            List<RideRequest> rrlist = new RideRequestDAC().SelectALLAcceptedRequestByPassengerId(rr);
            List<Ride> rides = new List<Ride>();
            if (rrlist != null)
            {
                foreach (RideRequest m in rrlist)
                {
                    Ride rtemp = new RideDAC().SelectById(m.RideId);
                    rides.Add(rtemp);
                }
                rides.TrimExcess();
                return Ok(rides);
            }
            else return NotFound();
        }

        [HttpGet]
        public IHttpActionResult SelectRideRequests(int uid)
        {
            Ride r = new Ride();
            r.RideStatusId = 1;
            r.UserId = uid;
            List<Ride> rides = new RideDAC().SelectAllRidesByUserId(r);
            List<RideRequest> rtemplist = new List<RideRequest>();
            List<JoinedRidersModel> jrlist = new List<JoinedRidersModel>();
            if (rides != null)
            {
             foreach (Ride ride in rides)
                {
                    rtemplist = new RideRequestDAC().SelectAllPendingRideRequestByRideId(ride.Id);
                    if (rtemplist != null)
                    {
                        foreach (RideRequest m in rtemplist)
                        {
                            JoinedRidersModel jrmodel = new JoinedRidersModel();
                            Users utemp = new UsersDAC().SelectById(m.PassengerId);
                            jrmodel.Id = m.Id;
                            jrmodel.PassengerId = m.PassengerId;
                            jrmodel.PickupLocation = m.PickupLocation;
                            jrmodel.Destination = m.Destination;
                            jrmodel.RideId = m.RideId;
                            jrmodel.RequestStatus = m.RequestStatus;
                            jrmodel.TotalAmount = m.TotalAmount;
                            jrmodel.UserName = utemp.UserName;
                            jrlist.Add(jrmodel);

                        }
                        rtemplist.Clear();
                    }
                    
                }
                jrlist.TrimExcess();
                return Ok(jrlist);      
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult JoinedRiders(int rid) 
        {
            RideRequest rr = new RideRequest();
            rr.RideId = rid;
            rr.RequestStatus = "2";
            List<RideRequest> rrlist = new RideRequestDAC().SelectALLAcceptedRequestByRideId(rr);
            List<JoinedRidersModel> jrlist = new List<JoinedRidersModel>();
            if (rrlist != null)
            {
                foreach (RideRequest m in rrlist)
                {
                    JoinedRidersModel jrmodel = new JoinedRidersModel();
                    Users utemp = new UsersDAC().SelectById(m.PassengerId);
                    jrmodel.Id = m.Id;
                    jrmodel.PassengerId = m.PassengerId;
                    jrmodel.PickupLocation = m.PickupLocation;
                    jrmodel.Destination = m.Destination;
                    jrmodel.RideId = m.RideId;
                    jrmodel.RequestStatus = m.RequestStatus;
                    jrmodel.TotalAmount = m.TotalAmount;
                    jrmodel.UserName = utemp.UserName;
                    jrlist.Add(jrmodel);
                }
                jrlist.TrimExcess();
                return Ok(jrlist);
            }
            else return NotFound();
        }

        [HttpPost]
        public IHttpActionResult RideRequestNotify(RideRequest rr) 
        {
            RideRequest rrquest = new RideRequest();
            rrquest.PassengerId = rr.PassengerId;
            rrquest.RideId = rr.RideId;
            rrquest.PickupLocation = rr.PickupLocation;
            rrquest.Destination = rr.Destination;
            rrquest.TotalAmount = rr.TotalAmount;
            rrquest.RequestStatus = "1";
            new RideRequestDAC().Insert(rrquest);
            Ride r = new RideDAC().SelectById(rr.RideId);
            Users u = new UsersDAC().SelectById(r.UserId);
            Users passenger = new UsersDAC().SelectById(rr.PassengerId);
            string message = "" + passenger.UserName + " requested to join your ride.";
            String res = SendNotificationFromFirebaseCloud(u.Token,passenger,rrquest, r, message, "Driver");
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult AcceptRejectRideRequestNotify(int uid,int rid, string action)
        {
              if(action == "AcceptRequest")
              {
                  RideRequest rRequest = new RideRequest();
                  rRequest.PassengerId = uid;
                  rRequest.RideId = rid;
                  rRequest.RequestStatus = "2";
                  new RideRequestDAC().RideRequestStatusUpdate(rRequest);
                  Ride rtemp = new RideDAC().SelectById(rid);
                  rtemp.AvailableSeats -= 1;
                  new RideDAC().RideUpdate(rtemp);
                  //new RideDAC().RideStatusUpdate(rtemp);
                  Ride r = new RideDAC().SelectById(rid);
                  Users u = new UsersDAC().SelectById(r.UserId);
                  Users passenger = new UsersDAC().SelectById(uid);
                  string message = "" + u.UserName + " accepted your ride joining request.";
                  RideRequest rr = new RideRequest();
                  String res = SendNotificationFromFirebaseCloud(passenger.Token,u, rr, r, message, "PassengerAccepted");
                  return Ok();
              }
              else if (action == "RejectRequest")
              {
                  RideRequest rRequest = new RideRequest();
                  rRequest.PassengerId = uid;
                  rRequest.RideId = rid;
                  rRequest.RequestStatus = "3";
                  new RideRequestDAC().RideRequestStatusUpdate(rRequest);
                  Ride r = new RideDAC().SelectById(rid);
                  Users u = new UsersDAC().SelectById(r.UserId);
                  Users passenger = new UsersDAC().SelectById(uid);
                  string message = "" + u.UserName + " rejected your ride joining request.";
                  RideRequest rr = new RideRequest();
                  String res = SendNotificationFromFirebaseCloud(passenger.Token,u, rr, r, message, "PassengerRejected");
                  return Ok();
              }
              
              return null;
        }

        [HttpGet]
        public IHttpActionResult LeaveRide(int uid, int rid) 
        {
            RideRequest rr = new RideRequest();
            rr.PassengerId = uid;
            rr.RideId = rid;
            rr.RequestStatus = "2";
            new RideRequestDAC().DeleteRideRequest(rr);
            Ride rtemp = new RideDAC().SelectById(rid);
            rtemp.AvailableSeats += 1;
            new RideDAC().RideUpdate(rtemp);
            Users driver = new UsersDAC().SelectById(rtemp.UserId);
            Users passenger = new UsersDAC().SelectById(uid);
            String message = passenger.UserName+" left your ride from "+rtemp.Source+" to "+rtemp.Destination+" which you posted earlier! ";
            NotifyDriverPassengerLeftStatus(driver.Token,message,"PassengerLeft");
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult KickoutPassenger(int rid, int uid)
        {
            RideRequest rr = new RideRequest();
            rr.PassengerId = uid;
            rr.RideId = rid;
            rr.RequestStatus = "2";
            new RideRequestDAC().DeleteRideRequest(rr);
            Ride rtemp = new RideDAC().SelectById(rid);
            rtemp.AvailableSeats += 1;
            new RideDAC().RideUpdate(rtemp);
            Users driver = new UsersDAC().SelectById(rtemp.UserId);
            Users passenger = new UsersDAC().SelectById(uid);
            string message = driver.UserName + " removed you from ride " + rtemp.Source + " to " + rtemp.Destination + ".";
            NotifyPassengerRideKickoutStatus(passenger.Token,message,"DriverRemovedPassenger");
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult UserProfileRating(int uid) 
        {
            Users u = new UsersDAC().SelectById(uid);
            Rating r = new RatingDAC().SelectByUserId(uid);
            if (u != null)
            {
                UserProfileRatingModel uprm = new UserProfileRatingModel();
                uprm.Id = u.Id;
                uprm.Student_EmployeeId = u.Student_EmployeeId;
                uprm.UserName = u.UserName;
                uprm.Phone = u.Phone;
                uprm.PhoneStatus = u.PhoneStatus;
                if (r != null)
                {
                    uprm.OneStar = r.OneStar;
                    uprm.TwoStar = r.TwoStar;
                    uprm.ThreeStar = r.ThreeStar;
                    uprm.FourStar = r.FourStar;
                    uprm.FiveStar = r.FiveStar;
                }
                else
                {
                    uprm.OneStar = null;
                    uprm.TwoStar = null;
                    uprm.ThreeStar = null;
                    uprm.FourStar = null;
                    uprm.FiveStar = null;
                }
                
                return Ok(uprm);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult RideRequestExist(int uid, int rid) 
        {
            RideRequest rr = new RideRequestDAC().SelectRequestByRideIdAndPassengerId(uid,rid);
            if (rr != null)
            {
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult CancelRide(int rid) 
        {
            Ride r = new Ride();
            r.Id = rid;
            r.RideStatusId = 4;
            new RideDAC().RideStatusUpdate(r);
            Ride ride = new RideDAC().SelectById(rid);
            Users driver = new UsersDAC().SelectById(ride.UserId);
            RideRequest rr = new RideRequest();
            rr.RideId = rid;
            rr.RequestStatus = "2";
            List<RideRequest> rrlist = new RideRequestDAC().SelectALLAcceptedRequestByRideId(rr);
            if (rrlist != null)
            {
                foreach (RideRequest i in rrlist)
                {
                    Users u = new UsersDAC().SelectById(i.PassengerId);
                    String message = driver.UserName+" Cancelled the ride from "+ride.Source+" to "+ride.Destination+" which you joined! ";
                    NotifyCancelledRideStatus(u.Token,message,"JoinedPassengers");
                }
            }
            new RideRequestDAC().DeleteAllRideRequestsByRideId(rid);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult UpdateRide(RideModel ridemodel)
        {
            Ride r = new Ride();
            r.Id = ridemodel.Id;
            r.DepartureTime = ridemodel.DepartureTime;
            r.DepartureDate = ridemodel.DepartureDate;
            r.PostDate = ridemodel.PostDate;
            r.AvailableSeats = ridemodel.AvailableSeats;
            r.CostPerKm = ridemodel.CostPerKm;
            r.RideFrequency = ridemodel.RideFrequency;
            r.VehicleType = ridemodel.VehicleType;
            r.Smoking = ridemodel.Smoking;
            r.FoodDrinks = ridemodel.FoodDrinks;
            r.UserId = ridemodel.UserId;
            r.Source = ridemodel.Source;
            r.Destination = ridemodel.Destination;
            r.Checkpoints = ridemodel.Checkpoints;
            r.RideStatusId = 1;
            new RideDAC().RideUpdate(r);
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult RideStarted(int rid)
        {
            Ride r = new Ride();
            r.Id = rid;
            r.RideStatusId = 2;
            new RideDAC().RideStatusUpdate(r);
            Ride rtemp = new RideDAC().SelectById(rid);
            Users driver = new UsersDAC().SelectById(rtemp.UserId);
            RideRequest rr = new RideRequest();
            rr.RideId = rid;
            rr.RequestStatus = "2";
            List<RideRequest> rrlist = new RideRequestDAC().SelectALLAcceptedRequestByRideId(rr);
            if (rrlist != null)
            {
                foreach (RideRequest i in rrlist)
                {
                    Users passenger = new UsersDAC().SelectById(i.PassengerId);
                    String message = driver.UserName + " Started ride from " + rtemp.Source + " to " + rtemp.Destination + ".";
                    NotifyRideStartedStatusToJoinedPassengers(passenger.Token, driver.UserName, message, rtemp, "RideStarted");
                }
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult RideEnded(int rid)
        {
            Ride r = new Ride();
            r.Id = rid;
            r.RideStatusId = 3;
            new RideDAC().RideStatusUpdate(r);
            Ride rtemp = new RideDAC().SelectById(rid);
            Users driver = new UsersDAC().SelectById(rtemp.UserId);
            RideRequest rr = new RideRequest();
            rr.RideId = rid;
            rr.RequestStatus = "2";
            List<RideRequest> rrlist = new RideRequestDAC().SelectALLAcceptedRequestByRideId(rr);
            if (rrlist != null)
            {
                foreach (RideRequest i in rrlist)
                {
                    Users passenger = new UsersDAC().SelectById(i.PassengerId);
                    string message = "Rate your ride with "+driver.UserName + " from " + rtemp.Source + " to " + rtemp.Destination + ".";
                    NotifyRateDriverToJoinedPassengers(passenger.Token, driver,rtemp, message, "RateDriver");
                }
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SingleRideDetail(int rid)
        {
            Ride r = new RideDAC().SelectById(rid);
            String s = "";
            return Ok(r);
        }

        [HttpPost]
        public IHttpActionResult InsertRating(Rating r)
        {
            Rating rating = new RatingDAC().SelectByUserId(r.UserId);
            if (rating != null)
            {
                Rating rup = new Rating();
                rup.UserId = r.UserId;
                if (r.OneStar == 1)
                {
                    rup.OneStar = rating.OneStar + 1;
                }
                else
                {
                    rup.OneStar = rating.OneStar;
                }
                if (r.TwoStar == 1)
                {
                    rup.TwoStar = rating.TwoStar + 1;
                }
                else
                {
                    rup.TwoStar = rating.TwoStar;
                }
                if (r.ThreeStar == 1)
                {
                    rup.ThreeStar = rating.ThreeStar + 1;
                }
                else
                {
                    rup.ThreeStar = rating.ThreeStar;
                }
                if (r.FourStar == 1)
                {
                    rup.FourStar = rating.FourStar + 1;
                }
                else
                {
                    rup.FourStar = rating.FourStar;
                }
                if (r.FiveStar == 1)
                {
                    rup.FiveStar = rating.FiveStar + 1;
                }
                else
                {
                    rup.FiveStar = rating.FiveStar;
                }
                new RatingDAC().RatingUpdate(rup);
            }
            else
            {
                Rating rins = new Rating();
                rins.UserId = r.UserId;
                if (r.OneStar == 1)
                {
                    rins.OneStar = 1;
                }
                if (r.TwoStar == 1)
                {
                    rins.TwoStar = 2;
                }
                if (r.ThreeStar == 1)
                {
                    rins.ThreeStar = 3;
                }
                if (r.FourStar == 1)
                {
                    rins.FourStar = 4;
                }
                if (r.FiveStar == 1)
                {
                    rins.FiveStar = 5;
                }
                new RatingDAC().Insert(rins);
            }
            
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult SendTextMessageToPassenger(int pid, int rid,string message) 
        {
            Ride rtemp = new RideDAC().SelectById(rid);
            Users driver = new UsersDAC().SelectById(rtemp.UserId);
            Users passenger = new UsersDAC().SelectById(pid);
            NotifyTextMessageToPassenger(passenger.Token,driver.UserName,message,"TextMessageToPassenger");
            return Ok();
        }

        //Firebase Push Notification
        public String SendNotificationFromFirebaseCloud(String Token,Users u,RideRequest rr, Ride r, string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        requesterid = u.Id,
                        requestername = u.UserName,
                        pickuplocation = rr.PickupLocation,
                        destination = rr.Destination,
                        rideId = r.Id,
                        notifyTo = notifyto
                    }
                };  

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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

        //Firebase Push Notification to Notify Ride Cancelled Ride Status
        public String NotifyCancelledRideStatus(String Token, string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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

        //Firebase Push Notification to Notify Driver that passenger left his/her ride
        public String NotifyDriverPassengerLeftStatus(String Token, string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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
        
        //Firebase Push Notification to Notify Passenger, Driver's text message
        public String NotifyTextMessageToPassenger(String Token,string driverName ,string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        drivername = driverName,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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

        //Firebase Push Notification to Notify Passenger that driver removed him/her out of ride.
        public String NotifyPassengerRideKickoutStatus(String Token, string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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
        
        //Firebase Push Notification to Notify Passengers that driver started his/her ride.
        public String NotifyRideStartedStatusToJoinedPassengers(String Token,string drivername, string message,Ride r, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        RideId = r.Id,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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
        //Firebase Push Notification to Notify Passengers that driver started his/her ride.
        public String NotifyRateDriverToJoinedPassengers(String Token, Users driver,Ride ride, string message, string notifyto)
        {
            var result = "-1";
            try
            {
                var ServerKey = "AAAA6AJ165Q:APA91bGCozKA3rqrpTIQod3jXWMMYBvmGhVSGRNumvk3YDxTNP5JBBNe8OOO0_IMRFK2d_T8z8hdVOp4BMGPdD3fMTv03sPxBOHtwQgKRKFClpp8VyzJYECIt-cZoynVsQo3EUU46iNsxp8w90oe68E-Wei3yl2CZw";
                var senderId = "996473695124";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = Token,
                    data = new
                    {
                        message = message,
                        drivername = driver.UserName,
                        driverid = driver.Id,
                        source = ride.Source,
                        destination = ride.Destination,
                        notifyTo = notifyto
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ServerKey));
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
