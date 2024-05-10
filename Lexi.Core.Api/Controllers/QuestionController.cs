using System;
using System.Collections.Generic;
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

		public QuestionController(IUpdateStorageBroker storageBroker)
		{
			this.updateStorageBroker = storageBroker;
		}

		[HttpPost]
		public async ValueTask<ActionResult<List<Question>>> AddQuestions()
		{
			var questions = new List<Question>
			{
				  new Question { Id = Guid.NewGuid(), Content = "Can you describe your home?", Number = 1, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "What do you like the most about your home?", Number = 2, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "How long have you lived in your current home?", Number = 3, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "Do you prefer living in a house or an apartment?", Number = 4, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "What is your favorite room in your home and why?", Number = 5, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "Are there any changes you would like to make to your home?", Number = 6, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "Who do you live with in your home?", Number = 7, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "What makes a house feel like a home to you?", Number = 8, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "Do you enjoy spending time at home or outside more?", Number = 9, QuestionType = "Home 🏡" },
				  new Question { Id = Guid.NewGuid(), Content = "Can you share a special memory related to your home?", Number = 10, QuestionType = "Home 🏡" }
			};

			foreach (var question in questions)
			{
				await this.updateStorageBroker.InsertQuestionAsync(question);
			}

			return Ok(questions);
		}
	}
}
