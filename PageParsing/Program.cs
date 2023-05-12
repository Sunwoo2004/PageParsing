using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using System.Drawing;
using System.ComponentModel;

namespace PageParsing
{
    internal class Program
    {
        static string szLink = "";
        static List<data.sDayLectures> sDayLectureList = new List<data.sDayLectures>();
        static void Main(string[] args)
        {
            sDayLectureList.Clear();

            INILoader kLoader = new INILoader();
            kLoader.SetFileName("C:\\Users\\Admin\\Desktop\\test.ini");
            kLoader.SetTitle("common");
            szLink = kLoader.LoadString("link","");

            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl(szLink);

                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

                string html = driver.PageSource;
                //Console.WriteLine(html);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);


                Console.Clear();

                // 시간 얻어오기 시작
                string szDateTime = "";
                FindXPath(doc, "//*[@id=\'container\']/div/div[2]/table/tbody/tr/th/div",out szDateTime);
                string[] szDateTimeArr = szDateTime.Split(' ');
                for (int i = 0; i < szDateTimeArr.Length; i++)
                {
                    //Console.WriteLine(szDateTimeArr[i]);
                }
                // 시간 얻어오기 끝

                // 요일 얻어오기 시작
                string szDay = "";
                FindXPath(doc, "//*[@id=\'container\']/div/div[1]/table", out szDay);
                //Console.WriteLine(szDay);
                // 요일 얻어오기 끝

                //시간표 가져오기
                for (int i = 1; i < 6; i++)
                {
                    //Console.WriteLine($"{i}번째 날");
                    List<data.sLectures> sLecturesList = new List<data.sLectures>();
                    for (int j = 1; j < 25; j++)
                    {
                        string[] szDump = new string[4];
                        for (int k = 0; k < szDump.Length; k++) //초기화
                        {
                            szDump[k] = "";
                        }

                        bool bFound = FindXPath(doc, "//*[@id=\'container\']/div/div[2]/table/tbody/tr/td[" + i + "]/div[1]/div[" + j + "]/h3", out szDump[0]);
                        FindXPath(doc, "//*[@id=\'container\']/div/div[2]/table/tbody/tr/td[" + i + "]/div[1]/div[" + j + "]/p/em", out szDump[1]);
                        FindXPath(doc, "//*[@id=\'container\']/div/div[2]/table/tbody/tr/td[" + i + "]/div[1]/div[" + j + "]/p/span", out szDump[2]);
                        if (bFound)
                        {
                            FindStyle(doc, "//*[@id=\'container\']/div/div[2]/table/tbody/tr/td[" + i + "]/div[1]/div[" + j + "]", out szDump[3]);

                            int iHeight = Convert.ToInt32(Regex.Replace(Regex.Match(szDump[3], @"height:\s*([^;]+)").Groups[1].Value.Trim(), @"px$", ""));

                            int iTop = Convert.ToInt32(Regex.Replace(Regex.Match(szDump[3], @"top:\s*([^;]+)").Groups[1].Value.Trim(), @"px$", ""));

                            data.sLectures lecture = new data.sLectures();
                            lecture.szLecturesName = szDump[0];
                            lecture.szProfessor = szDump[1];
                            lecture.szLectureRoom = szDump[2];
                            switch (iHeight)
                            {
                                case 51:
                                    lecture.iLecturesTime = 1;
                                    break;
                                case 101:
                                    lecture.iLecturesTime = 2;
                                    break;
                                default:
                                    lecture.iLecturesTime = 3;
                                    break;
                            }

                            //강의 정렬 시작점 450, 1시간 마다 50씩 증가 하는것 같음.
                            lecture.iTop = iTop;
                            lecture.iStartTime = 0; //아직은 시작 시간을 모른다;;
                            sLecturesList.Add(lecture);
                        }
                    }
                    data.sDayLectures sDayLectures = new data.sDayLectures();
                    sDayLectures.iLectureIDX = i;
                    sDayLectures.sLectureList = sLecturesList;
                    sDayLectureList.Add(sDayLectures);
                }
                //시간표 가져오기 끝

                //시간표 정렬
                for (int k = 0; k < sDayLectureList.Count; k++)
                {
                    for (int i = 0; i < sDayLectureList[k].sLectureList.Count - 1; i++)
                    {
                        for (int j = 0; j < sDayLectureList[k].sLectureList.Count - i - 1; j++)
                        {
                            if (sDayLectureList[k].sLectureList[j].iTop > sDayLectureList[k].sLectureList[j + 1].iTop)
                            {
                                data.sLectures temp = sDayLectureList[k].sLectureList[j];
                                sDayLectureList[k].sLectureList[j] = sDayLectureList[k].sLectureList[j + 1];
                                sDayLectureList[k].sLectureList[j + 1] = temp;
                            }
                        }
                    }
                }
                //시간표 정렬 끝

                //강의 시작 시간 시작

                for (int i = 0; i < sDayLectureList.Count; i++)
                {
                    for (int j = 0; j < sDayLectureList[i].sLectureList.Count; j++)
                    {
                        data.sLectures temp = sDayLectureList[i].sLectureList[j];

                        temp.iStartTime = GetLectureStartTime(temp.iTop);

                        sDayLectureList[i].sLectureList[j] = temp; //마지막에 복사
                    }
                }

                //강의 시작 시간 끝

                for (int i = 0; i < sDayLectureList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}일 차");
                    for (int j = 0; j < sDayLectureList[i].sLectureList.Count; j++)
                    {
                        Console.WriteLine($"강의 : {sDayLectureList[i].sLectureList[j].szLecturesName}, 교수 : {sDayLectureList[i].sLectureList[j].szProfessor}, 강의실 : {sDayLectureList[i].sLectureList[j].szLectureRoom}, 강의시간 : {sDayLectureList[i].sLectureList[j].iLecturesTime}시간, 시작시간 : {sDayLectureList[i].sLectureList[j].iStartTime}시");
                    }
                    Console.WriteLine("");
                }

                //추후에 웹 api를 위해 json 
                json.WriteJson(sDayLectureList); //여기로 들어가면 복사를 너무 많이하긴 한다. 수정필요
                //json 끝

            }

            Console.ReadKey();

        }

        static int GetLectureStartTime(int iTop)
        {
            switch (iTop)
            {
                case 450: //9시
                    return 9;
                case 500: //10시
                    return 10;
                case 550: //11시
                    return 11;
                case 600: //12시
                    return 12;
                case 650: //13시
                    return 13;
                case 700: //14시
                    return 14;
                case 750: //15시
                    return 15;
                case 800: //16시
                    return 16;
                case 850: //17시
                    return 17;
                case 900: //18시
                    return 18;
                case 950: //19시
                    return 19;
                case 1000: //20시
                    return 20;
                case 1050: //21시
                    return 21;
                case 1100: //22시
                    return 22;
                case 1150: //23시
                    return 23;
                default:
                    return -1;
            }
        }

        static bool FindStyle(HtmlDocument doc, string xPath, out string value)
        {
            value = "";
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xPath);

            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    value = node.GetAttributeValue("style", "");
                    return true;
                }
            }
            else
            {
                return false;
            }

            return false;
        }
        static bool FindXPath(HtmlDocument doc, string xPath, out string value)
        {
            value = "";
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xPath);

            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    value = node.InnerText;
                    return true;
                }
            }
            else
            {
                return false;
            }

            return false;
        }
    }
}
