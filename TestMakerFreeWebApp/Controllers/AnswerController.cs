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
    public class AnswerController : BaseApiController
    {
		#region Constructors
			public AnswerController(ApplicationDbContext context):base(context){}
		#endregion	

		#region RESTful conventions methods 
		/// <summary> 
		/// Retrieves the Answer with the given {id} 
		/// </summary> 
		/// &lt;param name="id">The ID of an existing Answer</param> 
		/// <returns>the Answer with the given {id}</returns> 
		[HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			var answer = DbContext.Answers.Where(w => w.Id == id).FirstOrDefault();

			if (answer == null) {
				return NotFound(new { Error = String.Format("Answer Id {0} not found", id )});
			}

			return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
        }


		/// <summary> 
		/// Adds a new Answer to the Database 
		/// </summary> 
		/// <param name="m">The AnswerViewModel containing the data to insert</param> 
		[HttpPut]
		public IActionResult Put([FromBody]AnswerViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var answer = model.Adapt<Answer>();

			answer.QuestionId = model.QuestionId;
			answer.Text = model.Text;
			answer.Notes = model.Notes;

			answer.CreatedDate = DateTime.Now;
			answer.LastModifiedDate = answer.CreatedDate;

			DbContext.Answers.Add(answer);
			DbContext.SaveChanges();

			return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);

        }

        /// <summary> 
        /// Edit the Answer with the given {id} 
        /// </summary> 
        /// <param name="m">The AnswerViewModel containing the data to update</param> 
        [HttpPost]
        public IActionResult Post([FromBody] AnswerViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var answer = DbContext.Answers.Where(w => w.Id == model.Id).FirstOrDefault();

			if (answer == null) {
				return NotFound(new { Error = String.Format("Answer ID {0} has not been found", model.Id) });
			}

			answer.QuestionId = model.QuestionId;
			answer.Text = model.Text;
			answer.Value = model.Value;
			answer.Notes = model.Notes;

			answer.LastModifiedDate = answer.CreatedDate;

			DbContext.SaveChanges();

			return new JsonResult(answer.Adapt<AnswerViewModel>(), JsonSettings);
		}

        /// <summary> 
        /// Deletes the Answer with the given {id} from the Database 
        /// </summary> 
        /// <param name="id">The ID of an existing Answer</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
			var answer = DbContext.Answers.Where(w => w.Id == id).FirstOrDefault();

			if (answer == null)
			{return NotFound(new { Error = String.Format("Answer ID {0} has not been found", id) });}

			DbContext.Answers.Remove(answer);
			DbContext.SaveChanges();

			return new OkResult();
		}
        #endregion


        [HttpGet("All/{questionId}")]
        public IActionResult All(int questionId)
        {

			var answers = DbContext.Answers.Where(w => w.QuestionId == questionId).ToArray();

            // output the result in JSON format 
            return new JsonResult(answers.Adapt<AnswerViewModel[]>(),JsonSettings);

			//var sampleAnswers = new List<AnswerViewModel>();

			//// add a first sample answer 
			//sampleAnswers.Add(new AnswerViewModel()
			//{
			//    Id = 1,
			//    QuestionId = questionId,
			//    Text = "Friends and family",
			//    CreatedDate = DateTime.Now,
			//    LastModifiedDate = DateTime.Now
			//});

			//// add a bunch of other sample answers 
			//for (int i = 2; i <= 5; i++)
			//{
			//    sampleAnswers.Add(new AnswerViewModel()
			//    {
			//        Id = i,
			//        QuestionId = questionId,
			//        Text = String.Format("Sample Answer {0}", i),
			//        CreatedDate = DateTime.Now,
			//        LastModifiedDate = DateTime.Now
			//    });
			//}

		}

	}
}
