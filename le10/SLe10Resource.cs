// SLe10Resource.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace le10 {
namespace Res {

using System.Resources;
using System.Reflection;
using System.Drawing;

//=================================================================================================
// Доступ к ресурсам этой сборки
//=================================================================================================
public class SLe10Resource {

    #region // Перечисление и имена ресурсов Icon

        public const String ICO_FLAG_RED     = "flag_9254";             // Флаг красный
        public const String ICO_FLAG_RED1    = "flag_7604";             // Флаг красный
        public const String ICO_FLAG_GREEN   = "flag1rightgreen2_6498"; // Флаг зелёный
        public const String ICO_FLAG_BLACK   = "flag1leftblack2_4211";  // Флаг чёрный
        public const String ICO_FLAG_BLUE    = "flag3rightblue2_4268";  // Флаг синий
        public const String ICO_FLAG_JAPAN   = "kiten_8065";            // Флаг японский
        public const String ICO_FLAG_CHESS   = "checkered_flag_1192";   // Флаг мелкие бело-чёрные квадраты
        public const String ICO_FLAG_MEDICAL = "flag_9254_1";           // Флаг медицинский

        public const String ICO_CAR_COMPACT_BLUE   = "pic_car_compact_blue";
        public const String ICO_CAR_COMPACT_GREEN  = "pic_car_compact_green";
        public const String ICO_CAR_COMPACT_GREY   = "pic_car_compact_grey";
        public const String ICO_CAR_COMPACT_ORANGE = "pic_car_compact_orange";
        public const String ICO_CAR_SEDAN_ORANGE   = "pic_car_sedan_orange";
        public const String ICO_CAR_MINIBUS_BLUE   = "pic_minibus_blue";
        public const String ICO_CAR_MINIBUS_GREEN  = "pic_minibus_green";
        public const String ICO_CAR_MINIBUS_GREY   = "pic_minibus_grey";
        public const String ICO_CAR_MINIBUS_ORANGE = "pic_minibus_orange";
        public const String ICO_CAR_MINIBUS_WHITE  = "pic_minibus_white";
    
        public enum eResIcon
        {
            FLAG_RED,     // Флаг красный
            FLAG_RED1,    // Флаг красный
            FLAG_GREEN,   // Флаг зелёный
            FLAG_BLACK,   // Флаг чёрный
            FLAG_BLUE,    // Флаг синий
            FLAG_JAPAN,   // Флаг японский
            FLAG_CHESS,   // Флаг мелкие бело-чёрные квадраты
            FLAG_MEDICAL, // Флаг медицинский
            CAR_COMPACT_BLUE,
            CAR_COMPACT_GREEN,
            CAR_COMPACT_GREY,
            CAR_COMPACT_ORANGE,
            CAR_SEDAN_ORANGE,
            CAR_MINIBUS_BLUE,
            CAR_MINIBUS_GREEN,
            CAR_MINIBUS_GREY,
            CAR_MINIBUS_ORANGE,
            CAR_MINIBUS_WHITE,
        }

    #endregion

    // Менеджер ресурсов
	private
    static ResourceManager pRes = new ResourceManager("le10.le10", Assembly.GetExecutingAssembly());

    // Возвращает ресурс Icon
	public static Icon myGetIcon(SLe10Resource.eResIcon e){
        String St = "";
        switch (e) {
            //
            case SLe10Resource.eResIcon.FLAG_RED:     St = SLe10Resource.ICO_FLAG_RED;     break;
            case SLe10Resource.eResIcon.FLAG_RED1:    St = SLe10Resource.ICO_FLAG_RED1;    break;
            case SLe10Resource.eResIcon.FLAG_GREEN:   St = SLe10Resource.ICO_FLAG_GREEN;   break;
            case SLe10Resource.eResIcon.FLAG_BLACK:   St = SLe10Resource.ICO_FLAG_BLACK;   break;
            case SLe10Resource.eResIcon.FLAG_BLUE:    St = SLe10Resource.ICO_FLAG_BLUE;    break;
            case SLe10Resource.eResIcon.FLAG_JAPAN:   St = SLe10Resource.ICO_FLAG_JAPAN;   break;
            case SLe10Resource.eResIcon.FLAG_CHESS:   St = SLe10Resource.ICO_FLAG_CHESS;   break;
            case SLe10Resource.eResIcon.FLAG_MEDICAL: St = SLe10Resource.ICO_FLAG_MEDICAL; break;
            //
            case SLe10Resource.eResIcon.CAR_COMPACT_BLUE:   St = SLe10Resource.ICO_CAR_COMPACT_BLUE;   break;
            case SLe10Resource.eResIcon.CAR_COMPACT_GREEN:  St = SLe10Resource.ICO_CAR_COMPACT_GREEN;  break;
            case SLe10Resource.eResIcon.CAR_COMPACT_GREY:   St = SLe10Resource.ICO_CAR_COMPACT_GREY;   break;
            case SLe10Resource.eResIcon.CAR_COMPACT_ORANGE: St = SLe10Resource.ICO_CAR_COMPACT_ORANGE; break;
            case SLe10Resource.eResIcon.CAR_SEDAN_ORANGE:   St = SLe10Resource.ICO_CAR_SEDAN_ORANGE;   break;
            case SLe10Resource.eResIcon.CAR_MINIBUS_BLUE:   St = SLe10Resource.ICO_CAR_MINIBUS_BLUE;   break;
            case SLe10Resource.eResIcon.CAR_MINIBUS_GREEN:  St = SLe10Resource.ICO_CAR_MINIBUS_GREEN;  break;
            case SLe10Resource.eResIcon.CAR_MINIBUS_GREY:   St = SLe10Resource.ICO_CAR_MINIBUS_GREY;   break;
            case SLe10Resource.eResIcon.CAR_MINIBUS_ORANGE: St = SLe10Resource.ICO_CAR_MINIBUS_ORANGE; break;
            case SLe10Resource.eResIcon.CAR_MINIBUS_WHITE:  St = SLe10Resource.ICO_CAR_MINIBUS_WHITE;  break;
        };
        if (St == "") return null;
        //
        return (Icon)SLe10Resource.pRes.GetObject(St);
    }

}

//=================================================================================================
};
};
