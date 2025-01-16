using BL.Domain;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IP_MVC.Helpers;
using BL.Domain.Answers;
using BL.Domain.Questions;
using BL.Implementations;
using IP_MVC.Models;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using static QRCoder.PayloadGenerator;
using WebApplication1.Models;

namespace IP_MVC.Controllers
{
    public class FlowController(
        IFlowManager flowManager,
        ISessionManager sessionManager,
        IProjectManager projectManager,
        IQuestionManager questionManager,
        IOptionManager optionManager,
        IUserManager userManager,

        UnitOfWork unitOfWork)
        : Controller
    {
        public async Task<ViewResult> Flow(int projectId, int? parentFlowId, bool? circular)
        {
            var active = HttpContext.Session.Get<bool>("projectActive");
            ViewBag.ProjectId = projectId;
            ViewBag.ParentFlowId = parentFlowId;
            ViewBag.ActiveProject = active;
            ViewBag.Circular = circular ?? false;
            var flows = await projectManager.GetParentFlowsByProjectIdAsync(projectId);
            if (active)
            {
                flows = await projectManager.GetAvailableFlowsByProjectIdAsync(projectId);
            }

            var isParentFlow = new Dictionary<int, bool>();
            var containsQuestions = new Dictionary<int, bool>();

            foreach (var flow in flows)
            {
                isParentFlow[flow.Id] = flowManager.IsParentFlow(flow.Id);
                containsQuestions[flow.Id] = questionManager.GetQuestionsByFlowId(flow.Id).Any();
            }

            ViewBag.IsParentFlow = isParentFlow;
            ViewBag.ContainsQuestions = containsQuestions;
            return View(flows);
        }

        public IActionResult SubFlow(int parentFlowId, bool? circular)
        {
            ViewBag.Circular = circular ?? false;
            ViewBag.ProjectId = flowManager.GetFlowById(parentFlowId).ProjectId;
            ViewBag.ParentFlowId = parentFlowId;
            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");

            var flows = flowManager.GetFlowsByParentId(parentFlowId).ToList();
            var containsQuestions = new Dictionary<int, bool>();

            foreach (var flow in flows)
            {
                containsQuestions[flow.Id] = questionManager.GetQuestionsByFlowId(flow.Id).Any();
            }
            ViewBag.ContainsQuestions = containsQuestions;

            return View(flows);
        }

        public IActionResult ActivateProject(int projectId, bool active, bool circular)
        {
            HttpContext.Session.Set("projectActive", active);
            if (active)
            {
                return RedirectToAction("Flow", new { projectId, circular });
            }

            return RedirectToAction("ProjectDashboard", "Project");
        }

        public async Task<IActionResult> PlayFlow(int parentFlowId, FlowType flowType, bool active )
        {
            unitOfWork.BeginTransaction();

            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");
            ViewBag.ActiveProject = active;

            var newSession = await sessionManager.CreateNewSession(parentFlowId);
            unitOfWork.Commit();
            HttpContext.Session.SetInt32("sessionId", newSession.Id);

            var queues = HttpContext.Session.Get<Dictionary<int, Queue<int>>>("queues") ??
                         new Dictionary<int, Queue<int>>();
            
            var newList = await flowManager.GetQuestionQueueByFlowIdAsync(parentFlowId);
            queues[parentFlowId] = newList;
            
            HttpContext.Session.Set("queues", queues);

            HttpContext.Session.SetInt32("currentIndex", 0);

            HttpContext.Session.Set("flowType", flowType);
            HttpContext.Session.SetInt32("parentFlowId", parentFlowId);
            
            return RedirectToAction("Question");
        }

        public IActionResult Question(int redirectedQuestionId)
        {
            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");
            
            // Retrieve the dictionary of queues from the session.
            var queues = HttpContext.Session.Get<Dictionary<int, Queue<int>>>("queues");

            // Retrieve the flow type from the session.
            var flowType = HttpContext.Session.Get<FlowType>("flowType");

            // Retrieve parentFlowId from the session.
            var parentFlowId = HttpContext.Session.GetInt32("parentFlowId") ?? 0;

            // If the dictionary is null or doesn't contain a queue for the current flow, redirect to the end of the flow.
            if (queues == null || !queues.ContainsKey(parentFlowId) || !queues[parentFlowId].Any())
            {
                return RedirectToAction("EndSubFlow");
            }

            // Retrieve the queue of question IDs for the current flow.
            var questionQueue = queues[parentFlowId];

            var currentIndex = redirectedQuestionId;

            // If the index is out of range, redirect to the end of the flow.
            if (currentIndex < 0 || currentIndex >= questionQueue.Count)
            {
                if (flowType == FlowType.LINEAR)
                {
                    return RedirectToAction("EndSubFlow");
                }

                currentIndex = 0;
            }

            // Retrieve the current question ID from the queue.
            var questionId = questionQueue.ElementAt(currentIndex);

            // Retrieve the question based on the ID and type.
            var question = questionManager.GetQuestionByIdAndType(questionId);
            var questionType = question.Type;

            if (question.Type == QuestionType.Open)
            {
                // Create QR code
                QRCodeGenerator qrCodeGenerator = new();
                Payload payload = new Url(Url.Action("OpenQuestion", "Flow", new {questionId}, protocol: HttpContext.Request.Scheme));
                QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(payload);
                BitmapByteQRCode qrCode = new(qrCodeData);
                string base64String = Convert.ToBase64String(qrCode.GetGraphic(20));
                ViewBag.QrImage = "data:image/png;base64," + base64String;
            }

            // Get the earlier answer given
            var sessionId = HttpContext.Session.GetInt32("sessionId") ?? 0;
            var earlierAnswer = sessionManager.GetAnswerByQuestionId(sessionId, questionId);

            // Prepare the view model.
            var viewModel = new QuestionViewModel
            {
                Question = question,
                QuestionType = questionType,
                EarlierAnswer = earlierAnswer
            };
            var playerCount = HttpContext.Session.GetInt32("playerCount");
            ViewBag.PlayerCount = playerCount;
            ViewData["earlierAnswer"] = earlierAnswer;
            ViewData["currentIndex"] = currentIndex;
            ViewData["questionCount"] = questionQueue.Count;
            ViewBag.FlowType = flowType;
            ViewBag.FlowId = question.FlowId;
            ViewBag.ParentFlowId = flowManager.GetParentFlowIdBySessionId(sessionId);
            ViewBag.QuestionId = questionId;
            ViewBag.Preview = false;
            ViewBag.ProjectId = flowManager.GetFlowById(parentFlowId).ProjectId;
            
            return View("Questions/Questions", viewModel);
        }
        
        public IActionResult EndSubFlow()
        {
            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");

            var sessionId = HttpContext.Session.GetInt32("sessionId") ?? 0;
            var session = sessionManager.GetSessionById(sessionId);
            if (session == null)
            {
                //TODO: Handle error
                return View();
            }

            var answers = sessionManager.GetAnswersBySessionId(sessionId).ToList();
            var questions = sessionManager.GetQuestionsBySessionId(sessionId).ToList();

            var parentId = flowManager.GetParentFlowIdBySessionId(sessionId);

            var model = new EndSubFlowViewModel
            {
                Questions = questions,
                Answers = answers,
                FlowId = parentId ?? 0
            };

            ViewData["Message"] = "Thank you for completing the subflow!";
            ViewBag.IsParentFlow = parentId != null;
            ViewBag.ProjectId = flowManager.GetFlowById(session.FlowId).ProjectId;
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveAnswerAndRedirect(int flowId, int id, AnswerViewModel model, int redirectedQuestionId)
        {
            var sessionId = HttpContext.Session.GetInt32("sessionId") ?? 0;
            
            // If there is no answer given, redirect to the next question
            if (model.AnswerPlayer1 == null && model.AnswerPlayer2 == null)
            {
                return RedirectToAction("Question", new {redirectedQuestionId });
            }
            
            // Join the answer, in case of multiple answers
            var answerTextPlayer1 = model.AnswerPlayer1 != null ? string.Join("\n", model.AnswerPlayer1): string.Empty;
            var answerTextPlayer2 = model.AnswerPlayer2 != null ? string.Join("\n", model.AnswerPlayer2) : string.Empty;
            var flowType = HttpContext.Session.Get<FlowType>("flowType");
            var flow = flowManager.GetFlowById(flowId);

            var currentQuestionId = model.QuestionId;
            
            // If no answers are given yet, save the answer
            var newAnswer = new Answer
            {
                QuestionId = currentQuestionId,
                AnswerTextPlayer1 = answerTextPlayer1,
                AnswerTextPlayer2 = answerTextPlayer2,
                Session = sessionManager.GetSessionById(sessionId),
                Flow = flow
            };
            unitOfWork.BeginTransaction();

            // If there is no answer given to this question yet, add the answer to the session
            if (sessionManager.GetAnswerByQuestionId(sessionId, currentQuestionId) == null)
            {
                sessionManager.AddAnswerToSession(sessionId, newAnswer, flowType);
            }
            else
            {
                // If an answer is already given, search the old answer and update it
                var oldAnswer = sessionManager.GetAnswerByQuestionId(sessionId, currentQuestionId);
                sessionManager.UpdateAnswer(oldAnswer, newAnswer);
            }
            
            unitOfWork.Commit();
            
            
            // Look for the options in the database
            var allOptions = optionManager.GetOptionsSingleOrMultipleChoiceQuestion(id)?.ToList();
            if (allOptions != null && allOptions.Any())
            {
                var queues = HttpContext.Session.Get<Dictionary<int, Queue<int>>>("queues");
                var flowQueue = queues[flowId];
                
                var matchingAnswerPlayer1 = allOptions.FirstOrDefault(o => o.Text == answerTextPlayer1);
                var matchingAnswerPlayer2 = allOptions.FirstOrDefault(o => o.Text == answerTextPlayer2);

                int? redirectedQuestionIdPlayer1 = null;
                int? redirectedQuestionIdPlayer2 = null;

                if (matchingAnswerPlayer1 != null)
                {
                    var nextPotentialQuestionId = matchingAnswerPlayer1.NextQuestionId;
                    if (nextPotentialQuestionId.HasValue && (flowQueue.Contains(nextPotentialQuestionId.Value) || nextPotentialQuestionId == -1))
                    {
                        redirectedQuestionIdPlayer1 = nextPotentialQuestionId.Value;
                        if (redirectedQuestionIdPlayer1 == -1)
                        {
                            return RedirectToAction("EndSubFlow");
                        }

                        if (redirectedQuestionIdPlayer1 != 0)
                        {
                            var questionPlayer1 = questionManager.GetQuestionByIdAndType(redirectedQuestionIdPlayer1.Value);

                            if (questionPlayer1 != null && questionPlayer1.FlowId == flowId)
                            {
                                redirectedQuestionIdPlayer1 = flowQueue.ToList().IndexOf(questionPlayer1.Id);
                                // redirectedQuestionIdPlayer1 = questionPlayer1.Position;
                            }
                        }
                    }
                }
                
                if (matchingAnswerPlayer2 != null)
                {
                    var nextPotentialQuestionId = matchingAnswerPlayer2.NextQuestionId;
                    if (nextPotentialQuestionId.HasValue && (flowQueue.Contains(nextPotentialQuestionId.Value) || nextPotentialQuestionId == -1))
                    {
                        redirectedQuestionIdPlayer2 = nextPotentialQuestionId.Value;
                        if (redirectedQuestionIdPlayer2 == -1)
                        {
                            return RedirectToAction("EndSubFlow");
                        }

                        if (redirectedQuestionIdPlayer2 != 0)
                        {
                            var questionPlayer2 = questionManager.GetQuestionByIdAndType(redirectedQuestionIdPlayer2.Value);

                            if (questionPlayer2 != null && questionPlayer2.FlowId == flowId)
                            {
                                redirectedQuestionIdPlayer2 = flowQueue.ToList().IndexOf(questionPlayer2.Id);
                                // redirectedQuestionIdPlayer2 = questionPlayer2.Position;
                            }
                        }
                    }
                }

                /*if (matchingAnswerPlayer2 != null)
                {
                    redirectedQuestionIdPlayer2 = matchingAnswerPlayer2.NextQuestionId;
                    if (redirectedQuestionIdPlayer2 == -1)
                    {
                        return RedirectToAction("EndSubFlow");
                    }

                    if (redirectedQuestionIdPlayer2 != 0 && redirectedQuestionIdPlayer2.HasValue)
                    {
                        var questionPlayer2 = questionManager.GetQuestionByIdAndType(redirectedQuestionIdPlayer2.Value);

                        if (questionPlayer2 != null && questionPlayer2.FlowId == flowId)
                        {
                            if (flowQueue.Contains(questionPlayer2.Id))
                            {
                                redirectedQuestionIdPlayer2 = flowQueue.ToList().IndexOf(questionPlayer2.Id);
                            }

                            // redirectedQuestionIdPlayer2 = questionPlayer2.Position;
                        }
                    }
                }*/

                if (matchingAnswerPlayer1 != null || matchingAnswerPlayer2 != null)
                {
                    redirectedQuestionId = redirectedQuestionIdPlayer1 ?? redirectedQuestionIdPlayer2 ?? redirectedQuestionId;

                    //Remove questions in the queue that are skipped
                    if (model.NextOrPreviousButtonClicked)
                    {
                        var list = flowQueue.ToList();

                        var currentIndex = list.IndexOf(currentQuestionId);

                        // redirectedQuestionId--;
                        var removeCounter = 0;
                        if (currentIndex < redirectedQuestionId)
                        {
                            for (int i = currentIndex + 1; i < redirectedQuestionId; i++)
                            {
                                list.RemoveAt(currentIndex + 1);
                                removeCounter++;
                            }

                            redirectedQuestionId -= removeCounter;
                            flowQueue.Clear();

                            foreach (var item in list)
                            {
                                flowQueue.Enqueue(item);
                            }

                            queues[flowId] = flowQueue;
                            HttpContext.Session.Set("queues", queues);
                        }
                    }
                }
            }

            return RedirectToAction("Question",
    new { id = sessionId, redirectedQuestionId, showQr = true});
        }

        public IActionResult OpenQuestion(int questionId)
        {
            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");
            
            // Haal de huidige vraag op
            var question = questionManager.GetQuestionByIdAndType(questionId);

            // Maak een QuestionViewModel aan en vul het met de vraag en het type
            var viewModel = new QuestionViewModel
            {
                Question = question,
                QuestionType = question.Type
            };
            
            return View("Questions/OpenQuestion", viewModel);
        }

        [HttpPost]
        public IActionResult SaveAnswerOpenQuestion(int flowId, AnswerViewModel model)
        {
            unitOfWork.BeginTransaction();
            
            // Join the answer, in case of multiple answers
            var answerTextPlayer1 = string.Join("\n", model.AnswerPlayer1);
            var answerTextPlayer2 = model.AnswerPlayer2 != null ? string.Join("\n", model.AnswerPlayer2) : string.Empty;
            var sessionId = HttpContext.Session.GetInt32("sessionId") ?? 0;
            var flowType = HttpContext.Session.Get<FlowType>("flowType");
            var flow = flowManager.GetFlowById(flowId);

            // If no answers are given yet, save the answer
            var newAnswer = new Answer
            {
                QuestionId = model.QuestionId,
                AnswerTextPlayer1 = answerTextPlayer1,
                AnswerTextPlayer2 = answerTextPlayer2,
                Session = sessionManager.GetSessionById(sessionId),
                Flow = flow
            };
            
            // If there is no answer given to this question yet, add the answer to the session
            if (sessionManager.GetAnswerByQuestionId(sessionId, model.QuestionId) == null)
            {
                sessionManager.AddAnswerToSession(sessionId, newAnswer, flowType);
            }
            else
            {
                // If an answer is already given, search the old answer and update it
                var oldAnswer = sessionManager.GetAnswerByQuestionId(sessionId, model.QuestionId);
                sessionManager.UpdateAnswer(oldAnswer, newAnswer);
            }
            
            
            unitOfWork.Commit();
            return RedirectToAction("CompletedOpenQuestion");
        }

        public IActionResult CompletedOpenQuestion()
        {
            ViewBag.ActiveProject = HttpContext.Session.Get<bool>("projectActive");
            
            return View();
        }
        
        public IActionResult Delete(int flowId)
        {
            unitOfWork.BeginTransaction();
            var flowToRemove = flowManager.GetFlowById(flowId);
            var projectId1 = flowToRemove.ProjectId;

            var subFlows = flowManager.GetFlowsByParentId(flowId);
            foreach (var subFlow in subFlows)
            {
                flowManager.DeleteAsync(subFlow);
                DeleteQuestionsInFlow(subFlow.Id);
            }

            DeleteQuestionsInFlow(flowId);
            
            flowManager.DeleteAsync(flowToRemove);
            unitOfWork.Commit();
            
            return RedirectToAction("Flow", new { projectId = projectId1 });
        }

        public void DeleteQuestionsInFlow(int flowId)
        {
            var questions = questionManager.GetQuestionsByFlowId(flowId);
            foreach (var question in questions)
            {
                if (question.Type == QuestionType.MultipleChoice || question.Type == QuestionType.SingleChoice)
                {
                    var options = optionManager.GetOptionsSingleOrMultipleChoiceQuestion(question.Id);
                    if (options != null)
                    {
                        foreach (var option in options)
                        {
                            optionManager.DeleteAsync(option);
                        }
                    }
                }

                questionManager.RemoveAnswersByQuestionId(question.Id);
                questionManager.DeleteAsync(question);
            }
        }

        [HttpGet]
        public IActionResult Edit(int parentFlowId)
        {
            ViewBag.ActiveProject = false;
            ViewBag.FlowId = parentFlowId;
            
            var flow = flowManager.GetFlowById(parentFlowId);
            var questions = questionManager.GetQuestionsByFlowId(parentFlowId).OrderBy(q => q.Position);

            var model = new FlowEditViewModel
            {
                Flow = flow,
                Questions = questions
            };
            var active = HttpContext.Session.Get<bool>("projectActive");
            ViewBag.ActiveProject = active;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int parentFlowId, FlowEditViewModel newFlowModel)
        {
            unitOfWork.BeginTransaction();
            var existingFlow = flowManager.GetFlowById(parentFlowId);

            if (existingFlow == null)
            {
                unitOfWork.Commit();
                return NotFound();
            }

            var newFlow = existingFlow;
            newFlow.Name = newFlowModel.Flow.Name;
            newFlow.Description = newFlowModel.Flow.Description;
            newFlow.StartDate = newFlowModel.Flow.StartDate.ToUniversalTime();
            newFlow.EndDate = newFlowModel.Flow.EndDate.ToUniversalTime();

            flowManager.UpdateAsync(existingFlow, newFlow);

            unitOfWork.Commit();
            return RedirectToAction("Flow", new { projectId = newFlow.ProjectId });
        }
        
        public IActionResult Create(Flow flow, int projectId, int? parentFlowId)
        {
            unitOfWork.BeginTransaction();
            //if (!ModelState.IsValid) return View(flow);

            flow.ProjectId = projectId;
            flow.ParentFlowId = parentFlowId;
            flow.StartDate = flow.StartDate.ToUniversalTime();
            flow.EndDate = flow.EndDate.ToUniversalTime();
            flowManager.AddAsync(flow);

            unitOfWork.Commit();

            if (parentFlowId == null)
            {
                return RedirectToAction("Flow", new { ProjectId = projectId });
            }

            return RedirectToAction("Flow", new { parentFlowId = flow.ParentFlowId });
        }

        [HttpPost]
        public IActionResult Reorder(int parentFlowId, int newPosition)
        {
            unitOfWork.BeginTransaction();
            var flow = flowManager.GetFlowById(parentFlowId);
            var oldPosition = flow.Position;
            flow.Position = newPosition;

            List<Flow> affectedFlows;
            if (newPosition < oldPosition)
            {
                // The flow has been moved up, so increment the position of all flows between the old and new position
                affectedFlows = flowManager.GetFlowsBetweenPositions(newPosition, oldPosition - 1).ToList();
                foreach (var affectedFlow in affectedFlows)
                {
                    affectedFlow.Position++;
                }
            }
            else
            {
                // The flow has been moved down, so decrement the position of all flows between the old and new position
                affectedFlows = flowManager.GetFlowsBetweenPositions(oldPosition + 1, newPosition).ToList();
                foreach (var affectedFlow in affectedFlows)
                {
                    affectedFlow.Position--;
                }
            }

            // Add the initially moved flow to the list of affected flows
            affectedFlows.Add(flow);

            // Update all affected flows at once
            flowManager.UpdateAllAsync(affectedFlows);

            unitOfWork.Commit();
            return RedirectToAction("Flow");
        }

        public IActionResult RedirectTroughPreview(int redirectedQuestionId, int flowId)
        {
            var questionsByFlowId = questionManager.GetQuestionsByFlowIdWithMedia(flowId).ToList();

            if (redirectedQuestionId < 0 || redirectedQuestionId >= questionsByFlowId.Count)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = "Question not found"
                };
                return View("Error", errorViewModel);
            }

            var question = questionsByFlowId[redirectedQuestionId];

            var viewModel = new QuestionViewModel()
            {
                Question = question,
                QuestionType = question.Type
            };
            ViewData["currentIndex"] = redirectedQuestionId;
            ViewData["questionCount"] = questionManager.GetQuestionsByFlowId(question.FlowId).Count();
            ViewBag.FlowType = question.Type;
            ViewBag.Preview = true;

            return View($"~/Views/Flow/Questions/Questions.cshtml", viewModel);
        }

        [HttpPost]
        public JsonResult SetPlayerCount([FromBody] PlayerCountModel model)
        {
            HttpContext.Session.SetInt32("playerCount", model.PlayerCount);
            return Json(new { success = true });
        }
        
        public IActionResult StopProjectSession(ExitFlowLoginModel model)
        {
            var loggedInUser = HttpContext.User.Identity?.Name;
            if (!userManager.CheckPassword(loggedInUser, model.Username, model.Password))
            {
                return NoContent();
            }
            
            HttpContext.Session.Set("projectActive", false);
            return RedirectToAction("ProjectDashboard", "Project");
        }
    }
}