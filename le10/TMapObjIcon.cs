// TMapObjIcon.h
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Robosoft.GIS.Geo.GDI.Public;
using le10;
using le10.Res;

namespace le10 {
namespace MapObj {

//=================================================================================================
// Объект отображения в виде иконки с текстом
//=================================================================================================
public class TMapObjIcon : UserObject {

    #region  // Типы

        // Данные инициализации
        public struct sIniData {
            public TRsMemMap              pMemMap;    // Указатель на карту
            public GeoPoint               pGeoPoint;  // Географическое положение
            public SLe10Resource.eResIcon pCodeIcon;  // Отображаемая Icon
            //
            public String pText;        // Надпись
            public System.Drawing.Brush pTextFtBrush; // Кисть для шрифта надписи
            public System.Drawing.Pen   pTextPen;     // Карандаш для оконтовки надписи
            public System.Drawing.Brush pTextBrush;   // Кисть для фона надписи
            //
            public String pSelText;        // Надпись
            public System.Drawing.Brush pSelTextFtBrush; // Кисть для шрифта надписи
            public System.Drawing.Pen   pSelTextPen;     // Карандаш для оконтовки надписи
            public System.Drawing.Brush pSelTextBrush;   // Кисть для фона надписи
        };

    #endregion

    #region // Создание объекта, поля и свойства

        protected
        TMapObjIcon(GeoPoint gp, System.Drawing.Pen pen, System.Drawing.Brush brush) : base(gp, pen, brush)
        {
            //
            this.vData.pMemMap = null;
            this.vData.pGeoPoint = null;
            this.vData.pCodeIcon = SLe10Resource.eResIcon.FLAG_RED;
            //
            this.vData.pText = null;
            this.vData.pTextFtBrush = null;
            this.vData.pTextPen = null;
            this.vData.pTextBrush = null;
            //
            this.vData.pSelText = null;
            this.vData.pSelTextFtBrush = null;
            this.vData.pSelTextPen = null;
            this.vData.pSelTextBrush = null;
            //
            this.pIcon = null;
            this.dSelected = false;
        }

        public
        static TMapObjIcon myCreate(TMapObjIcon.sIniData d)
        {
            TMapObjIcon ob = new TMapObjIcon(d.pGeoPoint, d.pTextPen, d.pTextBrush);
            ob.vData = d;
            ob.pIcon = SLe10Resource.myGetIcon(d.pCodeIcon);
            return ob;
        }

        public
        static TMapObjIcon myCreate(GeoPoint p, SLe10Resource.eResIcon icon, String text, TRsMemMap map)
        {
            TMapObjIcon.sIniData d = new TMapObjIcon.sIniData();
            //
            d.pMemMap = map;
            d.pGeoPoint = p;
            d.pCodeIcon = icon;
            //
            d.pText = text;
            d.pTextFtBrush = new SolidBrush(Color.Black);
            d.pTextPen = new System.Drawing.Pen(Color.Black);
            d.pTextBrush = null;
            //
            d.pSelText = text;
            d.pTextFtBrush = new SolidBrush(Color.Red);
            d.pTextPen = new System.Drawing.Pen(Color.Red);
            d.pTextBrush = null;
            //
            return TMapObjIcon.myCreate(d);
        }

        private TMapObjIcon.sIniData vData; // Данные инициализации
        public Icon pIcon;                  // Отображаемая Icon
        public bool dSelected;              // Выделенный / Невыделенный объект

    #endregion

    #region // Методы прорисовки

        // Вывод текста
        void lDrawText(String St, Graphics gr, Point p, System.Drawing.Font ft, System.Drawing.Brush brushFt, System.Drawing.Pen pen, System.Drawing.Brush brush) {
            // St       - Надпись
            // gr       - Графическая область
            // p        - Позиция надписи
            // ft       - Шрифт надписи
            // brushFt  - Цвет шрифта надписи
            // pen      - Цвет оконтовки надписи
            // brush    - Цвет заливки надписи

            // Начальная проверка
            if (St == null) return;
            if (St == "") return;
            if (gr == null) return;
            if (ft == null) return;
            if (brushFt == null) return;
            
            // Определяем оконтовку и заливку квадрата вывода
            if ((pen != null) && (brush != null)) {
                SizeF stSize = gr.MeasureString(St, ft);
                if (pen != null) gr.DrawRectangle(pen, 0.0F, 0.0F, stSize.Width, stSize.Height);
                if (brush != null) gr.FillRectangle(brush, 0.0F, 0.0F, stSize.Width, stSize.Height);
            };

            // Выводим текст
            gr.DrawString(St, ft, brushFt, p);
        }

        // Прорисовка объекта
        protected override void Render(Graphics gr, GeoPoint[] geoPoints, Point[] points, string language, bool detail) {
            // Точка отображения
            Point p = points[0];

            // Определяем размер иконки и шрифта
            System.Drawing.Size sizeIco = new System.Drawing.Size(50, 50);
            Single sizeFt = 8;
            if (this.vData.pMemMap != null) {
                //float d = this.vData.pMemMap.dScale;
                //if (d < 5000)                   { sizeIco = new System.Drawing.Size(75, 75); sizeFt = 10; };
                //if ((d >= 5000) && (d < 15000)) { sizeIco = new System.Drawing.Size(50, 50); sizeFt = 8;  };
                //if (d >= 15000)                 { sizeIco = new System.Drawing.Size(25, 25); sizeFt = 6;  };
                //
                sizeIco = new System.Drawing.Size(50, 50); sizeFt = 8;
            };

            // Отображение Icon
            if (this.pIcon != null)
                gr.DrawIcon(this.pIcon, new Rectangle(p, sizeIco));

            // Отображение текста
            String St = this.vData.pText;
            System.Drawing.Brush ftBrush = this.vData.pTextFtBrush;
            System.Drawing.Pen pen = this.vData.pTextPen;
            System.Drawing.Brush brush = this.vData.pTextBrush;
            if (this.dSelected == true) {
                St = this.vData.pSelText;
                ftBrush = this.vData.pSelTextFtBrush;
                pen = this.vData.pSelTextPen;
                brush = this.vData.pSelTextBrush;
            };
            System.Drawing.Font ft = new System.Drawing.Font("Microsoft Sans Serif", sizeFt, FontStyle.Regular, GraphicsUnit.Point, (byte)204);
            //
            p = new Point(p.X, p.Y + sizeIco.Height);
            this.lDrawText(St, gr, p, ft, ftBrush, pen, brush);
        }

    #endregion

};

//=================================================================================================
};
};
