/*author:Eda Nur Tokmak*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Facebook_Mvc.Models;
using Microsoft.AspNetCore.Http;
using Facebook_Mvc.Data;
using Microsoft.AspNetCore.Identity;
using Facebook_Mvc.ViewModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace Facebook_Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly Facebook_MvcContext _context;
        [Obsolete]
        private readonly IHostingEnvironment webHostEnvironment;

        [Obsolete]
        public HomeController(Facebook_MvcContext context, IHostingEnvironment hostenvironment)
        {
            _context = context;
            webHostEnvironment = hostenvironment;
            
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            User loggedInUser = _context.User.Where(x => x.Eposta == user.Eposta && x.Password == user.Password).FirstOrDefault();
            if (loggedInUser == null)
            {
                ViewBag.Message = "Girdiğin kullanıcı adı ve şifre kayıtlarımızdakiyle eşleşmedi. Lütfen doğru girdiğinden emin ol ve tekrar dene.";
                return View();
            }
            HttpContext.Session.SetString("UserName", loggedInUser.UserName);
            HttpContext.Session.SetString("LastName", loggedInUser.LastName);
            HttpContext.Session.SetString("Password", loggedInUser.Password);
            HttpContext.Session.SetString("Eposta", loggedInUser.Eposta);
            HttpContext.Session.SetString("ProfilePicture", loggedInUser.ProfilePicture);
            HttpContext.Session.SetString("BackgroundImg", loggedInUser.BackgroundImg);
            Response.Cookies.Append("LastLoggedInTime", DateTime.Now.ToString());
            return RedirectToAction("Index");
        }
       public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpPost]
        
        public IActionResult Signup(User user)
        {
            var exist = (from s in _context.User
                         where s.UserName == user.UserName
                         select s).FirstOrDefault<User>();
            if (exist == null)
            {
                user.ProfilePicture = "";
                user.BackgroundImg = "";
            _context.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("LastName", user.LastName);
            HttpContext.Session.SetString("Password", user.Password);
            HttpContext.Session.SetString("Eposta", user.Eposta);
            HttpContext.Session.SetString("Gender", user.Gender);
            HttpContext.Session.SetString("ProfilePicture",user.ProfilePicture);
            HttpContext.Session.SetString("BackgroundImg",user.BackgroundImg);
            Response.Cookies.Append("LastLoggedInTime", DateTime.Now.ToString());
            return RedirectToAction("Index");

                }
            else
            {
                ViewBag.Message = "Bu kullanıcı adına sahip bir kullanıcı zaten var";
            }
            return View();
            }
        //[HttpGet]
        public IActionResult Index()
        {
            //if (String.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            //{
            //    return RedirectToAction("Login");
            //}
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Name = HttpContext.Session.GetString("UserName")+" "+HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            ViewBag.BackgroundImg = HttpContext.Session.GetString("BackgroundImg");

            var posts = (from m in _context.Post
                         .Include("Comments")
                         .Include("Likes")
                         orderby m.CreateDate descending
                         select m);
            ViewBag.Post = posts.ToList();
            return View();
        }
        public IActionResult Profile()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.LastName = HttpContext.Session.GetString("UserName") + " " + HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            ViewBag.BackgroundImg = HttpContext.Session.GetString("BackgroundImg");
            ViewBag.Eposta = HttpContext.Session.GetString("Eposta");

            var posts = (from m in _context.Post
                        .Include("Comments")
                        .Include("Likes")
                         where m.Eposta == HttpContext.Session.GetString("Eposta")
                         orderby m.CreateDate descending
                         select m);


            ViewBag.Post = posts.ToList();

            return View();
        }

        //RESIMSTART
        // GET: FileUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<IActionResult> New(UserViewModel model)
        {
            User own = (from s in _context.User
                        where s.Eposta == HttpContext.Session.GetString("Eposta")
                        select s).FirstOrDefault<User>();
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                string uniqueFileName2 = UploadedFile2(model);

                User kullanıcı = new User
                {
                    ProfilePicture = uniqueFileName,
                    BackgroundImg = uniqueFileName2
                };
                own.ProfilePicture = kullanıcı.ProfilePicture;
                own.BackgroundImg = kullanıcı.BackgroundImg;
                HttpContext.Session.SetString("ProfilePicture", kullanıcı.ProfilePicture);
                HttpContext.Session.SetString("BackgroundImg", kullanıcı.BackgroundImg);

                //Profil Fotoğrafı Güncellendiğinde Atılacak Olan Post
                Post Photo_Post = new Post();
                Photo_Post.PostPicture = kullanıcı.ProfilePicture;
                Photo_Post.UserName = own.UserName;
                Photo_Post.LastName = own.LastName;
                Photo_Post.ProfilePicture = own.ProfilePicture;
                Photo_Post.Eposta = own.Eposta;

                //Profil Fotoğrafı Güncellendiğinde Atılacak Olan Post
                _context.Add(Photo_Post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile");
            }

            return View();

        }

        [Obsolete]
        private string UploadedFile(UserViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        private string UploadedFile2(UserViewModel model)
        {
            string uniqueFileName = null;

            if (model.BackgroundImg != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images2");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BackgroundImg.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.BackgroundImg.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        //RESIMEND

        [HttpPost]
        public IActionResult Text(Post post)
        {
            post.User = (from s in _context.User
                           where s.Eposta == HttpContext.Session.GetString("Eposta")
                           select s).FirstOrDefault<User>();
            post.Eposta = post.User.Eposta;
            post.ProfilePicture = post.User.ProfilePicture;
            post.UserName = post.User.UserName;
            post.LastName = post.User.LastName;
            post.Comments = post.Comments;
            _context.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        //Comment
        [HttpPost]
        public IActionResult Comment(Comment comment)
        {
            User commentOwner=(from s in _context.User
             where s.Eposta == HttpContext.Session.GetString("Eposta")
             select s).FirstOrDefault<User>();

            comment.CommentedBy = (from s in _context.User
                                   where s.Eposta == HttpContext.Session.GetString("Eposta")
                                   select s).FirstOrDefault<User>();
            comment.CommentedByName = commentOwner.UserName + " " + commentOwner.LastName;
            comment.CommentedByPicture = commentOwner.ProfilePicture;
            Post post = new Post();
            post = (from s in _context.Post
                                 where s.PostID == comment.PostID
                                 select s).FirstOrDefault<Post>();

            post.Comments = new List<Comment>();
            post.Comments.Add(comment);
            post.Comments = (from s in _context.Comment
                             where s.PostID == post.PostID
                             select s).ToList();
            ViewBag.Comments = post.Comments;

            _context.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Like(Like like)
        {
            User likeOwner = (from s in _context.User
                                 where s.Eposta == HttpContext.Session.GetString("Eposta")
                                 select s).FirstOrDefault<User>();

            like.User = (from s in _context.User
                                   where s.Eposta == HttpContext.Session.GetString("Eposta")
                                   select s).FirstOrDefault<User>();
            
            like.Post=(from s in _context.Post
                    where s.PostID == like.PostID
                    select s).FirstOrDefault<Post>();

            like.PostID = like.Post.PostID;
            like.UserID = like.User.UserID;
            like.Name = likeOwner.UserName + " " + likeOwner.LastName;
            Post post = new Post();
            post = (from s in _context.Post
                    where s.PostID == like.PostID
                    select s).FirstOrDefault<Post>();
            post.Likes = new List<Like>();
            post.Likes.Add(like);
            post.Likes = (from s in _context.Likes
                             where s.PostID == like.PostID
                             select s).ToList();
            
            post.LikeCount++;
            _context.Add(like);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //FINDFRIENDS
        public IActionResult FindFriends()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.LastName = HttpContext.Session.GetString("UserName") + " " + HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            var persons = (from s in _context.User
                           where s.Eposta != HttpContext.Session.GetString("Eposta")
                           select s).ToList();

            User getRequests = (from s in _context.User
                                where s.Eposta == HttpContext.Session.GetString("Eposta")
                                select s).FirstOrDefault<User>();

            //istek alınan kullanıcılar
            var requestUsers = new List<User>();
            requestUsers = (from s in _context.User
                            join f in _context.Friend on s.UserID equals f.RequestedById
                            where f.RequestedToId == getRequests.UserID
                            where f.FriendRequestFlag == FriendRequestFlag.None
                            select s).ToList();
            ViewBag.Requests = requestUsers;

            //istek atılan kullanıcılar
            var requestToUsers = (from s in _context.User
                                  join f in _context.Friend on s.UserID equals f.RequestedToId
                                  where f.RequestedById == getRequests.UserID
                                  where f.FriendRequestFlag == FriendRequestFlag.None
                                  select s).ToList();
            ViewBag.RequestTo = requestToUsers;

            //isteği kabul edilen kullanıcılar
            var acceptedUsers = new List<User>();
            acceptedUsers = (from s in _context.User
                             join f in _context.Friend on s.UserID equals f.RequestedById
                             where f.RequestedToId == getRequests.UserID
                             where f.FriendRequestFlag == FriendRequestFlag.Approved
                             select s).ToList();

            var acceptedUsersByMe = new List<User>();
            acceptedUsersByMe = (from s in _context.User
                                 join f in _context.Friend on s.UserID equals f.RequestedToId
                                 where f.RequestedById == getRequests.UserID
                                 where f.FriendRequestFlag == FriendRequestFlag.Approved
                                 select s).ToList();

            //arkadaslar
            ViewBag.Friends = acceptedUsers.Union(acceptedUsersByMe);

            //tanıyor olabileceğiniz kişiler
            var lastPersons = persons.Except(requestUsers.Union(requestToUsers));
            lastPersons = lastPersons.Except(acceptedUsers);
            lastPersons = lastPersons.Except(acceptedUsersByMe);
            TempData["Kisiler"] = lastPersons;
            ViewBag.Persons = lastPersons;
            return View();
        }

        public IActionResult RequestToHtml()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.LastName = HttpContext.Session.GetString("UserName") + " " + HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            var persons = (from s in _context.User
                           where s.Eposta != HttpContext.Session.GetString("Eposta")
                           select s).ToList();

            User getRequests = (from s in _context.User
                                where s.Eposta == HttpContext.Session.GetString("Eposta")
                                select s).FirstOrDefault<User>();

            //istek alınan kullanıcılar
            var requestUsers = (from s in _context.User
                                join f in _context.Friend on s.UserID equals f.RequestedById
                                where f.RequestedToId == getRequests.UserID
                                where f.FriendRequestFlag == FriendRequestFlag.None
                                select s).ToList();
            ViewBag.Requests = requestUsers;

            //istek atılan kullanıcılar
            var requestToUsers = (from s in _context.User
                                  join f in _context.Friend on s.UserID equals f.RequestedToId
                                  where f.RequestedById == getRequests.UserID
                                  where f.FriendRequestFlag == FriendRequestFlag.None
                                  select s).ToList();
            ViewBag.RequestTo = requestToUsers;

            //isteği kabul edilen kullanıcılar
            var acceptedUsers = new List<User>();
            acceptedUsers = (from s in _context.User
                             join f in _context.Friend on s.UserID equals f.RequestedById
                             where f.RequestedToId == getRequests.UserID
                             where f.FriendRequestFlag == FriendRequestFlag.Approved
                             select s).ToList();
            var acceptedUsersByMe = new List<User>();
            acceptedUsersByMe = (from s in _context.User
                                 join f in _context.Friend on s.UserID equals f.RequestedToId
                                 where f.RequestedById == getRequests.UserID
                                 where f.FriendRequestFlag == FriendRequestFlag.Approved
                                 select s).ToList();

            //arkadaslar
            ViewBag.Friends = acceptedUsers.Union(acceptedUsersByMe);

            //tanıyor olabileceğiniz kişiler
            var lastPersons = persons.Except(requestUsers.Union(requestToUsers));
            lastPersons = lastPersons.Except(acceptedUsers);
            lastPersons = lastPersons.Except(acceptedUsersByMe);
            ViewBag.Persons = lastPersons;
            return View();
        }

        [HttpPost]
        public IActionResult AddFriendRequest(User user)
        {
            User friendUser = (from b in _context.User
                               where b.UserID == user.UserID
                               select b).FirstOrDefault<User>();

            User userOwn = (from s in _context.User
                           where s.Eposta == HttpContext.Session.GetString("Eposta")
                           select s).FirstOrDefault<User>();
            var friendRequest = new Friend()
            {
                RequestedBy = userOwn,
                RequestedById = userOwn.UserID,
                RequestedTo = friendUser,
                RequestedToId=friendUser.UserID,
                FriendRequestFlag = FriendRequestFlag.None
            };
            userOwn.SentFriendRequests.Add(friendRequest);
            _context.Add(friendRequest);
            _context.SaveChanges();
            return RedirectToAction("FindFriends");
        }
        [HttpPost]
        public IActionResult ApproveFriendRequest(User user)
        {
            User userOwn = (from s in _context.User
                            where s.Eposta == HttpContext.Session.GetString("Eposta")
                            select s).FirstOrDefault<User>();
            Friend byFriend = (from s in _context.Friend
                            where s.RequestedById ==user.UserID
                            select s).FirstOrDefault<Friend>();

            byFriend.FriendRequestFlag = FriendRequestFlag.Approved;
            userOwn.ReceievedFriendRequests.Add(byFriend);
            _context.SaveChanges();
            return RedirectToAction("FindFriends");
        }
        public IActionResult FriendsTab()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.LastName = HttpContext.Session.GetString("UserName") + " " + HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            ViewBag.BackgroundImg = HttpContext.Session.GetString("BackgroundImg");
            ViewBag.Eposta = HttpContext.Session.GetString("Eposta");

            var persons = (from s in _context.User
                           where s.Eposta != HttpContext.Session.GetString("Eposta")
                           select s).ToList();

            User getRequests = (from s in _context.User
                                where s.Eposta == HttpContext.Session.GetString("Eposta")
                                select s).FirstOrDefault<User>();

            //istek alınan kullanıcılar
            var requestUsers = (from s in _context.User
                                join f in _context.Friend on s.UserID equals f.RequestedById
                                where f.RequestedToId == getRequests.UserID
                                where f.FriendRequestFlag == FriendRequestFlag.None
                                select s).ToList();
            ViewBag.Requests = requestUsers;

            //istek atılan kullanıcılar
            var requestToUsers = (from s in _context.User
                                  join f in _context.Friend on s.UserID equals f.RequestedToId
                                  where f.RequestedById == getRequests.UserID
                                  where f.FriendRequestFlag == FriendRequestFlag.None
                                  select s).ToList();
            ViewBag.RequestTo = requestToUsers;

            //isteği kabul edilen kullanıcılar
            var acceptedUsers = (from s in _context.User
                             join f in _context.Friend on s.UserID equals f.RequestedById
                             where f.RequestedToId == getRequests.UserID
                             where f.FriendRequestFlag == FriendRequestFlag.Approved
                             select s).ToList();
            //arkadaslar
            ViewBag.Friends = acceptedUsers;

            //tanıyor olabileceğiniz kişiler
            var lastPersons = persons.Except(requestUsers.Union(requestToUsers));
            lastPersons = lastPersons.Except(acceptedUsers);
            ViewBag.Persons = lastPersons;
            return View();
        }
        public IActionResult Messenger()
        {
            var persons = (from s in _context.User
                           where s.Eposta != HttpContext.Session.GetString("Eposta")
                           select s).ToList();

            User getRequests = (from s in _context.User
                                where s.Eposta == HttpContext.Session.GetString("Eposta")
                                select s).FirstOrDefault<User>();

                //istek alınan kullanıcılar
                var requestUsers = (from s in _context.User
                                join f in _context.Friend on s.UserID equals f.RequestedById
                                where f.RequestedToId == getRequests.UserID
                                where f.FriendRequestFlag == FriendRequestFlag.None
                                select s).ToList();
                ViewBag.Requests = requestUsers;
            //istek atılan kullanıcılar
            var requestToUsers = (from s in _context.User
                                  join f in _context.Friend on s.UserID equals f.RequestedToId
                                  where f.RequestedById == getRequests.UserID
                                  where f.FriendRequestFlag == FriendRequestFlag.None
                                  select s).ToList();
            ViewBag.RequestTo = requestToUsers;

            //isteği kabul edilen kullanıcılar
            var acceptedUsers = new List<User>();
            acceptedUsers = (from s in _context.User
                             join f in _context.Friend on s.UserID equals f.RequestedById
                             where f.RequestedToId == getRequests.UserID
                             where f.FriendRequestFlag == FriendRequestFlag.Approved
                             select s).ToList();

            var acceptedUsersByMe = new List<User>();
            acceptedUsersByMe = (from s in _context.User
                                 join f in _context.Friend on s.UserID equals f.RequestedToId
                                 where f.RequestedById == getRequests.UserID
                                 where f.FriendRequestFlag == FriendRequestFlag.Approved
                                 select s).ToList();

            //arkadaslar
            ViewBag.Friends = acceptedUsers.Union(acceptedUsersByMe);
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.LastName = HttpContext.Session.GetString("UserName") + " " + HttpContext.Session.GetString("LastName");
            ViewBag.ProfilePicture = HttpContext.Session.GetString("ProfilePicture");
            ViewBag.BackgroundImg = HttpContext.Session.GetString("BackgroundImg");
            ViewBag.Eposta = HttpContext.Session.GetString("Eposta");
            ViewBag.RecipientName = HttpContext.Session.GetString("Name");
            ViewBag.RecipientPP = HttpContext.Session.GetString("RecipientPP");
            ViewBag.RecipientEposta = HttpContext.Session.GetString("RecipientEposta");

            var msgs = (from s in _context.Messages
                        orderby s.SendDate ascending
                                select s).ToList();

            //var msgs_ = (from s in _context.Messages
            //            where s.RecipientEposta == HttpContext.Session.GetString("Eposta")
            //            where s.SenderEposta == HttpContext.Session.GetString("RecipientEposta")
            //            orderby s.SendDate ascending
            //            select s).ToList();

            ViewBag.Messages = msgs;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Message([FromBody]Messages messages)
        {
            var persons = (from s in _context.User
                           where s.Eposta != HttpContext.Session.GetString("Eposta")
                           select s).ToList();

            User getRequests = (from s in _context.User
                                where s.Eposta == HttpContext.Session.GetString("Eposta")
                                select s).FirstOrDefault<User>();

            //istek alınan kullanıcılar
            var requestUsers = (from s in _context.User
                                join f in _context.Friend on s.UserID equals f.RequestedById
                                where f.RequestedToId == getRequests.UserID
                                where f.FriendRequestFlag == FriendRequestFlag.None
                                select s).ToList();
            ViewBag.Requests = requestUsers;

            //istek atılan kullanıcılar
            var requestToUsers = (from s in _context.User
                                  join f in _context.Friend on s.UserID equals f.RequestedToId
                                  where f.RequestedById == getRequests.UserID
                                  where f.FriendRequestFlag == FriendRequestFlag.None
                                  select s).ToList();
            ViewBag.RequestTo = requestToUsers;

            //isteği kabul edilen kullanıcılar
            var acceptedUsers = new List<User>();
            acceptedUsers = (from s in _context.User
                             join f in _context.Friend on s.UserID equals f.RequestedById
                             where f.RequestedToId == getRequests.UserID
                             where f.FriendRequestFlag == FriendRequestFlag.Approved
                             select s).ToList();

            var acceptedUsersByMe = new List<User>();
            acceptedUsersByMe = (from s in _context.User
                                 join f in _context.Friend on s.UserID equals f.RequestedToId
                                 where f.RequestedById == getRequests.UserID
                                 where f.FriendRequestFlag == FriendRequestFlag.Approved
                                 select s).ToList();

            //arkadaslar
            ViewBag.Friends = acceptedUsers.Union(acceptedUsersByMe);

            messages.Sender= (from s in _context.User
                              where s.Eposta == HttpContext.Session.GetString("Eposta")
                              select s).FirstOrDefault<User>();
            messages.RecipientEposta = HttpContext.Session.GetString("RecipientEposta");
            messages.SenderEposta = HttpContext.Session.GetString("Eposta");
            messages.Recipient = (from s in _context.User
                                  where s.Eposta == messages.RecipientEposta
                                  select s).FirstOrDefault<User>();
            await _context.Messages.AddAsync(messages);
            await _context.SaveChangesAsync();
            var msgs = (from s in _context.Messages
                        where s.RecipientEposta == HttpContext.Session.GetString("RecipientEposta")
                        where s.Sender.Eposta == HttpContext.Session.GetString("Eposta")
                        orderby s.SendDate ascending
                        select s.Message).ToList();

            ViewBag.Messages = msgs;
            return Ok();
            

        }
        [HttpPost]
        public IActionResult getUser(Messages mesage)
        {
            User recipient = (from s in _context.User
                              where s.Eposta == mesage.RecipientEposta
                              select s).FirstOrDefault<User>();
           
            ViewBag.RecipientName = recipient.UserName + " " + recipient.LastName;
            HttpContext.Session.SetString("Name", recipient.UserName + " " + recipient.LastName);
            HttpContext.Session.SetString("RecipientEposta", mesage.RecipientEposta);
            HttpContext.Session.SetString("RecipientPP", recipient.ProfilePicture);

            var sendmessages = (from s in _context.Messages
                            where s.RecipientEposta == recipient.Eposta
                            where s.Sender.Eposta==HttpContext.Session.GetString("Eposta")
                            orderby s.SendDate ascending
                            select s.Message).ToList();

            var getmessages= (from s in _context.Messages
                              where s.RecipientEposta == HttpContext.Session.GetString("Eposta")
                              where s.Sender.Eposta == recipient.Eposta
                              orderby s.SendDate ascending
                              select s.Message).ToList();

            ViewBag.getMsg = getmessages;
            ViewBag.sendMsg = sendmessages;
            ViewBag.Messages = getmessages.Union(sendmessages);
            ViewBag.RecipientEPosta = recipient.Eposta;
            return RedirectToAction("Messenger");
        }
        [HttpPost]
        public IActionResult Share(Share share)
        {
            User shareOwner = (from s in _context.User
                              where s.Eposta == HttpContext.Session.GetString("Eposta")
                              select s).FirstOrDefault<User>();

            share.User = (from s in _context.User
                         where s.Eposta == HttpContext.Session.GetString("Eposta")
                         select s).FirstOrDefault<User>();

          
            
            Post post = new Post();
            post = (from s in _context.Post
                    where s.PostID == share.PostID
                    select s).FirstOrDefault<Post>();
            share.PostID = post.PostID;
            share.UserID = shareOwner.UserID;
            share.Name = shareOwner.UserName + " " + shareOwner.LastName;
            post.Shares = new List<Share>();
            post.Shares.Add(share);
          
            Post newPost = new Post();
            newPost.ProfilePicture = shareOwner.ProfilePicture;
            newPost.PostText = post.PostText;
            newPost.UserName = shareOwner.UserName;
            newPost.LastName = shareOwner.LastName;
            newPost.User = shareOwner;
            _context.Add(share);
            _context.Add(newPost);
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
/*author:Eda Nur Tokmak*/
