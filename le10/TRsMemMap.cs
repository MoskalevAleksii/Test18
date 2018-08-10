// TRsMemMap.h
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Robosoft.LicenseClient;
using Robosoft.GIS.Geo.GDI.Public;

namespace le10 {

//=================================================================================================
// Невизуальная карта GeoModule Robosoft
//=================================================================================================
public class TRsMemMap {

 #region // Конструктор

    public TRsMemMap(){
        this.dStateMap = TRsMemMap.eStateMap.LOAD_NO;
        this.dLanguage = TRsMemMap.eLanguage.RU;
        //
        this.pMapsModule = null;
        this.pMapView = null;
        this.pBitMap = null;
        //
        this.vCenterPoint = new GeoPoint();
        this.vCenterPoint.LatitudeDegree = (float)48.4;
        this.vCenterPoint.LongitudeDegree = (float)34.95;
        //
        this.vProection = MapProection.Merkator;
        this.vScale = 2000000;
        this.vScaleSensitivity = (float)0.3;
        this.vShowCoordinateGrid = true;
        this.vShowScaleBar = true;
        //
        this.dDetailDraw = true;
    }

 #endregion

 #region // Текущее состояние подключения к серверу лицензий

    public enum eStateSL {
        DISCON,         // Нет подключение к серверу лицензий
        CON,            // Есть подключение к серверу лицензий
        WAIT_CON,       // Ожидание подключения к серверу лицензий
        WAIT_DISCON,    // Ожидание отключения от сервера лицензий
    };

    private static TRsMemMap.eStateSL vStateSL = TRsMemMap.eStateSL.DISCON;

    public static TRsMemMap.eStateSL dStateSL { get { return TRsMemMap.vStateSL; } }

    public static String myGetStateSLText() {
        switch (TRsMemMap.vStateSL) {
            case TRsMemMap.eStateSL.DISCON:      return "Нет подключение к серверу лицензий";
            case TRsMemMap.eStateSL.CON:         return "Есть подключение к серверу лицензий";
            case TRsMemMap.eStateSL.WAIT_CON:    return "Ожидание подключения к серверу лицензий";
            case TRsMemMap.eStateSL.WAIT_DISCON: return "Ожидание отключения от сервера лицензий";
        }
        return "";
    }

    public static Color myGetStateSLColor() {
        switch (TRsMemMap.vStateSL) {
            case TRsMemMap.eStateSL.DISCON:      return Color.Red;
            case TRsMemMap.eStateSL.CON:         return Color.Green;
            case TRsMemMap.eStateSL.WAIT_CON:    return Color.Navy;
            case TRsMemMap.eStateSL.WAIT_DISCON: return Color.Navy;
        }
        return Color.Black;
    }

 #endregion

 #region // Текущее состояние карты

    public enum eStateMap {
        LOAD_NO,    // Нет загрузки карты
        LOAD_OK,    // Карта загружена
        LOAD_ER,    // Ошибка загрузки карты
    }

    public TRsMemMap.eStateMap dStateMap { get; private set; }

    public String myGetStateMapText() {
        switch (this.dStateMap) {
            case TRsMemMap.eStateMap.LOAD_NO: return "Нет загрузки карты";
            case TRsMemMap.eStateMap.LOAD_OK: return "Карта загружена";
            case TRsMemMap.eStateMap.LOAD_ER: return "Ошибка загрузки карты";
        };
        return "";
    }

    public Color myGetStateMapColor() {
        switch (this.dStateMap) {
            case TRsMemMap.eStateMap.LOAD_NO: return Color.Black;
            case TRsMemMap.eStateMap.LOAD_OK: return Color.Green;
            case TRsMemMap.eStateMap.LOAD_ER: return Color.Red;
        };
        return Color.Black;
    }

 #endregion

 #region // Язык отображения

    public enum eLanguage { RU, UA, EN, };

    public TRsMemMap.eLanguage dLanguage;

    public String myGetLanguage(){
        switch (this.dLanguage){
            case TRsMemMap.eLanguage.EN: return "en";
            case TRsMemMap.eLanguage.RU: return "ru";
            case TRsMemMap.eLanguage.UA: return "ua";
        }
        return "ru";
    }

 #endregion

 #region // Управление подключением к серверу лицензий (Статические методы и поля)

    // URL сервера лицензий
    public static String dUrlSL = "http://195.24.148.86:9064/licensing/";

    // Подключение к серверу лицензий
    public static void myConnectSL() {
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.CON) return;
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.WAIT_CON) return;
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.WAIT_DISCON) return;
        //
        if (LicenseConnection.Instance.Connected == true) {
            TRsMemMap.vStateSL = TRsMemMap.eStateSL.CON;
            return;
        };
        //
        TRsMemMap.vStateSL = TRsMemMap.eStateSL.WAIT_CON;
        String St = TRsMemMap.dUrlSL;
        System.EventHandler<ConnectionEventArgs> connectionEventHandler = new EventHandler<ConnectionEventArgs>(lChangeConnectionStatus);
        LicenseConnection.Instance.TryConnect(St, connectionEventHandler);
    }

    // Отключение от сервера лицензий
    public static void myDisconnectSL() {
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.DISCON) return;
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.WAIT_DISCON) return;
        if (TRsMemMap.vStateSL == TRsMemMap.eStateSL.WAIT_CON) return;
        //
        if (LicenseConnection.Instance.Connected == false) {
            TRsMemMap.vStateSL = TRsMemMap.eStateSL.DISCON;
            return;
        };
        //
        TRsMemMap.vStateSL = TRsMemMap.eStateSL.WAIT_DISCON;
        LicenseConnection.Instance.Disconnect();
    }

    // Реализация события подключения к серверу лицензий
    private static void lChangeConnectionStatus(object sender, ConnectionEventArgs e) {
        if (e.Connected){
            TRsMemMap.vStateSL = TRsMemMap.eStateSL.CON;
        }
        else{
            TRsMemMap.vStateSL = TRsMemMap.eStateSL.DISCON;
        };
    }

 #endregion
    
 #region // Поля класса

    private MapsModule pMapsModule; // Набор карт
    private MapView    pMapView;    // Средства для отбражения
    private Bitmap     pBitMap;     // Объект для отображения

    private GeoPoint     vCenterPoint;        // Центральная точка
    private MapProection vProection;          // Тип проекции
    private float        vScale;              // Масштаб
    private float        vScaleSensitivity;   // Чувствилельность к зуму (Например 0.3 = 30% от текущего масштаба)
    private bool         vShowCoordinateGrid; // Управление отображением координатной сетки
    private bool         vShowScaleBar;       // Управление отображением шкалы масштаба
    public  bool         dDetailDraw;         // Прорисовка всех деталий

    // Центральная точка
    public GeoPoint dCenterPoint {
        get {
            if (this.pMapView == null) return this.vCenterPoint;
            return this.pMapView.CenterPoint;
        }
        set {
            this.vCenterPoint = value;
            if (this.pMapView != null) this.pMapView.CenterPoint = value;
        }
    }

    // Тип проекции
    public MapProection dProection {
        get {
            if (this.pMapView == null) return this.vProection;
            return this.pMapView.Proection;
        }
        set {
            this.vProection = value;
            if (this.pMapView != null) this.pMapView.Proection = value;
        }
    }

    // Масштаб
    public float dScale {
        get {
            if (this.pMapView == null) return this.vScale;
            return this.pMapView.Scale;
        }
        set {
            this.vScale = value;
            if (this.pMapView != null) this.pMapView.Scale = value;
        }
    }

    // Чувствилельность к зуму (Например 0.3 = 30% от текущего масштаба)
    public float dScaleSensitivity {
        get {
            if (this.pMapView == null) return this.vScaleSensitivity;
            return this.pMapView.ScaleSensitivity;
        }
        set {
            this.vScaleSensitivity = value;
            if (this.pMapView != null) this.pMapView.ScaleSensitivity = value;
        }
    }

    // Управление отображением координатной сетки
    public bool dShowCoordinateGrid {
        get {
            if (this.pMapView == null) return this.vShowCoordinateGrid;
            return this.pMapView.ShowCoordinateGrid;
        }
        set {
            this.vShowCoordinateGrid = value;
            if (this.pMapView != null) this.pMapView.ShowCoordinateGrid = value;
        }
    }

    // Управление отображением шкалы масштаба
    public bool dShowScaleBar {
        get {
            if (this.pMapView == null) return this.vShowScaleBar;
            return this.pMapView.ShowScaleBar;
        }
        set {
            this.vShowScaleBar = value;
            if (this.pMapView != null) this.pMapView.ShowScaleBar = value;
        }
    }

 #endregion

 #region // Локальные методы

    // Загрузка списка карт
    private void lLoadMap(String[] files) {
        if (files == null) return;
        this.dStateMap = TRsMemMap.eStateMap.LOAD_ER;
        //if (this.pMapsModule == null) this.pMapsModule = new MapsModule();
        if (this.pMapsModule == null) this.pMapsModule = MapsModule.GetInstance("");
        if (this.pMapsModule.Maps.Length > 0) this.pMapsModule.UnloadAllMaps();

        //
        Map m;
        String St;
        for (int i = 0; i < files.Length; i++) {
            St = files[i];
            m = this.pMapsModule.ImportMap(St, true);
            if (m == null){
                St = "Невозможно загрузить карту " + St;
                throw new Exception(St);
            };
        };

        //
        if (this.pMapView == null){
            System.Drawing.Size s = new System.Drawing.Size(800, 600);
            this.pMapView = new MapView(this.vCenterPoint, this.vScale, s);
            this.pMapView.Proection          = this.vProection;
            this.pMapView.ScaleSensitivity   = this.vScaleSensitivity;
            this.pMapView.ShowCoordinateGrid = this.vShowCoordinateGrid;
            this.pMapView.ShowScaleBar       = this.vShowScaleBar;
        };

        //
        this.dStateMap = TRsMemMap.eStateMap.LOAD_OK;
    }

    // Прорисовка карты
    private void lRenderMap(Graphics gr, System.Drawing.Size s) {
        //
        if (this.dStateMap != TRsMemMap.eStateMap.LOAD_OK) return;

        //
        if (this.pBitMap == null)                 this.pBitMap = new Bitmap(s.Width, s.Height);
        if (this.pBitMap.Size.Width != s.Width)   this.pBitMap = new Bitmap(s.Width, s.Height);
        if (this.pBitMap.Size.Height != s.Height) this.pBitMap = new Bitmap(s.Width, s.Height);

        //
        this.pMapView.CanvasSize = this.pBitMap.Size;
        
        //
        Graphics mgr = Graphics.FromImage(this.pBitMap);
        try {
            String stLang = this.myGetLanguage();
            this.pMapView.ClearScrView(this.pMapsModule, mgr);
            this.pMapView.RenderMap(this.pMapsModule, mgr, stLang, this.dDetailDraw);
            this.pMapView.RenderUserObjects(mgr, stLang, this.dDetailDraw);
            gr.DrawImage(this.pBitMap, 0, 0, s.Width, s.Height);
        }
        finally { mgr.Dispose(); };
    }

    // Увеличение зума
    private void lZoomIn(){
        if (this.pMapView == null) return;
        this.pMapView.ZoomIn();
    }

    // Увеличение зума
    private void lZoomOut(){
        if (this.pMapView == null) return;
        this.pMapView.ZoomOut();
    }
    
    // Добавить пользовательский слой
    private bool lAddLayer(String id, String name) {
        if (this.pMapView == null) return false;
        Layer l = this.pMapView.GetLayerById(id);
        if (l != null) return false;
        l = this.pMapView.CreateLayer(id, name);
        if (l == null) return false;
        this.pMapView.AddLayer(l);
        return true;
    }

    // Удаляет пользовательский слой
    private void lRemoveLayer(String id) {
        if (this.pMapView == null) return;
        Layer l = this.pMapView.GetLayerById(id);
        if (l == null) return;
        this.pMapView.RemoveLayer(l);
    }

    // Удалить все пользовательские слои
    private void lRemoveAllLayer() {
        if (this.pMapView == null) return;
        Layer[] ls = this.pMapView.CopyLayersToArray();
        if (ls == null) return;
        for (int i = 0; i < ls.Length; i++)
            this.pMapView.RemoveLayer(ls[i]);
    }

    // Возвращает пользовательский слой
    private Layer lGetLayer(String id){
        if (this.pMapView == null) return null;
        return this.pMapView.GetLayerById(id);
    }

 #endregion

 #region // Доступные методы

    // Загрузка Карты или списка карт
    public void myLoadMap(String file) {
        String[] files = new String[1];
        files[0] = file;
        this.lLoadMap(files);
    }
    //
    public void myLoadMap(String[] files) {
        this.lLoadMap(files);
    }

    // Прорисовка карты
    public void myRenderMap(Graphics gr, System.Drawing.Size s) {
        this.lRenderMap(gr, s);
    }
    //
    public void myRenderMap(Graphics gr, System.Drawing.Size s, GeoPoint centerPoint) {
        this.dCenterPoint = centerPoint;
        this.lRenderMap(gr, s);
    }
    //
    public void myRenderMap(PictureBox pictureBox) {
        if (pictureBox == null) return;
        System.Drawing.Size s = pictureBox.Size;
        Graphics gr = pictureBox.CreateGraphics();
        try { this.lRenderMap(gr, s); }
        finally { gr.Dispose(); };
    }
    //
    public void myRenderMap(PictureBox pictureBox, GeoPoint centerPoint) {
        this.dCenterPoint = centerPoint;
        this.myRenderMap(pictureBox);
    }

    // Увеличение зума
    public void myZoomIn() {
        this.lZoomIn();
    }
    //
    public void myZoomIn(GeoPoint centerPoint) {
        this.dCenterPoint = centerPoint;
        this.lZoomIn();
    }

    // Уменьшение зума
    public void myZoomOut() {
        this.lZoomOut();
    }
    //
    public void myZoomOut(GeoPoint centerPoint) {
        this.dCenterPoint = centerPoint;
        this.lZoomOut();
    }
        
    // Добавить пользовательский слой
    public bool myAddLayer(String id, String name) {
        return this.lAddLayer(id, name);
    }

    // Удаляет пользовательский слой
    public void myRemoveLayer(String id) {
        this.lRemoveLayer(id);
    }

    // Удалить все пользовательские слои
    public void myRemoveAllLayer() {
        this.lRemoveAllLayer();
    }

    // Возвращает пользовательский слой
    public Layer myGetLayer(String id) {
        return this.lGetLayer(id);
    }

 #endregion

 #region // Трансляция методов из  this.pMapView

    // Метод для относительного изменения позиции камеры при перетаскивании пользователем карты
    public void myScroll(int canvasDx, int canvasDy) {
        if (this.pMapView == null) return;
        this.pMapView.Scroll(canvasDx, canvasDy);
    }

    // Преобразовует экранные координаты Point в гео-координаты GeoPoint
    public GeoPoint myGetCoordinate(Point canvasPoint) {
        if (this.pMapView == null) return null;
        return this.pMapView.GetCoordinate(canvasPoint);
    }

 #endregion

 #region // Трансляция методов из  this.pMapsModule

    // Поиск по адресу
    public AddressPoint[] mySearchByAddress(String country, String region, String borough, String settlement,
        String district, String block, String street, String building, Map[] maps, String language, bool caseSensitivity,
        bool exactMatching, bool fuzzySearch){

        if (this.pMapsModule == null) return null;
        return this.pMapsModule.SearchByAddress(country, region, borough, settlement, district, block, street, building, maps, language, caseSensitivity, exactMatching, fuzzySearch);
    }

 #endregion

};
 
//=================================================================================================
};
