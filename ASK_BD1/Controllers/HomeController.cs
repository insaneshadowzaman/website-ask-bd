using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASK_BD1.Models;

namespace ASK_BD1.Controllers
{
    public class HomeController : Controller
    {
        // ReSharper disable once InconsistentNaming
        private readonly DatabaseContext dbContext = new DatabaseContext();

        public ActionResult Index()
        {
            if (Session["user_email"] != null) return RedirectToAction("SignedIn");
            HttpCookie cookie = Request.Cookies["login_state"];
            if (cookie?["user_email"] == null)
            {
                var pair = new Tuple<IEnumerable<string>, IEnumerable<string>>(
                        dbContext.Districts.Select(d => d.division).Distinct().ToList(),
                        dbContext.Districts.Select(d => d.district1).Distinct().ToList()
                    );
                return View(pair);
            }
            Session["user_email"] = cookie["user_email"];
            return RedirectToAction("SignedIn");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Questions(string division)
        {
            var questions = 
                division != null ? 
                    dbContext.Questions.Where(q => q.division == division).ToList() : 
                    dbContext.Questions.ToList();
            ViewBag.division = division;
            return View(questions);
        }

        public ActionResult Logout()
        {
            Session["user_email"] = null;
            HttpCookie mycookie = new HttpCookie("login_state") { Expires = DateTime.Now.AddDays(-1d) };
            Response.Cookies.Add(mycookie);
            return RedirectToAction("Index");
        }

        public ActionResult Admin()
        {
            var users = dbContext.Users.ToList();
            return View(users);
        }

        public ActionResult SignedIn(string input_email = null, string input_pass = null, string input_remember = null)
        {
            if (input_email == "admin@admin")
            {
                if (input_pass == "abcd")
                {
                    return RedirectToAction("Admin");
                }
            }
            string userEmail = Session["user_email"]?.ToString();
                
            if (input_email == null && userEmail == null) return RedirectToAction("Index");

            if (input_email != null)
            {
                if(!dbContext.Users.Any(u => u.email == input_email && u.password == input_pass))
                {
                    // TODO : Handle error
                    return RedirectToAction("Index");
                }
                Session["user_email"] = dbContext.Users.Single(u => u.email == input_email && u.password == input_pass).email;
                userEmail = Session["user_email"].ToString();
            }

            var questions = dbContext.Questions.Where(q => q.uploader == userEmail).ToList();
            var answers = dbContext.Answers.Where(a => a.uploader == userEmail).ToList();
            Tuple<List<Question>, List<Answer>> pair = new Tuple<List<Question>, List<Answer>>(questions, answers);

            if (input_remember == null) return View(pair);

            HttpCookie mycookie = new HttpCookie("login_state")
            {
                ["user_email"] = input_email,
                Expires = DateTime.Now.AddDays(1d)
            };
            Response.Cookies.Add(mycookie);
            return View(pair);
        }

        public ActionResult AddQuestion(string input_question, string input_description, string input_division = null, string input_district = null)
        {
            Question ques = new Question()
            {
                title = input_question,
                description = input_description,
                uploader = Session["user_email"].ToString(),
                upload_date = DateTime.Now,
                division = input_division,
                district = input_district
            };
            dbContext.Questions.Add(ques);
            dbContext.SaveChanges();
            return RedirectToAction("SignedIn");
        }

        public ActionResult SignUp(string input_fname, string input_lname, string input_email, string input_pass, string input_repass)
        {
            if (input_pass != input_repass)
            {
                // TODO: Handle error
                return RedirectToAction("Index");
            }
            bool ifContainUser = dbContext.Users.Any(u => u.email == input_email && u.password == input_pass);
            if (ifContainUser)
            {
                // TODO: Handle error
                return RedirectToAction("Index");
            }

            User newUser = new User()
            {
                email = input_email,
                password = input_pass,
                name = input_fname
            };
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            Session["user_email"] = input_email;
            return RedirectToAction("SignedIn");
        }

        public ActionResult QuestionPage(int questionId = 1)
        {
            return View(dbContext.Questions.First(q => q.Id == questionId));
        }

        public ActionResult AnswerQuestion(string input_answer, int questionId)
        {
            dbContext.Answers.Add(new Answer()
            {
                answer1 = input_answer,
                ques_id = questionId,
                upload_date = DateTime.Now,
                uploader = Session["user_email"].ToString()
            });
            dbContext.SaveChanges();
            return RedirectToAction("QuestionPage","Home", new { questionId = questionId.ToString()});
        }

        public void Like(int questionId = 1, int answerId = 1, int like_or_dis = (int) Enums.VOTE_STATES.NONE)
        {
            var vote = new Vote()
            {
                email = Session["user_email"].ToString(),
                ques_id = questionId,
                ans_id = answerId
            };
            var currentVote = dbContext.Votes
                .FirstOrDefault(v => v.email == vote.email && v.ques_id == questionId && v.ans_id == answerId);

            if (currentVote != null)
            {
                var currentState = currentVote.up_down;
                if (like_or_dis == (int) Enums.VOTE_STATES.UP)
                {
                    if (currentState == (int) Enums.VOTE_STATES.UP)
                        vote.up_down = (int) Enums.VOTE_STATES.NONE;
                    else
                        vote.up_down = (int) Enums.VOTE_STATES.UP;
                }
                else
                {
                    if (currentState == (int) Enums.VOTE_STATES.DOWN)
                        vote.up_down = (int) Enums.VOTE_STATES.NONE;
                    else
                        vote.up_down = (int) Enums.VOTE_STATES.DOWN;
                }
                dbContext.Votes.Remove(currentVote);
            }
            else
            {
                if (like_or_dis == (int) Enums.VOTE_STATES.UP)
                    vote.up_down = (int) Enums.VOTE_STATES.UP;
                else
                    vote.up_down = (int) Enums.VOTE_STATES.DOWN;
            }
            if(vote.up_down != (int)Enums.VOTE_STATES.NONE)
                dbContext.Votes.Add(vote);
            dbContext.SaveChanges();
        }

        public JsonResult GetDistricts(string division)
        {
            var districts = dbContext.Districts.Where(d => d.division == division).Select(d => d.district1).ToArray();
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
    }
}