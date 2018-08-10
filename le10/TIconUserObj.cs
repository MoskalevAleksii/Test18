// TIconUserObj.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Robosoft.GIS.Geo.GDI.Public;

namespace le10 {

//=================================================================================================
// Обьект пользователя в виде иконки
//=================================================================================================
public class TIconUserObj : TEditUserObj {

 #region // Типы

    public enum DrawPosition {
        Center,
        LeftBottom,
    }

 #endregion

 #region // Конструктор

    public TIconUserObj(Icon icon, DrawPosition drawPosition, MapView mapView)
        : base(new GeoPoint[] { null }, null, null, mapView, "", TEditUserObj.eType.Point) {
        //
        _iconToDraw = icon;
        _drawPosition = drawPosition;
    }

 #endregion

 #region // Поля, свойства и методы

    protected Icon _iconToDraw;
    DrawPosition   _drawPosition;

    // Устанавливает/возвращает гео-позицию
    public GeoPoint Position {
        get {
            if (base.pts != null && pts.Length > 0) { return base.pts[0]; }
            else { return null; };
        }
        set {
            ApplyPoint(value);
            this.pts = new GeoPoint[] { value };
            this.Limits = GeoRect.FromPoints(pts);
        }
    }

    // Устанавливает положение на экране
    public Point PointCanvas {
        set {
            if (MapView != null) {
                List<GeoPoint> geoPts = new List<GeoPoint>();
                switch (_drawPosition) {
                    case DrawPosition.Center:
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X - _iconToDraw.Width / 2, value.Y - _iconToDraw.Height / 2)));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width / 2, value.Y - _iconToDraw.Height / 2)));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width / 2, value.Y + _iconToDraw.Height / 2)));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X - _iconToDraw.Width / 2, value.Y + _iconToDraw.Height / 2)));
                        break;
                    case DrawPosition.LeftBottom:
                        geoPts.Add(MapView.GetCoordinate(value));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width, value.Y)));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width, value.Y - _iconToDraw.Height)));
                        geoPts.Add(MapView.GetCoordinate(new Point(value.X, value.Y - _iconToDraw.Height)));
                        break;
                    default:
                        break;
                }
                ApplyPoints(geoPts, true);
                this.pts = geoPts.ToArray();
                this.Limits = GeoRect.FromPoints(pts);
            }
        }
    }

    private Point UpdatePosition(Point value) {
        if (MapView != null) {
            List<GeoPoint> geoPts = new List<GeoPoint>();
            switch (_drawPosition) {
                case DrawPosition.Center:
                    if (pts[0] != null) { geoPts.Add(pts[0]); };
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X - _iconToDraw.Width / 2, value.Y - _iconToDraw.Height / 2)));
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width / 2, value.Y - _iconToDraw.Height / 2)));
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width / 2, value.Y + _iconToDraw.Height / 2)));
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X - _iconToDraw.Width / 2, value.Y + _iconToDraw.Height / 2)));
                    break;
                case DrawPosition.LeftBottom:
                    geoPts.Add(pts[0]);
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width, value.Y)));
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X + _iconToDraw.Width, value.Y - _iconToDraw.Height)));
                    geoPts.Add(MapView.GetCoordinate(new Point(value.X, value.Y - _iconToDraw.Height)));
                    break;
                default:
                    break;
            }
            ApplyPoints(geoPts, true);
            this.pts = geoPts.ToArray();
            this.Limits = GeoRect.FromPoints(pts);
        };
        return value;
    }

    protected override void Render(Graphics gr, GeoPoint[] geoPoints, Point[] points, string language, bool detail) {
        Point pt = points[0];
        switch (_drawPosition) {
            case DrawPosition.Center:
                {
                bool prevVal = OptRefreshOnChangeObject;
                this.OptRefreshOnChangeObject = false;
                UpdatePosition(new Point(points[0].X + _iconToDraw.Width / 2, points[0].Y + _iconToDraw.Height / 2));
                this.OptRefreshOnChangeObject = prevVal;
                }
                break;
            case DrawPosition.LeftBottom:
                pt = new Point(points[0].X, points[0].Y - _iconToDraw.Height);
                {
                bool prevVal = OptRefreshOnChangeObject;
                this.OptRefreshOnChangeObject = false;
                UpdatePosition(points[0]);
                this.OptRefreshOnChangeObject = prevVal;
                }
                break;
            default:
                break;
        };
        gr.DrawIconUnstretched(_iconToDraw, new Rectangle(pt, _iconToDraw.Size));
    }

    public override void Select(int pointIndex, float k){
        base.Select(0, 0);
    }

 #endregion

}

//=================================================================================================
};
