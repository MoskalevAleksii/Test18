// TRsMap2.h
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Robosoft.LicenseClient;
using Robosoft.GIS.Geo.GDI.Public;

namespace le10 {

//=================================================================================================
// Карта GeoModule Robosoft
//=================================================================================================
public partial class TRsMap2 : UserControl {

 #region // Типы

    // Делегат события движения мыши над картой
    public delegate void fEventMouseMove(TRsMap2 sender, MouseEventArgs e, GeoPoint p);

    // Var данные
    public class TVarData {

        public TVarData(){
            this.dCenterPoint = new GeoPoint();
            this.dCenterPoint.LatitudeDegree = (float)48.4;
            this.dCenterPoint.LongitudeDegree = (float)34.95;
            //
            this.dProection = MapProection.Merkator;
            this.dScale = 2000000;
            this.dScaleSensitivity = (float)0.3;
            this.dShowCoordinateGrid = true;
            this.dShowScaleBar = true;
            //
            this.dDetailDraw = true;
        }

        public TVarData(Byte[] bytes){
            int i = 0;
            //
            float x = (float)BitConverter.ToDouble(bytes, i); i = i + 8;
            float y = (float)BitConverter.ToDouble(bytes, i); i = i + 8;
            this.dCenterPoint = new GeoPoint();
            this.dCenterPoint.LatitudeDegree = x;
            this.dCenterPoint.LongitudeDegree = y;
            //
            this.dProection          = (MapProection)bytes[i];                 i = i + 1;
            this.dScale              = (float)BitConverter.ToDouble(bytes, i); i = i + 8;
            this.dScaleSensitivity   = (float)BitConverter.ToDouble(bytes, i); i = i + 8;
            this.dShowCoordinateGrid = BitConverter.ToBoolean(bytes, i);       i = i + 1;
            this.dShowScaleBar       = BitConverter.ToBoolean(bytes, i);       i = i + 1;
            this.dDetailDraw         = BitConverter.ToBoolean(bytes, i);
        }

        public GeoPoint     dCenterPoint;        // 16 Центральная точка
        public MapProection dProection;          // 1  Тип проекции
        public float        dScale;              // 8  Масштаб
        public float        dScaleSensitivity;   // 8  Чувствилельность к зуму (Например 0.3 = 30% от текущего масштаба)
        public bool         dShowCoordinateGrid; // 1  Управление отображением координатной сетки
        public bool         dShowScaleBar;       // 1  Управление отображением шкалы масштаба
        public bool         dDetailDraw;         // 1  Прорисовка всех деталий

        public Byte[] myGetBytes() {
            Byte[] res = new Byte[36];
            Byte[] tmp;
            int i = 0;
            //
            tmp = BitConverter.GetBytes((double)this.dCenterPoint.LatitudeDegree);  tmp.CopyTo(res, i); i = i + 8;
            tmp = BitConverter.GetBytes((double)this.dCenterPoint.LongitudeDegree); tmp.CopyTo(res, i); i = i + 8;
            //
            res[i] = (Byte)this.dProection; i = i + 1;
            //
            tmp = BitConverter.GetBytes((double)this.dScale);            tmp.CopyTo(res, i); i = i + 8;
            tmp = BitConverter.GetBytes((double)this.dScaleSensitivity); tmp.CopyTo(res, i); i = i + 8;
            tmp = BitConverter.GetBytes(this.dShowCoordinateGrid);       tmp.CopyTo(res, i); i = i + 1;
            tmp = BitConverter.GetBytes(this.dShowScaleBar);             tmp.CopyTo(res, i); i = i + 1;
            tmp = BitConverter.GetBytes(this.dDetailDraw);               tmp.CopyTo(res, i);
            //
            return res;
        }

    };

 #endregion

 #region // Конструктор, поля, свойства и события
    
    // Конструктор
    public TRsMap2(){
        InitializeComponent();
        this.pMemMap = new TRsMemMap();
        this.vOkUpMouse = false;
    }
    
    // Карта в памяти (myMemMap.myRenderMap - не используйте этот метод вне этого класса)
    public TRsMemMap pMemMap { get; private set; }

    // События движения мыши над картой
    public event TRsMap2.fEventMouseMove evMouseMove;

    // Вспомагательные поля для функции перемещения карты
    private bool  vOkUpMouse; // Флаг нажатия левой клавиши
    private Point vCurPoint;  // Текущее положение указателя мыши при нажатой левой клавиши
    private Point vPoint;     // Предыдущее положение указателя мыши при нажатой левой клавиши

 #endregion

 #region // Доступные методы

    // Прорисовка карты
    public void myRenderMap() {
        this.pMemMap.myRenderMap(this.pictureBox1);
    }

    // Прорисовка карты
    public void myRenderMap(GeoPoint gp) {
        this.pMemMap.myRenderMap(this.pictureBox1, gp);
    }

 #endregion

 #region // Var данные

    public TRsMap2.TVarData myGetVar() {
        TRsMap2.TVarData v = new TRsMap2.TVarData();
        v.dCenterPoint        = this.pMemMap.dCenterPoint;
        v.dProection          = this.pMemMap.dProection;
        v.dScale              = this.pMemMap.dScale;
        v.dScaleSensitivity   = this.pMemMap.dScaleSensitivity;
        v.dShowCoordinateGrid = this.pMemMap.dShowCoordinateGrid;
        v.dShowScaleBar       = this.pMemMap.dShowScaleBar;
        v.dDetailDraw         = this.pMemMap.dDetailDraw;
        return v;
    }

    public void mySetVar(TRsMap2.TVarData v) {
        if (v == null) return;
        this.pMemMap.dCenterPoint        = v.dCenterPoint;
        this.pMemMap.dProection          = v.dProection;
        this.pMemMap.dScale              = v.dScale;
        this.pMemMap.dScaleSensitivity   = v.dScaleSensitivity;
        this.pMemMap.dShowCoordinateGrid = v.dShowCoordinateGrid;
        this.pMemMap.dShowScaleBar       = v.dShowScaleBar;
        this.pMemMap.dDetailDraw         = v.dDetailDraw;
    }

 #endregion

 #region // Реализация событий этого компонента

    // Событие прорисовка
    private void pictureBox1_Paint(object sender, PaintEventArgs e){
        System.Drawing.Size s = this.pictureBox1.Size;
        this.pMemMap.myRenderMap(e.Graphics, s);
    }

    // Нажатие клавиши мыши
    private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            this.vOkUpMouse = true;
            this.vCurPoint = e.Location;
            this.vPoint = e.Location;
        };
    }

    // Отпускание клавиши на мыши
    private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            this.vOkUpMouse = false;
        };
    }

    // Перемещение мыши по карте
    private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
        //
        this.vCurPoint = e.Location;
        if (this.pMemMap.dStateMap != TRsMemMap.eStateMap.LOAD_OK) return;
        
        // Алгоритм, если нажата левая клавиша
        do
        {   if (e.Button != MouseButtons.Left) break;
            if (this.vOkUpMouse != true) break;
            int dx = e.Location.X - this.vPoint.X;
            int dy = e.Location.Y - this.vPoint.Y;
            this.vPoint = e.Location;
            this.pMemMap.myScroll(-dx, dy);
            this.pMemMap.myRenderMap(this.pictureBox1);
        } while (false);

        // Генерация события
        if (this.evMouseMove == null) return;
        try {
            GeoPoint p = this.pMemMap.myGetCoordinate(e.Location);
            this.evMouseMove(this, e, p);
        }
        catch {};
    }

    // Двойной шелчок мышью
    private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e){
        //
        if (this.pMemMap.dStateMap != TRsMemMap.eStateMap.LOAD_OK) return;
        GeoPoint gp = this.pMemMap.myGetCoordinate(this.vCurPoint);
        if (gp == null) return;
        this.pMemMap.dCenterPoint = gp;
        this.pMemMap.myRenderMap(this.pictureBox1);
    }

 #endregion

};

//=================================================================================================
};
