using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.ViewModels;


using Mapster;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : Controller
    {
		#region Private Fields
		private ApplicationDbContext dbContext;
		#endregion

		#region Constructor
		public QuizController(ApplicationDbContext context)
		{
			dbContext = context;
		}
		#endregion


		#region RESTful conventions methods 
		/// <summary> 
		/// GET: api/quiz/{}id 
		/// Retrieves the Quiz with the given {id} 
		/// </summary> 
		/// <param name="id">The ID of an existing Quiz</param> 
		/// <returns>the Quiz with the given {id}</returns> 
		[HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			//// create a sample quiz to match the given request 
			//var v = new QuizViewModel()
			//{
			//    Id = id,
			//    Title = String.Format("Sample quiz with id {0}", id),
			//    Description = "Not a real quiz: it's just a sample!",
			//    CreatedDate = DateTime.Now,
			//    LastModifiedDate = DateTime.Now
			//};

			var quiz = dbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();

			if (quiz == null) {
				return NotFound(new
				{
					Error = String.Format("Quiz Id {0} has not been found",id)
				});
			}

            // output the result in JSON format 
            return new JsonResult(
				quiz.Adapt<QuizViewModel>(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                });
        }

        /// <summary> 
        /// Adds a new Quiz to the Database 
        /// </summary> 
        /// <param name="m">The QuizViewModel containing the data to insert</param> 
        [HttpPut]
        public IActionResult Put([FromBody]QuizViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var quiz = new Quiz
			{
				Title = model.Title,
				Description = model.Description,
				Text = model.Text,
				Notes = model.Notes,
				CreatedDate = DateTime.Now,
				LastModifiedDate= DateTime.Now 
			};

			quiz.UserId = dbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

			dbContext.Quizzes.Add(quiz);
			dbContext.SaveChanges();

            return new JsonResult(quiz.Adapt<QuizViewModel>(),new JsonSerializerSettings(){ Formatting=  Formatting.Indented});
        }

        /// <summary> 
        /// Edit the Quiz with the given {id} 
        /// </summary> 
        /// <param name="m">The QuizViewModel containing the data to update</param> 
        [HttpPost]
        public IActionResult Post([FromBody]QuizViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var quiz = dbContext.Quizzes.Where(w => w.Id == model.Id).FirstOrDefault();

			if (quiz == null) {
				return NotFound(new
				{
					Error = String.Format("Quiz Id {0} has not been found", model.Id)
				});
			}

			quiz.Title = model.Title;
			quiz.Description = model.Description;
			quiz.Text = model.Text;
			quiz.Notes = model.Notes;
			quiz.LastModifiedDate = DateTime.Now;

			dbContext.SaveChanges();

			return new JsonResult(quiz.Adapt<QuizViewModel>(),
			new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented
			});
		}

        /// <summary> 
        /// Deletes the Quiz with the given {id} from the Database 
        /// </summary> 
        /// <param name="id">The ID of an existing Quiz</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
			var quiz = dbContext.Quizzes.Where(w => w.Id == id).FirstOrDefault();

			if (quiz == null)
			{
				return NotFound(new
				{
					Error = String.Format("Quiz Id {0} has not been found", id)
				});
			}

			dbContext.Quizzes.Remove(quiz);
			dbContext.SaveChanges();

			return new OkResult();

		}
        #endregion

        #region Attribute-based routing methods 
        /// <summary> 
        /// GET: api/quiz/latest 
        /// Retrieves the {num} latest Quizzes 
        /// </summary> 
        /// <param name="num">the number of quizzes to retrieve</param> 
        /// <returns>the {num} latest Quizzes</returns> 
        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10) {
			//var sampleQuizzes = new List<QuizViewModel>();

			//sampleQuizzes.Add(new QuizViewModel()
			//{
			//    Id = 1,
			//    Title = "Which Shingeki No Kyojin character are you?",
			//    Description = "Anime-related personality test",
			//    CreatedDate = DateTime.Now,
			//    LastModifiedDate = DateTime.Now
			//});

			//for (int i = 2; i <= num; i++)
			//{
			//    sampleQuizzes.Add(new QuizViewModel()
			//    {
			//        Id = i,
			//        Title = String.Format("Sample Quiz {0}", i),
			//        Description = "This is a sample quiz",
			//        CreatedDate = DateTime.Now,
			//        LastModifiedDate = DateTime.Now
			//    });
			//}

			var latest = dbContext.Quizzes.OrderByDescending(q => q.CreatedDate).Take(num).ToArray();
            return new JsonResult(latest.Adapt<QuizViewModel[]>(), new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Formatting.Indented });

        }

        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10) {

			//var sampleQuizzes = ((JsonResult)Latest(num)).Value as List<QuizViewModel>;

			var byTitle = dbContext.Quizzes.OrderBy(o => o.Title).Take(num).ToArray();

            return new JsonResult(byTitle.Adapt<QuizViewModel[]>(), new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Formatting.Indented });

        }

        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {

			//var sampleQuizzes = ((JsonResult)Latest(num)).Value as List<QuizViewModel>;

			var random = dbContext.Quizzes.OrderBy(o => Guid.NewGuid()).Take(num).ToArray();

            return new JsonResult(random.Adapt<QuizViewModel[]>(), new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Formatting.Indented });

        }
        #endregion

    }
}
