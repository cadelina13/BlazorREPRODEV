using BlazorREPRODEV.App.Client.Pages;
using BlazorREPRODEV.App.Shared.Models;
using BlazorREPRODEV.App.Shared.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Spire.Pdf.Exporting.XPS.Schema;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace BlazorREPRODEV.App.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class APIController : Controller
    {
        public readonly IDbContextFactory<ApplicationDbContext> ctx;
        public readonly IWebHostEnvironment env;

        public APIController(IDbContextFactory<ApplicationDbContext> _ctx, IWebHostEnvironment _env)
        {
            ctx = _ctx;
            env = _env;
            AddInitialData();
        }
        [HttpGet("DownloadMaterial/{MaterialId}/{FileName}")]
        public IActionResult DownloadMaterial(string MaterialId, string FileName)
        {
            var filepath = System.IO.Path.Combine(env.WebRootPath, "reading_materials", MaterialId, FileName);
            var fs = new FileStream(filepath, FileMode.Open);
            return File(fs, "application/octet-stream", FileName);
        }
        [HttpGet("DownloadTemplate")]
        public IActionResult DownloadTemplate()
        {
            var filepath = System.IO.Path.Combine(env.WebRootPath, "template", "QUESTIONNAIRE_TEMPLATE.xlsx");
            var fs = new FileStream(filepath, FileMode.Open);
            return File(fs, "application/octet-stream", "QUESTIONNAIRE_TEMPLATE.xlsx");
        }
        [HttpPost("SaveMaterial")]
        public Materials SaveMaterial(Materials data)
        {
            using var db = ctx.CreateDbContext();
            db.Materials.Add(data);
            db.SaveChanges();
            return data;
        }
        [HttpPost("UploadMaterialAttachment")]
        public IActionResult UploadMaterialAttachment()
        {
            try
            {
                var id = Request.Headers.Where(x => x.Key == "id").Select(x => x.Value).FirstOrDefault().ToString();
                var dir = System.IO.Path.Combine(env.WebRootPath, "reading_materials", id);
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var origFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fileName = $"{origFileName}";
                        fileName = WebUtility.HtmlEncode(fileName);
                        var fullPath = System.IO.Path.Combine(dir, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
            return Ok();
        }
        [HttpGet("GetMaterialAttachments/{MaterialId}")]
        public List<MaterialAttachmentViewModel> GetMaterialAttachments(string MaterialId)
        {
            var dir = System.IO.Path.Combine(env.WebRootPath, "reading_materials", MaterialId);
            var rs = new List<MaterialAttachmentViewModel>();
            foreach (var f in System.IO.Directory.GetFiles(dir))
            {
                rs.Add(new MaterialAttachmentViewModel()
                {
                    FileName = System.IO.Path.GetFileName(f)
                });
            }
            return rs;
        }
        [HttpGet("GetMaterials")]
        public List<MaterialsViewModel> GetMaterials()
        {
            using var db = ctx.CreateDbContext();
            var list = db.Materials.Where(x => x.IsRemoved == 0).OrderByDescending(x=>x.DateCreated).ToList();
            var rs = new List<MaterialsViewModel>();
            foreach (var i in list)
            {
                var dir = System.IO.Path.Combine(env.WebRootPath, "reading_materials", i.Id.ToString());
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                var files = System.IO.Directory.GetFiles(dir);
                rs.Add(new MaterialsViewModel()
                {
                    Materials = i,
                    NumberOfAttachement = files.Count(),
                    Instructor = db.UserAccounts.Where(x => x.Id == i.UserId).FirstOrDefault()
                });
            }
            return rs;
        }
        [HttpGet("GetUserAccounts")]
        public List<UserAccount> GetUserAccounts()
        {
            using var db = ctx.CreateDbContext();
            return db.UserAccounts.Where(x => x.InActive == 0).ToList();
        }
        [HttpGet("GetUser/{userId}")]
        public UserAccount GetUser(string userId)
        {
            using var db = ctx.CreateDbContext();
            var id = Guid.Parse(userId);
            return db.UserAccounts.Where(x => x.Id == id).FirstOrDefault();
        }
        [HttpPost("SaveNewChat")]
        public UserChat SaveNewChat(UserChat data)
        {
            using (var db = ctx.CreateDbContext())
            {
                db.UserChats.Add(data);
                db.SaveChanges();
            }

            return data;
        }
        [HttpGet("GetChats")]
        public List<UserChat> GetChats()
        {
            var list = new List<UserChat>();
            using (var db = ctx.CreateDbContext())
            {
                list = db.UserChats.OrderByDescending(x => x.DateCreated).Take(100).ToList();
            }
            return list;
        }
        [HttpGet("GetNewChat/{chatId}")]
        public UserChat GetNewChat(string chatId)
        {
            using var db = ctx.CreateDbContext();
            var _id = Guid.Parse(chatId);
            return db.UserChats.Where(x => x.Id == _id).FirstOrDefault();
            
        }
        [HttpGet("GetTopStudentList")]
        public List<ExamRecord> GetTopStudentList()
        {
            using var db = ctx.CreateDbContext();
            return db.ExamRecords.OrderByDescending(x => x.Score).Take(3).Include(x => x.Student.Account).ToList();
        }
        [HttpGet("GetStudentExamRecord/{guid}")]
        public List<ExamRecord> GetStudentExamRecord(string guid)
        {
            using var db = ctx.CreateDbContext();
            var _id = Guid.Parse(guid);
            return db.ExamRecords.Include(x => x.Student.Account).Where(x => x.Student.Account.Id == _id).Include(x => x.ExamData.ExamQuestions).ToList();
        }
        [HttpPost("UpdateExamRecords")]
        public ExamRecord UpdateExamRecords(ExamRecord data)
        {
            using var db = ctx.CreateDbContext();
            db.ExamRecords.Update(data);
            db.SaveChanges();
            return data;
        }
        [HttpGet("GetAddedQuestionsGuid/{examRecordId}")]
        public List<ExamStudentRecord> GetAddedQuestionsGuid(string examRecordId)
        {
            using var db = ctx.CreateDbContext();
            var _id = Guid.Parse(examRecordId);
            return db.ExamStudentRecord.Where(x => x.ExamRecordId == _id).ToList();
        }
        [HttpPost("SaveStudentRecord")]
        public IActionResult SaveStudentRecord(ExamStudentRecord data)
        {
            using var db = ctx.CreateDbContext();
            var isExist = db.ExamStudentRecord.Any(x => x.ExamRecordId == data.ExamRecordId && x.QuestionId == data.QuestionId);
            if (!isExist)
            {
                db.ExamStudentRecord.Add(data);
                db.SaveChanges();
            }
            /* db.Entry(data).State = EntityState.Modified;
             db.SaveChanges();
             db.Entry(data).State = EntityState.Detached;
             foreach (var i in data.ExamStudentRecords)
             {
                 var isExist = db.ExamStudentRecord.Include(x => x.Question).Any(x => x.Question.Id == i.Question.Id);
                 if (!isExist)
                 {
                     db.ExamStudentRecord.Add(i);
                     db.SaveChanges();
                 }
             }
             db.SaveChanges();*/
            /*db.Entry(data.ExamRecord).State = EntityState.Modified;
            db.SaveChanges();
            db.Entry(data.ExamRecord).State = EntityState.Detached;
            db.SaveChanges();

            var exrec = db.ExamRecords.Include(x=>x.ExamStudentRecords).ThenInclude(x=>x.Question).Where(x => x.Id == data.ExamRecord.Id).FirstOrDefault();
            if (exrec.ExamStudentRecords == null) exrec.ExamStudentRecords = new List<ExamStudentRecord>();
            var isExist = db.ExamStudentRecord.Any(x => x.Question.Id == data.ExamStudentRecord.Question.Id);
            if (!isExist)
            {
                exrec.ExamStudentRecords.Add(data.ExamStudentRecord);
                db.Entry(exrec).State = EntityState.Modified;
                db.ExamStudentRecord.Add(data.ExamStudentRecord);
                db.SaveChanges();
            }*/
            return Ok(1);
        }

        [HttpGet("GetRegisteredStudentDetail/{userId}")]
        public RegisteredStudent GetRegisteredStudentDetail(string userId)
        {
            using var db = ctx.CreateDbContext();
            var _userId = Guid.Parse(userId);
            return db.RegisteredStudents.Include(x => x.Account).Where(x => x.Account.Id == _userId && x.Account.UserType == "STUDENT").FirstOrDefault();
        }
        [HttpPost("SaveExamRecord")]
        public ExamRecord SaveExamRecord(ExamRecord data)
        {
            using var db = ctx.CreateDbContext();
            db.Update(data.ExamData);
            db.Update(data.Student);
            db.ExamRecords.Add(data);
            db.SaveChanges();
            return db.ExamRecords.Where(x => x.Id == data.Id).Include(x => x.ExamData).ThenInclude(x => x.ExamQuestions).Include(x => x.Student).ThenInclude(x => x.Account).FirstOrDefault();
        }
        [HttpGet("GetExamRecord/{examId}/{userId}")]
        public IActionResult GetExamRecord(string examId, string userId)
        {
            using var db = ctx.CreateDbContext();
            var _userId = Guid.Parse(userId);
            var _examId = Guid.Parse(examId);
            var data = db.ExamRecords.Include(x => x.ExamData).Include(x => x.Student).ThenInclude(x => x.Account).Where(x => x.Student.Account.Id == _userId && x.ExamData.Id == _examId).ToList();
            if (data != null) return Ok(data);

            return Ok(0);
        }
        [HttpGet("GetExamDetails/{examId}")]
        public Exam GetExamDetails(string examId)
        {
            using var db = ctx.CreateDbContext();
            var _id = Guid.Parse(examId);
            var rs = db.Exams.Where(x => x.Id == _id).Include(x => x.ExamQuestions).ThenInclude(x => x.QuestionData).FirstOrDefault();
            return rs;
        }
        [HttpGet("AvailableExams")]
        public List<Exam> AvailableExams()
        {
            using var db = ctx.CreateDbContext();
            return db.Exams.Where(x => x.IsLive == 1).Include(x => x.ExamQuestions).ThenInclude(x => x.QuestionData).OrderByDescending(x => x.DateCreated).ToList();
        }
        [HttpGet("GetMyExamRecords/{studentId}")]
        public IActionResult ExamRecords(string studentId)
        {
            using var db = ctx.CreateDbContext();
            var result = 1;
            var _id = Guid.Parse(studentId);
            var rs = db.ExamRecords.Include(x => x.ExamData).ThenInclude(x => x.ExamQuestions).Include(x => x.Student).ThenInclude(x => x.Account).Where(x => x.Student.Account.Id == _id).OrderByDescending(x => x.DateTaken).ToList();
            if (rs != null) return Ok(rs);
            return Ok(result);
        }
        [HttpPost("PublishExam")]
        public Exam PublishExam(Exam data)
        {
            using var db = ctx.CreateDbContext();
            data.IsLive = 1;
            db.Update(data);
            db.SaveChanges();
            return data;
        }
        [HttpPost("RemoveExam")]
        public Exam RemoveExam(Exam data)
        {
            using var db = ctx.CreateDbContext();
            data.IsLive = 2;
            db.Update(data);
            db.SaveChanges();
            return data;
        }
        [HttpPost("AddExamQuestion")]
        public Exam AddExamQuestion(ExamViewModel d)
        {
            using var db = ctx.CreateDbContext();
            var json = JsonConvert.SerializeObject(d);
            Console.WriteLine(json);
            var exam = db.Exams.Include(x => x.ExamQuestions).ThenInclude(x => x.QuestionData).Where(x => x.Id == d.ExamId).FirstOrDefault();
            if (exam != null)
            {
                foreach (var q in d.ListOfQuestions)
                {
                    if (exam.ExamQuestions == null)
                    {
                        exam.ExamQuestions = new List<ExamQuestion>();
                        db.ExamQuestions.AddRange(exam.ExamQuestions);
                        db.SaveChanges();
                    }
                    var da = db.ExamQuestions.Where(x => x.QuestionData.Id == q.Id && exam.ExamQuestions.Select(x => x.Id).Contains(x.Id)).FirstOrDefault();
                    if (da == null)
                    {
                        da = new ExamQuestion();
                        da.QuestionData = q;
                        da.DateAdded = DateTime.Now;
                        exam.ExamQuestions.Add(da);
                        db.Update(exam);
                        db.SaveChanges();
                    }

                }
            }

            return exam;
        }
        [HttpPost("RemoveExamQuestion")]
        public ExamQuestion RemoveExamQuestion(ExamQuestion data)
        {
            using var db = ctx.CreateDbContext();
            db.ExamQuestions.Remove(data);
            db.SaveChanges();
            return data;
        }
        [HttpPost("UpdateExam")]
        public Exam UpdateExam(Exam data)
        {
            using var db = ctx.CreateDbContext();
            db.Exams.Update(data);
            db.ExamQuestions.AddRange(data.ExamQuestions);
            db.SaveChanges();
            return data;
        }

        [HttpPost("AddExam")]
        public Exam AddExam(Exam data)
        {
            using var db = ctx.CreateDbContext();
            data.DateCreated = DateTime.Now;
            db.Exams.Add(data);
            db.SaveChanges();
            return data;
        }
        [HttpGet("GetExams")]
        public List<Exam> GetExams()
        {
            using var db = ctx.CreateDbContext();
            return db.Exams.Include(x => x.ExamQuestions).ThenInclude(x => x.QuestionData).OrderByDescending(x => x.DateCreated).ToList();
        }
        [HttpPost("Account/SignIn")]
        public IActionResult AccountSignIn(UserAccount user)
        {
            using(var db = ctx.CreateDbContext())
            {
                bool isExist = false;
                var u = db.UserAccounts.Where(x => x.Username == user.Username && x.InActive == 0).FirstOrDefault();
                if (u != null)
                {
                    isExist = u.Password.Equals(user.Password, StringComparison.Ordinal);
                    if (u.UserType == "STUDENT")
                    {
                        var info = db.RegisteredStudents.Include(x => x.Account).Where(x => x.Account.Id == u.Id).FirstOrDefault();
                        if (info.IsAccepted == 0)
                            isExist = false;
                    }
                    if (isExist)
                    {
                        u.LastLoggedIn = DateTime.Now;
                        db.UserAccounts.Update(u);
                        db.SaveChanges();
                        return Ok(u);
                    }
                }
            }
           
            return Unauthorized();
        }


        [HttpGet("AcceptStudent/{Id}")]
        public void AcceptDeny(string Id)
        {
            using var db = ctx.CreateDbContext();
            var _Id = Guid.Parse(Id);
            var acc = db.RegisteredStudents.Where(x => x.Id == _Id).FirstOrDefault();
            acc.IsAccepted = 1;
            db.RegisteredStudents.Update(acc);
            db.SaveChanges();
        }
        [HttpGet("DenyStudent/{Id}")]
        public void DenyStudent(string Id)
        {
            using var db = ctx.CreateDbContext();
            var _Id = Guid.Parse(Id);
            var acc = db.RegisteredStudents.Include(x => x.Account).Where(x => x.Id == _Id).FirstOrDefault();
            db.RegisteredStudents.Remove(acc);
            db.UserAccounts.Remove(acc.Account);
            db.SaveChanges();
        }
        [HttpGet("GetSubjects")]
        public List<Subject> GetSubjects()
        {
            using var db = ctx.CreateDbContext();
            return db.Subjects.Include(x => x.ComponentSubjects).ThenInclude(x => x.Questions).ToList();
        }
        [HttpGet("GetRegisteredStudent")]
        public List<RegisteredStudent> GetRegisteredStudent()
        {
            using var db = ctx.CreateDbContext();
            return db.RegisteredStudents.Include(x => x.Account).Where(x=>x.Account.UserType == "STUDENT").OrderBy(x => x.Account.LastName).ToList();
        }
        [HttpGet("GetRegisteredStudent/{examid}")]
        public List<RegisteredStudent> GetRegisteredStudent(string examid)
        {
            using var db = ctx.CreateDbContext();
            var _id = Guid.Parse(examid);
            var examrecords = db.ExamRecords.Include(x => x.ExamData).Where(x => x.ExamData.IsLive == 1 && x.ExamData.Id == _id).Include(x => x.Student).ThenInclude(x => x.Account).ToList();
            return examrecords.Select(x => x.Student).ToList();
        }
        [HttpGet("GetPendingStudent")]
        public List<RegisteredStudent> GetPendingStudent()
        {
            using var db = ctx.CreateDbContext();
            return db.RegisteredStudents.Where(x => x.IsAccepted == 0).Take(5).Include(x => x.Account).OrderByDescending(x => x.Account.DateRegistered).ToList();
        }
        [HttpGet("GetNumberOfPeding")]
        public NotificationModel GetNumberOfPeding()
        {
            using var db = ctx.CreateDbContext();
            var notif = new NotificationModel();
            notif.Count = db.RegisteredStudents.Where(x => x.IsAccepted == 0).Count();
            notif.Date = DateTime.Now;
            return notif;
        }
        [HttpPost("AddStudent")]
        public int AddStudent(RegisteredStudent data)
        {
            using var db = ctx.CreateDbContext();
            var rs = 0;
            var IsUsernameExist = db.UserAccounts.Any(x => x.Username == data.Account.Username);
            var IsEmailExist = db.UserAccounts.Any(x => x.Email == data.Account.Email);
            if (IsUsernameExist)
            {
                rs = 100;
            }
            else if (IsEmailExist)
            {
                rs = 200;
            }
            else
            {
                data.Account.UserType = "STUDENT";
                db.RegisteredStudents.Add(data);
                db.SaveChanges();
            }
            return rs;
        }

        [HttpPost("UpdateQuestion")]
        public Question UpdateQuestion(Question data)
        {
            using var db = ctx.CreateDbContext();
            db.Questions.Update(data);
            db.SaveChanges();
            return data;
        }

        [HttpPost("UploadExcel")]
        public IActionResult UploadExcel()
        {
            using var db = ctx.CreateDbContext();
            try
            {
                var dir = System.IO.Path.Combine(env.WebRootPath, "tmp");
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var origFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileName = $"{Guid.NewGuid()}_{origFileName}";
                    fileName = WebUtility.HtmlEncode(fileName);
                    var fullPath = System.IO.Path.Combine(dir, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Workbook wb = new Workbook();
                    wb.LoadFromFile(fullPath);
                    Worksheet wsheet = wb.Worksheets[0];
                    var result = new UploadResult();
                    result.OrigFileName = origFileName;
                    result.FileName = fileName;
                    result.FilePath = fullPath;
                    result.StrSize = Helper.SizeSuffix(file.Length);
                    result.RowCount = wsheet.Rows.Length - 1;
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("SaveExcel/{filename}/{componentSubjectId}")]
        public List<Subject> SaveExcel(string filename, string componentSubjectId)
        {
            using var db = ctx.CreateDbContext();
            var _Id = Guid.Parse(componentSubjectId);
            var component = db.ComponentSubjects.Include(x => x.Questions).Where(x => x.Id == _Id).FirstOrDefault();
            var dir = System.IO.Path.Combine(env.WebRootPath, "tmp");
            var filePath = System.IO.Path.Combine(dir, filename);
            Workbook wb = new Workbook();
            wb.LoadFromFile(filePath);
            Worksheet wsheet = wb.Worksheets[0];
            for (var i = 1; i < wsheet.Rows.Length; i++)
            {
                var row = wsheet.Rows[i];
                var rowNumber = row.Columns[0].DisplayedText;
                var rowQuestion = row.Columns[1].DisplayedText;
                var rowChoice1 = row.Columns[2].DisplayedText;
                var rowChoice2 = row.Columns[3].DisplayedText;
                var rowChoice3 = row.Columns[4].DisplayedText;
                var rowChoice4 = row.Columns[5].DisplayedText;
                var rowCorrectAnswer = row.Columns[6].DisplayedText;
                var q = new Question();
                q.MyQuestion = rowQuestion?.ToString();
                q.Choice1 = rowChoice1?.ToString();
                q.Choice2 = rowChoice2?.ToString();
                q.Choice3 = rowChoice3?.ToString();
                q.Choice4 = rowChoice4?.ToString();
                q.CorrectAnswer = rowCorrectAnswer?.ToString();
                if (!string.IsNullOrEmpty(q.CorrectAnswer))
                {
                    if (rowCorrectAnswer.Contains("1")) q.CorrectAnswer = rowChoice1?.ToString();
                    if (rowCorrectAnswer.Contains("2")) q.CorrectAnswer = rowChoice2?.ToString();
                    if (rowCorrectAnswer.Contains("3")) q.CorrectAnswer = rowChoice3?.ToString();
                    if (rowCorrectAnswer.Contains("4")) q.CorrectAnswer = rowChoice4?.ToString();
                }
                component.Questions.Add(q);
            }
            db.ComponentSubjects.Update(component);
            db.SaveChanges();
            System.IO.File.Delete(filePath);
            return GetSubjects();
        }

        void AddInitialData()
        {
            using var db = ctx.CreateDbContext();
            var IsAdminExist = db.UserAccounts.Any(x => x.Username == "superadmin");
            if (!IsAdminExist)
            {
                var admin = new UserAccount();
                admin.Username = "superadmin";
                admin.Password = "@Admin1234";
                admin.FirstName = "Super";
                admin.LastName = "Admin";
                admin.UserType = "ADMINISTRATOR";
                admin.Email = "cadelina13@gmail.com";
                admin.DateRegistered = DateTime.Now;
                db.UserAccounts.Add(admin);
                db.SaveChanges();
            }
            else
            {
                var adminCount = db.UserAccounts.Where(x => x.Username == "superadmin").Count();
                if(adminCount > 1)
                {
                    db.UserAccounts.Remove(db.UserAccounts.Where(x => x.Username == "superadmin").FirstOrDefault());
                    db.SaveChanges();
                }
            }


            var dConfig = db.DatabaseConfigs.FirstOrDefault();
            if (dConfig == null)
            {
                dConfig = new DatabaseConfig();
                dConfig.InitialDataLoaded = 0;
                db.DatabaseConfigs.Add(dConfig);
                db.SaveChanges();
            }

            if (dConfig.InitialDataLoaded == 0)
            {
                var cs1 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Introduction to Criminology"},
                    new ComponentSubject(){ Name = "Theories of Crime Causation"},
                    new ComponentSubject(){ Name = "Professional Conduct and Ethical Standards"},
                    new ComponentSubject(){ Name = "Human Behavior and Victimology"},
                    new ComponentSubject(){ Name = "Dispute Resolution and Crises/Incidents Management"},
                    new ComponentSubject(){ Name = "Juvenile Delinquency & Juvenile Justice System"},
                    new ComponentSubject(){ Name = "Criminological Research 1 & 2"},
                    new ComponentSubject(){ Name = "Character Formation 1 Nationalism & Patriotism "},
                };
                var s1 = new Subject() { Name = "Criminology", Percentage = 20, ComponentSubjects = cs1 };

                var cs2 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Introduction to Philippine Criminal Justice System"},
                    new ComponentSubject(){ Name = "Criminal Law Book 1 RPC"},
                    new ComponentSubject(){ Name = "Human Rights Education"},
                    new ComponentSubject(){ Name = "Criminal Law Book 2 RPC"},
                    new ComponentSubject(){ Name = "Evidence"},
                    new ComponentSubject(){ Name = "Criminal Procedure and Court Testimony"},
                };
                var s2 = new Subject() { Name = "Criminal Law & Jurisprudence", Percentage = 20, ComponentSubjects = cs2 };

                var cs3 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Fundamentals of Criminal Investigation and Intelligence"},
                    new ComponentSubject(){ Name = "Specialized Crime Investigation 1 with Legal Medicine"},
                    new ComponentSubject(){ Name = "Traffic Management and Accident Investigation with Driving"},
                    new ComponentSubject(){ Name = "Special Crime Investigation 2"},
                    new ComponentSubject(){ Name = "Fire Technology and Arson Investigation"},
                    new ComponentSubject(){ Name = "Drug Education and Vice Control"},
                    new ComponentSubject(){ Name = "Technical English 1 (Legal Forms)"},
                    new ComponentSubject(){ Name = "Technical English 2 (Investigative Report)"},
                    new ComponentSubject(){ Name = "Introduction to Cybercrime/Environmental Law Protection"},
                };
                var s3 = new Subject() { Name = "Crime detection & Investigation", Percentage = 20, ComponentSubjects = cs3 };

                var cs4 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Personal Identification Techniques"},
                    new ComponentSubject(){ Name = "Forensic Photography"},
                    new ComponentSubject(){ Name = "Forensic Chemistry and Technology"},
                    new ComponentSubject(){ Name = "Questioned Documents Examination"},
                    new ComponentSubject(){ Name = "Lie Detection Techniques"},
                    new ComponentSubject(){ Name = "Forensic Ballistics"},
                };

                var s4 = new Subject() { Name = "Forensic Science", Percentage = 15, ComponentSubjects = cs4 };

                var cs5 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Law Enforcement Organization and Administration (Inter-Agency Approach)"},
                    new ComponentSubject(){ Name = "Comparative Models of Policing"},
                    new ComponentSubject(){ Name = "Introduction to Industrial Security Concepts"},
                    new ComponentSubject(){ Name = "Law Enforcement Planning and Operations with Crime Mapping"},
                    new ComponentSubject(){ Name = "Character Formation 2 (Leadership/Decision Making/Management and Administration)"},
                };
                var s5 = new Subject() { Name = "Law Enforcement Administration", Percentage = 15, ComponentSubjects = cs5 };

                var cs6 = new List<ComponentSubject>()
                {
                    new ComponentSubject(){ Name = "Institutional Corrections"},
                    new ComponentSubject(){ Name = "Non-Institutional Corrections"},
                    new ComponentSubject(){ Name = "Therapeutic Modalities"},
                };
                var s6 = new Subject() { Name = "Correctional Administration", Percentage = 10, ComponentSubjects = cs6 };

                db.Subjects.AddRange(s1, s2, s3, s4, s5, s6);
                db.ComponentSubjects.AddRange(cs1);
                db.ComponentSubjects.AddRange(cs2);
                db.ComponentSubjects.AddRange(cs3);
                db.ComponentSubjects.AddRange(cs4);
                db.ComponentSubjects.AddRange(cs5);
                db.ComponentSubjects.AddRange(cs6);
                dConfig.InitialDataLoaded = 1;
                db.DatabaseConfigs.Update(dConfig);
                db.SaveChanges();
            }
        }

    }
}
