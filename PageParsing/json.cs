using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Internal;

namespace PageParsing
{
    public class json
    {
        public static string szJsonPath = "C:\\Users\\Admin\\Desktop\\test.json";

        public static void WriteJson(List<data.sDayLectures> sDayLectureList)
        {
            if (!File.Exists(szJsonPath)) //존재 안한다면 만들고 쓴다
            {
                File.Create(szJsonPath);
                Thread.Sleep(100); //좀 쉼
            }
            InputJson(sDayLectureList);
        }
        public static void InputJson(List<data.sDayLectures> sDayLectureList)
        {
            JObject json = new JObject();

            for (int i = 0; i < sDayLectureList.Count; i++)
            {
                for (int j = 0; j < sDayLectureList[i].sLectureList.Count; j++)
                {
                    JObject addjson = new JObject(
                        new JProperty($"Day{i}_{j}", $"{i + 1}"), //1일부터 5일까지 있음 월 ~ 금
                        new JProperty($"LecturesName{i}_{j}", $"{sDayLectureList[i].sLectureList[j].szLecturesName}"),
                        new JProperty($"Professor{i}_{j}", $"{sDayLectureList[i].sLectureList[j].szProfessor}"),
                        new JProperty($"LectureRoom{i}_{j}", $"{sDayLectureList[i].sLectureList[j].szLectureRoom}"),
                        new JProperty($"LecturesTime{i}_{j}", $"{sDayLectureList[i].sLectureList[j].iLecturesTime}"),
                        new JProperty($"StartTime{i}_{j}", $"{sDayLectureList[i].sLectureList[j].iStartTime}")
                );
                    json.Merge(addjson);
                }
            }
            
            

            File.WriteAllText(szJsonPath, json.ToString());
        }
        //public static void ReadJson()
        //{
        //    string jsonFilePath = @"C:\test\test.json";
        //    string str = string.Empty;
        //    string users = string.Empty;

        //    //// Json 파일 읽기
        //    using (StreamReader file = File.OpenText(jsonFilePath))
        //    using (JsonTextReader reader = new JsonTextReader(file))
        //    {
        //        JObject json = (JObject)JToken.ReadFrom(reader);

        //        DataBase _db = new DataBase();

        //        _db.IP = (string)json["IP"].ToString();
        //        _db.ID = (string)json["ID"].ToString();
        //        _db.PW = (string)json["PW"].ToString();
        //        _db.SID = (string)json["SID"].ToString();
        //        _db.DATABASE = (string)json["DATABASE"].ToString();

        //        var user = json.SelectToken("USERS");
        //        var cnt = user.Count();

        //        for (int idx = 0; idx < user.Count(); idx++)
        //        {
        //            var name = user[idx].ToString();

        //            if (idx == 0)
        //            {
        //                users += $"{name}";
        //            }
        //            else
        //            {
        //                users += $" , {name}";
        //            }
        //        }

        //        str = $" IP : {_db.IP}\n ID : {_db.ID}\n PW : {_db.PW}\n SID :" + $" {_db.SID}\n DATABASE : {_db.DATABASE}\n USERS : {users}";
                
        //    }
        //}
    }
}
