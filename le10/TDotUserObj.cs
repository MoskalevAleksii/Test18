// TDotUserObj.h
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Robosoft.GIS.Geo.GDI.Public;

namespace le10 {

//=================================================================================================
// Точечный объект пользователя
//=================================================================================================
public class TDotUserObj : UserObject, IInfoObject {

    // Конструктор
    public TDotUserObj(GeoPoint gp, Color color, string infoText) : base(gp, null, new SolidBrush(color)) {
        this.gp            = gp;
        this.brush         = Brush;
        this.selectedBrush = new SolidBrush(Color.Red);
        this.InfoText      = infoText;
    }

    // Локальные поля
    private bool     selected = false;
    private Brush    brush;
    private Brush    selectedBrush;
    private GeoPoint gp;
    internal string  InfoText;
    
    // Переопределяем метод прорисовки
    protected override void Render(Graphics gr, GeoPoint[] geoPoints, Point[] points, string language, bool detail){
        foreach (Point p in points){
            gr.FillEllipse(Brush, p.X - 5, p.Y - 5, 10, 10);
        };
    }

    // Находится в выбранном состоянии?
    public bool Selected {
        get { return selected; }
        set {
            if (selected != value) {
                if (value) { Brush = selectedBrush; }
                else { Brush = brush; };
            };
            this.selected = value;
        }
    }

    // Реализация интерфейса
    public string Info { get { return InfoText; } }

}

//=================================================================================================
};
