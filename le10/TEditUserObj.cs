// TEditUserObj.h

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Robosoft.GIS.Geo.GDI.Public;

namespace le10 {

//=================================================================================================
// Редактируемый объект пользователя
//=================================================================================================
public class TEditUserObj : UserObject, IInfoObject {

 #region // Типы
    
    public enum eType {
        Polyline = 1,
        Polygon  = 2,
        Point    = 3,
    }

    public delegate void ChangedEventHandler(object sender, EventArgs args);

 #endregion

 #region // Конструктор

    public TEditUserObj(GeoPoint[] pts, Pen pen, Brush brush, MapView mapView, string infoText, TEditUserObj.eType geometryType)
        : base(pts, geometryType == TEditUserObj.eType.Polygon, pen, brush){
       
        //
        this.pts = pts;
        this.limits = GeoRect.FromPoints(pts);

        //
        if (pen != null)
        {
            // обычное перо
            this.unselected_pen = pen;

            // перо для выделения
            this.selected_pen = (pen.Clone() as Pen);
            this.selected_pen.Width = pen.Width + Math.Max((float)(pen.Width / 2.0), 1);
        }

        // вид на карту запоминаем для возможности смещения точек
        this.mapView = mapView;

        //
        this.geometryType = geometryType;
        this.InfoText = infoText;
    }

 #endregion

 #region // Поля и свойства

    // Локальные поля
    private   Pen        unselected_pen; // перо для невыделенного объекта
    private   Pen        selected_pen;   // перо для выделенного объекта
    protected GeoPoint[] pts;
    private   MapView    mapView;
    //
    private TEditUserObj.eType geometryType = TEditUserObj.eType.Polygon;
    //
    private int selectedPointIndex = -1; // если объект выделен у него задана точка // по умолчанию - ни одна точка не выделена
    //
    private GeoRect limits;

    // Доступные поля
    public bool EnableEdit = true;
    
    // Доступные свойства
    public MapView MapView {
        get { return mapView; }
        set { mapView = value; }
    }

    public GeoRect Limits {
        get { return limits; }
        protected set { limits = value; }
    }

    public event ChangedEventHandler Changed;

    public bool Selected { get { return selectedPointIndex >= 0; } }

 #endregion

 #region // Методы

    public virtual void Select(int pointIndex, float k) {
        if (k < 0.5) {
            // ближе к мыши первая точка отрезка на котором находится мышь
            this.selectedPointIndex = pointIndex;
        }
        else {
            // ближе к мыши последняя точка отрезка на котором находится мышь
            this.selectedPointIndex = pointIndex + 1;
        };

        // изменяем перо на выделенное
        this.Pen = selected_pen;
    }

    public void UnSelect(){
        this.selectedPointIndex = -1;
        this.Pen = unselected_pen;
    }

    protected override void Render(Graphics gr, GeoPoint[] geoPoints, Point[] points, string language, bool detail){
        // базовая прорисовка
        base.Render(gr, geoPoints, points, language, detail);

        // помечаем выделенную точку если она есть
        if (selectedPointIndex >= 0) {
            Point p = points[selectedPointIndex];
            gr.DrawRectangle(unselected_pen, p.X - 5, p.Y - 5, 10, 10);
        };
    }

    // Смещаем выделенную точку
    public void MovePoint(int dx, int dy) {
        if (!EnableEdit) return;

        int ind = selectedPointIndex;
        if (ind >= 0) {
            // объект выделен

            // т.к. объект замкнут, то возможно что выделен замыкающий отрезок, 
            // а его последняя точка - это первая точка базового набора
            if (ind >= pts.Length) { ind = 0; };

            // сдвигаем точку
            mapView.MoveCoordinate(pts[ind], dx, dy);

            // переиндексируем объект на карте
            ApplyPoints(pts, geometryType == TEditUserObj.eType.Polygon);

            // переиндексируем лимиты
            limits = GeoRect.FromPoints(pts);

            if (Changed != null) { Changed(this, EventArgs.Empty); };
        };
    }

 #endregion

 #region // Реализация интефейса IInfoObject

    internal string InfoText;

    public string Info { get { return InfoText; } }

 #endregion

}

//=================================================================================================
};
