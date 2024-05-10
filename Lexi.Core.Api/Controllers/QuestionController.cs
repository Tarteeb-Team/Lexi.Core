using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexi.Core.Api.Brokers.UpdateStorages;
using Lexi.Core.Api.Models.Foundations.Questions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace Lexi.Core.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class QuestionController : RESTFulController
	{
		private readonly IUpdateStorageBroker updateStorageBroker;

		public QuestionController(IUpdateStorageBroker storageBroker) =>
			this.updateStorageBroker = storageBroker;

		[HttpPost]
		public async ValueTask<ActionResult<List<Question>>> AddQuestions()
		{
			var questions = new List<Question>
			{
				 new Question { Id = Guid.NewGuid(), Content = "What is the weather like today?", Number = 1, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "Do you pay attention to the weather forecast?", Number = 2, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "What is your favorite type of weather?", Number = 3, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "How does the weather affect your daily activities?", Number = 4, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "Do you prefer hot or cold weather?", Number = 5, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "What outdoor activities do you enjoy doing in good weather?", Number = 6, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "How do people in your country typically dress during different seasons?", Number = 7, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "Have you ever experienced extreme weather conditions?", Number = 8, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "How do you feel when it's raining outside?", Number = 9, QuestionType = "Weather ☀️" },
				 new Question { Id = Guid.NewGuid(), Content = "Do you think climate change is affecting the weather patterns in your region?", Number = 10, QuestionType = "Weather ☀️" }
			};

			foreach (var question in questions)
			{
				await this.updateStorageBroker.InsertQuestionAsync(question);
			}

			return Ok(questions);
		}

		[HttpGet]
		public IQueryable<Question> GetAllQuestions()
		{
			IQueryable<Question> questions = this.updateStorageBroker.SelectAllQuestions();

			return questions;
		}
	}
}
