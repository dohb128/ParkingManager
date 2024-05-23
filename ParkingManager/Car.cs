using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManager
{
    internal class Car
    {
        /// <summary>
        /// 회차 시간(분단위)
        /// </summary>
        public const int TurningTime = 10; 

        /// <summary>
        /// 분당 요금(원단위)
        /// </summary>
        public const int PricePerMin = 100;

        /// <summary>
        /// 자동차 번호
        /// </summary>
        private string _carNumber;

        /// <summary>
        /// CarNumber의 getter()
        /// </summary>
        /// <returns>자동차 번호</returns>
        public string CarNumber
        {
            get {
                return _carNumber;
            }
        }
        
        /// <summary>
        /// 입차 시간
        /// </summary>
        private DateTime _inTime;

        /// <summary>
        /// InTime의 getter()
        /// </summary>
        /// <returns>입차 시간</returns>        
        public DateTime InTime
        {
            get {
                return this._inTime;
            }
        }

        /// <summary>
        /// 출차 시간
        /// </summary>
        private DateTime _outTime;

        public DateTime OutTime
        {
            get {
                return _outTime;
            }
        }

        /// <summary>        /// 
        /// 기본 생성자 형태
        /// </summary>
        public Car()
        {
        
        }

        //public Car(string carnumber)
        //{
        //    this.CarNumber = carnumber;
        //    this.InTime = DateTime.Now;
        //}

        /// <summary>
        /// 해당 생성자는 자동차 번호만 생성자 호출시 외부에서 가져온다.
        /// 그리고 자신의 외부에서 온 자동차 번호와, 현재 시간을 이용하여
        /// 다른 생성자를 호출한다.
        /// </summary>
        /// <param name="carnumber">자동차 번호</param>
        public Car(string carnumber) 
            : this(carnumber, DateTime.Now)
        {
            //현재 생성자에서는 특별히 수행할 문장이 필요 없다.            
        }

        /// <summary>
        /// 해당 생성자는 자동차 번호와 입차시간을 외부에서 가져온다.
        /// 그것을 이용하여 두 정보를 인스턴스 변수에 setting한다.
        /// </summary>
        /// <param name="carnumber">자동차 번호</param>
        /// <param name="intime">입차시간</param>
        public Car(string carnumber, DateTime intime)
        {
            this._carNumber = carnumber;
            this._inTime = intime;
        }

        public Car(string carnumber, DateTime intime, DateTime outtime)
            : this(carnumber, intime)
        {
            this._outTime = outtime;
        }

        /// <summary>
        /// 출차시 수행
        /// </summary>
        /// <returns>전체 주차 분</returns>
        public int Out()
        {
            //이미 출차한 차량의 출차시간을 다시 설정하지 못하도록 하는 안전 장치
            if(this._outTime > this._inTime) {
                return -1;
            }

            //현재 시간을 출차 시간으로 설정
            this._outTime = DateTime.Now;

            return (int)(_outTime - _inTime).TotalMinutes;
            
            //위 반환문은 아래와 같이 세부적으로 분해해서 볼 수 있다.
            //출차시간-입차시간=주차시간
            //TimeSpan t1 = OutTime - InTime;
            //
            //주차시간을 분으로 환산
            //분.초 형태의 double 타입으로 반환
            //double t2 = t1.TotalMinutes;
            //
            //소수점 이하를 버려 정수 형태로 변경한다.
            //int t3 = (int)t2;
            //
            //return t3;
        }

        public string Info(int emptyLot)
        {
            string result = $"차량번호 : {_carNumber}";
            result += Environment.NewLine;
            result += $"입차시간 : {_inTime}";
            result += Environment.NewLine;
            result += $"주차구역 : {emptyLot}";
            result += Environment.NewLine;
            return result;
        }
    }
}
