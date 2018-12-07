using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;
using Thrzn41.WebexTeams.Version1.OAuth2;
using Thrzn41.Util;
using System.Net;
using System.Threading;

namespace UnitTest.DotNetCore.Thrzn41.WebexTeams
{
    [TestClass]
    public class UnitTestBasicWithRetry
    {
        private const string UNIT_TEST_SPACE_TAG = "#webexteamsapiclientunittestspace";

        private const string UNIT_TEST_CREATED_PREFIX = "#createdwebexteamsapiclientunittest";

        TeamsAPIClient teams;

        Space unitTestSpace;

        [TestInitialize]
        public async Task Init()
        {

            byte[] encryptedToken;
            byte[] entropy;

            Person me = null;

            try
            {

                string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                var dirInfo = new DirectoryInfo(String.Format("{0}{1}.thrzn41{1}unittest{1}teams", userDir, Path.DirectorySeparatorChar));

                using (var stream = new FileStream(String.Format("{0}{1}teamstoken.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);

                    encryptedToken = memory.ToArray();
                }

                using (var stream = new FileStream(String.Format("{0}{1}tokenentropy.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);

                    entropy = memory.ToArray();
                }

                teams = TeamsAPI.CreateVersion1Client(LocalProtectedString.FromEncryptedData(encryptedToken, entropy), RetryExecutor.One);
            
                var rMe = await teams.GetMeAsync();

                if (rMe.IsSuccessStatus)
                {
                    me = rMe.Data;

                    var e = (await teams.ListSpacesAsync(type: SpaceType.Group, sortBy: SpaceSortBy.Created, max: 50)).GetListResultEnumerator();

                    while (await e.MoveNextAsync())
                    {
                        var r = e.CurrentResult;

                        if (r.IsSuccessStatus && r.Data.HasItems)
                        {
                            foreach (var item in r.Data.Items)
                            {
                                if (item.Title.Contains(UNIT_TEST_SPACE_TAG))
                                {
                                    unitTestSpace = item;
                                    break;
                                }
                            }
                        }

                        if (unitTestSpace != null)
                        {
                            break;
                        }
                    }
                }

            }
            catch (DirectoryNotFoundException) { }


            checkTeamsAPIClient(me);
            checkUnitTestSpace();
        }


        [TestCleanup]
        public async Task Clean()
        {
            if(this.teams == null)
            {
                return;
            }

            var e = (await teams.ListSpacesAsync(type: SpaceType.Group, sortBy: SpaceSortBy.Created, max: 50)).GetListResultEnumerator();

            while (await e.MoveNextAsync())
            {
                var r = e.CurrentResult;

                if (r.IsSuccessStatus && r.Data.HasItems)
                {
                    foreach (var item in r.Data.Items)
                    {
                        if (item.Title.Contains(UNIT_TEST_CREATED_PREFIX))
                        {
                            await teams.DeleteSpaceAsync(item);
                        }
                    }
                }
            }

        }

        private void checkTeamsAPIClient(Person me)
        {
            if (me == null || String.IsNullOrEmpty(me.Id))
            {
                Assert.Fail("You need to configure Cisco Webex Teams Token by using UnitTestTool.EncryptWebexTeamsTokenForm.");
            }
        }

        private void checkUnitTestSpace()
        {
            if (this.unitTestSpace == null)
            {
                Assert.Fail("You need to create manually a Space for unit test that contains name '#webexteamsapiclientunittestspace' in Title, and add bot or integration user that is used in the test.");
            }
        }

        [TestMethod]
        public async Task TestCreateMessage()
        {
            var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, "Hello, Cisco Webex Teams!!");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);
            Assert.IsNotNull(r.TrackingId);

            Assert.AreNotEqual(Guid.Empty, r.TransactionId);

            Assert.AreEqual("POST /v1/messages HTTP/1.1", r.RequestLine);

            Assert.AreEqual("Hello, Cisco Webex Teams!!", r.Data.Text);

            var resourceOperation = r.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.Message, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.Create, resourceOperation.Operation);
            Assert.AreEqual("CreateMessage", resourceOperation.ToString());

        }

        [TestMethod]
        public async Task TestCreateMessageWithMarkdown()
        {
            var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, "Hello, **markdown**!!");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);
            Assert.IsNotNull(r.TrackingId);

            Assert.AreEqual("Hello, markdown!!", r.Data.Text);

        }

        [TestMethod]
        public void TestTextWithoutMention()
        {
            var m = TeamsData.FromJsonString<Message>("{\"id\": \"mid\",\"roomId\": \"sid\",\"roomType\": \"group\",\"text\": \"test\",\"personId\": \"pid\",\"personEmail\": \"mail@example.com\",\"html\": \"test\",\"created\": \"2018-06-01T05:51:39.879Z\"}");

            Assert.AreEqual("test", m.TextWithoutMention);

            var text = "test";
            m = TeamsData.FromJsonString<Message>(String.Format("{{\"id\": \"mid\",\"roomId\": \"sid\",\"roomType\": \"group\",\"text\": \"person {0}\",\"personId\": \"pid\",\"personEmail\": \"mail@example.com\",\"html\": \"<p><spark-mention data-object-type=\\\"person\\\" data-object-id=\\\"pid2\\\">person</spark-mention> {1}</p>\",\"mentionedPeople\": [\"pid2\"],\"created\": \"2018-06-01T05:51:39.879Z\"}}", text, WebUtility.HtmlEncode(text)));
            Assert.AreEqual(text, m.TextWithoutMention);

            text = "te<>&st";
            m = TeamsData.FromJsonString<Message>(String.Format("{{\"id\": \"mid\",\"roomId\": \"sid\",\"roomType\": \"group\",\"text\": \"person {0}\",\"personId\": \"pid\",\"personEmail\": \"mail@example.com\",\"html\": \"<p><spark-mention data-object-type=\\\"person\\\" data-object-id=\\\"pid2\\\">person</spark-mention> {1}</p>\",\"mentionedPeople\": [\"pid2\"],\"created\": \"2018-06-01T05:51:39.879Z\"}}", text, WebUtility.HtmlEncode(text)));
            Assert.AreEqual(text, m.TextWithoutMention);

        }

        [TestMethod]
        public async Task TestMarkdownBuilder()
        {
            var md = new MarkdownBuilder();

            md.AppendBold("Hello, Markdown").AppendLine().AppendFormat("12{0}4", 3);
            md.AppendParagraphSeparater();
            md.Append("Hi ").AppendMentionToPerson("xyz_person_id", "Person").Append(", How are you?");

            Assert.AreEqual("**Hello, Markdown**  \n1234\n\nHi <@personId:xyz_person_id|Person>, How are you?", md.ToString());
            md.Clear();


            md.Append("Hi ").AppendMentionToPerson("to@example.com", "Name<>&#'Person").Append(", How are you?");

            Assert.AreEqual("Hi <@personEmail:to@example.com|Name&lt;&gt;&amp;#&#39;Person>, How are you?", md.ToString());
            md.Clear();

            md.AppendBold("Hello, Bold");
            md.AppendLine();

            int a = 411522630;
            int b = 3;

            md.AppendBoldFormat("{0} * {1} = {2}", a, b, a * b);
            md.AppendLine();
            md.AppendItalic("Hello, Italic");
            md.AppendParagraphSeparater();
            md.Append("Hello, New Paragraph!!").AppendLine().AppendBold("Hello, Bold again!").AppendLine();

            md.Append("Link: ").AppendLink("this is link", new Uri("https://www.example.com/path?p1=v1&p2=v2")).AppendLine();

            md.AppendBold("Ordered List:").AppendLine();
            md.AppendOrderedList("item-1").AppendOrderedList("item-2").AppendOrderedListFormat("item-{0}", 3).AppendParagraphSeparater();

            md.AppendBold("Unordered List:").AppendLine();
            md.AppendUnorderedList("item-1").AppendUnorderedList("item-2").AppendUnorderedListFormat("item-{0}", 3).AppendParagraphSeparater();

            md.AppendBlockQuote("This is block quote.").AppendLine();
            md.Append("Code: ").AppendInLineCode("printf(\"Hello, World!\");").Append("is very famous!").AppendLine();

            md.BeginCodeBlock()
              .Append("#include <stdio.h>\n")
              .Append("\n")
              .Append("int main(void)\n")
              .Append("{\n")
              .Append("    printf(\"Hello, World!!\\n\");\n")
              .Append("\n")
              .Append("    return 0;\n")
              .Append("}\n")
              .EndCodeBlock();

            md.Append("OK!");

            var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.IsTrue(r.IsSuccessStatus);


            md.Clear();
            md.AppendMentionToGroup(MentionedGroup.All).Append(", Hello All!");

            r = await this.teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.IsTrue(r.IsSuccessStatus);


            md.Clear();
            md.AppendMentionToAll().Append(", Hello All again!!");

            r = await this.teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.IsTrue(r.IsSuccessStatus);

        }

        [TestMethod]
        public async Task TestCreateAndDeleteMessage()
        {
            var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, "This message will be deleted.");

            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);

            var rdm = await this.teams.DeleteMessageAsync(r.Data);

            Assert.IsTrue(rdm.IsSuccessStatus);
            Assert.IsFalse(rdm.Data.HasValues);

            var resourceOperation = rdm.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.Message, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.Delete, resourceOperation.Operation);
            Assert.AreEqual("DeleteMessage", resourceOperation.ToString());

        }

        [TestMethod]
        public async Task TestCreateMessageWithAttachment()
        {
            
            var exePath = new FileInfo(Assembly.GetExecutingAssembly().Location);
            
            string path = String.Format("{0}{1}TestData{1}thrzn41.png", exePath.DirectoryName, Path.DirectorySeparatorChar);

            Uri fileUri = null;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var data = new TeamsFileData(fs, "mypng.png", TeamsMediaType.ImagePNG))
            {
                var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, "**Post with attachment!!**", data);
                Assert.IsTrue(r.IsSuccessStatus);
                Assert.IsTrue(r.Data.HasValues);
                Assert.IsTrue(r.Data.HasFiles);

                fileUri = r.Data.FileUris[0];
            }

            var r1 = await this.teams.GetFileInfoAsync(fileUri);
            Assert.IsTrue(r1.IsSuccessStatus);
            Assert.IsTrue(r1.Data.HasValues);

            Assert.AreEqual("mypng.png", r1.Data.FileName);
            Assert.AreEqual(TeamsMediaType.ImagePNG, r1.Data.MediaType);
            Assert.AreEqual(34991, r1.Data.Size);


            var r2 = await this.teams.GetFileDataAsync(fileUri);
            Assert.IsTrue(r2.IsSuccessStatus);
            Assert.IsTrue(r2.Data.HasValues);

            Assert.AreEqual("mypng.png", r2.Data.FileName);
            Assert.AreEqual(TeamsMediaType.ImagePNG, r2.Data.MediaType);
            Assert.AreEqual(34991, r2.Data.Size);

            using (var data = r2.Data.Stream)
            {
                Assert.AreEqual(34991, data.Length);
            }

        }

        [TestMethod]
        public async Task TestGetMe()
        {
            var r = await this.teams.GetMeAsync();

            Assert.IsTrue(r.IsSuccessStatus);

            var r2 = await this.teams.GetMeFromCacheAsync();
            Assert.IsTrue(r2.IsSuccessStatus);

            var r3 = await this.teams.GetMeFromCacheAsync();
            Assert.IsTrue(r3.IsSuccessStatus);
            Assert.AreEqual(r2.Data, r3.Data);

            var r4 = await this.teams.GetMeFromCacheAsync();
            Assert.IsTrue(r4.IsSuccessStatus);
            Assert.AreEqual(r2.Data, r4.Data);
            Assert.AreEqual(r3.Data, r4.Data);
        }


        [TestMethod]
        public async Task TestPagination()
        {
            for (int i = 0; i < 5; i++)
            {
                var r = await this.teams.CreateSpaceAsync(String.Format("Test Space {0}{1}", UNIT_TEST_CREATED_PREFIX, i));

                Assert.IsTrue(r.IsSuccessStatus);
            }

            var rls = await this.teams.ListSpacesAsync(max:2);

            Assert.IsTrue(rls.IsSuccessStatus);
            Assert.IsTrue(rls.HasNext);
            Assert.AreEqual(2, rls.Data.ItemCount);

            var resourceOperation = rls.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.Space, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.List, resourceOperation.Operation);
            Assert.AreEqual("ListSpace", resourceOperation.ToString());

            var guid = rls.ListTransactionId;

            rls = await rls.ListNextAsync();

            Assert.IsTrue(rls.IsSuccessStatus);
            Assert.IsTrue(rls.HasNext);
            Assert.AreEqual(2, rls.Data.ItemCount);
            Assert.AreEqual(guid, rls.ListTransactionId);

        }


        [TestMethod]
        public async Task TestListResultEnumerator()
        {
            for (int i = 0; i < 5; i++)
            {
                var r = await this.teams.CreateSpaceAsync(String.Format("Test Space {0}{1}", UNIT_TEST_CREATED_PREFIX, i));

                Assert.IsTrue(r.IsSuccessStatus);
            }


            var guid = Guid.Empty;

            int counter = 0;

            var rls = await this.teams.ListSpacesAsync(max: 2);

            var e = rls.GetListResultEnumerator();

            while(await e.MoveNextAsync())
            {
                rls = e.CurrentResult;

                Assert.IsTrue(rls.IsSuccessStatus);
                Assert.IsTrue(rls.HasNext);
                Assert.AreEqual(2, rls.Data.ItemCount);

                var resourceOperation = rls.ParseResourceOperation();
                Assert.AreEqual(TeamsResource.Space, resourceOperation.Resource);
                Assert.AreEqual(TeamsOperation.List, resourceOperation.Operation);
                Assert.AreEqual("ListSpace", resourceOperation.ToString());

                if (guid != Guid.Empty)
                {
                    Assert.AreEqual(guid, rls.ListTransactionId);
                }
                else
                {
                    guid = rls.ListTransactionId;
                }

                if( ++counter >= 2 )
                {
                    // In this test, break after 2nd loop.
                    break;
                }
            }

        }


        [TestMethod]
        public void TestPartialErrorResponse()
        {
            string str = "{\"items\":[{\"id\":\"id01\",\"title\":\"title01\",\"type\":\"group\",\"isLocked\":true,\"teamId\":\"teamid01\",\"lastActivity\":\"2016-04-21T19:12:48.920Z\",\"created\":\"2016-04-21T19:01:55.966Z\"},{\"id\":\"id02\",\"title\":\"xyz...\",\"errors\":{\"title\":{\"code\":\"kms_failure\",\"reason\":\"Key management server failed to respond appropriately. For more information: https://developer.webex.com/errors.html\"}}}]}";

            var spaces = TeamsObject.FromJsonString<SpaceList>(str);

            var space = spaces.Items[1];

            var errors = space.GetPartialErrors();

            Assert.IsTrue(errors.ContainsKey("title"));
            Assert.AreEqual(PartialErrorCode.KMSFailure, errors["title"].Code);
            Assert.AreEqual("Key management server failed to respond appropriately. For more information: https://developer.webex.com/errors.html", errors["title"].Reason);

        }

        [TestMethod]
        public void TestErrorResponse()
        {
            string str = "{\"errorCode\":1,\"message\":\"The requested resource could not be found.\",\"errors\":[{\"errorCode\":1,\"description\":\"The requested resource could not be found.\"}],\"trackingId\":\"xyz\"}";

            var spaces = TeamsObject.FromJsonString<SpaceList>(str);

            var errors = spaces.GetErrors();

            Assert.AreEqual(1, errors[0].ErrorCode);
            Assert.AreEqual("The requested resource could not be found.", errors[0].Description);

        }



        [TestMethod]
        public async Task TestTeamsResultException()
        {

            try
            {
                var teams = TeamsAPI.CreateVersion1Client("this token does not exist");

                var result = await teams.GetMeAsync();

                var me = result.GetData();
            }
            catch(TeamsResultException sre)
            {
                Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, sre.HttpStatusCode);
                Assert.AreEqual("The request requires a valid access token set in the Authorization request header.", sre.Message);
                Assert.IsNotNull(sre.TrackingId);
            }

            try
            {
                var result = await this.teams.CreateMessageAsync("this space id does not exist", "hello");

                var m = result.GetData();
            }
            catch (TeamsResultException sre)
            {
                Assert.AreEqual(System.Net.HttpStatusCode.NotFound, sre.HttpStatusCode);
                //Assert.AreEqual("The requested resource could not be found.(ErrorCode:1)", sre.Message);
                Assert.IsTrue(sre.Message.StartsWith("The requested resource could not be found."));
                Assert.IsNotNull(sre.TrackingId);
            }

        }



        [TestMethod]
        public async Task TestCancellation()
        {
            var cancel = new CancellationTokenSource();
            cancel.Cancel();

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(
                async () =>
                {
                    var r = await this.teams.CreateMessageAsync(unitTestSpace.Id, "Hello, Cisco Webex Teams!!", cancellationToken: cancel.Token);
                }
            );

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(
                async () =>
                {
                    var r = await this.teams.ListSpacesAsync(max: 1, cancellationToken: cancel.Token);
                }
            );

        }

    }
}
