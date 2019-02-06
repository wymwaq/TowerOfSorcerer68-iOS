using UnityEngine;
using System.Collections.Generic;

public static class SaveHeroInfo
{

    /// <summary>
    /// 当前地图Id ( id 1 = 第 0 层 | id 2 = 第 1 层 )
    /// </summary>
    /// <value>The current map identifier.</value>
    public static int CurMapId { get; set; }


    //记录到存档用
    public static int HeroTilePosX { get; set; }
    public static int HeroTilePosY { get; set; }
    public static int heroDir { get; set; }
    /// <summary>
    /// 到达的极限楼层[id] (mapid 从0开始)
    /// </summary>
    /// <value>The reach floor identifier max.</value>
    public static Vector2Int ReachFloorIdLimit { get; set; }
    /// <summary>
    /// 已购买次数
    /// </summary>
    /// <value>The purchase count.</value>
    public static int PurchaseCount { get; set; }




    /// <summary>
    /// 商店的当前价格
    /// </summary>
    /// <value>The shop price.</value>
    public static int ShopPrice
    {

        get
        {

            int purchaseCountTemp = PurchaseCount;
            int _price = 20;
            while (purchaseCountTemp > 0)
            {

                _price = _price + (20 * PurchaseCount);
                purchaseCountTemp--;

            }
            return _price;

        }

    }


    /// <summary>
    /// 基本属性
    /// </summary>
    /// <value>The attributes.</value>
    public static List<int> Attributes { get; set; }


    /// <summary>
    /// 道具拥有情况
    /// </summary>
    /// <value>The properties.</value>
    public static List<int> Props { get; set; }




    public static void Init()
    {

        Attributes = new List<int>();
        Props = new List<int>();
        //楼层初始化（怪物、Npc、门、装备、道具）
        CurMapId = System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["mapId"][1]);//初始楼层id | = 2
        heroDir = System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["heroDir"][1]);
        ReachFloorIdLimit = new Vector2Int(2, System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["maxArriveMapId"][1]));
        PurchaseCount = System.Convert.ToInt32(Dict.GetAllDict()[Dict.SaveDBName]["z_save_in_map"]["purchaseCount"][1]);

        string _dbName = Dict.SaveDBName;
        string _tabName = "z_save_hero_info";
        Dictionary<string, Dictionary<int, object>> _cols = Dict.GetAllDict()[_dbName][_tabName];
        //属性赋值
        //int _index = 1;
        //if (Dict.GetInt(_dbName, _tabName, "saveId", 1) > 0)
        //{

        //    for (int _id = 1; _id <= _cols.Count; _id++)
        //    {

        //        foreach (string _col in _cols.Keys)
        //        {

        //            if (_index == _id)
        //            {

        //                int _value;
        //                _value = Dict.GetInt(_dbName, _tabName, _col, 1);

        //            }

        //        }

        //    }

        //}
        //else
        //{

        Attributes.Add(Dict.GetInt(_dbName, _tabName, "saveId", 1));//saveId     ==>  0
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "hp", 1));//hp             ==>  1
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "at", 1));//at             ==>  2
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "df", 1));//df             ==>  3
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "gold", 1));//gold           ==>  4
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "exp", 1));//exp            ==>  5
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "lv", 1));//lv             ==>  6
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "equip01", 1));//equip01        ==>  7
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "equip02", 1));//equip02        ==>  8
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "keyRed", 1));//keyred         ==>  9
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "keyBlue", 1));//keyblue        ==>  10
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "keyYellow", 1));//keyyellow      ==>  11
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "isProp2Advanced", 1));//isprop2advanced  ==>  12 火眼金睛是否强化 0 未强化 1 已强化
        Attributes.Add(Dict.GetInt(_dbName, _tabName, "isProp16Advanced", 1));//isprop16advanced ==>  13 筋斗云是否强化 0 未强化 1 已强化

        //}

        int _i = 0;
        GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        foreach (var text in _gm.MyUI.AttrsValue)
        {

            text.text = "";
            text.text += Attributes[_i];
            _i++;

        }

        //道具赋值
        _dbName = Dict.SqlDBName;
        _tabName = "prop";
        for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
        {

            Props.Add(Dict.GetInt(_dbName, _tabName, "value", _id));

        }
        RefreshUI();

    }


    /// <summary>
    /// 所到达的最高层的MapId刷新
    /// </summary>
    public static void RefreshUI()
    {

        GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        //楼层极限值更新
        ReachFloorIdLimit = new Vector2Int(CurMapId < ReachFloorIdLimit.x ? CurMapId : ReachFloorIdLimit.x, CurMapId > ReachFloorIdLimit.y ? CurMapId : ReachFloorIdLimit.y);
        //道具Btn
        for (int _index = 0; _index < Props.Count; _index++)
        {

            //0显示 1消失
            if (Props[_index] == 0)
            {

                _gm.MyUI.ItemBtns[_index].GetComponent<UnityEngine.UI.Image>().sprite = _gm.MyUI.ItemUIImages[_index];
                _gm.MyUI.ItemBtns[_index].GetComponent<UnityEngine.UI.Image>().raycastTarget = true;

            }
            else
            {

                _gm.MyUI.ItemBtns[_index].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("UI/ItemEmpty");
                _gm.MyUI.ItemBtns[_index].GetComponent<UnityEngine.UI.Image>().raycastTarget = false;

            }

        }

    }

}






//public int MapId { get; set; }
//public int Year { get; set; }
//public int Month { get; set; }
//public int Day { get; set; }
//public int Hour { get; set; }
//public int Minute { get; set; }
//public int Second { get; set; }

//MapId = Dict.GetInt(dbName, tabName, "mapId", 1);
//Year = Dict.GetInt(dbName, tabName, "year", 1);
//Month = Dict.GetInt(dbName, tabName, "month", 1);
//Day = Dict.GetInt(dbName, tabName, "day", 1);
//Hour = Dict.GetInt(dbName, tabName, "hour", 1);
//Minute = Dict.GetInt(dbName, tabName, "minute", 1);
//Second = Dict.GetInt(dbName, tabName, "second", 1);

//Dict.GetAllDict()[dbName][tabName]