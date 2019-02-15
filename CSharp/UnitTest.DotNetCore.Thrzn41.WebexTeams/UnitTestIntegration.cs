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
    public class UnitTestIntegration
    {
        private const string UNIT_TEST_SPACE_TAG = "#webexteamsapiclientunittestspace";

        private const string UNIT_TEST_CREATED_PREFIX = "#createdwebexteamsapiclientunittest";

        private static TeamsAPIClient teams;

        private static Space unitTestSpace;

        [ClassInitialize]
        public static async Task Init(TestContext testContext)
        {

            byte[] encryptedInfo;
            byte[] entropy;

            Person me = null;

            try
            {

                string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                var dirInfo = new DirectoryInfo(String.Format("{0}{1}.thrzn41{1}unittest{1}teams", userDir, Path.DirectorySeparatorChar));

                using (var stream = new FileStream(String.Format("{0}{1}teamsintegration.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);

                    encryptedInfo = memory.ToArray();
                }

                using (var stream = new FileStream(String.Format("{0}{1}integrationentropy.dat", dirInfo.FullName, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);

                    entropy = memory.ToArray();
                }

                var token = TeamsObject.FromJsonString<TeamsIntegrationInfo>(LocalProtectedString.FromEncryptedData(encryptedInfo, entropy).DecryptToString());

                teams = TeamsAPI.CreateVersion1Client(token.TokenInfo, new TeamsRetryOnErrorHandler(4, TimeSpan.FromSeconds(15.0f)));

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
            catch (FileNotFoundException) { }

            checkTeamsAPIClient(me);
            checkUnitTestSpace();
        }


        [ClassCleanup]
        public static async Task Clean()
        {
            if(teams == null)
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

        private static void checkTeamsAPIClient(Person me)
        {
            if (me == null || String.IsNullOrEmpty(me.Id))
            {
                Assert.Fail("You need to configure Cisco Webex Teams info by using UnitTestTool.EncryptTeamsInfo.");
            }
        }

        private static void checkUnitTestSpace()
        {
            if (unitTestSpace == null)
            {
                Assert.Fail("You need to create manually a Space for unit test that contains name '#webexteamsapiclientunittestspace' in Title, and add bot or integration user that is used in the test.");
            }
        }

        [TestMethod]
        public async Task TestCreateMessage()
        {
            var r = await teams.CreateMessageAsync(unitTestSpace.Id, "Hello, Cisco Webex Teams!!");

            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
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
            var r = await teams.CreateMessageAsync(unitTestSpace.Id, "Hello, **markdown**!!");

            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
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

            var r = await teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
            Assert.IsTrue(r.IsSuccessStatus);


            md.Clear();
            md.AppendMentionToGroup(MentionedGroup.All).Append(", Hello All!");

            r = await teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
            Assert.IsTrue(r.IsSuccessStatus);


            md.Clear();
            md.AppendMentionToAll().Append(", Hello All again!!");

            r = await teams.CreateMessageAsync(unitTestSpace.Id, md.ToString());
            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
            Assert.IsTrue(r.IsSuccessStatus);

        }

        [TestMethod]
        public async Task TestCreateAndDeleteMessage()
        {
            var r = await teams.CreateMessageAsync(unitTestSpace.Id, "This message will be deleted.");

            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
            Assert.IsTrue(r.IsSuccessStatus);
            Assert.IsTrue(r.Data.HasValues);

            Assert.IsNotNull(r.Data.Id);

            var rdm = await teams.DeleteMessageAsync(r.Data);

            Assert.AreEqual(HttpStatusCode.NoContent, rdm.HttpStatusCode);
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
                var r = await teams.CreateMessageAsync(unitTestSpace.Id, "**Post with attachment!!**", data);
                Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
                Assert.IsTrue(r.IsSuccessStatus);
                Assert.IsTrue(r.Data.HasValues);
                Assert.IsTrue(r.Data.HasFiles);

                fileUri = r.Data.FileUris[0];
            }

            var r1 = await teams.GetFileInfoAsync(fileUri);
            Assert.AreEqual(HttpStatusCode.OK, r1.HttpStatusCode);
            Assert.IsTrue(r1.IsSuccessStatus);
            Assert.IsTrue(r1.Data.HasValues);

            Assert.AreEqual("mypng.png", r1.Data.FileName);
            Assert.AreEqual(TeamsMediaType.ImagePNG, r1.Data.MediaType);
            Assert.AreEqual(34991, r1.Data.Size);

            var resourceOperation = r1.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.FileInfo, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.Get, resourceOperation.Operation);
            Assert.AreEqual("GetFileInfo", resourceOperation.ToString());



            var r2 = await teams.GetFileDataAsync(fileUri);
            Assert.AreEqual(HttpStatusCode.OK, r2.HttpStatusCode);
            Assert.IsTrue(r2.IsSuccessStatus);
            Assert.IsTrue(r2.Data.HasValues);

            Assert.AreEqual("mypng.png", r2.Data.FileName);
            Assert.AreEqual(TeamsMediaType.ImagePNG, r2.Data.MediaType);
            Assert.AreEqual(34991, r2.Data.Size);

            resourceOperation = r2.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.FileData, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.Get, resourceOperation.Operation);
            Assert.AreEqual("GetFileData", resourceOperation.ToString());

            using (var data = r2.Data.Stream)
            {
                Assert.AreEqual(34991, data.Length);
            }



            using (var stream = new MemoryStream())
            {
                var r3 = await teams.CopyFileDataToStreamAsync(fileUri, stream);
                Assert.AreEqual(HttpStatusCode.OK, r3.HttpStatusCode);
                Assert.IsTrue(r3.IsSuccessStatus);
                Assert.IsTrue(r3.Data.HasValues);

                Assert.AreEqual("mypng.png", r3.Data.FileName);
                Assert.AreEqual(TeamsMediaType.ImagePNG, r3.Data.MediaType);
                Assert.AreEqual(34991, r3.Data.Size);

                resourceOperation = r3.ParseResourceOperation();
                Assert.AreEqual(TeamsResource.FileData, resourceOperation.Resource);
                Assert.AreEqual(TeamsOperation.Get, resourceOperation.Operation);
                Assert.AreEqual("GetFileData", resourceOperation.ToString());

                Assert.AreEqual(34991, stream.Length);
            }

        }

        [TestMethod]
        public async Task TestGetMe()
        {
            var r = await teams.GetMeAsync();

            Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
            Assert.IsTrue(r.IsSuccessStatus);

            var r2 = await teams.GetMeFromCacheAsync();
            Assert.AreEqual(HttpStatusCode.OK, r2.HttpStatusCode);
            Assert.IsTrue(r2.IsSuccessStatus);

            var r3 = await teams.GetMeFromCacheAsync();
            Assert.AreEqual(HttpStatusCode.OK, r3.HttpStatusCode);
            Assert.IsTrue(r3.IsSuccessStatus);
            Assert.AreEqual(r2.Data, r3.Data);

            var r4 = await teams.GetMeFromCacheAsync();
            Assert.AreEqual(HttpStatusCode.OK, r4.HttpStatusCode);
            Assert.IsTrue(r4.IsSuccessStatus);
            Assert.AreEqual(r2.Data, r4.Data);
            Assert.AreEqual(r3.Data, r4.Data);
        }


        [TestMethod]
        public async Task TestPagination()
        {
            for (int i = 0; i < 5; i++)
            {
                var r = await teams.CreateSpaceAsync(String.Format("Test Space {0}{1}", UNIT_TEST_CREATED_PREFIX, i));

                Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
                Assert.IsTrue(r.IsSuccessStatus);
            }

            var rls = await teams.ListSpacesAsync(max:2);

            Assert.AreEqual(HttpStatusCode.OK, rls.HttpStatusCode);
            Assert.IsTrue(rls.IsSuccessStatus);
            Assert.IsTrue(rls.HasNext);
            Assert.AreEqual(2, rls.Data.ItemCount);

            var resourceOperation = rls.ParseResourceOperation();
            Assert.AreEqual(TeamsResource.Space, resourceOperation.Resource);
            Assert.AreEqual(TeamsOperation.List, resourceOperation.Operation);
            Assert.AreEqual("ListSpace", resourceOperation.ToString());

            var guid = rls.ListTransactionId;

            rls = await rls.ListNextAsync();

            Assert.AreEqual(HttpStatusCode.OK, rls.HttpStatusCode);
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
                var r = await teams.CreateSpaceAsync(String.Format("Test Space {0}{1}", UNIT_TEST_CREATED_PREFIX, i));

                Assert.AreEqual(HttpStatusCode.OK, r.HttpStatusCode);
                Assert.IsTrue(r.IsSuccessStatus);
            }


            var guid = Guid.Empty;

            int counter = 0;

            var rls = await teams.ListSpacesAsync(max: 2);

            var e = rls.GetListResultEnumerator();

            while(await e.MoveNextAsync())
            {
                rls = e.CurrentResult;

                Assert.AreEqual(HttpStatusCode.OK, rls.HttpStatusCode);
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
                var result = await teams.CreateMessageAsync("this space id does not exist", "hello");

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
                    var r = await teams.CreateMessageAsync(unitTestSpace.Id, "Hello, Cisco Webex Teams!!", cancellationToken: cancel.Token);
                }
            );

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(
                async () =>
                {
                    var r = await teams.ListSpacesAsync(max: 1, cancellationToken: cancel.Token);
                }
            );

        }

    }
}
