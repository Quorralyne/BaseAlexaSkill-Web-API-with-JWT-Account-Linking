using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web.Http;
using BaseAlexaSkill.Models;

namespace BaseAlexaSkill.Controllers
{
    [RoutePrefix("api/alexa")]
    public class AlexaController : ApiController
    {
        [HttpPost, Route("mycompanyskill")]
        public AlexaResponse MyCompanySkill(AlexaRequest request)
        {
            AlexaResponse response = null;

            switch (request.Request.Intent.Name)
            {
                case "HelloIntent":
                    response = HelloIntentHandler();
                    break;
                case "FavoriteBandIntent":
                    response = FavoriteBandIntentHandler(request);
                    break;
            }

            return response;
        }

        private AlexaResponse FavoriteBandIntentHandler(AlexaRequest request)
        {
            var jwtEncodedString = request.Session.User.AccessToken;

            if (request.Session.User.AccessToken != null)
            {
                //Decode name and favorite band from the access token
                var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                var bandName = token.Claims.First(c => c.Type == "favoriteBand").Value;
                var name = token.Claims.First(c => c.Type == "firstName").Value;

                var response = new AlexaResponse(
                    "Hello " + name + ", your favorite band is " + bandName + ".");
                response.Response.Card.Title = name + "'s Favorite Band";
                response.Response.Card.Content = bandName;
                response.Response.ShouldEndSession = true;

                return response;
            }
            else
            {
                var response = new AlexaResponse("You are not currently linked to this skill. Please go into your Alexa app and sign in.");
                response.Response.Card.Type = "LinkAccount";
                response.Response.ShouldEndSession = true;

                return response;
            }
        }

        private AlexaResponse HelloIntentHandler()
        {
            var response = new AlexaResponse("Hello from My Company.");
            response.Response.Reprompt.OutputSpeech.Text =
                "You can tell me to say hello, what is my favorite band, or cancel to exit.";
            response.Response.ShouldEndSession = false;
            return response;
        }

        [HttpPost, Route("demo")]
        public dynamic AlexaDemo(dynamic request)
        {
            return new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = "Hello there and welcome."
                    },
                    card = new
                    {
                        type = "Simple",
                        title = "Hello VS Live",
                        content = "Hello VS Live!"
                    },
                    shouldEndSession = true
                }
            };
        }

        [HttpPost, Route("ssmldemo")]
        public dynamic AlexaSSMLDemo(dynamic request)
        {
            return new
            {
                version = "1.0",
                response = new
                {
                    shouldEndSession = true,
                    outputSpeech = new
                    {
                        type = "SSML",
                        ssml = 
                        "<speak> Hello, V.S. " +
                        "<phoneme alphabet='ipa' ph='laɪv'>Live</phoneme>!" +
                        "</speak>"
                    }
                }
            };
        }
    }
}
