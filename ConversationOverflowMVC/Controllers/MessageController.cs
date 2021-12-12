using ConversationOverflowMVC.Dto;
using ConversationOverflowMVC.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Classes;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly HttpClient _httpClientConversationOverflowAPI;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConversationOverflowAPI _conversationOverflowAPI;
        private readonly Dictionary<string, string> _contentType = new Dictionary<string, string>
        {
            { ".txt", "text/plain"},
            { ".pdf", "application/pdf" },
            { ".doc", "application/vnd.ms-word" },
            { ".docx", "application/vnd.ms-word" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheet.sheet" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".csv", "text/csv" }
        };
        [ViewData]
        public bool IsAuthenticated { get; set; }
        [ViewData]
        public string AuthenticatedUser { get; set; }
        public MessageController(IConversationOverflowAPI conversationOverflowAPI,
             IWebHostEnvironment hostingEnvironment)
        {
            _conversationOverflowAPI = conversationOverflowAPI;
            _httpClientConversationOverflowAPI = conversationOverflowAPI.Initial();
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> _ListMessage(int groupId, int startId, int count)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Message/RangeAt/" + groupId + "/" + startId + "/" + count);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<MessageDto> messages = await httpResponseMessage.Content.ReadFromJsonAsync<List<MessageDto>>();

                    return PartialView("_ListMessage", messages);
                }
                else return PartialView("_NotFoundMessage", httpResponseMessage.IsSuccessStatusCode);
            }
            else return PartialView("_NotFoundMessage", false);
        }

        [HttpGet]
        public async Task<IActionResult> _ListMessageLast(int groupId, int interval)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("Message/" + groupId + "/" + interval + "/0");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    List<MessageDto> messages = await httpResponseMessage.Content.ReadFromJsonAsync<List<MessageDto>>();

                    return PartialView("_ListMessage", messages);
                }
                else return PartialView("_NotFoundMessage", httpResponseMessage.IsSuccessStatusCode);
            }
            else return PartialView("_NotFoundMessage", false);
        }

        [HttpGet]
        public async Task<IActionResult> _Message(string login, string text)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/login/" + login);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    MessageDto messageDto = new MessageDto()
                    {
                        Message = new Message()
                        {
                            Text = text,
                            CreationTime = DateTime.Now
                        },
                        UserId = user.Id,
                        UserName = user.FirstName + " " + user.LastName,
                        Login = user.Login,
                        ImagePath = user.ImagePath
                    };

                    return PartialView("_Message", messageDto);
                }
                else return PartialView("_NotFoundMessage", httpResponseMessage.IsSuccessStatusCode);
            }
            else return PartialView("_NotFoundMessage", false);
        }

        [HttpGet]
        public async Task<IActionResult> _Attachment(string login, string attachment)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                    await _httpClientConversationOverflowAPI.GetAsync("User/login/" + login);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                    MessageDto messageDto = new MessageDto()
                    {
                        Message = new Message()
                        {
                            Text = attachment,
                            CreationTime = DateTime.Now
                        },
                        Attachment = new Attachment(),
                        UserId = user.Id,
                        UserName = user.FirstName + " " + user.LastName,
                        Login = user.Login,
                        ImagePath = user.ImagePath
                    };

                    return PartialView("_Message", messageDto);
                }
                else return PartialView("_NotFoundMessage", httpResponseMessage.IsSuccessStatusCode);
            }
            else return PartialView("_NotFoundMessage", false);
        }

        [HttpPost]
        public async Task<bool> SendTextMessageToGroup(int groupId, string text)
        {
            if (ReloadHttpClient().Result)
            {
                SendDto sendDto = new SendDto()
                {
                    UserMessage = new UserMessage()
                    {
                        GroupId = groupId
                    },
                    Message = new Message()
                    {
                        Text = text,
                        CreationTime = DateTime.Now
                    }
                };

                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PostAsJsonAsync<SendDto>("Message/Send", sendDto);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    bool result = await httpResponseMessage.Content.ReadFromJsonAsync<bool>();

                    return result;
                }
                else return false;
            }
            else return false;
        }

        [HttpPost]
        public async Task<int> SendTextMessageToUser(int userId, string text)
        {
            if (ReloadHttpClient().Result)
            {
                UserIdDto userIdDto = new UserIdDto()
                {
                    UserId = userId
                };

                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.PostAsJsonAsync<UserIdDto>("Group/AddPrivateGroup", userIdDto);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Group group = await httpResponseMessage.Content.ReadFromJsonAsync<Group>();

                    if (group != null)
                    {
                        bool result = await SendTextMessageToGroup(group.Id, text);
                        if (result) return group.Id;
                        else return 0;
                    }
                    else return 0;
                }
                else return 0;
            }
            else return 0;
        }

        [HttpPost]
        public async Task<bool> SendAttachment(IFormFile file, int groupId)
        {
            if (ReloadHttpClient().Result)
            {
                if (file != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Content");
                    uploadFolder = Path.Combine(uploadFolder, groupId.ToString());
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var form = new MultipartFormDataContent())
                    {
                        using (var fs = file.OpenReadStream())
                        {
                            using (var streamContent = new StreamContent(fs))
                            {
                                using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
                                {
                                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

                                    form.Add(fileContent, "file", Path.GetFileName(filePath));
                                    form.Add(new StringContent(filePath, Encoding.UTF8, "text/xml"), "filepath");

                                    if (ReloadHttpClient().Result)
                                    {
                                        HttpResponseMessage httpResponseMessage = await _httpClientConversationOverflowAPI.PostAsync("Message/Attachment", form);

                                        if (httpResponseMessage.IsSuccessStatusCode)
                                        {
                                            if (ReloadHttpClient().Result)
                                            {
                                                SendDto sendDto = new SendDto()
                                                {
                                                    UserMessage = new UserMessage()
                                                    {
                                                        GroupId = groupId
                                                    },
                                                    Message = new Message()
                                                    {
                                                        Text = file.FileName,
                                                        CreationTime = DateTime.Now
                                                    },
                                                    Attachment = new Attachment()
                                                    {
                                                        Path = "Content/" + groupId + "/" + fileName,
                                                        CreationTime = DateTime.Now
                                                    }
                                                };

                                                httpResponseMessage =
                                                    await _httpClientConversationOverflowAPI.PostAsJsonAsync<SendDto>("Message/Send", sendDto);

                                                if (httpResponseMessage.IsSuccessStatusCode)
                                                {
                                                    bool result = await httpResponseMessage.Content.ReadFromJsonAsync<bool>();

                                                    return result;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int groupId, int messageId)
        {
            if (ReloadHttpClient().Result)
            {
                HttpResponseMessage httpResponseMessage =
                        await _httpClientConversationOverflowAPI.GetAsync("Message/Attachment/" + groupId + "/" + messageId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Attachment attachment = await httpResponseMessage.Content.ReadFromJsonAsync<Attachment>();

                    if (attachment != null)
                    {
                        string path = Path.Combine(_hostingEnvironment.ContentRootPath, attachment.Path);
                        var memory = new MemoryStream();
                        using (var stream = new FileStream(path, FileMode.Open))
                        {
                            await stream.CopyToAsync(memory);
                        }
                        memory.Position = 0;
                        string contentType = _contentType.GetValueOrDefault(Path.GetExtension(attachment.Path));

                        return File(memory, contentType, Path.GetFileName(path));
                    }
                    else return null;
                }
                else return null;
            }

            return null;
        }

        private async Task<bool> ReloadHttpClient()
        {
            UserDto userDto = new UserDto()
            {
                Login = User?.Identity?.Name,
                PasswordHash = GetPasswordHash().Result
            };
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.PostAsJsonAsync<UserDto>("User/ReloadHttpClient", userDto);

            return httpResponseMessage.IsSuccessStatusCode;
        }

        private async Task<string> GetPasswordHash()
        {
            HttpResponseMessage httpResponseMessage =
                await _httpClientConversationOverflowAPI.GetAsync("User/login/" + User?.Identity?.Name);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();

                if (user.Location == null) user.Location = new Location();

                return user.PasswordHash;
            }

            return "";
        }
    }
}
