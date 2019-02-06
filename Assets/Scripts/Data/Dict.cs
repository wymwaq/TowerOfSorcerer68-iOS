using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Dict
{

    public static int MapWidth { get; set; }
    public static int MapHeight { get; set; }
    public static int MapCount { get; set; }
    public static int SaveId { get; set; }
    public static string SqlDBName { get; set; }
    public static string SaveDBName
    {

        get
        {

            Debug.Log("SaveId：" + SaveId);
            return "SaveDb_" + SaveId;

        }

    }

    public static string WallDBName { get; set; }

    public static string SystemDBName { get; set; }

    ////(暂时用不到) 创建Map Wall DB( 墙体数据)用的  已用过 
    //public List<List<List<string>>> MapsPre { get; set; }


    /// <summary>
    /// Data字典
    /// </summary>
    static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>> _dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>>();


    /// <summary>
    /// 获得字典
    /// </summary>
    /// <returns>The get.</returns>
    public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>> GetAllDict()
    {

        return _dict;

    }


    /// <summary>
    /// 字典Dict初始化
    /// </summary>
    public static void InitDict()
    {

        List<string> _dbfilenames = StringToList(

                                                    "System," +
                                                    "Sql," +
                                                    "Wall," +
                                                    //Save 0 是数据初始化DB文件
                                                    "SaveDb_0," +
                                                    "SaveDb_1," +
                                                    "SaveDb_2," +
                                                    "SaveDb_3," +
                                                    "SaveDb_4," +
                                                    "SaveDb_5," +
                                                    "SaveDb_6," +
                                                    "SaveDb_7," +
                                                    "SaveDb_8," +
                                                    "SaveDb_9," +
                                                    "SaveDb_10"

                                                );

        foreach (string _dbfilename in _dbfilenames)
        {

            //_dict.Add(_dbfilename, DBDataBase.DictBase.DBToDict(_dbfilename));
            DBDataBase.DictBase.AddDBToDict(_dbfilename);

        }
        _dict = DBDataBase.DictBase.Dicts;

        MapWidth = GetInt(SqlDBName, "system_int", "value", 6);
        MapHeight = GetInt(SqlDBName, "system_int", "value", 7);
        MapCount = GetColCount(SqlDBName, "map");

    }


    /// <summary>
    /// 将存档数据初始化 
    /// </summary>
    /// <param name="_savedbname">Savedbname.</param>
    public static void InitSaveDict(string _savedbname)
    {

        foreach (string _tabName in _dict[_savedbname].Keys)
        {

            foreach (string _colName in _dict[_savedbname][_tabName].Keys)
            {

                foreach (int _id in _dict[_savedbname][_tabName][_colName].Keys)
                {

                    _dict[_savedbname][_tabName][_colName][_id] = _dict["SaveDb_0"][_tabName][_colName][_id];

                }

            }

        }

    }


    /// <summary>
    /// 获得Object值
    /// </summary>
    /// <returns>The object.</returns>
    /// <param name="_tab">表格名.</param>
    /// <param name="_col">字段名.</param>
    /// <param name="_id">字段Id.</param>
    public static object GetObject(string _db, string _tab, string _col, int _id)
    {

        return _dict[_db][_tab][_col][_id];

    }


    /// <summary>
    /// 获得指定表数据的条数
    /// </summary>
    /// <returns>The col count.</returns>
    /// <param name="_tab">Tab.</param>
    public static int GetColCount(string _db, string _tab)
    {

        string _col = _dict[_db][_tab].ContainsKey("id") ? "id" : "saveId";
        return _dict[_db][_tab][_col].Count;

    }


    /// <summary>
    /// 获得Int类的Object
    /// </summary>
    /// <returns>The int.</returns>
    /// <param name="_tab">Tab.</param>
    /// <param name="_col">Col.</param>
    /// <param name="_id">Identifier.</param>
    public static int GetInt(string _db, string _tab, string _col, int _id)
    {

        int _value;
        //读存档数值
        if (_col == "value")
        {

            if (_db == "Sql")
            {

                switch (_tab)
                {

                    case "map_obj":
                        _db = SaveDBName;
                        _tab = "z_save_map_obj";
                        //由于z_save_map_obj有个id0的存在 所以需要+1
                        //id++;
                        break;
                    case "auto_move":
                        _db = SaveDBName;
                        _tab = "z_save_auto_move";
                        break;
                    case "story":
                        _db = SaveDBName;
                        _tab = "z_save_story";
                        break;
                    case "prop":
                        _db = SaveDBName;
                        _tab = "z_save_prop";
                        break;
                    case "z_save_base_info":
                        _db = "System";
                        break;
                    case "z_save_hero_info":
                        _db = SaveDBName;
                        break;
                    case "map_obj_change":
                    case "system_float":
                    case "system_int":
                        break;

                }

            }
            if (_db == "Wall")
            {

                _db = SaveDBName;
                _tab = "z_save_wall_map_" + System.Text.RegularExpressions.Regex.Replace(_tab, @"[^0-9]+", "");

            }

        }

        //系统Sql
        if (_col == "is_music_on" || _col == "is_sound_on" || _col == "title_bg")
        {

            _db = "System";
            _tab = "z_save_system";

        }

        object _obj = GetObject(_db, _tab, _col, _id);
        bool _isInt = _obj is long || _obj is int;
        if (!_isInt)
        {

            Debug.Log(_obj + " 的type为非int,为 " + _obj.GetType());

        }

        _value = _isInt ? int.Parse(_obj.ToString()) : 0;

        if (_tab == "z_save_base_info")
        {

            _db = "System";

        }

        if (_col == "year" || _col == "month" || _col == "day" || _col == "hour" || _col == "minute" || _col == "second")
        {

            _db = "System";

        }

        return _value;

    }


    /// <summary>
    /// 获得Float类的Object
    /// </summary>
    /// <returns>The float.</returns>
    /// <param name="_tab">Tab.</param>
    /// <param name="_col">Col.</param>
    /// <param name="_id">Identifier.</param>
    public static float GetFloat(string _db, string _tab, string _col, int _id)
    {

        object _obj = GetObject(_db, _tab, _col, _id);
        bool _isFloat = _obj is float;
        if (!_isFloat) Debug.Log(_obj + " 的type为非float,为 " + _obj.GetType());

        return _isFloat ? float.Parse(_obj.ToString()) : 0f;

    }


    /// <summary>
    /// 获得String类的Object
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="_tab">Tab.</param>
    /// <param name="_col">Col.</param>
    /// <param name="_id">Identifier.</param>
    public static string GetString(string _db, string _tab, string _col, int _id)
    {

        object _obj = GetObject(_db, _tab, _col, _id);
        bool _isString = _obj is string;
        if (!_isString) Debug.Log(_obj + " 的type为非string,为 " + _obj.GetType());

        return _isString ? _obj.ToString() : "";

    }


    public static void SetInt(string _db, string _tab, string _col, int _id, int _value)
    {

        if (_col == "value")
        {

            if (_db == "Sql")
            {

                switch (_tab)
                {

                    case "map_obj":
                        _db = SaveDBName;
                        _tab = "z_save_map_obj";
                        break;
                    case "auto_move":
                        _db = SaveDBName;
                        _tab = "z_save_auto_move";
                        break;
                    case "story":
                        _db = SaveDBName;
                        _tab = "z_save_story";
                        break;
                    case "prop":
                        _db = SaveDBName;
                        _tab = "z_save_prop";
                        break;
                    case "z_save_base_info":
                        _db = "System";
                        break;
                    case "z_save_hero_info":
                        _db = SaveDBName;
                        break;
                    case "map_obj_change":
                    case "system_float":
                    case "system_int":
                        break;

                }

            }
            if (_db == "Wall")
            {

                _db = SaveDBName;
                _tab = "z_save_wall_map_" + SaveHeroInfo.CurMapId;

            }

        }

        //系统Sql
        if (_col == "is_music_on" || _col == "is_sound_on" || _col == "title_bg")
        {

            _db = "System";
            _tab = "z_save_system";

        }

        if (_tab == "z_save_base_info")
        {

            _db = "System";

        }

        if (_col == "year" || _col == "month" || _col == "day" || _col == "hour" || _col == "minute" || _col == "second")
        {

            _db = "System";

        }

        _dict[_db][_tab][_col][_id] = _value;
        if (_db == "System")
        {

            DBDataBase.DictBase.ZSaveSystem();
            DBDataBase.DictBase.UpdateDBToDict("System");

        }

    }


    public static List<string> StringToList(string longstring)
    {

        List<string> list = new List<string>(longstring.Split(','));
        return list;

    }


    /// <summary>
    /// 判断字符串是否为正整数
    /// </summary>
    /// <returns><c>true</c>, if integer was ised, <c>false</c> otherwise.</returns>
    /// <param name="value">Value.</param>
    public static bool IsInteger(string value)
    {

        return !Regex.IsMatch(value, "^([0-9]{1,}[.][0-9]*)$") && Regex.IsMatch(value, @"^[1-9]\d*$");

    }


    /// <summary>
    /// 初始化墙体数据到Dict
    /// </summary>
    /// <returns>The init.</returns>
    /// <param name="_mapres">Mapres.</param>
    static List<List<string>> WallsInit(string _mapres)
    {

        TextAsset textAsset = (TextAsset)Resources.Load(_mapres);//载入Map.csv，注意不要有其它格式叫Map的，或许有未知错误
        string[] map_row_string = textAsset.text.Trim().Split('\n');//清除这个Map.csv前前后后的换行，空格之类的，并按换行符分割每一行
        int map_row_max_cells = 0;//计算这个二维表中，最大列数，也就是在一行中最多有个单元格
        List<List<string>> map_Collections = new List<List<string>>();//设置一个C#容器map_Collections
        for (int i = 0; i < map_row_string.Length; i++)//读取每一行的数据
        {

            List<string> map_row = new List<string>(map_row_string[i].Split(','));//按逗号分割每个一个单元格
            if (map_row_max_cells < map_row.Count)
            {//求一行中最多有个单元格，未来要据此生成一个Plane来放Cube的

                map_row_max_cells = map_row.Count;

            }
            map_Collections.Add(map_row);//整理好，放到容器map_Collections中

        }
        return map_Collections;

    }

}