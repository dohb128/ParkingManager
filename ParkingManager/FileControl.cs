/*202141093 도효빈*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkingManager
{
    internal class FileControl
    {
        string basePath = "c:\\carsA";
        string baseFile = "list.txt"; //현재 주차 정보 저장
        string oldFile = "oldlist.txt";//과거 주차 정보 저장

        bool ExistBasePath()
        {
            if (false == Directory.Exists(basePath)) {
                if(null == Directory.CreateDirectory(basePath)) {
                    return false;
                }
            }

            return true;
        }

        public void WriteCurrentInfo(Car[] cars)
        {
            if (ExistBasePath()) {
                try {
                    var fullname = Path.Combine(basePath ,baseFile);

                    //쓰기: 새로만들기(Create) or 추가하기(Append)
                    var fs = new FileStream(fullname, FileMode.Create);
                    var sw = new StreamWriter(fs, Encoding.UTF8);

                    //현재 주차 정보 내용을 파일에 출력(쓰기)
                    for(int i=0; i < cars.Length; i++) {
                        if (cars[i] != null) {
                            var lotno = i.ToString();
                            var carnum = cars[i].CarNumber;
                            var intime = cars[i].InTime.ToString("yyyyMMdd HHmmss");

                            var result = $"{lotno}|{carnum}|{intime}";

                            sw.WriteLine(result);
                        }
                    }

                    sw.Close();
                    fs.Close();
                }catch(Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void ReadCurrentInfo(Car[] cars)
        {
            if (ExistBasePath()) {
                try {
                    var fullname = Path.Combine(basePath, baseFile);

                    if (File.Exists(fullname)) {
                        var fs = new FileStream(fullname, FileMode.Open);
                        var sr = new StreamReader(fs, Encoding.UTF8);

                        while (false == sr.EndOfStream) {

                            //line = "0|1111|20230531 122115";
                            var line = sr.ReadLine();

                            if (string.IsNullOrEmpty(line)) {
                                continue;
                            }
                            //splitlines = {"0","1111","20230531 122115" }
                            var splitlines = line.Split(new char[] { '|'});
                            if(splitlines.Length == 3) {
                                if (int.TryParse(splitlines[0], out int lotno)) {
                                    string num = splitlines[1].Trim();

                                    if (DateTime.TryParseExact(
                                        splitlines[2]
                                        , "yyyyMMdd HHmmss"
                                        , null
                                        , System.Globalization.DateTimeStyles.None
                                        , out DateTime intime)) {
                                        cars[lotno] = new Car(num, intime);
                                    }
                                }
                            }
                        }

                        sr.Close();
                        fs.Close();
                    }

                }catch(Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        internal void WriteHistoryInfo(List<Car> carsHistory)
        {
           //basePath\\oldFile
           //파일 생성하기
           //주차 : lotno, carnumber, inTime
           //기록 : carnumber, inTime,outTime
           if (ExistBasePath()) {
                try {
                    var fullname = Path.Combine(basePath, oldFile);

                    var fs = new FileStream(fullname, FileMode.Create);
                    var sw = new StreamWriter(fs, Encoding.UTF8);

                    foreach (var car in carsHistory)
                    {
                        var carnum = car.CarNumber;
                        var intime = car.InTime.ToString("yyyyMMdd HHmmss");
                        var outtime = car.OutTime.ToString("yyyyMMdd HHmmss");

                        var result = $"{carnum}|{intime}|{outtime}";

                        sw.WriteLine(result);
                    }

                    fs.Close();
                    sw.Close();

                } catch(Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        internal void ReadHistoryInfo(List<Car> carsHistory)
        {
            // basePath\\oldFile
            //파일이 있는 경우에만 읽기
            //carnumber, inTime, outTime

            if (ExistBasePath()) {
                try {
                var fullname = Path.Combine(basePath, oldFile);
                
                
                    var fs = new FileStream(fullname, FileMode.Open);
                    var sr = new StreamReader(fs, Encoding.UTF8);

                    while (false == sr.EndOfStream) {
                        //line = "1111|20230603 235412|20230605 235412"
                        var line = sr.ReadLine();

                        if (string.IsNullOrEmpty(line)) {
                            continue;
                        }

                        //split lines = {"1111","20230603 235412","20230605 235412"}
                        var splitlines = line.Split(new char[] { '|' });
                        if (splitlines.Length == 3) {
                            if (int.TryParse(splitlines[0], out int carnum)) {
                                string carnumber = splitlines[0].Trim();

                                if (DateTime.TryParseExact(
                                    splitlines[1],
                                    "yyMMdd HHmmss",
                                    null,
                                    System.Globalization.DateTimeStyles.None,
                                    out DateTime intime) &&
                                    DateTime.TryParseExact(
                                    splitlines[2],
                                    "yyMMdd HHmmss",
                                    null,
                                    System.Globalization.DateTimeStyles.None,
                                    out DateTime outtime)) {
                                    carsHistory.Add(new Car(carnumber, intime, outtime));
                                }
                            }
                        }
                    }

                    fs.Close();
                    sr.Close();
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
