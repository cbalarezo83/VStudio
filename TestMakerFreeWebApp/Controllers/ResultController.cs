using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestMakerFreeWebApp.ViewModels;
using Newtonsoft.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using TestMakerFreeWebApp.Data.Models;
using TestMakerFreeWebApp.Data;
using Mapster;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace TestMakerFreeWebApp.Controllers
{
    public class ResultController : BaseApiController
    {
		#region Constructor
		public ResultController(ApplicationDbContext context,
                                        RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IConfiguration configuration) : base(context, roleManager, userManager, configuration) { }
        #endregion
        #region RESTful conventions methods 
        /// <summary> 
        /// Retrieves the Result with the given {id} 
        /// </summary> 
        /// &lt;param name="id">The ID of an existing Result</param> 
        /// <returns>the Result with the given {id}</returns> 
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
			var result = DbContext.Results.Where(w => w.Id == id).FirstOrDefault();

			if (result == null) {
				return NotFound(new { Error = String.Format("Result ID {0} has not been found", id)});
			}
			return new JsonResult(result.Adapt<ResultViewModel>(),JsonSettings);
        }

        /// <summary> 
        /// Adds a new Result to the Database 
        /// </summary> 
        /// <param name="m">The ResultViewModel containing the data to insert</param> 
        [HttpPut]
        public IActionResult Put([FromBody]ResultViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var result = model.Adapt<Result>();

			// override those properties 
			// that should be set from the server-side only
			result.CreatedDate = DateTime.Now;
			result.LastModifiedDate = result.CreatedDate;

			DbContext.Results.Add(result);
			DbContext.SaveChanges();

			return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
        }

        /// <summary> 
        /// Edit the Result with the given {id} 
        /// </summary> 
        /// <param name="m">The ResultViewModel containing the data to update</param> 
        [HttpPost]
        public IActionResult Post([FromBody] ResultViewModel model)
        {
			if (model == null) return new StatusCodeResult(500);

			var result = DbContext.Results.Where(w => w.Id == model.Id).FirstOrDefault();

			if (result == null) {
				return NotFound(new { Error = String.Format("Result ID{0} has not been found", model.Id) });
			}


			result.QuizId = model.QuizId;
			result.Text = model.Text;
			result.MinValue = model.MinValue;
			result.MaxValue = model.MaxValue;
			result.Notes = model.Notes;

			DbContext.SaveChanges();

			return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
		}

        /// <summary> 
        /// Deletes the Result with the given {id} from the Database 
        /// </summary> 
        /// <param name="id">The ID of an existing Result</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
			var result = DbContext.Results.Where(w => w.Id == id).FirstOrDefault();

			if (result == null)
			{
				return NotFound(new { Error = String.Format("Result ID{0} has not been found", id) });
			}

			DbContext.Results.Remove(result);
			DbContext.SaveChanges();

			return new OkResult();
		}
        #endregion


        // GET api/question/all 
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {

			var results = DbContext.Results.Where(w => w.QuizId == quizId).ToArray();

			// output the result in JSON format 
			return new JsonResult(results.Adapt<ResultViewModel[]>(), JsonSettings);


			//var sampleResults = new List<ResultViewModel>();

			//add a first sample result

			//sampleResults.Add(new ResultViewModel()
			//{
			//	Id = 1,
			//	QuizId = quizId,
			//	Text = "What do you value most in your life?",
			//	CreatedDate = DateTime.Now,
			//	LastModifiedDate = DateTime.Now
			//});

			//add a bunch of other sample results

			//for (int i = 2; i <= 5; i++)
			//{
			//	sampleResults.Add(new ResultViewModel()
			//	{
			//		Id = i,
			//		QuizId = quizId,
			//		Text = String.Format("Sample Question {0}", i),
			//		CreatedDate = DateTime.Now,
			//		LastModifiedDate = DateTime.Now
			//	});
			//}
		}
	}
}
