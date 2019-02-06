using System.Data;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;

namespace DBDataBase
{

    /// <summary>
    /// 此类为静态
    /// </summary>
    public static class DictBase
    {

        //根字典
        public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>> Dicts = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>>();

        /// <summary>
        /// DB数据存储到字典中
        /// </summary>
        /// <returns>The to dic.</returns>
        /// <param name="_dbname">DB文件的name，不加后缀</param>
        public static void AddDBToDict(string _dbname)
        {

            Dicts.Add(_dbname, new Dictionary<string, Dictionary<string, Dictionary<int, object>>>());

            string _path = GetPath(_dbname);

            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");

            //创建新db链接
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    //获取所有Table
                    DataTable _tables = _conn.GetSchema("Tables");
                    foreach (DataRow _row in _tables.Rows)
                    {

                        //每个Table的名称
                        string _tableName = (string)_row[2];
                        //获得每个Table中所有Column
                        _comm.CommandText = "PRAGMA table_info(" + _tableName + ")";
                        //Table名附加到Key
                        Dicts[_dbname].Add(_tableName, new Dictionary<string, Dictionary<int, object>>());

                        using (SqliteDataReader _reader = _comm.ExecuteReader(CommandBehavior.KeyInfo))
                        {

                            //遍历对应Table的全部Column
                            while (_reader.Read())
                            {

                                //从这里开始就出错
                                //每个Column的名称
                                string _columnName = _reader.GetString(1);
                                Dicts[_dbname][_tableName].Add(_columnName, new Dictionary<int, object>());

                            }

                        }

                    }

                }
                //UnityEngine.Debug.Log(dict["Sql"]["auto_move"]["hero_pos_y"]);

                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    foreach (string _tableName in Dicts[_dbname].Keys)
                    {

                        foreach (string _columnName in Dicts[_dbname][_tableName].Keys)
                        {

                            //默认序号
                            int _id = 1;
                            _comm.CommandText = "SELECT " + _columnName + " FROM " + _tableName;
                            using (SqliteDataReader _reader = _comm.ExecuteReader(CommandBehavior.KeyInfo))
                            {

                                if (_reader.HasRows)
                                {

                                    while (_reader.Read())
                                    {

                                        //UnityEngine.Debug.Log(table_name + "/" + column_name + "/" + reader.GetValue(0).ToString());
                                        Dicts[_dbname][_tableName][_columnName].Add(_id, _reader.GetValue(0));
                                        _id++;

                                    }

                                }

                            }

                        }

                    }

                }
                _conn.Close();
                //Debug.Log(dict["Sql"]["auto_move"]["map_id"][3].GetType());
                //Debug.Log(dict["SaveDb_01"]["z_save_in_map"]["mapId"]);
                //UnityEngine.Debug.Log(dict["Wall"]);
                Debug.Log("***************************Sqlite全部加载完毕***************************");

            }

        }


        //更新Db属性到Dict 主要用于保存之后更新dict的数据
        public static void UpdateDBToDict(string _dbname)
        {

            Dict.GetAllDict().Remove(_dbname);
            AddDBToDict(_dbname);

        }


        /// <summary>
        /// 将所有数据保存到指定Db
        /// </summary>
        /// <param name="_saveid">Saveid.</param>
        public static void SaveToSaveDb(int _saveid)
        {

            Debug.Log("***************************Sqlite全部初始化完毕***************************");
            ZSaveAutoMove(_saveid);
            ZSaveHeroInfo(_saveid);
            ZSaveInMap(_saveid);
            ZSaveMapObj(_saveid);
            ZSaveProp(_saveid);
            ZSaveStory(_saveid);
            ZSaveAllWallMaps(_saveid);
            ZSaveBaseInfo();

        }


        /// <summary>
        /// 初始化指定SaveDb 将指定SaveDb中的数值初始化至新游戏(非保存至db中) 初始化模版为SaveDb_0
        /// </summary>
        public static void InitSaveDb(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _path = GetPath(_dbName);

            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");

            //创建新db链接
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    foreach (string _tabName in Dict.GetAllDict()[_dbName].Keys)
                    {

                        for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                        {

                            foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                            {

                                if (_colName == "id" || _colName == "saveId")
                                {

                                    continue;

                                }

                                _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + Dict.GetAllDict()["SaveDb_0"][_tabName][_colName][_id] + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }

                    }

                }
                _conn.Close();
                //Debug.Log(dict["Sql"]["auto_move"]["map_id"][3].GetType());
                //Debug.Log(dict["SaveDb_01"]["z_save_in_map"]["mapId"]);
                //UnityEngine.Debug.Log(dict["Wall"]);
                Debug.Log("***************************Sqlite全部初始化完毕***************************");

            }

        }


        public static void PosYConvert(string _dbname)
        {

            string _path = GetPath(_dbname);

            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");

            //创建新db链接
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    foreach (string _tabName in Dict.GetAllDict()[_dbname].Keys)
                    {

                        for (int _id = 1; _id <= Dict.GetColCount(_dbname, _tabName); _id++)
                        {

                            foreach (string _colName in Dict.GetAllDict()[_dbname][_tabName].Keys)
                            {

                                switch (_colName)
                                {

                                    case "hero_pos_y":
                                    case "start_pos_y":
                                    case "end_pos_y":
                                    case "high_pos_y":
                                    case "pos_y":
                                    case "cur_pos_y":
                                    case "tar_pos_y":
                                    case "heroTilePosY":
                                        _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + (11 - System.Convert.ToInt32(Dict.GetAllDict()[_dbname][_tabName][_colName][_id]) - 1) + " WHERE " + "id" + " = " + _id;
                                        _comm.ExecuteNonQuery();
                                        break;


                                }

                            }

                        }

                    }

                }
                _conn.Close();
                //Debug.Log(dict["Sql"]["auto_move"]["map_id"][3].GetType());
                //Debug.Log(dict["SaveDb_01"]["z_save_in_map"]["mapId"]);
                //UnityEngine.Debug.Log(dict["Wall"]);
                Debug.Log("***************************Sqlite全部初始化完毕***************************");

            }

        }



        /// <summary>
        /// 创建SQLite文件 string包括路径与不包括后缀
        /// </summary>
        /// <param name="_sqlname">Sqlname.</param>
        public static void CreateDataBase(string _sqlname)
        {

            if (!System.IO.File.Exists(_sqlname + ".db"))
            {

                SqliteConnection.CreateFile(_sqlname + ".db");

            }

        }


        /// <summary>
        /// SaveDb中的z_save_auto_move
        /// </summary>
        static void ZSaveAutoMove(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_auto_move";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }
                            if (_colName == "value")
                            {

                                _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_hero_info
        /// </summary>
        static void ZSaveHeroInfo(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_hero_info";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }

                            int _newValue = 0;
                            switch (_colName)
                            {

                                case "hp":
                                    _newValue = SaveHeroInfo.Attributes[1];
                                    break;
                                case "at":
                                    _newValue = SaveHeroInfo.Attributes[2];
                                    break;
                                case "df":
                                    _newValue = SaveHeroInfo.Attributes[3];
                                    break;
                                case "gold":
                                    _newValue = SaveHeroInfo.Attributes[4];
                                    break;
                                case "exp":
                                    _newValue = SaveHeroInfo.Attributes[5];
                                    break;
                                case "lv":
                                    _newValue = SaveHeroInfo.Attributes[6];
                                    break;
                                case "equip01":
                                    _newValue = SaveHeroInfo.Attributes[7];
                                    break;
                                case "equip02":
                                    _newValue = SaveHeroInfo.Attributes[8];
                                    break;
                                case "keyRed":
                                    _newValue = SaveHeroInfo.Attributes[9];
                                    break;
                                case "keyBlue":
                                    _newValue = SaveHeroInfo.Attributes[10];
                                    break;
                                case "keyYellow":
                                    _newValue = SaveHeroInfo.Attributes[11];
                                    break;
                                case "isProp2Advanced":
                                    _newValue = SaveHeroInfo.Attributes[12];
                                    break;
                                case "isProp16Advanced":
                                    _newValue = SaveHeroInfo.Attributes[13];
                                    break;

                            }
                            _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + _newValue + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_in_map
        /// </summary>
        static void ZSaveInMap(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_in_map";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }

                            int _newValue = 0;
                            switch (_colName)
                            {

                                case "mapId":
                                    _newValue = SaveHeroInfo.CurMapId;
                                    break;
                                case "heroTilePosX":
                                    _newValue = SaveHeroInfo.HeroTilePosX;
                                    break;
                                case "heroTilePosY":
                                    _newValue = SaveHeroInfo.HeroTilePosY;
                                    break;
                                case "heroDir":
                                    _newValue = SaveHeroInfo.heroDir;
                                    break;
                                case "maxArriveMapId":
                                    _newValue = SaveHeroInfo.ReachFloorIdLimit.y;
                                    break;
                                case "purchaseCount":
                                    _newValue = SaveHeroInfo.PurchaseCount;
                                    break;

                            }
                            _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + _newValue + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_map_obj
        /// </summary>
        static void ZSaveMapObj(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_map_obj";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }

                            if (_colName == "value")
                            {

                                _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_prop
        /// </summary>
        static void ZSaveProp(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_prop";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }

                            if (_colName == "value")
                            {

                                _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + SaveHeroInfo.Props[_id - 1] + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_story
        /// </summary>
        static void ZSaveStory(int _saveid)
        {

            string _dbName = "SaveDb_" + _saveid;
            string _tabName = "z_save_story";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }

                            if (_colName == "value")
                            {

                                _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// SaveDb中的z_save_wall_map集合
        /// </summary>
        static void ZSaveAllWallMaps(int _saveid)
        {

            for (int _mapId = 1; _mapId <= Dict.GetColCount(Dict.SqlDBName, "map"); _mapId++)
            {

                string _dbName = "SaveDb_" + _saveid;
                string _tabName = "z_save_wall_map_" + _mapId;
                string _path = GetPath(_dbName);
                //文件不存在
                if (!System.IO.File.Exists(_path))
                {

                    Debug.Log("********************" + _path + "不存在" + "********************");
                    return;

                }
                Debug.Log("DB文件存在");
                using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
                {

                    _conn.Open();
                    //创建新命令
                    using (SqliteCommand _comm = _conn.CreateCommand())
                    {

                        for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                        {

                            foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                            {

                                if (_colName == "id" || _colName == "saveId")
                                {

                                    continue;

                                }

                                if (_colName == "value")
                                {

                                    _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                                    _comm.ExecuteNonQuery();

                                }

                            }

                        }

                    }
                    _conn.Close();

                }

            }

        }


        /// <summary>
        /// System中的z_save_system
        /// </summary>
        public static void ZSaveSystem()
        {

            string _dbName = Dict.SystemDBName;
            string _tabName = "z_save_system";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }
                            _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }

                    }

                }
                _conn.Close();

            }

        }


        /// <summary>
        /// System中的z_save_base_info
        /// </summary>
        public static void ZSaveBaseInfo()
        {

            string _dbName = Dict.SystemDBName;
            string _tabName = "z_save_base_info";
            string _path = GetPath(_dbName);
            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");
            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                    {

                        foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                        {

                            if (_colName == "id" || _colName == "saveId")
                            {

                                continue;

                            }
                            _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + System.Convert.ToInt32(Dict.GetAllDict()[_dbName][_tabName][_colName][_id]) + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }

                    }

                }
                _conn.Close();

            }

        }


        static string GetPath(string _dbname)
        {

            //string _path = Application.streamingAssetsPath + "/" + _dbname + ".db";
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {

                Debug.Log("*************************** Platform Is Editor *******************************");
                return Application.dataPath + "/StreamingAssets/" + _dbname + ".db";

            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                Debug.Log("*************************** Platform Is iPhone *******************************");
                return Application.dataPath + "/Raw/" + _dbname + ".db";

            }
            if (Application.platform == RuntimePlatform.Android)
            {

                Debug.Log("*************************** Platform Is Android *******************************");
                return "jar:file://" + Application.dataPath + "!/assets/" + _dbname + ".db";

            }
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {

                Debug.Log("*************************** Platform Is PC && OSX *******************************");
                return Application.dataPath + "/StreamingAssets/" + _dbname + ".db";

            }
            return null;

        }


        /// <summary>
        /// 不用 初始化Db中的SaveId
        /// </summary>
        static void InitSaveId()
        {

            for (int _saveId = 0; _saveId < 10; _saveId++)
            {

                string _dbName = "SaveDb_" + _saveId;
                string _path = GetPath(_dbName);
                //文件不存在
                if (!System.IO.File.Exists(_path))
                {

                    return;

                }
                using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
                {

                    _conn.Open();
                    //创建新命令
                    using (SqliteCommand _comm = _conn.CreateCommand())
                    {

                        foreach (string _tabName in Dict.GetAllDict()[_dbName].Keys)
                        {

                            foreach (string _colName in Dict.GetAllDict()[_dbName][_tabName].Keys)
                            {

                                for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabName); _id++)
                                {

                                    if (_colName == "saveId")
                                    {

                                        _comm.CommandText = "UPDATE " + _tabName + " SET " + _colName + " = " + _saveId + " WHERE " + "id" + " = " + _id;
                                        _comm.ExecuteNonQuery();

                                    }

                                }

                            }

                        }

                    }
                    _conn.Close();

                }


            }

        }


    }

}




/*
/// <summary>
/// 将Dict存储到DB文件
/// </summary>
/// <param name="_saveId">saveId.</param>
public static void SaveGameDataToDB(int _saveId)
{

    GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    string _sqlDbName = Dict.SqlDBName;
    string _saveDbName = Dict.SaveDBName;
    string _path = GetPath(_sqlDbName);
    //文件不存在
    if (!System.IO.File.Exists(_path))
    {

        Debug.Log("********************" + _path + "不存在" + "********************");
        return;

    }
    Debug.Log("DB文件存在");

    using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
    {

        _conn.Open();
        //创建新命令
        using (SqliteCommand _comm = _conn.CreateCommand())
        {

            foreach (string _tabNameKey in Dict.GetAllDict()[_saveDbName].Keys)
            {

                switch (_tabNameKey)
                {

                    case "z_save_auto_move":
                        for (int _id = 1; _id <= Dict.GetColCount(_saveDbName, _tabNameKey); _id++)
                        {

                            _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + Dict.GetAllDict()[_saveDbName][_tabNameKey]["value"][_id] + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }
                        break;

                    case "z_save_hero_info":
                        foreach (string _colName in Dict.GetAllDict()[_saveDbName][_tabNameKey].Keys)
                        {

                            switch (_colName)
                            {

                                case "hp":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[1] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "at":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[2] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "df":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[3] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "gold":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[4] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "exp":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[5] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "lv":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[6] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "equip01":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[7] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "equip02":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[8] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "keyRed":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[9] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "keyBlue":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[10] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "keyYellow":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[11] + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "isProp16Advanced":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.IsProp16Advaced + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "isProp2Advanced":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.IsProp2Advaced + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;

                            }

                        }
                        break;

                    case "z_save_in_map":
                        foreach (string _colName in Dict.GetAllDict()[_saveDbName][_tabNameKey].Keys)
                        {

                            switch (_colName)
                            {

                                case "mapId"://当前mapId
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.CurMapId + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;//当前posX
                                case "heroTilePosX":
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + _gm.MStartPos.MPositionX + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "heroTilePosY"://当前posY
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + _gm.MStartPos.MPositionY + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "heroDir"://当前人物方向
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + _gm.MyPlayer.MAnimator.GetInteger("direction") + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;
                                case "maxArriveMapId"://到达最大MapId
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.ReachFloorIdLimit.y + " WHERE " + "id" + " = " + 1;
                                    _comm.ExecuteNonQuery();
                                    break;

                            }

                        }
                        break;

                    case "z_save_map_obj":
                        for (int _id = 1; _id <= Dict.GetColCount(_saveDbName, _tabNameKey); _id++)
                        {

                            _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + Dict.GetAllDict()[_saveDbName][_tabNameKey]["value"][_id] + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }
                        break;

                    case "z_save_prop":
                        for (int _id = 1; _id <= Dict.GetColCount(_saveDbName, _tabNameKey); _id++)
                        {

                            _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + Dict.GetAllDict()[_saveDbName][_tabNameKey]["value"][_id] + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }
                        break;
                    case "z_save_story":
                        for (int _id = 1; _id <= Dict.GetColCount(_saveDbName, _tabNameKey); _id++)
                        {

                            //Dict.GetAllDict()[_saveDbName][_tabNameKey]["value"][_id]
                            _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + Dict.GetInt(_sqlDbName, "story", "value", _id) + " WHERE " + "id" + " = " + _id;
                            _comm.ExecuteNonQuery();

                        }
                        break;
                    //Wall
                    default:
                        for (int _mapId = 1; _mapId <= Dict.GetColCount(Dict.SqlDBName, "map"); _mapId++)
                        {

                            string _wallTabName = "z_save_wall_map" + _mapId;
                            for (int _id = 1; _id <= Dict.GetColCount(_saveDbName, _wallTabName); _id++)
                            {

                                _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + Dict.GetAllDict()[_saveDbName][_tabNameKey]["value"][_id] + " WHERE " + "id" + " = " + _id;
                                _comm.ExecuteNonQuery();

                            }

                        }
                        break;

                }

            }

        }
        _conn.Close();

    }

}



/// <summary>
/// 将Dict存储到DB文件
/// </summary>
public static void SaveSystemDataToDB()
{

    GameManager _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    string _systemDbName = Dict.SystemDBName;
    string _path = GetPath(_systemDbName);
    //文件不存在
    if (!System.IO.File.Exists(_path))
    {

        Debug.Log("********************" + _path + "不存在" + "********************");
        return;

    }
    Debug.Log("DB文件存在");

    using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
    {

        _conn.Open();
        //创建新命令
        using (SqliteCommand _comm = _conn.CreateCommand())
        {

            foreach (string _tabNameKey in Dict.GetAllDict()[_systemDbName].Keys)
            {

                switch (_tabNameKey)
                {

                    case "z_save_base_info":
                        foreach (string _colName in Dict.GetAllDict()[_systemDbName][_tabNameKey].Keys)
                        {

                            for (int _id = 1; _id <= Dict.GetColCount(_systemDbName, _tabNameKey); _id++)
                            {

                                switch (_colName)
                                {

                                    case "at":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[2] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "df":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[3] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "hp":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.Attributes[1] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "mapId":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + SaveHeroInfo.CurMapId + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "value":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["value"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "year":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["year"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "month":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["month"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "day":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["day"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "hour":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["hour"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "minute":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["minute"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;
                                    case "second":
                                        _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetAllDict()[_systemDbName][_tabNameKey]["second"][_id] + " WHERE " + "id" + " = " + _id;
                                        //_comm.ExecuteNonQuery();
                                        break;

                                }

                            }

                        }
                        break;
                    case "z_save_system":
                        foreach (string _colName in Dict.GetAllDict()[_systemDbName][_tabNameKey].Keys)
                        {

                            _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + _colName + " = " + Dict.GetInt(_systemDbName, _tabNameKey, _colName, 1) + " WHERE " + "id" + " = " + 1;
                            _comm.ExecuteNonQuery();

                        }
                        break;

                }

            }

        }
        _conn.Close();

    }

}
*/


//foreach (string _tabNameKey in Dict.GetAllDict()[_dbName].Keys)
//{

//    if (_tabNameKey =="auto_move"|| _tabNameKey == "auto_move")
//    {

//        for (int _id = 1; _id <= Dict.GetColCount(_dbName, _tabNameKey); _id++)
//        {


//            //_comm.CommandText = "UPDATE " + "表名称TabName" + " SET " + "列名称ColName" + " = " + "新值" + " WHERE " + "列名称" + " = " + "某值";
//            _comm.CommandText = "UPDATE " + _tabName + " SET " + _colNameKey + " = " + Dict["Sql"]["map_obj"]["value"][_id] + " WHERE " + "id" + " = " + _id;
//            _comm.ExecuteNonQuery();

//        }

//    }

//}







/*
        /// <summary>
        /// 初始化存档 把Sql里的初始值全部赋给指定存档
        /// </summary>
        /// <param name="Dict">Dict.</param>
        /// <param name="_dbname">Dbname.</param>
        public static void SaveDBDefault(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>> Dict, string _dbname)
        {

            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _dbname + ".db"))
            {

                _conn.Open();

                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    foreach (string _tabNameKey in Dict[_dbname].Keys)
                    {

                        //Sql中的名称
                        string _sqlTabName = "";
                        switch (_tabNameKey)
                        {

                            case "z_save_story":
                                _sqlTabName = "story";
                                for (int _id = 1; _id <= Dict["Sql"][_sqlTabName]["id"].Count; _id++)
                                {

                                    //_comm.CommandText = "UPDATE " + "表名称TabName" + " SET " + "列名称ColName" + " = " + "新值" + " WHERE " + "列名称" + " = " + "某值";
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + 0 + " WHERE " + "id" + " = " + _id;
                                    _comm.ExecuteNonQuery();

                                }
                                break;
                            case "z_save_map_obj":
                                _sqlTabName = "map_obj";
                                for (int _id = 1; _id <= Dict["Sql"][_sqlTabName]["id"].Count; _id++)
                                {

                                    //_comm.CommandText = "UPDATE " + "表名称TabName" + " SET " + "列名称ColName" + " = " + "新值" + " WHERE " + "列名称" + " = " + "某值";
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + 0 + " WHERE " + "id" + " = " + _id;
                                    _comm.ExecuteNonQuery();

                                }
                                break;
                            case "z_save_auto_move":
                                _sqlTabName = "auto_move";
                                for (int _id = 1; _id <= Dict["Sql"][_sqlTabName]["id"].Count; _id++)
                                {

                                    //_comm.CommandText = "UPDATE " + "表名称TabName" + " SET " + "列名称ColName" + " = " + "新值" + " WHERE " + "列名称" + " = " + "某值";
                                    _comm.CommandText = "UPDATE " + _tabNameKey + " SET " + "value" + " = " + 0 + " WHERE " + "id" + " = " + _id;
                                    _comm.ExecuteNonQuery();

                                }
                                break;
                            case "z_save_hero_info":
                                break;
                            case "z_save_prop":
                                break;
                            case "z_save_in_map":
                                break;

                        }

                    }

                }

                _conn.Close();

            }

        }


        /// <summary>
        /// 将Dict存储到DB文件
        /// </summary>
        /// <param name="Dict">Dict.</param>
        /// <param name="_dbname">DB文件名字 不需要后缀.</param>
        /// <param name="_tabname">Tabname.</param>
        public static void SaveDictToDB(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<int, object>>>> Dict, string _dbname, string _tabname)
        {

            string _path = Application.streamingAssetsPath + "/" + _dbname + ".db";
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {

                Debug.Log("*************************** Platform Is Editor *******************************");
                _path = Application.dataPath + "/StreamingAssets/" + _dbname + ".db";

            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                Debug.Log("*************************** Platform Is iPhone *******************************");
                _path = Application.dataPath + "/Raw/" + _dbname + ".db";

            }
            if (Application.platform == RuntimePlatform.Android)
            {

                Debug.Log("*************************** Platform Is Android *******************************");
                _path = "jar:file://" + Application.dataPath + "!/assets/" + _dbname + ".db";

            }

            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
            {

                Debug.Log("*************************** Platform Is PC && OSX *******************************");
                _path = Application.dataPath + "/StreamingAssets/" + _dbname + ".db";

            }


            //文件不存在
            if (!System.IO.File.Exists(_path))
            {

                Debug.Log("********************" + _path + "不存在" + "********************");
                return;

            }
            Debug.Log("DB文件存在");

            using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _path))
            {

                _conn.Open();
                //创建新命令
                using (SqliteCommand _comm = _conn.CreateCommand())
                {

                    foreach (string _tabNameKey in Dict[_dbname].Keys)
                    {

                        if (_tabname == _tabNameKey)
                        {

                            foreach (string _colNameKey in Dict[_dbname][_tabname].Keys)
                            {

                                if (_tabname != "system" && _tabname != "z_save_base_info")
                                {

                                    if (_colNameKey == "value")
                                    {

                                        for (int _id = 1; _id <= Dict["Sql"]["map_obj"]["id"].Count; _id++)
                                        {

                                            //_comm.CommandText = "UPDATE " + "表名称TabName" + " SET " + "列名称ColName" + " = " + "新值" + " WHERE " + "列名称" + " = " + "某值";
                                            _comm.CommandText = "UPDATE " + _tabname + " SET " + _colNameKey + " = " + Dict["Sql"]["map_obj"]["value"][_id] + " WHERE " + "id" + " = " + _id;
                                            _comm.ExecuteNonQuery();

                                        }

                                    }

                                }

                            }

                        }

                    }


                }
                _conn.Close();

            }

        }
            */















///// <summary>
///// 创建Save文件中的墙体储存表
///// </summary>
//public static void CreateSaveDbWall()
//{

//    //创建新db链接
//    using (SqliteConnection _conn = new SqliteConnection("URI=file:SaveDb_1.db"))
//    {

//        _conn.Open();
//        GameManager _gm = UnityEngine.GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
//        //创建新命令
//        //创建DataBase
//        for (int _mapId = 0; _mapId < _gm.GetDict().MapsPre.Count; _mapId++)
//        {

//            using (SqliteCommand _comm = _conn.CreateCommand())
//            {

//                _comm.CommandText = "CREATE TABLE z_save_wall_map_" + (_mapId + 1) + " (id INTENGER , pos_x INTENGER , pos_y INTENGER , value INTERGER)";
//                _comm.ExecuteNonQuery();

//            }

//        }

//        using (SqliteCommand _comm = _conn.CreateCommand())
//        {

//            for (int _mapId = 0; _mapId < _gm.GetDict().MapsPre.Count; _mapId++)
//            {

//                int _mapGridId = 1;
//                for (int _y = 0; _y < _gm.GetDict().MapsPre[_mapId].Count; _y++)
//                {

//                    for (int _x = 0; _x < _gm.GetDict().MapsPre[_mapId][_y].Count; _x++)
//                    {

//                        //UnityEngine.Debug.Log("MapId: " + ( _mapId + 1 ) + " | x: " + _x + " | y: " + _y + " | value: " + _gm.GetDict().Maps[_mapId][_y][_x]);
//                        _comm.CommandText = "INSERT INTO z_save_wall_map_" + (_mapId + 1) + " (id, pos_x, pos_y, value) VALUES (" + _mapGridId + "," + _x + ", " + _y + ", " + _gm.GetDict().MapsPre[_mapId][_y][_x] + ")";
//                        _comm.ExecuteNonQuery();
//                        _mapGridId++;

//                    }

//                }

//            }

//        }

//        _conn.Close();

//    }

//}


///// <summary>
///// 创建Table string包括路径与不包括后缀
///// </summary>
///// <param name="_sqlname">Sqlname.</param>
//public static void CreateMapTables(string _sqlname)
//{

//    //创建新db链接
//    using (SqliteConnection _conn = new SqliteConnection("URI=file:" + _sqlname + ".db"))
//    {

//        _conn.Open();
//        GameManager _gm = UnityEngine.GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
//        //创建新命令
//        //创建DataBase
//        for (int _mapId = 0; _mapId < _gm.GetDict().MapsPre.Count; _mapId++)
//        {

//            using (SqliteCommand _comm = _conn.CreateCommand())
//            {

//                _comm.CommandText = "CREATE TABLE map_" + (_mapId + 1) + " (id INTENGER , pos_x INTENGER , pos_y INTENGER , value INTERGER)";
//                _comm.ExecuteNonQuery();

//            }

//        }

//        using (SqliteCommand _comm = _conn.CreateCommand())
//        {

//            for (int _mapId = 0; _mapId < _gm.GetDict().MapsPre.Count; _mapId++)
//            {

//                int _mapGridId = 1;
//                for (int _y = 0; _y < _gm.GetDict().MapsPre[_mapId].Count; _y++)
//                {

//                    for (int _x = 0; _x < _gm.GetDict().MapsPre[_mapId][_y].Count; _x++)
//                    {

//                        //UnityEngine.Debug.Log("MapId: " + ( _mapId + 1 ) + " | x: " + _x + " | y: " + _y + " | value: " + _gm.GetDict().Maps[_mapId][_y][_x]);
//                        _comm.CommandText = "INSERT INTO map_" + (_mapId + 1) + " (id, pos_x, pos_y, value) VALUES (" + _mapGridId + "," + _x + ", " + _y + ", " + _gm.GetDict().MapsPre[_mapId][_y][_x] + ")";
//                        _comm.ExecuteNonQuery();
//                        _mapGridId++;

//                    }

//                }

//            }

//        }

//        _conn.Close();

//    }

//}