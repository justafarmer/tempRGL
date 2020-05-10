using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using rungreenlake.Models;
using rungreenlake.Models.ViewModels;
using rungreenlake.web.Areas.Identity.Data;
using rungreenlake.Controllers;
using rungreenlake.web.Data;

namespace SprintOne.Controllers
{
    [Authorize]
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<RungreenlakeUser> _userManager;

        public ThreadsController(ApplicationDbContext context, UserManager<RungreenlakeUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Threads
        public async Task<IActionResult> Index(int? retrieveThreadID)
        {
            var viewModel = new MailboxViewModel();

            viewModel.Threads = await _context.Threads
                .Include(c => c.Conversations)
                    .ThenInclude(m => m.Message)
                .ToListAsync();

            if (retrieveThreadID != null)
            {
                Thread thread = viewModel.Threads
                    .Where(t => t.ThreadID == retrieveThreadID)
                    .Single();

                viewModel.Messages = thread.Conversations.Select(c => c.Message);
            }
            return View(viewModel);

            //return View(await _context.Threads.ToListAsync());
        }

        public async Task<IActionResult> Mail(int? retrieveThreadID)
        {
            var currid = GetUserID();

            var viewModel = new MailboxViewModel();

            viewModel.Threads = await _context.Threads
                .Include(c => c.Conversations)
                    .ThenInclude(m => m.Message)
                        .ThenInclude(mp => mp.Profile)
                .Include(p => p.InitiatorProfile)
                .Include(r => r.ReceiverProfile)
                .Where(t => t.InitiatorID == currid || t.ReceiverID == currid)
                .ToListAsync();

            if (retrieveThreadID != null)
            {
                Thread thread = viewModel.Threads
                    .Where(t => t.ThreadID == retrieveThreadID)
                    .Single();

                viewModel.Messages = thread.Conversations
                    .Select(c => c.Message);

                foreach (Message m in viewModel.Messages)
                {
                    m.Profile = _context.Profiles
                        .Where(i => i.ProfileID == m.MsgSenderID)
                        .Single();
                }
            }

            return View(viewModel);
            //return View(await _context.Threads.ToListAsync());
        }


        // GET: Threads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thread = await _context.Threads
                .FirstOrDefaultAsync(m => m.ThreadID == id);
            if (thread == null)
            {
                return NotFound();
            }

            return View(thread);
        }

        // GET: Threads/Create
        public IActionResult Send()
        {
            var viewModel = new MailboxViewModel();
            var testList = _context.RunGreenLakeUsers
                .Select(p => new { p.LinkID, p.FirstName, p.LastName })
                .ToList();
            viewModel.ReceiverList = new SelectList(testList, "LinkID", "FirstName");

            return View(viewModel);
        }


        // POST: Threads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send([Bind("ReceiverID", "Header", "Body")] MailboxViewModel mailBox)
        {

            if (ModelState.IsValid)
            {
                var currid = GetUserID();
                var addThread = new Thread();
                addThread.InitiatorID = currid;
                addThread.ReceiverID = mailBox.ReceiverID;
                _context.Add(addThread);
                await _context.SaveChangesAsync();

                var addMessage = new Message();
                addMessage.MsgHeader = mailBox.Header;
                addMessage.MsgBody = mailBox.Body;
                addMessage.MsgSenderID = currid;
                _context.Add(addMessage);
                await _context.SaveChangesAsync();

                var addConversation = new Conversation();
                addConversation.MessageID = addMessage.MessageID;
                addConversation.ThreadID = addThread.ThreadID;
                _context.Add(addConversation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Mail));
            }

            var testList = _context.RunGreenLakeUsers
                .Select(p => new { p.LinkID, p.FirstName, p.LastName })
                .ToList();
            mailBox.ReceiverList = new SelectList(testList, "LinkID", "FirstName");
            return View(mailBox);
        }



        // GET: Threads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Threads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ThreadID,InitiatorID,ReceiverID")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thread);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thread);
        }

        public int GetUserID()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            RungreenlakeUser user = _context.Users.Find(userId);
            var currid = user.LinkID;
            return currid;
        }
    }
}
