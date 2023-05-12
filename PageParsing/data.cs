using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageParsing
{
    public class data
    {
        public struct sDayLectures
        {
            public int iLectureIDX { get; set; } //강의 날짜 인덱스
            public List<sLectures> sLectureList { get; set; }
        }
        public struct sLectures
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
