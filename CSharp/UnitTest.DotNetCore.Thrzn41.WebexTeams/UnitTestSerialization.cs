using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Thrzn41.Util;
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;

namespace UnitTest.DotNetCore.Thrzn41.WebexTeams
{

    [TestClass]
    public class UnitTestSerialization
    {
        public const string DATE_TIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
        public static string TEAMS_OBJECTS_DIRECTORY { get; set; }

        [ClassInitialize]
        public static void Test(TestContext testContext)
        {
            var exePath = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = new DirectoryInfo(String.Format("{0}{1}TestData{1}TeamsObjects", exePath.DirectoryName, Path.DirectorySeparatorChar));

            TEAMS_OBJECTS_DIRECTORY = dir.FullName;
        }


        public static T Load<T>(string fileName)
            where T : TeamsObject, new()
        {
            T result = null;

            using (var fs     = new FileStream(String.Format("{0}{1}{2}", TEAMS_OBJECTS_DIRECTORY, Path.DirectorySeparatorChar, fileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, UTF8Utils.UTF8_WITHOUT_BOM, true))
            {
                result = TeamsObject.FromJsonString<T>(reader.ReadToEnd());
            }

            return result;
        }

        [TestMethod]
        public void TestExtensionData()
        {
            var m = Load<Message>("Message.json");

            var e = m.ToExtensionObject();
        }

        [TestMethod]
        public void TestMessage()
        {
            var r = Load<Message>("Message.json");

            Assert.AreEqual("Y2lzY29zcGFyazovL3VzL01FU1NBR0UvOTJkYjNiZTAtNDNiZC0xMWU2LThhZTktZGQ1YjNkZmM1NjVk", r.Id);
            Assert.AreEqual("Y2lzY29zcGFyazovL3VzL1JPT00vYmJjZWIxYWQtNDNmMS0zYjU4LTkxNDctZjE0YmIwYzRkMTU0", r.SpaceId);
            Assert.AreEqual("group", r.SpaceTypeName);
            Assert.AreEqual(SpaceType.Group, r.SpaceType);
        }

        [TestMethod]
        public void TestAttachmentAction()
        {
            var r = Load<AttachmentAction>("AttachmentAction.json");

            Assert.AreEqual("Y2lzY29zcGFyazovL3VzL0NBTExTLzU0MUFFMzBFLUUyQzUtNERENi04NTM4LTgzOTRDODYzM0I3MQo", r.Id);
            Assert.AreEqual("Y2lzY29zcGFyazovL3VzL1BFT1BMRS83MTZlOWQxYy1jYTQ0LTRmZ", r.PersonId);
            Assert.AreEqual("L3VzL1BFT1BMRS80MDNlZmUwNy02Yzc3LTQyY2UtOWI", r.SpaceId);
            Assert.AreEqual("submit", r.TypeName);
            Assert.AreEqual(AttachmentActionType.Submit, r.Type);
            Assert.AreEqual("GFyazovL3VzL1BFT1BMRS80MDNlZmUwNy02Yzc3LTQyY2UtOWI4NC", r.MessageId);
            Assert.AreEqual("2016-05-10T19:41:00.100Z", r.Created.Value.ToString(DATE_TIME_FORMAT));

            Assert.IsTrue(r.HasValues);
            Assert.IsFalse(r.HasErrors);
            Assert.IsFalse(r.HasSerializationErrors);
            Assert.IsFalse(r.HasExtensionData);

            var input = r.Inputs;
            Assert.AreEqual("Thrzn41", input.GetInputValue<string>("Name"));
            Assert.AreEqual("https://example.com", input.GetInputValue<string>("Url"));
            Assert.AreEqual("thrzn41@example.com", input.GetInputValue<string>("Email"));
            Assert.AreEqual("+81 abc xyz", input.GetInputValue<string>("Tel"));

            Assert.IsTrue(input.ContainsKey("Name"));
            Assert.IsFalse(input.ContainsKey("InvalidKeyName"));

            Assert.AreEqual(4, input.Keys.Count);
        }

        [TestMethod]
        public void TestAttachmentActionInput()
        {
            var input = AttachmentActionInputs.FromJsonString(
                @"
                {
                  ""Name"": ""Thrzn41"",
                  ""Url"": ""https://example.com"",
                  ""Email"": ""thrzn41@example.com"",
                  ""Tel"": ""+81 abc xyz""
                }
                "
                );

            Assert.AreEqual("Thrzn41", input.GetInputValue<string>("Name"));
            Assert.AreEqual("https://example.com", input.GetInputValue<string>("Url"));
            Assert.AreEqual("thrzn41@example.com", input.GetInputValue<string>("Email"));
            Assert.AreEqual("+81 abc xyz", input.GetInputValue<string>("Tel"));


            input = AttachmentActionInputs.FromObject(
                new
                {
                    Name = "Thrzn41",
                    Url = "https://example.com",
                    Email = "thrzn41@example.com",
                    Tel = "+81 abc xyz",
                }
            );

            Assert.AreEqual("Thrzn41", input.GetInputValue<string>("Name"));
            Assert.AreEqual("https://example.com", input.GetInputValue<string>("Url"));
            Assert.AreEqual("thrzn41@example.com", input.GetInputValue<string>("Email"));
            Assert.AreEqual("+81 abc xyz", input.GetInputValue<string>("Tel"));

        }


    }
}
