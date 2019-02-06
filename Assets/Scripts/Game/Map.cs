using UnityEngine;

public class Map
{

    GameManager GM { get; set; }
    public GameObject Plane { get; set; }
    public GameObject Path { get; set; }
    public GameObject Wall { get; set; }
    public GameObject MapRoot { get; set; }
    /// <summary>
    /// 存放楼层独有的特殊的gameobject 如楼层表现层
    /// </summary>
    /// <value>The temp.</value>
    public GameObject Temp { get; set; }

    public void Init()
    {

        GM = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        //各种Object的Prefabs
        Plane = new GameObject();
        Plane.transform.position = new Vector3(0, 0, 1);
        Path = new GameObject();
        Path.transform.position = new Vector3(0, 0, 1);
        Wall = new GameObject();
        Wall.transform.position = new Vector3(0, 0, 1);
        if (Application.isEditor)
        {

            Plane.name = "Plane";
            Path.name = "Path";
            Wall.name = "Wall";

        }

        //创造图块路面
        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;
        for (int x = 0; x < _mapWidth; x++)
        {

            for (int y = 0; y < _mapHeight; y++)
            {

                //Gray
                //CreateBaseCube(x, y, new Color(0.2f, 0.2f, 0.2f));
                CreateBaseCube(x, y, Color.white);
            }

        }

    }


    /// <summary>
    /// 触发传送点
    /// </summary>
    public void CreateCurMapAllObjs()
    {

        ClearCurMapObj();
        Temp = new GameObject();
        Temp.transform.SetParent(Path.transform);
        if (Application.isEditor)
        {

            Temp.name = "Temp";

        }

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";


        //创建auto_move
        //auto_move表中的id
        for (int _autoMoveId = 1; _autoMoveId <= Dict.GetColCount(_dbName, "auto_move"); _autoMoveId++)
        {

            if (Dict.GetInt(_dbName, "auto_move", "map_id", _autoMoveId) == SaveHeroInfo.CurMapId)
            {

                //value 0 : 出现, 1 : 消失
                if (Dict.GetInt(_dbName, "auto_move", "value", _autoMoveId) == 0)
                {

                    Vector2Int _heroPos = new Vector2Int(Dict.GetInt(_dbName, "auto_move", "hero_pos_x", _autoMoveId), Dict.GetInt(_dbName, "auto_move", "hero_pos_y", _autoMoveId));
                    GM.MPointGrid[_heroPos.x, _heroPos.y].MAutoMoveId = _autoMoveId;

                }

            }

        }

        //添加object
        //通过id遍历map_obj
        for (int _mapObjId = 1; _mapObjId <= Dict.GetColCount(_dbName, _tabName); _mapObjId++)
        {

            //0 obj初始存在，1 obj初始不存在
            int _objValue = Dict.GetInt(_dbName, _tabName, "value", _mapObjId);
            int _switchOn = Dict.GetInt(_dbName, _tabName, "switch_on", _mapObjId);
            if (_objValue == 0 && _switchOn == 0)
            {

                //获得对应id的map_id
                int _mapId = Dict.GetInt(_dbName, _tabName, "map_id", _mapObjId);
                //获得对应map_id == 当前mapid 的数据id
                if (_mapId == SaveHeroInfo.CurMapId)
                {

                    //Debug.Log(_mapObjId + Dict.GetInt(_dbName, _tabName, "note", _mapObjId));
                    CreateMapObjById(_mapObjId);

                    //是楼梯的情况 则判断是否创建WarpPoint
                    int _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId);
                    if (_typeId == 7 || _typeId == 8 || _typeId == 18)
                    {

                        //添加warp point
                        for (int _mapWarpPointId = 1; _mapWarpPointId <= Dict.GetAllDict()[_dbName]["map_warp_point"]["id"].Count; _mapWarpPointId++)
                        {

                            if (Dict.GetInt(_dbName, "map_warp_point", "cur_map_id", _mapWarpPointId) == SaveHeroInfo.CurMapId)
                            {

                                Vector2Int _mapObjPos = new Vector2Int(Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId), Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId));
                                Vector2Int _warpPointPos = new Vector2Int(Dict.GetInt(_dbName, "map_warp_point", "cur_pos_x", _mapWarpPointId), Dict.GetInt(_dbName, "map_warp_point", "cur_pos_y", _mapWarpPointId));
                                if (_mapObjPos == _warpPointPos)
                                {

                                    //需要判断是上楼还是下楼 楼梯的类型
                                    //int _mapObjId = _dict.GetInt(dbName, tabName, "tar_map_id", id) > _dict.GetInt(dbName, tabName, "cur_map_id", id) ? 7 : 8;
                                    CreateWarpPoint(_mapWarpPointId);
                                    break;

                                }

                            }

                        }

                    }

                }

            }
            //如果SwitchOn 的 story已经激活
            else if (_objValue == 0 && _switchOn > 0 && Dict.GetInt(_dbName, "story", "value", _switchOn) == 1)
            {

                //获得对应id的map_id
                int _mapId = Dict.GetInt(_dbName, _tabName, "map_id", _mapObjId);
                //获得对应map_id == 当前mapid 的数据id
                if (_mapId == SaveHeroInfo.CurMapId)
                {

                    //Debug.Log(_mapObjId + Dict.GetInt(_dbName, _tabName, "note", _mapObjId));
                    CreateMapObjById(_mapObjId);

                    //是楼梯的情况 则判断是否创建WarpPoint
                    int _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapObjId);
                    if (_typeId == 7 || _typeId == 8 || _typeId == 18)
                    {

                        //添加warp point
                        for (int _mapWarpPointId = 1; _mapWarpPointId <= Dict.GetAllDict()[_dbName]["map_warp_point"]["id"].Count; _mapWarpPointId++)
                        {

                            if (Dict.GetInt(_dbName, "map_warp_point", "cur_map_id", _mapWarpPointId) == _mapId)
                            {

                                Vector2Int _mapObjPos = new Vector2Int(Dict.GetInt(_dbName, _tabName, "pos_x", _mapObjId), Dict.GetInt(_dbName, _tabName, "pos_y", _mapObjId));
                                Vector2Int _warpPointPos = new Vector2Int(Dict.GetInt(_dbName, "map_warp_point", "cur_pos_x", _mapWarpPointId), Dict.GetInt(_dbName, "map_warp_point", "cur_pos_y", _mapWarpPointId));
                                if (_mapObjPos == _warpPointPos)
                                {

                                    //需要判断是上楼还是下楼 楼梯的类型
                                    //int _mapObjId = _dict.GetInt(dbName, tabName, "tar_map_id", id) > _dict.GetInt(dbName, tabName, "cur_map_id", id) ? 7 : 8;
                                    CreateWarpPoint(_mapWarpPointId);
                                    break;

                                }

                            }

                        }

                    }

                }

            }

        }

        //Wall建立
        _dbName = Dict.WallDBName;
        _tabName = "map_" + SaveHeroInfo.CurMapId;
        for (int _wallId = 1; _wallId <= Dict.GetColCount(_dbName, _tabName); _wallId++)
        {

            int _value = Dict.GetInt(_dbName, _tabName, "value", _wallId);
            //0: 存在  1: 消失
            if (_value == 0)
            {

                int _x = Dict.GetInt(_dbName, _tabName, "pos_x", _wallId);
                int _y = Dict.GetInt(_dbName, _tabName, "pos_y", _wallId);
                CreateMapObj(Wall.transform, Resources.Load<Sprite>("Textures/tile_wall"), _x, _y);
                GM.MPointGrid[_x, _y].IsWall = true;
                GM.MPointGrid[_x, _y].MWallId = _wallId;
                if (Application.isEditor)
                {

                    GM.MPointGrid[_x, _y].MGameObject.name = "普通墙体";

                }

            }

        }

    }


    ////检测Story是否激活
    //bool CheckStory(int _mapobjid)
    //{

    //    for (int i = 0; i < max; i++)
    //    {

    //    }

    //    return true;

    //}


    /// <summary>
    /// 创建map obj
    /// </summary>
    /// <param name="_mapobjid">Mapobjid.</param>
    public void CreateMapObjById(int _mapobjid)
    {

        string _dbName = Dict.SqlDBName;
        string _tabName = "map_obj";
        //创建obj
        int _typeId = Dict.GetInt(_dbName, _tabName, "type_id", _mapobjid);
        int _miniTypeId;
        string _typeTabName;
        bool _isStair = false;
        Vector2Int _pos = new Vector2Int
        (

           Dict.GetInt(_dbName, _tabName, "pos_x", _mapobjid),
           Dict.GetInt(_dbName, _tabName, "pos_y", _mapobjid)

        );

        switch (_typeId)
        {

            case 13://Npc
                _typeTabName = "npc";
                _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapobjid);
                break;

            case 14://Monster
            case 71://Boss怪
                _typeTabName = "monster";
                _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapobjid);
                break;

            case 16://装饰物
                _typeTabName = "";//不知道 待确认
                _miniTypeId = Dict.GetInt(_dbName, _tabName, "mini_type_id", _mapobjid);
                break;

            case 7://上楼梯左
            case 8://下楼梯
            case 18://上楼梯右
                    //如果图格处已有物品
                if (GM.MPointGrid[_pos.x, _pos.y].IsObj)
                {

                    if (Application.isEditor)
                    {

                        Debug.Log("此图格已有物品");

                    }
                    return;

                }
                //是 传送点&显示 的情况
                for (int _warpPointId = 1; _warpPointId <= Dict.GetColCount(Dict.SqlDBName, "map_warp_point"); _warpPointId++)
                {

                    if (SaveHeroInfo.CurMapId == Dict.GetInt(Dict.SqlDBName, "map_warp_point", "cur_map_id", _warpPointId))
                    {

                        if (_pos == new Vector2Int(Dict.GetInt(Dict.SqlDBName, "map_warp_point", "cur_pos_x", _warpPointId), Dict.GetInt(Dict.SqlDBName, "map_warp_point", "cur_pos_y", _warpPointId)))
                        {

                            CreateWarpPoint(_warpPointId);

                        }

                    }

                }

                _typeTabName = "map_obj_type";
                //TypeId就是MiniTypeId
                _miniTypeId = _typeId;
                _isStair = true;
                break;

            default:
                _typeTabName = "map_obj_type";
                //TypeId就是MiniTypeId
                _miniTypeId = _typeId;
                break;

        }

        string _res = Dict.GetString(_dbName, _typeTabName, "res", _miniTypeId);
        int _width = Dict.GetInt(_dbName, "map_obj", "width", _mapobjid);
        int _height = Dict.GetInt(_dbName, "map_obj", "height", _mapobjid);
        //Debug.Log(pos.x + " | " + pos.y + " | " + _res);
        CreateObj(_pos.x, _pos.y, _res, _mapobjid, _typeId, _width, _height, _isStair);

    }


    /// <summary>
    /// 直接创建MapObj
    /// </summary>
    /// <param name="_sprite">Sprite.</param>
    /// <param name="_x">X.</param>
    /// <param name="_y">Y.</param>
    public void CreateMapObj(Transform _parent, Sprite _sprite, int _x, int _y)
    {

        GameObject _obj = new GameObject();
        SpriteRenderer _image = _obj.AddComponent<SpriteRenderer>();
        _image.sprite = _sprite;
        _obj.transform.localScale = new Vector3(1, 1, 1.5f);
        _obj.transform.position = new Vector3(_x, _y, 0);
        _obj.transform.SetParent(_parent);
        _obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
        //有null的Obj则消除
        if (GM.MPointGrid[_x, _y].MGameObject != null)
        {

            Object.Destroy(GM.MPointGrid[_x, _y].MGameObject);

        }
        GM.MPointGrid[_x, _y].MGameObject = _obj;
        GM.MPointGrid[_x, _y].MType = PointType.Obstacle;
        GM.MPointGrid[_x, _y].IsObj = true;

    }


    /// <summary>
    /// 创建传送点
    /// </summary>
    /// <param name="_warppointid">Warppointid.</param>
    public void CreateWarpPoint(int _warppointid)
    {

        string _dbname = Dict.SqlDBName;
        string _tabname = "map_warp_point";
        Vector2Int _pos = new Vector2Int
        (

            Dict.GetInt(_dbname, _tabname, "cur_pos_x", _warppointid),
            Dict.GetInt(_dbname, _tabname, "cur_pos_y", _warppointid)

        );

        if (GM.MPointGrid[_pos.x, _pos.y].MGameObject != null)
        {

            Debug.Log("已经存在MapObj : " + GM.MPointGrid[_pos.x, _pos.y].MMapObjId);
            return;
            //Object.Destroy(GM.MPointGrid[pos.x, pos.y].MGameObject);

        }

        GameObject _obj = new GameObject();
        _obj.transform.localScale = new Vector3(1, 1, 1.5f);
        _obj.transform.position = new Vector3(_pos.x, _pos.y, 0);
        _obj.transform.SetParent(Path.transform);
        GM.MPointGrid[_pos.x, _pos.y].MGameObject = _obj;
        GM.MPointGrid[_pos.x, _pos.y].MType = PointType.Obstacle;
        //GM.mPointGrid[_x, _y].MMapObjId = mapobjid;
        GM.MPointGrid[_pos.x, _pos.y].MWarpPointId = _warppointid;
        GM.MPointGrid[_pos.x, _pos.y].IsObj = true;
        if (Application.isEditor)
        {

            _obj.name = "传送点";

        }

    }


    /// <summary>
    /// Creates the object.
    /// </summary>
    /// <param name="_x">The x coordinate.</param>
    /// <param name="_y">The y coordinate.</param>
    /// <param name="_objpath">Objpath.</param>
    /// <param name="_mapobjid">Mapobjid.</param>
    /// <param name="_isStair">If set to <c>true</c> 是否是楼梯物件（较特殊，特殊处理）.</param>
    void CreateObj(int _x, int _y, string _objpath, int _mapobjid, int _typeid, int _width, int _height, bool _isStair = false)
    {

        //测试!!!!!! 去掉初始化机关门 和 怪物
        //if (Dict.GetInt("Sql", "map_obj", "type_id", mapobjid) == 42 || Dict.GetInt("Sql", "map_obj", "type_id", mapobjid) == 14)
        //{

        //    return;

        //}

        GameObject _obj = (GameObject)Object.Instantiate(Resources.Load(_objpath)) as GameObject;
        _obj.transform.localScale = new Vector3(1, 1, 1.5f);
        _obj.transform.position = new Vector3(_x, _y, 0);

        if (!_isStair)
        {

            _obj.transform.SetParent(Path.transform);

            //层级关系
            if (_obj.GetComponent<SpriteRenderer>())
            {

                switch (_typeid)
                {

                    case 13://npc
                    case 14://怪物
                        _obj.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        break;

                    default:
                        _obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        break;

                }

            }

            //obj的大小尺寸设置
            for (int _w = 0; _w < _width; _w++)
            {

                for (int _h = 0; _h < _height; _h++)
                {

                    if (GM.MPointGrid[_x + _w, _y - _h].MGameObject != null)
                    {

                        Object.Destroy(GM.MPointGrid[_x, _y].MGameObject);

                    }

                    GM.MPointGrid[_x + _w, _y - _h].MGameObject = _obj;
                    GM.MPointGrid[_x + _w, _y - _h].MType = PointType.Obstacle;
                    GM.MPointGrid[_x + _w, _y - _h].MMapObjId = _mapobjid;
                    GM.MPointGrid[_x + _w, _y - _h].IsObj = true;

                    //如果宽高都为1  则不执行下去
                    if (_width == 1 && _height == 1)
                    {

                        return;

                    }

                    //width或者height大于1的物件
                    if (GM.MPointGrid[_x + _w, _y - _h].MConnectPoint == null)
                    {

                        GM.MPointGrid[_x + _w, _y - _h].MConnectPoint = new System.Collections.Generic.List<AStarPoint>();

                    }

                    //赋予关联格子点
                    GM.MPointGrid[_x, _y].MConnectPoint.Add(GM.MPointGrid[_x + _w, _y - _h]);

                    //关联格子点的相互关联
                    foreach (AStarPoint _p in GM.MPointGrid[_x, _y].MConnectPoint)
                    {

                        _p.MConnectPoint = GM.MPointGrid[_x, _y].MConnectPoint;

                    }

                }

            }

        }
        else
        {

            _obj.transform.SetParent(Temp.transform);
            if (Application.isEditor)
            {

                _obj.name = "楼梯表现层";

            }

        }

        //GM.mPointGrid[x, y].MMapId = mapid;
        //GM.mPointGrid[x, y].MTypeId = typeid;
        //GM.mPointGrid[x, y].MMiniTypeId = minitypeid;

    }


    /// <summary>
    /// 清除当前地图上所有物件信息
    /// </summary>
    void ClearCurMapObj()
    {

        int _mapWidth = Dict.MapWidth;
        int _mapHeight = Dict.MapHeight;
        for (int _x = 0; _x < _mapWidth; _x++)
        {

            for (int _y = 0; _y < _mapHeight; _y++)
            {

                GM.MPointGrid[_x, _y].Clear();

            }

        }

        //清除Temp中的gameobject
        Object.Destroy(Temp);

    }


    void CreateBaseCube(int _x, int _y, Color _color)
    {

        GameObject _obj = (GameObject)Object.Instantiate(Resources.Load("P_Ground")) as GameObject;
        _obj.AddComponent<BoxCollider2D>();
        //_obj.AddComponent<SpriteRenderer>();
        _obj.transform.SetParent(Plane.transform);
        _obj.transform.localPosition = new Vector3(_x, _y, 0);
        //go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        //go.GetComponent<MeshRenderer>().materials[0].SetColor("_ShaderColor", color);
        _obj.GetComponent<SpriteRenderer>().color = _color;
        Cube _objCube = _obj.AddComponent<Cube>();
        if (Application.isEditor)
        {

            _objCube.name = "路面";

        }
        _objCube.FindPath = GM.FindPath;
        GM.MBaseCubeList.Add(new Vector2(_x, _y), _objCube);

    }

}