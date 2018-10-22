using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RemoteLoggingService.Controllers;
using RemoteLoggingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RemoteLoggingServiceTest
{
    public class UsersControllerTests
    {       
        public UsersControllerTests()
        {
            
            
        }
       
        [Fact]
        public async Task Test1()
        {            
            var userList = new[]
            {
                new User()
                {
                    Name = "јртем якутин",
                    Email =  "artem-av-st@yandex.ru",
                    IsApproved = true,                                        
                    UserRole = new UserRole()
                    {
                        Name = "Administrator",
                        Id = 3
                    }
                }
            };
            var mockedContext = new Mock<IRepository>();
            //mockedContext.Setup(x => x.Users).ReturnsDbSet(userList);
            UsersController controller = new UsersController(mockedContext.Object);
            var result = await controller.Index();
            var list = ((JsonResult)result).Value as List<User>;
                        
            Assert.Equal("јртем якутин", list[0].Name);
            Assert.Equal("artem-av-st@yandex.ru", list[0].Email);
            Assert.Equal(3, list[0].UserRole.Id);
            Assert.Equal("Administrator", list[0].UserRole.Name);
        }
    }
}
