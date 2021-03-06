﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;


using TestMakerFreeWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace TestMakerFreeWebApp.Data
{
	public class DbSeeder
	{
		#region Public Methods
		public static void Seed(ApplicationDbContext dbContext,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager)
		{

            if (!dbContext.Users.Any())
            {
                CreateUsers(dbContext, roleManager, userManager).GetAwaiter().GetResult();
            }

			if (!dbContext.Quizzes.Any()) CreateQuizzes(dbContext);
		}
		#endregion

		#region Seed Methods

		private static async Task CreateUsers(ApplicationDbContext dbContext,
                                                RoleManager<IdentityRole> roleManager,
                                                UserManager<ApplicationUser> userManager)
		{
			DateTime createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
			DateTime lastModifiedDate = DateTime.Now;


            string role_Administrator = "Administrator";
            string role_RegisteredUser = "RegisteredUser";

            if (!await roleManager.RoleExistsAsync(role_Administrator)) {
                await roleManager.CreateAsync(new IdentityRole(role_Administrator));
            }

            if (!await roleManager.RoleExistsAsync(role_RegisteredUser))
            {
                await roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));
            }


            var user_Admin = new ApplicationUser()
			{
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = "Admin",
				Email = "admin@testmakerfree.com",
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			};

            if (await userManager.FindByNameAsync(user_Admin.UserName) == null) {

                await userManager.CreateAsync(user_Admin, "Pass4Admin");
                await userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await userManager.AddToRoleAsync(user_Admin, role_Administrator);

                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;
            }
			//dbContext.Users.AddRange(user_Admin);

#if DEBUG

			var user_Ryan = new ApplicationUser()
			{
                SecurityStamp = Guid.NewGuid().ToString(),
				UserName = "Ryan",
				Email = "ryan@testmakerfree.com",
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			};

			var user_Solice = new ApplicationUser()
			{
                SecurityStamp = Guid.NewGuid().ToString(),
				UserName = "Solice",
				Email = "solice@testmakerfree.com",
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			};

			var user_Vodan = new ApplicationUser()
			{
                SecurityStamp = Guid.NewGuid().ToString(),
				UserName = "Vodan",
				Email = "vodan@testmakerfree.com",
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			};

            if (await userManager.FindByNameAsync(user_Ryan.UserName) == null)
            {

                await userManager.CreateAsync(user_Ryan, "Pass4Ryan");
                await userManager.AddToRoleAsync(user_Ryan, role_RegisteredUser);

                user_Ryan.EmailConfirmed = true;
                user_Ryan.LockoutEnabled = false;
            }

            if (await userManager.FindByNameAsync(user_Solice.UserName) == null)
            {

                await userManager.CreateAsync(user_Solice, "Pass4Solice");
                await userManager.AddToRoleAsync(user_Solice, role_RegisteredUser);

                user_Solice.EmailConfirmed = true;
                user_Solice.LockoutEnabled = false;
            }


            if (await userManager.FindByNameAsync( user_Vodan.UserName) == null)
            {

                await userManager.CreateAsync(user_Vodan, "Pass4Vodan");
                await userManager.AddToRoleAsync(user_Vodan, role_RegisteredUser);

                user_Vodan.EmailConfirmed = true;
                user_Vodan.LockoutEnabled = false;
            }

            // Insert sample registered users into the Database
            //dbContext.Users.AddRange(user_Ryan, user_Solice, user_Vodan);


#endif

            //dbContext.SaveChanges();

            await dbContext.SaveChangesAsync();
        }

		private static void CreateQuizzes(ApplicationDbContext dbContext)
		{

			DateTime createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
			DateTime lastModifiedDate = DateTime.Now;

			var authorId = dbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

#if DEBUG
			var num = 47;

			for (int i = 1; i <= num; i++)
			{
				CreateSampleQuiz(dbContext, i, authorId, num - i, 3, 3, 3, createdDate.AddDays(-num));
			}
#endif

			EntityEntry<Quiz> e1 = dbContext.Quizzes.Add(new Quiz()
			{
				UserId = authorId,
				Title = "Are you more Light or Dark side of the Force?",
				Description = "Star Wars personality test",
				Text = @"Choose wisely you must, young padawan: " +
				"this test will prove if your will is strong enough " +
				"to adhere to the principles of the light side of the Force " +
				"or if you're fated to embrace the dark side. " +
				"No you want to become a true JEDI, you can't possibly  miss this!",

				ViewCount = 2343,
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			});

			EntityEntry<Quiz> e2 = dbContext.Quizzes.Add(new Quiz()
			{
				UserId = authorId,
				Title = "GenX, GenY or Genz?",
				Description = "Find out what decade most represents you",
				Text = @"Do you feel confortable in your generation? " +
				"What year should you have been born in?" +
				"Here's a bunch of questions that will help you to find out!",
				ViewCount = 4180,
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			});

			EntityEntry<Quiz> e3 = dbContext.Quizzes.Add(new Quiz()
			{
				UserId = authorId,
				Title = "Which Shingeki No Kyojin character are you?",
				Description = "Attack On Titan personality test",
				Text = @"Do you relentlessly seek revenge like Eren? " +
						"Are you willing to put your like on the stake to protect your friends like Mikasa ? " +
						"Would you trust your fighting skills like Levi " +
						"or rely on your strategies and tactics like Arwin? " +
						"Unveil your true self with this Attack On Titan personality test!",
				ViewCount = 5203,
				CreatedDate = createdDate,
				LastModifiedDate = lastModifiedDate
			});

			// persist the changes on the Database
			dbContext.SaveChanges();

		}


		#endregion

		#region Utility Methods
		/// <summary>
		/// Creates a sample quiz and add it to the Database
		/// together with a sample set of questions, answers & results.
		/// </summary>
		/// <param name="userId">the author ID</param>
		/// <param name="id">the quiz ID</param>
		/// <param name="createdDate">the quiz CreatedDate</param>
		private static void CreateSampleQuiz(ApplicationDbContext dbContext,
											int num,
											string authorId,
											int viewCount,
											int numberOfQuestions,
											int numberOfAnswersPerQuestion,
											int numberOfResults,
											DateTime createdDate)
		{

			var quiz = new Quiz()
			{
				UserId = authorId,
				Title = String.Format("Quiz {0} Title", num),
				Description = String.Format("This is a sample description for quiz {0}.", num),
				Text = "This is a sample quiz created by the DbSeeder class for testing purposes. " +
						"All the questions, answers & results are auto-generated as well.",
				ViewCount = viewCount,
				CreatedDate = createdDate,
				LastModifiedDate = createdDate
			};


			dbContext.Quizzes.Add(quiz);
			dbContext.SaveChanges();

			for (int i = 0; i < numberOfQuestions; i++)
			{
				var question = new Question()
				{
					QuizId = quiz.Id,
					Text = "This is a sample question created by the DbSeeder class for testing purposes. " +
							"All the child answers are auto-generated as well.",
					CreatedDate = createdDate,
					LastModifiedDate = createdDate

				};
				dbContext.Questions.Add(question);
				dbContext.SaveChanges();

				for (int i2 = 0; i2 < numberOfAnswersPerQuestion; i2++)
				{
					var e2 = dbContext.Answers.Add(new Answer()
					{
						QuestionId = question.Id,
						Text = "This is a sample answer created by the DbSeeder class for testing purposes. ",
						Value = i2,
						CreatedDate = createdDate,
						LastModifiedDate = createdDate

					});
				}
			}

			for (int i = 0; i < numberOfResults; i++)
			{
				dbContext.Results.Add(new Result()
				{
					QuizId = quiz.Id,
					Text = "This is a sample result created by the DbSeeder class for testing purposes. ",
					MinValue = 0,// max value should be equal to answers number * max answer value
					MaxValue = numberOfAnswersPerQuestion * 2,
					CreatedDate = createdDate,
					LastModifiedDate = createdDate

				});
			}
			dbContext.SaveChanges();

		}
		#endregion
	}
}
