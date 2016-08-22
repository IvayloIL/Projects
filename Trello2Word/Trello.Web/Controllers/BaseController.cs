using System;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Authenticators;

namespace Trello.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Uri baseUrl;
        protected RestClient restClient;

        protected BaseController()
        {
            this.BaseUrl = new Uri("https://trello.com/1/");
            this.RestClient = new RestClient(BaseUrl)
            {
                Authenticator = OAuth1Authenticator.ForRequestToken(Startup.ConsumerKey, Startup.ConsumerSecret),
            };
            
            
        }

        

        public Uri BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        public RestClient RestClient
        {
            get { return restClient; }
            set { restClient = value; }
        }
    }
}