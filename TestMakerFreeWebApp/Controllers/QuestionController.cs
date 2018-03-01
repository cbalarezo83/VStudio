using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using TestMakerFreeWebApp.Data;
using TestMakerFreeWebApp.Data.Models;
using Mapster;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestMakerFreeWebApp.Controllers
{
    public class QuestionController : BaseApiController
    {
		#region Constructor
			public QuestionController(ApplicationDbContext context) : base(context) { }
		#endregion

		#region RESTful conventions methods 
		/// <summary> 
		/// Retrieves the Question with the given {id} 
		/// </summary> 
		/// &lt;param name="id">The ID of an existing Question</param> 
		/// <returns>the Question with the given {id}</returns> 
		[HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			var question = DbContext.Questions.Where(i => i.Id == id).FirstOrDefault();

			if (question == null) {
				return NotFound(new {Error = String.Format("Question ID {0} has not been found", id)});
			}

			return new JsonResult(question.Adapt<QuestionViewModel>(),JsonSettings);
        }

		/// <summary> 
		/// Adds a new Question to the Database 
		/// </summary> 
		/// <param name="m">The QuestionViewModel containing the data to insert</param> 
		[HttpPut]
		public IActionResult Put([FromBody]QuestionViewModel model)
		{
			if (model == null) return new StatusCodeResult(500);

			var question = model.Adapt<Question>();

			question.QuizId = model.QuizId;
			question.Text = model.Text;
			question.Notes = model.Notes;

			question.CreatedDate = DateTime.Now;
			question.LastModifiedDate = question.CreatedDate;

			DbContext.Questions.Add(question);
			DbContext.SaveChanges();

			return new JsonResult(question.Adapt<QuestionViewModel>(), JsonSettings);

        }

        /// <summary> 
        /// Edit the Question with the given {id} 
        /// </summary> 
        /// <param name="m">The QuestionViewModel containing the data to update</param> 
        [HttpPost]
        public IActionResult Post([FromBody]QuestionViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var question = DbContext.Questions.Where(w => w.Id == model.Id).FirstOrDefault();

			if (question == null) {
				return NotFound(new{Error = String.Format("Question ID {0} has not been found", model.Id)});
			}

			question.QuizId = model.QuizId;
			question.Text = model.Text;
			question.Notes = model.Notes;

			// properties set from server-side
			question.LastModifiedDate = question.CreatedDate;

			DbContext.SaveChanges();

			return new JsonResult(question.Adapt<QuestionViewModel>(), JsonSettings);
        }

        /// <summary> 
        /// Deletes the Question with the given {id} from the Database 
        /// </summary> 
        /// <param name="id">The ID of an existing Question</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
			var question = DbContext.Questions.Where(w => w.Id == id).FirstOrDefault();

			if (question == null) {
				return NotFound(new { Error = String.Format("Question ID {0} has not been found", id) });
			}

			DbContext.Questions.Remove(question);
			DbContext.SaveChanges();

			return new OkResult();

        }
        #endregion


        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId) {

			var questions = DbContext.Questions.Where(q => q.QuizId == quizId).ToArray();

            // output the result in JSON format 
            return new JsonResult(
                questions.Adapt<QuestionViewModel[]>(),JsonSettings);


			//var sampleQuestions = new List<QuestionViewModel>();

			//// add a first sample question 
			//sampleQuestions.Add(new QuestionViewModel()
			//{
			//    Id = 1,
			//    QuizId = quizId,
			//    Text = "What do you value most in your life?",
			//    CreatedDate = DateTime.Now,
			//    LastModifiedDate = DateTime.Now
			//});

			//// add a bunch of other sample questions 
			//for (int i = 2; i <= 5; i++)
			//{
			//    sampleQuestions.Add(new QuestionViewModel()
			//    {
			//        Id = i,
			//        QuizId = quizId,
			//        Text = String.Format("Sample Question {0}", i),
			//        CreatedDate = DateTime.Now,
			//        LastModifiedDate = DateTime.Now
			//    });
			//}
		}
	}
}
