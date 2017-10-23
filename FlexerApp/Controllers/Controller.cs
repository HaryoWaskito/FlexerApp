using FlexerApp.Contexts;
using FlexerApp.Models;
using RestSharp;
using System;
using System.Linq;
using System.Text;

namespace FlexerApp.Controllers
{
    public class Controller
    {
        private const string API_URL = "http://52.163.112.40:2345";
        private const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The context database
        /// </summary>
        private Context _contextDB = new Context();

        /// <summary>
        /// Logins to server.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        public bool LoginToServer(LoginModel login)
        {
            bool isSuccessLogin = false;
            try
            {
                var requestBody = new StringBuilder();
                var client = new RestClient(string.Format("{0}/login", API_URL));
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                requestBody.AppendFormat("\"Email\" : \"{0}\",", login.Email);
                requestBody.AppendFormat("\"Password\" : \"{0}\",", login.Password);
                requestBody.AppendFormat("\"LocationType\" : \"{0}\",", login.LocationType);
                requestBody.AppendFormat("\"IPAddress\" : \"{0}\",", login.IPAddress);
                requestBody.AppendFormat("\"City\" : \"{0}\",", login.City);
                requestBody.AppendFormat("\"Lat\" : {0},", login.Lat);
                requestBody.AppendFormat("\"Long\" : {0},", login.Long);
                requestBody.AppendFormat("\"gmtdiff\" : {0},", login.GMTDiff);
                requestBody.AppendFormat("\"clienttime\" : \"{0}\"", login.LoginDate.ToString("yyyy-MM-dd HH:mm:ss"));

                request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                isSuccessLogin = response.StatusCode.ToString() == "OK" ? true : false;

                if (isSuccessLogin)
                {
                    var responseList = response.Content.Split(',').ToList();
                    login.sessionID = Convert.ToInt32(responseList[1].Split(':')[1]);
                    login.loginToken = responseList[3].Split(':')[1].Substring(1, responseList[3].Split(':')[1].Length - 3);

                    _contextDB.CreateLoginSession(login);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccessLogin;
        }

        /// <summary>
        /// Logouts from server.
        /// </summary>
        /// <param name="sessionID">The session identifier.</param>
        /// <returns></returns>
        public bool LogoutFromServer(long sessionID)
        {
            bool isSuccessLogout = false;
            try
            {
                var requestBody = new StringBuilder();
                var client = new RestClient(string.Format("{0}/logout", API_URL));
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");

                requestBody.AppendFormat("\"SessionID\" : \"{0}\",", sessionID);

                request.AddParameter("application/json", string.Concat("{", requestBody.ToString(), "}"), ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                isSuccessLogout = response.StatusCode.ToString() == "OK" ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccessLogout;
        }
    }
}
