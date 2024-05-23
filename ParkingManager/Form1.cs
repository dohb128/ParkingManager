using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkingManager
{
    public partial class FormMain : Form
    {
        Car[] cars = null; //cars는 인스턴스 변수

        //기존에 주차했던 자동차 정보를 보관
        List<Car> carsHistroy;

        public FormMain()//생성자 (특수메소드)
        {
            InitializeComponent(); //Designer.cs의 메소드

            //아래 코드는 지역 변수 cars를 생성합니다.
            //생성자(특수메소드)의 실행이 끝나면
            //지역변수인 cars 변수가 사라집니다.
            //그래서 cars를 [지역 변수]가 아닌
            //[인스턴스 변수]로 만들어야 합니다.
            //FormMain의 인스턴스가 있는 동안
            //유지할 수 있게 됩니다.
            //Car[] cars = new Car[5];

            //인스턴스 변수 cars에
            //Car 인스턴스 5개를 저장할 배열 인스턴스를 만들어
            //참조값을 복사
            cars = new Car[5];

            //출차를 완료한 차량의 정보를 누적하기 위한 List
            carsHistroy = new List<Car>();

        }

        private void btnEntry_Click(object sender, EventArgs e)
        {
            #region comment
            //1. 차량번호를 넣었는지 검사
            //모든 UI 컨트롤의 Text 속성은
            //string(참조타입)임!!!

            //Type1.
            //var isEmpty =
            //    tbxCarNumber.Text == null
            //    || tbxCarNumber.Text == "";

            //Type2.
            //var isEmpty = string.IsNullOrEmpty(tbxCarNumber.Text);
            //
            //if (isEmpty == true) {
            //    MessageBox.Show("차량번호넣어!");
            //    tbxCarNumber.Focus();
            //    return; // 메소드 즉시 종료
            //}
            #endregion

            if (IsEmptyNumber()) {
                return;
            }

            //2. 차량번호(기존 주차) 중복 검사
            int emptyLot = -1; //비어있는 인덱스 넣을 변수

            for(int i=0; i < cars.Length; i++) {
                if (cars[i] == null) {
                    if(emptyLot == -1) {//배열은 0이상부터 시작.
                        emptyLot = i;
                    }
                } else {
                    if (cars[i].CarNumber == tbxCarNumber.Text) {
                        MessageBox.Show("동일차량번호");
                        tbxCarNumber.Focus();
                        return;
                    }
                }
            }

            //3. 주차가능 구역이 없는지 있는지
            if(emptyLot == -1) {
                MessageBox.Show("만차");
                return;
            }
            //4. 1,2,3 모두 통과한 경우
            // 입차 진행위해 Car인스턴스 만들고
            // 배열에 넣는다

            Car car = new Car(tbxCarNumber.Text); //지역변수 car
            //car.CarNumber = tbxCarNumber.Text;
            //car.InTime = DateTime.Now;
            
            //안해도 자동으로 MinValue들어있음
            //car.OutTime = DateTime.MinValue;

            //인스턴스를 알고 있는 지역변수의
            //참조 정보를 배열로 복사
            cars[emptyLot] = car;

            //5. 결과 출력
            //$"..." : 문자열보간
            //파이썬의 f-string과 유사
            //string result = $"차량번호 : {car.GetCarNumber()}";
            //result += Environment.NewLine;
            //result += $"입차시간 : {car.GetInTime()}" ;
            //result += Environment.NewLine;
            //result += $"주차구역 : {emptyLot}";
            //result += Environment.NewLine;
            //result += "입차 완료";

            tbxResult.Text = car.Info(emptyLot) + "입차 완료";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (IsEmptyNumber()) {
                return;
            }

            //1. 차가 있는지 검사
            bool result = FindCar(tbxCarNumber.Text.Trim(), out int index);

            if(result == false) {
                MessageBox.Show("해당 차량 없음");
                tbxCarNumber.Focus();
                return;
            }

            //2. 현재 시간을 출차 시간으로 설정
            //cars[index].OutTime = DateTime.Now;

            //3. 출차시간-입차시간=주차시간 계산
            //TimeSpan결과값이 나옴.
            //var termtime = cars[index].OutTime
            //               - cars[index].InTime;
            var termmin = cars[index].Out(); //termtime.TotalMinutes;
            if(termmin == -1) {
                return;
            }

            //4. 주차시간 통해 요금 계산
            var totalprice = termmin * Car.PricePerMin;

            //5. 결과 출력
            if(termmin <= Car.TurningTime) {
                tbxResult.Text = "회차입니다.";
            } else {
                tbxResult.Text = $"{termmin}분 주차, 요금:{totalprice}원";
            }

            //6. 주차칸 비우기!!!!!
            carsHistroy.Add(cars[index]);
            cars[index] = null;
        }

        private bool IsEmptyNumber()
        {
            var isEmpty = string.IsNullOrEmpty(tbxCarNumber.Text);

            if (isEmpty == true) {
                MessageBox.Show("차량번호넣어!");
                tbxCarNumber.Focus();
            }

            return isEmpty;
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            if (IsEmptyNumber()) {
                return;
            }

            //1. 차가 있는지 검사
            bool result = FindCar(tbxCarNumber.Text.Trim(), out int index);

            //if (!result) {
            if (result == false) {
                MessageBox.Show("해당 차량 없음");
                tbxCarNumber.Focus();
                return;
            }

            tbxResult.Text = cars[index].Info(index);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (IsEmptyNumber()) {
                return;
            }

            //장기 기록에 차가 있는지 검사
            List<Car> findlist = FindHistoryCar(tbxCarNumber.Text.Trim());

            if (findlist != null && findlist.Count > 0) {
                //있으면
                var result = MessageBox.Show("정말삭제?", "주의", MessageBoxButtons.YesNo);

                if(result == DialogResult.Yes) {
                    //삭제 진행
                    for(int i=0; i < findlist.Count; i++) {
                        //리스트 삭제
                        carsHistroy.Remove(findlist[i]);
                    }
                    MessageBox.Show(tbxCarNumber.Text + "기록 삭제 완료 /건수:"+ findlist.Count);
                }
            } else {
                //없으면
                MessageBox.Show("해당 차량 없음");
                tbxCarNumber.Focus();
                return;
            }

        }

        private bool FindCar(string number, out int index)
        {
            //1. 차가 있는지 검사
            index = -1;

            for (int i = 0; i < cars.Length; i++) {
                if (cars[i] != null
                    && cars[i].CarNumber == number) {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        private List<Car> FindHistoryCar(string number)
        {
            List<Car> findcars = new List<Car>();

            //var a = "abc".Length;
            //var b = "abc".Count;

            //for(int i=0; i < carsHistroy.Count; i++) {
            //    if(carsHistroy[i].CarNumber == number) {
            //        findcars.Add(carsHistroy[i]);
            //    }
            //}

            foreach(var car in carsHistroy) {
                if(car.CarNumber == number) {
                    findcars.Add(car);
                }
            }

            return findcars;
        }

        private void rtbLongTerm_CheckedChanged(object sender, EventArgs e)
        {
            dtpExitDate.Visible = rtbLongTerm.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileControl fc = new FileControl();
            fc.WriteCurrentInfo(cars);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileControl fc = new FileControl();
            Array.Clear(cars, 0, cars.Length);
            fc.ReadCurrentInfo(cars);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileControl fc = new FileControl();
            fc.WriteHistoryInfo(carsHistroy);
            MessageBox.Show("완료");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileControl fc = new FileControl();
            carsHistroy.Clear();
            fc.ReadHistoryInfo(carsHistroy);
            MessageBox.Show("완료");
        }
    }//class
}//ns
