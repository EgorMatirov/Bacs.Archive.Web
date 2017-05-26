using System.Linq;
using Bacs.Archive.Web.Models.ProblemViewModels;
using Bacs.Archive.Web.Services.ArchiveClient;
using Bacs.Archive.Web.Services.TestsFetcher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore;

namespace Bacs.Archive.Web.Controllers
{
    [Authorize(Roles="User")]
    public class ProblemController : Controller
    {
        private readonly IArchiveClientService _archiveClientService;
        private readonly ITestsFetcher _testsFetcher;

        public ProblemController(IArchiveClientService archiveClientService, ITestsFetcher testsFetcher)
        {
            _archiveClientService = archiveClientService;
            _testsFetcher = testsFetcher;
        }

        public ActionResult Index(int page = 1, string query = "")
        {
            const int problemsPerPage = 15;
            var allProblems = _archiveClientService.GetAll()
                .Select(x => new IndexViewModel.ProblemInfo {Id = x.Id, Name = x.Name, Revision = x.Revision})
                .Where(x => x.Id.StartsWith(query))
                .OrderBy(x => x.Id)
                .ToList();
            return View(new IndexViewModel(){Problems = allProblems.ToPagedList(problemsPerPage, page), SearchQuery = query});
        }

        public ActionResult Details(string id)
        {
            var problem = _archiveClientService.GetFull(id);
            return problem == null ? (ActionResult) RedirectToAction(nameof(Index), new { query = id }) : View(problem);
        }

        public ActionResult Create()
        {
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var bytes = collection.Files.Single().OpenReadStream().AsEnumerable();
            return RedirectToAction("Details", new {id = _archiveClientService.Create(bytes)});
        }

        public ActionResult Tests(string id)
        {
            var problem = _archiveClientService.GetFull(id);
            var problemArchive = _archiveClientService.GetArchive(id);
            var tests = _testsFetcher.FetchTests(problemArchive, id, problem.TestGroups.SelectMany(x => x.Tests).ToArray());
            return View(new TestsViewModel(id, problem.TestGroups, tests));
        }

        public ActionResult EditFlags(string id)
        {
            var problemFull = _archiveClientService.GetFull(id);
            var reservedFlags = problemFull
                .ReservedFlags
                .Select(x => new EditFlagsViewModel.ReservedFlag
                {
                    Enabled = x.Enabled,
                    Name = x.Flag.ToString()
                })
                .ToList();
            var customFlags = problemFull.CustomFlags.Select(x => x.Name);
            var model = new EditFlagsViewModel()
            {
                Id = problemFull.Id,
                CustomFlags = customFlags,
                ReservedFlags = reservedFlags
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditFlags(EditFlagsViewModel model)
        {
            var customFlags = model.CustomFlags.Select(x => new ProblemFull.CustomFlag(x));
            var reservedFlags = model.ReservedFlags.Select(x => ProblemFull.ReservedFlag.FromName(x.Name, x.Enabled));
            _archiveClientService.UpdateFlags(model.Id, customFlags, reservedFlags);
            return View();
        }

        [HttpGet]
        public IActionResult Download(string id)
        {
            return File(_archiveClientService.GetArchive(id).ToArray(), "application/zip", $"{id}.zip");
        }
    }
}