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
        static List<sDayLectures> sDayLectureList = new List<sDayLectures>();
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
                    List<sLectures> sLecturesList = new List<sLectures>();
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

                            sLectures lecture = new sLectures();
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
                    sDayLectures sDayLectures = new sDayLectures();
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
                                sLectures temp = sDayLectureList[k].sLectureList[j];
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
                        sLectures temp = sDayLectureList[i].sLectureList[j];

                        switch (temp.iTop)
                        {
                            case 450: //9시
                                temp.iStartTime = 9;
                                break;
                            case 500: //10시
                                temp.iStartTime = 10;
                                break;
                            case 550: //11시
                                temp.iStartTime = 11;
                                break;
                            case 600: //12시
                                temp.iStartTime = 12;
                                break;
                            case 650: //13시
                                temp.iStartTime = 13;
                                break;
                            case 700: //14시
                                temp.iStartTime = 14;
                                break;
                            case 750: //15시
                                temp.iStartTime = 15;
                                break;
                            case 800: //16시
                                temp.iStartTime = 16;
                                break;
                            case 850: //17시
                                temp.iStartTime = 17;
                                break;
                            case 900: //18시
                                temp.iStartTime = 18;
                                break;
                            case 950: //19시
                                temp.iStartTime = 19;
                                break;
                            case 1000: //20시
                                temp.iStartTime = 20;
                                break;
                            case 1050: //21시
                                temp.iStartTime = 21;
                                break;
                            case 1100: //22시
                                temp.iStartTime = 22;
                                break;
                            case 1150: //23시
                                temp.iStartTime = 23;
                                break;
                            default:
                                break;
                        }

                        sDayLectureList[i].sLectureList[j] = temp; //마지막에 복사
                    }
                }

                //강의 시작 시간 끝

                for (int i = 0; i < sDayLectureList.Count; i++)
                {
                    Console.WriteLine($"{i + 1}일 차");
                    for (int j = 0; j < sDayLectureList[i].sLectureList.Count; j++)
                    {
                        Console.WriteLine($"강의 : {sDayLectureList[i].sLectureList[j].szLecturesName}, 교수 : {sDayLectureList[i].sLectureList[j].szProfessor}, 강의실 : {sDayLectureList[i].sLectureList[j].szLectureRoom}, 강의시간 : {sDayLectureList[i].sLectureList[j].iLecturesTime}시간, 시작시간 : {sDayLectureList[i].sLectureList[j].iStartTime}");
                    }
                    Console.WriteLine("");
                }

            }

            Console.ReadKey();

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

        struct sDayLectures
        {
            public int iLectureIDX { get; set; } //강의 날짜 인덱스
            public List<sLectures> sLectureList { get; set; }
        }
        struct sLectures
        { 
            public string szLecturesName { get; set; } //강의 이름
            public string szProfessor { get; set; } //교수 이름
            public string szLectureRoom { get; set; } //강의실 이름
            public int iLecturesTime { get; set; } //강의 진행 시간
            public int iTop { get; set; } //강의 순서를 정렬하기 위해..
            public int iStartTime { get; set; } //강의 시작 시간

        }

    }
}
