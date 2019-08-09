using Domain.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Services.Email;
using Services.Email.Models;
using Services.Modules;
using System.IO;

namespace Services.Tests
{
    [TestClass]
    public class EmailWorkerTest
    {
        [TestMethod]
        public void Send_Test()
        {
            // arrange
            var kernel = new StandardKernel(new Domain.Modules.Module(TestEnvironment: true), new Modules.Module(TestEnvironment: true));
            var email = kernel.Get<IEmailWorker>();

            var emailModel = new EmailModel
            {
                Title = "UnitTesting with this email",
                RecipientDisplayName = "Alberto",
                RecipientEmail = "alberto@dummy.com",
                QuoteMessage = "Nothing to relate in this email, just a dummy recover password test test. Thanks!"
            };

            // act
            var htmlString = "<div style='width: 100vw; min-width: 700px;'>" +
                "   <h1>@Model.Title</h1>" +
                "   <hr />" +
                "   <div>Dear <b>@Model.RecipientDisplayName</b>,</div>" +
                "   <div style='display: flex; align-items: baseline; flex-direction: column; justify-content: space-between; " +
                "       max-width: 500px;margin: 0 auto; background-color: #b7b7b7; color:#2e2e2e'>" +
                "       <blockquote>@Model.ContentMessage</blockquote>" +
                "       <em>Thanks in advance, John.</em>" +
                "       <hr />" +
                "       <address>" +
                "           Visit us at:<br>" +
                "           Example.com<br>" +
                "           Box 564, Disneyland<br>" +
                "           USA" +
                "       </address>" +
                "   </div>" +
                "</div>";

            email.Send(htmlString, emailModel);

            // assert
            var generatedFilePath = (email.Smtp() as FakeSmtp).FilePath + emailModel.Title + ".html";
            Assert.IsTrue(File.Exists(generatedFilePath));
            Assert.AreEqual(htmlString, File.ReadAllText(generatedFilePath));
        }
    }
}
